using UnityEngine;
using System;
using System.Collections.Generic;
using RealmCrawler.Equipment;
using RealmCrawler.CharacterStats;

namespace RealmCrawler.Core
{
    [Serializable]
    public class InventoryItem
    {
        public Guid itemId;
        public ScriptableObject itemData;
        public int quantity;

        public InventoryItem(ScriptableObject data)
        {
            if (data is EquipmentData equipment)
            {
                itemId = equipment.ItemId;
            }
            else if (data is WeaponData weapon)
            {
                itemId = weapon.ItemId;
            }
            
            itemData = data;
            quantity = 1;
        }
    }

    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Data")]
        [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

        public IReadOnlyList<InventoryItem> Items => items;

        public event Action<InventoryItem> OnItemAdded;
        public event Action<InventoryItem> OnItemRemoved;
        public event Action OnInventoryChanged;

        public void AddItem(ScriptableObject itemData)
        {
            if (itemData == null)
            {
                Debug.LogWarning("PlayerInventory: Cannot add null item");
                return;
            }

            ScriptableObject itemInstance;
            
            if (itemData is EquipmentData equipment)
            {
                itemInstance = CreateEquipmentInstance(equipment);
            }
            else if (itemData is WeaponData weapon)
            {
                itemInstance = CreateWeaponInstance(weapon);
            }
            else
            {
                itemInstance = itemData;
            }

            InventoryItem newItem = new InventoryItem(itemInstance);
            items.Add(newItem);
            Debug.Log($"PlayerInventory: Added new unique instance of {itemData.name}");

            OnItemAdded?.Invoke(newItem);
            OnInventoryChanged?.Invoke();
        }

        private EquipmentData CreateEquipmentInstance(EquipmentData original)
        {
            EquipmentData instance = Instantiate(original);
            instance.name = $"{original.name} (Instance)";
            
            if (original.Modifiers != null && original.Modifiers.Count > 0)
            {
                instance.ClearModifiers();
                foreach (var modifier in original.Modifiers)
                {
                    if (modifier != null)
                    {
                        StatModifierBase modifierCopy = Instantiate(modifier);
                        instance.AddModifier(modifierCopy);
                    }
                }
            }
            
            return instance;
        }

        private WeaponData CreateWeaponInstance(WeaponData original)
        {
            WeaponData instance = Instantiate(original);
            instance.name = $"{original.name} (Instance)";
            return instance;
        }

        public bool RemoveItem(Guid itemId, int quantity = 1)
        {
            InventoryItem item = items.Find(i => i.itemId == itemId);
            
            if (item == null)
            {
                Debug.LogWarning($"PlayerInventory: Item with ID {itemId} not found");
                return false;
            }

            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                items.Remove(item);
                Debug.Log($"PlayerInventory: Removed {item.itemData.name} from inventory");
            }

            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool HasItem(Guid itemId)
        {
            return items.Exists(i => i.itemId == itemId);
        }

        public int GetItemCount(Guid itemId)
        {
            InventoryItem item = items.Find(i => i.itemId == itemId);
            return item?.quantity ?? 0;
        }

        public List<InventoryItem> GetItemsByCategory(EquipmentCategory category)
        {
            return items.FindAll(i => 
            {
                if (i.itemData is EquipmentData equipment)
                {
                    return equipment.Category == category;
                }
                return false;
            });
        }

        public List<InventoryItem> GetWeapons()
        {
            return items.FindAll(i => i.itemData is WeaponData);
        }

        public List<InventoryItem> GetAllEquipment()
        {
            return items.FindAll(i => i.itemData is EquipmentData);
        }

        public void ClearInventory()
        {
            items.Clear();
            OnInventoryChanged?.Invoke();
            Debug.Log("PlayerInventory: Cleared all items");
        }
    }
}
