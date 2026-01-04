using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using RealmCrawler.Core;
using RealmCrawler.Equipment;
using RealmCrawler.Spells;
using RealmCrawler.Player;

namespace RealmCrawler.UI
{
    public enum LoadoutCategory
    {
        Hat,
        Cloak,
        Boots,
        Reliquary,
        Weapon
    }

    public class LoadoutUIController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private LoadoutCameraManager cameraManager;

        private VisualElement root;
        private Label loadoutCostLabel;
        private Label currentSoulsLabel;
        private ListView equipmentList;
        private Label healthStatLabel;
        private Label manaStatLabel;
        private Label speedStatLabel;
        private Label xpRadiusStatLabel;
        private VisualElement primaryCantripIcon;
        private VisualElement secondaryCantripIcon;
        private VisualElement primaryCantripSlot;
        private VisualElement secondaryCantripSlot;
        private Button backButton;
        private Button playButton;
        private Button resetCameraButton;
        private VisualElement detailPopup;
        private VisualElement equipmentIconLarge;
        private Label equipmentNameLabel;
        private Label equipmentStatsLabel;
        private Label equipmentBuffLabel;
        private Label equipmentCostLabel;
        private Button purchaseButton;
        private Button popupBackButton;

        private VisualElement cantripTooltip;
        private Label cantripTooltipName;
        private Label cantripTooltipDesc;
        private Label cantripTooltipStats;

        private Button hatTabButton;
        private Button cloakTabButton;
        private Button bootsTabButton;
        private Button reliquaryTabButton;
        private Button weaponTabButton;

        private GameManager gameManager;
        private PlayerVisualEquipment playerVisualEquipment;
        private LoadoutCategory currentCategory = LoadoutCategory.Hat;
        private List<ScriptableObject> currentItems = new List<ScriptableObject>();
        private ScriptableObject selectedEquipment;
        private bool isPopupOpen = false;
        private ScriptableObject lastClosedItem = null;

        private void OnEnable()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            playerVisualEquipment = FindFirstObjectByType<PlayerVisualEquipment>();
            
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found! Please add GameManager to the scene.");
                return;
            }

            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }

            if (uiDocument == null)
            {
                Debug.LogError("UIDocument component not found!");
                return;
            }

            if (uiDocument.visualTreeAsset == null)
            {
                Debug.LogError("Visual Tree Asset not assigned to UIDocument!");
                return;
            }

            root = uiDocument.rootVisualElement;
            
            if (root == null)
            {
                Debug.LogError("Root visual element is null!");
                return;
            }

            Debug.Log("Querying UI elements...");
            QueryUIElements();
            
            if (equipmentList == null)
            {
                Debug.LogError("Equipment list not found in UI!");
                return;
            }

            Debug.Log("Setting up event handlers...");
            SetupEventHandlers();
            
            detailPopup.style.display = DisplayStyle.None;
            detailPopup.pickingMode = PickingMode.Ignore;
            cantripTooltip.style.display = DisplayStyle.None;
            
            Debug.Log("Refreshing UI...");
            RefreshUI();
            PopulateEquipmentList();
        }

        private void QueryUIElements()
        {
            Debug.Log($"Root element children count: {root.childCount}");
            
            loadoutCostLabel = root.Q<Label>("loadout-cost");
            currentSoulsLabel = root.Q<Label>("current-souls");
            equipmentList = root.Q<ListView>("equipment-list");
            healthStatLabel = root.Q<Label>("health-stat");
            manaStatLabel = root.Q<Label>("mana-stat");
            speedStatLabel = root.Q<Label>("speed-stat");
            xpRadiusStatLabel = root.Q<Label>("xp-radius-stat");
            primaryCantripIcon = root.Q<VisualElement>("primary-icon");
            secondaryCantripIcon = root.Q<VisualElement>("secondary-icon");
            primaryCantripSlot = root.Q<VisualElement>("primary-cantrip");
            secondaryCantripSlot = root.Q<VisualElement>("secondary-cantrip");
            backButton = root.Q<Button>("back-button");
            playButton = root.Q<Button>("play-button");
            resetCameraButton = root.Q<Button>("reset-camera-button");
            detailPopup = root.Q<VisualElement>("detail-popup");
            equipmentIconLarge = root.Q<VisualElement>("equipment-icon-large");
            equipmentNameLabel = root.Q<Label>("equipment-name");
            equipmentStatsLabel = root.Q<Label>("equipment-stats");
            equipmentBuffLabel = root.Q<Label>("equipment-buff");
            equipmentCostLabel = root.Q<Label>("equipment-cost");
            purchaseButton = root.Q<Button>("purchase-button");
            popupBackButton = root.Q<Button>("popup-back-button");

            cantripTooltip = root.Q<VisualElement>("cantrip-tooltip");
            cantripTooltipName = cantripTooltip?.Q<Label>("cantrip-tooltip-name");
            cantripTooltipDesc = cantripTooltip?.Q<Label>("cantrip-tooltip-desc");
            cantripTooltipStats = cantripTooltip?.Q<Label>("cantrip-tooltip-stats");

            hatTabButton = root.Q<Button>("hat-tab");
            cloakTabButton = root.Q<Button>("cloak-tab");
            bootsTabButton = root.Q<Button>("boots-tab");
            reliquaryTabButton = root.Q<Button>("reliquary-tab");
            weaponTabButton = root.Q<Button>("weapon-tab");
            
            Debug.Log($"Successfully queried all UI elements. Equipment list: {equipmentList != null}");
        }

        private void SetupEventHandlers()
        {
            hatTabButton.clicked += () => { SwitchCategory(LoadoutCategory.Hat); MoveCameraToView(LoadoutCategory.Hat); };
            cloakTabButton.clicked += () => { SwitchCategory(LoadoutCategory.Cloak); MoveCameraToView(LoadoutCategory.Cloak); };
            bootsTabButton.clicked += () => { SwitchCategory(LoadoutCategory.Boots); MoveCameraToView(LoadoutCategory.Boots); };
            reliquaryTabButton.clicked += () => { SwitchCategory(LoadoutCategory.Reliquary); MoveCameraToView(LoadoutCategory.Reliquary); };
            weaponTabButton.clicked += () => { SwitchCategory(LoadoutCategory.Weapon); MoveCameraToView(LoadoutCategory.Weapon); };

            backButton.clicked += OnBackButtonClicked;
            playButton.clicked += OnPlayButtonClicked;
            resetCameraButton.clicked += OnResetCameraButtonClicked;
            
            purchaseButton.clicked += OnPurchaseButtonClicked;
            popupBackButton.clicked += CloseDetailPopup;

            equipmentList.makeItem = MakeEquipmentListItem;
            equipmentList.bindItem = BindEquipmentListItem;
            equipmentList.selectionChanged += OnEquipmentSelected;
            equipmentList.itemsChosen += OnEquipmentChosen;
            equipmentList.selectionType = SelectionType.Single;
            equipmentList.fixedItemHeight = 80;
            equipmentList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            SetupCantripTooltips();
        }

        private void MoveCameraToView(LoadoutCategory category)
        {
            if (cameraManager == null) return;

            switch (category)
            {
                case LoadoutCategory.Hat:
                    cameraManager.MoveToHatView();
                    break;
                case LoadoutCategory.Cloak:
                    cameraManager.MoveToCloakView();
                    break;
                case LoadoutCategory.Boots:
                    cameraManager.MoveToBootView();
                    break;
                case LoadoutCategory.Reliquary:
                    cameraManager.MoveToRelicView();
                    break;
                case LoadoutCategory.Weapon:
                    cameraManager.MoveToWeaponView();
                    break;
            }
        }

        private void OnEquipmentChosen(IEnumerable<object> chosenItems)
        {
            lastClosedItem = null;
        }

        private void OnEquipmentSelected(IEnumerable<object> selectedItems)
        {
            if (isPopupOpen)
                return;

            foreach (object item in selectedItems)
            {
                selectedEquipment = item as ScriptableObject;
                if (selectedEquipment != null)
                {
                    if (selectedEquipment == lastClosedItem)
                    {
                        equipmentList.ClearSelection();
                        return;
                    }
                    
                    ShowDetailPopup(selectedEquipment);
                }
                break;
            }
        }

        private void SetupCantripTooltips()
        {
            primaryCantripSlot.RegisterCallback<MouseEnterEvent>(evt => ShowCantripTooltip(primaryCantripIcon, true));
            primaryCantripSlot.RegisterCallback<MouseLeaveEvent>(evt => HideCantripTooltip());
            
            secondaryCantripSlot.RegisterCallback<MouseEnterEvent>(evt => ShowCantripTooltip(secondaryCantripIcon, false));
            secondaryCantripSlot.RegisterCallback<MouseLeaveEvent>(evt => HideCantripTooltip());
        }

        private void SwitchCategory(LoadoutCategory category)
        {
            currentCategory = category;
            lastClosedItem = null;
            PopulateEquipmentList();
            UpdateTabButtonStates();
        }

        private void UpdateTabButtonStates()
        {
            hatTabButton.RemoveFromClassList("selected-tab");
            cloakTabButton.RemoveFromClassList("selected-tab");
            bootsTabButton.RemoveFromClassList("selected-tab");
            reliquaryTabButton.RemoveFromClassList("selected-tab");
            weaponTabButton.RemoveFromClassList("selected-tab");

            switch (currentCategory)
            {
                case LoadoutCategory.Hat:
                    hatTabButton.AddToClassList("selected-tab");
                    break;
                case LoadoutCategory.Cloak:
                    cloakTabButton.AddToClassList("selected-tab");
                    break;
                case LoadoutCategory.Boots:
                    bootsTabButton.AddToClassList("selected-tab");
                    break;
                case LoadoutCategory.Reliquary:
                    reliquaryTabButton.AddToClassList("selected-tab");
                    break;
                case LoadoutCategory.Weapon:
                    weaponTabButton.AddToClassList("selected-tab");
                    break;
            }
        }

        private void PopulateEquipmentList()
        {
            if (gameManager?.EquipmentDB == null)
            {
                Debug.LogError("EquipmentDatabase is null!");
                return;
            }

            currentItems.Clear();

            switch (currentCategory)
            {
                case LoadoutCategory.Hat:
                    currentItems.AddRange(gameManager.EquipmentDB.GetAllHats());
                    break;
                case LoadoutCategory.Cloak:
                    currentItems.AddRange(gameManager.EquipmentDB.GetAllCloaks());
                    break;
                case LoadoutCategory.Boots:
                    currentItems.AddRange(gameManager.EquipmentDB.GetAllBoots());
                    break;
                case LoadoutCategory.Reliquary:
                    currentItems.AddRange(gameManager.EquipmentDB.GetAllReliquaries());
                    break;
                case LoadoutCategory.Weapon:
                    currentItems.AddRange(gameManager.EquipmentDB.GetAllWeapons());
                    break;
            }

            equipmentList.itemsSource = currentItems;
            equipmentList.Rebuild();

            Debug.Log($"Populated {currentCategory} list with {currentItems.Count} items");
        }

        private VisualElement MakeEquipmentListItem()
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("equipment-item");
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingTop = 8;
            container.style.paddingBottom = 8;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.minHeight = 70;
            container.style.height = 70;

            VisualElement icon = new VisualElement();
            icon.name = "item-icon";
            icon.style.width = 50;
            icon.style.height = 50;
            icon.style.minWidth = 50;
            icon.style.minHeight = 50;
            icon.style.marginRight = 10;
            icon.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            icon.style.borderTopLeftRadius = 5;
            icon.style.borderTopRightRadius = 5;
            icon.style.borderBottomLeftRadius = 5;
            icon.style.borderBottomRightRadius = 5;
            icon.style.flexShrink = 0;

            Label nameLabel = new Label();
            nameLabel.name = "item-name";
            nameLabel.style.fontSize = 18;
            nameLabel.style.color = Color.white;
            nameLabel.style.flexGrow = 1;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            nameLabel.style.overflow = Overflow.Hidden;
            nameLabel.style.textOverflow = TextOverflow.Ellipsis;
            nameLabel.style.whiteSpace = WhiteSpace.NoWrap;

            Label costLabel = new Label();
            costLabel.name = "item-cost";
            costLabel.style.fontSize = 16;
            costLabel.style.color = new Color(1f, 0.84f, 0f);
            costLabel.style.width = 100;
            costLabel.style.minWidth = 100;
            costLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            costLabel.style.flexShrink = 0;

            container.Add(icon);
            container.Add(nameLabel);
            container.Add(costLabel);

            return container;
        }

        private void BindEquipmentListItem(VisualElement element, int index)
        {
            if (index < 0 || index >= currentItems.Count)
                return;

            ScriptableObject item = currentItems[index];
            VisualElement icon = element.Q<VisualElement>("item-icon");
            Label nameLabel = element.Q<Label>("item-name");
            Label costLabel = element.Q<Label>("item-cost");

            string itemName = "Unknown";
            int cost = 0;
            Sprite iconSprite = null;
            bool isEquipped = false;

            if (item is EquipmentData equipment)
            {
                itemName = equipment.EquipmentName;
                cost = equipment.RentalCost;
                iconSprite = equipment.Icon;
                
                if (item is HatData hat)
                    isEquipped = gameManager.CurrentLoadout.hat == hat;
                else if (item is CloakData cloak)
                    isEquipped = gameManager.CurrentLoadout.cloak == cloak;
                else if (item is BootsData boots)
                    isEquipped = gameManager.CurrentLoadout.boots == boots;
                else if (item is ReliquaryData reliquary)
                    isEquipped = gameManager.CurrentLoadout.reliquary == reliquary;
            }
            else if (item is WeaponData weapon)
            {
                itemName = weapon.WeaponName;
                cost = weapon.RentalCost;
                iconSprite = weapon.Icon;
                isEquipped = gameManager.CurrentLoadout.weapon == weapon;
            }

            nameLabel.text = itemName;
            
            if (isEquipped)
            {
                costLabel.text = "EQUIPPED";
                costLabel.style.color = new Color(0.5f, 1f, 0.5f);
            }
            else
            {
                costLabel.text = cost == 0 ? "FREE" : $"{cost} souls";
                costLabel.style.color = new Color(1f, 0.84f, 0f);
            }

            if (iconSprite != null)
            {
                icon.style.backgroundImage = new StyleBackground(iconSprite);
            }
            else
            {
                icon.style.backgroundImage = StyleKeyword.None;
            }
        }

        private void ShowDetailPopup(ScriptableObject equipment)
        {
            if (equipment == null)
                return;

            lastClosedItem = null;

            string itemName = "Unknown";
            int cost = 0;
            Sprite iconSprite = null;
            string statsText = "";
            bool isEquipped = false;

            if (equipment is HatData hat)
            {
                itemName = hat.EquipmentName;
                cost = hat.RentalCost;
                iconSprite = hat.Icon;
                statsText = $"Mana Bonus: +{hat.ManaBonus}";
                isEquipped = gameManager.CurrentLoadout.hat == hat;
            }
            else if (equipment is CloakData cloak)
            {
                itemName = cloak.EquipmentName;
                cost = cloak.RentalCost;
                iconSprite = cloak.Icon;
                statsText = $"Health Bonus: +{cloak.HealthBonus}";
                isEquipped = gameManager.CurrentLoadout.cloak == cloak;
            }
            else if (equipment is BootsData boots)
            {
                itemName = boots.EquipmentName;
                cost = boots.RentalCost;
                iconSprite = boots.Icon;
                statsText = $"Speed Multiplier: x{boots.SpeedMultiplier:F2}";
                isEquipped = gameManager.CurrentLoadout.boots == boots;
            }
            else if (equipment is ReliquaryData reliquary)
            {
                itemName = reliquary.EquipmentName;
                cost = reliquary.RentalCost;
                iconSprite = reliquary.Icon;
                statsText = $"XP Radius Bonus: +{reliquary.XpCollectRadiusBonus}";
                isEquipped = gameManager.CurrentLoadout.reliquary == reliquary;
            }
            else if (equipment is WeaponData weapon)
            {
                itemName = weapon.WeaponName;
                cost = weapon.RentalCost;
                iconSprite = weapon.Icon;
                statsText = $"Primary: {weapon.PrimaryCantrip?.CantripName ?? "None"}\nSecondary: {weapon.SecondaryCantrip?.CantripName ?? "None"}";
                isEquipped = gameManager.CurrentLoadout.weapon == weapon;
            }

            equipmentNameLabel.text = itemName;
            equipmentStatsLabel.text = statsText;
            equipmentBuffLabel.text = equipment is EquipmentData equipData ? equipData.Description : (equipment as WeaponData)?.Description ?? "";
            equipmentCostLabel.text = cost == 0 ? "FREE" : $"Cost: {cost} souls";

            if (iconSprite != null)
            {
                equipmentIconLarge.style.backgroundImage = new StyleBackground(iconSprite);
            }
            else
            {
                equipmentIconLarge.style.backgroundImage = StyleKeyword.None;
            }

            UpdatePurchaseButton(cost, isEquipped);

            detailPopup.style.display = DisplayStyle.Flex;
            detailPopup.pickingMode = PickingMode.Position;
            isPopupOpen = true;
        }

        private void UpdatePurchaseButton(int cost, bool isEquipped)
        {
            bool canAfford = gameManager.Souls >= cost;

            if (isEquipped)
            {
                purchaseButton.text = "EQUIPPED";
                purchaseButton.SetEnabled(false);
            }
            else if (canAfford)
            {
                purchaseButton.text = "PURCHASE & EQUIP";
                purchaseButton.SetEnabled(true);
            }
            else
            {
                purchaseButton.text = "INSUFFICIENT SOULS";
                purchaseButton.SetEnabled(false);
            }
        }

        private void CloseDetailPopup()
        {
            lastClosedItem = selectedEquipment;
            
            detailPopup.style.display = DisplayStyle.None;
            detailPopup.pickingMode = PickingMode.Ignore;
            equipmentList.ClearSelection();
            selectedEquipment = null;
            isPopupOpen = false;
        }

        private void OnPurchaseButtonClicked()
        {
            if (selectedEquipment == null)
                return;

            int cost = 0;
            string itemName = "";

            if (selectedEquipment is HatData hat)
            {
                cost = hat.RentalCost;
                itemName = hat.EquipmentName;
                
                if (gameManager.Souls >= cost)
                {
                    gameManager.AddSouls(-cost);
                    gameManager.CurrentLoadout.hat = hat;
                    Debug.Log($"Purchased and equipped hat: {itemName} for {cost} souls");
                }
            }
            else if (selectedEquipment is CloakData cloak)
            {
                cost = cloak.RentalCost;
                itemName = cloak.EquipmentName;
                
                if (gameManager.Souls >= cost)
                {
                    gameManager.AddSouls(-cost);
                    gameManager.CurrentLoadout.cloak = cloak;
                    Debug.Log($"Purchased and equipped cloak: {itemName} for {cost} souls");
                }
            }
            else if (selectedEquipment is BootsData boots)
            {
                cost = boots.RentalCost;
                itemName = boots.EquipmentName;
                
                if (gameManager.Souls >= cost)
                {
                    gameManager.AddSouls(-cost);
                    gameManager.CurrentLoadout.boots = boots;
                    Debug.Log($"Purchased and equipped boots: {itemName} for {cost} souls");
                }
            }
            else if (selectedEquipment is ReliquaryData reliquary)
            {
                cost = reliquary.RentalCost;
                itemName = reliquary.EquipmentName;
                
                if (gameManager.Souls >= cost)
                {
                    gameManager.AddSouls(-cost);
                    gameManager.CurrentLoadout.reliquary = reliquary;
                    Debug.Log($"Purchased and equipped reliquary: {itemName} for {cost} souls");
                }
            }
            else if (selectedEquipment is WeaponData weapon)
            {
                cost = weapon.RentalCost;
                itemName = weapon.WeaponName;
                
                if (gameManager.Souls >= cost)
                {
                    gameManager.AddSouls(-cost);
                    gameManager.CurrentLoadout.weapon = weapon;
                    Debug.Log($"Purchased and equipped weapon: {itemName} for {cost} souls");
                }
            }

            PopulateEquipmentList();
            CloseDetailPopup();
            RefreshUI();
            UpdatePlayerVisuals();
        }

        private void UpdatePlayerVisuals()
        {
            if (playerVisualEquipment == null)
            {
                Debug.LogWarning("PlayerVisualEquipment not found in scene. Skipping visual update.");
                return;
            }

            playerVisualEquipment.ApplyLoadout(gameManager.CurrentLoadout);
        }

        private void RefreshUI()
        {
            if (gameManager == null)
                return;

            int totalCost = gameManager.CurrentLoadout.CalculateTotalCost();
            loadoutCostLabel.text = $"Loadout Cost: {totalCost} souls";
            currentSoulsLabel.text = $"Current Souls: {gameManager.Souls}";

            LoadoutStats stats = gameManager.CurrentLoadout.CalculateStats();

            float displayHealth = stats.totalHealth > 0 ? stats.totalHealth : stats.baseHealth;
            float displayMana = stats.totalMana > 0 ? stats.totalMana : stats.baseMana;
            float displaySpeed = stats.totalSpeed > 0 ? stats.totalSpeed : stats.baseSpeed;
            float displayXpRadius = stats.totalXpRadius > 0 ? stats.totalXpRadius : stats.baseXpRadius;

            healthStatLabel.text = $"Health: {displayHealth:F0}";
            manaStatLabel.text = $"Mana: {displayMana:F0}";
            speedStatLabel.text = $"Speed: {displaySpeed:F1}";
            xpRadiusStatLabel.text = $"XP Radius: {displayXpRadius:F1}";

            UpdateCantripIcons();
        }

        private void UpdateCantripIcons()
        {
            if (gameManager.CurrentLoadout.weapon != null)
            {
                if (gameManager.CurrentLoadout.weapon.PrimaryCantrip?.Icon != null)
                {
                    primaryCantripIcon.style.backgroundImage = new StyleBackground(gameManager.CurrentLoadout.weapon.PrimaryCantrip.Icon);
                }
                else
                {
                    primaryCantripIcon.style.backgroundImage = StyleKeyword.None;
                }

                if (gameManager.CurrentLoadout.weapon.SecondaryCantrip?.Icon != null)
                {
                    secondaryCantripIcon.style.backgroundImage = new StyleBackground(gameManager.CurrentLoadout.weapon.SecondaryCantrip.Icon);
                }
                else
                {
                    secondaryCantripIcon.style.backgroundImage = StyleKeyword.None;
                }
            }
        }

        private void ShowCantripTooltip(VisualElement cantripIcon, bool isPrimary)
        {
            if (gameManager?.CurrentLoadout?.weapon == null)
                return;

            CantripData cantrip = isPrimary ? 
                gameManager.CurrentLoadout.weapon.PrimaryCantrip : 
                gameManager.CurrentLoadout.weapon.SecondaryCantrip;

            if (cantrip == null)
                return;

            cantripTooltipName.text = cantrip.CantripName;
            cantripTooltipDesc.text = cantrip.Description ?? "No description available";
            
            string statsText = $"Damage: {cantrip.BaseDamage}\n";
            statsText += $"Attack Speed: {cantrip.AttackSpeed}/s\n";
            statsText += $"Projectile Speed: {cantrip.ProjectileSpeed}";
            cantripTooltipStats.text = statsText;
            
            cantripTooltip.style.display = DisplayStyle.Flex;
        }

        private void HideCantripTooltip()
        {
            cantripTooltip.style.display = DisplayStyle.None;
        }

        private void OnBackButtonClicked()
        {
            Debug.Log("Back button clicked - returning to main menu");
        }

        private void OnResetCameraButtonClicked()
        {
            if (cameraManager != null)
            {
                cameraManager.MoveToDefaultView();
            }
        }

        private void OnPlayButtonClicked()
        {
            if (!gameManager.CanAffordLoadout())
            {
                Debug.LogWarning("Cannot afford this loadout!");
                return;
            }

            Debug.Log("Starting game with selected loadout!");
            SceneManager.LoadScene("Scene_GameplayTest");
        }
    }
}
