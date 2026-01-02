using UnityEngine;
using RealmCrawler.Spells;

namespace RealmCrawler.Equipment
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "RealmCrawler/Equipment/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string weaponName;
        [SerializeField] [TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;

        [Header("Cantrips")]
        [SerializeField] private CantripData primaryCantrip;
        [SerializeField] private CantripData secondaryCantrip;

        [Header("Stats")]
        [SerializeField] private float damageMultiplier = 1f;

        [Header("Spell Element Bonuses")]
        [SerializeField] private SpellElement buffedElement;
        [SerializeField] [Range(0f, 1f)] private float elementDamageBonus = 0.2f;

        [Header("Rental System")]
        [SerializeField] private int rentalCost = 0;

        [Header("Visuals")]
        [SerializeField] private GameObject weaponVisualPrefab;

        public string WeaponName => weaponName;
        public string Description => description;
        public Sprite Icon => icon;
        public CantripData PrimaryCantrip => primaryCantrip;
        public CantripData SecondaryCantrip => secondaryCantrip;
        public float DamageMultiplier => damageMultiplier;
        public SpellElement BuffedElement => buffedElement;
        public float ElementDamageBonus => elementDamageBonus;
        public int RentalCost => rentalCost;
        public GameObject WeaponVisualPrefab => weaponVisualPrefab;
    }
}
