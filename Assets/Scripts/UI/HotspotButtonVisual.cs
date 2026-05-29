using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
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

        private void Awake()
        {
            CacheReferences();
            ApplyVisualState();
        }

        private void Reset()
        {
            CacheReferences();
            ApplyVisualState();
        }

        private void OnValidate()
        {
            CacheReferences();
            ApplyVisualState();
        }

        public void SetDisplayLabel(string label)
        {
            displayLabel = label;
            ApplyVisualState();
        }

        public void SetDebugVisible(bool visible)
        {
            showDebugLabel = visible;
            ApplyVisualState();
        }

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

        public void MakeTransparentHotspot()
        {
            showDebugLabel = false;
            ApplyVisualState();
        }

        public void MakeDebugVisible()
        {
            showDebugLabel = true;
            ApplyVisualState();
        }

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
