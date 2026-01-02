# ScriptableObjects Setup Guide

This guide contains all instructions for creating the ScriptableObject assets needed for RealmCrawler.

---

## Table of Contents

1. [Folder Structure](#folder-structure)
2. [Spell System Assets](#1-spell-system-assets)
3. [Equipment System Assets](#2-equipment-system-assets)
4. [Enemy System Assets](#3-enemy-system-assets)
5. [Wave System Assets](#4-wave-system-assets)
6. [Progression System Assets](#5-progression-system-assets)
7. [Weapon & Cantrip Setup Guide](#weapon--cantrip-setup-guide)
8. [Projectile Prefab Creation](#projectile-prefab-creation)
9. [Manager Scripts Setup](#manager-scripts-setup)
10. [Understanding Data Flow](#understanding-data-flow)
11. [Asset Creation Checklist](#asset-creation-checklist)
12. [Visual Asset Requirements](#visual-asset-requirements)

---

## Folder Structure

Organize your ScriptableObject assets in the following structure:

```
/Assets/RealmCrawler_Project/ScriptableObjects/
  /Spells/
    /Eldritch/
      - EldritchProjectile.asset
      - EldritchAOE.asset
      - EldritchDebuff.asset
      - EldritchUtility.asset
    /Fire/
      - FireProjectile.asset
      - FireAOE.asset
      - FireDebuff.asset
      - FireUtility.asset
    /Lightning/
      - LightningProjectile.asset
      - LightningAOE.asset
      - LightningDebuff.asset
      - LightningUtility.asset
  /Cantrips/
    - WandPrimary.asset
    - WandSecondary.asset
    - StaffPrimary.asset
    - StaffSecondary.asset
    - TomePrimary.asset
    - TomeSecondary.asset
  /Weapons/
    - Wand.asset
    - Staff.asset
    - Tome.asset
  /Equipment/
    /Hats/
      - BasicHat.asset
      - AdvancedHat.asset
      (3-5 total)
    /Cloaks/
      - BasicCloak.asset
      - AdvancedCloak.asset
      (3-5 total)
    /Boots/
      - BasicBoots.asset
      - AdvancedBoots.asset
      (3-5 total)
    /Reliquaries/
      - BasicReliquary.asset
      - AdvancedReliquary.asset
      (3-5 total)
  /Enemies/
    - Skeleton.asset
    - RangedImp.asset
    - MeleeImp.asset
    - Golem.asset
    - Necromonger.asset
  /Waves/
    - DefaultWaveConfig.asset
  /Upgrades/
    - HealthBoost10.asset
    - ManaBoost10.asset
    - SpellDamageBoost.asset
    - CantripDamageBoost.asset
    - CurrencyBonus.asset
    (15-20 total upgrade variants)
```

---

## 1. Spell System Assets

### SpellData (12 Total Assets)

**How to Create:**
1. Right-click in Project window
2. Navigate to: `Create > RealmCrawler > Spells > Spell Data`
3. Name the asset (e.g., "EldritchProjectile")

**Assets to Create:**

**Eldritch Element (4 spells):**
- EldritchProjectile.asset
- EldritchAOE.asset
- EldritchDebuff.asset
- EldritchUtility.asset

**Fire Element (4 spells):**
- FireProjectile.asset
- FireAOE.asset
- FireDebuff.asset
- FireUtility.asset

**Lightning Element (4 spells):**
- LightningProjectile.asset
- LightningAOE.asset
- LightningDebuff.asset
- LightningUtility.asset

**Fields to Configure:**

**Basic Info:**
- Spell Name: Display name
- Description: What the spell does
- Icon: Spell icon sprite (2D)

**Spell Properties:**
- Element: Eldritch / Fire / Lightning
- Type: Projectile / AOE / Debuff / Utility

**Resource Costs:**
- Mana Cost: How much mana to cast (default: 10)
- Cooldown Duration: Seconds before can cast again (default: 2)

**Damage & Effects:**
- Base Damage: Starting damage value (default: 10)
- AOE Radius: For AOE spells, explosion radius (default: 0)
- Debuff Duration: For debuff spells, how long effect lasts (default: 0)

**Visuals & Audio:**
- Spell Prefab: GameObject with projectile/effect visuals
- Cast Sound: AudioClip played on cast

---

### CantripData (6 Total Assets)

**How to Create:**
1. Right-click in Project window
2. Navigate to: `Create > RealmCrawler > Spells > Cantrip Data`
3. Name the asset (e.g., "WandPrimary")

**Assets to Create:**
- WandPrimary.asset
- WandSecondary.asset
- StaffPrimary.asset
- StaffSecondary.asset
- TomePrimary.asset
- TomeSecondary.asset

**Fields to Configure:**

**Basic Info:**
- Cantrip Name: Display name
- Description: What the cantrip does
- Icon: Cantrip icon sprite (2D)

**Combat Properties:**
- Base Damage: Starting damage (default: 5)
- Attack Speed: Attacks per second (default: 1)
- Projectile Speed: How fast projectile travels (default: 10)

**Visuals & Audio:**
- Projectile Prefab: GameObject with projectile visuals
- Cast Sound: AudioClip played on cast

---

### WeaponData (3 Total Assets)

**How to Create:**
1. Right-click in Project window
2. Navigate to: `Create > RealmCrawler > Equipment > Weapon Data`
3. Name the asset (e.g., "Wand")

**Assets to Create:**
- Wand.asset
- Staff.asset
- Tome.asset

**Fields to Configure:**

**Basic Info:**
- Weapon Name: Display name (e.g., "Wand")
- Description: Weapon description
- Icon: Weapon icon sprite (2D)

**Cantrips:**
- Primary Cantrip: Reference to cantrip asset (Left Click)
- Secondary Cantrip: Reference to cantrip asset (Right Click)

**Stats:**
- Damage Multiplier: Weapon damage modifier (default: 1.0)

**Rental System:**
- Rental Cost: Dark Bargains cost (default: 0 for basic weapons)

**Visuals:**
- Weapon Visual Prefab: 3D model displayed on player

**Example Setup:**
```
Wand.asset:
  - Primary Cantrip: WandPrimary.asset
  - Secondary Cantrip: WandSecondary.asset
  - Damage Multiplier: 1.0
  - Rental Cost: 0
```

---

## 2. Equipment System Assets

### HatData (3-5 Assets)

**How to Create:**
`Create > RealmCrawler > Equipment > Hat Data`

**Fields to Configure:**
- Equipment Name: Display name
- Description: What it does
- Icon: Equipment icon sprite
- Category: Hat (auto-set)
- Rental Cost: Dark Bargains price
- Visual Prefab: 3D model for player head
- **Mana Bonus:** How much mana it adds (e.g., 10, 20, 30)

**Example Assets:**
- BasicHat.asset (Mana Bonus: +10, Cost: 5)
- AdvancedHat.asset (Mana Bonus: +25, Cost: 15)
- EliteHat.asset (Mana Bonus: +50, Cost: 30)

---

### CloakData (3-5 Assets)

**How to Create:**
`Create > RealmCrawler > Equipment > Cloak Data`

**Fields to Configure:**
- Equipment Name: Display name
- Description: What it does
- Icon: Equipment icon sprite
- Category: Cloak (auto-set)
- Rental Cost: Dark Bargains price
- Visual Prefab: 3D model for player body
- **Health Bonus:** How much health it adds (e.g., 20, 40, 60)

**Example Assets:**
- BasicCloak.asset (Health Bonus: +20, Cost: 5)
- AdvancedCloak.asset (Health Bonus: +50, Cost: 15)
- EliteCloak.asset (Health Bonus: +100, Cost: 30)

---

### BootsData (3-5 Assets)

**How to Create:**
`Create > RealmCrawler > Equipment > Boots Data`

**Fields to Configure:**
- Equipment Name: Display name
- Description: What it does
- Icon: Equipment icon sprite
- Category: Boots (auto-set)
- Rental Cost: Dark Bargains price
- Visual Prefab: 3D model for player feet
- **Speed Multiplier:** Movement speed multiplier (e.g., 1.2 = 20% faster)

**Example Assets:**
- BasicBoots.asset (Speed: 1.15x, Cost: 5)
- AdvancedBoots.asset (Speed: 1.3x, Cost: 15)
- EliteBoots.asset (Speed: 1.5x, Cost: 30)

---

### ReliquaryData (3-5 Assets)

**How to Create:**
`Create > RealmCrawler > Equipment > Reliquary Data`

**Fields to Configure:**
- Equipment Name: Display name
- Description: What it does
- Icon: Equipment icon sprite
- Category: Reliquary (auto-set)
- Rental Cost: Dark Bargains price
- Visual Prefab: 3D model for player waist/belt
- **XP Collect Radius Bonus:** Additional collection radius (e.g., +2, +4, +6 units)

**Example Assets:**
- BasicReliquary.asset (Radius: +2, Cost: 5)
- AdvancedReliquary.asset (Radius: +5, Cost: 15)
- EliteReliquary.asset (Radius: +10, Cost: 30)

---

## 3. Enemy System Assets

### EnemyData (5 Total Assets)

**How to Create:**
1. Right-click in Project window
2. Navigate to: `Create > RealmCrawler > Enemies > Enemy Data`
3. Name the asset (e.g., "Skeleton")

**Assets to Create:**

**1. Skeleton.asset**
- Enemy Name: "Skeleton"
- Behavior Type: MeleeRusher
- Base Health: 50
- Base Damage: 10
- Base Speed: 3.5
- Essence Drop Count: 1
- Enemy Prefab: Skeleton prefab reference

**2. RangedImp.asset**
- Enemy Name: "Ranged Imp"
- Behavior Type: RangedShooter
- Base Health: 40
- Base Damage: 8
- Base Speed: 3.0
- Essence Drop Count: 1
- Enemy Prefab: Ranged Imp prefab reference

**3. MeleeImp.asset**
- Enemy Name: "Melee Imp"
- Behavior Type: Swarmer
- Base Health: 30
- Base Damage: 5
- Base Speed: 5.0
- Essence Drop Count: 1
- Enemy Prefab: Melee Imp prefab reference

**4. Golem.asset**
- Enemy Name: "Golem"
- Behavior Type: Tank
- Base Health: 150
- Base Damage: 20
- Base Speed: 2.0
- Essence Drop Count: 3
- Enemy Prefab: Golem prefab reference

**5. Necromonger.asset**
- Enemy Name: "Necromonger"
- Behavior Type: Support
- Base Health: 60
- Base Damage: 12
- Base Speed: 2.5
- Essence Drop Count: 2
- Enemy Prefab: Necromonger prefab reference

---

## 4. Wave System Assets

### WaveConfigData (1 Asset)

**How to Create:**
1. Right-click in Project window
2. Navigate to: `Create > RealmCrawler > Waves > Wave Config Data`
3. Name it "DefaultWaveConfig"

**Fields to Configure:**

**Base Wave Settings:**
- Base Enemy Count: 10 (starting number of enemies)
- Enemy Count Scaling Percent: 0.1 (10% more enemies per wave)

**Stat Scaling Per Wave:**
- Health Scaling Percent: 0.15 (15% more health per wave)
- Damage Scaling Percent: 0.1 (10% more damage per wave)
- Speed Scaling Percent: 0.05 (5% faster per wave)

**Spawn Timing:**
- Initial Spawn Percent: 0.5 (50% spawn immediately)
- Trickle Spawn Percent: 0.1 (10% of remaining spawn every interval)
- Spawn Interval Min: 2.0 seconds
- Spawn Interval Max: 3.0 seconds

**Wave Transitions:**
- Wave Breaker Duration: 10.0 seconds (break between waves)

**Spawn Points:**
- Min Spawn Points: 4
- Max Spawn Points: 8

**Wave Phases:**
The wave phases are pre-configured with the following composition:

- **Waves 1-2:** 100% Skeletons
- **Waves 3-5:** 70% Skeletons, 30% Ranged Imps
- **Waves 6-8:** 60% Skeletons, 20% Ranged Imps, 20% Melee Imps
- **Waves 9-11:** 50% Skeletons, 20% Ranged Imps, 20% Melee Imps, 10% Golems
- **Waves 12+:** 40% Skeletons, 20% Ranged Imps, 20% Melee Imps, 10% Golems, 10% Necromongers

**Enemy References:**
After creating the 5 enemy data assets, assign them here:
- Skeleton Data: Skeleton.asset
- Ranged Imp Data: RangedImp.asset
- Melee Imp Data: MeleeImp.asset
- Golem Data: Golem.asset
- Necromonger Data: Necromonger.asset

---

## Asset Creation Checklist

### Spell System
- [ ] 12 SpellData assets (4 per element)
- [ ] 6 CantripData assets (2 per weapon)
- [ ] 3 WeaponData assets

### Equipment System
- [ ] 3-5 HatData assets
- [ ] 3-5 CloakData assets
- [ ] 3-5 BootsData assets
- [ ] 3-5 ReliquaryData assets

### Enemy System
- [ ] 5 EnemyData assets (Skeleton, Ranged Imp, Melee Imp, Golem, Necromonger)

### Wave System
- [ ] 1 WaveConfigData asset (DefaultWaveConfig)

### Progression System
- [ ] 15-20 UpgradeData assets (various upgrade types)

---

## 5. Progression System Assets

### UpgradeData (15-20 Total Assets)

**How to Create:**
1. Right-click in Project window
2. Navigate to: `Create > RealmCrawler > Progression > Upgrade Data`
3. Name the asset (e.g., "HealthBoost10")

**Upgrade Types Available:**
- `HealthBoost` - Increases max health by value
- `ManaBoost` - Increases max mana by value
- `SpellUnlock` - Unlocks a spell for hotbar assignment
- `SpellDamageBoost` - Boosts spell damage by percentage
- `CantripDamageBoost` - Boosts cantrip damage by percentage
- `CurrencyBonus` - Awards Dark Bargains currency

**Fields to Configure:**

**Basic Info:**
- Upgrade Name: Display name (e.g., "Vitality Surge")
- Description: What the upgrade does (e.g., "Increase max health by 10")
- Icon: Upgrade icon sprite (2D)

**Upgrade Properties:**
- Upgrade Type: Select from dropdown
- Value: Numerical value (e.g., 10 for +10 health, 0.15 for +15% damage)

**Spell Unlock (if applicable):**
- Spell To Unlock: Reference to SpellData (only used if type is SpellUnlock)

**Weighting:**
- Spawn Weight: 0.0 to 1.0 (affects random selection probability)
  - 1.0 = normal weight
  - 0.5 = half as likely to appear
  - 0.0 = never appears

**Example Assets:**

**Health Upgrades:**
```
HealthBoost10.asset:
  - Upgrade Name: "Vitality Surge"
  - Description: "Increase max health by 10"
  - Type: HealthBoost
  - Value: 10
  - Spawn Weight: 1.0

HealthBoost25.asset:
  - Upgrade Name: "Greater Vitality"
  - Description: "Increase max health by 25"
  - Type: HealthBoost
  - Value: 25
  - Spawn Weight: 0.7
```

**Mana Upgrades:**
```
ManaBoost10.asset:
  - Upgrade Name: "Arcane Reservoir"
  - Description: "Increase max mana by 10"
  - Type: ManaBoost
  - Value: 10
  - Spawn Weight: 1.0
```

**Damage Upgrades:**
```
SpellDamageBoost15.asset:
  - Upgrade Name: "Mystic Amplification"
  - Description: "Increase spell damage by 15%"
  - Type: SpellDamageBoost
  - Value: 0.15
  - Spawn Weight: 0.8

CantripDamageBoost10.asset:
  - Upgrade Name: "Weapon Mastery"
  - Description: "Increase cantrip damage by 10%"
  - Type: CantripDamageBoost
  - Value: 0.10
  - Spawn Weight: 1.0
```

**Spell Unlock Upgrades:**
```
UnlockEldritchAOE.asset:
  - Upgrade Name: "Unlock: Eldritch Blast"
  - Description: "Unlocks Eldritch AOE spell"
  - Type: SpellUnlock
  - Value: 0
  - Spell To Unlock: EldritchAOE.asset
  - Spawn Weight: 0.5
```

**Currency Upgrades:**
```
CurrencyBonus50.asset:
  - Upgrade Name: "Dark Favor"
  - Description: "Gain 50 Dark Bargains"
  - Type: CurrencyBonus
  - Value: 50
  - Spawn Weight: 0.6
```

**Recommended Asset List (15-20 upgrades):**
- HealthBoost10.asset (common)
- HealthBoost25.asset (uncommon)
- HealthBoost50.asset (rare)
- ManaBoost10.asset (common)
- ManaBoost25.asset (uncommon)
- ManaBoost50.asset (rare)
- SpellDamageBoost10.asset (common)
- SpellDamageBoost15.asset (uncommon)
- SpellDamageBoost25.asset (rare)
- CantripDamageBoost10.asset (common)
- CantripDamageBoost20.asset (uncommon)
- CurrencyBonus50.asset (uncommon)
- CurrencyBonus100.asset (rare)
- UnlockEldritchAOE.asset (spell unlock)
- UnlockFireProjectile.asset (spell unlock)
- UnlockLightningDebuff.asset (spell unlock)
- (Add more spell unlocks for each of your 12 spells)

---

## Weapon & Cantrip Setup Guide

### Weapon Design Philosophy

Each weapon class should feel unique through:
1. **Unique cantrips** - Different visuals, projectile behavior per weapon
2. **Element affinity** - Each weapon buffs a specific spell element
3. **Unique playstyle** - Different stats create different combat feel

---

### Recommended Weapon Configurations

#### **Wand (Eldritch Specialist)**

**Weapon Stats:**
- **Weapon Name:** "Prototype Wand"
- **Description:** "Quick-casting wand that amplifies Eldritch magic"
- **Damage Multiplier:** `1.0`
- **Buffed Element:** `Eldritch`
- **Element Damage Bonus:** `0.2` (20% bonus to Eldritch spells)
- **Rental Cost:** `0`

**Cantrips:**
- **Primary Cantrip:** `SO_Cantrip_Proto_WandPrimary`
  - Cantrip Name: "Eldritch Bolt"
  - Description: "Quick burst of eldritch energy"
  - Base Damage: `4`
  - Attack Speed: `1.2` (faster)
  - Projectile Speed: `15` (fast)
  - Projectile Prefab: Your primary projectile prefab
  
- **Secondary Cantrip:** `SO_Cantrip_Proto_WandSecondary`
  - Cantrip Name: "Eldritch Burst"
  - Description: "Powerful eldritch projectile"
  - Base Damage: `8`
  - Attack Speed: `0.6`
  - Projectile Speed: `10`
  - Projectile Prefab: Your secondary projectile prefab

**Playstyle:** Fast, agile, mobile gameplay. Synergizes with Eldritch spells.

---

#### **Staff (Fire Specialist)**

**Weapon Stats:**
- **Weapon Name:** "Prototype Staff"
- **Description:** "Powerful staff that channels Fire magic"
- **Damage Multiplier:** `1.2`
- **Buffed Element:** `Fire`
- **Element Damage Bonus:** `0.2` (20% bonus to Fire spells)
- **Rental Cost:** `0`

**Cantrips:**
- **Primary Cantrip:** `SO_Cantrip_Proto_StaffPrimary`
  - Cantrip Name: "Flame Lance"
  - Description: "Piercing flame projectile"
  - Base Damage: `6`
  - Attack Speed: `0.8` (slower)
  - Projectile Speed: `12`
  - Projectile Prefab: Your primary projectile prefab
  
- **Secondary Cantrip:** `SO_Cantrip_Proto_StaffSecondary`
  - Cantrip Name: "Fireball"
  - Description: "Explosive ball of fire"
  - Base Damage: `12`
  - Attack Speed: `0.4` (slow but powerful)
  - Projectile Speed: `8`
  - Projectile Prefab: Your secondary projectile prefab

**Playstyle:** High damage, slower casting. Synergizes with Fire spells.

---

#### **Tome (Lightning Specialist)**

**Weapon Stats:**
- **Weapon Name:** "Prototype Tome"
- **Description:** "Ancient tome that conducts Lightning magic"
- **Damage Multiplier:** `0.9`
- **Buffed Element:** `Lightning`
- **Element Damage Bonus:** `0.2` (20% bonus to Lightning spells)
- **Rental Cost:** `0`

**Cantrips:**
- **Primary Cantrip:** `SO_Cantrip_Proto_TomePrimary`
  - Cantrip Name: "Spark"
  - Description: "Rapid lightning spark"
  - Base Damage: `3`
  - Attack Speed: `1.5` (very fast)
  - Projectile Speed: `20` (very fast)
  - Projectile Prefab: Your primary projectile prefab
  
- **Secondary Cantrip:** `SO_Cantrip_Proto_TomeSecondary`
  - Cantrip Name: "Chain Lightning"
  - Description: "Arcing bolt of lightning"
  - Base Damage: `10`
  - Attack Speed: `0.5`
  - Projectile Speed: `18`
  - Projectile Prefab: Your secondary projectile prefab

**Playstyle:** Rapid-fire, chain attacks. Synergizes with Lightning spells.

---

### Weapon Comparison Table

```
WEAPON | CANTRIP DMG | ATTACK SPEED | ELEMENT BUFF | PLAYSTYLE
-------|-------------|--------------|--------------|----------
Wand   | 4/8         | Fast (1.2)   | Eldritch     | Mobile/Agile
Staff  | 6/12        | Slow (0.8)   | Fire         | Heavy Hitter
Tome   | 3/10        | Fastest(1.5) | Lightning    | Rapid-Fire
```

---

### How Element Bonuses Work

When you equip a weapon:
- **Wand equipped** → All Eldritch spells deal +20% damage
- **Staff equipped** → All Fire spells deal +20% damage  
- **Tome equipped** → All Lightning spells deal +20% damage

This creates strategic choices:
- Unlocked an Eldritch spell? Wand synergizes best
- Got Fire spells? Staff is your best bet
- Lightning build? Tome all the way

---

## Projectile Prefab Creation

### Overview

Projectiles are the visual and functional representations of cantrips. Each projectile needs:
1. **Visual mesh** - 3D model that flies through the air
2. **Rigidbody** - For physics-based movement
3. **Collider** - To detect hits
4. **ProjectileMover script** - Movement and damage logic

---

### Step-by-Step: Creating Projectile Prefabs

#### **Step 1: Create the GameObject**

1. **Create Empty GameObject:**
   - In Hierarchy: `GameObject > Create Empty`
   - Rename: `PrimaryCantrip_Projectile`
   - Reset Transform to (0,0,0)

2. **Add Visual Mesh:**
   - Drag your projectile mesh as a **child** of the projectile GameObject
   - Reset the child's transform
   - Adjust scale if needed
   - Add material/shader for visual effect

---

#### **Step 2: Add Required Components**

**On the Parent GameObject (`PrimaryCantrip_Projectile`):**

1. **Add Rigidbody:**
   - Click `Add Component > Physics > Rigidbody`
   - **Uncheck** "Use Gravity"
   - Set Collision Detection to "Continuous"
   - Leave "Is Kinematic" unchecked

2. **Add Collider:**
   - Click `Add Component > Physics > Capsule Collider` (or Sphere Collider)
   - **Check** "Is Trigger"
   - Adjust size to match projectile

3. **Add ProjectileMover Script:**
   - Click `Add Component`
   - Search: `ProjectileMover`
   - Click to add

---

#### **Step 3: Configure ProjectileMover**

**In the Inspector, set these values:**

**For Primary Cantrip:**
- **Speed:** `10` (default, will be overridden by CantripData)
- **Lifetime:** `5` seconds
- **Damage:** `5` (default, will be overridden by CantripData)
- **Hit Layers:** Leave default

**For Secondary Cantrip:**
- **Speed:** `8` (default)
- **Lifetime:** `5` seconds
- **Damage:** `8` (default)
- **Hit Layers:** Leave default

**Note:** The actual speed and damage will be set by ScriptableObjects at runtime. These are just fallback defaults.

---

#### **Step 4: Save as Prefab**

1. **Create Prefab:**
   - Drag GameObject from Hierarchy to `/Assets/RealmCrawler_Project/Prefabs/Projectiles/`
   - Name: `PRB_Proto_PrimaryCantrip_Projectile`

2. **Delete from Scene:**
   - Delete the GameObject from Hierarchy (we only need the prefab)

3. **Repeat for Secondary:**
   - Follow same steps for `PRB_Proto_SecondaryCantrip_Projectile`

---

### Projectile Prefab Checklist

After creating both prefabs, verify each has:

- [ ] Parent GameObject with projectile name
- [ ] Child GameObject with visual mesh
- [ ] Rigidbody component (Use Gravity = OFF)
- [ ] Collider component (Is Trigger = ON)
- [ ] ProjectileMover component
- [ ] Saved as prefab in `/Prefabs/Projectiles/`

---

### Linking Projectiles to Cantrips

**For Each of Your 6 CantripData Assets:**

1. **Select the CantripData asset** (e.g., `SO_Cantrip_Proto_WandPrimary`)
2. **Find "Projectile Prefab" field** in Inspector
3. **Drag the appropriate prefab:**
   - All **Primary** cantrips → `PRB_Proto_PrimaryCantrip_Projectile`
   - All **Secondary** cantrips → `PRB_Proto_SecondaryCantrip_Projectile`

**Current Setup (Prototyping):**
- For now, all primary cantrips share the same projectile visual
- All secondary cantrips share the same projectile visual
- Different speeds/damages still create unique feel

**Future Setup (Polish):**
- Create 6 unique projectile prefabs with different visuals
- Wand projectiles: Purple/dark energy
- Staff projectiles: Orange/red flames
- Tome projectiles: Blue/white lightning

---

### Weapon Visual Prefabs

**Simpler than projectiles - just visual models!**

**For Each Weapon (Wand, Staff, Tome):**

1. **Drag mesh into scene**
2. **Rename** (e.g., `Wand_Visual`)
3. **Reset Transform** (0,0,0 position/rotation)
4. **Add material** if needed
5. **Adjust scale** for player hand size
6. **Save as prefab** in `/Prefabs/Weapons/`
7. **Delete from scene**
8. **Link to WeaponData:** Drag prefab into "Weapon Visual Prefab" field

**No scripts needed!** PlayerVisualEquipment handles instantiation.

---

## Understanding Data Flow

### The Problem: Prefabs vs ScriptableObjects

**Common Question:** 
> "Will the ScriptableObject stats like projectile speed overwrite the projectile prefab script values?"

**Short Answer:** Not automatically - you need to connect them properly!

---

### Current System: How Data Flows

```
Step 1: Configure ScriptableObjects
┌─────────────────────────────┐
│ CantripData Asset           │
│ - Projectile Speed: 15      │
│ - Base Damage: 4            │
│ - Projectile Prefab: [Ref] │
└──────────────┬──────────────┘
               │
               ↓
Step 2: SpellManager Reads Data
┌──────────────────────────────┐
│ SpellManager.CastCantrip()  │
│ - Reads CantripData          │
│ - Reads WeaponData           │
│ - Calculates final damage   │
└──────────────┬───────────────┘
               │
               ↓
Step 3: Spawn & Configure Projectile
┌────────────────────────────────┐
│ Instantiate Prefab             │
│ Get ProjectileMover component  │
│ projectile.SetSpeed(15)        │
│ projectile.SetDamage(4 × 1.2)  │
└────────────────────────────────┘
               │
               ↓
Step 4: Projectile Flies!
┌────────────────────────────────┐
│ ProjectileMover.FixedUpdate()  │
│ - Moves at speed 15            │
│ - Deals damage 4.8 on hit      │
└────────────────────────────────┘
```

---

### Detailed Example: Wand Primary Cantrip

**1. You Configure Assets:**

**CantripData (`SO_Cantrip_Proto_WandPrimary`):**
```
Cantrip Name: "Eldritch Bolt"
Base Damage: 4
Attack Speed: 1.2
Projectile Speed: 15
Projectile Prefab: PRB_Proto_PrimaryCantrip_Projectile
```

**WeaponData (`SO_Weapon_Proto_Wand`):**
```
Weapon Name: "Prototype Wand"
Damage Multiplier: 1.0
Primary Cantrip: SO_Cantrip_Proto_WandPrimary
```

**Projectile Prefab (`PRB_Proto_PrimaryCantrip_Projectile`):**
```
ProjectileMover Component:
  Speed: 10 (default fallback)
  Damage: 5 (default fallback)
  Lifetime: 5
```

---

**2. At Runtime (Player Casts Cantrip):**

```csharp
// Player input triggers this
spellManager.TryCastCantrip(spellManager.PrimaryCantrip);

// Inside SpellManager.CastCantrip():

// A. Spawn projectile from prefab
GameObject projectileObj = Instantiate(
    cantrip.ProjectilePrefab,           // PRB_Proto_PrimaryCantrip_Projectile
    spellCastPoint.position,
    spellCastPoint.rotation
);

// B. Get the ProjectileMover component
ProjectileMover projectile = projectileObj.GetComponent<ProjectileMover>();

// C. Calculate final damage with weapon multiplier
float finalDamage = cantrip.BaseDamage;        // 4
if (equippedWeapon != null)
{
    finalDamage *= equippedWeapon.DamageMultiplier;  // 4 × 1.0 = 4
}

// D. Apply ScriptableObject values to projectile
projectile.SetDamage(finalDamage);      // Sets damage to 4
projectile.SetSpeed(cantrip.ProjectileSpeed);  // Sets speed to 15

// E. Projectile now flies with correct values!
// - Speed: 15 (from CantripData, not prefab's 10)
// - Damage: 4 (from CantripData, not prefab's 5)
```

---

**3. Result:**

✅ Projectile uses **CantripData speed (15)**, not prefab speed (10)  
✅ Projectile uses **CantripData damage (4)**, not prefab damage (5)  
✅ Weapon multiplier applies correctly (4 × 1.0 = 4)  
✅ Different cantrips can have different speeds/damages!

---

### Key Concept: "Prefab as Template"

Think of prefabs and ScriptableObjects this way:

**Prefab = Template/Starting Point**
- Defines structure (components, hierarchy)
- Has default/fallback values
- Used when nothing configures it

**ScriptableObject = Data Source**
- The real gameplay values
- Easily editable by designers
- Applied at runtime

**SpellManager = Connector**
- Reads ScriptableObject data
- Spawns prefab
- Configures prefab with ScriptableObject values

---

### Why This Approach?

**Benefits:**

✅ **Easy Balancing:**
- Change cantrip speed in ScriptableObject
- Instant gameplay change, no prefab editing

✅ **Weapon Variety:**
- Same projectile prefab
- Different CantripData → Different behavior

✅ **Designer-Friendly:**
- All tuning values in ScriptableObjects
- No need to edit prefabs or code

✅ **Damage Calculations:**
- Weapon multipliers apply correctly
- Upgrade bonuses can be added later
- Element bonuses can be implemented

**Example:**
```
Same Projectile Prefab → Used for all 6 cantrips

But at runtime:
- WandPrimary flies at speed 15, damage 4
- StaffPrimary flies at speed 12, damage 7.2 (6 × 1.2)
- TomePrimary flies at speed 20, damage 2.7 (3 × 0.9)

All using the SAME prefab, configured differently!
```

---

### Common Mistakes to Avoid

❌ **Editing values in prefab expecting them to be used**
- Prefab values are just defaults
- ScriptableObject values are the real source

❌ **Forgetting to call SetSpeed() and SetDamage()**
- Without these calls, prefab defaults are used
- ScriptableObject values are ignored

❌ **Not linking projectile prefab to CantripData**
- SpellManager won't know which prefab to spawn
- Cantrip casting fails silently

✅ **Correct Workflow:**
1. Create projectile prefab with default values
2. Create CantripData and link the prefab
3. Configure speed/damage in CantripData
4. SpellManager applies CantripData values at runtime
5. Test and balance by editing CantripData only

---

## Manager Scripts Setup

These manager scripts work with your ScriptableObjects to create functional gameplay systems.

### PlayerVisualEquipment.cs

**Purpose:** Manages visual representation of equipped gear on the player character.

**Setup Instructions:**

1. **Attach to Player GameObject:**
   - Select your Player GameObject
   - Add Component > Player Visual Equipment

2. **Create Equipment Slot Transforms:**
   ```
   Player GameObject
     ├── Body (3D Model)
     ├── HatSlot (Empty GameObject)
     ├── CloakSlot (Empty GameObject)
     ├── BootsSlot (Empty GameObject)
     ├── WeaponSlot (Empty GameObject - right hand)
     └── ReliquarySlot (Empty GameObject - waist)
   ```

3. **Position the Slots:**
   - **HatSlot:** Position at top of player head
   - **CloakSlot:** Position at upper back/shoulders
   - **BootsSlot:** Position at feet level
   - **WeaponSlot:** Position in right hand (or dominant hand bone if rigged)
   - **ReliquarySlot:** Position at waist/belt area

4. **Assign Slots in Inspector:**
   - Drag each empty GameObject into the corresponding slot field

5. **Usage in Code:**
   ```csharp
   // Get reference
   PlayerVisualEquipment visualEquipment = GetComponent<PlayerVisualEquipment>();
   
   // Equip gear (call when player rents/finds equipment)
   visualEquipment.EquipWeapon(wandData);
   visualEquipment.EquipHat(advancedHatData);
   visualEquipment.EquipCloak(basicCloakData);
   visualEquipment.EquipBoots(eliteBootsData);
   visualEquipment.EquipReliquary(basicReliquaryData);
   
   // Clear specific equipment
   visualEquipment.EquipWeapon(null); // Removes weapon visual
   
   // Clear all equipment (useful for death/reset)
   visualEquipment.ClearAllEquipment();
   ```

**How It Works:**
- When you call `EquipWeapon(weaponData)`, it automatically:
  1. Destroys the old weapon visual (if any)
  2. Instantiates the new weapon's `WeaponVisualPrefab`
  3. Parents it to the `WeaponSlot` transform
  4. Keeps a reference for later cleanup

**Tips:**
- Make sure your equipment visual prefabs have (0,0,0) local position/rotation
- If using rigged characters, parent slots to bones (e.g., WeaponSlot to RightHand bone)
- Test with placeholder cubes before creating final 3D models

---

### SpellManager.cs

**Purpose:** Manages spell slots, cantrips, mana, cooldowns, and spell casting logic.

**Setup Instructions:**

1. **Attach to Player GameObject:**
   - Select your Player GameObject
   - Add Component > Spell Manager

2. **Configure in Inspector:**
   
   **Spell Slots (Optional - can be set at runtime):**
   - Q Spell Slot: Assign a SpellData asset
   - E Spell Slot: Assign a SpellData asset
   - Shift Spell Slot: Assign a SpellData asset
   
   **Weapon & Cantrips (Optional - set at runtime):**
   - Equipped Weapon: Assign a WeaponData asset
   
   **Spell Cast Settings:**
   - Spell Cast Point: Transform where spells spawn (usually in front of player)
   - Global Cooldown Duration: 0.1 seconds (prevents spam)

3. **Create Spell Cast Point:**
   ```
   Player GameObject
     └── SpellCastPoint (Empty GameObject)
         - Position: 1-2 units in front of player at chest height
         - Rotation: Facing forward
   ```
   Drag `SpellCastPoint` into the inspector field.

4. **Usage in Code:**

   **Basic Spell Casting:**
   ```csharp
   SpellManager spellManager = GetComponent<SpellManager>();
   
   // Cast Q spell
   if (Input.GetKeyDown(KeyCode.Q))
   {
       spellManager.TryCastSpell(spellManager.QSpellSlot);
   }
   
   // Cast primary cantrip (left mouse button)
   if (Input.GetMouseButtonDown(0))
   {
       spellManager.TryCastCantrip(spellManager.PrimaryCantrip);
   }
   ```

   **Dynamic Spell Assignment (Level-Up System):**
   ```csharp
   // When player levels up and selects spell unlock upgrade
   void OnUpgradeSelected(UpgradeData upgrade)
   {
       if (upgrade.Type == UpgradeType.SpellUnlock)
       {
           // Assign to first available slot (0=Q, 1=E, 2=Shift)
           spellManager.AssignSpellToSlot(upgrade.SpellToUnlock, 0);
       }
   }
   ```

   **Equipment Integration:**
   ```csharp
   // When player equips a weapon
   void EquipWeapon(WeaponData weapon)
   {
       spellManager.EquipWeapon(weapon);
       // Now spellManager.PrimaryCantrip and SecondaryCantrip are updated
   }
   ```

   **Mana Management:**
   ```csharp
   // Set max mana (call when equipment changes)
   float totalManaBonus = hatData.ManaBonus + otherBonuses;
   spellManager.SetMaxMana(100f + totalManaBonus);
   
   // Regenerate mana over time (in Update)
   void Update()
   {
       spellManager.RegenerateMana(manaRegenRate * Time.deltaTime);
   }
   
   // Check if player can cast a spell
   if (spellManager.CanCastSpell(spellManager.QSpellSlot))
   {
       Debug.Log("Can cast Q spell!");
   }
   ```

   **UI Integration (Events):**
   ```csharp
   void Start()
   {
       // Subscribe to events
       spellManager.OnSpellCast += HandleSpellCast;
       spellManager.OnCantripCast += HandleCantripCast;
       spellManager.OnManaChanged += UpdateManaUI;
   }
   
   void HandleSpellCast(SpellData spell)
   {
       Debug.Log($"Cast spell: {spell.SpellName}");
       // Trigger spell animation, VFX, etc.
   }
   
   void HandleCantripCast(CantripData cantrip)
   {
       Debug.Log($"Cast cantrip: {cantrip.CantripName}");
   }
   
   void UpdateManaUI(float current, float max)
   {
       // Update mana bar
       manaBar.fillAmount = current / max;
       manaText.text = $"{current:F0} / {max:F0}";
   }
   
   void OnDestroy()
   {
       // Unsubscribe from events
       spellManager.OnSpellCast -= HandleSpellCast;
       spellManager.OnCantripCast -= HandleCantripCast;
       spellManager.OnManaChanged -= UpdateManaUI;
   }
   ```

   **Cooldown Display (for UI):**
   ```csharp
   void Update()
   {
       // Get cooldown remaining for Q spell
       float cooldown = spellManager.GetSpellCooldownRemaining(spellManager.QSpellSlot);
       
       if (cooldown > 0f)
       {
           qSpellCooldownText.text = cooldown.ToString("F1");
       }
       else
       {
           qSpellCooldownText.text = "";
       }
   }
   ```

**Public Properties:**
- `QSpellSlot`, `ESpellSlot`, `ShiftSpellSlot` - Current spell assignments
- `PrimaryCantrip`, `SecondaryCantrip` - From equipped weapon
- `CurrentMana`, `MaxMana` - Mana values (read-only)

**Public Methods:**
- `TryCastSpell(SpellData)` - Returns true if cast succeeded
- `TryCastCantrip(CantripData)` - Returns true if cast succeeded
- `CanCastSpell(SpellData)` - Check if spell is castable
- `IsSpellOnCooldown(SpellData)` - Check cooldown status
- `GetSpellCooldownRemaining(SpellData)` - Get seconds remaining
- `EquipWeapon(WeaponData)` - Set equipped weapon
- `AssignSpellToSlot(SpellData, int slotIndex)` - Assign spell to hotbar
- `SetMaxMana(float)` - Update max mana pool
- `AddMana(float)` - Add mana (clamped to max)
- `RegenerateMana(float)` - Same as AddMana (semantic clarity)

**Events:**
- `OnSpellCast(SpellData)` - Fired when spell is cast
- `OnCantripCast(CantripData)` - Fired when cantrip is cast
- `OnManaChanged(float current, float max)` - Fired when mana changes

**Tips:**
- Spell/cantrip prefabs are instantiated at `SpellCastPoint` position/rotation
- Make sure your spell prefabs have movement/damage logic components
- Use `OnSpellCast` event to trigger animations, screen shake, etc.
- The manager handles all mana/cooldown validation automatically

---

## Integration Example: Complete Player Setup

Here's how all the scripts and ScriptableObjects work together:

**1. Scene Setup (Player GameObject):**
```
Player
  ├── PlayerVisualEquipment (component)
  ├── SpellManager (component)
  ├── HatSlot (empty transform)
  ├── CloakSlot (empty transform)
  ├── BootsSlot (empty transform)
  ├── WeaponSlot (empty transform)
  ├── ReliquarySlot (empty transform)
  └── SpellCastPoint (empty transform)
```

**2. Gear Rental Phase (Start of Run):**
```csharp
public class GearRentalManager : MonoBehaviour
{
    [SerializeField] private PlayerVisualEquipment visualEquipment;
    [SerializeField] private SpellManager spellManager;
    
    public void OnGearSelected(WeaponData weapon, HatData hat, CloakData cloak, 
                                BootsData boots, ReliquaryData reliquary)
    {
        // Apply visuals
        visualEquipment.EquipWeapon(weapon);
        visualEquipment.EquipHat(hat);
        visualEquipment.EquipCloak(cloak);
        visualEquipment.EquipBoots(boots);
        visualEquipment.EquipReliquary(reliquary);
        
        // Apply weapon cantrips
        spellManager.EquipWeapon(weapon);
        
        // Apply stat bonuses
        float maxHealth = 100f + (cloak?.HealthBonus ?? 0f);
        float maxMana = 100f + (hat?.ManaBonus ?? 0f);
        float speedMultiplier = 1f * (boots?.SpeedMultiplier ?? 1f);
        float xpRadius = 5f + (reliquary?.XpCollectRadiusBonus ?? 0f);
        
        spellManager.SetMaxMana(maxMana);
        // Apply other stats to respective systems...
    }
}
```

**3. Level-Up Phase:**
```csharp
public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private UpgradeData[] availableUpgrades;
    
    public void OnUpgradeSelected(UpgradeData upgrade)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.HealthBoost:
                // Increase max health
                playerHealth.IncreaseMaxHealth(upgrade.Value);
                break;
                
            case UpgradeType.ManaBoost:
                float newMaxMana = spellManager.MaxMana + upgrade.Value;
                spellManager.SetMaxMana(newMaxMana);
                break;
                
            case UpgradeType.SpellUnlock:
                // Find first empty slot or let player choose
                int slot = FindEmptySpellSlot();
                spellManager.AssignSpellToSlot(upgrade.SpellToUnlock, slot);
                break;
                
            case UpgradeType.SpellDamageBoost:
                // Apply damage multiplier to player stats
                playerStats.spellDamageMultiplier += upgrade.Value;
                break;
                
            case UpgradeType.CantripDamageBoost:
                playerStats.cantripDamageMultiplier += upgrade.Value;
                break;
                
            case UpgradeType.CurrencyBonus:
                currencySystem.AddCurrency((int)upgrade.Value);
                break;
        }
    }
}
```

**4. Combat Phase:**
```csharp
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private SpellManager spellManager;
    
    void Update()
    {
        // Cantrips (weapon attacks)
        if (Input.GetMouseButton(0)) // Hold left click
        {
            spellManager.TryCastCantrip(spellManager.PrimaryCantrip);
        }
        
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            spellManager.TryCastCantrip(spellManager.SecondaryCantrip);
        }
        
        // Spells (abilities)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            spellManager.TryCastSpell(spellManager.QSpellSlot);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            spellManager.TryCastSpell(spellManager.ESpellSlot);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            spellManager.TryCastSpell(spellManager.ShiftSpellSlot);
        }
        
        // Mana regeneration
        spellManager.RegenerateMana(10f * Time.deltaTime); // 10 mana/sec
    }
}
```

---

## Asset Creation Checklist

## Visual Asset Requirements

### 2D Sprites (Icons)
You'll need icons for all the above assets:
- **Spell Icons:** 12 sprites
- **Cantrip Icons:** 6 sprites
- **Weapon Icons:** 3 sprites
- **Equipment Icons:** 12-20 sprites (hats, cloaks, boots, reliquaries)

**Total:** ~35-45 icon sprites

### 3D Prefabs
You'll need 3D models/prefabs for:
- **Weapon Models:** 3 prefabs (attach to player hand)
- **Hat Models:** 3-5 prefabs (attach to player head)
- **Cloak Models:** 3-5 prefabs (attach to player body)
- **Boots Models:** 3-5 prefabs (attach to player feet)
- **Reliquary Models:** 3-5 prefabs (attach to player waist)
- **Spell Effect Prefabs:** 12 prefabs (projectiles, AOE effects)
- **Cantrip Projectile Prefabs:** 6 prefabs
- **Enemy Prefabs:** 5 prefabs

**Total:** ~40-50 3D prefabs

---

## Tips & Best Practices

1. **Start Simple:** Create basic versions first, then add advanced/elite variants
2. **Naming Convention:** Use clear, descriptive names (e.g., "AdvancedHat" not "Hat2")
3. **Placeholder Assets:** Use Unity primitives (cubes, spheres) for initial testing
4. **Balance Later:** Don't worry about perfect stats initially, focus on getting all assets created
5. **Test Incrementally:** Test each system as you create assets (e.g., create 1 weapon first, test, then create others)
6. **Use Inspector Tooltips:** Hover over fields in the Inspector to see what each field does

---

## Quick Start Order

Recommended order for creating assets:

1. **Create Enemy Assets First** (needed for Wave Config)
   - Skeleton, Ranged Imp, Melee Imp, Golem, Necromonger

2. **Create Wave Config** (assign enemy references)
   - DefaultWaveConfig

3. **Create Cantrip Assets** (needed for Weapons)
   - 6 cantrip assets

4. **Create Weapon Assets** (assign cantrip references)
   - Wand, Staff, Tome

5. **Create Spell Assets**
   - 12 spell assets (can test without these initially)

6. **Create Equipment Assets**
   - Start with 1 of each type, expand later
   - BasicHat, BasicCloak, BasicBoots, BasicReliquary

---

**Last Updated:** 2024  
**For Questions:** Refer to Game Design Document in `/Assets/GameDesignDocument.md`
