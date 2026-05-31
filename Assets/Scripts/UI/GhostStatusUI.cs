// -----------------------------------------------------------------------------
// Codex comment pass: Ghost Status UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\GhostStatusUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Ghost Status UI UI elements, keeping references cached and visuals synchronized.
    public class GhostStatusUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Text stateText;
        [SerializeField] private Text dangerText;
        [SerializeField] private Text chaseText;
        [SerializeField] private Text hideText;
        [SerializeField] private bool hideWhenInactive = false;

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            Unsubscribe();
        }

        // Refreshes frame-dependent input, timers, animation, or visual state.
        private void Update()
        {
            Refresh();
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
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

        // Performs the Subscribe operation while keeping its implementation details inside this script.
        private void Subscribe()
        {
            // Update refresh is used for this lightweight debug UI.
        }

        // Performs the Unsubscribe operation while keeping its implementation details inside this script.
        private void Unsubscribe()
        {
            // Reserved for future event-driven UI updates.
        }

        // Performs the Empty To NA operation while keeping its implementation details inside this script.
        private string EmptyToNA(string value)
        {
            return string.IsNullOrEmpty(value) ? "N/A" : value;
        }
    }
}
