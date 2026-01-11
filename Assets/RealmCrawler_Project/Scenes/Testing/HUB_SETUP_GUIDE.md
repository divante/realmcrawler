# 🏛️ Hub System Setup Guide

Complete setup instructions for the 3-station hub system with clickable 3D objects.

---

## 📋 What Was Created

### **Scripts** (in `/Assets/RealmCrawler_Project/Scripts/UI/`)
1. `HubStationManager.cs` - Central controller for station switching
2. `InteractableObject.cs` - Clickable 3D objects (NPC, Chest)
3. `MainHubUIController.cs` - Main hub UI controller
4. `NPCStationUIController.cs` - NPC rental/shop UI controller
5. `ChestStationUIController.cs` - Inventory/equip UI controller

### **UI Assets** (in `/Assets/RealmCrawler_Project/UI/`)
1. `HubStyles.uss` - Shared stylesheet for all 3 UIs
2. `MainHubUI.uxml` - Main hub UI layout
3. `NPCUI.uxml` - NPC station UI layout
4. `ChestUI.uxml` - Chest station UI layout

---

## 🛠️ Scene Setup Instructions

### **Step 1: Create Camera Positions**

Create empty GameObjects in your scene as camera targets:

```
Scene_Loadout
├── CameraPositions (empty parent)
│   ├── MainHubCamTransform
│   ├── NPCCamTransform
│   └── ChestCamTransform
```

**Position them:**
- **MainHubCamTransform:** Wide view of the hub (shows NPC + Chest objects)
- **NPCCamTransform:** Close-up on NPC character
- **ChestCamTransform:** Close-up on chest object

---

### **Step 2: Create 3D Interactable Objects**

**A. Create NPC Object:**
1. Create a 3D object (Cube, Capsule, or import NPC model)
2. Name it `NPC_Interactable`
3. Add Component → `InteractableObject`
4. Add a **Collider** (BoxCollider, CapsuleCollider, etc.)
5. Set Layer to `Default` (so raycast can hit it)

**In Inspector:**
- Interactable Type: `NPC`
- Interaction Range: `5.0`
- Raycast Layer Mask: `Everything` (default)

**B. Create Chest Object:**
1. Create a 3D object (Cube or import chest model)
2. Name it `Chest_Interactable`
3. Add Component → `InteractableObject`
4. Add a **Collider**
5. Set Layer to `Default`

**In Inspector:**
- Interactable Type: `Chest`
- Interaction Range: `5.0`
- Raycast Layer Mask: `Everything` (default)

---

### **Step 3: Create UI Documents**

**A. Main Hub UI:**
1. Create → UI Toolkit → UI Document
2. Name it `MainHubUIDocument`
3. Assign UXML: `MainHubUI.uxml`
4. Add Component → `MainHubUIController`

**B. NPC UI:**
1. Create → UI Toolkit → UI Document
2. Name it `NPCUIDocument`
3. Assign UXML: `NPCUI.uxml`
4. Add Component → `NPCStationUIController`

**C. Chest UI:**
1. Create → UI Toolkit → UI Document
2. Name it `ChestUIDocument`
3. Assign UXML: `ChestUI.uxml`
4. Add Component → `ChestStationUIController`

---

### **Step 4: Setup HubStationManager**

1. Create empty GameObject named `HubStationManager`
2. Add Component → `HubStationManager`

**In Inspector, assign:**
- **UI Documents:**
  - Main Hub UI: `MainHubUIDocument`
  - NPC UI: `NPCUIDocument`
  - Chest UI: `ChestUIDocument`

- **Camera:**
  - Hub Camera: Drag your Main Camera
  - Main Hub Camera Transform: `MainHubCamTransform`
  - NPC Camera Transform: `NPCCamTransform`
  - Chest Camera Transform: `ChestCamTransform`
  - Camera Transition Duration: `1.0`

- **3D Interactables:**
  - NPC Object: `NPC_Interactable`
  - Chest Object: `Chest_Interactable`

---

### **Step 5: Final Scene Hierarchy**

Your scene should look like this:

```
Scene_Loadout
├── Main Camera
├── Directional Light
├── EventSystem
├── GameManager
├── Player (your character model)
│
├── CameraPositions
│   ├── MainHubCamTransform
│   ├── NPCCamTransform
│   └── ChestCamTransform
│
├── Interactables
│   ├── NPC_Interactable (3D model + InteractableObject)
│   └── Chest_Interactable (3D model + InteractableObject)
│
├── UI
│   ├── MainHubUIDocument (UIDocument + MainHubUIController)
│   ├── NPCUIDocument (UIDocument + NPCStationUIController)
│   └── ChestUIDocument (UIDocument + ChestStationUIController)
│
└── HubStationManager
```

---

## 🎮 How It Works

### **Flow:**

**Main Hub (Default):**
- Camera at `MainHubCamTransform`
- Shows `MainHubUI` (stats, buffs, cantrips, buttons)
- NPC and Chest 3D objects are visible and clickable
- Player can click on 3D objects to switch stations

**NPC Station:**
- Click on NPC object → switches to NPC station
- Camera transitions to `NPCCamTransform`
- Shows `NPCUI` (rental/trade/sell services)
- NPC and Chest objects hidden
- Back button returns to Main Hub

**Chest Station:**
- Click on Chest object → switches to Chest station
- Camera transitions to `ChestCamTransform`
- Shows `ChestUI` (inventory browser, equip gear)
- NPC and Chest objects hidden
- Back button returns to Main Hub

---

## 🖱️ Interaction System

**Hovering:**
- Mouse over 3D object → highlight effect activates
- Uses emission color for glow (requires material with emission)

**Clicking:**
- Left-click on NPC → opens NPC station
- Left-click on Chest → opens Chest station
- Only works when NOT hovering over UI elements

**Raycasting:**
- Uses `Physics.Raycast` from camera through mouse position
- Checks against `Interaction Range` (default 5.0)
- Respects `Raycast Layer Mask` setting

---

## 🎨 Customization Options

### **Camera Behavior:**

In `HubStationManager`:
- `Camera Transition Duration` - How long camera takes to move
- `Camera Easing` - Animation curve for smooth transitions

### **Interaction:**

In `InteractableObject`:
- `Interaction Range` - Max distance for clicking
- `Raycast Layer Mask` - Which layers can be clicked
- `Highlight Color` - Emission color on hover
- `Highlight Intensity` - Brightness of glow

### **UI Styling:**

Edit `/Assets/RealmCrawler_Project/UI/HubStyles.uss`:
- Change colors, sizes, layouts
- USS uses standard CSS-like syntax
- Hot-reloads in Play mode

---

## 📝 Next Steps

### **Integrate with Existing Systems:**

**1. Connect CharacterData to MainHubUI:**
- Read actual stats from `CharacterData.GetStatValue()`
- Display real health, mana, speed, damage

**2. Connect EquipmentManager to MainHubUI:**
- Show equipped weapon's cantrips
- Display cantrip icons and names
- Show active buffs from equipment

**3. Implement NPC Services:**
- Rent gear functionality
- Trade/reroll system
- Sell gear for currency

**4. Implement Chest Inventory:**
- List owned gear by category
- Show gear details on selection
- Equip/unequip functionality
- Reuse existing loadout UI logic

**5. Add Visual Polish:**
- Particle effects on object hover
- Sound effects for clicks
- UI animations on station switch
- Custom 3D models for NPC and Chest

---

## 🐛 Troubleshooting

**Objects not clickable:**
- Check objects have Colliders attached
- Verify Layer is set to something in Raycast Layer Mask
- Ensure camera has `Physics Raycaster` component (or use built-in raycasting)

**UI not showing:**
- Check UXML files are assigned to UIDocuments
- Verify USS stylesheet is linked in UXML
- Check `HubStationManager` references are assigned

**Camera not moving:**
- Verify Camera Transform references are assigned
- Check transforms are positioned correctly
- Ensure Camera Transition Duration > 0

**Hover highlight not working:**
- Material must support emission
- Check Highlight Color and Intensity values
- Verify Renderer component exists on object

---

## ✅ Testing Checklist

- [ ] Main Hub UI displays on scene start
- [ ] Stats panel shows placeholder values
- [ ] Cantrip slots are visible
- [ ] Start Run button works (loads Scene_Main)
- [ ] Settings button responds to click
- [ ] Main Menu button works (loads Scene_MainMenu)
- [ ] NPC object highlights on mouse hover
- [ ] Chest object highlights on mouse hover
- [ ] Clicking NPC → switches to NPC station
- [ ] Clicking Chest → switches to Chest station
- [ ] Camera smoothly transitions between stations
- [ ] Back button returns to Main Hub from NPC station
- [ ] Back button returns to Main Hub from Chest station
- [ ] 3D objects hidden when in NPC/Chest stations
- [ ] Cantrip tooltips show on hover

---

**System is ready for integration!** 🎉

Connect your existing equipment/stat systems to the UI controllers to bring it all together.
