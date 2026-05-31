// -----------------------------------------------------------------------------
// Codex comment pass: Title Menu UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\TitleMenuUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Title Menu UI UI elements, keeping references cached and visuals synchronized.
    public class TitleMenuUI : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button deleteSaveButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Text statusText;
        [SerializeField] private bool refreshOnEnable = true;
        [SerializeField] private string noSaveMessage = "No save data found.";
        [SerializeField] private string saveDeletedMessage = "Save data deleted.";

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            if (newGameButton != null)
            {
                newGameButton.onClick.RemoveListener(HandleNewGameClicked);
                newGameButton.onClick.AddListener(HandleNewGameClicked);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueClicked);
                continueButton.onClick.AddListener(HandleContinueClicked);
            }

            if (deleteSaveButton != null)
            {
                deleteSaveButton.onClick.RemoveListener(HandleDeleteSaveClicked);
                deleteSaveButton.onClick.AddListener(HandleDeleteSaveClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(HandleQuitClicked);
                quitButton.onClick.AddListener(HandleQuitClicked);
            }

            if (refreshOnEnable)
            {
                Refresh();
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (newGameButton != null)
            {
                newGameButton.onClick.RemoveListener(HandleNewGameClicked);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueClicked);
            }

            if (deleteSaveButton != null)
            {
                deleteSaveButton.onClick.RemoveListener(HandleDeleteSaveClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(HandleQuitClicked);
            }
        }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            Refresh();
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        public void Refresh()
        {
            bool hasSaveData = HasSaveData();

            if (continueButton != null)
            {
                continueButton.interactable = hasSaveData;
            }

            if (deleteSaveButton != null)
            {
                deleteSaveButton.interactable = hasSaveData;
            }

            SetStatus(string.Empty);
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleNewGameClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiClick);
            }

            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager instance is missing.");
                return;
            }

            GameManager.Instance.StartNewGame();
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleContinueClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiClick);
            }

            if (!HasSaveData())
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayUi(AudioCue.UiFail);
                }

                SetStatus(noSaveMessage);
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager instance is missing.");
                return;
            }

            if (!GameManager.Instance.ContinueGame())
            {
                SetStatus(noSaveMessage);
            }
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleDeleteSaveClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiClick);
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.DeleteSave();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            Refresh();
            SetStatus(saveDeletedMessage);
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleQuitClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiClick);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
                return;
            }

            Debug.Log("QuitGame requested.");
            Application.Quit();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool HasSaveData()
        {
            return SaveManager.Instance != null && SaveManager.Instance.HasSaveData();
        }
    }
}
