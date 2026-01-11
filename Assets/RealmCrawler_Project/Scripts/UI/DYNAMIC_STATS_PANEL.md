# Dynamic Stats Panel System

## Overview
The Main Hub UI stats panel now **dynamically displays all stats** from the character's `CharacterData` component instead of showing hardcoded values.

## What Changed

### Before
- Stats panel showed 4 hardcoded stats: Health, Mana, Speed, Damage
- Values were placeholder text ("100", "50", etc.)
- Adding new stats required modifying UI code and UXML

### After
- Stats panel automatically displays **ALL stats** configured in CharacterData
- Shows real-time calculated values including modifiers from equipment
- Automatically updates when inventory changes (equipment added/removed)
- Adding new stats only requires adding them to the CharacterData component

## How It Works

### Stat Discovery
The `RefreshStats()` method:
1. Access the `baseStats` list from CharacterData using reflection
2. Loops through all StatData ScriptableObjects
3. Gets the current calculated value for each stat using `characterData.GetStatValue()`
4. Creates a UI row for each stat dynamically

### Dynamic UI Generation
For each stat, creates:
```
┌─────────────────────────────────┐
│ Health:              100        │  ← stat-row
│ Mana:                50         │
│ Eldritch Damage:     0          │
│ Fire Damage:         0          │
│ Lightning Damage:    0          │
│ Critical Hit:        5.0        │
│ Reliquary Pickup Radius: 1.5    │
│ Speed:               5.0        │
│ Damage:              1.0        │
└─────────────────────────────────┘
```

### Value Formatting
- Whole numbers: `100`, `50`, `5`
- Decimals: `5.0`, `1.5`

### Auto-Refresh
The UI refreshes when:
- Scene loads (OnEnable)
- Inventory changes (equipment added/removed/modified)

## Files Modified

### `/Assets/RealmCrawler_Project/Scripts/UI/MainHubUIController.cs`
**Changes:**
- Added `using RealmCrawler.CharacterStats;`
- Replaced individual stat label fields with `statsPanel` reference
- `RefreshStats()` now dynamically builds stats from CharacterData
- Added `FormatStatValue()` to format float values nicely
- Subscribed to `PlayerInventory.OnInventoryChanged` for auto-refresh

**Key Methods:**
```csharp
private void RefreshStats()
{
    // Access baseStats via reflection
    // For each stat: create row with name and current value
    // Add to stats panel
}

private string FormatStatValue(float value)
{
    // Format as integer if whole number, else one decimal
}
```

### `/Assets/RealmCrawler_Project/UI/MainHubUI.uxml`
**Changes:**
- Removed hardcoded stat rows
- Added `name="stats-panel"` to the stats container
- Stats are now generated at runtime

## Current Stats Displayed

Based on the Player's CharacterData in Scene_Loadout:
1. **Health** - Player health points
2. **Mana** - Mana/magic points
3. **Eldritch Damage** - Eldritch damage type
4. **Fire Damage** - Fire damage type
5. **Lightning Damage** - Lightning damage type
6. **Critical Hit** - Critical hit chance/multiplier
7. **Reliquary Pickup Radius** - Item pickup range
8. **Speed** - Movement speed
9. **Damage** - Base damage multiplier

## Adding New Stats

To add a new stat to the UI:

1. Create a new StatData ScriptableObject:
   - Right-click in `/ScriptableObjects/Character/Stat/`
   - Create → RealmCrawler → Characters → Stat
   - Name it (e.g., "Defense.asset")
   - Set the base value

2. Add to Player:
   - Select the Player GameObject in the scene
   - Find the CharacterData component
   - Add the new stat to the "Base Stats" list

3. **That's it!** The UI will automatically show the new stat

## Testing

To verify the dynamic stats work:

1. Enter Play mode in Scene_Loadout
2. Check the Main Hub UI stats panel
3. All 9 stats should appear with their current values
4. Equip/unequip items with modifiers
5. Stats should update automatically to reflect modifiers

## Benefits

✅ **Scalable**: Add unlimited stats without UI code changes  
✅ **Accurate**: Shows real calculated values from CharacterData  
✅ **Automatic**: Updates when equipment changes  
✅ **Clean**: No hardcoded values or manual updates needed  
✅ **Data-Driven**: Stats configuration lives in ScriptableObjects  

## Related Files

- `/Assets/RealmCrawler_Project/Scripts/UI/MainHubUIController.cs` - UI controller
- `/Assets/RealmCrawler_Project/UI/MainHubUI.uxml` - UI layout
- `/Assets/RealmCrawler_Project/Scripts/Characters/CharacterData.cs` - Character stats system
- `/Assets/RealmCrawler_Project/Scripts/Characters/Stats/StatData.cs` - Stat ScriptableObject
