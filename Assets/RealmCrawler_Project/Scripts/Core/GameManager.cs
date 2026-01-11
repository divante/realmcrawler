using UnityEngine;
using UnityEngine.SceneManagement;
using RealmCrawler.Equipment;

namespace RealmCrawler.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
                return instance;
            }
        }

        [Header("Player Resources")]
        [SerializeField] private int souls = 1000;

        [Header("Current Loadout")]
        [SerializeField] private PlayerLoadout currentLoadout;

        [Header("Equipment Database")]
        [SerializeField] private EquipmentDatabase equipmentDatabase;

        public int Souls
        {
            get => souls;
            private set => souls = Mathf.Max(0, value);
        }

        public PlayerLoadout CurrentLoadout => currentLoadout;
        public EquipmentDatabase EquipmentDB => equipmentDatabase;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            if (currentLoadout == null)
            {
                currentLoadout = new PlayerLoadout();
            }
        }

        public bool CanAffordLoadout()
        {
            int totalCost = currentLoadout.CalculateTotalCost();
            return souls >= totalCost;
        }

        public bool RentLoadout()
        {
            int totalCost = currentLoadout.CalculateTotalCost();
            
            if (souls < totalCost)
            {
                Debug.LogWarning($"Not enough souls! Need {totalCost}, have {souls}");
                return false;
            }

            souls -= totalCost;
            Debug.Log($"Loadout rented for {totalCost} souls. Remaining: {souls}");
            return true;
        }

        public void AddSouls(int amount)
        {
            souls += amount;
            Debug.Log($"Added {amount} souls. Total: {souls}");
        }

        public void ModifySouls(int amount)
        {
            souls += amount;
            souls = Mathf.Max(0, souls);
            Debug.Log($"Modified souls by {amount}. Total: {souls}");
        }

        public void SetLoadout(HatData hat, CloakData cloak, BootsData boots, WeaponData weapon, ReliquaryData reliquary)
        {
            currentLoadout.hat = hat;
            currentLoadout.cloak = cloak;
            currentLoadout.boots = boots;
            currentLoadout.weapon = weapon;
            currentLoadout.reliquary = reliquary;
        }

        public void LoadGameScene(string sceneName = "GameScene")
        {
            if (!CanAffordLoadout())
            {
                Debug.LogError("Cannot start run - insufficient souls!");
                return;
            }

            if (!RentLoadout())
            {
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void LoadLoadoutScene(string sceneName = "LoadoutScene")
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ApplyLoadoutToPlayer(EquipmentManager equipmentManager)
        {
            if (equipmentManager == null)
            {
                Debug.LogError("EquipmentManager is null!");
                return;
            }

            equipmentManager.SetEquipment(
                currentLoadout.hat,
                currentLoadout.cloak,
                currentLoadout.boots,
                currentLoadout.weapon,
                currentLoadout.reliquary
            );

            equipmentManager.ApplyEquipmentLoadout();
            Debug.Log("Loadout applied to player!");
        }

        public void ResetToDefaultLoadout()
        {
            if (equipmentDatabase != null)
            {
                currentLoadout.hat = equipmentDatabase.GetDefaultHat();
                currentLoadout.cloak = equipmentDatabase.GetDefaultCloak();
                currentLoadout.boots = equipmentDatabase.GetDefaultBoots();
                currentLoadout.weapon = equipmentDatabase.GetDefaultWeapon();
                currentLoadout.reliquary = equipmentDatabase.GetDefaultReliquary();
            }
        }
    }

    [System.Serializable]
    public class PlayerLoadout
    {
        public HatData hat;
        public CloakData cloak;
        public BootsData boots;
        public WeaponData weapon;
        public ReliquaryData reliquary;

        public int CalculateTotalCost()
        {
            int total = 0;
            if (hat != null) total += hat.RentalCost;
            if (cloak != null) total += cloak.RentalCost;
            if (boots != null) total += boots.RentalCost;
            if (weapon != null) total += weapon.RentalCost;
            if (reliquary != null) total += reliquary.RentalCost;
            return total;
        }

        public bool IsValid()
        {
            return hat != null && cloak != null && boots != null && weapon != null && reliquary != null;
        }

        public LoadoutStats CalculateStats()
        {
            LoadoutStats stats = new LoadoutStats();
            
            stats.baseHealth = 100f;
            stats.baseMana = 100f;
            stats.baseSpeed = 10f;
            stats.baseXpRadius = 5f;

            // NOTE: Old direct stat bonus system removed - use StatModifierBase system instead
            // Equipment now applies bonuses via ApplyModifiers(CharacterData) method
            stats.totalMana = stats.baseMana;
            stats.totalHealth = stats.baseHealth;
            stats.totalSpeed = stats.baseSpeed;
            stats.totalXpRadius = stats.baseXpRadius;

            return stats;
        }
    }

    [System.Serializable]
    public struct LoadoutStats
    {
        public float baseHealth;
        public float baseMana;
        public float baseSpeed;
        public float baseXpRadius;

        public float totalHealth;
        public float totalMana;
        public float totalSpeed;
        public float totalXpRadius;
    }
}
