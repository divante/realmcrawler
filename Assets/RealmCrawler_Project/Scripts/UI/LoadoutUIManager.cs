using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RealmCrawler.Core;

namespace RealmCrawler.UI
{
    public class LoadoutUIManager : MonoBehaviour
    {
        [Header("Top Bar")]
        [SerializeField] private TextMeshProUGUI loadoutCostText;
        [SerializeField] private TextMeshProUGUI currentSoulsText;

        [Header("Stats Panel")]
        [SerializeField] private TextMeshProUGUI healthStatText;
        [SerializeField] private TextMeshProUGUI manaStatText;
        [SerializeField] private TextMeshProUGUI speedStatText;
        [SerializeField] private TextMeshProUGUI xpRadiusStatText;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button backButton;

        [Header("References")]
        [SerializeField] private CantripDisplayPanel cantripDisplay;
        [SerializeField] private EquipmentGridManager equipmentGrid;

        [Header("Settings")]
        [SerializeField] private string gameSceneName = "GameScene";

        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;

            if (gameManager.CurrentLoadout == null || !gameManager.CurrentLoadout.IsValid())
            {
                gameManager.ResetToDefaultLoadout();
            }

            if (playButton != null)
                playButton.onClick.AddListener(OnPlayButtonClicked);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);

            RefreshUI();
        }

        public void RefreshUI()
        {
            UpdateTopBar();
            UpdateStatsDisplay();
            UpdatePlayButton();

            if (cantripDisplay != null)
                cantripDisplay.RefreshDisplay();

            if (equipmentGrid != null)
                equipmentGrid.RefreshGrid();
        }

        private void UpdateTopBar()
        {
            if (loadoutCostText != null)
            {
                int cost = gameManager.CurrentLoadout.CalculateTotalCost();
                loadoutCostText.text = $"Current Loadout Cost: {cost}";
                
                if (cost > gameManager.Souls)
                    loadoutCostText.color = Color.red;
                else
                    loadoutCostText.color = Color.white;
            }

            if (currentSoulsText != null)
            {
                currentSoulsText.text = $"Current Souls: {gameManager.Souls}";
            }
        }

        private void UpdateStatsDisplay()
        {
            LoadoutStats stats = gameManager.CurrentLoadout.CalculateStats();

            if (healthStatText != null)
            {
                float bonus = stats.totalHealth - stats.baseHealth;
                string sign = bonus >= 0 ? "+" : "";
                healthStatText.text = $"Health: {stats.totalHealth:F0} ({sign}{bonus:F0})";
            }

            if (manaStatText != null)
            {
                float bonus = stats.totalMana - stats.baseMana;
                string sign = bonus >= 0 ? "+" : "";
                manaStatText.text = $"Mana: {stats.totalMana:F0} ({sign}{bonus:F0})";
            }

            if (speedStatText != null)
            {
                float bonus = stats.totalSpeed - stats.baseSpeed;
                string sign = bonus >= 0 ? "+" : "";
                speedStatText.text = $"Speed: {stats.totalSpeed:F1} ({sign}{bonus:F1})";
            }

            if (xpRadiusStatText != null)
            {
                float bonus = stats.totalXpRadius - stats.baseXpRadius;
                string sign = bonus >= 0 ? "+" : "";
                xpRadiusStatText.text = $"XP Radius: {stats.totalXpRadius:F1} ({sign}{bonus:F1})";
            }
        }

        private void UpdatePlayButton()
        {
            if (playButton != null)
            {
                bool canStart = gameManager.CanAffordLoadout() && gameManager.CurrentLoadout.IsValid();
                playButton.interactable = canStart;
            }
        }

        private void OnPlayButtonClicked()
        {
            if (gameManager.CanAffordLoadout())
            {
                gameManager.LoadGameScene(gameSceneName);
            }
            else
            {
                Debug.LogWarning("Cannot afford loadout!");
            }
        }

        private void OnBackButtonClicked()
        {
            Debug.Log("Back button clicked - implement main menu transition here");
        }
    }
}
