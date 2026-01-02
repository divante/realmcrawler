# Loadout System - Quick Reference

## 📦 What Was Created

### Core Scripts:

| Script | Location | Purpose |
|--------|----------|---------|
| **GameManager** | `/Scripts/Core/` | Singleton manager, persists between scenes, holds loadout & currency |
| **EquipmentDatabase** | `/Scripts/Core/` | ScriptableObject listing all available equipment |
| **LoadoutUIManager** | `/Scripts/UI/` | Controls loadout scene UI, handles equipment selection |
| **LoadoutApplier** | `/Scripts/Core/` | Applies loadout to player in game scene |

### Data Classes:

- **PlayerLoadout** - Stores current equipment selection (hat, cloak, boots, weapon, reliquary)
- **LoadoutStats** - Calculated stats from loadout (health, mana, speed, XP radius)

---

## 🎯 Quick Setup (5 Steps)

### 1. Create Database:
```
Right-click in Project → Create > RealmCrawler > Core > Equipment Database
Assign all prototype equipment
```

### 2. Create GameManager:
```
Create Empty GameObject → Add GameManager component
Assign EquipmentDatabase
Set Souls: 1000
```

### 3. Create UI:
```
Create Canvas with TextMeshProUGUI elements for:
- Souls, Total Cost
- 5 Equipment names (hat, cloak, boots, weapon, reliquary)
- 4 Stats (health, mana, speed, XP radius)
- 2 Buttons (Start Run, Reset)
```

### 4. Setup LoadoutUIManager:
```
Add LoadoutUIManager component to Canvas
Assign all UI references
Set Game Scene Name
```

### 5. Setup Game Scene:
```
Add LoadoutApplier to Player GameObject
Assign EquipmentManager reference
Add scenes to Build Settings
```

---

## 🔄 System Flow

```
LoadoutScene
    ↓
Player views default equipment
    ↓
Stats display automatically
    ↓
(Future: Player selects different equipment)
    ↓
Click "Start Run"
    ↓
GameManager.LoadGameScene()
    ↓
GameScene loads
    ↓
LoadoutApplier.Start() executes
    ↓
GameManager.ApplyLoadoutToPlayer()
    ↓
EquipmentManager.SetEquipment()
    ↓
EquipmentManager.ApplyEquipmentLoadout()
    ↓
Visual equipment spawns
Stats applied
    ↓
Run begins!
```

---

## 💻 Code Examples

### Get Current Loadout:
```csharp
using RealmCrawler.Core;

PlayerLoadout loadout = GameManager.Instance.CurrentLoadout;
HatData currentHat = loadout.hat;
```

### Check Affordability:
```csharp
bool canAfford = GameManager.Instance.CanAffordLoadout();
int souls = GameManager.Instance.Souls;
```

### Calculate Stats:
```csharp
LoadoutStats stats = GameManager.Instance.CurrentLoadout.CalculateStats();
float health = stats.totalHealth;  // 150
float mana = stats.totalMana;      // 120
```

### Change Equipment:
```csharp
GameManager.Instance.CurrentLoadout.hat = newHatData;
// UI will auto-refresh if using LoadoutUIManager.RefreshUI()
```

### Apply to Player (Automatic):
```csharp
// In game scene, LoadoutApplier handles this automatically
// But you can manually call:
GameManager.Instance.ApplyLoadoutToPlayer(equipmentManager);
```

---

## 📊 Default Loadout Stats

With all prototype equipment:

| Stat | Calculation | Value |
|------|-------------|-------|
| **Health** | 100 (base) + 50 (cloak) | **150** |
| **Mana** | 100 (base) + 20 (hat) | **120** |
| **Speed** | 10 (base) × 1.3 (boots) | **13** |
| **XP Radius** | 5 (base) + 3 (reliquary) | **8** |
| **Total Cost** | 0 + 0 + 0 + 0 + 0 | **0 souls** |

---

## 🎮 GameManager API

### Properties:
```csharp
int Souls                    // Current currency
PlayerLoadout CurrentLoadout // Current equipment selection
EquipmentDatabase EquipmentDB // Reference to equipment database
```

### Methods:
```csharp
bool CanAffordLoadout()      // Check if player has enough souls
bool RentLoadout()           // Deduct cost and rent equipment
void AddSouls(int amount)    // Add currency

void SetLoadout(hat, cloak, boots, weapon, reliquary)
void ResetToDefaultLoadout() // Reset to database defaults

void LoadGameScene(string sceneName)
void LoadLoadoutScene(string sceneName)

void ApplyLoadoutToPlayer(EquipmentManager equipmentManager)
```

---

## 📋 EquipmentDatabase API

### Get All Equipment:
```csharp
List<HatData> hats = database.GetAllHats();
List<CloakData> cloaks = database.GetAllCloaks();
List<BootsData> boots = database.GetAllBoots();
List<WeaponData> weapons = database.GetAllWeapons();
List<ReliquaryData> reliquaries = database.GetAllReliquaries();
```

### Get Defaults:
```csharp
HatData defaultHat = database.GetDefaultHat();
```

### Get Affordable Equipment:
```csharp
List<HatData> affordableHats = database.GetAffordableHats(souls);
```

---

## 🔧 LoadoutUIManager API

### Methods:
```csharp
void RefreshUI()             // Update all UI elements

void SelectHat(HatData hat)
void SelectCloak(CloakData cloak)
void SelectBoots(BootsData boots)
void SelectWeapon(WeaponData weapon)
void SelectReliquary(ReliquaryData reliquary)
```

### Usage:
```csharp
// When player clicks equipment button:
loadoutUIManager.SelectHat(newHat);
// UI automatically refreshes
```

---

## ✅ Testing

### In Loadout Scene:
1. Enter Play Mode
2. Check console for: `"Loadout applied to player!"` (should NOT appear in loadout scene)
3. Verify UI displays:
   - Souls: 1000
   - Total Cost: 0
   - Equipment: Proto Hat, Proto Cloak, etc.
   - Health: 150, Mana: 120, Speed: 13, XP Radius: 8

### Scene Transition:
1. Click "Start Run" button
2. Game scene should load
3. Check console for: `"Loadout applied to player!"`
4. Verify equipment appears on player visually
5. Check player stats match loadout

---

## 🚀 Future Expansion

### Equipment Selection UI:
Create popup panels that show all available equipment and let player click to select.

### Economy:
Set `rentalCost` values in equipment ScriptableObjects, earn souls after runs.

### Unlocks:
Add `isUnlocked` bool to equipment, unlock based on progression.

### Persistence:
Save souls and unlocks to PlayerPrefs or save file.

### Character Preview:
3D preview window showing player with equipped gear.

---

## 🎯 Summary

**What's Working:**
- ✅ Equipment data flows from LoadoutScene → GameScene
- ✅ Stats calculated automatically
- ✅ Visual equipment spawns on player
- ✅ Currency system (souls) ready
- ✅ Scene transitions
- ✅ Default loadout system

**What's Next:**
- 🔲 Equipment selection UI (popups/buttons)
- 🔲 Set rental costs on equipment
- 🔲 Earn souls after runs
- 🔲 Visual polish (icons, animations)
- 🔲 Character preview window

---

**You now have a complete loadout → game flow!** 🎉
