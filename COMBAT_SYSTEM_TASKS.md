# RealmCrawler - Action Combat System

## Overview

Complete action combat system for RealmCrawler supporting:
- Weapon-based combat (fast/heavy attacks per weapon)
- 4 customizable spell slots
- Action phases (windup → execute → recovery)
- Keyboard+Mouse AND Controller input
- Integration with existing state machine

---

## Architecture Summary

```
ActionDefinition (ScriptableObject)
├── WeaponAttackDefinition (Fast/Heavy patterns)
└── SpellDefinition (4 customizable slots)
        │
        ▼
ActionRuntime (Base Execution)
├── WeaponAttackRuntime (Phases, Hitboxes)
└── SpellRuntime (Cooldowns, Projectiles)
```

### Key Components

| System | Components |
|--------|-----------|
| **Input** | InputActionAsset, CombatInputHandler |
| **Weapons** | WeaponDefinition, AttackPattern, WeaponManager, HitboxManager |
| **Spells** | SpellDefinition, SpellRuntime, SpellManager, ProjectileSpawner |
| **Actions** | ActionDefinition, ActionRuntime, WeaponAttackRuntime, SpellRuntime |
| **State** | CombatState, ActionTransition conditions |
| **Feedback** | DamageNumberSpawner, CombatVFXSpawner, CombatAudioManager |

---

## Phase 1: Core Infrastructure

### [TASK-001] Create InputActionAsset for Combat
  What: Define all combat input actions (KB+Mouse + Controller)
  Why: Foundation for all player input
  Done when: Input actions tested in Unity, both KB+Mouse and controller work
  Depends on: none

  **Input Mapping:**
  ```
  Keyboard/Mouse:
    - FastAttack → Mouse0 (Left Click)
    - HeavyAttack → Mouse1 (Right Click)
    - Spell1 → Q
    - Spell2 → E
    - Spell3 → R
    - Spell4 → F
    - SwitchWeapon → 1-4 (or Tab)

  Controller:
    - FastAttack → Cross/A
    - HeavyAttack → Circle/B
    - Spell1 → L1/LB
    - Spell2 → R1/RB
    - Spell3 → L2/LT
    - Spell4 → R2/RT
    - SwitchWeapon → D-Pad Up/Down
  ```

### [TASK-002] Create CombatInputHandler component
  What: Subscribe to input actions, expose events for attacks/spells/weapon switch
  Why: Centralized input handling
  Done when: Events fire correctly on input, input priority implemented
  Depends on: [TASK-001]

  **Events to expose:**
  - `OnAttackRequested(AttackType)`
  - `OnSpellRequested(int slotIndex)`
  - `OnWeaponSwitchRequested(int weaponIndex)`
  - `CheckInputPriority()` - Don't interrupt recovery
  - `QueueInput()` - If action in progress

### [TASK-003] Create ActionDefinition base ScriptableObject
  What: Base action data with name, type, and runtime reference
  Why: Foundation for all action types
  Done when: Can create ActionDefinition asset in Unity
  Depends on: none

  **Fields:**
  ```csharp
  public string actionName;
  public ActionRuntime actionRuntime; // SerializeReference
  public ActionRuntime Runtime(GameObject owner);
  ```

### [TASK-004] Create ActionRuntime base class
  What: Base action execution with phase system (Windup/Execute/Recovery)
  Why: All actions share lifecycle
  Done when: ActionRuntime can transition through phases
  Depends on: [TASK-003]

  **Phase System:**
  ```csharp
  enum ActionPhase { Idle, Windup, Execute, Recovery, Completed }
  
  public ActionPhase CurrentPhase;
  public float PhaseTimer;
  public bool IsCancelable;
  public bool IsInterruptible;
  
  public void StartAction();
  public void CancelAction();
  public void CompleteAction();
  
  public event Action OnPhaseChanged;
  public event Action OnActionCompleted;
  public event Action OnActionCancelled;
  ```

---

## Phase 2: Weapon System

### [TASK-005] Create WeaponDefinition ScriptableObject
  What: Weapon data with fast/heavy attack patterns
  Why: Store weapon-specific combat data
  Done when: Can create weapon asset with attack patterns
  Depends on: [TASK-003]

  **Fields:**
  ```csharp
  public string weaponName;
  public WeaponType weaponType; // Melee/Ranged
  public AttackPattern fastAttack;
  public AttackPattern heavyAttack;
  public AnimationClip fastAttackClip;
  public AnimationClip heavyAttackClip;
  ```

### [TASK-006] Create AttackPattern data class
  What: Timing, damage, hitbox data for attacks
  Why: Reusable attack configuration
  Done when: AttackPattern serializes correctly
  Depends on: [TASK-005]

  **Fields:**
  ```csharp
  public float windupTime;
  public float executeTime;
  public float recoveryTime;
  public float damage;
  public float hitboxRange;
  public float hitboxAngle;
  public bool canChain; // Can combo into next attack
  ```

### [TASK-007] Create WeaponAttackDefinition
  What: Extends ActionDefinition for weapon attacks
  Why: Connect weapons to action system
  Done when: WeaponAttackDefinition creates WeaponAttackRuntime
  Depends on: [TASK-004], [TASK-005]

  **Fields:**
  ```csharp
  public WeaponDefinition weapon;
  public AttackType attackType; // Fast/Heavy
  ```

### [TASK-008] Create WeaponAttackRuntime
  What: Execute weapon attacks with phases and hitboxes
  Why: Handle attack timing and damage
  Done when: Attack spawns hitbox during Execute phase
  Depends on: [TASK-007]

  **Fields:**
  ```csharp
  protected WeaponDefinition Weapon;
  protected AttackType AttackType;
  protected AttackPattern CurrentPattern;
  protected Collider2D HitboxCollider;
  
  public void SpawnHitbox();
  public void DestroyHitbox();
  public void ApplyDamage(GameObject target);
  public bool CanChainNextAttack;
  ```

### [TASK-009] Create HitboxManager component
  What: Spawn/destroy melee hitboxes on weapon
  Why: Detect enemies in weapon range
  Done when: Hitbox triggers on enemy collision
  Depends on: [TASK-008]

  **Features:**
  - Spawn box collider at weapon tip
  - Rotate with weapon animation
  - Trigger on enemy collision
  - Apply damage + knockback

### [TASK-010] Create WeaponManager component
  What: Track equipped weapon, handle switching
  Why: Manage weapon loadout
  Done when: Can switch weapons via input
  Depends on: [TASK-002], [TASK-005]

  **Features:**
  - Track equipped weapon
  - Handle weapon switching (0.5s animation)
  - Cannot switch during action recovery
  - Update UI with current weapon

---

## Phase 3: Spell System

### [TASK-011] Create SpellDefinition ScriptableObject
  What: Spell data with type, cooldown, effects
  Why: Store spell-specific data
  Done when: Can create spell asset in Unity
  Depends on: [TASK-003]

  **Fields:**
  ```csharp
  public string spellName;
  public SpellType spellType; // Projectile/AoE/Buff/Debuff
  public float cooldown;
  public float cost; // mana/energy
  public float castTime;
  public SpellEffect effect;
  public AnimationClip animationClip;
  ```

### [TASK-012] Create SpellRuntime class
  What: Execute spells with cooldown tracking
  Why: Handle spell casting and cooldowns
  Done when: Spell can cast and enter cooldown
  Depends on: [TASK-004], [TASK-011]

  **Fields:**
  ```csharp
  protected SpellDefinition Spell;
  protected float CooldownTimer;
  protected bool IsOnCooldown;
  protected int SpellSlotIndex; // 0-3
  
  public void StartCooldown();
  public void CheckCooldown();
  public void SpawnProjectile();
  public void ApplySpellEffect();
  ```

### [TASK-013] Create SpellManager component
  What: Manage 4 spell slots, track cooldowns
  Why: Handle spell loadout
  Done when: All 4 slots functional with cooldowns
  Depends on: [TASK-012]

  **Features:**
  - 4 spell slots (independent cooldowns)
  - Cooldowns persist across state transitions
  - Visual cooldown indicator on UI
  - Spell customization (loadout system)

### [TASK-014] Create ProjectileSpawner component
  What: Spawn projectiles for spell effects
  Why: Visual + functional spell effects
  Done when: Projectile spawns and deals damage
  Depends on: [TASK-013]

  **Features:**
  - Spawn projectile at character position
  - Projectile follows direction
  - Lifetime + damage on collision
  - VFX + audio on spawn/hit

---

## Phase 4: State Machine Integration

### [TASK-015] Create CombatState Definition
  What: State with all weapon/spell actions
  Why: Combat state in FSM
  Done when: CombatState asset created with actions
  Depends on: [TASK-007], [TASK-012]

  **Actions:**
  - WeaponAttackDefinition (Fast)
  - WeaponAttackDefinition (Heavy)
  - SpellDefinition (Slot 1-4)

### [TASK-016] Integrate ActionRuntime with StateRuntime
  What: Connect actions to state machine lifecycle
  Why: Actions run within states
  Done when: Actions activate/deactivate with state
  Depends on: [TASK-004], [TASK-015]

  **Integration:**
  - Actions activate on state enter
  - Actions deactivate on state exit
  - Actions can request state transitions

### [TASK-017] Create ActionTransition conditions
  What: Transition to Idle when actions complete
  Why: Clean state flow
  Done when: CombatState → IdleState on action complete
  Depends on: [TASK-016]

  **Conditions:**
  - NoActionActiveCondition
  - AllCooldownsExpiredCondition

---

## Phase 5: Combat Feedback

### [TASK-018] Create DamageNumberSpawner
  What: Floating damage text on hit
  Why: Combat feedback
  Done when: Damage numbers appear on enemy hit
  Depends on: [TASK-009]

  **Features:**
  - Spawn at hit location
  - Float up + fade out
  - Color by damage type
  - Pooling for performance

### [TASK-019] Create CombatVFXSpawner
  What: Hit effects, weapon swing VFX
  Why: Visual feedback
  Done when: VFX play on attack execute
  Depends on: [TASK-008]

  **Features:**
  - Weapon swing trails
  - Hit sparks/particles
  - Spell cast effects
  - Pooling for performance

### [TASK-020] Create CombatAudioManager
  What: Weapon swing sounds, hit sounds
  Why: Audio feedback
  Done when: Audio plays on attack phases
  Depends on: [TASK-008]

  **Features:**
  - Weapon swing audio (per weapon)
  - Hit impact sounds
  - Spell cast audio
  - Volume + pitch variation

---

## Phase 6: Polish & Edge Cases

### [TASK-021] Implement action queueing
  What: Queue inputs during recovery
  Why: Responsive controls
  Done when: Queued input executes after recovery
  Depends on: [TASK-002], [TASK-004]

  **Features:**
  - Queue max 2 inputs
  - Execute queued input after recovery
  - Clear queue on state change

### [TASK-022] Implement animation integration
  What: Trigger animations on action phases
  Why: Visual combat
  Done when: Animations sync with action phases
  Depends on: [TASK-008], [TASK-012]

  **Features:**
  - Windup → Start animation
  - Execute → Hit frame
  - Recovery → End animation
  - Cancel → Interrupt animation

### [TASK-023] Create UI cooldown indicators
  What: Visual cooldowns for spells
  Why: Player feedback
  Done when: Cooldowns visible on UI
  Depends on: [TASK-013]

  **Features:**
  - 4 spell slot icons
  - Overlay fade on cooldown
  - Tooltip on hover
  - Update in real-time

### [TASK-024] Implement weapon switching animation
  What: Smooth weapon switch with animation
  Why: Visual polish
  Done when: Weapon switch plays animation
  Depends on: [TASK-010], [TASK-022]

  **Features:**
  - 0.5s switch animation
  - Cannot attack during switch
  - Visual weapon change

### [TASK-025] Create action debug overlay
  What: Debug UI showing current action state
  Why: Development debugging
  Done when: Debug overlay shows action phases
  Depends on: [TASK-004]

  **Features:**
  - Current action name
  - Current phase
  - Phase timer
  - Cooldown timers

---

## Execution Order

```
Phase 1: Core Infrastructure
  TASK-001 → TASK-002 → TASK-003 → TASK-004

Phase 2: Weapon System
  TASK-005 → TASK-006 → TASK-007 → TASK-008 → TASK-009 → TASK-010

Phase 3: Spell System
  TASK-011 → TASK-012 → TASK-013 → TASK-014

Phase 4: State Machine Integration
  TASK-015 → TASK-016 → TASK-017

Phase 5: Combat Feedback
  TASK-018 → TASK-019 → TASK-020

Phase 6: Polish & Edge Cases
  TASK-021 → TASK-022 → TASK-023 → TASK-024 → TASK-025
```

---

## Design Decisions

### Action Cancellation

| Phase | Can Cancel? | Can Interrupt? |
|-------|-------------|----------------|
| Windup | ✅ Yes | ✅ Yes |
| Execute | ❌ No | ❌ No |
| Recovery | ❌ No | ❌ No |

### Action Chaining

- Fast Attack → Fast Attack (if `canChain` = true)
- Fast Attack → Heavy Attack (always allowed)
- Heavy Attack → Fast Attack (after recovery)
- Spells → Any attack (after cast)

### Weapon Switching

- Can switch during Idle state only
- Switching takes 0.5s animation
- Cannot switch during action recovery

### Spell Cooldowns

- Each spell slot has independent cooldown
- Cooldowns persist across state transitions
- Visual cooldown indicator on UI

### Hit Detection

- Melee: Box collider on weapon tip (rotate with animation)
- Ranged: Projectile with lifetime + damage on collision
- Spells: Varies by type (projectile, AoE, instant)

---

## Notes

- All tasks are atomic (1-4 hours each)
- Dependencies are explicit
- Can execute phases in order or parallelize within phase
- Start with Phase 1 before moving to weapons/spells
- Test each phase before proceeding

---

*Generated by EDI - Diego's Personal Assistant*
*Last Updated: $(date)*
