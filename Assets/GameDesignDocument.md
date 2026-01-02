# RealmCrawler - Game Design Document

## Project Overview

**Elevator Pitch**  
"Death is just the beginning of the negotiation. RealmCrawler is a stylized, twin-stick survival roguelite set in a hauntingly beautiful pocket dimension. As a Warlock fighting to reclaim your soul from an eldritch Patron, you must slot powerful gear and evolve your spells to survive a relentless supernatural siege. The island is floating, the clock is ticking, and the Patron is watching. How long can you cheat the debt?"

**Genre:** Roguelite, Survivor-like, Twin-Stick Shooter  
**Platform(s):** PC, Mac, Linux (Console/mobile potential)  
**Target Audience:** All ages, Fantasy fans, Casual players

---

## Core Loop

1. Fight through waves of enemies that increase in difficulty
2. Earn XP to level up and unlock spells during the run
3. Collect Dark Bargains currency based on performance
4. Use currency to rent gear for your next run
5. Survive as long as possible

**Victory Condition:** Survive as long as possible and reach the highest level

---

## Camera System

**Style:** Angled top-down (centered, not isometric) that follows the player

**Technical Specs:**

- Height: 10-15 units above player
- Angle: 50-60° looking down
- Follow Type: Smooth (Lerp or SmoothDamp in LateUpdate)
- Projection: Test both Orthographic (8-12 size) and Perspective (50-60° FOV)

**Implementation:**

- Option A: Custom follow script with smooth damping
- Option B: Cinemachine Virtual Camera (recommended)

**Design Goals:**

- Clear view of action and surrounding enemies
- Supports twin-stick aiming clarity
- Medium smoothing (not instant, not sluggish)
- Optional dead zone for stable framing

---

## Player Controls & Stats

### Input Mapping

**Keyboard & Mouse:**

- WASD: Movement
- Mouse: Aim direction
- Left Click: Primary Cantrip
- Right Click: Secondary Cantrip
- Q, E: Spell Slots 1 & 2
- Shift: Utility Spell Slot

**Gamepad:**

- Left Stick: Movement
- Right Stick: Aim direction
- LT, RT: Cantrips
- A, B, X, Y: Spells and Utility

### Player Stats

- **Health:** Affected by Cloak equipment
- **Mana:** Affected by Hat equipment, regenerates over time
- **XP:** Collected from enemy essence drops
- **Currency:** Dark Bargains (persists between runs)
- **Speed:** Affected by Boots equipment

**Note:** No jump or crouch mechanics. Simple flat map with boundaries and foliage/decor.

---

## Spell System

### Weapons & Cantrips

**3 Weapon Types (MVP):**

1. **Wand** - 2 unique cantrips
2. **Staff** - 2 unique cantrips
3. **Tome** - 2 unique cantrips

**Cantrips:**

- Mapped to LC (Primary) and RC (Secondary)
- No mana cost, unlimited use
- Weaker than spells
- Unique to each weapon
- Cannot be replaced unless weapon is changed

### Spell Pool

**Total Spells:** 12 (unlockable during runs, reset on death)

**3 Elements × 4 Spell Types:**

- **Eldritch:** Projectile, AOE, Debuff, Utility
- **Fire:** Projectile, AOE, Debuff, Utility
- **Lightning:** Projectile, AOE, Debuff, Utility

**Spell Slots:**

- Q, E, Shift = 3 available slots for spells
- Chosen from random offerings during level-ups

**Resource System:**

- Mana cost + ~2 second cooldown (global cooldown to prevent spam)
- Mana regenerates over time (rate affected by equipment/upgrades)

---

## Progression System

### XP Collection

**Essence Orbs:**

- Visual: Floating wispy glowing orbs
- Theme: Enemy essence captured by Reliquary
- Behavior: Auto-collect within radius (orbs float to player)
- Scaling: Harder enemies drop more essence orbs
- Reliquary equipment increases collection radius

**XP Curve:**

- Increases per level
- No level cap (currently, may test and add later)

### Level-Up System

**Flow:**

1. Game pauses when leveling up
2. 3 random upgrade options appear
3. Player selects 1 upgrade
4. Gameplay resumes immediately

**Upgrade Types:**

- Health or Mana stat boost
- Weapon/Cantrip damage boost
- New spell to slot (offered every other level)
- Boost to currently-slotted spell
- Small currency payout bonus

---

## Gear & Meta Progression

### Currency: Dark Bargains

**Earning:** Based on level reached at end of run  
**Persistence:** Carries over between runs until spent  
**Usage:** Rent gear before each run

### Gear Rental System

**5 Equipment Categories:**

1. **Hat** - Affects Mana pool
2. **Cloak** - Affects Health
3. **Boots** - Affects Player Speed
4. **Weapon** - Affects Attack/Elemental Bonus (determines cantrips)
5. **Reliquary** - Affects XP Collect Radius

**Rental Rules:**

- All gear available from start (not locked)
- Each item has a Dark Bargains price
- Gear rented per run (not permanently owned)
- On death, all gear returns to Patron
- Can start run with no gear ("naked run")
- Choose up to 1 item per category if affordable

---

## Enemy Design

### Enemy Roster (5 Types - MVP)

**1. Skeleton (Melee Rusher)**

- Behavior: Run straight at player
- Attack: Contact damage
- Scaling: Starts slow, gets faster and stronger

**2. Ranged Imp (Ranged Shooter)**

- Behavior: Keep distance, fire projectiles
- Element: Fire
- Scaling: Grows stronger, deals more damage

**3. Melee Imp (Swarmer)**

- Behavior: Fast melee rushes
- Stats: Fast but weak, comes in large numbers

**4. Golem (Tank)**

- Behavior: Slow movement
- Stats: High health, dangerous up close

**5. Necromonger (Support/Special)**

- Behavior: Complex AI (heal allies OR attack player)
- Attack: Projectile that slows + damages player
- Element: Eldritch
- Positioning: Maintains distance before attacking

### Enemy Drops & Scaling

**Drops:**

- Essence orbs only (XP)
- Harder enemies drop more essences
- No currency drops

**Scaling Per Wave:**

- More health
- More damage
- Faster movement
- More numerous
- New enemy type introduced every 3 waves

### Enemy AI

**Pathfinding:** All enemies use Unity NavMesh  
**Behavior:** Simple movement, ranged positioning, complex decision trees  
**Visual Design:** Unified "Patron's minions" aesthetic with distinct silhouettes  
**No bosses/elites for MVP**

---

## Wave System

### Wave Trigger & Flow

**Wave Progression:**
- Clear-all based: Kill all enemies to complete wave
- Wave ends when all enemies are defeated
- 10 second breather between waves with visual "Wave X Incoming" notification

### Enemy Spawning

**Spawn Timing:**
- 50% of wave enemies spawn immediately at wave start
- Remaining 50% trickle in: 10% of remaining total every 2-3 seconds
- Example: Wave with 20 enemies = 10 spawn immediately, then 1 every 2-3 seconds

**Spawn Locations:**
- 4-8 random spawn points along map edges
- Enemies spawn from random available spawn points
- Off-screen or at map boundaries

### Enemy Count & Scaling

**Base Wave Size:**
- Wave 1: 10 Skeletons (baseline)
- Enemy count scales via percentage growth per wave (inspector-adjustable)

**Stat Scaling (Per Wave):**
- Health: Percentage increase (inspector-adjustable)
- Damage: Percentage increase (inspector-adjustable)
- Speed: Percentage increase (inspector-adjustable)

### Enemy Composition by Wave Phase

**Waves 1-2: Early Game**
- 100% Skeletons
- Total: 10 enemies (baseline)

**Waves 3-5: Ranged Introduction**
- 70% Skeletons
- 30% Ranged Imps

**Waves 6-8: Swarm Phase**
- 60% Skeletons
- 20% Ranged Imps
- 20% Melee Imps (Swarmers)

**Waves 9-11: Tank Introduction**
- 50% Skeletons
- 20% Ranged Imps
- 20% Melee Imps
- 10% Golems

**Waves 12+: Full Roster**
- 40% Skeletons
- 20% Ranged Imps
- 20% Melee Imps
- 10% Golems
- 10% Necromongers

**Note:** All ratios are inspector-adjustable for easy balancing

---

## Map & Environment

- Simple flat map with boundaries
- Foliage and decorative elements
- Floating island pocket dimension theme
- **Wells** spawn on map for HP/Mana restoration

---

## Narrative & Aesthetics

### Story

**Setting:** Fantasy-inspired floating island pocket dimension  
**Protagonist:** A Warlock whose pact has come due  
**Conflict:** Bargained for freedom by surviving the Patron's trials  
**Antagonist:** The Patron and their minions

### Visual Style

- 3D, angled top-down perspective
- Stylized with toon shaders, mid to low poly
- **Colors:** Desaturated purples, blues, pinks, greens, greys, black + orange/red accents

---

## Tutorial & Difficulty

**Tutorial:** First run only, Reliquary guides movement, combat, XP collection

**Difficulty Curve:**

- Waves 1-3: Easy
- Waves 4-10: Scale slightly
- Waves 11-20: Scale consistently
- Beyond 20: Continues scaling

---

## Development Roadmap

**Engine:** Unity 6 (6000.3)  
**Tools:** Blender, Photoshop, Canva, ZBrush, Bezi, Gemini

**MVP Checklist:**

- [ ] Basic movement prototype
- [ ] Core interaction mechanic
- [ ] Simple placeholder art
- [ ] Win/Loss screen

---

## Technical Implementation Notes

### Recommended Unity Architecture

**Spell System:**
- ScriptableObject per spell (properties: element, type, cooldown, mana cost, damage)
- ScriptableObject per weapon (cantrip references)
- Spell slot manager (tracks Q/E/Shift assignments)
- Cooldown system with UI feedback
- Mana system with regen tick rate

**Progression System:**
- Gear shop/loadout UI before run starts
- Currency display (Dark Bargains balance)
- XP orb collection zone with trigger collider
- Level-up UI (pauses with Time.timeScale = 0)
- Upgrade pool system (alternates spell offers)
- End-of-run summary (level reached + Dark Bargains earned)

**Enemy System:**
- Enemy ScriptableObjects for stats (base health, damage, speed)
- NavMesh-based pathfinding
- State machine for complex AI (Necromonger)
- Wave spawn manager with inspector-adjustable parameters
- Difficulty scaling system (percentage-based stat multipliers)

**Wave System:**
- WaveManager ScriptableObject with:
  - Enemy composition ratios per wave phase (inspector arrays)
  - Spawn count scaling percentage per wave
  - Stat scaling percentages (health, damage, speed)
  - Spawn interval timing (2-3 seconds)
  - Wave break duration (10 seconds)
- Spawn point array (4-8 transforms at map edges)
- Enemy pool/object pooling for performance
- Wave UI notification system

---

## Project Structure

All assets must be placed in the corresponding directories:
- Code: `/Assets/Scripts`
- Materials: `/Assets/Materials`
- Prefabs: `/Assets/Prefabs`
- 3D models: `/Assets/Models`
- Scenes: `/Assets/Scenes`

---

**Last Updated:** 2024  
**Status:** Pre-Production / Design Phase
