using UnityEngine;
using RealmCrawler.Equipment;

namespace RealmCrawler.Player
{
    public class PlayerVisualEquipment : MonoBehaviour
    {
        [Header("Equipment Slots")]
        [SerializeField] private Transform hatSlot;
        [SerializeField] private Transform cloakSlot;
        [SerializeField] private Transform bootsSlot;
        [SerializeField] private Transform weaponSlot;
        [SerializeField] private Transform reliquarySlot;

        private GameObject currentHatVisual;
        private GameObject currentCloakVisual;
        private GameObject currentBootsVisual;
        private GameObject currentWeaponVisual;
        private GameObject currentReliquaryVisual;

        public void EquipWeapon(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                ClearWeapon();
                return;
            }

            ClearWeapon();

            if (weaponData.WeaponVisualPrefab != null && weaponSlot != null)
            {
                currentWeaponVisual = Instantiate(weaponData.WeaponVisualPrefab, weaponSlot);
            }
        }

        public void EquipHat(HatData hatData)
        {
            if (hatData == null)
            {
                ClearHat();
                return;
            }

            ClearHat();

            if (hatData.VisualPrefab != null && hatSlot != null)
            {
                currentHatVisual = Instantiate(hatData.VisualPrefab, hatSlot);
            }
        }

        public void EquipCloak(CloakData cloakData)
        {
            if (cloakData == null)
            {
                ClearCloak();
                return;
            }

            ClearCloak();

            if (cloakData.VisualPrefab != null && cloakSlot != null)
            {
                currentCloakVisual = Instantiate(cloakData.VisualPrefab, cloakSlot);
            }
        }

        public void EquipBoots(BootsData bootsData)
        {
            if (bootsData == null)
            {
                ClearBoots();
                return;
            }

            ClearBoots();

            if (bootsData.VisualPrefab != null && bootsSlot != null)
            {
                currentBootsVisual = Instantiate(bootsData.VisualPrefab, bootsSlot);
            }
        }

        public void EquipReliquary(ReliquaryData reliquaryData)
        {
            if (reliquaryData == null)
            {
                ClearReliquary();
                return;
            }

            ClearReliquary();

            if (reliquaryData.VisualPrefab != null && reliquarySlot != null)
            {
                currentReliquaryVisual = Instantiate(reliquaryData.VisualPrefab, reliquarySlot);
            }
        }

        public void ClearAllEquipment()
        {
            ClearWeapon();
            ClearHat();
            ClearCloak();
            ClearBoots();
            ClearReliquary();
        }

        private void ClearWeapon()
        {
            if (currentWeaponVisual != null)
            {
                Destroy(currentWeaponVisual);
                currentWeaponVisual = null;
            }
        }

        private void ClearHat()
        {
            if (currentHatVisual != null)
            {
                Destroy(currentHatVisual);
                currentHatVisual = null;
            }
        }

        private void ClearCloak()
        {
            if (currentCloakVisual != null)
            {
                Destroy(currentCloakVisual);
                currentCloakVisual = null;
            }
        }

        private void ClearBoots()
        {
            if (currentBootsVisual != null)
            {
                Destroy(currentBootsVisual);
                currentBootsVisual = null;
            }
        }

        private void ClearReliquary()
        {
            if (currentReliquaryVisual != null)
            {
                Destroy(currentReliquaryVisual);
                currentReliquaryVisual = null;
            }
        }

        private void OnDestroy()
        {
            ClearAllEquipment();
        }
    }
}
