using UnityEngine;
using RealmCrawler.Core;

public class LoadoutApplier : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;

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
            GameManager.Instance.ApplyLoadoutToPlayer(equipmentManager);
        }
        else
        {
            Debug.LogWarning("LoadoutApplier: GameManager not found. Using default loadout.");
        }
    }
}
