# Equipment & Weapon Configuration Guide (Updated)

## 📋 **Overview**

Your equipment and weapons now have a **complete item system** with:
- ✅ **Auto-generated GUIDs** for performance and unique identification
- ✅ **Rarity tiers** (Common → Legendary)
- ✅ **Stat modifiers** (static + array of secondary modifiers)
- ✅ **Spell integration** for equipment
- ✅ **Full metadata** (name, flavor text, icons)

This system uses the same GUID auto-generation pattern as your `StatData` ScriptableObjects for optimal performance!

---

## 🎯 **How GUID Auto-Generation Works**

### **The Magic Behind the Scenes**

When you create a new equipment or weapon ScriptableObject:

1. **You create the asset** → Right-click → Create → RealmCrawler → Equipment → [Type]
2. **Unity saves the file** → Asset gets a file path like `/ScriptableObjects/Weapons/Staff/SO_FireStaff.asset`
3. **Custom Editor triggers** → `OnEnable()` detects it's a new asset
4. **Auto-fill happens:**
   - **Item Name** ← Pulled from file name (`SO_FireStaff` → `SO_FireStaff`)
   - **Item ID (GUID)** ← `Guid.NewGuid()` generates unique ID

**Result:** Every item gets a guaranteed-unique GUID that's **fast to compare** (way faster than string names)!

### **Why GUIDs Are Better**

```csharp
// ❌ Slow string comparison
if (item.ItemName == "Fire Staff") { }

// ✅ Fast GUID comparison (just comparing two numbers)
if (item.ItemId == targetItemId) { }
```

**Performance difference:**
- String comparison: ~100-200 nanoseconds
- GUID comparison: ~10-20 nanoseconds
- **10x faster!** 🚀

### **Using GUIDs in Your Code**

```csharp
// Store item references by GUID
Dictionary<Guid, EquipmentData> equipmentLookup = new();

// Fast lookup
EquipmentData item = equipmentLookup[itemGuid];

// Compare items
if (item1.ItemId == item2.ItemId)
{
    // Same item!
}
```

---

## 📦 **Equipment Structure (Complete)**

### **Equipment Fields:**

```
IDENTIFICATION:
├── Item ID (Guid)         ← Auto-generated, unique identifier
├── Item Name (string)     ← Auto-set from filename, editable
├── Flavor Text (string)   ← Lore/description for players
└── Item Icon (Sprite)     ← Visual icon

CLASSIFICATION:
├── Category (enum)        ← Hat, Cloak, Boots, Reliquary
└── Rarity (enum)          ← Common, Uncommon, Rare, Epic, Legendary

SPELL:
└── Spell (SpellData)      ← Optional spell granted by this equipment

STAT MODIFIERS:
├── Static Modifier        ← Primary/guaranteed modifier
└── Modifiers (List)       ← Array of secondary/random modifiers

RENTAL SYSTEM:
└── Rental Cost (int)      ← Gold cost to rent for one run

VISUALS:
└── Visual Prefab (GameObject) ← 3D model for character
```

### **Weapon Fields:**

```
IDENTIFICATION:
├── Item ID (Guid)            ← Auto-generated, unique
├── Item Name (string)        ← Auto-set from filename
├── Flavor Text (string)      ← Lore description
└── Item Icon (Sprite)        ← Visual icon

CLASSIFICATION:
└── Rarity (enum)             ← Common → Legendary

CANTRIPS:
├── Primary Cantrip           ← Main attack cantrip
└── Secondary Cantrip         ← Alternate/special cantrip

STAT MODIFIERS:
├── Static Modifier           ← Guaranteed weapon modifier
└── Secondary Modifiers (List) ← Additional modifiers

LEGACY STATS:
├── Damage Multiplier (float)
├── Buffed Element (enum)
└── Element Damage Bonus (float)

RENTAL SYSTEM:
└── Rental Cost (int)

VISUALS:
└── Weapon Visual Prefab (GameObject)
```

---

## 🎨 **Rarity System**

### **Rarity Tiers:**

```csharp
public enum ItemRarity
{
    Common,      // White/Gray
    Uncommon,    // Green
    Rare,        // Blue
    Epic,        // Purple
    Legendary    // Orange/Gold
}
```

### **Suggested Rarity Guidelines:**

**Common:**
- 1 static modifier OR 1-2 small modifiers
- Low stat bonuses (+5 Health, +5% Speed)
- Rental: 10-50 gold

**Uncommon:**
- 1 static + 1 secondary modifier
- Medium bonuses (+10-15 Health, +10% Speed)
- Rental: 50-150 gold

**Rare:**
- 1 static + 2-3 secondary modifiers
- Good bonuses (+20-30 Health, +15% Speed)
- May include spell
- Rental: 150-300 gold

**Epic:**
- 1 strong static + 3-4 secondary modifiers
- Large bonuses (+40-50 Health, +20% Speed)
- Often includes spell
- Rental: 300-600 gold

**Legendary:**
- 1 powerful static + 4-5 secondary modifiers
- Huge bonuses (+75-100 Health, +30% Speed)
- Always includes powerful spell
- Unique effects
- Rental: 600-1000+ gold

---

## 📝 **Step-by-Step: Creating Equipment**

### **STEP 1: Create Core Stats** (One-time setup)

**Location:** `/Assets/RealmCrawler_Project/ScriptableObjects/Character/Stat/`

Right-click → Create → RealmCrawler → Characters → Stat

Create these:
- `SO_Stat_Health` (Base: 100)
- `SO_Stat_Mana` (Base: 50)
- `SO_Stat_Speed` (Base: 5)
- `SO_Stat_Damage` (Base: 1)
- `SO_Stat_Defense` (Base: 0)
- `SO_Stat_CritChance` (Base: 0.05)

---

### **STEP 2: Create Stat Modifiers** (Reusable library)

**Location:** `/Assets/RealmCrawler_Project/ScriptableObjects/Character/Stat/Modifier/`

Right-click → Create → RealmCrawler → Characters → Simple Stat Modifier

**Create a modifier library:**

```
Health Modifiers:
├── SO_Mod_Health_Plus10     (Stat: Health, Type: Flat, Value: 10)
├── SO_Mod_Health_Plus20     (Stat: Health, Type: Flat, Value: 20)
├── SO_Mod_Health_Plus50     (Stat: Health, Type: Flat, Value: 50)
└── SO_Mod_Health_Plus100    (Stat: Health, Type: Flat, Value: 100)

Speed Modifiers:
├── SO_Mod_Speed_Plus5Pct    (Stat: Speed, Type: Percent, Value: 0.05)
├── SO_Mod_Speed_Plus10Pct   (Stat: Speed, Type: Percent, Value: 0.10)
└── SO_Mod_Speed_Plus15Pct   (Stat: Speed, Type: Percent, Value: 0.15)

Damage Modifiers:
├── SO_Mod_Damage_Plus10Pct  (Stat: Damage, Type: Percent, Value: 0.10)
├── SO_Mod_Damage_Plus15Pct  (Stat: Damage, Type: Percent, Value: 0.15)
└── SO_Mod_Damage_Plus20Pct  (Stat: Damage, Type: Percent, Value: 0.20)
```

---

### **STEP 3: Create Equipment**

**Example: Common Health Hat**

**Location:** `/ScriptableObjects/Equipment/Hats/`

1. Right-click → Create → RealmCrawler → Equipment → Hat Data
2. Name: `SO_Hat_LeatherCap_Common`
3. Configure:

```
IDENTIFICATION:
├── Item ID: (auto-generated GUID)
├── Item Name: "Leather Cap"  (you can rename from filename)
├── Flavor Text: "A simple leather cap. Better than nothing."
└── Item Icon: [Drag hat icon sprite]

CLASSIFICATION:
├── Category: Hat (auto-set)
└── Rarity: Common

SPELL: None

STAT MODIFIERS:
├── Static Modifier: SO_Mod_Health_Plus10
└── Modifiers: (Size: 0) - leave empty for common

RENTAL SYSTEM:
└── Rental Cost: 25

VISUALS:
└── Visual Prefab: [Drag leather cap model]
```

**Example: Rare Multi-Stat Boots**

**Location:** `/ScriptableObjects/Equipment/Boots/`

1. Create → Equipment → Boots Data
2. Name: `SO_Boots_Swiftrunner_Rare`
3. Configure:

```
IDENTIFICATION:
├── Item Name: "Swiftrunner Boots"
├── Flavor Text: "Enchanted boots worn by legendary scouts."
└── Item Icon: [Boots icon]

CLASSIFICATION:
├── Category: Boots
└── Rarity: Rare

SPELL: [Drag a dash/teleport spell if you have one]

STAT MODIFIERS:
├── Static Modifier: SO_Mod_Speed_Plus15Pct
└── Modifiers: (Size: 2)
    ├── Element 0: SO_Mod_Health_Plus20
    └── Element 1: SO_Mod_Defense_Plus5Pct

RENTAL COST: 200
```

---

### **STEP 4: Create Weapons**

**Example: Epic Fire Staff**

**Location:** `/ScriptableObjects/Weapons/Staff/`

1. Right-click → Create → RealmCrawler → Equipment → Weapon Data
2. Name: `SO_Weapon_InfernoStaff_Epic`
3. Configure:

```
IDENTIFICATION:
├── Item ID: (auto-generated)
├── Item Name: "Inferno Staff"
├── Flavor Text: "A staff blazing with eternal fire. Its flames never die."
└── Item Icon: [Fire staff icon]

CLASSIFICATION:
└── Rarity: Epic

CANTRIPS:
├── Primary Cantrip: [Your fireball cantrip]
└── Secondary Cantrip: [Your fire AOE cantrip]

STAT MODIFIERS:
├── Static Modifier: SO_Mod_Damage_Plus20Pct
└── Secondary Modifiers: (Size: 3)
    ├── Element 0: SO_Mod_Mana_Plus25
    ├── Element 1: SO_Mod_Health_Plus30
    └── Element 2: SO_Mod_CritChance_Plus5Pct

LEGACY STATS:
├── Damage Multiplier: 1.3
├── Buffed Element: Fire
└── Element Damage Bonus: 0.25 (25% fire damage bonus)

RENTAL COST: 500

VISUALS:
└── Weapon Visual Prefab: [Fire staff model]
```

---

## 🔧 **Using Equipment in Code**

### **Equipping Items:**

```csharp
using RealmCrawler.Equipment;
using RealmCrawler.Characters;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private CharacterData playerStats;
    
    private EquipmentData currentHelmet;
    
    public void EquipItem(EquipmentData equipment)
    {
        // Unequip old item
        if (currentHelmet != null)
        {
            currentHelmet.RemoveModifiers(playerStats);
        }
        
        // Equip new item
        currentHelmet = equipment;
        equipment.ApplyModifiers(playerStats);
        
        Debug.Log($"Equipped: {equipment.ItemName} (Rarity: {equipment.Rarity})");
        Debug.Log($"Item ID: {equipment.ItemId}");
    }
}
```

### **Comparing Items:**

```csharp
// Fast GUID comparison
if (item1.ItemId == item2.ItemId)
{
    Debug.Log("Same item!");
}

// Check rarity
if (equipment.Rarity >= ItemRarity.Rare)
{
    Debug.Log("This is a rare or better item!");
}
```

### **Accessing Modifiers:**

```csharp
// Get the primary modifier
if (equipment.StaticModifier != null)
{
    Debug.Log($"Static Modifier: {equipment.StaticModifier.name}");
}

// Loop through secondary modifiers
foreach (var modifier in equipment.Modifiers)
{
    Debug.Log($"Modifier: {modifier.name}");
}
```

---

## 🎨 **Rarity Color Coding (Recommended)**

Add this to your UI code:

```csharp
public static Color GetRarityColor(ItemRarity rarity)
{
    return rarity switch
    {
        ItemRarity.Common => new Color(0.7f, 0.7f, 0.7f),      // Gray
        ItemRarity.Uncommon => new Color(0.1f, 0.9f, 0.1f),    // Green
        ItemRarity.Rare => new Color(0.1f, 0.5f, 1f),          // Blue
        ItemRarity.Epic => new Color(0.7f, 0.2f, 1f),          // Purple
        ItemRarity.Legendary => new Color(1f, 0.6f, 0f),       // Orange
        _ => Color.white
    };
}
```

---

## ⚠️ **Backwards Compatibility**

Your existing code using old property names will still work but show **obsolete warnings**:

**Old Code (still works):**
```csharp
string name = equipment.EquipmentName;  // ⚠️ Warning: Use ItemName instead
Sprite icon = equipment.Icon;            // ⚠️ Warning: Use ItemIcon instead
string desc = equipment.Description;     // ⚠️ Warning: Use FlavorText instead
```

**New Code (recommended):**
```csharp
string name = equipment.ItemName;        // ✅ No warning
Sprite icon = equipment.ItemIcon;        // ✅ No warning
string desc = equipment.FlavorText;      // ✅ No warning
Guid id = equipment.ItemId;              // ✅ New GUID system
ItemRarity rarity = equipment.Rarity;    // ✅ New rarity system
```

**To fix warnings:** Search and replace in your IDE:
- `EquipmentName` → `ItemName`
- `.Icon` → `.ItemIcon`
- `.Description` → `.FlavorText`
- `WeaponName` → `ItemName`

---

## 🚀 **Quick Start Checklist**

- [ ] **1. Create core stats** (Health, Mana, Speed, Damage, Defense)
- [ ] **2. Create modifier library** (10-15 common modifiers)
- [ ] **3. Create one equipment of each rarity** (test the system)
- [ ] **4. Create one weapon** (test cantrips + modifiers)
- [ ] **5. Test equipping in-game** (verify stats apply correctly)
- [ ] **6. (Optional) Update old code** to use new property names

---

## 📚 **Related Files**

- `EquipmentData.cs` - Base equipment class with GUID system
- `WeaponData.cs` - Weapon-specific data with GUID system
- `CharacterData.cs` - Manages character stats
- `StatData.cs` - Individual stat definitions (uses same GUID pattern)
- `StatModifierBase.cs` - Base modifier class
- `SimpleStatModifier.cs` - Flat/Percent modifiers

---

**Questions? The system is ready to use!** 🎉
