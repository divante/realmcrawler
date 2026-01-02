using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RealmCrawler.Core;
using RealmCrawler.Equipment;

namespace RealmCrawler.UI
{
    public class EquipmentGridManager : MonoBehaviour
    {
        [Header("Tab Buttons")]
        [SerializeField] private Button hatTabButton;
        [SerializeField] private Button cloakTabButton;
        [SerializeField] private Button bootsTabButton;
        [SerializeField] private Button reliquaryTabButton;
        [SerializeField] private Button weaponTabButton;

        [Header("List Settings")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject equipmentListItemPrefab;
        [SerializeField] private Sprite defaultEquipmentIcon;

        [Header("References")]
        [SerializeField] private EquipmentDetailPopup detailPopup;

        private EquipmentCategory currentCategory = EquipmentCategory.Hat;
        private List<GameObject> currentListItems = new List<GameObject>();
        private GameManager gameManager;

        public enum EquipmentCategory
        {
            Hat,
            Cloak,
            Boots,
            Reliquary,
            Weapon
        }

        void Start()
        {
            gameManager = GameManager.Instance;

            if (hatTabButton != null)
                hatTabButton.onClick.AddListener(() => SwitchToCategory(EquipmentCategory.Hat));

            if (cloakTabButton != null)
                cloakTabButton.onClick.AddListener(() => SwitchToCategory(EquipmentCategory.Cloak));

            if (bootsTabButton != null)
                bootsTabButton.onClick.AddListener(() => SwitchToCategory(EquipmentCategory.Boots));

            if (reliquaryTabButton != null)
                reliquaryTabButton.onClick.AddListener(() => SwitchToCategory(EquipmentCategory.Reliquary));

            if (weaponTabButton != null)
                weaponTabButton.onClick.AddListener(() => SwitchToCategory(EquipmentCategory.Weapon));

            SwitchToCategory(EquipmentCategory.Hat);
        }

        public void SwitchToCategory(EquipmentCategory category)
        {
            currentCategory = category;
            UpdateTabVisuals();
            PopulateGrid();
        }

        private void UpdateTabVisuals()
        {
            ResetAllTabs();

            switch (currentCategory)
            {
                case EquipmentCategory.Hat:
                    if (hatTabButton != null) HighlightTab(hatTabButton);
                    break;
                case EquipmentCategory.Cloak:
                    if (cloakTabButton != null) HighlightTab(cloakTabButton);
                    break;
                case EquipmentCategory.Boots:
                    if (bootsTabButton != null) HighlightTab(bootsTabButton);
                    break;
                case EquipmentCategory.Reliquary:
                    if (reliquaryTabButton != null) HighlightTab(reliquaryTabButton);
                    break;
                case EquipmentCategory.Weapon:
                    if (weaponTabButton != null) HighlightTab(weaponTabButton);
                    break;
            }
        }

        private void ResetAllTabs()
        {
            if (hatTabButton != null) SetTabColor(hatTabButton, Color.white);
            if (cloakTabButton != null) SetTabColor(cloakTabButton, Color.white);
            if (bootsTabButton != null) SetTabColor(bootsTabButton, Color.white);
            if (reliquaryTabButton != null) SetTabColor(reliquaryTabButton, Color.white);
            if (weaponTabButton != null) SetTabColor(weaponTabButton, Color.white);
        }

        private void HighlightTab(Button tab)
        {
            SetTabColor(tab, new Color(1f, 0.8f, 0.3f));
        }

        private void SetTabColor(Button button, Color color)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }

        private void PopulateGrid()
        {
            ClearGrid();

            if (gameManager == null)
            {
                Debug.LogError("GameManager is null! Cannot populate list.");
                return;
            }

            if (gameManager.EquipmentDB == null)
            {
                Debug.LogError("EquipmentDatabase is null! Make sure it's assigned in GameManager.");
                return;
            }

            List<ScriptableObject> items = GetItemsForCurrentCategory();

            Debug.Log($"Populating list for {currentCategory}: Found {items.Count} items");

            if (items.Count == 0)
            {
                Debug.LogWarning($"No items found for category {currentCategory}.");
                return;
            }

            if (equipmentListItemPrefab == null)
            {
                Debug.LogError("EquipmentListItemPrefab is null! Cannot create list items.");
                return;
            }

            if (listContainer == null)
            {
                Debug.LogError("ListContainer is null! Cannot parent list items.");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                CreateEquipmentListItem(items[i], i);
            }

            Debug.Log($"Created {currentListItems.Count} list items in container.");
        }

        private List<ScriptableObject> GetItemsForCurrentCategory()
        {
            List<ScriptableObject> items = new List<ScriptableObject>();

            switch (currentCategory)
            {
                case EquipmentCategory.Hat:
                    items.AddRange(gameManager.EquipmentDB.GetAllHats());
                    break;
                case EquipmentCategory.Cloak:
                    items.AddRange(gameManager.EquipmentDB.GetAllCloaks());
                    break;
                case EquipmentCategory.Boots:
                    items.AddRange(gameManager.EquipmentDB.GetAllBoots());
                    break;
                case EquipmentCategory.Reliquary:
                    items.AddRange(gameManager.EquipmentDB.GetAllReliquaries());
                    break;
                case EquipmentCategory.Weapon:
                    items.AddRange(gameManager.EquipmentDB.GetAllWeapons());
                    break;
            }

            return items;
        }

        private void CreateEquipmentListItem(ScriptableObject item, int index)
        {
            if (equipmentListItemPrefab == null || listContainer == null)
            {
                Debug.LogError($"Cannot create list item: prefab or container is null!");
                return;
            }

            GameObject listItemObj = Instantiate(equipmentListItemPrefab, listContainer);
            currentListItems.Add(listItemObj);

            Image iconImage = listItemObj.transform.Find("IconImage")?.GetComponent<Image>();
            TextMeshProUGUI nameText = listItemObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = listItemObj.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
            Button button = listItemObj.GetComponent<Button>();
            Image backgroundImage = listItemObj.GetComponent<Image>();

            string itemName = GetItemName(item);
            Sprite itemIcon = GetItemIcon(item);
            int itemCost = GetItemCost(item);

            if (nameText != null)
                nameText.text = itemName;
            
            if (costText != null)
                costText.text = itemCost > 0 ? $"{itemCost} souls" : "FREE";

            if (iconImage != null)
            {
                if (itemIcon != null)
                {
                    iconImage.sprite = itemIcon;
                    iconImage.color = Color.white;
                }
                else if (defaultEquipmentIcon != null)
                {
                    iconImage.sprite = defaultEquipmentIcon;
                    iconImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                else
                {
                    iconImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
            }

            bool isEquipped = IsItemCurrentlyEquipped(item);
            
            if (isEquipped && backgroundImage != null)
            {
                backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.9f);
                
                if (nameText != null)
                    nameText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    
                if (costText != null)
                    costText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            }

            if (button != null)
            {
                button.onClick.AddListener(() => OnEquipmentClicked(item));
            }

            Debug.Log($"Created list item: {itemName} (Icon: {(itemIcon != null ? "Yes" : "Default")}, Cost: {itemCost})");
        }

        private Sprite GetItemIcon(ScriptableObject item)
        {
            Sprite icon = null;

            if (item is HatData hat)
                icon = hat.Icon;
            else if (item is CloakData cloak)
                icon = cloak.Icon;
            else if (item is BootsData boots)
                icon = boots.Icon;
            else if (item is ReliquaryData reliquary)
                icon = reliquary.Icon;
            else if (item is WeaponData weapon)
                icon = weapon.Icon;

            return icon;
        }

        private int GetItemCost(ScriptableObject item)
        {
            if (item is HatData hat)
                return hat.RentalCost;
            else if (item is CloakData cloak)
                return cloak.RentalCost;
            else if (item is BootsData boots)
                return boots.RentalCost;
            else if (item is ReliquaryData reliquary)
                return reliquary.RentalCost;
            else if (item is WeaponData weapon)
                return weapon.RentalCost;

            return 0;
        }

        private bool IsItemCurrentlyEquipped(ScriptableObject item)
        {
            if (gameManager?.CurrentLoadout == null)
                return false;

            if (item is HatData hat)
                return gameManager.CurrentLoadout.hat == hat;
            else if (item is CloakData cloak)
                return gameManager.CurrentLoadout.cloak == cloak;
            else if (item is BootsData boots)
                return gameManager.CurrentLoadout.boots == boots;
            else if (item is ReliquaryData reliquary)
                return gameManager.CurrentLoadout.reliquary == reliquary;
            else if (item is WeaponData weapon)
                return gameManager.CurrentLoadout.weapon == weapon;

            return false;
        }

        private string GetItemName(ScriptableObject item)
        {
            if (item is HatData hat) return hat.EquipmentName;
            if (item is CloakData cloak) return cloak.EquipmentName;
            if (item is BootsData boots) return boots.EquipmentName;
            if (item is ReliquaryData reliquary) return reliquary.EquipmentName;
            if (item is WeaponData weapon) return weapon.WeaponName;
            return "Unknown";
        }

        private void OnEquipmentClicked(ScriptableObject item)
        {
            if (detailPopup != null)
            {
                detailPopup.ShowEquipmentDetails(item, currentCategory);
            }
        }

        private void ClearGrid()
        {
            Debug.Log($"ClearGrid called: destroying {currentListItems.Count} items");
            foreach (GameObject btn in currentListItems)
            {
                Destroy(btn);
            }
            currentListItems.Clear();
        }

        public void RefreshGrid()
        {
            PopulateGrid();
        }
    }
}
