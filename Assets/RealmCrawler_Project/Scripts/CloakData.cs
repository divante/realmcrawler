using UnityEngine;

namespace RealmCrawler.Equipment
{
    [CreateAssetMenu(fileName = "New Cloak", menuName = "RealmCrawler/Equipment/Cloak Data")]
    public class CloakData : EquipmentData
    {
        [Header("Cloak Stats")]
        [SerializeField] private float healthBonus = 20f;

        public float HealthBonus => healthBonus;
    }
}
