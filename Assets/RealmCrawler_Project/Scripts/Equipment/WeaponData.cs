using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using RealmCrawler.Spells;
using RealmCrawler.CharacterStats;

namespace RealmCrawler.Equipment
{
    [Serializable]
    public class ComboNode
    {
        public CantripData hit;

        [SerializeReference]
        public ComboNode lightNext;

        [SerializeReference]
        public ComboNode heavyNext;
    }

    [CreateAssetMenu(fileName = "New Weapon", menuName = "RealmCrawler/Equipment/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField] private Guid itemId;
        [SerializeField] private string itemName;
        [SerializeField] [TextArea(2, 4)] private string flavorText;
        [SerializeField] private Sprite itemIcon;

        [Header("Classification")]
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;

        [Header("Combo Tree")]
        [SerializeField] private ComboNode lightComboRoot;
        [SerializeField] private ComboNode heavyComboRoot;

        [Header("Stat Modifiers")]
        [SerializeField] private StatModifierBase staticModifier;
        [SerializeField] private List<StatModifierBase> secondaryModifiers = new List<StatModifierBase>();

        [Header("Legacy Stats")]
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private SpellElement buffedElement;
        [SerializeField] [Range(0f, 1f)] private float elementDamageBonus = 0.2f;

        [Header("Rental System")]
        [SerializeField] private int rentalCost = 0;

        [Header("Visuals")]
        [SerializeField] private GameObject weaponVisualPrefab;

        public Guid ItemId => itemId;
        public string ItemName => itemName;
        public string FlavorText => flavorText;
        public Sprite ItemIcon => itemIcon;
        public ItemRarity Rarity => rarity;
        public ComboNode LightComboRoot => lightComboRoot;
        public ComboNode HeavyComboRoot => heavyComboRoot;

        // UI compatibility — returns the first hit in each combo chain
        public CantripData PrimaryCantrip => lightComboRoot?.hit;
        public CantripData SecondaryCantrip => heavyComboRoot?.hit;
        public StatModifierBase StaticModifier => staticModifier;
        public IReadOnlyList<StatModifierBase> SecondaryModifiers => secondaryModifiers;
        public float DamageMultiplier => damageMultiplier;
        public SpellElement BuffedElement => buffedElement;
        public float ElementDamageBonus => elementDamageBonus;
        public int RentalCost => rentalCost;
        public GameObject WeaponVisualPrefab => weaponVisualPrefab;

        [System.Obsolete("Use ItemName instead")]
        public string WeaponName => itemName;
        
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

        public void ApplyModifiers(CharacterData characterData)
        {
            if (staticModifier != null)
            {
                characterData.AddModifier(staticModifier);
            }

            foreach (var modifier in secondaryModifiers)
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

            foreach (var modifier in secondaryModifiers)
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
    [CustomEditor(typeof(WeaponData))]
    [CanEditMultipleObjects]
    public class WeaponDataEditor : Editor
    {
        private void OnEnable()
        {
            WeaponData weapon = (WeaponData)target;
            
            if (string.IsNullOrEmpty(weapon.ItemName))
            {
                string path = AssetDatabase.GetAssetPath(weapon);
                if (!string.IsNullOrEmpty(path))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    Undo.RecordObject(weapon, "Set Item Name");
                    weapon.SetItemName(fileName);
                    EditorUtility.SetDirty(weapon);
                }
            }
            
            if (weapon.ItemId == Guid.Empty)
            {
                weapon.SetItemId(Guid.NewGuid());
                EditorUtility.SetDirty(weapon);
            }
        }
    }
#endif
}
