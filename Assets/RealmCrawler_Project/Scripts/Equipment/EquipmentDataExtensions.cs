using UnityEngine;

namespace RealmCrawler.Equipment
{
    public static class EquipmentDataExtensions
    {
        [System.Obsolete("Use ItemName instead")]
        public static string EquipmentName(this EquipmentData equipment)
        {
            return equipment.ItemName;
        }

        [System.Obsolete("Use ItemIcon instead")]
        public static Sprite Icon(this EquipmentData equipment)
        {
            return equipment.ItemIcon;
        }

        [System.Obsolete("Use FlavorText instead")]
        public static string Description(this EquipmentData equipment)
        {
            return equipment.FlavorText;
        }

        [System.Obsolete("Use ItemName instead")]
        public static string WeaponName(this WeaponData weapon)
        {
            return weapon.ItemName;
        }

        [System.Obsolete("Use ItemIcon instead")]
        public static Sprite Icon(this WeaponData weapon)
        {
            return weapon.ItemIcon;
        }

        [System.Obsolete("Use FlavorText instead")]
        public static string Description(this WeaponData weapon)
        {
            return weapon.FlavorText;
        }
    }
}
