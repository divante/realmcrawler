using UnityEngine;
using RealmCrawler.Core;

namespace RealmCrawler.Core
{
    public class PlaySceneSetup : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool logGameManagerStatus = true;

        private void Start()
        {
            CheckGameManagerPresence();
        }

        private void CheckGameManagerPresence()
        {
            if (GameManager.Instance != null)
            {
                if (logGameManagerStatus)
                {
                    Debug.Log("✅ PlaySceneSetup: GameManager found and active!");
                    Debug.Log($"Current Souls: {GameManager.Instance.Souls}");
                    LogLoadout();
                }
            }
            else
            {
                Debug.LogWarning("⚠️ PlaySceneSetup: GameManager NOT found! Did you start from the Loadout scene?");
                Debug.LogWarning("For testing: You can add a GameManager to this scene, but normally it should persist from the loadout scene.");
            }
        }

        private void LogLoadout()
        {
            PlayerLoadout loadout = GameManager.Instance.CurrentLoadout;
            
            Debug.Log("=== CURRENT LOADOUT ===");
            Debug.Log($"Hat: {(loadout.hat != null ? loadout.hat.EquipmentName : "NONE")}");
            Debug.Log($"Cloak: {(loadout.cloak != null ? loadout.cloak.EquipmentName : "NONE")}");
            Debug.Log($"Boots: {(loadout.boots != null ? loadout.boots.EquipmentName : "NONE")}");
            Debug.Log($"Weapon: {(loadout.weapon != null ? loadout.weapon.WeaponName : "NONE")}");
            Debug.Log($"Reliquary: {(loadout.reliquary != null ? loadout.reliquary.EquipmentName : "NONE")}");
            
            LoadoutStats stats = loadout.CalculateStats();
            Debug.Log($"Total Loadout Cost: {loadout.CalculateTotalCost()} souls");
            Debug.Log($"Stats - Health: {stats.totalHealth}, Mana: {stats.totalMana}");
        }
    }
}
