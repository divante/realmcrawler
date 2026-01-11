# Inventory Instance System

## Overview
The inventory system now creates **unique runtime instances** of equipment and weapons when they're added to the player's inventory. This ensures that when you modify an item (e.g., reroll modifiers), the changes only affect that specific item instance, not all items of the same type.

## Problem Solved
**Before**: When you rerolled modifiers on an equipment item, ALL items with the same name would change because they all referenced the same ScriptableObject asset.

**After**: Each item in your inventory is a unique runtime copy. Rerolling modifiers only affects that specific item instance.

## How It Works

### When Items Are Added
When `PlayerInventory.AddItem()` is called:

1. **Equipment Items**: 
   - Creates a runtime copy using `Instantiate()`
   - Copies all modifiers in the modifiers list
   - Each modifier is also instantiated to be unique
   - Static modifiers remain as references (they don't change)

2. **Weapon Items**:
   - Creates a runtime copy using `Instantiate()`
   - Currently weapons don't have modifiable modifiers

3. **Other Items**:
   - Stored as-is (no copying needed)

### Key Changes

**`PlayerInventory.cs`**:
```csharp
public void AddItem(ScriptableObject itemData)
{
    // Creates unique instance for equipment/weapons
    ScriptableObject itemInstance = CreateEquipmentInstance(equipment);
    
    // Each item is unique - no quantity stacking
    InventoryItem newItem = new InventoryItem(itemInstance);
    items.Add(newItem);
}

private EquipmentData CreateEquipmentInstance(EquipmentData original)
{
    // Create runtime copy
    EquipmentData instance = Instantiate(original);
    
    // Copy all modifiers so they're unique
    foreach (var modifier in original.Modifiers)
    {
        StatModifierBase modifierCopy = Instantiate(modifier);
        instance.AddModifier(modifierCopy);
    }
    
    return instance;
}
```

**`EquipmentData.cs`**:
- Added `ClearModifiers()` method to clear the modifiers list before adding copies

## Important Notes

### No More Stacking
Items **no longer stack** in the inventory. Each item added creates a new unique entry.

**Why?** Since each item can have different modifiers after rerolling, they can't be stacked together. Each item is unique.

### Original Assets Are Safe
The original ScriptableObject assets in your project (`/Assets/ScriptableObjects/Equipment/...`) are **never modified**. All modifications happen on runtime instances.

### Memory Consideration
Creating instances uses slightly more memory than referencing shared assets, but this is negligible for inventory systems and is the standard approach for games with modifiable items.

## Example Flow

1. Player purchases "Proto Hat I" from shop
2. `AddItem()` creates a unique runtime instance: "Proto Hat I (Instance)"
3. Item appears in inventory with 1x Mana +20 modifier
4. Player rerolls modifier → gets Fire Damage +5
5. This ONLY affects this specific hat instance
6. Player purchases another "Proto Hat I"
7. New unique instance is created with the original Mana +20 modifier
8. Player now has TWO separate "Proto Hat I" items with different modifiers

## Testing

To verify the system works:

1. **Purchase an item** from the shop
2. **Reroll its modifiers** at the Alterations station
3. **Purchase the same item again** from the shop
4. Check both items in inventory - they should have different modifiers
5. The new item should have the original modifiers from the asset

## Related Files

- `/Assets/RealmCrawler_Project/Scripts/PlayerInventory.cs` - Instance creation logic
- `/Assets/RealmCrawler_Project/Scripts/EquipmentData.cs` - Equipment data and modifier management
- `/Assets/RealmCrawler_Project/Scripts/UI/AlterationsSystem.cs` - Reroll system
- `/Assets/RealmCrawler_Project/Scripts/UI/NPCStationUIController.cs` - UI for alterations
