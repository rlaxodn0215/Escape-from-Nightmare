using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Image))]
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

        private void Awake()
        {
            EnsureImage();
        }

        private void OnEnable()
        {
            if (loadOnEnable)
            {
                LoadSprite();
            }
        }

        private void Reset()
        {
            EnsureImage();
        }

        private void OnValidate()
        {
            EnsureImage();
        }

        public void SetResourcesPath(string path)
        {
            resourcesPath = path;
        }

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

        private void EnsureImage()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }
        }
    }
}
