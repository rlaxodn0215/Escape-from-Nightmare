// -----------------------------------------------------------------------------
// Codex comment pass: Hide Exit Button
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\HideExitButton.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Presentation controller for Hide Exit Button UI elements, keeping references cached and visuals synchronized.
    public class HideExitButton : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private bool showOnlyWhileHiding = true;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheButton();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
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

            RefreshVisibility();
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        // Refreshes frame-dependent input, timers, animation, or visual state.
        private void Update()
        {
            RefreshVisibility();
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheButton();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Performs the Handle Click operation while keeping its implementation details inside this script.
        private void HandleClick()
        {
            if (HideManager.Instance == null)
            {
                Debug.LogWarning("HideManager instance is missing.");
                return;
            }

            if (ScreenFadeManager.Instance != null)
            {
                ScreenFadeManager.Instance.PlayTransition(HideManager.Instance.ForceExitHidePoint);
                return;
            }

            HideManager.Instance.ForceExitHidePoint();
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        private void RefreshVisibility()
        {
            if (rootObject == null || !showOnlyWhileHiding)
            {
                return;
            }

            bool shouldShow = HideManager.Instance != null && HideManager.Instance.IsHiding;
            if (rootObject.activeSelf != shouldShow)
            {
                rootObject.SetActive(shouldShow);
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
