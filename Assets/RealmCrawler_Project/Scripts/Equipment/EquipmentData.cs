using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using RealmCrawler.CharacterStats;
using RealmCrawler.Spells;

namespace RealmCrawler.Equipment
{
    public enum EquipmentCategory
    {
        Hat,
        Cloak,
        Boots,
        Reliquary
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public abstract class EquipmentData : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private Guid itemId;
        [SerializeField] private string itemName;
        [SerializeField] [TextArea(2, 4)] private string flavorText;
        [SerializeField] private Sprite itemIcon;

        [Header("Classification")]
        [SerializeField] private EquipmentCategory category;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;

        [Header("Spell")]
        [SerializeField] private SpellData spell;

        [Header("Stat Modifiers")]
        [SerializeField] private StatModifierBase staticModifier;
        [SerializeField] private List<StatModifierBase> modifiers = new List<StatModifierBase>();

        [Header("Rental System")]
        [SerializeField] private int rentalCost = 0;

        [Header("Visuals")]
        [SerializeField] private GameObject visualPrefab;

        public Guid ItemId => itemId;
        public string ItemName => itemName;
        public string FlavorText => flavorText;
        public Sprite ItemIcon => itemIcon;
        public EquipmentCategory Category => category;
        public ItemRarity Rarity => rarity;
        public SpellData Spell => spell;
        public StatModifierBase StaticModifier => staticModifier;
        public IReadOnlyList<StatModifierBase> Modifiers => modifiers;
        public int RentalCost => rentalCost;
        public GameObject VisualPrefab => visualPrefab;

        [System.Obsolete("Use ItemName instead")]
        public string EquipmentName => itemName;
        
        [System.Obsolete("Use ItemIcon instead")]
        public Sprite Icon => itemIcon;
        
        [System.Obsolete("Use FlavorText instead")]
        public string Description => flavorText;

        internal void SetItemId(Guid id)
        {
            itemId = id;
        }

        internal void SetItemName(string name)
        {
            itemName = name;
        }

        public void ReplaceModifier(int index, StatModifierBase newModifier)
        {
            if (index < 0 || index >= modifiers.Count)
            {
                Debug.LogWarning($"EquipmentData: Invalid modifier index {index}");
                return;
            }

            modifiers[index] = newModifier;
        }

        public void AddModifier(StatModifierBase modifier)
        {
            if (modifier != null)
            {
                modifiers.Add(modifier);
            }
        }

        public void ClearModifiers()
        {
            modifiers.Clear();
        }

        public void RemoveModifierAt(int index)
        {
            if (index >= 0 && index < modifiers.Count)
            {
                modifiers.RemoveAt(index);
            }
        }

        public void ApplyModifiers(CharacterData characterData)
        {
            if (staticModifier != null)
            {
                characterData.AddModifier(staticModifier);
            }

            foreach (var modifier in modifiers)
            {
                if (modifier != null)
                {
                    characterData.AddModifier(modifier);
                }
            }
        }

        public void RemoveModifiers(CharacterData characterData)
        {
            if (staticModifier != null)
            {
                var handler = characterData.GetComponent<Transform>()
                    .Find($"{staticModifier.name}_Handler")
                    ?.GetComponent<StatModifierHandlerBase>();
                
                if (handler != null)
                {
                    characterData.RemoveModifier(handler);
                }
            }

            foreach (var modifier in modifiers)
            {
                if (modifier != null)
                {
                    var handler = characterData.GetComponent<Transform>()
                        .Find($"{modifier.name}_Handler")
                        ?.GetComponent<StatModifierHandlerBase>();
                    
                    if (handler != null)
                    {
                        characterData.RemoveModifier(handler);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EquipmentData), true)]
    [CanEditMultipleObjects]
    public class EquipmentDataEditor : Editor
    {
        private void OnEnable()
        {
            EquipmentData equipment = (EquipmentData)target;
            
            if (string.IsNullOrEmpty(equipment.ItemName))
            {
                string path = AssetDatabase.GetAssetPath(equipment);
                if (!string.IsNullOrEmpty(path))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    Undo.RecordObject(equipment, "Set Item Name");
                    equipment.SetItemName(fileName);
                    EditorUtility.SetDirty(equipment);
                }
            }
            
            if (equipment.ItemId == Guid.Empty)
            {
                equipment.SetItemId(Guid.NewGuid());
                EditorUtility.SetDirty(equipment);
            }
        }
    }
#endif
}
