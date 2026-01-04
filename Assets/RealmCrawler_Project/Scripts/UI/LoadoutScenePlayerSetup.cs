using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RealmCrawler.UI
{
    public class LoadoutScenePlayerSetup : MonoBehaviour
    {
        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "Scene_Loadout")
            {
                DisablePlayerControls();
                DisablePlayerPhysics();
            }
        }

        private void DisablePlayerControls()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
                Debug.Log("Disabled PlayerInput in loadout scene.");
            }

            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
                Debug.Log("Disabled PlayerController in loadout scene.");
            }

            CharacterPhysics characterPhysics = GetComponent<CharacterPhysics>();
            if (characterPhysics != null)
            {
                characterPhysics.enabled = false;
                Debug.Log("Disabled CharacterPhysics in loadout scene.");
            }

            CharacterAnimationController animController = GetComponentInChildren<CharacterAnimationController>();
            if (animController != null)
            {
                animController.enabled = false;
                Debug.Log("Disabled CharacterAnimationController in loadout scene.");
            }
        }

        private void DisablePlayerPhysics()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                Debug.Log("Set Rigidbody to kinematic in loadout scene.");
            }
        }
    }
}
