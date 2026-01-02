using UnityEngine;

namespace RealmCrawler.Equipment
{
    [CreateAssetMenu(fileName = "New Boots", menuName = "RealmCrawler/Equipment/Boots Data")]
    public class BootsData : EquipmentData
    {
        [Header("Boots Stats")]
        [SerializeField] private float speedMultiplier = 1.2f;

        public float SpeedMultiplier => speedMultiplier;
    }
}
