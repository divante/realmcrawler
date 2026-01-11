# Quick Start: Alterations System Setup

## ⚡ 5-Minute Setup Guide

### Step 1: Create the Config Asset (30 seconds)

1. Navigate to `/Assets/RealmCrawler_Project/ScriptableObjects/Core/`
2. You already have `AlterationsConfig.asset` there! ✓

### Step 2: Configure Your First Pool (2 minutes)

Select `AlterationsConfig.asset` and add a pool:

```
Modifier Pools (Size: 1)
  ├── Element 0
      ├── Stat: [Drag Health StatData here]
      ├── Modifier Type: Flat
      └── Tiers (Size: 2)
          ├── Element 0 (Common Tier)
          │   ├── Tier Name: "Common"
          │   ├── Modifier Templates (Size: 1)
          │   │   └── Element 0: [Drag Health_+20.asset]
          │   ├── Min Value: 10  (ignored when templates exist)
          │   ├── Max Value: 20  (ignored when templates exist)
          │   └── Weight: 100
          │
          └── Element 1 (Rare Tier)
              ├── Tier Name: "Rare"
              ├── Modifier Templates (Size: 0) - leave empty to use range
              ├── Min Value: 30
              ├── Max Value: 50
              └── Weight: 100
```

**What this means:**
- When rerolling a Health +20 modifier (Tier 0):
  - 60% chance → Stays Tier 0, gets `Health_+20` value (20)
  - 30% chance → Upgrades to Tier 1, gets random 30-50
  - 10% chance → Upgrades to Tier 2 (if exists)

### Step 3: Set Costs and Chances (30 seconds)

Still in `AlterationsConfig.asset`:

```
Costs:
  ├── Reroll Cost Per Modifier: 50

Upgrade Chances:
  ├── Same Tier Chance: 60
  ├── Higher Tier Chance: 30
  └── Much Higher Tier Chance: 10
```

### Step 4: Hook Up to UI (1 minute)

1. Open your NPC Station scene
2. Find the GameObject with `NPCStationUIController`
3. In Inspector:
   ```
   NPC Station UI Controller
     ├── ...
     └── Alterations Config: [Drag AlterationsConfig.asset here]
   ```

### Step 5: Test! (1 minute)

1. Make sure you have equipment with modifiers in player inventory
2. Enter Play mode
3. Open NPC Station
4. Click "Alterations" button
5. Select an item with modifiers
6. Toggle modifier(s)
7. Click "REROLL SELECTED"

---

## 📋 Template Approach vs Range Approach

### Use Templates When:
✅ You have existing SimpleStatModifier assets  
✅ You want exact, predefined values  
✅ You want to reuse modifiers across multiple systems  
✅ You want quick setup (just drag and drop)  

**Example:**
```
Tier 0: Health_+20.asset, Health_+25.asset
Tier 1: Health_+40.asset, Health_+45.asset, Health_+50.asset
```

### Use Ranges When:
✅ You want procedural/random values  
✅ You don't have existing modifier assets  
✅ You want fine control over value distribution  
✅ You want continuous value ranges  

**Example:**
```
Tier 0: Min=10, Max=25
Tier 1: Min=26, Max=50
```

---

## 🎮 Example Setup Using Your Existing Modifiers

Here's how to set up pools using what you already have:

### Pool 1: Health (Flat)
```
Stat: Health
Type: Flat
Tiers:
  [0] Common
      Templates: Health_+20.asset
  [1] Rare
      Range: 40-70
  [2] Epic  
      Range: 80-120
```

### Pool 2: Health (Percent)
```
Stat: Health
Type: Percent
Tiers:
  [0] Common
      Templates: Health_+5%.asset
  [1] Rare
      Range: 8-12
  [2] Epic
      Range: 15-25
```

### Pool 3: Mana (Flat)
```
Stat: Mana
Type: Flat
Tiers:
  [0] Common
      Templates: Mana_+20.asset
  [1] Rare
      Range: 35-60
```

### Pool 4: Fire Damage (Flat)
```
Stat: Fire Damage
Type: Flat
Tiers:
  [0] Common
      Templates: Fire_Damage+5.asset
  [1] Rare
      Range: 10-18
```

---

## 🔧 Troubleshooting

**"No modifier pool found for stat X"**
- Add a pool for that stat + type combination
- Make sure Stat field is assigned
- Check modifier type matches (Flat vs Percent)

**"Could not determine tier for modifier value Y"**  
Using templates:
- Make sure the modifier's exact value matches a template value
- Check for floating point precision (20.0 vs 20.001)

Using ranges:
- Ensure tier ranges cover the modifier's value
- Check for gaps between tier ranges

**Reroll button is disabled**
- Select an item with modifiers
- Toggle at least one modifier checkbox
- Ensure item is equipment (not weapons for now)

**Templates show in Inspector but values look wrong**
- System uses template VALUE, not the template itself
- Check the template asset's Value field is correct

---

## 💡 Pro Tips

### Mix Templates and Ranges
You can use templates for some tiers and ranges for others!

```
Tiers:
  [0] Common - Uses: Health_+20.asset (exact value: 20)
  [1] Rare   - Range: 40-60 (random between)
  [2] Epic   - Uses: Health_+100.asset (exact value: 100)
```

### Multiple Templates Per Tier
Add variety by including multiple templates:

```
Tier 0:
  Templates:
    - Health_+20.asset
    - Health_+22.asset  
    - Health_+25.asset
```
System randomly picks one each reroll!

### Create New Modifiers for Higher Tiers
Your existing modifiers are tier 0. Create stronger ones:

1. Right-click `/ScriptableObjects/Character/Stat/Modifier/`
2. Create → RealmCrawler → Characters → Simple Stat Modifier
3. Name it: `Health_+50.asset`
4. Set Value: 50
5. Drag into Tier 1 templates

---

## Next Steps

1. ✅ Set up one pool to test
2. ✅ Test with existing equipment  
3. ✅ Add more pools for other stats
4. ✅ Create higher-tier modifier assets
5. ✅ Tune costs and probabilities based on gameplay

For detailed info, see `ALTERATIONS_SYSTEM_SETUP.md`
