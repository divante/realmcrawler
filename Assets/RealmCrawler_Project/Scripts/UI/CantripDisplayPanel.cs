using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using RealmCrawler.Core;
using RealmCrawler.Player;
using RealmCrawler.Spells;

namespace RealmCrawler.UI
{
    public class CantripDisplayPanel : MonoBehaviour
    {
        [Header("Cantrip Icons")]
        [SerializeField] private Image primaryCantripIcon;
        [SerializeField] private Image secondaryCantripIcon;

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI primaryLabel;
        [SerializeField] private TextMeshProUGUI secondaryLabel;

        [Header("Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI tooltipTitleText;
        [SerializeField] private TextMeshProUGUI tooltipDescriptionText;
        [SerializeField] private TextMeshProUGUI tooltipStatsText;

        [Header("Hover Detectors")]
        [SerializeField] private GameObject primaryHoverArea;
        [SerializeField] private GameObject secondaryHoverArea;

        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;

            if (primaryLabel != null)
                primaryLabel.text = "Left Click";

            if (secondaryLabel != null)
                secondaryLabel.text = "Right Click";

            SetupHoverDetection();
            HideTooltip();
            RefreshDisplay();
        }

        private void SetupHoverDetection()
        {
            if (primaryHoverArea != null)
            {
                AddHoverEvents(primaryHoverArea, true);
            }

            if (secondaryHoverArea != null)
            {
                AddHoverEvents(secondaryHoverArea, false);
            }
        }

        private void AddHoverEvents(GameObject hoverArea, bool isPrimary)
        {
            EventTrigger trigger = hoverArea.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = hoverArea.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { OnCantripHoverEnter(isPrimary); });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnCantripHoverExit(); });
            trigger.triggers.Add(exitEntry);
        }

        public void RefreshDisplay()
        {
            if (gameManager?.CurrentLoadout?.weapon == null)
            {
                ClearDisplay();
                return;
            }

            var weapon = gameManager.CurrentLoadout.weapon;

            if (weapon.PrimaryCantrip != null && primaryCantripIcon != null)
            {
                if (weapon.PrimaryCantrip.Icon != null)
                {
                    primaryCantripIcon.sprite = weapon.PrimaryCantrip.Icon;
                    primaryCantripIcon.color = Color.white;
                }
                else
                {
                    primaryCantripIcon.sprite = null;
                    primaryCantripIcon.color = new Color(1, 1, 1, 0.3f);
                    Debug.LogWarning($"Primary Cantrip '{weapon.PrimaryCantrip.CantripName}' has no icon assigned!");
                }
            }
            else if (primaryCantripIcon != null)
            {
                primaryCantripIcon.sprite = null;
                primaryCantripIcon.color = new Color(1, 1, 1, 0.3f);
            }

            if (weapon.SecondaryCantrip != null && secondaryCantripIcon != null)
            {
                if (weapon.SecondaryCantrip.Icon != null)
                {
                    secondaryCantripIcon.sprite = weapon.SecondaryCantrip.Icon;
                    secondaryCantripIcon.color = Color.white;
                }
                else
                {
                    secondaryCantripIcon.sprite = null;
                    secondaryCantripIcon.color = new Color(1, 1, 1, 0.3f);
                    Debug.LogWarning($"Secondary Cantrip '{weapon.SecondaryCantrip.CantripName}' has no icon assigned!");
                }
            }
            else if (secondaryCantripIcon != null)
            {
                secondaryCantripIcon.sprite = null;
                secondaryCantripIcon.color = new Color(1, 1, 1, 0.3f);
            }
        }

        private void ClearDisplay()
        {
            if (primaryCantripIcon != null)
            {
                primaryCantripIcon.sprite = null;
                primaryCantripIcon.color = new Color(1, 1, 1, 0.3f);
            }

            if (secondaryCantripIcon != null)
            {
                secondaryCantripIcon.sprite = null;
                secondaryCantripIcon.color = new Color(1, 1, 1, 0.3f);
            }
        }

        private void OnCantripHoverEnter(bool isPrimary)
        {
            if (gameManager?.CurrentLoadout?.weapon == null)
                return;

            var weapon = gameManager.CurrentLoadout.weapon;
            CantripData cantrip = isPrimary ? weapon.PrimaryCantrip : weapon.SecondaryCantrip;

            if (cantrip == null)
                return;

            ShowTooltip(cantrip);
        }

        private void OnCantripHoverExit()
        {
            HideTooltip();
        }

        private void ShowTooltip(CantripData cantrip)
        {
            if (tooltipPanel != null)
                tooltipPanel.SetActive(true);

            if (tooltipTitleText != null)
                tooltipTitleText.text = cantrip.CantripName;

            if (tooltipDescriptionText != null)
                tooltipDescriptionText.text = cantrip.Description;

            if (tooltipStatsText != null)
            {
                string stats = $"Damage: {cantrip.BaseDamage}\n";
                stats += $"Attack Speed: {cantrip.AttackSpeed}s\n";
                stats += $"Projectile Speed: {cantrip.ProjectileSpeed}";
                tooltipStatsText.text = stats;
            }
        }

        private void HideTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }
    }
}
