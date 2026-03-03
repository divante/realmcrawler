# RealmCrawler — Combat System Implementation Plan

## Architecture Overview

The combat system builds on the existing data-driven FSM (`StateDefinition` / `StateRuntime` / `ActionRuntime`).
Combat is driven by a combo graph on weapons, a cast-type system on spells, a shared `CombatContext` for mid-combo spell bonuses, and a `VeilSystem` that governs spell power and instability.

---

## Data Changes

### WeaponData — Combo Tree
Replace `primaryCantrip` / `secondaryCantrip` with a tree of `ComboNode`s.

```
ComboNode
  CantripData  hit          — what this strike does
  ComboNode    lightNext    — follow-up on light press (null = chain ends)
  ComboNode    heavyNext    — follow-up on heavy press (null = can't branch here)

WeaponData
  ComboNode  lightComboRoot
  ComboNode  heavyComboRoot
```

Supports arbitrary branching: light→light→light, light→light→heavy, heavy→light, etc.

### SpellData — Cast Types & Mutations
Add cast type and per-type parameters:

```
SpellCastType: Instant | Charged | Channeled

Charged params:   float maxChargeTime, float chargeMultiplier
Channeled params: float channelFireRate, float maxChannelDuration

List<SpellMutation> possibleMutations
```

`SpellMutation` is a ScriptableObject describing one variant behaviour per spell
(e.g. "Split into 3", "Explode on cast", "Reverse direction").
`VeilSystem` selects one at high instability.

---

## New Components

### CombatLoadout *(MonoBehaviour)*
Single source of truth for what the player has equipped mid-run.
```
WeaponData    equippedWeapon
SpellData[4]  equippedSpells
```
Fed by `EquipmentManager` on loadout change. Queried by action runtimes.

### CombatContext *(MonoBehaviour)*
Lightweight state shared between `ComboAttackAction` and `SpellCastAction`.
```
bool  IsInCombo
int   ComboHitsLanded
```
`ComboAttackAction` writes to it. `SpellCastAction` reads it to apply mid-combo bonus.

### VeilSystem *(MonoBehaviour)*
Tracks veil integrity and drives spell instability.
```
float veilIntegrity          — 1.0 (intact) → 0.0 (broken)

void  ConsumeVeil(float)     — called by SpellCastAction on every cast
float SpellPowerMultiplier   — increases as veil breaks
bool  RollMisfire()          — probability ramps at low integrity
SpellMutation SelectMutation(SpellData) — returns mutation or null

event OnThresholdCrossed(float threshold)
event OnVeilBroken()
```
Other systems (invasion spawner, difficulty scaler) subscribe to the events.

---

## Input Expansion

### ICharacterController
Add:
```
event Action  AttackLightEvent
event Action  AttackHeavyEvent
event Action<int>  SpellCastPressedEvent(slot)
event Action<int>  SpellCastReleasedEvent(slot)   — needed for Charged / Channeled
```

### PlayerController
Wire new Input System actions: `OnAttackLight`, `OnAttackHeavy`, `OnSpell1`–`OnSpell4`
(both `started` and `canceled` phases for spell hold detection).

---

## Conditions

Following the `IsMovingCondition` pattern (subscribe on Activate, unsubscribe on Deactivate):

| Condition | Fires when |
|-----------|-----------|
| `AttackLightPressedCondition` | `AttackLightEvent` received |
| `AttackHeavyPressedCondition` | `AttackHeavyEvent` received |
| `SpellCastPressedCondition` | `SpellCastPressedEvent(slot)` — inspectable `int slot` field |
| `AnimationFinishedCondition` | Animation event callback via a bridge component |
| `TookHitCondition` | Damage received event from health/hit system |

---

## Action Runtimes

### ComboAttackAction *(ActionRuntime subclass)*
Fields: `AttackType startOn` (Light / Heavy)

Behaviour:
1. On `Activate` — start at `lightComboRoot` or `heavyComboRoot` from `CombatLoadout`
2. Fire current node's `CantripData` (spawn projectile, trigger animator)
3. Write to `CombatContext` (`IsInCombo = true`, increment `ComboHitsLanded`)
4. Animation event opens combo window — buffer `light` or `heavy` input
5. On window close:
   - Buffered input + valid next node → advance, repeat from step 2
   - No input or null next node → fire `ActionCompleted`, clear `CombatContext`
6. On `Deactivate` — clear `CombatContext`

### SpellCastAction *(ActionRuntime subclass)*
Fields: `int slot`

Behaviour:
1. On `Activate` — read `CombatLoadout.equippedSpells[slot]`
2. Apply mid-combo bonus if `CombatContext.IsInCombo`
3. Query `VeilSystem`:
   - Multiply damage by `SpellPowerMultiplier`
   - `RollMisfire()` → alter fire direction if true
   - `SelectMutation()` → apply mutation to spawned spell if non-null
4. Call `VeilSystem.ConsumeVeil(spell.veilCost)`
5. Branch on `SpellCastType`:
   - **Instant** — spawn immediately, trigger animator, wait for `AnimationFinishedCondition`
   - **Charged** — start charge phase on Activate; on `SpellCastReleasedEvent` scale by charge ratio, spawn, exit
   - **Channeled** — fire at `channelFireRate` each tick; on `SpellCastReleasedEvent` or max duration, exit
6. Fire `ActionCompleted` when done

---

## Animation Integration

Extend `CharacterAnimationController`:
- Add trigger hashes: `LightAttack`, `HeavyAttack`, `SpellCast`, `Stagger`
- Expose `TriggerCombat(int hash)` for action runtimes to call

Add `AnimationEventBridge` MonoBehaviour on the character:
- Animation clips fire events that call `OnAnimationFinished()` / `OnComboWindowOpen()` / `OnComboWindowClose()`
- `AnimationFinishedCondition` subscribes to `OnAnimationFinished`
- `ComboAttackAction` subscribes to `OnComboWindowOpen` / `OnComboWindowClose`

---

## State Graph

### States
- `Idle` — existing
- `Move` — existing
- `LightAttack` — entry point for light combo root
- `HeavyAttack` — entry point for heavy combo root
- `SpellCast1` – `SpellCast4` — one per slot
- `Stagger` — interrupt state

### Transitions
```
Idle/Move     → LightAttack     AttackLightPressedCondition
Idle/Move     → HeavyAttack     AttackHeavyPressedCondition
Idle/Move     → SpellCast[N]    SpellCastPressedCondition(N)

LightAttack   → Idle            ActionCompleted (combo ended)
LightAttack   → HeavyAttack     AttackHeavyPressedCondition (cancel into heavy)
HeavyAttack   → Idle            ActionCompleted

SpellCast[N]  → Idle            AnimationFinishedCondition
SpellCast[N]  → Stagger         TookHitCondition

Stagger       → Idle            AnimationFinishedCondition

Any           → Stagger         TookHitCondition  (add to all relevant states)
```

---

## Implementation Order

| Step | Task | Depends on |
|------|------|-----------|
| 1 | Expand `ICharacterController` + `PlayerController` with combat input | — |
| 2 | Revise `WeaponData` to combo tree (`ComboNode`) | — |
| 3 | Add `SpellCastType`, charge/channel params, `possibleMutations` to `SpellData` | — |
| 4 | `CombatLoadout` component | — |
| 5 | `CombatContext` component | 4 |
| 6 | `VeilSystem` component | 3 |
| 7 | Combat conditions | 1 |
| 8 | `AnimationEventBridge` + extend `CharacterAnimationController` | — |
| 9 | `ComboAttackAction` runtime | 2, 4, 5, 8 |
| 10 | `SpellCastAction` runtime | 3, 4, 5, 6 |
| 11 | Wire state graph assets in editor | all above |
