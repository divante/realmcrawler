using UnityEngine;
using System.Collections;

namespace RealmCrawler.UI
{
    public class LoadoutCameraManager : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera loadoutCamera;

        [Header("Camera View Transforms")]
        [SerializeField] private Transform defaultView;
        [SerializeField] private Transform hatView;
        [SerializeField] private Transform cloakView;
        [SerializeField] private Transform bootView;
        [SerializeField] private Transform relicView;
        [SerializeField] private Transform weaponView;

        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 1.0f;
        [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private Transform currentTargetView;
        private Coroutine transitionCoroutine;

        private void Awake()
        {
            if (loadoutCamera == null)
            {
                loadoutCamera = Camera.main;
            }
        }

        private void Start()
        {
            if (defaultView != null)
            {
                SetCameraToView(defaultView, immediate: true);
                currentTargetView = defaultView;
            }
        }

        public void MoveToDefaultView()
        {
            MoveToView(defaultView, "Default");
        }

        public void MoveToHatView()
        {
            MoveToView(hatView, "Hat");
        }

        public void MoveToCloakView()
        {
            MoveToView(cloakView, "Cloak");
        }

        public void MoveToBootView()
        {
            MoveToView(bootView, "Boot");
        }

        public void MoveToRelicView()
        {
            MoveToView(relicView, "Relic");
        }

        public void MoveToWeaponView()
        {
            MoveToView(weaponView, "Weapon");
        }

        private void MoveToView(Transform targetView, string viewName)
        {
            if (targetView == null)
            {
                Debug.LogWarning($"LoadoutCameraManager: {viewName} view transform is not assigned.");
                return;
            }

            if (currentTargetView == targetView)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"LoadoutCameraManager: Already at {viewName} view.");
                }
                return;
            }

            currentTargetView = targetView;

            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }

            transitionCoroutine = StartCoroutine(TransitionToView(targetView, viewName));
        }

        private IEnumerator TransitionToView(Transform targetView, string viewName)
        {
            if (loadoutCamera == null || targetView == null)
            {
                yield break;
            }

            Vector3 startPosition = loadoutCamera.transform.position;
            Quaternion startRotation = loadoutCamera.transform.rotation;

            Vector3 endPosition = targetView.position;
            Quaternion endRotation = targetView.rotation;

            float elapsedTime = 0f;

            if (showDebugLogs)
            {
                Debug.Log($"LoadoutCameraManager: Transitioning to {viewName} view.");
            }

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / transitionDuration);
                float easedT = easingCurve.Evaluate(t);

                loadoutCamera.transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
                loadoutCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, easedT);

                yield return null;
            }

            loadoutCamera.transform.position = endPosition;
            loadoutCamera.transform.rotation = endRotation;

            if (showDebugLogs)
            {
                Debug.Log($"LoadoutCameraManager: Arrived at {viewName} view.");
            }

            transitionCoroutine = null;
        }

        private void SetCameraToView(Transform targetView, bool immediate)
        {
            if (loadoutCamera == null || targetView == null)
            {
                return;
            }

            loadoutCamera.transform.position = targetView.position;
            loadoutCamera.transform.rotation = targetView.rotation;
        }
    }
}
