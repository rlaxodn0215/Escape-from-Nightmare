// -----------------------------------------------------------------------------
// Codex comment pass: Ending Panel UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\EndingPanelUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Ending Panel UI UI elements, keeping references cached and visuals synchronized.
    public class EndingPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Text titleText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button skipButton;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private string defaultTitle = "Ending";
        [SerializeField] private string defaultMessage = "You escaped from the nightmare.";

        public event Action SkipRequested;

        public bool IsVisible
        {
            get { return rootObject != null && rootObject.activeSelf; }
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
            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(HandleSkipClicked);
                skipButton.onClick.AddListener(HandleSkipClicked);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(HandleSkipClicked);
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
            Show(defaultTitle, defaultMessage);
        }

        // Makes the related panel or visual element visible and fills in its current content.
        public void Show(string title, string message)
        {
            SetRootActive(true);

            if (titleText != null)
            {
                titleText.text = !string.IsNullOrEmpty(title) ? title : defaultTitle;
            }

            if (messageText != null)
            {
                messageText.text = !string.IsNullOrEmpty(message) ? message : defaultMessage;
            }
        }

        // Hides the related panel or visual element and clears transient interaction state.
        public void Hide()
        {
            SetRootActive(false);
        }

        // Handles a UI button press and routes it to the matching game flow.
        private void HandleSkipClicked()
        {
            if (SkipRequested != null)
            {
                SkipRequested();
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetRootActive(bool active)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            rootObject.SetActive(active);
        }
    }
}
