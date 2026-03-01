using UnityEngine;
using UnityEngine.InputSystem;

namespace RealmCrawler.Core
{
    public class PlayScenePlayerSetup : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool logSetup = true;

        private void Start()
        {
            EnablePlayerControls();
            EnablePlayerPhysics();
        }

        private void EnablePlayerControls()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = true;
                if (logSetup) Debug.Log("✅ Enabled PlayerInput in play scene.");
            }

            ICharacterController playerController = GetComponent<ICharacterController>();
            if (playerController != null)
            {
                playerController.Enable();
                if (logSetup) Debug.Log("✅ Enabled PlayerController in play scene.");
            }

            CharacterPhysics characterPhysics = GetComponent<CharacterPhysics>();
            if (characterPhysics != null)
            {
                characterPhysics.enabled = true;
                if (logSetup) Debug.Log("✅ Enabled CharacterPhysics in play scene.");
            }

            CharacterAnimationController animController = GetComponentInChildren<CharacterAnimationController>();
            if (animController != null)
            {
                animController.enabled = true;
                if (logSetup) Debug.Log("✅ Enabled CharacterAnimationController in play scene.");
            }
        }

        private void EnablePlayerPhysics()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                if (logSetup) Debug.Log("✅ Enabled Rigidbody physics in play scene.");
            }
        }
    }
}
