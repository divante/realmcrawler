# Loadout Scene Setup Guide

## 🎯 Overview

This guide walks you through setting up the complete Loadout scene where players select equipment before starting a run.

---

## ✅ Step 1: Create Equipment Database Asset

### Create the Database ScriptableObject:

1. Navigate to `/Assets/RealmCrawler_Project/ScriptableObjects/Core/`
   - Create the folder if it doesn't exist

2. Right-click → `Create > RealmCrawler > Core > Equipment Database`

3. Name it: `EquipmentDatabase`

4. Select and configure in Inspector:

   **Available Equipment:**
   - **Available Hats:** Drag `SO_Proto_Hat` (add more later)
   - **Available Cloaks:** Drag `SO_Proto_Cloak`
   - **Available Boots:** Drag `SO_Proto_Boots`
   - **Available Weapons:** Drag all 3 weapon SOs:
     - `SO_Weapon_Proto_Wand`
     - `SO_Weapon_Proto_Staff`
     - `SO_Weapon_Proto_Tome`
   - **Available Reliquaries:** Drag `SO_Proto_Reliquary`

   **Default Equipment (Starting Loadout):**
   - **Default Hat:** `SO_Proto_Hat`
   - **Default Cloak:** `SO_Proto_Cloak`
   - **Default Boots:** `SO_Proto_Boots`
   - **Default Weapon:** `SO_Weapon_Proto_Wand`
   - **Default Reliquary:** `SO_Proto_Reliquary`

5. Save the asset

---

## ✅ Step 2: Create GameManager GameObject

### In Any Scene (Recommended: Create in Loadout Scene):

1. **Create Empty GameObject:**
   - Hierarchy → Right-click → `Create Empty`
   - Name: `GameManager`

2. **Add GameManager Component:**
   - Add Component → `GameManager` (RealmCrawler.Core)

3. **Configure Inspector:**
   - **Souls:** `1000` (starting currency for testing)
   - **Equipment Database:** Drag the `EquipmentDatabase` asset you created

4. **Important:** 
   - GameManager automatically persists via `DontDestroyOnLoad`
   - It only needs to exist in the first scene (Loadout Scene)
   - It will survive scene transitions

---

## ✅ Step 3: Create Loadout Scene

### Create the Scene:

1. `File > New Scene`
2. Save as: `/Assets/RealmCrawler_Project/Scenes/Menus/LoadoutScene.unity`

### Scene Structure:

```
LoadoutScene
├── GameManager (only if this is first scene to load)
├── Main Camera
├── Directional Light
└── Canvas (see Step 4)
```

---

## ✅ Step 4: Create Basic UI Layout

### Create Canvas:

1. **Create UI Canvas:**
   - Hierarchy → Right-click → `UI > Canvas`
   - Canvas Scaler → UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`

2. **Create Main Panel:**
   ```
   Canvas
   └── LoadoutPanel (Image, centered)
       ├── Header
       │   ├── SoulsText (TextMeshProUGUI)
       │   └── TotalCostText (TextMeshProUGUI)
       │
       ├── EquipmentSlots
       │   ├── HatSlot
       │   │   └── HatNameText (TextMeshProUGUI)
       │   ├── CloakSlot
       │   │   └── CloakNameText (TextMeshProUGUI)
       │   ├── BootsSlot
       │   │   └── BootsNameText (TextMeshProUGUI)
       │   ├── WeaponSlot
       │   │   └── WeaponNameText (TextMeshProUGUI)
       │   └── ReliquarySlot
       │       └── ReliquaryNameText (TextMeshProUGUI)
       │
       ├── StatsPanel
       │   ├── HealthStatText (TextMeshProUGUI)
       │   ├── ManaStatText (TextMeshProUGUI)
       │   ├── SpeedStatText (TextMeshProUGUI)
       │   └── XpRadiusStatText (TextMeshProUGUI)
       │
       └── ButtonPanel
           ├── StartRunButton (Button)
           └── ResetLoadoutButton (Button)
   ```

### Quick Setup (Simplified):

For prototyping, you can create a minimal version:

1. **Create LoadoutUI GameObject:**
   - Hierarchy → Right-click → `Create Empty`
   - Name: `LoadoutUI`
   - Parent to Canvas

2. **Add Text Elements:**
   - Create 10 TextMeshProUGUI objects as children
   - Name them according to the structure above

3. **Add Buttons:**
   - Create 2 Button objects
   - Name: `StartRunButton` and `ResetLoadoutButton`

---

## ✅ Step 5: Configure LoadoutUIManager

### Add Component:

1. **Select Canvas or LoadoutUI GameObject**

2. **Add Component:** `LoadoutUIManager` (RealmCrawler.UI)

3. **Assign References in Inspector:**

   **UI References:**
   - **Souls Text:** Drag `SoulsText` TextMeshProUGUI
   - **Total Cost Text:** Drag `TotalCostText` TextMeshProUGUI

   **Equipment Slot Displays:**
   - **Hat Name Text:** Drag `HatNameText`
   - **Cloak Name Text:** Drag `CloakNameText`
   - **Boots Name Text:** Drag `BootsNameText`
   - **Weapon Name Text:** Drag `WeaponNameText`
   - **Reliquary Name Text:** Drag `ReliquaryNameText`

   **Stats Display:**
   - **Health Stat Text:** Drag `HealthStatText`
   - **Mana Stat Text:** Drag `ManaStatText`
   - **Speed Stat Text:** Drag `SpeedStatText`
   - **XP Radius Stat Text:** Drag `XpRadiusStatText`

   **Buttons:**
   - **Start Run Button:** Drag `StartRunButton` Button
   - **Reset Loadout Button:** Drag `ResetLoadoutButton` Button

   **Settings:**
   - **Game Scene Name:** `GameScene` (name of your game scene)

---

## ✅ Step 6: Test Loadout Scene

### Testing:

1. **Open LoadoutScene**

2. **Enter Play Mode**

3. **You Should See:**
   - Souls: 1000
   - Total Cost: 0 (all prototype equipment is free)
   - Equipment names displayed (Proto Hat, Proto Cloak, etc.)
   - Stats calculated:
     - Health: 150 (100 base + 50 cloak)
     - Mana: 120 (100 base + 20 hat)
     - Speed: 13.0 (10 base × 1.3 boots)
     - XP Radius: 8.0 (5 base + 3 reliquary)

4. **Test Buttons:**
   - **Reset Loadout:** Should refresh to default equipment
   - **Start Run:** Should attempt to load GameScene (will fail if scene doesn't exist yet)

---

## ✅ Step 7: Setup Game Scene Integration

### In Your Game Scene:

1. **Open your game scene** (create one if needed)

2. **Find or Create Player GameObject**

3. **Add LoadoutApplier Component:**
   - Select Player GameObject
   - Add Component → `LoadoutApplier`

4. **Configure LoadoutApplier:**
   - **Equipment Manager:** Drag your `EquipmentManager` component (on Player)
   - If left empty, it will auto-find EquipmentManager

5. **How It Works:**
   - When game scene loads, `LoadoutApplier.Start()` runs
   - It gets the loadout from `GameManager.Instance`
   - Applies it to the player's `EquipmentManager`
   - Equipment visuals spawn automatically
   - Stats are applied

---

## ✅ Step 8: Add Game Scene to Build Settings

### Required for Scene Transitions:

1. **File > Build Settings**

2. **Add Scenes:**
   - Drag `LoadoutScene` to "Scenes In Build" (index 0)
   - Drag `GameScene` to "Scenes In Build" (index 1)

3. **Close Build Settings**

4. **Update LoadoutUIManager:**
   - In LoadoutScene, select LoadoutUIManager
   - Set **Game Scene Name** to exact name: `GameScene`

---

## 🎮 Complete Flow

### Scene Flow:

```
1. LoadoutScene loads
   ↓
2. GameManager initializes (DontDestroyOnLoad)
   ↓
3. LoadoutUIManager displays default equipment
   ↓
4. Player views stats (all calculated automatically)
   ↓
5. Player clicks "Start Run"
   ↓
6. GameManager.LoadGameScene() called
   ↓
7. GameScene loads
   ↓
8. LoadoutApplier applies equipment to player
   ↓
9. Player spawns with equipped gear visible
   ↓
10. Run begins!
```

---

## 🔧 Troubleshooting

### "GameManager not found"
- Make sure GameManager exists in LoadoutScene
- GameManager auto-creates if missing, but won't have database assigned
- Assign EquipmentDatabase manually if auto-created

### "Equipment Database is null"
- Check GameManager has EquipmentDatabase assigned
- Make sure you created the EquipmentDatabase ScriptableObject

### "Loadout not applied in game"
- Check LoadoutApplier is on Player GameObject in game scene
- Verify EquipmentManager reference is assigned
- Check console for errors

### "Start Run button does nothing"
- Check Game Scene Name matches exactly
- Verify scenes are in Build Settings
- Check you have enough souls (should be 1000 at start)

### "Stats show 0 or NaN"
- Check all equipment in database has proper stats assigned
- Verify default equipment is set in EquipmentDatabase
- Check CurrentLoadout is valid

---

## 📝 Next Steps

### Phase 1: Basic Testing (Do This Now)
- [ ] Create EquipmentDatabase asset
- [ ] Setup GameManager in scene
- [ ] Create basic Loadout UI
- [ ] Test scene transitions
- [ ] Verify stats display correctly

### Phase 2: Equipment Selection (Future)
- [ ] Create equipment selection popups
- [ ] Add equipment buttons for each slot
- [ ] Show equipment details on hover
- [ ] Filter by affordability

### Phase 3: Visual Polish (Future)
- [ ] Add character preview window
- [ ] Equipment icons/images
- [ ] Animations and transitions
- [ ] Sound effects
- [ ] Tooltips

### Phase 4: Economy (Future)
- [ ] Set rental costs on equipment (currently all 0)
- [ ] Add soul rewards after runs
- [ ] Save/load soul balance
- [ ] Unlock system for equipment

---

## 🎯 Testing Checklist

- [ ] GameManager persists between scenes
- [ ] Default loadout displays correctly
- [ ] Stats calculate properly (Health: 150, Mana: 120, etc.)
- [ ] Souls display (should be 1000)
- [ ] Total cost displays (should be 0)
- [ ] Start Run button works
- [ ] Reset Loadout button works
- [ ] Scene transitions to game scene
- [ ] Equipment appears on player in game scene
- [ ] Player stats match loadout stats

---

## 💡 Quick Start Summary

**Minimum to test:**

1. Create `EquipmentDatabase` asset → assign prototype equipment
2. Create `GameManager` GameObject → assign database
3. Create simple UI with TextMeshProUGUI elements
4. Add `LoadoutUIManager` → assign all text references
5. Add both scenes to Build Settings
6. Test!

**You're done when:**
- Loadout scene shows equipment names and stats
- Start Run button loads game scene
- Equipment appears on player in game scene

---

Good luck! 🚀
