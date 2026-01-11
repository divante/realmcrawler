using UnityEngine;
using RealmCrawler.Equipment;
using RealmCrawler.Core;

namespace RealmCrawler.Player
{
    public class PlayerVisualEquipment : MonoBehaviour
    {
        [Header("Equipment Slots")]
        [SerializeField] private Transform hatSlot;
        [SerializeField] private Transform cloakSlot;
        [SerializeField] private Transform leftBootSlot;
        [SerializeField] private Transform rightBootSlot;
        [SerializeField] private Transform weaponSlot;
        [SerializeField] private Transform reliquarySlot;

        private GameObject currentHatVisual;
        private GameObject currentCloakVisual;
        private GameObject currentLeftBootVisual;
        private GameObject currentRightBootVisual;
        private GameObject currentWeaponVisual;
        private GameObject currentReliquaryVisual;

        public void ApplyLoadout(PlayerLoadout loadout)
        {
            if (loadout == null)
            {
                Debug.LogWarning("Loadout is null. Cannot apply equipment.");
                return;
            }

            EquipHat(loadout.hat);
            EquipCloak(loadout.cloak);
            EquipBoots(loadout.boots);
            EquipWeapon(loadout.weapon);
            EquipReliquary(loadout.reliquary);

            Debug.Log("Applied equipment loadout to player visuals.");
        }

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
                currentWeaponVisual.transform.localPosition = Vector3.zero;
                currentWeaponVisual.transform.localRotation = Quaternion.identity;
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
                currentHatVisual.transform.localPosition = Vector3.zero;
                currentHatVisual.transform.localRotation = Quaternion.identity;
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
                currentCloakVisual.transform.localPosition = Vector3.zero;
                currentCloakVisual.transform.localRotation = Quaternion.identity;
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

            if (bootsData.VisualPrefab != null)
            {
                GameObject bootsPrefab = bootsData.VisualPrefab;

                Transform leftBootMesh = FindChildRecursive(bootsPrefab.transform, "Left");
                Transform rightBootMesh = FindChildRecursive(bootsPrefab.transform, "Right");

                if (leftBootMesh != null && leftBootSlot != null)
                {
                    currentLeftBootVisual = Instantiate(bootsPrefab, leftBootSlot);
                    currentLeftBootVisual.transform.localPosition = Vector3.zero;
                    currentLeftBootVisual.transform.localRotation = Quaternion.identity;

                    Transform rightInLeft = FindChildRecursive(currentLeftBootVisual.transform, "Right");
                    if (rightInLeft != null)
                    {
                        rightInLeft.gameObject.SetActive(false);
                    }
                }

                if (rightBootMesh != null && rightBootSlot != null)
                {
                    currentRightBootVisual = Instantiate(bootsPrefab, rightBootSlot);
                    currentRightBootVisual.transform.localPosition = Vector3.zero;
                    currentRightBootVisual.transform.localRotation = Quaternion.identity;

                    Transform leftInRight = FindChildRecursive(currentRightBootVisual.transform, "Left");
                    if (leftInRight != null)
                    {
                        leftInRight.gameObject.SetActive(false);
                    }
                }
            }
        }

        private Transform FindChildRecursive(Transform parent, string nameContains)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Contains(nameContains))
                {
                    return child;
                }

                Transform result = FindChildRecursive(child, nameContains);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
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
                currentReliquaryVisual.transform.localPosition = Vector3.zero;
                currentReliquaryVisual.transform.localRotation = Quaternion.identity;
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
            if (currentLeftBootVisual != null)
            {
                Destroy(currentLeftBootVisual);
                currentLeftBootVisual = null;
            }

            if (currentRightBootVisual != null)
            {
                Destroy(currentRightBootVisual);
                currentRightBootVisual = null;
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
