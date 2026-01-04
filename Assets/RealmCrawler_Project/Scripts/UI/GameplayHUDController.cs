using UnityEngine;
using UnityEngine.UIElements;
using RealmCrawler.Equipment;
using RealmCrawler.Spells;

namespace RealmCrawler.UI
{
    public class GameplayHUDController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private EquipmentManager equipmentManager;

        [Header("UI Icons")]
        [SerializeField] private Sprite healthIconSprite;
        [SerializeField] private Sprite manaIconSprite;
        [SerializeField] private Sprite pauseButtonIconSprite;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private VisualElement root;

        private VisualElement healthIcon;
        private Label healthValue;
        private VisualElement healthBarFill;
        private VisualElement manaIcon;
        private Label manaValue;
        private VisualElement manaBarFill;

        private Label waveNumber;
        private VisualElement xpBarFill;

        private VisualElement primaryCantripIcon;
        private VisualElement secondaryCantripIcon;
        private VisualElement pauseIcon;

        private float currentHealth;
        private float maxHealth;
        private float currentMana;
        private float maxMana;

        private WeaponData currentWeapon;

        private void OnEnable()
        {
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }

            if (equipmentManager == null)
            {
                equipmentManager = FindFirstObjectByType<EquipmentManager>();
            }

            SetupUI();
            InitializeValues();
            SubscribeToEquipmentEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEquipmentEvents();
        }

        private void SubscribeToEquipmentEvents()
        {
            if (equipmentManager != null)
            {
                equipmentManager.OnWeaponEquipped.AddListener(OnWeaponChanged);
            }
        }

        private void UnsubscribeFromEquipmentEvents()
        {
            if (equipmentManager != null)
            {
                equipmentManager.OnWeaponEquipped.RemoveListener(OnWeaponChanged);
            }
        }

        private void OnWeaponChanged(WeaponData newWeapon)
        {
            currentWeapon = newWeapon;
            UpdateCantripIcons();
        }

        private void SetupUI()
        {
            root = uiDocument.rootVisualElement;

            healthIcon = root.Q<VisualElement>("health-icon");
            healthValue = root.Q<Label>("health-value");
            healthBarFill = root.Q<VisualElement>("health-bar-fill");
            manaIcon = root.Q<VisualElement>("mana-icon");
            manaValue = root.Q<Label>("mana-value");
            manaBarFill = root.Q<VisualElement>("mana-bar-fill");

            waveNumber = root.Q<Label>("wave-number");
            xpBarFill = root.Q<VisualElement>("xp-bar-fill");

            primaryCantripIcon = root.Q<VisualElement>("primary-cantrip-icon");
            secondaryCantripIcon = root.Q<VisualElement>("secondary-cantrip-icon");

            pauseIcon = root.Q<VisualElement>("pause-icon");

            Button pauseButton = root.Q<Button>("pause-button");
            if (pauseButton != null)
            {
                pauseButton.clicked += OnPauseButtonClicked;
            }

            ApplyCustomIcons();

            if (showDebugLogs)
            {
                Debug.Log("GameplayHUD: UI elements queried successfully.");
            }
        }

        private void ApplyCustomIcons()
        {
            if (healthIconSprite != null && healthIcon != null)
            {
                healthIcon.style.backgroundImage = new StyleBackground(healthIconSprite);
                if (showDebugLogs)
                {
                    Debug.Log("GameplayHUD: Applied custom health icon.");
                }
            }

            if (manaIconSprite != null && manaIcon != null)
            {
                manaIcon.style.backgroundImage = new StyleBackground(manaIconSprite);
                if (showDebugLogs)
                {
                    Debug.Log("GameplayHUD: Applied custom mana icon.");
                }
            }

            if (pauseButtonIconSprite != null && pauseIcon != null)
            {
                pauseIcon.style.backgroundImage = new StyleBackground(pauseButtonIconSprite);
                if (showDebugLogs)
                {
                    Debug.Log("GameplayHUD: Applied custom pause button icon.");
                }
            }
        }

        private void InitializeValues()
        {
            SetWaveNumber(1);
            SetXPProgress(0f);

            currentHealth = 100f;
            maxHealth = 100f;
            currentMana = 100f;
            maxMana = 100f;
            
            if (equipmentManager == null)
            {
                Debug.LogWarning("GameplayHUD: EquipmentManager not found. Using default values.");
            }
        }

        private void Start()
        {
            RefreshFromEquipmentManager();
            UpdateCantripIcons();
        }

        private void RefreshFromEquipmentManager()
        {
            if (equipmentManager != null)
            {
                maxHealth = equipmentManager.MaxHealth;
                currentHealth = maxHealth;
                maxMana = equipmentManager.MaxMana;
                currentMana = maxMana;

                UpdateHealthUI();
                UpdateManaUI();

                if (showDebugLogs)
                {
                    Debug.Log($"GameplayHUD refreshed - Health: {currentHealth}/{maxHealth}, Mana: {currentMana}/{maxMana}");
                }
            }
        }

        private void UpdateCantripIcons()
        {
            if (equipmentManager == null || equipmentManager.EquippedWeapon == null)
            {
                if (showDebugLogs)
                {
                    Debug.Log("GameplayHUD: No weapon equipped. Cantrip icons will remain empty.");
                }
                return;
            }

            currentWeapon = equipmentManager.EquippedWeapon;

            if (currentWeapon.PrimaryCantrip != null && currentWeapon.PrimaryCantrip.Icon != null)
            {
                SetCantripIcon(primaryCantripIcon, currentWeapon.PrimaryCantrip.Icon);
                if (showDebugLogs)
                {
                    Debug.Log($"GameplayHUD: Set primary cantrip icon - {currentWeapon.PrimaryCantrip.CantripName}");
                }
            }

            if (currentWeapon.SecondaryCantrip != null && currentWeapon.SecondaryCantrip.Icon != null)
            {
                SetCantripIcon(secondaryCantripIcon, currentWeapon.SecondaryCantrip.Icon);
                if (showDebugLogs)
                {
                    Debug.Log($"GameplayHUD: Set secondary cantrip icon - {currentWeapon.SecondaryCantrip.CantripName}");
                }
            }
        }

        private void SetCantripIcon(VisualElement iconElement, Sprite sprite)
        {
            if (iconElement == null || sprite == null) return;

            iconElement.style.backgroundImage = new StyleBackground(sprite);
        }

        public void SetHealth(float current, float max)
        {
            currentHealth = Mathf.Clamp(current, 0, max);
            maxHealth = max;
            UpdateHealthUI();
        }

        public void SetMana(float current, float max)
        {
            currentMana = Mathf.Clamp(current, 0, max);
            maxMana = max;
            UpdateManaUI();
        }

        private void UpdateHealthUI()
        {
            if (healthValue != null)
            {
                healthValue.text = $"{Mathf.RoundToInt(currentHealth)}";
            }

            if (healthBarFill != null)
            {
                float percentage = maxHealth > 0 ? (currentHealth / maxHealth) * 100f : 0f;
                healthBarFill.style.width = Length.Percent(percentage);
            }
        }

        private void UpdateManaUI()
        {
            if (manaValue != null)
            {
                manaValue.text = $"{Mathf.RoundToInt(currentMana)}";
            }

            if (manaBarFill != null)
            {
                float percentage = maxMana > 0 ? (currentMana / maxMana) * 100f : 0f;
                manaBarFill.style.width = Length.Percent(percentage);
            }
        }

        public void SetWaveNumber(int wave)
        {
            if (waveNumber != null)
            {
                waveNumber.text = $"Wave {wave}";
            }
        }

        public void SetXPProgress(float percentage)
        {
            if (xpBarFill != null)
            {
                percentage = Mathf.Clamp01(percentage);
                xpBarFill.style.width = Length.Percent(percentage * 100f);
            }
        }

        private void OnPauseButtonClicked()
        {
            Debug.Log("Pause button clicked - Pause menu not implemented yet.");
        }

        private void Update()
        {
            if (equipmentManager != null)
            {
                if (maxHealth != equipmentManager.MaxHealth || maxMana != equipmentManager.MaxMana)
                {
                    maxHealth = equipmentManager.MaxHealth;
                    maxMana = equipmentManager.MaxMana;
                    
                    currentHealth = Mathf.Min(currentHealth, maxHealth);
                    currentMana = Mathf.Min(currentMana, maxMana);
                    
                    UpdateHealthUI();
                    UpdateManaUI();
                }
            }
        }
    }
}
