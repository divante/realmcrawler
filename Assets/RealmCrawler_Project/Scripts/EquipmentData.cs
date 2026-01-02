using UnityEngine;

namespace RealmCrawler.Equipment
{
    public enum EquipmentCategory
    {
        Hat,
        Cloak,
        Boots,
        Reliquary
    }

    public abstract class EquipmentData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string equipmentName;
        [SerializeField] [TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private EquipmentCategory category;

        [Header("Rental System")]
        [SerializeField] private int rentalCost = 0;

        [Header("Visuals")]
        [SerializeField] private GameObject visualPrefab;

        public string EquipmentName => equipmentName;
        public string Description => description;
        public Sprite Icon => icon;
        public EquipmentCategory Category => category;
        public int RentalCost => rentalCost;
        public GameObject VisualPrefab => visualPrefab;
    }
}
