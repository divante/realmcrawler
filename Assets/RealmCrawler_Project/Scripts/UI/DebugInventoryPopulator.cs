using UnityEngine;
using RealmCrawler.Core;
using RealmCrawler.Equipment;

namespace RealmCrawler.UI
{
    public class DebugInventoryPopulator : MonoBehaviour
    {
        [Header("Test Items")]
        [SerializeField] private ScriptableObject[] testItems;

        private void Start()
        {
            PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
            
            if (inventory == null)
            {
                Debug.LogError("DebugInventoryPopulator: PlayerInventory not found!");
                return;
            }

            if (testItems == null || testItems.Length == 0)
            {
                Debug.LogWarning("DebugInventoryPopulator: No test items assigned!");
                return;
            }

            Debug.Log($"DebugInventoryPopulator: Adding {testItems.Length} test items to inventory");
            
            foreach (var item in testItems)
            {
                if (item != null)
                {
                    inventory.AddItem(item);
                }
            }
        }
    }
}
