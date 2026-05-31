// -----------------------------------------------------------------------------
// Codex comment pass: Hotspot Button Visual
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\HotspotButtonVisual.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Presentation controller for Hotspot Button Visual UI elements, keeping references cached and visuals synchronized.
    public class HotspotButtonVisual : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private Text labelText;
        [SerializeField] private GameObject labelRoot;
        [SerializeField] private bool showDebugLabel = true;
        [SerializeField] private float visibleAlpha = 0.25f;
        [SerializeField] private float hiddenAlpha = 0.02f;
        [SerializeField] private bool keepRaycastTarget = true;
        [SerializeField] private string displayLabel;

        public bool ShowDebugLabel
        {
            get { return showDebugLabel; }
        }

        public string DisplayLabel
        {
            get { return displayLabel; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheReferences();
            ApplyVisualState();
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheReferences();
            ApplyVisualState();
        }

        // Keeps Inspector-edited values and cached references valid while working in the editor.
        private void OnValidate()
        {
            CacheReferences();
            ApplyVisualState();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetDisplayLabel(string label)
        {
            displayLabel = label;
            ApplyVisualState();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetDebugVisible(bool visible)
        {
            showDebugLabel = visible;
            ApplyVisualState();
        }

        // Applies calculated settings to Unity components or runtime state.
        public void ApplyVisualState()
        {
            CacheReferences();

            if (labelText != null)
            {
                labelText.text = displayLabel;
                labelText.enabled = showDebugLabel;
            }

            if (labelRoot != null)
            {
                labelRoot.SetActive(showDebugLabel);
            }

            if (buttonImage != null)
            {
                Color color = buttonImage.color;
                color.a = showDebugLabel ? visibleAlpha : hiddenAlpha;
                buttonImage.color = color;
                if (keepRaycastTarget)
                {
                    buttonImage.raycastTarget = true;
                }
            }
        }

        // Performs the Make Transparent Hotspot operation while keeping its implementation details inside this script.
        public void MakeTransparentHotspot()
        {
            showDebugLabel = false;
            ApplyVisualState();
        }

        // Performs the Make Debug Visible operation while keeping its implementation details inside this script.
        public void MakeDebugVisible()
        {
            showDebugLabel = true;
            ApplyVisualState();
        }

        // Performs the Cache References operation while keeping its implementation details inside this script.
        private void CacheReferences()
        {
            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }

            if (labelText == null)
            {
                labelText = GetComponentInChildren<Text>(true);
            }

            if (labelRoot == null && labelText != null)
            {
                labelRoot = labelText.gameObject;
            }
        }
    }
}
