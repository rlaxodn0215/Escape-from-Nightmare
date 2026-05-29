using UnityEngine;

namespace EscapeFromNightmare
{
    public class DebugHotspotOverlay : MonoBehaviour
    {
        [SerializeField] private bool showOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;
        [SerializeField] private bool allowRuntimeToggle = true;

        public bool IsVisible { get; private set; }

        private void Start()
        {
            SetVisible(showOnStart);
        }

        private void Update()
        {
            if (allowRuntimeToggle && Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
        }

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

        public void Toggle()
        {
            SetVisible(!IsVisible);
        }

        private HotspotButtonVisual[] FindAllHotspots()
        {
            return Object.FindObjectsByType<HotspotButtonVisual>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
    }
}
