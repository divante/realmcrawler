# Equipment System Setup Guide

## Overview

The equipment system allows players to rent and equip gear before starting a run. Equipment is **locked during gameplay** - no swapping allowed once the run begins.

---

## Equipment Types & Stats

| Equipment Type | Stat Bonus | Default Value |
|---------------|------------|---------------|
| **Hat** | Mana Bonus | +20 mana |
| **Cloak** | Health Bonus | +50 health |
| **Boots** | Speed Multiplier | x1.3 (30% faster) |
| **Reliquary** | XP Collect Radius | +3 units |
| **Weapon** | Damage Multiplier & Element Buff | Varies by weapon |

---

## Player Setup

### 1. Add Components to Player GameObject

Your player needs these components:

```
Player (GameObject)
├── EquipmentManager        (NEW - manages loadout & stats)
├── PlayerVisualEquipment   (existing - handles visuals)
├── CharacterPhysics        (existing - movement)
├── SpellManager            (existing - spells & weapons)
└── PlayerController        (existing - input)
```

### 2. Create Equipment Slots on Player Model

Add empty GameObjects as attachment points for equipment:

```
Player
├── PlayerModel
├── Armature
│   ├── Hips
│   │   ├── Spine
│   │   │   └── Chest
│   │   │       ├── Slot_Cloak     (Empty GameObject)
│   │   │       ├── Neck
│   │   │       │   └── Head
│   │   │       │       └── Slot_Hat (Empty GameObject)
│   │   │       └── Shoulder.R
│   │   │           └── Arm.R
│   │   │               └── Hand.R
│   │   │                   └── Slot_Weapon (Empty GameObject)
│   │   └── Slot_Boots (Empty GameObject at hips)
│   └── Slot_Reliquary (Empty GameObject at hips/belt)
```

### 3. Configure EquipmentManager Inspector

Select the Player GameObject and configure `EquipmentManager`:

#### Current Equipment Loadout:
- **Equipped Hat:** Drag `SO_Proto_Hat` (for testing)
- **Equipped Cloak:** Drag `SO_Proto_Cloak`
- **Equipped Boots:** Drag `SO_Proto_Boots`
- **Equipped Weapon:** Drag `SO_Weapon_Proto_Wand`
- **Equipped Reliquary:** Drag `SO_Proto_Reliquary`

#### Base Stats:
- **Base Health:** `100`
- **Base Mana:** `100`
- **Base Move Speed:** `10` (should match CharacterPhysics default)
- **Base XP Radius:** `5`

#### References (Auto-assigned if on same GameObject):
- **Visual Equipment:** `PlayerVisualEquipment` component
- **Character Physics:** `CharacterPhysics` component
- **Spell Manager:** `SpellManager` component

### 4. Configure PlayerVisualEquipment Inspector

Assign the equipment slots you created:

- **Hat Slot:** Drag `Slot_Hat` GameObject
- **Cloak Slot:** Drag `Slot_Cloak`
- **Boots Slot:** Drag `Slot_Boots`
- **Weapon Slot:** Drag `Slot_Weapon`
- **Reliquary Slot:** Drag `Slot_Reliquary`

---

## How It Works

### At Run Start (Automatic):

1. **EquipmentManager.Start()** calls `ApplyEquipmentLoadout()`
2. **Calculate Stats:**
   - Base Health (100) + Cloak Bonus (50) = **150 Max Health**
   - Base Mana (100) + Hat Bonus (20) = **120 Max Mana**
   - Base Speed (10) × Boots Multiplier (1.3) = **13 Move Speed**
   - Base XP Radius (5) + Reliquary Bonus (3) = **8 XP Radius**

3. **Apply Visuals:**
   - Instantiate equipment prefabs at attachment slots
   - Hat appears on head, cloak on chest, boots on hips, etc.

4. **Apply Weapon:**
   - Set weapon in SpellManager
   - Enable cantrip casting with weapon bonuses

5. **Apply Movement:**
   - Update CharacterPhysics moveSpeed to calculated value

### During Gameplay:

- Equipment **cannot be changed**
- Stats remain constant for entire run
- Visual equipment stays attached

### In Menu Scene (Future):

Your menu system will call:

```csharp
equipmentManager.SetEquipment(hat, cloak, boots, weapon, reliquary);
```

This updates the loadout that will be applied when the run starts.

---

## Testing Equipment

### Method 1: Inspector Testing

1. Open player prefab or scene
2. Select Player GameObject
3. In `EquipmentManager` Inspector, assign different equipment SOs
4. Enter Play Mode
5. Equipment visuals and stats automatically apply

### Method 2: Runtime Testing (via Debug)

Create a simple test script:

```csharp
using UnityEngine;
using RealmCrawler.Equipment;

public class EquipmentTester : MonoBehaviour
{
    public EquipmentManager equipmentManager;
    
    public HatData testHat;
    public CloakData testCloak;
    public BootsData testBoots;
    public WeaponData testWeapon;
    public ReliquaryData testReliquary;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            equipmentManager.SetEquipment(testHat, testCloak, testBoots, testWeapon, testReliquary);
            equipmentManager.ApplyEquipmentLoadout();
            Debug.Log($"Equipment applied! Max Health: {equipmentManager.MaxHealth}, Move Speed: {equipmentManager.MoveSpeed}");
        }
    }
}
```

---

## Integration with Menu Scene (Future)

When you create your menu scene, you'll:

1. **Display available equipment** (query from ScriptableObject assets)
2. **Show rental costs** (from `rentalCost` field)
3. **Let player select equipment** per slot
4. **On "Start Run":**
   ```csharp
   equipmentManager.SetEquipment(selectedHat, selectedCloak, selectedBoots, selectedWeapon, selectedReliquary);
   ```
5. **Load game scene** - EquipmentManager persists and applies loadout

### Option A: DontDestroyOnLoad

Make EquipmentManager persist:

```csharp
void Awake()
{
    DontDestroyOnLoad(gameObject);
    // ... rest of code
}
```

### Option B: Save/Load Loadout

Save equipment IDs to a data manager, then load in game scene.

---

## Events for UI Updates

EquipmentManager exposes UnityEvents:

```csharp
equipmentManager.OnHatEquipped.AddListener(UpdateHatUI);
equipmentManager.OnCloakEquipped.AddListener(UpdateCloakUI);
equipmentManager.OnBootsEquipped.AddListener(UpdateBootsUI);
equipmentManager.OnWeaponEquipped.AddListener(UpdateWeaponUI);
equipmentManager.OnReliquaryEquipped.AddListener(UpdateReliquaryUI);
```

Use these in your menu to update UI when equipment changes.

---

## API Reference

### EquipmentManager Public Methods:

```csharp
ApplyEquipmentLoadout()
```
Recalculates stats, applies visuals, weapon, and movement speed.

```csharp
SetEquipment(HatData hat, CloakData cloak, BootsData boots, WeaponData weapon, ReliquaryData reliquary)
```
Sets all equipment at once and invokes all events.

```csharp
SetHat(HatData hat)
SetCloak(CloakData cloak)
SetBoots(BootsData boots)
SetWeapon(WeaponData weapon)
SetReliquary(ReliquaryData reliquary)
```
Set individual equipment pieces.

### EquipmentManager Public Properties:

```csharp
float MaxHealth { get; }
float MaxMana { get; }
float MoveSpeed { get; }
float XpRadius { get; }

HatData EquippedHat { get; }
CloakData EquippedCloak { get; }
BootsData EquippedBoots { get; }
WeaponData EquippedWeapon { get; }
ReliquaryData EquippedReliquary { get; }
```

---

## Checklist

- [ ] EquipmentManager component added to Player
- [ ] PlayerVisualEquipment configured with all 5 slots
- [ ] Equipment slot GameObjects created on player armature
- [ ] Base stats configured in EquipmentManager
- [ ] Prototype equipment assigned for testing
- [ ] Test in Play Mode - equipment appears visually
- [ ] Verify stats are calculated correctly
- [ ] Verify movement speed changes with boots

---

## Next Steps

1. **Create player character** with proper armature and slots
2. **Test equipment system** with prototype assets
3. **Design menu scene** for equipment rental/selection
4. **Implement persistence** between menu and game scenes
5. **Create additional equipment variations** with different stats
6. **Add cloth physics to cloaks** for visual polish

---

## Troubleshooting

**Equipment doesn't appear visually:**
- Check that `PlayerVisualEquipment` has all slots assigned
- Verify equipment ScriptableObjects have `visualPrefab` assigned
- Make sure slot GameObjects are in correct hierarchy position

**Stats not applying:**
- Check `EquipmentManager` references are assigned
- Verify `ApplyEquipmentLoadout()` is called
- Use Debug.Log to check calculated stat values

**Movement speed unchanged:**
- Verify `CharacterPhysics` reference is assigned
- Check that base speed in EquipmentManager matches CharacterPhysics
- Ensure boots have `speedMultiplier` > 0

**Weapon not working:**
- Check `SpellManager` reference is assigned
- Verify weapon has cantrips assigned
- Make sure weapon has projectile prefabs linked
