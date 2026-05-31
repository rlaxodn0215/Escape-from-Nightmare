// -----------------------------------------------------------------------------
// Codex comment pass: Game Over Panel UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\GameOverPanelUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Game Over Panel UI UI elements, keeping references cached and visuals synchronized.
    public class GameOverPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Text messageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnTitleButton;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private string defaultMessage = "You were caught.";

        // Stores the is Visible value used by this script's runtime or editor workflow.
        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            if (hideOnAwake)
            {
                Hide();
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
                restartButton.onClick.AddListener(HandleRestartClicked);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.RemoveListener(HandleReturnTitleClicked);
                returnTitleButton.onClick.AddListener(HandleReturnTitleClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged -= HandleGameStateChanged;
                GameManager.Instance.StateChanged += HandleGameStateChanged;
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.RemoveListener(HandleReturnTitleClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged -= HandleGameStateChanged;
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Makes the related panel or visual element visible and fills in its current content.
        public void Show()
        {
            SetRootActive(true);

            if (messageText != null)
            {
                messageText.text = defaultMessage;
            }
        }

        // Hides the related panel or visual element and clears transient interaction state.
        public void Hide()
        {
            SetRootActive(false);
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartFromCheckpoint();
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleReturnTitleClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToTitle();
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }
        }

        // Performs the Handle Game State Changed operation while keeping its implementation details inside this script.
        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                Show();
                return;
            }

            Hide();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetRootActive(bool active)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            isVisible = active;
            if (rootObject == gameObject)
            {
                Graphic rootGraphic = GetComponent<Graphic>();
                if (rootGraphic != null)
                {
                    rootGraphic.enabled = active;
                    rootGraphic.raycastTarget = active;
                }

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(active);
                }

                return;
            }

            rootObject.SetActive(active);
        }
    }
}
