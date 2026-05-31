// -----------------------------------------------------------------------------
// Codex comment pass: Clickable Button
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\ClickableButton.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Scene interaction component for Clickable Button, converting player input into manager-level requests.
    public class ClickableButton : MonoBehaviour
    {
        // Stores the clickable Id value used by this script's runtime or editor workflow.
        public string clickableId;
        // Stores the clickable Type value used by this script's runtime or editor workflow.
        public ClickableType clickableType;
        // Stores the linked Location Id value used by this script's runtime or editor workflow.
        public string linkedLocationId;
        // Stores the linked View Id value used by this script's runtime or editor workflow.
        public string linkedViewId;
        // Stores the linked Door Id value used by this script's runtime or editor workflow.
        public string linkedDoorId;
        // Stores the linked Puzzle Id value used by this script's runtime or editor workflow.
        public string linkedPuzzleId;
        // Stores the linked Clue Image Id value used by this script's runtime or editor workflow.
        public string linkedClueImageId;
        // Stores the linked Item Id value used by this script's runtime or editor workflow.
        public string linkedItemId;
        // Stores the required Item Id value used by this script's runtime or editor workflow.
        public string requiredItemId;
        // Stores the target Object Id value used by this script's runtime or editor workflow.
        public string targetObjectId;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;

        public string ClickableId
        {
            get { return clickableId; }
        }

        public ClickableType ClickableType
        {
            get { return clickableType; }
        }

        public string LinkedLocationId
        {
            get { return linkedLocationId; }
        }

        public string LinkedViewId
        {
            get { return linkedViewId; }
        }

        public string LinkedDoorId
        {
            get { return linkedDoorId; }
        }

        public string LinkedPuzzleId
        {
            get { return linkedPuzzleId; }
        }

        public string LinkedClueImageId
        {
            get { return linkedClueImageId; }
        }

        public string LinkedItemId
        {
            get { return linkedItemId; }
        }

        public string RequiredItemId
        {
            get { return requiredItemId; }
        }

        public string TargetObjectId
        {
            get { return targetObjectId; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheButton();
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheButton();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
                button.onClick.AddListener(OnClicked);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
            }
        }

        // Performs the Cache Button operation while keeping its implementation details inside this script.
        private void CacheButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        // Converts a click callback into a request that the gameplay managers understand.
        private void OnClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiClick);
            }

            if (InteractionManager.Instance == null)
            {
                Debug.LogWarning("InteractionManager instance is missing.");
                return;
            }

            InteractionManager.Instance.HandleClick(this);
        }
    }
}
