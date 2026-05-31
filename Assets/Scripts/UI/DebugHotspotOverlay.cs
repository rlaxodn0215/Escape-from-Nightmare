// -----------------------------------------------------------------------------
// Codex comment pass: Debug Hotspot Overlay
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\DebugHotspotOverlay.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Presentation controller for Debug Hotspot Overlay UI elements, keeping references cached and visuals synchronized.
    public class DebugHotspotOverlay : MonoBehaviour
    {
        [SerializeField] private bool showOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;
        [SerializeField] private bool allowRuntimeToggle = true;

        public bool IsVisible { get; private set; }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            SetVisible(showOnStart);
        }

        // Refreshes frame-dependent input, timers, animation, or visual state.
        private void Update()
        {
            if (allowRuntimeToggle && Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetVisible(bool visible)
        {
            IsVisible = visible;
            HotspotButtonVisual[] hotspots = FindAllHotspots();
            for (int i = 0; i < hotspots.Length; i++)
            {
                if (hotspots[i] != null)
                {
                    hotspots[i].SetDebugVisible(visible);
                }
            }
        }

        // Performs the Toggle operation while keeping its implementation details inside this script.
        public void Toggle()
        {
            SetVisible(!IsVisible);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private HotspotButtonVisual[] FindAllHotspots()
        {
            return Object.FindObjectsByType<HotspotButtonVisual>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
    }
}
