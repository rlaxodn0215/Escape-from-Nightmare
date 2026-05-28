using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class GhostStatusUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Text stateText;
        [SerializeField] private Text dangerText;
        [SerializeField] private Text chaseText;
        [SerializeField] private Text hideText;
        [SerializeField] private bool hideWhenInactive = false;

        private void Awake()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            Refresh();
        }

        private void Reset()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        public void Refresh()
        {
            GhostManager ghostManager = GhostManager.Instance;
            ChaseManager chaseManager = ChaseManager.Instance;
            HideManager hideManager = HideManager.Instance;

            if (stateText != null)
            {
                if (ghostManager != null)
                {
                    stateText.text = "Ghost: " + ghostManager.RuntimeState + " / " + EmptyToNA(ghostManager.CurrentGhostLocationId);
                }
                else
                {
                    stateText.text = "Ghost: N/A";
                }
            }

            if (dangerText != null)
            {
                if (ghostManager != null)
                {
                    dangerText.text = "Danger: " + ghostManager.DangerLevel.ToString("0.00") + " / " + ghostManager.DangerThreshold.ToString("0.00");
                }
                else
                {
                    dangerText.text = "Danger: N/A";
                }
            }

            if (chaseText != null)
            {
                if (chaseManager != null)
                {
                    chaseText.text = "Chase: " + chaseManager.IsChasing + " / Moves " + chaseManager.MoveCountDuringChase + " / " + chaseManager.MaxMovesBeforeCatch;
                }
                else
                {
                    chaseText.text = "Chase: N/A";
                }
            }

            if (hideText != null)
            {
                if (hideManager != null)
                {
                    hideText.text = "Hide: " + hideManager.IsHiding + " / Safe " + hideManager.CanExitSafely;
                }
                else
                {
                    hideText.text = "Hide: N/A";
                }
            }

            if (hideWhenInactive && rootObject != null && rootObject != gameObject)
            {
                bool ghostInactive = ghostManager == null || ghostManager.RuntimeState == GhostRuntimeState.Inactive;
                bool chaseInactive = chaseManager == null || !chaseManager.IsChasing;
                bool hideInactive = hideManager == null || !hideManager.IsHiding;
                rootObject.SetActive(!(ghostInactive && chaseInactive && hideInactive));
            }
        }

        private void Subscribe()
        {
            // Update refresh is used for this lightweight debug UI.
        }

        private void Unsubscribe()
        {
            // Reserved for future event-driven UI updates.
        }

        private string EmptyToNA(string value)
        {
            return string.IsNullOrEmpty(value) ? "N/A" : value;
        }
    }
}
