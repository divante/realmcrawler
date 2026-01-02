# Custom Loadout UI - Quick Reference

## 📦 What Was Created

### New Scripts:

1. **EquipmentGridManager.cs**
   - Manages tabbed equipment browsing
   - Populates grid with available equipment
   - Handles tab switching (Hat/Cloak/Boots/Reliquary/Weapon)
   - Opens detail popup when equipment clicked

2. **EquipmentDetailPopup.cs**
   - Shows equipment details in popup
   - Displays icon, name, stats, buffs, cost
   - Purchase button to select equipment
   - Back button to close popup

3. **CantripDisplayPanel.cs**
   - Shows primary/secondary cantrip icons
   - Displays "Left Click" and "Right Click" labels
   - Tooltip on hover with cantrip details

4. **LoadoutUIManager.cs** (Updated)
   - Simplified for your custom layout
   - Manages top bar (cost & souls)
   - Updates stats panel with +/- bonuses
   - Play and Back buttons

---

## 🎯 UI Components Needed

### Required UI Elements:

**Top Bar (thin, full width):**
- `LoadoutCostText` - Shows total loadout cost
- `CurrentSoulsText` - Shows available souls

**Equipment Panel (left side):**
- "EQUIPMENT" label
- 5 tab buttons (Hat, Cloak, Boots, Reliquary, Weapon)
- Scroll view with grid layout for equipment buttons
- Equipment button prefab (to spawn in grid)

**Equipment Detail Popup (center, hidden by default):**
- Equipment icon image
- Equipment name text
- Stats text
- Buff description text
- Cost text
- Purchase button
- Back button

**Stats Panel (right side):**
- "STATS" label
- Health stat text (with +/- bonus)
- Mana stat text (with +/- bonus)
- Speed stat text (with +/- bonus)
- XP Radius stat text (with +/- bonus)

**Cantrip Display (bottom center):**
- Primary cantrip icon + "Left Click" label + hover area
- Secondary cantrip icon + "Right Click" label + hover area
- Tooltip panel (hidden by default) with title, description, stats

**Buttons:**
- Back button (top right)
- Play button (bottom right)

---

## 🔧 Script Connections

### EquipmentGridManager:

**Needs:**
- 5 tab buttons (Hat, Cloak, Boots, Reliquary, Weapon)
- Grid container (Content GameObject inside Scroll View)
- Equipment button prefab
- Reference to EquipmentDetailPopup

**Does:**
- Switches between equipment categories
- Populates grid with available items
- Opens detail popup on equipment click

### EquipmentDetailPopup:

**Needs:**
- Popup panel GameObject
- Icon image, name/stats/buff/cost texts
- Purchase and Back buttons
- Reference to LoadoutUIManager

**Does:**
- Shows equipment details
- Handles purchase (selects equipment)
- Closes on Back button

### CantripDisplayPanel:

**Needs:**
- Primary/Secondary cantrip icon images
- Primary/Secondary labels
- Hover areas (GameObjects with Image for detection)
- Tooltip panel with title/description/stats texts

**Does:**
- Displays weapon cantrips
- Shows tooltip on hover
- Updates when weapon changes

### LoadoutUIManager:

**Needs:**
- Top bar texts (cost, souls)
- Stats panel texts (health, mana, speed, XP radius)
- Play and Back buttons
- References to CantripDisplayPanel and EquipmentGridManager

**Does:**
- Updates all UI when loadout changes
- Calculates and displays stats with bonuses
- Handles Play button (starts game)
- Handles Back button (returns to menu)

---

## 🎨 Key Features

### Equipment Selection Flow:

```
1. Click tab (e.g., "Hat")
   ↓
2. Grid populates with all hats
   ↓
3. Click a hat in grid
   ↓
4. Detail popup appears with hat info
   ↓
5. Click "Purchase"
   ↓
6. Hat is equipped
   ↓
7. Stats update automatically
   ↓
8. Popup closes
```

### Cantrip Tooltips:

```
1. Weapon is equipped
   ↓
2. Cantrip icons update with weapon's cantrips
   ↓
3. Hover over icon
   ↓
4. Tooltip appears with:
   - Cantrip name
   - Description
   - Stats (damage, attack speed, projectile speed)
   ↓
5. Move mouse away
   ↓
6. Tooltip hides
```

### Stat Display Format:

```
Health: 150 (+50)
        ↑    ↑
      Total  Bonus from equipment
```

- Shows total stat value
- Shows bonus/penalty in parentheses
- Bonus calculated as: Total - Base

---

## ⚙️ Setup Checklist

- [ ] Create Canvas with Screen Space Overlay
- [ ] Create top bar with cost/souls texts
- [ ] Create equipment panel with tabs and grid
- [ ] Create equipment button prefab
- [ ] Create equipment detail popup (start disabled)
- [ ] Create stats panel with 4 stat texts
- [ ] Create cantrip display panel with 2 icons + hover areas
- [ ] Create cantrip tooltip (start disabled)
- [ ] Create back button (top right)
- [ ] Create play button (bottom right)
- [ ] Add LoadoutUIManager to scene
- [ ] Add EquipmentGridManager to scene
- [ ] Add EquipmentDetailPopup component to popup
- [ ] Add CantripDisplayPanel component to cantrip panel
- [ ] Assign all references in all 4 scripts
- [ ] Test in Play Mode

---

## 🧪 Testing

### What Should Work:

✅ Tabs switch equipment categories
✅ Grid populates with items
✅ Click item → popup appears
✅ Purchase → equipment equips
✅ Stats update with bonuses shown
✅ Cantrip icons show weapon cantrips
✅ Hover cantrip → tooltip appears
✅ Play button loads game (if affordable)
✅ Back button works (currently logs to console)

### Common Issues:

**Grid doesn't populate:**
- Check EquipmentDatabase is assigned in GameManager
- Check GridContainer reference is set to Content GameObject
- Check Equipment Button Prefab is assigned

**Popup doesn't appear:**
- Check Detail Popup reference in EquipmentGridManager
- Check Popup Panel is assigned in EquipmentDetailPopup

**Tooltips don't show:**
- Check Hover Areas have Image component
- Check EventTrigger is added (script does this automatically)
- Check Tooltip Panel is assigned

**Stats show 0 or wrong values:**
- Check GameManager has valid loadout
- Check Equipment has stats assigned in ScriptableObjects

---

## 📝 Customization Tips

### Change Grid Layout:

In Grid Layout Group (on Content):
- **Cell Size:** Bigger = fewer columns
- **Spacing:** Adjust gap between items
- **Constraint:** Change column count

### Style Panels:

All panels use Image component:
- **Color:** Change background color
- **Alpha:** Adjust transparency (0-255)
- **Sprite:** Use custom panel sprite

### Tab Button Icons:

Replace text with images:
1. Delete Text child from button
2. Add Image child
3. Assign icon sprite
4. EquipmentGridManager still works

### Equipment Button Style:

Edit the prefab:
- Add icon Image component
- Resize/reposition text
- Add background sprite
- Adjust colors

---

## 🚀 Future Enhancements

### Visual Polish:
- Add equipment icons to grid buttons
- Add category icons to tabs
- Animate popup open/close
- Add particle effects
- Add sound effects

### Features:
- Sort/filter equipment
- Search bar
- Equipment comparison
- Favorites/bookmarks
- Preview equipment on 3D character

### Economy:
- Show "owned" vs "rental" items
- Lock expensive items
- Show unlock requirements
- Daily deals/discounts

---

Good luck with your custom UI! 🎨
