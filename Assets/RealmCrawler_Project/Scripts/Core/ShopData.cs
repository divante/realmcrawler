using UnityEngine;
using System;
using System.Collections.Generic;
using RealmCrawler.Equipment;

namespace RealmCrawler.Core
{
    [Serializable]
    public class ShopItemEntry
    {
        public ScriptableObject item;
        public int stockQuantity = 1;
        [Range(0f, 1f)] public float spawnChance = 1f;

        [HideInInspector] public int currentStock;
        [HideInInspector] public bool isPurchased;

        public ShopItemEntry()
        {
            currentStock = stockQuantity;
            isPurchased = false;
        }
    }

    [CreateAssetMenu(fileName = "New Shop Data", menuName = "RealmCrawler/Shop/Shop Data")]
    public class ShopData : ScriptableObject
    {
        [Header("Shop Configuration")]
        [SerializeField] private string shopName = "Merchant's Shop";
        [SerializeField] [TextArea(2, 4)] private string shopDialogue = "Welcome, traveler! Browse my wares.";

        [Header("Item Pool")]
        [SerializeField] private List<ShopItemEntry> hatPool = new List<ShopItemEntry>();
        [SerializeField] private List<ShopItemEntry> cloakPool = new List<ShopItemEntry>();
        [SerializeField] private List<ShopItemEntry> bootsPool = new List<ShopItemEntry>();
        [SerializeField] private List<ShopItemEntry> reliquaryPool = new List<ShopItemEntry>();
        [SerializeField] private List<ShopItemEntry> weaponPool = new List<ShopItemEntry>();

        [Header("Randomization Settings")]
        [SerializeField] private bool randomizeStock = false;
        [SerializeField] [Range(1, 20)] private int minItemsPerCategory = 3;
        [SerializeField] [Range(1, 20)] private int maxItemsPerCategory = 5;

        public string ShopName => shopName;
        public string ShopDialogue => shopDialogue;
        public bool RandomizeStock => randomizeStock;

        public List<ShopItemEntry> GetAvailableHats() => GetAvailableItems(hatPool);
        public List<ShopItemEntry> GetAvailableCloaks() => GetAvailableItems(cloakPool);
        public List<ShopItemEntry> GetAvailableBoots() => GetAvailableItems(bootsPool);
        public List<ShopItemEntry> GetAvailableReliquaries() => GetAvailableItems(reliquaryPool);
        public List<ShopItemEntry> GetAvailableWeapons() => GetAvailableItems(weaponPool);

        private List<ShopItemEntry> GetAvailableItems(List<ShopItemEntry> pool)
        {
            if (!randomizeStock)
            {
                return new List<ShopItemEntry>(pool);
            }

            List<ShopItemEntry> available = new List<ShopItemEntry>();
            int targetCount = UnityEngine.Random.Range(minItemsPerCategory, maxItemsPerCategory + 1);

            List<ShopItemEntry> eligibleItems = pool.FindAll(entry => 
                UnityEngine.Random.value <= entry.spawnChance
            );

            for (int i = 0; i < Mathf.Min(targetCount, eligibleItems.Count); i++)
            {
                if (eligibleItems.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, eligibleItems.Count);
                    ShopItemEntry selected = eligibleItems[randomIndex];
                    
                    ShopItemEntry copy = new ShopItemEntry
                    {
                        item = selected.item,
                        stockQuantity = selected.stockQuantity,
                        spawnChance = selected.spawnChance,
                        currentStock = selected.stockQuantity,
                        isPurchased = false
                    };
                    
                    available.Add(copy);
                    eligibleItems.RemoveAt(randomIndex);
                }
            }

            return available;
        }

        public void ResetStock()
        {
            ResetPoolStock(hatPool);
            ResetPoolStock(cloakPool);
            ResetPoolStock(bootsPool);
            ResetPoolStock(reliquaryPool);
            ResetPoolStock(weaponPool);
        }

        private void ResetPoolStock(List<ShopItemEntry> pool)
        {
            foreach (var entry in pool)
            {
                entry.currentStock = entry.stockQuantity;
                entry.isPurchased = false;
            }
        }
    }
}
