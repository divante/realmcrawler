using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RealmCrawler.Core;
using RealmCrawler.Equipment;

namespace RealmCrawler.UI
{
    public class EquipmentDetailPopup : MonoBehaviour
    {
        [Header("Popup Panel")]
        [SerializeField] private GameObject popupPanel;

        [Header("Display Elements")]
        [SerializeField] private Image equipmentIconImage;
        [SerializeField] private TextMeshProUGUI equipmentNameText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI buffText;
        [SerializeField] private TextMeshProUGUI costText;

        [Header("Buttons")]
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button backButton;

        [Header("References")]
        [SerializeField] private LoadoutUIManager loadoutUIManager;

        private ScriptableObject currentItem;
        private EquipmentGridManager.EquipmentCategory currentCategory;
        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;

            if (purchaseButton != null)
                purchaseButton.onClick.AddListener(OnPurchaseClicked);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);

            HidePopup();
        }

        public void ShowEquipmentDetails(ScriptableObject item, EquipmentGridManager.EquipmentCategory category)
        {
            currentItem = item;
            currentCategory = category;

            if (popupPanel != null)
                popupPanel.SetActive(true);

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (currentItem == null)
                return;

            string itemName = "";
            string stats = "";
            string buff = "";
            int cost = 0;

            if (currentItem is HatData hat)
            {
                itemName = hat.EquipmentName;
                stats = $"Mana Bonus: +{hat.ManaBonus}";
                buff = "Increases maximum mana";
                cost = hat.RentalCost;
            }
            else if (currentItem is CloakData cloak)
            {
                itemName = cloak.EquipmentName;
                stats = $"Health Bonus: +{cloak.HealthBonus}";
                buff = "Increases maximum health";
                cost = cloak.RentalCost;
            }
            else if (currentItem is BootsData boots)
            {
                itemName = boots.EquipmentName;
                stats = $"Speed Multiplier: x{boots.SpeedMultiplier:F2}";
                buff = $"Increases movement speed by {(boots.SpeedMultiplier - 1f) * 100f:F0}%";
                cost = boots.RentalCost;
            }
            else if (currentItem is ReliquaryData reliquary)
            {
                itemName = reliquary.EquipmentName;
                stats = $"XP Radius: +{reliquary.XpCollectRadiusBonus}";
                buff = "Increases XP collection radius";
                cost = reliquary.RentalCost;
            }
            else if (currentItem is WeaponData weapon)
            {
                itemName = weapon.WeaponName;
                stats = $"Damage Multiplier: x{weapon.DamageMultiplier:F2}";
                
                string cantripInfo = "";
                if (weapon.PrimaryCantrip != null)
                    cantripInfo += $"Primary: {weapon.PrimaryCantrip.CantripName}\n";
                if (weapon.SecondaryCantrip != null)
                    cantripInfo += $"Secondary: {weapon.SecondaryCantrip.CantripName}";
                
                buff = cantripInfo;
                cost = weapon.RentalCost;
            }

            if (equipmentNameText != null)
                equipmentNameText.text = itemName;

            if (statsText != null)
                statsText.text = stats;

            if (buffText != null)
                buffText.text = buff;

            if (costText != null)
            {
                costText.text = $"Cost: {cost} Souls";
                
                if (cost > gameManager.Souls)
                    costText.color = Color.red;
                else
                    costText.color = Color.white;
            }

            if (purchaseButton != null)
            {
                purchaseButton.interactable = (cost <= gameManager.Souls);
            }
        }

        private void OnPurchaseClicked()
        {
            if (currentItem == null)
                return;

            switch (currentCategory)
            {
                case EquipmentGridManager.EquipmentCategory.Hat:
                    gameManager.CurrentLoadout.hat = currentItem as HatData;
                    break;
                case EquipmentGridManager.EquipmentCategory.Cloak:
                    gameManager.CurrentLoadout.cloak = currentItem as CloakData;
                    break;
                case EquipmentGridManager.EquipmentCategory.Boots:
                    gameManager.CurrentLoadout.boots = currentItem as BootsData;
                    break;
                case EquipmentGridManager.EquipmentCategory.Reliquary:
                    gameManager.CurrentLoadout.reliquary = currentItem as ReliquaryData;
                    break;
                case EquipmentGridManager.EquipmentCategory.Weapon:
                    gameManager.CurrentLoadout.weapon = currentItem as WeaponData;
                    break;
            }

            if (loadoutUIManager != null)
                loadoutUIManager.RefreshUI();

            HidePopup();
        }

        private void OnBackClicked()
        {
            HidePopup();
        }

        public void HidePopup()
        {
            if (popupPanel != null)
                popupPanel.SetActive(false);

            currentItem = null;
        }
    }
}
