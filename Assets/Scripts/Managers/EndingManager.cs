// -----------------------------------------------------------------------------
// Codex comment pass: Ending Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\EndingManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Runtime owner for the Ending Manager system, keeping shared state and events behind one access point.
    public class EndingManager : Singleton<EndingManager>
    {
        [SerializeField] private EndingPanelUI endingPanel;
        [SerializeField] private float returnToTitleDelaySeconds = 3f;
        [SerializeField] private bool returnToTitleAfterEnding = true;
        [SerializeField] private string endingTitle = "Ending";
        [SerializeField] private string endingMessage = "You escaped from the nightmare.";

        // Stores the ending Routine value used by this script's runtime or editor workflow.
        private Coroutine endingRoutine;

        public bool ReturnToTitleAfterEnding
        {
            get { return returnToTitleAfterEnding; }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetReturnToTitleAfterEnding(bool value)
        {
            returnToTitleAfterEnding = value;
        }

        // Stops any active ending overlay or return-to-title countdown for editor manual play reset flows.
        public void StopEnding()
        {
            if (endingRoutine != null)
            {
                StopCoroutine(endingRoutine);
                endingRoutine = null;
            }

            if (endingPanel != null)
            {
                endingPanel.Hide();
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            SubscribePanel();
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            UnsubscribePanel();
        }

        // Performs the Play Ending operation while keeping its implementation details inside this script.
        public void PlayEnding()
        {
            if (endingRoutine != null)
            {
                StopCoroutine(endingRoutine);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Ending);
            }

            endingRoutine = StartCoroutine(EndingRoutine());
        }

        // Performs the Ending Routine operation while keeping its implementation details inside this script.
        public IEnumerator EndingRoutine()
        {
            bool shouldReturnToTitle = returnToTitleAfterEnding;

            if (endingPanel != null)
            {
                endingPanel.Show(endingTitle, endingMessage);
            }
            else
            {
                Debug.LogWarning("EndingPanelUI is not assigned.");
            }

            yield return new WaitForSeconds(Mathf.Max(0f, returnToTitleDelaySeconds));

            if (shouldReturnToTitle)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ReturnToTitle();
                }
                else
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }

            endingRoutine = null;
        }

        // Performs the Skip Ending operation while keeping its implementation details inside this script.
        public void SkipEnding()
        {
            if (endingRoutine != null)
            {
                StopCoroutine(endingRoutine);
                endingRoutine = null;
            }

            if (endingPanel != null)
            {
                endingPanel.Hide();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToTitle();
            }
            else
            {
                SceneManager.LoadScene("TitleScene");
            }
        }

        // Performs the Subscribe Panel operation while keeping its implementation details inside this script.
        private void SubscribePanel()
        {
            if (endingPanel != null)
            {
                endingPanel.SkipRequested -= SkipEnding;
                endingPanel.SkipRequested += SkipEnding;
            }
        }

        // Performs the Unsubscribe Panel operation while keeping its implementation details inside this script.
        private void UnsubscribePanel()
        {
            if (endingPanel != null)
            {
                endingPanel.SkipRequested -= SkipEnding;
            }
        }
    }
}
