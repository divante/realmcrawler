using UnityEngine;

namespace RealmCrawler.Equipment
{
    [CreateAssetMenu(fileName = "New Reliquary", menuName = "RealmCrawler/Equipment/Reliquary Data")]
    public class ReliquaryData : EquipmentData
    {
        [Header("Reliquary Stats")]
        [SerializeField] private float xpCollectRadiusBonus = 2f;

        public float XpCollectRadiusBonus => xpCollectRadiusBonus;
    }
}
