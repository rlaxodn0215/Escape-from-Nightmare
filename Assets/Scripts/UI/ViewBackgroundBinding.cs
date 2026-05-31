// -----------------------------------------------------------------------------
// Codex comment pass: View Background Binding
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\ViewBackgroundBinding.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Image))]
    // Presentation controller for View Background Binding UI elements, keeping references cached and visuals synchronized.
    public class ViewBackgroundBinding : MonoBehaviour
    {
        [SerializeField] private string resourcesPath;
        [SerializeField] private Image targetImage;
        [SerializeField] private bool loadOnEnable = true;
        [SerializeField] private bool hideImageWhenMissing = false;
        [SerializeField] private Color fallbackColor = new Color(0.05f, 0.05f, 0.05f, 1f);

        public string ResourcesPath
        {
            get { return resourcesPath; }
        }

        public Image TargetImage
        {
            get { return targetImage; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            EnsureImage();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            if (loadOnEnable)
            {
                LoadSprite();
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            EnsureImage();
        }

        // Keeps Inspector-edited values and cached references valid while working in the editor.
        private void OnValidate()
        {
            EnsureImage();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetResourcesPath(string path)
        {
            resourcesPath = path;
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        public bool LoadSprite()
        {
            EnsureImage();
            if (targetImage == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(resourcesPath))
            {
                ClearSprite();
                return false;
            }

            Sprite sprite = Resources.Load<Sprite>(resourcesPath);
            if (sprite != null)
            {
                targetImage.sprite = sprite;
                targetImage.enabled = true;
                targetImage.color = Color.white;
                targetImage.raycastTarget = false;
                return true;
            }

            Debug.LogWarning("View background sprite not found at Resources path: " + resourcesPath, this);
            ClearSprite();
            return false;
        }

        // Performs the Clear Sprite operation while keeping its implementation details inside this script.
        public void ClearSprite()
        {
            EnsureImage();
            if (targetImage == null)
            {
                return;
            }

            targetImage.sprite = null;
            targetImage.color = fallbackColor;
            targetImage.enabled = !hideImageWhenMissing;
            targetImage.raycastTarget = false;
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private void EnsureImage()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }
        }
    }
}
