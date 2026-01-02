using UnityEngine;

namespace RealmCrawler.Equipment
{
    [CreateAssetMenu(fileName = "New Hat", menuName = "RealmCrawler/Equipment/Hat Data")]
    public class HatData : EquipmentData
    {
        [Header("Hat Stats")]
        [SerializeField] private float manaBonus = 10f;

        public float ManaBonus => manaBonus;
    }
}
