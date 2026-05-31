// -----------------------------------------------------------------------------
// Codex comment pass: Navigation Button
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\NavigationButton.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Scene interaction component for Navigation Button, converting player input into manager-level requests.
    public class NavigationButton : MonoBehaviour
    {
        [SerializeField] private NavigationActionType actionType;
        [SerializeField] private string targetLocationId;
        [SerializeField] private string targetViewId;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;

        public NavigationActionType ActionType
        {
            get { return actionType; }
        }

        public string TargetLocationId
        {
            get { return targetLocationId; }
        }

        public string TargetViewId
        {
            get { return targetViewId; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheButton();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheButton();
        }

        // Performs the Handle Click operation while keeping its implementation details inside this script.
        private void HandleClick()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiClick);
            }

            if (LocationManager.Instance == null)
            {
                Debug.LogWarning("LocationManager instance is missing.");
                return;
            }

            switch (actionType)
            {
                case NavigationActionType.RotateLeft:
                    PlayMovementTransition(LocationManager.Instance.RotateLeft);
                    break;
                case NavigationActionType.RotateRight:
                    PlayMovementTransition(LocationManager.Instance.RotateRight);
                    break;
                case NavigationActionType.SetLocation:
                    if (string.IsNullOrEmpty(targetLocationId))
                    {
                        Debug.LogWarning("NavigationButton targetLocationId is empty: " + name, this);
                        return;
                    }

                    PlayMovementTransition(() => LocationManager.Instance.SetLocation(targetLocationId, targetViewId));
                    break;
                case NavigationActionType.SetView:
                    if (string.IsNullOrEmpty(targetViewId))
                    {
                        Debug.LogWarning("NavigationButton targetViewId is empty: " + name, this);
                        return;
                    }

                    PlayMovementTransition(() => LocationManager.Instance.SetView(targetViewId));
                    break;
                default:
                    Debug.LogWarning("Unhandled navigation action type: " + actionType, this);
                    break;
            }
        }

        private void PlayMovementTransition(System.Action action)
        {
            if (ScreenFadeManager.Instance != null)
            {
                ScreenFadeManager.Instance.PlayTransition(action);
                return;
            }

            if (action != null)
            {
                action();
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
    }
}
