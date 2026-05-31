// -----------------------------------------------------------------------------
// Codex comment pass: Clue Image Panel UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\ClueImagePanelUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Clue Image Panel UI UI elements, keeping references cached and visuals synchronized.
    public class ClueImagePanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Image clueImage;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button closeButton;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private string defaultLockedTitle = "Locked";
        [SerializeField] private string defaultLockedMessage = "You cannot examine this yet.";

        public bool IsVisible
        {
            get
            {
                GameObject target = rootObject != null ? rootObject : gameObject;
                return target.activeSelf;
            }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            EnsureRootObject();

            if (hideOnAwake)
            {
                Hide();
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
                closeButton.onClick.AddListener(HandleCloseClicked);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            EnsureRootObject();

            if (closeButton == null)
            {
                closeButton = GetComponentInChildren<Button>(true);
            }
        }

        // Makes the related panel or visual element visible and fills in its current content.
        public void ShowClue(ClueRecord record, Sprite sprite)
        {
            if (record == null)
            {
                ShowMessageOnly("Missing Clue", "Clue data was not found.");
                return;
            }

            SetRootActive(true);

            if (clueImage != null)
            {
                clueImage.sprite = sprite;
                clueImage.enabled = sprite != null;
            }

            if (titleText != null)
            {
                titleText.text = GetDisplayTitle(record);
            }

            if (descriptionText != null)
            {
                descriptionText.text = GetDescription(record);
            }

            if (messageText != null)
            {
                messageText.text = string.Empty;
            }
        }

        // Makes the related panel or visual element visible and fills in its current content.
        public void ShowLockedMessage(string message)
        {
            SetRootActive(true);

            if (clueImage != null)
            {
                clueImage.sprite = null;
                clueImage.enabled = false;
            }

            if (titleText != null)
            {
                titleText.text = defaultLockedTitle;
            }

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (messageText != null)
            {
                messageText.text = !string.IsNullOrEmpty(message) ? message : defaultLockedMessage;
            }
        }

        // Makes the related panel or visual element visible and fills in its current content.
        public void ShowMessageOnly(string title, string message)
        {
            SetRootActive(true);

            if (clueImage != null)
            {
                clueImage.sprite = null;
                clueImage.enabled = false;
            }

            if (titleText != null)
            {
                titleText.text = title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        // Hides the related panel or visual element and clears transient interaction state.
        public void Hide()
        {
            if (clueImage != null)
            {
                clueImage.sprite = null;
                clueImage.enabled = false;
            }

            if (titleText != null)
            {
                titleText.text = string.Empty;
            }

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (messageText != null)
            {
                messageText.text = string.Empty;
            }

            SetRootActive(false);
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleCloseClicked()
        {
            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.HideCurrentImage();
                return;
            }

            Hide();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetRootActive(bool active)
        {
            EnsureRootObject();
            rootObject.SetActive(active);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private string GetDisplayTitle(ClueRecord record)
        {
            if (record == null)
            {
                return string.Empty;
            }

            return !string.IsNullOrEmpty(record.displayName) ? record.displayName : record.clueId;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private string GetDescription(ClueRecord record)
        {
            return record != null && !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private void EnsureRootObject()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }
    }
}
