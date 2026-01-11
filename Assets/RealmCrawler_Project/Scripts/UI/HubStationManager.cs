using UnityEngine;
using UnityEngine.UIElements;

namespace RealmCrawler.UI
{
    public enum HubStation
    {
        MainHub,
        NPC,
        Chest
    }

    public class HubStationManager : MonoBehaviour
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument mainHubUI;
        [SerializeField] private UIDocument npcUI;
        [SerializeField] private UIDocument chestUI;

        [Header("Camera")]
        [SerializeField] private Camera hubCamera;
        [SerializeField] private Transform mainHubCameraTransform;
        [SerializeField] private Transform npcCameraTransform;
        [SerializeField] private Transform chestCameraTransform;
        [SerializeField] private float cameraTransitionDuration = 1.0f;
        [SerializeField] private AnimationCurve cameraEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("3D Interactables")]
        [SerializeField] private GameObject npcObject;
        [SerializeField] private GameObject chestObject;

        private HubStation currentStation = HubStation.MainHub;
        private Coroutine cameraTransitionCoroutine;

        private void Awake()
        {
            if (hubCamera == null)
            {
                hubCamera = Camera.main;
            }
        }

        private void Start()
        {
            SwitchToStation(HubStation.MainHub, immediate: true);
        }

        public void SwitchToStation(HubStation station, bool immediate = false)
        {
            Debug.Log($"HubStationManager: SwitchToStation called - Target: {station}, Current: {currentStation}, Immediate: {immediate}");

            if (currentStation == station && !immediate)
            {
                Debug.Log($"HubStationManager: Already at {station}, skipping.");
                return;
            }

            currentStation = station;

            HideAllUIs();

            switch (station)
            {
                case HubStation.MainHub:
                    Debug.Log("HubStationManager: Switching to Main Hub");
                    ShowUI(mainHubUI);
                    MoveCameraTo(mainHubCameraTransform, immediate);
                    break;

                case HubStation.NPC:
                    Debug.Log("HubStationManager: Switching to NPC Station");
                    ShowUI(npcUI);
                    MoveCameraTo(npcCameraTransform, immediate);
                    break;

                case HubStation.Chest:
                    Debug.Log("HubStationManager: Switching to Chest Station");
                    ShowUI(chestUI);
                    MoveCameraTo(chestCameraTransform, immediate);
                    break;
            }
        }

        public void ReturnToMainHub()
        {
            SwitchToStation(HubStation.MainHub);
        }

        public void OpenNPCStation()
        {
            SwitchToStation(HubStation.NPC);
        }

        public void OpenChestStation()
        {
            SwitchToStation(HubStation.Chest);
        }

        private void HideAllUIs()
        {
            if (mainHubUI != null) mainHubUI.rootVisualElement.style.display = DisplayStyle.None;
            if (npcUI != null) npcUI.rootVisualElement.style.display = DisplayStyle.None;
            if (chestUI != null) chestUI.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void ShowUI(UIDocument uiDocument)
        {
            if (uiDocument != null)
            {
                uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            }
        }

        private void Show3DObjects(bool show)
        {
            if (npcObject != null) npcObject.SetActive(show);
            if (chestObject != null) chestObject.SetActive(show);
        }

        private void MoveCameraTo(Transform targetTransform, bool immediate)
        {
            if (hubCamera == null || targetTransform == null)
            {
                return;
            }

            if (cameraTransitionCoroutine != null)
            {
                StopCoroutine(cameraTransitionCoroutine);
            }

            if (immediate)
            {
                hubCamera.transform.position = targetTransform.position;
                hubCamera.transform.rotation = targetTransform.rotation;
            }
            else
            {
                cameraTransitionCoroutine = StartCoroutine(TransitionCamera(targetTransform));
            }
        }

        private System.Collections.IEnumerator TransitionCamera(Transform targetTransform)
        {
            Vector3 startPosition = hubCamera.transform.position;
            Quaternion startRotation = hubCamera.transform.rotation;

            Vector3 endPosition = targetTransform.position;
            Quaternion endRotation = targetTransform.rotation;

            float elapsedTime = 0f;

            while (elapsedTime < cameraTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / cameraTransitionDuration);
                float easedT = cameraEasing.Evaluate(t);

                hubCamera.transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
                hubCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, easedT);

                yield return null;
            }

            hubCamera.transform.position = endPosition;
            hubCamera.transform.rotation = endRotation;

            cameraTransitionCoroutine = null;
        }
    }
}
