using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using RealmCrawler.Core;
using RealmCrawler.Equipment;
using RealmCrawler.Player;
using RealmCrawler.CharacterStats;

namespace RealmCrawler.UI
{
    public class MainHubUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;

        private VisualElement root;
        private GameManager gameManager;
        private EquipmentManager equipmentManager;
        private CharacterData characterData;

        private Button startRunButton;
        private Button settingsButton;
        private Button mainMenuButton;

        private VisualElement statsPanel;

        private VisualElement buffsPanel;
        private VisualElement cantripsPanel;
        private VisualElement primaryCantripSlot;
        private VisualElement secondaryCantripSlot;
        private Label primaryCantripName;
        private Label secondaryCantripName;

        private VisualElement cantripTooltip;
        private Label cantripTooltipName;
        private Label cantripTooltipDescription;

        private void OnEnable()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            equipmentManager = FindFirstObjectByType<EquipmentManager>();
            characterData = FindFirstObjectByType<CharacterData>();

            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }

            if (uiDocument == null || uiDocument.visualTreeAsset == null)
            {
                Debug.LogError("MainHubUIController: UIDocument or VisualTreeAsset not assigned!");
                return;
            }

            root = uiDocument.rootVisualElement;
            BindUIElements();
            RegisterCallbacks();
            
            PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += RefreshUI;
            }
            
            RefreshUI();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
            
            PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= RefreshUI;
            }
        }

        private void BindUIElements()
        {
            startRunButton = root.Q<Button>("start-run-button");
            settingsButton = root.Q<Button>("settings-button");
            mainMenuButton = root.Q<Button>("main-menu-button");

            statsPanel = root.Q<VisualElement>("stats-panel");

            buffsPanel = root.Q<VisualElement>("buffs-panel");
            cantripsPanel = root.Q<VisualElement>("cantrips-panel");

            primaryCantripSlot = root.Q<VisualElement>("primary-cantrip-slot");
            secondaryCantripSlot = root.Q<VisualElement>("secondary-cantrip-slot");
            primaryCantripName = root.Q<Label>("primary-cantrip-name");
            secondaryCantripName = root.Q<Label>("secondary-cantrip-name");

            cantripTooltip = root.Q<VisualElement>("cantrip-tooltip");
            cantripTooltipName = root.Q<Label>("tooltip-cantrip-name");
            cantripTooltipDescription = root.Q<Label>("tooltip-cantrip-description");

            if (cantripTooltip != null)
            {
                cantripTooltip.style.display = DisplayStyle.None;
            }
        }

        private void RegisterCallbacks()
        {
            if (startRunButton != null)
            {
                startRunButton.clicked += OnStartRunClicked;
            }

            if (settingsButton != null)
            {
                settingsButton.clicked += OnSettingsClicked;
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.clicked += OnMainMenuClicked;
            }

            if (primaryCantripSlot != null)
            {
                primaryCantripSlot.RegisterCallback<MouseEnterEvent>(evt => OnCantripHoverEnter(true));
                primaryCantripSlot.RegisterCallback<MouseLeaveEvent>(evt => OnCantripHoverExit());
            }

            if (secondaryCantripSlot != null)
            {
                secondaryCantripSlot.RegisterCallback<MouseEnterEvent>(evt => OnCantripHoverEnter(false));
                secondaryCantripSlot.RegisterCallback<MouseLeaveEvent>(evt => OnCantripHoverExit());
            }
        }

        private void UnregisterCallbacks()
        {
            if (startRunButton != null)
            {
                startRunButton.clicked -= OnStartRunClicked;
            }

            if (settingsButton != null)
            {
                settingsButton.clicked -= OnSettingsClicked;
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.clicked -= OnMainMenuClicked;
            }

            if (primaryCantripSlot != null)
            {
                primaryCantripSlot.UnregisterCallback<MouseEnterEvent>(evt => OnCantripHoverEnter(true));
                primaryCantripSlot.UnregisterCallback<MouseLeaveEvent>(evt => OnCantripHoverExit());
            }

            if (secondaryCantripSlot != null)
            {
                secondaryCantripSlot.UnregisterCallback<MouseEnterEvent>(evt => OnCantripHoverEnter(false));
                secondaryCantripSlot.UnregisterCallback<MouseLeaveEvent>(evt => OnCantripHoverExit());
            }
        }

        public void RefreshUI()
        {
            RefreshStats();
            RefreshBuffs();
            RefreshCantrips();
        }

        private void RefreshStats()
        {
            if (statsPanel == null)
            {
                return;
            }

            statsPanel.Clear();

            if (characterData == null)
            {
                Label errorLabel = new Label("Character data not found");
                errorLabel.AddToClassList("stat-value");
                errorLabel.style.color = new Color(1f, 0.5f, 0.5f);
                statsPanel.Add(errorLabel);
                return;
            }

            System.Reflection.FieldInfo baseStatsField = characterData.GetType().GetField("baseStats", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (baseStatsField == null)
            {
                Debug.LogWarning("MainHubUIController: Could not access baseStats field");
                return;
            }

            var baseStats = baseStatsField.GetValue(characterData) as System.Collections.Generic.List<StatData>;

            if (baseStats == null || baseStats.Count == 0)
            {
                Label noStatsLabel = new Label("No stats configured");
                noStatsLabel.AddToClassList("stat-value");
                noStatsLabel.style.color = new Color(1f, 0.8f, 0.5f);
                statsPanel.Add(noStatsLabel);
                return;
            }

            foreach (StatData stat in baseStats)
            {
                if (stat == null) continue;

                float currentValue = characterData.GetStatValue(stat);
                Debug.Log($"MainHubUI: {stat.StatName} - BaseValue: {stat.BaseValue}, CurrentValue: {currentValue}");

                VisualElement statRow = new VisualElement();
                statRow.AddToClassList("stat-row");
                statRow.style.flexDirection = FlexDirection.Row;
                statRow.style.justifyContent = Justify.SpaceBetween;
                statRow.style.marginBottom = 5;

                Label statLabel = new Label($"{stat.StatName}:");
                statLabel.AddToClassList("stat-label");
                statLabel.style.color = Color.white;
                statLabel.style.fontSize = 16;

                Label valueLabel = new Label(FormatStatValue(currentValue));
                valueLabel.AddToClassList("stat-value");
                valueLabel.style.color = new Color(0.8f, 1f, 0.8f);
                valueLabel.style.fontSize = 16;
                valueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

                statRow.Add(statLabel);
                statRow.Add(valueLabel);
                statsPanel.Add(statRow);
            }
        }

        private string FormatStatValue(float value)
        {
            if (value % 1 == 0)
            {
                return value.ToString("F0");
            }
            else
            {
                return value.ToString("F1");
            }
        }

        private void RefreshBuffs()
        {
            if (buffsPanel == null)
            {
                return;
            }

            buffsPanel.Clear();
        }

        private void RefreshCantrips()
        {
            if (equipmentManager == null)
            {
                return;
            }

            if (primaryCantripName != null)
            {
                primaryCantripName.text = "Primary";
            }

            if (secondaryCantripName != null)
            {
                secondaryCantripName.text = "Secondary";
            }
        }

        private void OnCantripHoverEnter(bool isPrimary)
        {
            if (cantripTooltip == null)
            {
                return;
            }

            cantripTooltip.style.display = DisplayStyle.Flex;

            if (cantripTooltipName != null)
            {
                cantripTooltipName.text = isPrimary ? "Primary Cantrip" : "Secondary Cantrip";
            }

            if (cantripTooltipDescription != null)
            {
                cantripTooltipDescription.text = "Cantrip description here.";
            }
        }

        private void OnCantripHoverExit()
        {
            if (cantripTooltip != null)
            {
                cantripTooltip.style.display = DisplayStyle.None;
            }
        }

        private void OnStartRunClicked()
        {
            Debug.Log("MainHubUIController: Start Run clicked!");
            SceneManager.LoadScene("Scene_Main");
        }

        private void OnSettingsClicked()
        {
            Debug.Log("MainHubUIController: Settings clicked!");
        }

        private void OnMainMenuClicked()
        {
            Debug.Log("MainHubUIController: Main Menu clicked!");
            SceneManager.LoadScene("Scene_MainMenu");
        }
    }
}
