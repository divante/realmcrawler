using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RealmCrawler.Equipment;

namespace RealmCrawler.Core
{
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "RealmCrawler/Core/Equipment Database")]
    public class EquipmentDatabase : ScriptableObject
    {
        [Header("Available Equipment")]
        [SerializeField] private List<HatData> availableHats = new List<HatData>();
        [SerializeField] private List<CloakData> availableCloaks = new List<CloakData>();
        [SerializeField] private List<BootsData> availableBoots = new List<BootsData>();
        [SerializeField] private List<WeaponData> availableWeapons = new List<WeaponData>();
        [SerializeField] private List<ReliquaryData> availableReliquaries = new List<ReliquaryData>();

        [Header("Default Equipment (Starting Loadout)")]
        [SerializeField] private HatData defaultHat;
        [SerializeField] private CloakData defaultCloak;
        [SerializeField] private BootsData defaultBoots;
        [SerializeField] private WeaponData defaultWeapon;
        [SerializeField] private ReliquaryData defaultReliquary;

        public List<HatData> GetAllHats() => availableHats;
        public List<CloakData> GetAllCloaks() => availableCloaks;
        public List<BootsData> GetAllBoots() => availableBoots;
        public List<WeaponData> GetAllWeapons() => availableWeapons;
        public List<ReliquaryData> GetAllReliquaries() => availableReliquaries;

        public HatData GetDefaultHat() => defaultHat;
        public CloakData GetDefaultCloak() => defaultCloak;
        public BootsData GetDefaultBoots() => defaultBoots;
        public WeaponData GetDefaultWeapon() => defaultWeapon;
        public ReliquaryData GetDefaultReliquary() => defaultReliquary;

        public List<HatData> GetAffordableHats(int souls)
        {
            return availableHats.Where(h => h != null && h.RentalCost <= souls).ToList();
        }

        public List<CloakData> GetAffordableCloaks(int souls)
        {
            return availableCloaks.Where(c => c != null && c.RentalCost <= souls).ToList();
        }

        public List<BootsData> GetAffordableBoots(int souls)
        {
            return availableBoots.Where(b => b != null && b.RentalCost <= souls).ToList();
        }

        public List<WeaponData> GetAffordableWeapons(int souls)
        {
            return availableWeapons.Where(w => w != null && w.RentalCost <= souls).ToList();
        }

        public List<ReliquaryData> GetAffordableReliquaries(int souls)
        {
            return availableReliquaries.Where(r => r != null && r.RentalCost <= souls).ToList();
        }

        public int GetTotalEquipmentCount()
        {
            return availableHats.Count + availableCloaks.Count + availableBoots.Count +
                   availableWeapons.Count + availableReliquaries.Count;
        }

        void OnValidate()
        {
            if (defaultHat == null && availableHats.Count > 0)
                defaultHat = availableHats[0];
            
            if (defaultCloak == null && availableCloaks.Count > 0)
                defaultCloak = availableCloaks[0];
            
            if (defaultBoots == null && availableBoots.Count > 0)
                defaultBoots = availableBoots[0];
            
            if (defaultWeapon == null && availableWeapons.Count > 0)
                defaultWeapon = availableWeapons[0];
            
            if (defaultReliquary == null && availableReliquaries.Count > 0)
                defaultReliquary = availableReliquaries[0];
        }
    }
}
