using UnityEngine;
using UnityEngine.UIElements;
using RealmCrawler.Core;
using RealmCrawler.Equipment;
using RealmCrawler.Player;
using RealmCrawler.CharacterStats;
using System.Collections.Generic;
using System.Linq;

namespace RealmCrawler.UI
{
    public class NPCStationUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private ShopData shopData;
        [SerializeField] private AlterationsConfig alterationsConfig;

        private VisualElement root;
        private HubStationManager stationManager;
        private GameManager gameManager;
        private PlayerInventory playerInventory;
        private PlayerVisualEquipment playerVisualEquipment;
        private AlterationsSystem alterationsSystem;

        private Button backButton;
        private Button shopWaresButton;
        private Button alterationsButton;

        private VisualElement mainMenuPanel;
        private VisualElement shopWaresPanel;
        private VisualElement alterationsPanel;
        
        private Label npcDialogueLabel;
        private Label soulsLabel;
        private Label shopSoulsLabel;
        
        private ListView npcInventoryList;
        private ListView playerInventoryList;
        private Button shopBackButton;
        private Button alterationsBackButton;
        
        private Button npcAllTabButton;
        private Button npcHatTabButton;
        private Button npcCloakTabButton;
        private Button npcBootsTabButton;
        private Button npcWeaponTabButton;
        private Button npcReliquaryTabButton;
        
        private VisualElement itemDetailPanel;
        private VisualElement noItemSelectedPanel;
        private Label detailNameLabel;
        private Label detailPriceLabel;
        private Label detailStatsLabel;
        private VisualElement detailIconElement;
        private Button actionButton;
        
        private ListView alterationsInventoryList;
        private VisualElement alterationsDetailPanel;
        private VisualElement alterationsNoSelectionPanel;
        private Label alterationsSoulsLabel;
        private Label alterationsDetailName;
        private VisualElement alterationsDetailIcon;
        private VisualElement modifiersContainer;
        private Label rerollCostLabel;
        private Button rerollButton;
        
        private string currentNPCCategory = "All";
        private List<ShopItemEntry> currentNPCItems = new List<ShopItemEntry>();
        private List<InventoryItem> currentPlayerItems = new List<InventoryItem>();
        private List<InventoryItem> currentAlterationsItems = new List<InventoryItem>();
        private object selectedItem;
        private bool isNPCItemSelected;
        
        private InventoryItem selectedAlterationItem;
        private List<int> selectedModifierIndices = new List<int>();

        private void OnEnable()
        {
            stationManager = FindFirstObjectByType<HubStationManager>();
            gameManager = FindFirstObjectByType<GameManager>();
            playerInventory = FindFirstObjectByType<PlayerInventory>();
            playerVisualEquipment = FindFirstObjectByType<PlayerVisualEquipment>();

            if (alterationsConfig != null)
            {
                alterationsSystem = new AlterationsSystem(alterationsConfig);
            }

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
                Debug.LogError("NPCStationUIController: UIDocument or VisualTreeAsset not assigned!");
                return;
            }

            root = uiDocument.rootVisualElement;
            BindUIElements();
            RegisterCallbacks();
            
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += RefreshPlayerInventoryList;
            }
            
            RefreshUI();
            ShowMainMenu();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
            
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= RefreshPlayerInventoryList;
            }
        }

        private void BindUIElements()
        {
            backButton = root.Q<Button>("back-button");
            shopWaresButton = root.Q<Button>("shop-wares-button");
            alterationsButton = root.Q<Button>("alterations-button");

            mainMenuPanel = root.Q<VisualElement>("main-menu-panel");
            shopWaresPanel = root.Q<VisualElement>("shop-wares-panel");
            alterationsPanel = root.Q<VisualElement>("alterations-panel");
            
            npcDialogueLabel = root.Q<Label>("npc-dialogue");
            soulsLabel = root.Q<Label>("souls-label");
            shopSoulsLabel = root.Q<Label>("shop-souls-label");
            
            npcInventoryList = root.Q<ListView>("npc-inventory-list");
            playerInventoryList = root.Q<ListView>("player-inventory-list");
            shopBackButton = root.Q<Button>("shop-back-button");
            alterationsBackButton = root.Q<Button>("alterations-back-button");
            
            if (shopBackButton == null)
            {
                Debug.LogError("NPCStationUIController: shop-back-button not found in UXML!");
            }
            else
            {
                Debug.Log("NPCStationUIController: shop-back-button found successfully");
            }
            
            npcAllTabButton = root.Q<Button>("npc-all-tab");
            npcHatTabButton = root.Q<Button>("npc-hat-tab");
            npcCloakTabButton = root.Q<Button>("npc-cloak-tab");
            npcBootsTabButton = root.Q<Button>("npc-boots-tab");
            npcWeaponTabButton = root.Q<Button>("npc-weapon-tab");
            npcReliquaryTabButton = root.Q<Button>("npc-reliquary-tab");
            
            itemDetailPanel = root.Q<VisualElement>("item-detail-panel");
            noItemSelectedPanel = root.Q<VisualElement>("no-item-selected-panel");
            detailNameLabel = root.Q<Label>("detail-name");
            detailPriceLabel = root.Q<Label>("detail-price");
            detailStatsLabel = root.Q<Label>("detail-stats");
            detailIconElement = root.Q<VisualElement>("detail-icon");
            actionButton = root.Q<Button>("action-button");
            
            alterationsInventoryList = root.Q<ListView>("alterations-inventory-list");
            alterationsDetailPanel = root.Q<VisualElement>("alterations-detail-panel");
            alterationsNoSelectionPanel = root.Q<VisualElement>("alterations-no-selection-panel");
            alterationsSoulsLabel = root.Q<Label>("alterations-souls-label");
            alterationsDetailName = root.Q<Label>("alterations-detail-name");
            alterationsDetailIcon = root.Q<VisualElement>("alterations-detail-icon");
            modifiersContainer = root.Q<VisualElement>("modifiers-container");
            rerollCostLabel = root.Q<Label>("reroll-cost-label");
            rerollButton = root.Q<Button>("reroll-button");

            if (npcInventoryList != null)
            {
                npcInventoryList.makeItem = MakeNPCInventoryItem;
                npcInventoryList.bindItem = BindNPCInventoryItem;
                npcInventoryList.selectionChanged += OnNPCItemSelected;
                npcInventoryList.selectionType = SelectionType.Single;
                npcInventoryList.fixedItemHeight = 70;
            }
            
            if (playerInventoryList != null)
            {
                playerInventoryList.makeItem = MakePlayerInventoryItem;
                playerInventoryList.bindItem = BindPlayerInventoryItem;
                playerInventoryList.selectionChanged += OnPlayerItemSelected;
                playerInventoryList.selectionType = SelectionType.Single;
                playerInventoryList.fixedItemHeight = 70;
            }
            
            if (alterationsInventoryList != null)
            {
                alterationsInventoryList.makeItem = MakeAlterationsInventoryItem;
                alterationsInventoryList.bindItem = BindAlterationsInventoryItem;
                alterationsInventoryList.selectionChanged += OnAlterationsItemSelected;
                alterationsInventoryList.selectionType = SelectionType.Single;
                alterationsInventoryList.fixedItemHeight = 70;
            }
        }

        private void RegisterCallbacks()
        {
            if (backButton != null)
            {
                backButton.clicked += OnBackClicked;
            }

            if (shopWaresButton != null)
            {
                shopWaresButton.clicked += OnShopWaresClicked;
            }

            if (alterationsButton != null)
            {
                alterationsButton.clicked += OnAlterationsClicked;
            }

            if (shopBackButton != null)
            {
                Debug.Log("NPCStationUIController: Registering shop-back-button callback");
                shopBackButton.clicked += OnShopBackClicked;
            }
            else
            {
                Debug.LogWarning("NPCStationUIController: shopBackButton is null, cannot register callback");
            }
            
            if (alterationsBackButton != null)
            {
                alterationsBackButton.clicked += OnAlterationsBackClicked;
            }
            
            if (npcAllTabButton != null)
            {
                npcAllTabButton.clicked += () => OnNPCCategorySelected("All");
            }

            if (npcHatTabButton != null)
            {
                npcHatTabButton.clicked += () => OnNPCCategorySelected("Hat");
            }

            if (npcCloakTabButton != null)
            {
                npcCloakTabButton.clicked += () => OnNPCCategorySelected("Cloak");
            }

            if (npcBootsTabButton != null)
            {
                npcBootsTabButton.clicked += () => OnNPCCategorySelected("Boots");
            }

            if (npcWeaponTabButton != null)
            {
                npcWeaponTabButton.clicked += () => OnNPCCategorySelected("Weapon");
            }

            if (npcReliquaryTabButton != null)
            {
                npcReliquaryTabButton.clicked += () => OnNPCCategorySelected("Reliquary");
            }
            
            if (actionButton != null)
            {
                actionButton.clicked += OnActionButtonClicked;
            }
            
            if (rerollButton != null)
            {
                rerollButton.clicked += OnRerollButtonClicked;
            }
        }

        private void UnregisterCallbacks()
        {
            if (backButton != null)
            {
                backButton.clicked -= OnBackClicked;
            }

            if (shopWaresButton != null)
            {
                shopWaresButton.clicked -= OnShopWaresClicked;
            }

            if (alterationsButton != null)
            {
                alterationsButton.clicked -= OnAlterationsClicked;
            }

            if (shopBackButton != null)
            {
                shopBackButton.clicked -= OnShopBackClicked;
            }
            
            if (alterationsBackButton != null)
            {
                alterationsBackButton.clicked -= OnAlterationsBackClicked;
            }
            
            if (npcAllTabButton != null)
            {
                npcAllTabButton.clicked -= () => OnNPCCategorySelected("All");
            }

            if (npcHatTabButton != null)
            {
                npcHatTabButton.clicked -= () => OnNPCCategorySelected("Hat");
            }

            if (npcCloakTabButton != null)
            {
                npcCloakTabButton.clicked -= () => OnNPCCategorySelected("Cloak");
            }

            if (npcBootsTabButton != null)
            {
                npcBootsTabButton.clicked -= () => OnNPCCategorySelected("Boots");
            }

            if (npcWeaponTabButton != null)
            {
                npcWeaponTabButton.clicked -= () => OnNPCCategorySelected("Weapon");
            }

            if (npcReliquaryTabButton != null)
            {
                npcReliquaryTabButton.clicked -= () => OnNPCCategorySelected("Reliquary");
            }
            
            if (actionButton != null)
            {
                actionButton.clicked -= OnActionButtonClicked;
            }
            
            if (rerollButton != null)
            {
                rerollButton.clicked -= OnRerollButtonClicked;
            }
            
            if (alterationsInventoryList != null)
            {
                alterationsInventoryList.selectionChanged -= OnAlterationsItemSelected;
            }
        }

        private void RefreshUI()
        {
            if (npcDialogueLabel != null)
            {
                string dialogue = shopData != null ? shopData.ShopDialogue : "Welcome, traveler. What can I help you with?";
                npcDialogueLabel.text = dialogue;
            }

            if (soulsLabel != null && gameManager != null)
            {
                soulsLabel.text = $"Souls: {gameManager.Souls}";
            }
            
            if (shopSoulsLabel != null && gameManager != null)
            {
                shopSoulsLabel.text = $"Souls: {gameManager.Souls}";
            }
            
            if (alterationsSoulsLabel != null && gameManager != null)
            {
                alterationsSoulsLabel.text = $"Souls: {gameManager.Souls}";
            }
        }

        private void OnBackClicked()
        {
            if (stationManager != null)
            {
                stationManager.ReturnToMainHub();
            }
        }

        private void OnShopWaresClicked()
        {
            if (shopData == null)
            {
                Debug.LogError("NPCStationUIController: Cannot open shop - ShopData is not assigned in the Inspector!");
                return;
            }
            
            OpenShopWares();
        }

        private void OnAlterationsClicked()
        {
            ShowAlterationsPanel();
        }
        
        private void OnShopBackClicked()
        {
            Debug.Log("NPCStationUIController: OnShopBackClicked called!");
            ClearSelection();
            ShowMainMenu();
        }
        
        private void OnAlterationsBackClicked()
        {
            ShowMainMenu();
        }
        
        private void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.Flex;
            if (shopWaresPanel != null) shopWaresPanel.style.display = DisplayStyle.None;
            if (alterationsPanel != null) alterationsPanel.style.display = DisplayStyle.None;
        }

        private void ShowShopWaresPanel()
        {
            if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.None;
            if (shopWaresPanel != null) shopWaresPanel.style.display = DisplayStyle.Flex;
            if (alterationsPanel != null) alterationsPanel.style.display = DisplayStyle.None;
        }
        
        private void ShowAlterationsPanel()
        {
            if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.None;
            if (shopWaresPanel != null) shopWaresPanel.style.display = DisplayStyle.None;
            if (alterationsPanel != null) alterationsPanel.style.display = DisplayStyle.Flex;
            
            RefreshAlterationsInventoryList();
            ClearAlterationsSelection();
            RefreshUI();
        }

        private void OpenShopWares()
        {
            currentNPCCategory = "All";
            RefreshNPCInventoryList();
            RefreshPlayerInventoryList();
            UpdateCategoryTabs();
            ShowShopWaresPanel();
            RefreshUI();
            ClearSelection();
        }
        
        private void OnNPCCategorySelected(string category)
        {
            currentNPCCategory = category;
            RefreshNPCInventoryList();
            UpdateCategoryTabs();
            ClearSelection();
        }
        
        private void UpdateCategoryTabs()
        {
            const string activeClass = "category-tab--active";
            
            npcAllTabButton?.RemoveFromClassList(activeClass);
            npcHatTabButton?.RemoveFromClassList(activeClass);
            npcCloakTabButton?.RemoveFromClassList(activeClass);
            npcBootsTabButton?.RemoveFromClassList(activeClass);
            npcWeaponTabButton?.RemoveFromClassList(activeClass);
            npcReliquaryTabButton?.RemoveFromClassList(activeClass);

            switch (currentNPCCategory)
            {
                case "All":
                    npcAllTabButton?.AddToClassList(activeClass);
                    break;
                case "Hat":
                    npcHatTabButton?.AddToClassList(activeClass);
                    break;
                case "Cloak":
                    npcCloakTabButton?.AddToClassList(activeClass);
                    break;
                case "Boots":
                    npcBootsTabButton?.AddToClassList(activeClass);
                    break;
                case "Weapon":
                    npcWeaponTabButton?.AddToClassList(activeClass);
                    break;
                case "Reliquary":
                    npcReliquaryTabButton?.AddToClassList(activeClass);
                    break;
            }
        }
        
        private void RefreshNPCInventoryList()
        {
            if (shopData == null)
                return;

            currentNPCItems.Clear();

            switch (currentNPCCategory)
            {
                case "All":
                    currentNPCItems.AddRange(shopData.GetAvailableHats());
                    currentNPCItems.AddRange(shopData.GetAvailableCloaks());
                    currentNPCItems.AddRange(shopData.GetAvailableBoots());
                    currentNPCItems.AddRange(shopData.GetAvailableReliquaries());
                    currentNPCItems.AddRange(shopData.GetAvailableWeapons());
                    break;
                case "Hat":
                    currentNPCItems.AddRange(shopData.GetAvailableHats());
                    break;
                case "Cloak":
                    currentNPCItems.AddRange(shopData.GetAvailableCloaks());
                    break;
                case "Boots":
                    currentNPCItems.AddRange(shopData.GetAvailableBoots());
                    break;
                case "Reliquary":
                    currentNPCItems.AddRange(shopData.GetAvailableReliquaries());
                    break;
                case "Weapon":
                    currentNPCItems.AddRange(shopData.GetAvailableWeapons());
                    break;
            }

            currentNPCItems.RemoveAll(item => item.item == null);

            if (npcInventoryList != null)
            {
                npcInventoryList.itemsSource = currentNPCItems;
                npcInventoryList.Rebuild();
            }
        }
        
        private void RefreshPlayerInventoryList()
        {
            if (playerInventory == null)
                return;

            currentPlayerItems.Clear();
            currentPlayerItems.AddRange(playerInventory.Items);

            if (playerInventoryList != null)
            {
                playerInventoryList.itemsSource = currentPlayerItems;
                playerInventoryList.Rebuild();
            }
        }

        private VisualElement MakeNPCInventoryItem()
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingTop = 8;
            container.style.paddingBottom = 8;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.minHeight = 70;
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            container.style.marginBottom = 5;
            container.style.borderBottomLeftRadius = 5;
            container.style.borderBottomRightRadius = 5;
            container.style.borderTopLeftRadius = 5;
            container.style.borderTopRightRadius = 5;

            VisualElement icon = new VisualElement();
            icon.name = "item-icon";
            icon.style.width = 50;
            icon.style.height = 50;
            icon.style.marginRight = 10;
            icon.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            icon.style.borderTopLeftRadius = 5;
            icon.style.borderTopRightRadius = 5;
            icon.style.borderBottomLeftRadius = 5;
            icon.style.borderBottomRightRadius = 5;

            VisualElement textContainer = new VisualElement();
            textContainer.style.flexGrow = 1;
            textContainer.style.flexDirection = FlexDirection.Column;

            Label nameLabel = new Label();
            nameLabel.name = "item-name";
            nameLabel.style.fontSize = 18;
            nameLabel.style.color = Color.white;

            Label priceLabel = new Label();
            priceLabel.name = "item-price";
            priceLabel.style.fontSize = 14;
            priceLabel.style.color = new Color(1f, 0.84f, 0f);

            textContainer.Add(nameLabel);
            textContainer.Add(priceLabel);

            container.Add(icon);
            container.Add(textContainer);

            return container;
        }

        private void BindNPCInventoryItem(VisualElement element, int index)
        {
            if (index < 0 || index >= currentNPCItems.Count)
                return;

            ShopItemEntry entry = currentNPCItems[index];
            if (entry.item == null) return;

            VisualElement icon = element.Q<VisualElement>("item-icon");
            Label nameLabel = element.Q<Label>("item-name");
            Label priceLabel = element.Q<Label>("item-price");

            string itemName = "";
            int price = 0;
            Sprite iconSprite = null;

            if (entry.item is EquipmentData equipment)
            {
                itemName = equipment.ItemName;
                price = equipment.RentalCost;
                iconSprite = equipment.ItemIcon;
            }
            else if (entry.item is WeaponData weapon)
            {
                itemName = weapon.ItemName;
                price = weapon.RentalCost;
                iconSprite = weapon.ItemIcon;
            }

            nameLabel.text = itemName;
            priceLabel.text = $"{price} Souls";

            if (iconSprite != null)
            {
                icon.style.backgroundImage = new StyleBackground(iconSprite);
            }
            else
            {
                icon.style.backgroundImage = StyleKeyword.None;
            }
        }
        
        private VisualElement MakePlayerInventoryItem()
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingTop = 8;
            container.style.paddingBottom = 8;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.minHeight = 70;
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            container.style.marginBottom = 5;
            container.style.borderBottomLeftRadius = 5;
            container.style.borderBottomRightRadius = 5;
            container.style.borderTopLeftRadius = 5;
            container.style.borderTopRightRadius = 5;

            VisualElement icon = new VisualElement();
            icon.name = "item-icon";
            icon.style.width = 50;
            icon.style.height = 50;
            icon.style.marginRight = 10;
            icon.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            icon.style.borderTopLeftRadius = 5;
            icon.style.borderTopRightRadius = 5;
            icon.style.borderBottomLeftRadius = 5;
            icon.style.borderBottomRightRadius = 5;

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

            container.Add(icon);
            container.Add(nameLabel);
            container.Add(quantityLabel);

            return container;
        }

        private void BindPlayerInventoryItem(VisualElement element, int index)
        {
            if (index < 0 || index >= currentPlayerItems.Count)
                return;

            InventoryItem invItem = currentPlayerItems[index];
            VisualElement icon = element.Q<VisualElement>("item-icon");
            Label nameLabel = element.Q<Label>("item-name");
            Label quantityLabel = element.Q<Label>("item-quantity");

            string itemName = "";
            Sprite iconSprite = null;

            if (invItem.itemData is EquipmentData equipment)
            {
                itemName = equipment.ItemName;
                iconSprite = equipment.ItemIcon;
            }
            else if (invItem.itemData is WeaponData weapon)
            {
                itemName = weapon.ItemName;
                iconSprite = weapon.ItemIcon;
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
        }
        
        private void OnNPCItemSelected(IEnumerable<object> selectedItems)
        {
            if (playerInventoryList != null)
            {
                playerInventoryList.ClearSelection();
            }

            var selectedList = selectedItems.ToList();
            if (selectedList.Count > 0 && selectedList[0] is ShopItemEntry entry)
            {
                selectedItem = entry;
                isNPCItemSelected = true;
                ShowItemDetails(entry);
            }
        }
        
        private void OnPlayerItemSelected(IEnumerable<object> selectedItems)
        {
            if (npcInventoryList != null)
            {
                npcInventoryList.ClearSelection();
            }

            var selectedList = selectedItems.ToList();
            if (selectedList.Count > 0 && selectedList[0] is InventoryItem invItem)
            {
                selectedItem = invItem;
                isNPCItemSelected = false;
                ShowItemDetails(invItem);
            }
        }
        
        private void ClearSelection()
        {
            selectedItem = null;
            
            if (npcInventoryList != null)
            {
                npcInventoryList.ClearSelection();
            }
            
            if (playerInventoryList != null)
            {
                playerInventoryList.ClearSelection();
            }
            
            if (itemDetailPanel != null)
            {
                itemDetailPanel.style.display = DisplayStyle.None;
            }
            
            if (noItemSelectedPanel != null)
            {
                noItemSelectedPanel.style.display = DisplayStyle.Flex;
            }
        }
        
        private void ShowItemDetails(ShopItemEntry entry)
        {
            if (itemDetailPanel != null)
            {
                itemDetailPanel.style.display = DisplayStyle.Flex;
            }
            
            if (noItemSelectedPanel != null)
            {
                noItemSelectedPanel.style.display = DisplayStyle.None;
            }

            string itemName = "";
            string stats = "";
            int price = 0;
            Sprite iconSprite = null;

            if (entry.item is EquipmentData equipment)
            {
                itemName = equipment.ItemName;
                price = equipment.RentalCost;
                iconSprite = equipment.ItemIcon;
                stats = GetEquipmentStats(equipment);
            }
            else if (entry.item is WeaponData weapon)
            {
                itemName = weapon.ItemName;
                price = weapon.RentalCost;
                iconSprite = weapon.ItemIcon;
                stats = GetWeaponStats(weapon);
            }

            if (detailNameLabel != null)
            {
                detailNameLabel.text = itemName;
            }

            if (detailPriceLabel != null)
            {
                detailPriceLabel.text = $"Price: {price} Souls";
            }

            if (detailStatsLabel != null)
            {
                detailStatsLabel.text = stats;
            }

            if (detailIconElement != null && iconSprite != null)
            {
                detailIconElement.style.backgroundImage = new StyleBackground(iconSprite);
            }

            UpdateActionButton();
        }
        
        private void ShowItemDetails(InventoryItem invItem)
        {
            if (itemDetailPanel != null)
            {
                itemDetailPanel.style.display = DisplayStyle.Flex;
            }
            
            if (noItemSelectedPanel != null)
            {
                noItemSelectedPanel.style.display = DisplayStyle.None;
            }

            string itemName = "";
            string stats = "";
            int sellPrice = 0;
            Sprite iconSprite = null;

            if (invItem.itemData is EquipmentData equipment)
            {
                itemName = equipment.ItemName;
                sellPrice = Mathf.RoundToInt(equipment.RentalCost * 0.5f);
                iconSprite = equipment.ItemIcon;
                stats = GetEquipmentStats(equipment);
            }
            else if (invItem.itemData is WeaponData weapon)
            {
                itemName = weapon.ItemName;
                sellPrice = Mathf.RoundToInt(weapon.RentalCost * 0.5f);
                iconSprite = weapon.ItemIcon;
                stats = GetWeaponStats(weapon);
            }

            if (detailNameLabel != null)
            {
                detailNameLabel.text = itemName;
            }

            if (detailPriceLabel != null)
            {
                detailPriceLabel.text = $"Sell Value: {sellPrice} Souls";
            }

            if (detailStatsLabel != null)
            {
                detailStatsLabel.text = stats;
            }

            if (detailIconElement != null && iconSprite != null)
            {
                detailIconElement.style.backgroundImage = new StyleBackground(iconSprite);
            }

            UpdateActionButton();
        }

        private void UpdateActionButton()
        {
            if (actionButton == null)
                return;

            if (isNPCItemSelected && selectedItem is ShopItemEntry entry)
            {
                int price = 0;
                if (entry.item is EquipmentData equipment)
                {
                    price = equipment.RentalCost;
                }
                else if (entry.item is WeaponData weapon)
                {
                    price = weapon.RentalCost;
                }

                bool canAfford = gameManager != null && gameManager.Souls >= price;
                bool alreadyPurchased = entry.isPurchased;

                actionButton.text = "PURCHASE";
                actionButton.SetEnabled(canAfford && !alreadyPurchased);
                
                if (alreadyPurchased)
                {
                    actionButton.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
                }
                else if (canAfford)
                {
                    actionButton.style.backgroundColor = new Color(0.2f, 0.6f, 0.2f);
                }
                else
                {
                    actionButton.style.backgroundColor = new Color(0.6f, 0.2f, 0.2f);
                }
            }
            else if (!isNPCItemSelected && selectedItem is InventoryItem invItem)
            {
                bool isEquipped = IsItemEquipped(invItem.itemData);
                
                actionButton.text = "SELL";
                actionButton.SetEnabled(!isEquipped);
                
                if (isEquipped)
                {
                    actionButton.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    actionButton.style.backgroundColor = new Color(0.8f, 0.6f, 0.0f);
                }
            }
        }
        
        private void OnActionButtonClicked()
        {
            if (isNPCItemSelected && selectedItem is ShopItemEntry entry)
            {
                PurchaseItem(entry);
            }
            else if (!isNPCItemSelected && selectedItem is InventoryItem invItem)
            {
                SellItem(invItem);
            }
        }
        
        private void PurchaseItem(ShopItemEntry entry)
        {
            if (entry.isPurchased)
            {
                Debug.Log("Item already purchased!");
                return;
            }

            int price = 0;
            if (entry.item is EquipmentData equipment)
            {
                price = equipment.RentalCost;
            }
            else if (entry.item is WeaponData weapon)
            {
                price = weapon.RentalCost;
            }

            if (gameManager == null || gameManager.Souls < price)
            {
                Debug.Log("Not enough souls!");
                return;
            }

            gameManager.AddSouls(-price);
            entry.isPurchased = true;
            entry.currentStock--;

            if (playerInventory != null)
            {
                playerInventory.AddItem(entry.item);
            }

            RefreshNPCInventoryList();
            RefreshUI();
            UpdateActionButton();
            
            Debug.Log($"Purchased {entry.item.name} for {price} souls!");
        }
        
        private void SellItem(InventoryItem invItem)
        {
            if (IsItemEquipped(invItem.itemData))
            {
                Debug.Log("Cannot sell equipped items!");
                return;
            }

            int sellPrice = 0;
            System.Guid itemId = System.Guid.Empty;
            
            if (invItem.itemData is EquipmentData equipment)
            {
                sellPrice = Mathf.RoundToInt(equipment.RentalCost * 0.5f);
                itemId = equipment.ItemId;
            }
            else if (invItem.itemData is WeaponData weapon)
            {
                sellPrice = Mathf.RoundToInt(weapon.RentalCost * 0.5f);
                itemId = weapon.ItemId;
            }

            if (gameManager != null)
            {
                gameManager.AddSouls(sellPrice);
            }

            if (playerInventory != null)
            {
                playerInventory.RemoveItem(itemId, 1);
            }

            ClearSelection();
            RefreshPlayerInventoryList();
            RefreshUI();
            
            Debug.Log($"Sold {invItem.itemData.name} for {sellPrice} souls!");
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
        
        private string GetEquipmentStats(EquipmentData equipment)
        {
            string stats = "";
            
            if (!string.IsNullOrEmpty(equipment.FlavorText))
            {
                stats += $"{equipment.FlavorText}\n\n";
            }
            
            stats += $"Type: {equipment.Category}\n";
            stats += $"Rarity: {equipment.Rarity}\n\n";
            
            if (equipment.StaticModifier != null)
            {
                stats += "STATIC MODIFIER:\n";
                stats += $"• {GetModifierDisplayText(equipment.StaticModifier)}\n\n";
            }
            
            if (equipment.Modifiers != null && equipment.Modifiers.Count > 0)
            {
                stats += "MODIFIERS:\n";
                foreach (var modifier in equipment.Modifiers)
                {
                    if (modifier != null)
                    {
                        stats += $"• {GetModifierDisplayText(modifier)}\n";
                    }
                }
                stats += "\n";
            }
            
            if (equipment.Spell != null)
            {
                stats += $"SPELL: {equipment.Spell.name}\n";
            }

            return stats;
        }
        
        private string GetWeaponStats(WeaponData weapon)
        {
            string stats = "";
            
            if (!string.IsNullOrEmpty(weapon.FlavorText))
            {
                stats += $"{weapon.FlavorText}\n\n";
            }
            
            stats += $"Rarity: {weapon.Rarity}\n";
            stats += $"Damage Multiplier: x{weapon.DamageMultiplier}\n\n";
            
            if (weapon.PrimaryCantrip != null)
            {
                stats += $"PRIMARY: {weapon.PrimaryCantrip.name}\n";
            }
            
            if (weapon.SecondaryCantrip != null)
            {
                stats += $"SECONDARY: {weapon.SecondaryCantrip.name}\n";
            }

            return stats;
        }
        
        private VisualElement MakeAlterationsInventoryItem()
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.paddingTop = 5;
            container.style.paddingBottom = 5;
            container.style.marginBottom = 5;
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            container.style.borderTopLeftRadius = 5;
            container.style.borderTopRightRadius = 5;
            container.style.borderBottomLeftRadius = 5;
            container.style.borderBottomRightRadius = 5;

            VisualElement icon = new VisualElement();
            icon.name = "item-icon";
            icon.style.width = 50;
            icon.style.height = 50;
            icon.style.marginRight = 10;
            icon.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            icon.style.borderTopLeftRadius = 5;
            icon.style.borderTopRightRadius = 5;
            icon.style.borderBottomLeftRadius = 5;
            icon.style.borderBottomRightRadius = 5;
            container.Add(icon);

            VisualElement textContainer = new VisualElement();
            textContainer.style.flexGrow = 1;

            Label nameLabel = new Label();
            nameLabel.name = "item-name";
            nameLabel.style.fontSize = 16;
            nameLabel.style.color = Color.white;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            textContainer.Add(nameLabel);

            Label modifierCountLabel = new Label();
            modifierCountLabel.name = "modifier-count";
            modifierCountLabel.style.fontSize = 12;
            modifierCountLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            textContainer.Add(modifierCountLabel);

            container.Add(textContainer);

            return container;
        }

        private void BindAlterationsInventoryItem(VisualElement element, int index)
        {
            if (index < 0 || index >= currentAlterationsItems.Count)
                return;

            InventoryItem invItem = currentAlterationsItems[index];
            Label nameLabel = element.Q<Label>("item-name");
            Label modifierCountLabel = element.Q<Label>("modifier-count");
            VisualElement iconElement = element.Q<VisualElement>("item-icon");

            if (invItem.itemData is EquipmentData equipment)
            {
                if (nameLabel != null)
                {
                    nameLabel.text = equipment.ItemName;
                }

                if (modifierCountLabel != null)
                {
                    int modCount = equipment.Modifiers != null ? equipment.Modifiers.Count : 0;
                    modifierCountLabel.text = $"{modCount} modifier(s) • {equipment.Category}";
                }

                if (iconElement != null && equipment.ItemIcon != null)
                {
                    iconElement.style.backgroundImage = new StyleBackground(equipment.ItemIcon);
                }
            }
            else if (invItem.itemData is WeaponData weapon)
            {
                if (nameLabel != null)
                {
                    nameLabel.text = weapon.ItemName;
                }

                if (modifierCountLabel != null)
                {
                    modifierCountLabel.text = "Weapon";
                }

                if (iconElement != null && weapon.ItemIcon != null)
                {
                    iconElement.style.backgroundImage = new StyleBackground(weapon.ItemIcon);
                }
            }
        }

        private void RefreshAlterationsInventoryList()
        {
            currentAlterationsItems.Clear();

            if (playerInventory != null)
            {
                currentAlterationsItems.AddRange(playerInventory.Items.Where(item => 
                    item.itemData is EquipmentData || item.itemData is WeaponData));
            }

            if (alterationsInventoryList != null)
            {
                alterationsInventoryList.itemsSource = currentAlterationsItems;
                alterationsInventoryList.Rebuild();
            }
        }

        private void OnAlterationsItemSelected(IEnumerable<object> selectedItems)
        {
            object selected = selectedItems.FirstOrDefault();
            
            if (selected == null)
            {
                ClearAlterationsSelection();
                return;
            }

            if (selected is InventoryItem invItem)
            {
                selectedAlterationItem = invItem;
                selectedModifierIndices.Clear();
                ShowAlterationDetails(invItem);
            }
        }

        private void ClearAlterationsSelection()
        {
            selectedAlterationItem = null;
            selectedModifierIndices.Clear();

            if (alterationsDetailPanel != null)
            {
                alterationsDetailPanel.style.display = DisplayStyle.None;
            }

            if (alterationsNoSelectionPanel != null)
            {
                alterationsNoSelectionPanel.style.display = DisplayStyle.Flex;
            }

            if (alterationsInventoryList != null)
            {
                alterationsInventoryList.ClearSelection();
            }
        }

        private void ShowAlterationDetails(InventoryItem invItem)
        {
            if (alterationsDetailPanel != null)
            {
                alterationsDetailPanel.style.display = DisplayStyle.Flex;
            }

            if (alterationsNoSelectionPanel != null)
            {
                alterationsNoSelectionPanel.style.display = DisplayStyle.None;
            }

            string itemName = "";
            Sprite iconSprite = null;
            List<StatModifierBase> modifiers = new List<StatModifierBase>();

            if (invItem.itemData is EquipmentData equipment)
            {
                itemName = equipment.ItemName;
                iconSprite = equipment.ItemIcon;
                if (equipment.Modifiers != null)
                {
                    modifiers.AddRange(equipment.Modifiers);
                }
            }
            else if (invItem.itemData is WeaponData weapon)
            {
                itemName = weapon.ItemName;
                iconSprite = weapon.ItemIcon;
            }

            if (alterationsDetailName != null)
            {
                alterationsDetailName.text = itemName;
            }

            if (alterationsDetailIcon != null && iconSprite != null)
            {
                alterationsDetailIcon.style.backgroundImage = new StyleBackground(iconSprite);
            }

            RebuildModifierToggles(modifiers);
            UpdateRerollCost();
        }

        private void RebuildModifierToggles(List<StatModifierBase> modifiers)
        {
            if (modifiersContainer == null)
                return;

            modifiersContainer.Clear();

            if (modifiers.Count == 0)
            {
                Label noModsLabel = new Label("No modifiers available to reroll");
                noModsLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                noModsLabel.style.fontSize = 14;
                noModsLabel.style.marginTop = 10;
                modifiersContainer.Add(noModsLabel);
                
                if (rerollButton != null)
                {
                    rerollButton.SetEnabled(false);
                }
                return;
            }

            for (int i = 0; i < modifiers.Count; i++)
            {
                int index = i;
                StatModifierBase modifier = modifiers[i];
                
                VisualElement modRow = new VisualElement();
                modRow.style.flexDirection = FlexDirection.Row;
                modRow.style.alignItems = Align.Center;
                modRow.style.marginBottom = 8;
                modRow.style.paddingLeft = 10;
                modRow.style.paddingRight = 10;
                modRow.style.paddingTop = 8;
                modRow.style.paddingBottom = 8;
                modRow.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
                modRow.style.borderTopLeftRadius = 4;
                modRow.style.borderTopRightRadius = 4;
                modRow.style.borderBottomLeftRadius = 4;
                modRow.style.borderBottomRightRadius = 4;

                Toggle toggle = new Toggle();
                toggle.style.marginRight = 10;
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue)
                    {
                        if (!selectedModifierIndices.Contains(index))
                        {
                            selectedModifierIndices.Add(index);
                        }
                    }
                    else
                    {
                        selectedModifierIndices.Remove(index);
                    }
                    UpdateRerollCost();
                });

                modRow.Add(toggle);

                Label modLabel = new Label(GetModifierDisplayText(modifier));
                modLabel.style.flexGrow = 1;
                modLabel.style.color = Color.white;
                modLabel.style.fontSize = 14;
                modRow.Add(modLabel);

                modifiersContainer.Add(modRow);
            }

            if (rerollButton != null)
            {
                rerollButton.SetEnabled(true);
            }
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

        private void UpdateRerollCost()
        {
            if (rerollCostLabel == null || alterationsSystem == null)
                return;

            int cost = alterationsSystem.CalculateRerollCost(selectedModifierIndices.Count);
            rerollCostLabel.text = $"Cost: {cost} Souls";

            if (rerollButton != null)
            {
                rerollButton.SetEnabled(selectedModifierIndices.Count > 0);
            }
        }

        private void OnRerollButtonClicked()
        {
            if (selectedAlterationItem == null || selectedModifierIndices.Count == 0)
            {
                Debug.LogWarning("NPCStationUIController: No item or modifiers selected for reroll");
                return;
            }

            if (alterationsSystem == null)
            {
                Debug.LogError("NPCStationUIController: AlterationsSystem is not initialized");
                return;
            }

            if (gameManager == null)
            {
                Debug.LogError("NPCStationUIController: GameManager not found");
                return;
            }

            int cost = alterationsSystem.CalculateRerollCost(selectedModifierIndices.Count);

            if (gameManager.Souls < cost)
            {
                Debug.LogWarning($"NPCStationUIController: Not enough souls. Need {cost}, have {gameManager.Souls}");
                return;
            }

            if (!(selectedAlterationItem.itemData is EquipmentData equipment))
            {
                Debug.LogWarning("NPCStationUIController: Selected item is not equipment");
                return;
            }

            gameManager.ModifySouls(-cost);

            foreach (int index in selectedModifierIndices.OrderByDescending(x => x))
            {
                if (index >= 0 && index < equipment.Modifiers.Count)
                {
                    StatModifierBase currentModifier = equipment.Modifiers[index];
                    StatModifierBase newModifier = alterationsSystem.RerollModifier(currentModifier);
                    
                    if (newModifier != null)
                    {
                        equipment.ReplaceModifier(index, newModifier);
                        Debug.Log($"NPCStationUIController: Rerolled modifier at index {index}");
                    }
                }
            }

            selectedModifierIndices.Clear();
            ShowAlterationDetails(selectedAlterationItem);
            RefreshUI();
        }
    }
}
