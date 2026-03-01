using UnityEngine;
using System.Collections;

namespace RealmCrawler.UI
{
    public class ChestLidAnimator : MonoBehaviour
    {
        [Header("Lid Settings")]
        [SerializeField] private Transform lidHinge;
        [SerializeField] private float openAngle = 45f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Axis")]
        [SerializeField] private Vector3 rotationAxis = Vector3.right;

        private Quaternion closedRotation;
        private Quaternion openRotation;
        private Coroutine animationCoroutine;
        private HubStationManager stationManager;
        private bool isOpen = false;

        private void Awake()
        {
            if (lidHinge == null)
            {
                Transform hingeChild = transform.Find("Chest_Lid_Hinge");
                if (hingeChild != null)
                {
                    lidHinge = hingeChild;
                }
                else
                {
                    Debug.LogError($"ChestLidAnimator on {gameObject.name}: Lid hinge not assigned and 'Chest_Lid_Hinge' not found!");
                    enabled = false;
                    return;
                }
            }

            closedRotation = lidHinge.localRotation;
            openRotation = closedRotation * Quaternion.AngleAxis(openAngle, rotationAxis);
        }

        private void Start()
        {
            stationManager = FindFirstObjectByType<HubStationManager>();

            if (stationManager == null)
            {
                Debug.LogError($"ChestLidAnimator on {gameObject.name}: HubStationManager not found in scene!");
            }
        }

        private void Update()
        {
            if (stationManager == null) return;

            HubStation currentStation = GetCurrentStation(stationManager);

            if (currentStation == HubStation.Chest && !isOpen)
            {
                OpenLid();
            }
            else if (currentStation != HubStation.Chest && isOpen)
            {
                CloseLid();
            }
        }

        public void OpenLid()
        {
            if (isOpen) return;
            isOpen = true;
            AnimateLid(openRotation);
        }

        public void CloseLid()
        {
            if (!isOpen) return;
            isOpen = false;
            AnimateLid(closedRotation);
        }

        private void AnimateLid(Quaternion targetRotation)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(AnimateLidCoroutine(targetRotation));
        }

        private IEnumerator AnimateLidCoroutine(Quaternion targetRotation)
        {
            Quaternion startRotation = lidHinge.localRotation;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / animationDuration);
                float easedT = openCurve.Evaluate(t);

                lidHinge.localRotation = Quaternion.Slerp(startRotation, targetRotation, easedT);

                yield return null;
            }

            lidHinge.localRotation = targetRotation;
            animationCoroutine = null;
        }

        private HubStation GetCurrentStation(HubStationManager manager)
        {
            var field = manager.GetType().GetField("currentStation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                return (HubStation)field.GetValue(manager);
            }
            return HubStation.MainHub;
        }
    }
}
