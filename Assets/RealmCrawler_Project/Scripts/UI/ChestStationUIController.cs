using UnityEngine;
using UnityEngine.UIElements;
using RealmCrawler.Core;
using RealmCrawler.Equipment;
using RealmCrawler.Player;
using RealmCrawler.CharacterStats;
using System.Collections;
using System.Collections.Generic;

namespace RealmCrawler.UI
{
    public class ChestStationUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement root;
        private HubStationManager stationManager;
        private GameManager gameManager;
        private PlayerInventory playerInventory;
        private PlayerVisualEquipment playerVisualEquipment;

        private Button backButton;
        private ListView inventoryList;

        private Button hatTabButton;
        private Button cloakTabButton;
        private Button bootsTabButton;
        private Button weaponTabButton;
        private Button reliquaryTabButton;
        private Button allTabButton;

        private Label currentCategoryLabel;
        private VisualElement equipmentDetailPanel;
        private VisualElement noSelectionPanel;
        private Label detailNameLabel;
        private Label detailStatsLabel;
        private VisualElement detailIconElement;

        private string currentCategory = "All";
        private List<InventoryItem> currentItems = new List<InventoryItem>();
        private InventoryItem selectedItem;

        private void OnEnable()
        {
            stationManager = FindFirstObjectByType<HubStationManager>();
            gameManager = FindFirstObjectByType<GameManager>();
            playerInventory = FindFirstObjectByType<PlayerInventory>();
            playerVisualEquipment = FindFirstObjectByType<PlayerVisualEquipment>();

            if (playerInventory == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerInventory = player.GetComponent<PlayerInventory>();
                    if (playerInventory == null)
                    {
                        playerInventory = player.AddComponent<PlayerInventory>();
                    }
                }
            }

            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }

            if (uiDocument == null || uiDocument.visualTreeAsset == null)
            {
                Debug.LogError("ChestStationUIController: UIDocument or VisualTreeAsset not assigned!");
                return;
            }

            root = uiDocument.rootVisualElement;
            BindUIElements();
            RegisterCallbacks();
            
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += RefreshInventoryList;
            }
            
            RefreshUI();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
            
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= RefreshInventoryList;
            }
        }

        private void BindUIElements()
        {
            backButton = root.Q<Button>("back-button");
            inventoryList = root.Q<ListView>("inventory-list");

            hatTabButton = root.Q<Button>("hat-tab");
            cloakTabButton = root.Q<Button>("cloak-tab");
            bootsTabButton = root.Q<Button>("boots-tab");
            weaponTabButton = root.Q<Button>("weapon-tab");
            reliquaryTabButton = root.Q<Button>("reliquary-tab");
            allTabButton = root.Q<Button>("all-tab");

            currentCategoryLabel = root.Q<Label>("category-label");
            equipmentDetailPanel = root.Q<VisualElement>("equipment-detail");
            noSelectionPanel = root.Q<VisualElement>("no-selection-panel");
            detailNameLabel = root.Q<Label>("detail-name");
            detailStatsLabel = root.Q<Label>("detail-stats");
            detailIconElement = root.Q<VisualElement>("detail-icon");

            if (inventoryList != null)
            {
                inventoryList.makeItem = MakeInventoryItem;
                inventoryList.bindItem = BindInventoryItem;
                inventoryList.selectionChanged += OnItemSelected;
                inventoryList.selectionType = SelectionType.Single;
                inventoryList.fixedItemHeight = 70;
                
                inventoryList.style.flexGrow = 1;
                inventoryList.style.minHeight = 200;
                
                Debug.Log("ChestStationUIController: ListView configured with fixedItemHeight=70, minHeight=200, and flexGrow=1");
            }
        }

        private void RegisterCallbacks()
        {
            if (backButton != null)
            {
                backButton.clicked += OnBackClicked;
            }

            if (hatTabButton != null)
            {
                hatTabButton.clicked += () => OnCategorySelected("Hat");
            }

            if (cloakTabButton != null)
            {
                cloakTabButton.clicked += () => OnCategorySelected("Cloak");
            }

            if (bootsTabButton != null)
            {
                bootsTabButton.clicked += () => OnCategorySelected("Boots");
            }

            if (weaponTabButton != null)
            {
                weaponTabButton.clicked += () => OnCategorySelected("Weapon");
            }

            if (reliquaryTabButton != null)
            {
                reliquaryTabButton.clicked += () => OnCategorySelected("Reliquary");
            }

            if (allTabButton != null)
            {
                allTabButton.clicked += () => OnCategorySelected("All");
            }
        }

        private void UnregisterCallbacks()
        {
            if (backButton != null)
            {
                backButton.clicked -= OnBackClicked;
            }

            if (hatTabButton != null)
            {
                hatTabButton.clicked -= () => OnCategorySelected("Hat");
            }

            if (cloakTabButton != null)
            {
                cloakTabButton.clicked -= () => OnCategorySelected("Cloak");
            }

            if (bootsTabButton != null)
            {
                bootsTabButton.clicked -= () => OnCategorySelected("Boots");
            }

            if (weaponTabButton != null)
            {
                weaponTabButton.clicked -= () => OnCategorySelected("Weapon");
            }

            if (reliquaryTabButton != null)
            {
                reliquaryTabButton.clicked -= () => OnCategorySelected("Reliquary");
            }

            if (allTabButton != null)
            {
                allTabButton.clicked -= () => OnCategorySelected("All");
            }
        }

        private void RefreshUI()
        {
            OnCategorySelected(currentCategory);
        }

        private void OnCategorySelected(string category)
        {
            currentCategory = category;
            
            UpdateTabStyles();
            
            if (currentCategoryLabel != null)
            {
                string displayText = category == "All" ? "All Items" : $"{category}s";
                currentCategoryLabel.text = displayText;
            }

            RefreshInventoryList();
            ClearSelection();
        }

        private void UpdateTabStyles()
        {
            RemoveActiveClass(allTabButton);
            RemoveActiveClass(hatTabButton);
            RemoveActiveClass(cloakTabButton);
            RemoveActiveClass(bootsTabButton);
            RemoveActiveClass(weaponTabButton);
            RemoveActiveClass(reliquaryTabButton);

            switch (currentCategory)
            {
                case "All":
                    AddActiveClass(allTabButton);
                    break;
                case "Hat":
                    AddActiveClass(hatTabButton);
                    break;
                case "Cloak":
                    AddActiveClass(cloakTabButton);
                    break;
                case "Boots":
                    AddActiveClass(bootsTabButton);
                    break;
                case "Weapon":
                    AddActiveClass(weaponTabButton);
                    break;
                case "Reliquary":
                    AddActiveClass(reliquaryTabButton);
                    break;
            }
        }

        private void AddActiveClass(Button button)
        {
            if (button != null)
                button.AddToClassList("category-tab--active");
        }

        private void RemoveActiveClass(Button button)
        {
            if (button != null)
                button.RemoveFromClassList("category-tab--active");
        }

        private void ClearSelection()
        {
            if (inventoryList != null)
            {
                inventoryList.ClearSelection();
            }
            selectedItem = null;
            HideItemDetail();
        }

        private void RefreshInventoryList()
        {
            if (playerInventory == null)
            {
                currentItems.Clear();
                if (inventoryList != null)
                {
                    inventoryList.itemsSource = currentItems;
                    inventoryList.Rebuild();
                }
                return;
            }

            currentItems.Clear();

            switch (currentCategory)
            {
                case "Hat":
                    currentItems = playerInventory.GetItemsByCategory(EquipmentCategory.Hat);
                    break;
                case "Cloak":
                    currentItems = playerInventory.GetItemsByCategory(EquipmentCategory.Cloak);
                    break;
                case "Boots":
                    currentItems = playerInventory.GetItemsByCategory(EquipmentCategory.Boots);
                    break;
                case "Reliquary":
                    currentItems = playerInventory.GetItemsByCategory(EquipmentCategory.Reliquary);
                    break;
                case "Weapon":
                    currentItems = playerInventory.GetWeapons();
                    break;
                case "All":
                default:
                    currentItems = new List<InventoryItem>(playerInventory.Items);
                    break;
            }

            if (inventoryList != null)
            {
                inventoryList.itemsSource = currentItems;
                inventoryList.Rebuild();
                Debug.Log($"ChestStationUIController: ListView rebuilt with {currentItems.Count} items. ListView.itemsSource count: {inventoryList.itemsSource?.Count ?? 0}");
                Debug.Log($"ChestStationUIController: ListView layout - width: {inventoryList.layout.width}, height: {inventoryList.layout.height}, visible: {inventoryList.visible}, display: {inventoryList.resolvedStyle.display}");
                
                StartCoroutine(RebuildListViewNextFrame());
            }

            Debug.Log($"ChestStationUIController: Viewing {currentCategory} inventory ({currentItems.Count} items)");
            
            ClearSelection();
        }

        private VisualElement MakeInventoryItem()
        {
            Debug.Log("🔨 MakeInventoryItem called - creating new item template");
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingTop = 8;
            container.style.paddingBottom = 8;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.minHeight = 60;
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            container.style.marginBottom = 5;

            VisualElement icon = new VisualElement();
            icon.name = "item-icon";
            icon.style.width = 50;
            icon.style.height = 50;
            icon.style.marginRight = 10;
            icon.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);

            Label nameLabel = new Label();
            nameLabel.name = "item-name";
            nameLabel.style.fontSize = 18;
            nameLabel.style.color = Color.white;
            nameLabel.style.flexGrow = 1;

            Label quantityLabel = new Label();
            quantityLabel.name = "item-quantity";
            quantityLabel.style.fontSize = 16;
            quantityLabel.style.color = new Color(0.8f, 0.8f, 0.8f);
            quantityLabel.style.width = 60;
            quantityLabel.style.unityTextAlign = TextAnchor.MiddleRight;

            Button equipItemButton = new Button();
            equipItemButton.name = "equip-item-button";
            equipItemButton.text = "EQUIP";
            equipItemButton.style.width = 100;
            equipItemButton.style.height = 40;
            equipItemButton.style.marginLeft = 10;
            equipItemButton.style.backgroundColor = new Color(0.2f, 0.6f, 0.2f);
            equipItemButton.style.color = Color.white;
            equipItemButton.style.fontSize = 14;
            equipItemButton.style.borderTopLeftRadius = 5;
            equipItemButton.style.borderTopRightRadius = 5;
            equipItemButton.style.borderBottomLeftRadius = 5;
            equipItemButton.style.borderBottomRightRadius = 5;

            container.Add(icon);
            container.Add(nameLabel);
            container.Add(quantityLabel);
            container.Add(equipItemButton);

            return container;
        }

        private void BindInventoryItem(VisualElement element, int index)
        {
            try
            {
                Debug.Log($"🔧 BindInventoryItem called: index={index}, currentItems.Count={currentItems.Count}");
                
                if (index < 0 || index >= currentItems.Count)
                {
                    Debug.LogWarning($"BindInventoryItem: Invalid index {index} for {currentItems.Count} items");
                    return;
                }

                InventoryItem invItem = currentItems[index];
                Debug.Log($"🔧 Binding item: {invItem.itemData?.name}");
                
                VisualElement icon = element.Q<VisualElement>("item-icon");
                Label nameLabel = element.Q<Label>("item-name");
                Label quantityLabel = element.Q<Label>("item-quantity");
                Button equipItemButton = element.Q<Button>("equip-item-button");

                string itemName = "";
                Sprite iconSprite = null;
                bool isEquipped = false;

                if (invItem.itemData is EquipmentData equipment)
                {
                    itemName = equipment.ItemName;
                    iconSprite = equipment.ItemIcon;
                    isEquipped = IsItemEquipped(equipment);
                }
                else if (invItem.itemData is WeaponData weapon)
                {
                    itemName = weapon.ItemName;
                    iconSprite = weapon.ItemIcon;
                    isEquipped = IsItemEquipped(weapon);
                }

                nameLabel.text = itemName;
                quantityLabel.text = $"x{invItem.quantity}";

                if (iconSprite != null)
                {
                    icon.style.backgroundImage = new StyleBackground(iconSprite);
                }
                else
                {
                    icon.style.backgroundImage = StyleKeyword.None;
                }

                if (equipItemButton != null)
                {
                    if (equipItemButton.userData is System.Action oldCallback)
                    {
                        equipItemButton.clicked -= oldCallback;
                    }
                    
                    System.Action newCallback = () =>
                    {
                        Debug.Log($"🔵 EQUIP BUTTON CLICKED for {invItem.itemData.name}");
                        OnEquipItemFromList(invItem);
                    };
                    equipItemButton.clicked += newCallback;
                    equipItemButton.userData = newCallback;
                    
                    if (isEquipped)
                    {
                        equipItemButton.text = "UNEQUIP";
                        equipItemButton.style.backgroundColor = new Color(0.8f, 0.6f, 0.0f);
                    }
                    else
                    {
                        equipItemButton.text = "EQUIP";
                        equipItemButton.style.backgroundColor = new Color(0.2f, 0.6f, 0.2f);
                    }
                    
                    Debug.Log($"ChestStation: Registered equip button for {itemName}, isEquipped: {isEquipped}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"BindInventoryItem error at index {index}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private bool IsItemEquipped(ScriptableObject itemData)
        {
            if (gameManager == null || gameManager.CurrentLoadout == null)
                return false;

            if (itemData is HatData hat)
            {
                return gameManager.CurrentLoadout.hat == hat;
            }
            else if (itemData is CloakData cloak)
            {
                return gameManager.CurrentLoadout.cloak == cloak;
            }
            else if (itemData is BootsData boots)
            {
                return gameManager.CurrentLoadout.boots == boots;
            }
            else if (itemData is ReliquaryData reliquary)
            {
                return gameManager.CurrentLoadout.reliquary == reliquary;
            }
            else if (itemData is WeaponData weapon)
            {
                return gameManager.CurrentLoadout.weapon == weapon;
            }

            return false;
        }

        private void OnItemSelected(IEnumerable<object> selectedItems)
        {
            Debug.Log("ChestStationUIController: OnItemSelected called");
            
            foreach (object item in selectedItems)
            {
                selectedItem = item as InventoryItem;
                
                if (selectedItem != null)
                {
                    Debug.Log($"ChestStationUIController: Item selected - {selectedItem.itemData.name}");
                    ShowItemDetail(selectedItem);
                }
                else
                {
                    Debug.LogWarning("ChestStationUIController: Selected item is not InventoryItem type");
                }
                
                return;
            }
        }

        private void ShowItemDetail(InventoryItem invItem)
        {
            if (invItem == null)
            {
                HideItemDetail();
                return;
            }

            if (equipmentDetailPanel != null)
                equipmentDetailPanel.style.display = DisplayStyle.Flex;
            
            if (noSelectionPanel != null)
                noSelectionPanel.style.display = DisplayStyle.None;

            string itemName = "";
            string stats = "";
            Sprite iconSprite = null;

            if (invItem.itemData is EquipmentData equipment)
            {
                itemName = equipment.ItemName;
                iconSprite = equipment.ItemIcon;
                stats = GetEquipmentStatsText(equipment);
            }
            else if (invItem.itemData is WeaponData weapon)
            {
                itemName = weapon.ItemName;
                iconSprite = weapon.ItemIcon;
                stats = GetWeaponStatsText(weapon);
            }

            if (detailNameLabel != null)
                detailNameLabel.text = itemName;

            if (detailStatsLabel != null)
                detailStatsLabel.text = stats;

            if (detailIconElement != null)
            {
                if (iconSprite != null)
                {
                    detailIconElement.style.backgroundImage = new StyleBackground(iconSprite);
                }
                else
                {
                    detailIconElement.style.backgroundImage = StyleKeyword.None;
                }
            }
        }

        private void HideItemDetail()
        {
            if (equipmentDetailPanel != null)
                equipmentDetailPanel.style.display = DisplayStyle.None;
            
            if (noSelectionPanel != null)
                noSelectionPanel.style.display = DisplayStyle.Flex;
        }

        private string GetEquipmentStatsText(EquipmentData equipment)
        {
            if (equipment == null) return "No data available";
            
            string text = "";
            
            text += $"Type: {equipment.Category}\n";
            text += $"Rental Cost: {equipment.RentalCost} Souls\n\n";
            
            if (!string.IsNullOrEmpty(equipment.FlavorText))
            {
                text += $"\"{equipment.FlavorText}\"\n\n";
            }
            
            text += "MODIFIERS:\n";
            int modCount = 0;
            
            if (equipment.StaticModifier != null)
            {
                text += $"• {GetModifierDisplayText(equipment.StaticModifier)}\n";
                modCount++;
            }
            
            foreach (var mod in equipment.Modifiers)
            {
                if (mod != null)
                {
                    text += $"• {GetModifierDisplayText(mod)}\n";
                    modCount++;
                }
            }
            
            if (modCount == 0)
                text += "• No modifiers\n";
            
            return text;
        }

        private string GetWeaponStatsText(WeaponData weapon)
        {
            if (weapon == null) return "No data available";
            
            string text = "";
            
            text += "Type: Weapon\n";
            text += $"Rental Cost: {weapon.RentalCost} Souls\n\n";
            
            if (!string.IsNullOrEmpty(weapon.FlavorText))
            {
                text += $"\"{weapon.FlavorText}\"\n\n";
            }
            
            text += "CANTRIPS:\n";
            text += $"• Primary: {weapon.PrimaryCantrip?.CantripName ?? "None"}\n";
            text += $"• Secondary: {weapon.SecondaryCantrip?.CantripName ?? "None"}\n";
            
            return text;
        }

        private string GetModifierStatsText(EquipmentData equipment)
        {
            return GetEquipmentStatsText(equipment);
        }

        private string GetModifierDisplayText(StatModifierBase modifier)
        {
            if (modifier == null)
                return "Unknown Modifier";

            if (modifier is SimpleStatModifier simpleMod)
            {
                string statName = simpleMod.Stat != null ? simpleMod.Stat.StatName : "Unknown";
                string valueText = simpleMod.ModType == ModifierType.Percent 
                    ? $"{simpleMod.Value:F1}%" 
                    : $"+{simpleMod.Value:F0}";
                return $"{statName} {valueText}";
            }

            return modifier.name;
        }

        private void OnEquipItemFromList(InventoryItem item)
        {
            Debug.Log($"ChestStationUIController: OnEquipItemFromList called for {item.itemData.name}");
            
            if (item == null || item.itemData == null)
            {
                Debug.LogWarning("ChestStationUIController: Invalid item in OnEquipItemFromList");
                return;
            }

            if (gameManager == null)
            {
                Debug.LogError("ChestStationUIController: GameManager not found! Cannot equip items without GameManager.");
                return;
            }

            if (gameManager.CurrentLoadout == null)
            {
                Debug.LogError("ChestStationUIController: CurrentLoadout is null! Cannot equip items.");
                return;
            }

            CharacterData characterData = FindFirstObjectByType<CharacterData>();
            if (characterData == null)
            {
                Debug.LogError("ChestStationUIController: CharacterData not found! Cannot apply modifiers.");
                return;
            }

            bool isCurrentlyEquipped = IsItemEquipped(item.itemData);

            if (item.itemData is HatData hat)
            {
                if (isCurrentlyEquipped)
                {
                    RemoveEquipmentModifiers(gameManager.CurrentLoadout.hat, characterData);
                    gameManager.CurrentLoadout.hat = null;
                    Debug.Log($"🔸 ChestStationUIController: Unequipped Hat - {hat.ItemName}");
                }
                else
                {
                    if (gameManager.CurrentLoadout.hat != null)
                    {
                        RemoveEquipmentModifiers(gameManager.CurrentLoadout.hat, characterData);
                    }
                    gameManager.CurrentLoadout.hat = hat;
                    ApplyEquipmentModifiers(hat, characterData);
                    Debug.Log($"✅ ChestStationUIController: Successfully equipped Hat - {hat.ItemName}");
                }
            }
            else if (item.itemData is CloakData cloak)
            {
                if (isCurrentlyEquipped)
                {
                    RemoveEquipmentModifiers(gameManager.CurrentLoadout.cloak, characterData);
                    gameManager.CurrentLoadout.cloak = null;
                    Debug.Log($"🔸 ChestStationUIController: Unequipped Cloak - {cloak.ItemName}");
                }
                else
                {
                    if (gameManager.CurrentLoadout.cloak != null)
                    {
                        RemoveEquipmentModifiers(gameManager.CurrentLoadout.cloak, characterData);
                    }
                    gameManager.CurrentLoadout.cloak = cloak;
                    ApplyEquipmentModifiers(cloak, characterData);
                    Debug.Log($"✅ ChestStationUIController: Successfully equipped Cloak - {cloak.ItemName}");
                }
            }
            else if (item.itemData is BootsData boots)
            {
                if (isCurrentlyEquipped)
                {
                    RemoveEquipmentModifiers(gameManager.CurrentLoadout.boots, characterData);
                    gameManager.CurrentLoadout.boots = null;
                    Debug.Log($"🔸 ChestStationUIController: Unequipped Boots - {boots.ItemName}");
                }
                else
                {
                    if (gameManager.CurrentLoadout.boots != null)
                    {
                        RemoveEquipmentModifiers(gameManager.CurrentLoadout.boots, characterData);
                    }
                    gameManager.CurrentLoadout.boots = boots;
                    ApplyEquipmentModifiers(boots, characterData);
                    Debug.Log($"✅ ChestStationUIController: Successfully equipped Boots - {boots.ItemName}");
                }
            }
            else if (item.itemData is ReliquaryData reliquary)
            {
                if (isCurrentlyEquipped)
                {
                    RemoveEquipmentModifiers(gameManager.CurrentLoadout.reliquary, characterData);
                    gameManager.CurrentLoadout.reliquary = null;
                    Debug.Log($"🔸 ChestStationUIController: Unequipped Reliquary - {reliquary.ItemName}");
                }
                else
                {
                    if (gameManager.CurrentLoadout.reliquary != null)
                    {
                        RemoveEquipmentModifiers(gameManager.CurrentLoadout.reliquary, characterData);
                    }
                    gameManager.CurrentLoadout.reliquary = reliquary;
                    ApplyEquipmentModifiers(reliquary, characterData);
                    Debug.Log($"✅ ChestStationUIController: Successfully equipped Reliquary - {reliquary.ItemName}");
                }
            }
            else if (item.itemData is WeaponData weapon)
            {
                if (isCurrentlyEquipped)
                {
                    gameManager.CurrentLoadout.weapon = null;
                    Debug.Log($"🔸 ChestStationUIController: Unequipped Weapon - {weapon.ItemName}");
                }
                else
                {
                    gameManager.CurrentLoadout.weapon = weapon;
                    Debug.Log($"✅ ChestStationUIController: Successfully equipped Weapon - {weapon.ItemName}");
                }
            }
            else
            {
                Debug.LogWarning($"ChestStationUIController: Unknown item type - {item.itemData.GetType().Name}");
            }

            UpdatePlayerVisuals();
            RefreshInventoryList();
            
            MainHubUIController mainHubUI = FindFirstObjectByType<MainHubUIController>();
            if (mainHubUI != null)
            {
                mainHubUI.RefreshUI();
                Debug.Log("ChestStationUIController: Triggered MainHubUI stats refresh after equipping");
            }
        }

        private void ApplyEquipmentModifiers(EquipmentData equipment, CharacterData characterData)
        {
            if (equipment != null && characterData != null)
            {
                Debug.Log($"ChestStation: Applying modifiers from {equipment.ItemName}");
                equipment.ApplyModifiers(characterData);
                Debug.Log($"ChestStation: Modifiers applied from {equipment.ItemName}");
            }
            else
            {
                Debug.LogWarning($"ChestStation: Cannot apply modifiers - equipment: {equipment != null}, characterData: {characterData != null}");
            }
        }

        private void RemoveEquipmentModifiers(EquipmentData equipment, CharacterData characterData)
        {
            if (equipment == null || characterData == null) return;

            System.Reflection.FieldInfo preModifiersField = characterData.GetType().GetField("preModifiers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.FieldInfo postModifiersField = characterData.GetType().GetField("postModifiers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (preModifiersField == null || postModifiersField == null)
            {
                Debug.LogWarning("Could not access modifier fields in CharacterData");
                return;
            }

            var preModifiers = preModifiersField.GetValue(characterData) as System.Collections.Generic.List<StatModifierHandlerBase>;
            var postModifiers = postModifiersField.GetValue(characterData) as System.Collections.Generic.List<StatModifierHandlerBase>;

            System.Collections.Generic.List<StatModifierHandlerBase> modifiersToRemove = new System.Collections.Generic.List<StatModifierHandlerBase>();

            if (equipment.StaticModifier != null)
            {
                string staticModifierName = $"{equipment.StaticModifier.name}_Handler";
                foreach (var handler in preModifiers)
                {
                    if (handler != null && handler.gameObject.name == staticModifierName)
                    {
                        modifiersToRemove.Add(handler);
                    }
                }
                foreach (var handler in postModifiers)
                {
                    if (handler != null && handler.gameObject.name == staticModifierName)
                    {
                        modifiersToRemove.Add(handler);
                    }
                }
            }

            foreach (var modifier in equipment.Modifiers)
            {
                if (modifier != null)
                {
                    string modifierName = $"{modifier.name}_Handler";
                    foreach (var handler in preModifiers)
                    {
                        if (handler != null && handler.gameObject.name == modifierName)
                        {
                            modifiersToRemove.Add(handler);
                        }
                    }
                    foreach (var handler in postModifiers)
                    {
                        if (handler != null && handler.gameObject.name == modifierName)
                        {
                            modifiersToRemove.Add(handler);
                        }
                    }
                }
            }

            foreach (var handler in modifiersToRemove)
            {
                characterData.RemoveModifier(handler);
            }

            Debug.Log($"Removed modifiers from {equipment.ItemName} from CharacterData");
        }

        private void UpdatePlayerVisuals()
        {
            if (playerVisualEquipment == null)
            {
                Debug.LogWarning("PlayerVisualEquipment not found in scene. Skipping visual update.");
                return;
            }

            if (gameManager == null || gameManager.CurrentLoadout == null)
            {
                Debug.LogWarning("GameManager or CurrentLoadout is null. Cannot update visuals.");
                return;
            }

            playerVisualEquipment.ApplyLoadout(gameManager.CurrentLoadout);
            Debug.Log("✅ ChestStationUIController: Player visuals updated with new loadout!");
        }

        private void OnBackClicked()
        {
            if (stationManager != null)
            {
                stationManager.ReturnToMainHub();
            }
        }

        private IEnumerator RebuildListViewNextFrame()
        {
            yield return null;
            if (inventoryList != null)
            {
                Debug.Log($"ChestStationUIController: Rebuild next frame - layout width: {inventoryList.layout.width}, height: {inventoryList.layout.height}");
                inventoryList.Rebuild();
                inventoryList.RefreshItems();
            }
        }
    }
}
