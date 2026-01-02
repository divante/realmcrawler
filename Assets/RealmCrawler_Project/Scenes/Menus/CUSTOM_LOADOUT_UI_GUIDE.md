##🎨 Custom Loadout UI Setup Guide

Complete guide for implementing your custom loadout menu design with equipment browser, cantrip display, and stats panel.

---

## 📐 UI Layout Overview

```
┌─────────────────────────────────────────────────────────────────┐
│  Current Loadout Cost: 100              Current Souls: 1000    │ ← Top Bar
├──────────────────┬──────────────────────┬─────────────────────┤
│                  │                      │                     │
│  EQUIPMENT       │                      │    [BACK]           │
│  ┌────────────┐  │                      │                     │
│  │[H][C][B]   │  │                      │    STATS            │
│  │[R][W]      │  │                      │  ┌─────────────┐    │
│  ├────────────┤  │                      │  │Health: 150  │    │
│  │            │  │                      │  │  (+50)      │    │
│  │   GRID     │  │    3D BACKGROUND     │  │Mana: 120    │    │
│  │   AREA     │  │       VISIBLE        │  │  (+20)      │    │
│  │            │  │                      │  │Speed: 13.0  │    │
│  │            │  │                      │  │  (+3.0)     │    │
│  └────────────┘  │                      │  │XP Rad: 8.0  │    │
│                  │                      │  │  (+3.0)     │    │
│                  │                      │  └─────────────┘    │
│                  │                      │                     │
│                  │    ┌──────────┐      │                     │
│                  │    │ PRIMARY  │      │                     │
│                  │    │Left Click│      │                     │
│                  │    └──────────┘      │                     │
│                  │    ┌──────────┐      │                     │
│                  │    │SECONDARY │      │       [PLAY]        │
│                  │    │RightClick│      │                     │
│                  │    └──────────┘      │                     │
└──────────────────┴──────────────────────┴─────────────────────┘

LEFT: Equipment browser with tabs + grid
MIDDLE: 3D scene background + cantrip displays at bottom
RIGHT: Back button + Stats panel + Play button
```

---

## ✅ Step 1: Create Canvas Foundation

### 1.1 Create Canvas:

1. **Hierarchy → Right-click → UI → Canvas**
2. Select Canvas, in Inspector:
   - **Render Mode:** `Screen Space - Overlay`
   - **Canvas Scaler:**
     - UI Scale Mode: `Scale With Screen Size`
     - Reference Resolution: `1920 x 1080`
   - **Graphic Raycaster:** Leave enabled (for clicks)

### 1.2 Set Canvas Background to Transparent:

The Canvas itself should have NO background Image component. The 3D scene will be visible through it.

---

## ✅ Step 2: Create Top Bar (Status Bar)

### 2.1 Create Top Bar Panel:

1. **Right-click Canvas → UI → Panel**
2. Rename: `TopBar`
3. **Configure Rect Transform:**
   - Anchor Preset: **Top Stretch** (hold Shift+Alt, click top-center)
   - Height: `60`
   - Pos Y: `0`
   - Left: `0`, Right: `0`

4. **Configure Image component:**
   - Color: `Black` with Alpha: `150` (semi-transparent)
   - Or use your custom panel sprite

### 2.2 Create Loadout Cost Text:

1. **Right-click TopBar → UI → Text - TextMeshPro**
2. Rename: `LoadoutCostText`
3. **Rect Transform:**
   - Anchor: **Middle-Left**
   - Pos X: `50`, Pos Y: `0`
   - Width: `400`, Height: `40`
4. **Text:**
   - Text: `Current Loadout Cost: 0`
   - Font Size: `24`
   - Alignment: Left, Middle
   - Color: White

### 2.3 Create Souls Text:

1. **Right-click TopBar → UI → Text - TextMeshPro**
2. Rename: `CurrentSoulsText`
3. **Rect Transform:**
   - Anchor: **Middle-Right**
   - Pos X: `-50`, Pos Y: `0`
   - Width: `300`, Height: `40`
4. **Text:**
   - Text: `Current Souls: 1000`
   - Font Size: `24`
   - Alignment: Right, Middle
   - Color: Gold/Yellow

---

## ✅ Step 3: Create Equipment Panel (LEFT SIDE)

### 3.1 Create Equipment Container Panel:

1. **Right-click Canvas → UI → Panel**
2. Rename: `EquipmentPanel`
3. **Rect Transform:**
   - Anchor: **Top-Left**
   - Pos X: `20`, Pos Y: `-80`
   - Width: `400`, Height: `700`
4. **Image:**
   - Color: Black, Alpha: `180`

### 3.2 Create "EQUIPMENT" Label:

1. **Right-click EquipmentPanel → UI → Text - TextMeshPro**
2. Rename: `EquipmentLabel`
3. **Rect Transform:**
   - Anchor: **Top-Center**
   - Pos X: `0`, Pos Y: `-20`
   - Width: `360`, Height: `40`
4. **Text:**
   - Text: `EQUIPMENT`
   - Font Size: `28`
   - Style: Bold
   - Alignment: Center
   - Color: Gold/Yellow

### 3.3 Create Tab Container:

1. **Right-click EquipmentPanel → Create Empty**
2. Rename: `TabContainer`
3. **Rect Transform:**
   - Anchor: **Top-Center**
   - Pos X: `0`, Pos Y: `-70`
   - Width: `360`, Height: `60`

### 3.4 Create Tab Buttons (5 buttons):

You'll create 5 tab buttons for: Hat, Cloak, Boots, Reliquary, Weapon

**For each tab button:**

1. **Right-click TabContainer → UI → Button - TextMeshPro**
2. Rename: `HatTabButton` (or CloakTabButton, etc.)
3. **Rect Transform:**
   - Width: `60`, Height: `60`
   - Manually position in a row:
     - Hat: Pos X: `-140`
     - Cloak: Pos X: `-70`
     - Boots: Pos X: `0`
     - Reliquary: Pos X: `70`
     - Weapon: Pos X: `140`
4. **Button Text (child):**
   - Change to icon letter or symbol:
     - Hat: `H`
     - Cloak: `C`
     - Boots: `B`
     - Reliquary: `R`
     - Weapon: `W`
   - Font Size: `32`
   - (Later, replace with actual icon images)

### 3.5 Create Grid Container:

1. **Right-click EquipmentPanel → UI → Scroll View**
2. Rename: `EquipmentGridScrollView`
3. **Rect Transform:**
   - Anchor: **Top-Center**
   - Pos X: `0`, Pos Y: `-150`
   - Width: `360`, Height: `520`
4. **Remove Horizontal Scrollbar** (delete it)
5. **Keep Vertical Scrollbar**
6. **Select Content (child of Viewport):**
   - Add **Grid Layout Group** component:
     - Cell Size: `100 x 100`
     - Spacing: `10 x 10`
     - Start Corner: Upper Left
     - Start Axis: Horizontal
     - Child Alignment: Upper Center
     - Constraint: Fixed Column Count = `3`
   - Add **Content Size Fitter:**
     - Vertical Fit: `Preferred Size`

This is your `gridContainer` reference for EquipmentGridManager!

### 3.6 Create Equipment Button Prefab:

1. **Right-click EquipmentPanel → UI → Button - TextMeshPro**
2. Rename: `EquipmentButtonPrefab`
3. **Rect Transform:**
   - Width: `100`, Height: `100`
4. **Configure Button:**
   - Add Image component for equipment icon (optional)
   - Text shows equipment name
5. **Drag to Project** → Create Prefab
6. **Delete from scene** (it's now a prefab)

---

## ✅ Step 4: Create Equipment Detail Popup

### 4.1 Create Popup Panel:

1. **Right-click Canvas → UI → Panel**
2. Rename: `EquipmentDetailPopup`
3. **Rect Transform:**
   - Anchor: **Center**
   - Pos: `0, 0`
   - Width: `500`, Height: `600`
4. **Image:**
   - Color: Dark gray/black, Alpha: `220`
5. **IMPORTANT:** Start disabled (uncheck at top of Inspector)

### 4.2 Create Popup Elements:

```
EquipmentDetailPopup
├── EquipmentIconImage (Image, 128x128, top center)
├── EquipmentNameText (TextMeshProUGUI, below icon)
├── StatsText (TextMeshProUGUI, below name)
├── BuffText (TextMeshProUGUI, below stats)
├── CostText (TextMeshProUGUI, below buff)
├── ButtonPanel (Empty GameObject)
│   ├── PurchaseButton (Button)
│   └── BackButton (Button)
```

**Quick Setup:**

1. **Icon:**
   - Right-click Popup → UI → Image
   - Rename: `EquipmentIconImage`
   - Size: 128x128, Top-center, Pos Y: -100

2. **Name:**
   - Right-click Popup → UI → Text - TextMeshPro
   - Rename: `EquipmentNameText`
   - Size: 400x50, Pos Y: -200
   - Font Size: 32, Bold, Center

3. **Stats:**
   - Text - TextMeshPro: `StatsText`
   - Size: 400x100, Pos Y: -280
   - Font Size: 20

4. **Buff:**
   - Text - TextMeshPro: `BuffText`
   - Size: 400x100, Pos Y: -380

5. **Cost:**
   - Text - TextMeshPro: `CostText`
   - Size: 400x40, Pos Y: -460
   - Font Size: 24, Bold

6. **Buttons:**
   - Create 2 buttons side by side at bottom:
   - `PurchaseButton`: Pos X: -120, Pos Y: -520, Size: 180x60, Text: "PURCHASE"
   - `BackButton`: Pos X: 120, Pos Y: -520, Size: 180x60, Text: "BACK"

---

## ✅ Step 5: Create Stats Panel (RIGHT SIDE)

### 5.1 Create Stats Panel:

1. **Right-click Canvas → UI → Panel**
2. Rename: `StatsPanel`
3. **Rect Transform:**
   - Anchor: **Top-Right**
   - Pos X: `-20`, Pos Y: `-80`
   - Width: `300`, Height: `400`
   - Pivot: `1, 1` (top-right)
4. **Image:**
   - Color: Black, Alpha: `180`

### 5.2 Create "STATS" Label:

1. **Right-click StatsPanel → UI → Text - TextMeshPro**
2. Rename: `StatsLabel`
3. **Rect Transform:**
   - Anchor: Top-Center
   - Size: 260x40, Pos Y: -20
4. **Text:**
   - Text: `STATS`
   - Font Size: 28, Bold, Center
   - Color: Gold

### 5.3 Create Stat Text Elements:

Create 4 text elements inside StatsPanel:

1. **HealthStatText:**
   - Pos Y: -80
   - Text: `Health: 150 (+50)`
   - Font Size: 20
   - Width: 260, Height: 30

2. **ManaStatText:**
   - Pos Y: -120
   - Text: `Mana: 120 (+20)`

3. **SpeedStatText:**
   - Pos Y: -160
   - Text: `Speed: 13.0 (+3.0)`

4. **XpRadiusStatText:**
   - Pos Y: -200
   - Text: `XP Radius: 8.0 (+3.0)`

---

## ✅ Step 6: Create Cantrip Display (BOTTOM MIDDLE)

### 6.1 Create Cantrip Panel:

1. **Right-click Canvas → UI → Panel**
2. Rename: `CantripDisplayPanel`
3. **Rect Transform:**
   - Anchor: **Bottom-Center**
   - Pos X: `0`, Pos Y: `100`
   - Width: `300`, Height: `160`
4. **Image:**
   - Color: Black, Alpha: `180`

### 6.2 Create Primary Cantrip Display:

1. **Right-click CantripDisplayPanel → Create Empty**
2. Rename: `PrimaryHoverArea`
3. **Add Image component** (for hover detection)
   - Alpha: `0` (invisible, just for hover)
4. **Rect Transform:**
   - Size: 100x100
   - Pos X: -80, Pos Y: 20

5. **Create Icon:**
   - Right-click PrimaryHoverArea → UI → Image
   - Rename: `PrimaryCantripIcon`
   - Size: 80x80

6. **Create Label:**
   - Right-click PrimaryHoverArea → UI → Text - TextMeshPro
   - Rename: `PrimaryLabel`
   - Text: `Left Click`
   - Pos Y: -60, Size: 100x30
   - Font Size: 16, Center

### 6.3 Create Secondary Cantrip Display:

Same as primary, but:
- Rename: `SecondaryHoverArea`
- Pos X: `80`
- Icon: `SecondaryCantripIcon`
- Label: `Right Click`

### 6.4 Create Tooltip Panel:

1. **Right-click Canvas → UI → Panel**
2. Rename: `CantripTooltip`
3. **Rect Transform:**
   - Anchor: Bottom-Center
   - Pos Y: 280
   - Size: 300x200
4. **Image:**
   - Color: Black, Alpha: `240`
5. **Start Disabled**

**Add to tooltip:**
```
CantripTooltip
├── TooltipTitleText (cantrip name, size 24, bold, top)
├── TooltipDescriptionText (description, size 16, middle)
└── TooltipStatsText (damage/speed/etc, size 14, bottom)
```

---

## ✅ Step 7: Create Buttons (RIGHT SIDE)

### 7.1 Back Button:

1. **Right-click Canvas → UI → Button - TextMeshPro**
2. Rename: `BackButton`
3. **Rect Transform:**
   - Anchor: **Top-Right**
   - Pos X: `-20`, Pos Y: `-20`
   - Size: 120x50
   - Pivot: `1, 1`
4. **Text:** `BACK`

### 7.2 Play Button:

1. **Right-click Canvas → UI → Button - TextMeshPro**
2. Rename: `PlayButton`
3. **Rect Transform:**
   - Anchor: **Bottom-Right**
   - Pos X: `-20`, Pos Y: `20`
   - Size: 200x80
   - Pivot: `1, 0`
4. **Text:** `PLAY`
   - Font Size: 36, Bold
   - Color: Green/Gold

---

## ✅ Step 8: Connect Scripts to UI

### 8.1 Create UIManager GameObject:

1. **Right-click Canvas → Create Empty**
2. Rename: `UIManager`

### 8.2 Add LoadoutUIManager Component:

1. **Select UIManager**
2. **Add Component → LoadoutUIManager**
3. **Assign References:**

**Top Bar:**
- Loadout Cost Text: Drag `LoadoutCostText`
- Current Souls Text: Drag `CurrentSoulsText`

**Stats Panel:**
- Health Stat Text: Drag `HealthStatText`
- Mana Stat Text: Drag `ManaStatText`
- Speed Stat Text: Drag `SpeedStatText`
- XP Radius Stat Text: Drag `XpRadiusStatText`

**Buttons:**
- Play Button: Drag `PlayButton`
- Back Button: Drag `BackButton`

**Settings:**
- Game Scene Name: Type your game scene name

### 8.3 Add EquipmentGridManager Component:

1. **Select UIManager (or create separate GameObject)**
2. **Add Component → EquipmentGridManager**
3. **Assign References:**

**Tab Buttons:**
- Hat Tab Button: Drag `HatTabButton`
- Cloak Tab Button: Drag `CloakTabButton`
- Boots Tab Button: Drag `BootsTabButton`
- Reliquary Tab Button: Drag `ReliquaryTabButton`
- Weapon Tab Button: Drag `WeaponTabButton`

**Grid Settings:**
- Grid Container: Drag `Content` (inside EquipmentGridScrollView → Viewport)
- Equipment Button Prefab: Drag your prefab from Project
- Grid Columns: `3`
- Button Spacing: `10`

### 8.4 Add EquipmentDetailPopup Component:

1. **Select the EquipmentDetailPopup GameObject**
2. **Add Component → EquipmentDetailPopup**
3. **Assign References:**

**Popup Panel:**
- Popup Panel: Drag itself (EquipmentDetailPopup)

**Display Elements:**
- Equipment Icon Image: Drag `EquipmentIconImage`
- Equipment Name Text: Drag `EquipmentNameText`
- Stats Text: Drag `StatsText`
- Buff Text: Drag `BuffText`
- Cost Text: Drag `CostText`

**Buttons:**
- Purchase Button: Drag `PurchaseButton`
- Back Button: Drag `BackButton` (in popup)

**References:**
- Loadout UI Manager: Drag `UIManager` (with LoadoutUIManager component)

### 8.5 Link DetailPopup to GridManager:

1. **Select UIManager (with EquipmentGridManager)**
2. **In EquipmentGridManager:**
   - Detail Popup: Drag `EquipmentDetailPopup` GameObject

### 8.6 Add CantripDisplayPanel Component:

1. **Select CantripDisplayPanel GameObject**
2. **Add Component → CantripDisplayPanel**
3. **Assign References:**

**Cantrip Icons:**
- Primary Cantrip Icon: Drag `PrimaryCantripIcon`
- Secondary Cantrip Icon: Drag `SecondaryCantripIcon`

**Labels:**
- Primary Label: Drag `PrimaryLabel`
- Secondary Label: Drag `SecondaryLabel`

**Tooltip:**
- Tooltip Panel: Drag `CantripTooltip`
- Tooltip Title Text: Drag `TooltipTitleText`
- Tooltip Description Text: Drag `TooltipDescriptionText`
- Tooltip Stats Text: Drag `TooltipStatsText`

**Hover Detectors:**
- Primary Hover Area: Drag `PrimaryHoverArea`
- Secondary Hover Area: Drag `SecondaryHoverArea`

### 8.7 Link CantripDisplay to LoadoutUIManager:

1. **Select UIManager (with LoadoutUIManager)**
2. **In LoadoutUIManager:**
   - Cantrip Display: Drag `CantripDisplayPanel`
   - Equipment Grid: Drag UIManager itself (or GameObject with EquipmentGridManager)

---

## ✅ Step 9: Final Checks

### 9.1 Hierarchy Should Look Like:

```
Canvas
├── TopBar
│   ├── LoadoutCostText
│   └── CurrentSoulsText
├── EquipmentPanel
│   ├── EquipmentLabel
│   ├── TabContainer
│   │   ├── HatTabButton
│   │   ├── CloakTabButton
│   │   ├── BootsTabButton
│   │   ├── ReliquaryTabButton
│   │   └── WeaponTabButton
│   └── EquipmentGridScrollView
│       └── Viewport
│           └── Content (gridContainer reference)
├── StatsPanel
│   ├── StatsLabel
│   ├── HealthStatText
│   ├── ManaStatText
│   ├── SpeedStatText
│   └── XpRadiusStatText
├── CantripDisplayPanel
│   ├── PrimaryHoverArea
│   │   ├── PrimaryCantripIcon
│   │   └── PrimaryLabel
│   └── SecondaryHoverArea
│       ├── SecondaryCantripIcon
│       └── SecondaryLabel
├── BackButton
├── PlayButton
├── EquipmentDetailPopup (disabled)
│   ├── EquipmentIconImage
│   ├── EquipmentNameText
│   ├── StatsText
│   ├── BuffText
│   ├── CostText
│   ├── PurchaseButton
│   └── BackButton
├── CantripTooltip (disabled)
│   ├── TooltipTitleText
│   ├── TooltipDescriptionText
│   └── TooltipStatsText
└── UIManager (with all 3 manager scripts)
```

### 9.2 Testing:

1. **Enter Play Mode**
2. **Top bar should show:** Souls and Loadout Cost
3. **Tabs should work:** Click tabs to switch categories
4. **Grid should populate:** Equipment buttons appear
5. **Click equipment:** Popup shows details
6. **Purchase:** Selects equipment, closes popup
7. **Stats update:** Right panel shows calculated stats with bonuses
8. **Cantrip icons:** Show weapon cantrips
9. **Hover cantrips:** Tooltip appears
10. **Play button:** Starts game (if affordable)

---

## 🎨 Visual Polish Tips

### Semi-Transparent Panels:

All panels should have:
- Image component
- Color: Black (or custom color)
- Alpha: 150-220 (semi-transparent)
- This lets 3D background show through

### Tab Button States:

Selected tab should highlight (already handled in code):
- Normal: White
- Selected: Gold/Yellow tint

### Purchase Button:

Should be disabled if can't afford:
- Red text when can't afford
- Grayed out when insufficient souls

### Stat Bonuses:

Show `+` or `-` next to values:
- Health: 150 (+50) ← green
- Speed: 10.0 (-2.0) ← red (if negative)

---

## 🚀 Next Steps

1. **Add icons** to replace letter placeholders in tabs
2. **Add equipment icons** to grid buttons and popup
3. **Style buttons** with custom sprites/colors
4. **Add animations** for popup open/close
5. **Add sound effects** for clicks
6. **Test with 3D background** scene

---

Your custom loadout UI is now complete! 🎉
