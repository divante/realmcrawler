# 🛒 Shop & Inventory System Setup Guide

Complete guide for setting up the NPC rental shop and player inventory chest system.

---

## 📋 System Overview

### **Components Created:**
1. **PlayerInventory** - Manages items purchased by the player
2. **ShopData** (ScriptableObject) - Configures shop items and randomization
3. **NPCStationUIController** - Shop UI with purchase functionality
4. **ChestStationUIController** - Inventory UI with equip functionality

### **Flow:**
```
NPC Interaction → Open Shop → Browse Items → Purchase → Add to Inventory
                                                              ↓
Chest Interaction → View Inventory → Select Item → Equip to Loadout
```

---

## ✅ Step 1: Create Shop Data Asset

1. In Project window, navigate to: `/Assets/RealmCrawler_Project/ScriptableObjects/`
2. Create a new folder: `Shops`
3. Right-click in `Shops` folder
4. Select: `Create > RealmCrawler > Shop > Shop Data`
5. Name it: `SO_MerchantShop`

### **Configure Shop Data:**

Open `SO_MerchantShop` in Inspector:

#### **Shop Configuration:**
- **Shop Name**: "Merchant's Rental Shop"
- **Shop Dialogue**: "Welcome, traveler! Rent my finest equipment!"

#### **Item Pool - Hats:**
Click **+** to add entries:
- **Item**: Drag `SO_Proto_Hat` asset
- **Stock Quantity**: `1`
- **Spawn Chance**: `1.0` (100% chance to appear)

Repeat for `SO_Proto_Hat_II` and any other hats.

#### **Item Pool - Cloaks:**
- Add your cloak assets (e.g., `SO_Proto_Cloak`)

#### **Item Pool - Boots:**
- Add your boots assets

#### **Item Pool - Reliquaries:**
- Add your reliquary assets

#### **Item Pool - Weapons:**
- Add your weapon assets

#### **Randomization Settings:**
- **Randomize Stock**: ☑ Enable for random shop inventory each time
- **Min Items Per Category**: `2`
- **Max Items Per Category**: `4`

> **Note:** If `Randomize Stock` is disabled, ALL items in the pool will appear in the shop.

---

## ✅ Step 2: Setup Player Inventory

### **Add to Player GameObject:**

1. In the Hierarchy, find or create your **Player** GameObject
2. Make sure it has the **Tag**: `Player`
3. Add Component: **Player Inventory**

The `PlayerInventory` component will:
- Store purchased items
- Track item quantities
- Fire events when inventory changes
- Persist during the scene

---

## ✅ Step 3: Update NPC Station UI

### **Configure NPCStationUIController:**

1. Find your NPC station GameObject in the scene
2. Select the GameObject with `NPCStationUIController` component
3. In Inspector:
   - **UI Document**: Should already be assigned
   - **Shop Data**: Drag `SO_MerchantShop` here

### **Required UI Elements** (UXML):

The `NPCUI.uxml` file should have these elements:

```xml
<ui:UXML>
    <!-- Main Menu Panel -->
    <ui:VisualElement name="main-menu-panel">
        <ui:Label name="npc-dialogue" text="Welcome!" />
        <ui:Label name="souls-label" text="Souls: 0" />
        <ui:Button name="rent-button" text="RENT EQUIPMENT" />
        <ui:Button name="trade-button" text="TRADE" />
        <ui:Button name="sell-button" text="SELL" />
        <ui:Button name="back-button" text="BACK" />
    </ui:VisualElement>
    
    <!-- Shop Panel (hidden by default) -->
    <ui:VisualElement name="shop-panel" style="display: none;">
        <ui:ListView name="shop-list" />
        <ui:Button name="shop-back-button" text="BACK" />
    </ui:VisualElement>
</ui:UXML>
```

> **Note:** The shop panel UI elements are created programmatically, but you need the container elements.

---

## ✅ Step 4: Update Chest Station UI

### **Configure ChestStationUIController:**

1. Find your Chest station GameObject in the scene
2. Select the GameObject with `ChestStationUIController` component
3. UI Document should be assigned

### **Required UI Elements** (UXML):

The `ChestUI.uxml` file should have:

```xml
<ui:UXML>
    <ui:Label name="category-label" text="Inventory" />
    
    <!-- Category Tabs -->
    <ui:Button name="all-tab" text="ALL" />
    <ui:Button name="hat-tab" text="HATS" />
    <ui:Button name="cloak-tab" text="CLOAKS" />
    <ui:Button name="boots-tab" text="BOOTS" />
    <ui:Button name="reliquary-tab" text="RELIQUARIES" />
    <ui:Button name="weapon-tab" text="WEAPONS" />
    
    <!-- Inventory List -->
    <ui:ListView name="inventory-list" />
    
    <!-- Detail Panel -->
    <ui:VisualElement name="equipment-detail" style="display: none;">
        <ui:VisualElement name="detail-icon" />
        <ui:Label name="detail-name" />
        <ui:Label name="detail-stats" />
        <ui:Button name="equip-button" text="EQUIP" />
    </ui:VisualElement>
    
    <ui:Button name="back-button" text="BACK" />
</ui:UXML>
```

---

## ✅ Step 5: Test the System

### **Testing the Shop:**

1. Play the scene
2. Interact with the NPC
3. Click **"RENT EQUIPMENT"** button
4. The shop panel should open showing available items
5. Each item displays:
   - ✅ Icon (if assigned)
   - ✅ Name
   - ✅ Cost in souls
   - ✅ BUY button

6. Click **BUY** on an item:
   - ✅ Souls are deducted from player
   - ✅ Item is added to inventory
   - ✅ Button changes to "SOLD" and disables
   - ✅ Item is removed from shop list

### **Testing the Inventory:**

1. After purchasing items, interact with the Chest
2. Chest UI should open showing your inventory
3. Use category tabs to filter items
4. Click an item to view details
5. Click **EQUIP** to add it to your loadout

---

## 🎨 Customization Options

### **ShopData Configuration:**

#### **Static Shop (No Randomization):**
```csharp
Randomize Stock: ☐ Disabled
```
All items in the pool appear in the shop every time.

#### **Random Shop:**
```csharp
Randomize Stock: ☑ Enabled
Min Items Per Category: 2
Max Items Per Category: 5
```
Shop will randomly select 2-5 items from each category pool.

#### **Weighted Randomization:**
```csharp
Spawn Chance:
- Common items: 1.0 (100%)
- Rare items: 0.5 (50%)
- Legendary items: 0.1 (10%)
```

### **Multiple Shops:**

Create different shop assets for different NPCs:
- `SO_WeaponSmithShop` - Only weapons
- `SO_ArmorMerchantShop` - Only equipment
- `SO_RareItemsShop` - High-tier items with low spawn chance

---

## 🔧 Advanced Features

### **Custom Item Filtering:**

Edit `ShopData.cs` to add custom filters:

```csharp
public List<ShopItemEntry> GetRareItemsOnly()
{
    return hatPool.FindAll(entry => 
        (entry.item as EquipmentData)?.Rarity >= ItemRarity.Rare
    );
}
```

### **Quantity Tracking:**

Items track stock quantity:
```csharp
ShopItemEntry entry = new ShopItemEntry
{
    item = myHat,
    stockQuantity = 3,  // Can sell 3 copies
    spawnChance = 1f
};
```

### **Inventory Events:**

Subscribe to inventory changes:

```csharp
void OnEnable()
{
    playerInventory.OnItemAdded += HandleItemAdded;
    playerInventory.OnItemRemoved += HandleItemRemoved;
    playerInventory.OnInventoryChanged += RefreshUI;
}

void HandleItemAdded(InventoryItem item)
{
    Debug.Log($"Added {item.itemData.name} to inventory!");
    // Play sound effect, show notification, etc.
}
```

---

## 📊 System Architecture

```
ShopData (ScriptableObject)
├── Shop Configuration (name, dialogue)
├── Hat Pool (List<ShopItemEntry>)
├── Cloak Pool
├── Boots Pool
├── Reliquary Pool
├── Weapon Pool
└── Randomization Settings

PlayerInventory (MonoBehaviour on Player)
├── Items (List<InventoryItem>)
├── OnItemAdded (Event)
├── OnItemRemoved (Event)
└── OnInventoryChanged (Event)

InventoryItem
├── itemId (Guid) - Unique identifier
├── itemData (ScriptableObject) - Reference to equipment/weapon
└── quantity (int) - Stack count

ShopItemEntry
├── item (ScriptableObject) - Equipment or Weapon
├── stockQuantity (int) - How many available
├── spawnChance (float) - Probability to appear (0-1)
├── currentStock (int) - Runtime tracking
└── isPurchased (bool) - Already bought
```

---

## ✅ Quick Checklist

Before testing, verify:

- ☐ `SO_MerchantShop` created with items configured
- ☐ Player GameObject has `PlayerInventory` component
- ☐ Player GameObject has Tag: `Player`
- ☐ NPC GameObject has `ShopData` reference assigned
- ☐ `NPCUI.uxml` has required UI elements
- ☐ `ChestUI.uxml` has required UI elements
- ☐ Equipment items have costs assigned (`RentalCost` field)
- ☐ Equipment items have icons assigned (optional but recommended)
- ☐ `GameManager` exists in scene for souls tracking

---

## 🐛 Troubleshooting

### **"Shop is empty!"**
- Check `ShopData` asset has items in the pools
- Verify `NPCStationUIController` has `shopData` assigned
- Check spawn chance values (0.0 = never spawns, 1.0 = always spawns)

### **"Purchase button doesn't work!"**
- Verify `GameManager` exists and tracks souls
- Check equipment `RentalCost` is set correctly
- Ensure player has enough souls

### **"Inventory doesn't show items!"**
- Verify `PlayerInventory` component exists on Player GameObject
- Check Player has Tag: `Player`
- Check `ChestStationUIController` finds `PlayerInventory` in `OnEnable()`

### **"Items don't have icons!"**
- Assign sprites to `ItemIcon` field on equipment assets
- Icons are optional but improve UX

---

## 🎯 Next Steps

Now that you have a working shop and inventory:

1. **Create more shop variations** with different item pools
2. **Add UI styling** to match your game's aesthetic
3. **Implement save/load** for persistent inventory
4. **Add sound effects** for purchase and equip actions
5. **Create tooltips** showing item stats on hover
6. **Add rarity colors** to item displays
7. **Implement sell-back** functionality at the NPC

---

**System is ready to use! 🎉**

Test it by:
1. Running the scene
2. Talking to the NPC → Click "RENT EQUIPMENT"
3. Purchasing items with souls
4. Opening the Chest to view your inventory
5. Equipping items to your loadout
