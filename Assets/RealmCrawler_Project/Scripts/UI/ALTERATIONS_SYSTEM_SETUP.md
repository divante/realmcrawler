# Alterations System Setup Guide

## Overview
The Alterations system allows players to reroll secondary stat modifiers on equipment by spending souls. The system features:
- Weighted tier-based reroll mechanics
- Configurable upgrade chances (same tier, +1 tier, +2 tiers)
- Multi-select modifier rerolling
- Visual UI with toggles for each modifier

## Components Created

### 1. AlterationsSystem.cs
**Location:** `/Assets/RealmCrawler_Project/Scripts/UI/AlterationsSystem.cs`

**Classes:**
- `AlterationsConfig` - ScriptableObject configuration for the system
- `ModifierTier` - Defines value ranges and weights for modifier tiers
- `ModifierPool` - Collection of tiers for a specific stat type
- `AlterationsSystem` - Core logic for rerolling modifiers

**Key Features:**
- Tier-based randomization
- Weighted upgrade chances
- Dynamic cost calculation

### 2. Updated Files

#### NPCStationUIController.cs
Added Alterations panel support:
- Inventory list for player equipment
- Detail panel with modifier toggles
- Reroll button and cost display
- Integration with AlterationsSystem

#### SimpleStatModifierData.cs
Added internal setters:
- `SetStat(StatData)` - Set the stat reference
- `SetModifierType(ModifierType)` - Set flat/percent type
- `SetValue(float)` - Set modifier value
- `ModType` property exposed

#### StatModifierBase.cs
Changed `_stat` field from `private` to `protected` for access in derived classes.

#### EquipmentData.cs
Added modifier manipulation methods:
- `ReplaceModifier(int index, StatModifierBase newModifier)` - Replace a modifier at index
- `AddModifier(StatModifierBase modifier)` - Add new modifier
- `RemoveModifierAt(int index)` - Remove modifier by index

#### GameManager.cs
Added `ModifySouls(int amount)` method for spending/adding souls with clamping to 0.

## Setup Instructions

### Step 1: Create Alterations Config Asset

1. In Unity Project window, navigate to `/Assets/RealmCrawler_Project/ScriptableObjects/`
2. Create folder: `UI` (if it doesn't exist)
3. Right-click in the `UI` folder
4. Select **Create → RealmCrawler → UI → Alterations Config**
5. Name it: `SO_AlterationsConfig`

### Step 2: Configure Modifier Pools

You have **TWO OPTIONS** for setting up each tier:

#### **Option 1: Use Existing SimpleStatModifier Assets (RECOMMENDED)**

This is the easiest method - just drag your existing modifier assets!

1. Select `SO_AlterationsConfig`
2. In the Inspector, expand **Modifier Pools**
3. Add a new pool entry:
   - **Stat:** Drag your StatData asset (e.g., `Health.asset`)
   - **Modifier Type:** Select `Flat` or `Percent`
   - **Tiers:** Add tier entries from lowest to highest

For each tier:
- **Tier Name:** Name it (e.g., "Common", "Rare", "Epic")
- **Modifier Templates:** Drag existing SimpleStatModifier assets from `/Assets/RealmCrawler_Project/ScriptableObjects/Character/Stat/Modifier/`
  - Example: Drag `Health_+20.asset` into Tier 1
  - You can add multiple modifiers per tier - system picks one randomly

**Example: Health Modifier Pool (Using Templates)**

```
Pool 0:
  Stat: Health
  Modifier Type: Flat
  Tiers:
    Tier 0 (Common):
      Tier Name: Common
      Modifier Templates:
        - Health_+20.asset
      
    Tier 1 (Rare):
      Tier Name: Rare  
      Modifier Templates:
        - Health_+50.asset
        - Health_+60.asset
      
    Tier 2 (Epic):
      Tier Name: Epic
      Modifier Templates:
        - Health_+100.asset
```

#### **Option 2: Use Value Ranges**

If you don't have existing modifier assets or want procedural values:

1. Leave **Modifier Templates** empty
2. Set **Min Value** and **Max Value** for the tier
3. System will generate random values in that range

**Example: Health Modifier Pool (Using Ranges)**

```
Pool 0:
  Stat: Health
  Modifier Type: Flat
  Tiers:
    Tier 0 (Common):
      Tier Name: Common
      Modifier Templates: (empty)
      Min Value: 10
      Max Value: 20
    
    Tier 1 (Uncommon):
      Tier Name: Uncommon
      Modifier Templates: (empty)
      Min Value: 21
      Max Value: 35
    
    Tier 2 (Rare):
      Tier Name: Rare
      Modifier Templates: (empty)
      Min Value: 36
      Max Value: 50
```

**IMPORTANT:** The system checks for templates first. If templates exist, it ignores Min/Max values!

### Step 3: Configure Costs and Chances

In `SO_AlterationsConfig` Inspector:

**Costs:**
- **Reroll Cost Per Modifier:** `50` (souls per modifier selected)

**Upgrade Chances:**
- **Same Tier Chance:** `60%` (60% to stay in current tier)
- **Higher Tier Chance:** `30%` (+1 tier upgrade)
- **Much Higher Tier Chance:** `10%` (+2 tier upgrade)

**Note:** These percentages work as a cascade:
- 10% chance for +2 tiers
- 30% chance for +1 tier (if not +2)
- 60% chance for same tier (if neither above)

### Step 4: Assign Config to UI Controller

1. Find your NPC Station UI GameObject in the scene
2. Select the GameObject with `NPCStationUIController` component
3. In Inspector, find **Alterations Config** field
4. Drag `SO_AlterationsConfig` into this field

### Quick Setup Using Your Existing Modifiers

Based on your existing modifiers in `/Assets/RealmCrawler_Project/ScriptableObjects/Character/Stat/Modifier/`:

**Health Pool (Flat):**
- Tier 0: `Health_+20.asset`
- Tier 1: (create new or use value range 30-50)

**Health Pool (Percent):**
- Tier 0: `Health_+5%.asset`
- Tier 1: (create new or use value range 7-12)

**Mana Pool (Flat):**
- Tier 0: `Mana_+20.asset`
- Tier 1: (create new or use value range 30-50)

**Mana Pool (Percent):**
- Tier 0: `Mana_+5%.asset`
- Tier 1: (create new or use value range 7-12)

**Fire Damage Pool (Flat):**
- Tier 0: `Fire_Damage+5.asset`
- Tier 1: (create new or use value range 10-20)

**Fire Damage Pool (Percent):**
- Tier 0: `Fire_Damage+5%.asset`
- Tier 1: (create new or use value range 7-15)

And so on for Lightning and Eldritch damage types!

## Usage Workflow

### Player Perspective

1. Player opens NPC Station
2. Clicks **"Alterations"** button
3. Sees their inventory on the left (equipment only)
4. Selects an item
5. Right panel shows:
   - Item name and icon
   - List of modifiers with toggles
   - Cost display
   - Reroll button
6. Player toggles modifiers to reroll
7. Cost updates dynamically (50 souls × number selected)
8. Clicks **"REROLL SELECTED"**
9. System:
   - Deducts souls
   - Rerolls each selected modifier based on tier chances
   - Updates item with new modifiers
   - Refreshes UI

### Example Reroll Scenario

**Item:** Proto Cloak II
**Current Modifiers:**
- Health +20 (Tier 1: Uncommon)
- Health +5% (Tier 0: Common)

**Player Action:**
1. Toggles both modifiers
2. Cost: 100 souls (2 × 50)
3. Clicks REROLL

**Possible Outcomes:**

**Health +20 (currently Tier 1):**
- 60% → Stays Tier 1: Rerolls to 21-35
- 30% → Upgrades to Tier 2: Rolls 36-50
- 10% → Upgrades to Tier 3: Rolls 51-75

**Health +5% (currently Tier 0):**
- 60% → Stays Tier 0: Rerolls to 3%-7%
- 30% → Upgrades to Tier 1: Rolls 8%-15%
- 10% → Upgrades to Tier 2: Rolls 16%-25%

## Technical Details

### Modifier Tier Detection

The system automatically detects a modifier's current tier by matching its value against tier ranges in the config.

### Reroll Algorithm

```
1. Find modifier pool for stat + type
2. Determine current tier index by value
3. Roll for tier upgrade:
   - 0-10: +2 tiers (clamped to max)
   - 11-40: +1 tier (clamped to max)
   - 41-100: Same tier
4. Select random value within new tier range
5. Create new modifier instance with new value
6. Replace old modifier in equipment
```

### Important Notes

- **Modifiers are created at runtime** - New modifier instances are created during reroll
- **Original modifiers are not modified** - Equipment gets new instances
- **Equipment must be in player inventory** - Only owned items can be altered
- **Static modifiers cannot be rerolled** - Only items in the `modifiers` list

## Extending the System

### Adding New Stat Types

1. Create `StatData` asset for the stat (e.g., `EldritchDamage.asset`)
2. Add modifier pool in `SO_AlterationsConfig`
3. Define tiers with value ranges
4. Modifiers using this stat will automatically be rerollable

### Custom Upgrade Chances per Pool

Currently, all pools share the same upgrade chances. To make pool-specific chances:

1. Add `sameTierChance`, `higherTierChance`, `muchHigherTierChance` fields to `ModifierPool` class
2. Update `AlterationsSystem.RollNewTierIndex()` to accept pool-specific chances
3. Update config UI to show per-pool settings

### Preventing Downgrades

Current system only upgrades or maintains tier. To allow downgrades:

1. Add `lowerTierChance` to `AlterationsConfig`
2. Update `RollNewTierIndex()` to handle negative tier changes
3. Adjust probability distribution

## Troubleshooting

### "No modifier pool found for stat"
**Cause:** The modifier's stat or type doesn't have a matching pool in config.
**Solution:** Add a modifier pool for that stat + type combination.

### "Could not determine tier for modifier value"
**Cause:** Modifier value doesn't fall within any tier's min/max range.
**Solution:** Check tier ranges have no gaps and cover the modifier's value.

### Reroll button disabled
**Cause:** No modifiers selected or item has no rerollable modifiers.
**Solution:** Ensure item has modifiers in the list and toggle at least one.

### Souls not deducting
**Cause:** GameManager reference missing or souls calculation error.
**Solution:** Check NPCStationUIController has gameManager reference in scene.

## Future Enhancements

### Suggested Features
1. **Lock Modifier**: Allow players to lock one modifier while rerolling others
2. **Guaranteed Upgrade**: Special currency for guaranteed tier upgrades
3. **Reroll History**: Track what modifiers were before reroll
4. **Preset Tiers**: Quick-create common tier configurations
5. **Modifier Preview**: Show possible outcomes before committing
6. **Batch Reroll**: Reroll all modifiers on multiple items at once
7. **Tier Visualization**: Color-code modifiers by tier in UI
8. **Sound Effects**: Audio feedback for reroll success/tier upgrade
9. **Particle Effects**: Visual effect when modifier upgrades tier
10. **Alteration Statistics**: Track total rerolls, average upgrades, etc.

## API Reference

### AlterationsSystem

```csharp
public class AlterationsSystem
{
    // Constructor
    public AlterationsSystem(AlterationsConfig config)
    
    // Reroll a modifier and return new instance
    public StatModifierBase RerollModifier(StatModifierBase currentModifier)
    
    // Calculate cost for rerolling N modifiers
    public int CalculateRerollCost(int modifierCount)
}
```

### AlterationsConfig

```csharp
[CreateAssetMenu(fileName = "AlterationsConfig", menuName = "RealmCrawler/UI/Alterations Config")]
public class AlterationsConfig : ScriptableObject
{
    public int RerollCostPerModifier { get; }
    public float SameTierChance { get; }
    public float HigherTierChance { get; }
    public float MuchHigherTierChance { get; }
    public IReadOnlyList<ModifierPool> ModifierPools { get; }
}
```

### ModifierPool

```csharp
public class ModifierPool
{
    public StatData stat;
    public ModifierType modifierType;
    public List<ModifierTier> tiers;
}
```

### ModifierTier

```csharp
public class ModifierTier
{
    public string tierName;
    public float minValue;
    public float maxValue;
    public int weight; // Currently unused, for future weighted selection
}
```
