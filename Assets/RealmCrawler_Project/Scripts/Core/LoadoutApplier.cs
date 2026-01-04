using UnityEngine;
using RealmCrawler.Core;

public class LoadoutApplier : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private bool debugMode = true;

    void Start()
    {
        if (equipmentManager == null)
        {
            equipmentManager = FindFirstObjectByType<EquipmentManager>();
        }

        if (equipmentManager == null)
        {
            Debug.LogError("LoadoutApplier: No EquipmentManager found in scene!");
            return;
        }

        ApplyLoadout();
    }

    private void ApplyLoadout()
    {
        if (GameManager.Instance != null)
        {
            if (debugMode)
            {
                Debug.Log("=== LOADOUT APPLIER - Applying Equipment from GameManager ===");
                Debug.Log($"Hat: {(GameManager.Instance.CurrentLoadout.hat != null ? GameManager.Instance.CurrentLoadout.hat.EquipmentName : "None")}");
                Debug.Log($"Cloak: {(GameManager.Instance.CurrentLoadout.cloak != null ? GameManager.Instance.CurrentLoadout.cloak.EquipmentName : "None")}");
                Debug.Log($"Boots: {(GameManager.Instance.CurrentLoadout.boots != null ? GameManager.Instance.CurrentLoadout.boots.EquipmentName : "None")}");
                Debug.Log($"Weapon: {(GameManager.Instance.CurrentLoadout.weapon != null ? GameManager.Instance.CurrentLoadout.weapon.WeaponName : "None")}");
                Debug.Log($"Reliquary: {(GameManager.Instance.CurrentLoadout.reliquary != null ? GameManager.Instance.CurrentLoadout.reliquary.EquipmentName : "None")}");
            }

            GameManager.Instance.ApplyLoadoutToPlayer(equipmentManager);

            if (debugMode)
            {
                Debug.Log($"Applied stats - Health: {equipmentManager.MaxHealth}, Mana: {equipmentManager.MaxMana}, Speed: {equipmentManager.MoveSpeed}, XP Radius: {equipmentManager.XpRadius}");
            }
        }
        else
        {
            Debug.LogWarning("LoadoutApplier: GameManager not found. Using default loadout.");
        }
    }
}
