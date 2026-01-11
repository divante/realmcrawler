using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RealmCrawler.UI
{
    public enum InteractableType
    {
        NPC,
        Chest
    }

    [RequireComponent(typeof(Collider))]
    public class InteractableObject : MonoBehaviour
    {
        [Header("Interactable Settings")]
        [SerializeField] private InteractableType interactableType;
        [SerializeField] private float interactionRange = 5f;
        [SerializeField] private LayerMask raycastLayerMask = ~0;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject highlightEffect;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float highlightIntensity = 1.5f;

        private HubStationManager stationManager;
        private bool isHovered = false;
        private Material[] originalMaterials;
        private Renderer objectRenderer;

        private void Awake()
        {
            objectRenderer = GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                originalMaterials = objectRenderer.materials;
            }
        }

        private void Start()
        {
            stationManager = FindFirstObjectByType<HubStationManager>();

            if (stationManager == null)
            {
                Debug.LogError($"InteractableObject on {gameObject.name}: HubStationManager not found in scene!");
            }

            if (highlightEffect != null)
            {
                highlightEffect.SetActive(false);
            }
        }

        private void Update()
        {
            CheckMouseHover();
            CheckMouseClick();
        }

        private void CheckMouseHover()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            bool currentlyHovering = false;

            if (Physics.Raycast(ray, out hit, interactionRange, raycastLayerMask))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        currentlyHovering = true;
                    }
                    else
                    {
                        if (interactableType == InteractableType.Chest)
                        {
                            Debug.Log($"Chest raycast hit something else: {hit.collider.gameObject.name} at distance {hit.distance:F2}");
                        }
                    }
                }
            }
            else
            {
                if (interactableType == InteractableType.Chest && Time.frameCount % 60 == 0)
                {
                    Debug.Log($"Chest raycast missed everything. Range: {interactionRange}");
                }
            }

            if (currentlyHovering && !isHovered)
            {
                OnHoverEnter();
            }
            else if (!currentlyHovering && isHovered)
            {
                OnHoverExit();
            }
        }

        private void CheckMouseClick()
        {
            if (isHovered && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log($"InteractableObject: Click detected on {gameObject.name}. Triggering interaction...");
                OnInteract();
            }
        }

        private void OnHoverEnter()
        {
            isHovered = true;
            Debug.Log($"InteractableObject: Hovering over {gameObject.name} ({interactableType})");

            if (highlightEffect != null)
            {
                highlightEffect.SetActive(true);
            }

            if (objectRenderer != null)
            {
                Material[] materials = objectRenderer.materials;
                foreach (Material mat in materials)
                {
                    mat.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                    mat.EnableKeyword("_EMISSION");
                }
            }

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void OnHoverExit()
        {
            isHovered = false;

            if (highlightEffect != null)
            {
                highlightEffect.SetActive(false);
            }

            if (objectRenderer != null && originalMaterials != null)
            {
                Material[] materials = objectRenderer.materials;
                foreach (Material mat in materials)
                {
                    mat.SetColor("_EmissionColor", Color.black);
                    mat.DisableKeyword("_EMISSION");
                }
            }

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void OnInteract()
        {
            Debug.Log($"InteractableObject: Clicked on {gameObject.name} ({interactableType})");

            if (stationManager == null)
            {
                Debug.LogWarning($"Cannot interact with {gameObject.name}: HubStationManager not found!");
                return;
            }

            Debug.Log($"InteractableObject: Opening {interactableType} station...");

            switch (interactableType)
            {
                case InteractableType.NPC:
                    stationManager.OpenNPCStation();
                    break;

                case InteractableType.Chest:
                    stationManager.OpenChestStation();
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
