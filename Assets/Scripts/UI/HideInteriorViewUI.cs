using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [Serializable]
    public class HideInteriorViewMapping
    {
        public string hidePointId;
        public string resourcesPath;
    }

    [RequireComponent(typeof(Image))]
    public class HideInteriorViewUI : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private GameObject rootObject;
        [SerializeField] private List<HideInteriorViewMapping> hideInteriorViews = new List<HideInteriorViewMapping>();
        [SerializeField] private string fallbackResourcePath;

        private CanvasGroup canvasGroup;
        private CanvasGroup rootCanvasGroup;
        private bool isShowing;
        private bool isSubscribed;
        private string lastMissingMappingHidePoint;
        private string lastMissingResourcePath;

        private void Awake()
        {
            CacheReferences();
            SetVisible(false);
        }

        private void OnEnable()
        {
            EnsureSubscribed();
            SetVisible(isShowing);
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            EnsureSubscribed();
            SynchronizeWithCurrentHideState();
        }

        private void Reset()
        {
            CacheReferences();
        }

        private void OnValidate()
        {
            CacheReferences();
        }

        private void EnsureSubscribed()
        {
            if (isSubscribed)
            {
                return;
            }

            if (HideManager.Instance == null)
            {
                return;
            }

            HideManager.Instance.HideEntered -= HandleHideEntered;
            HideManager.Instance.HideEntered += HandleHideEntered;
            HideManager.Instance.HideExited -= HandleHideExited;
            HideManager.Instance.HideExited += HandleHideExited;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed || HideManager.Instance == null)
            {
                return;
            }

            HideManager.Instance.HideEntered -= HandleHideEntered;
            HideManager.Instance.HideExited -= HandleHideExited;
            isSubscribed = false;
        }

        private void SynchronizeWithCurrentHideState()
        {
            if (HideManager.Instance == null || !HideManager.Instance.IsHiding)
            {
                if (isShowing)
                {
                    SetVisible(false);
                }

                return;
            }

            string resourcePath = GetResourcePathForHidePoint(HideManager.Instance.CurrentHidePointId);
            if (!string.IsNullOrEmpty(resourcePath))
            {
                if (!isShowing && LoadSpriteWithFallback(resourcePath))
                {
                    SetVisible(true);
                }

                return;
            }

            if (isShowing)
            {
                SetVisible(false);
            }
        }

        private void HandleHideEntered(string hidePointId)
        {
            string resourcePath = GetResourcePathForHidePoint(hidePointId);
            if (string.IsNullOrEmpty(resourcePath))
            {
                SetVisible(false);
                return;
            }

            if (LoadSpriteWithFallback(resourcePath))
            {
                SetVisible(true);
            }
        }

        private void HandleHideExited(string hidePointId)
        {
            SetVisible(false);
        }

        private bool LoadSprite(string resourcePath)
        {
            CacheReferences();
            if (targetImage == null || string.IsNullOrEmpty(resourcePath))
            {
                return false;
            }

            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite == null)
            {
                if (lastMissingResourcePath != resourcePath)
                {
                    Debug.LogWarning("Hide interior sprite not found at Resources path: " + resourcePath, this);
                    lastMissingResourcePath = resourcePath;
                }

                return false;
            }

            lastMissingResourcePath = string.Empty;
            targetImage.sprite = sprite;
            targetImage.color = Color.white;
            targetImage.preserveAspect = false;
            targetImage.raycastTarget = false;
            return true;
        }

        private bool LoadSpriteWithFallback(string resourcePath)
        {
            if (LoadSprite(resourcePath))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(fallbackResourcePath) && fallbackResourcePath != resourcePath)
            {
                return LoadSprite(fallbackResourcePath);
            }

            return false;
        }

        private string GetResourcePathForHidePoint(string hidePointId)
        {
            if (!string.IsNullOrEmpty(hidePointId))
            {
                for (int i = 0; i < hideInteriorViews.Count; i++)
                {
                    HideInteriorViewMapping mapping = hideInteriorViews[i];
                    if (mapping != null && mapping.hidePointId == hidePointId && !string.IsNullOrEmpty(mapping.resourcesPath))
                    {
                        return mapping.resourcesPath;
                    }
                }
            }

            if (!string.IsNullOrEmpty(fallbackResourcePath))
            {
                return fallbackResourcePath;
            }

            if (lastMissingMappingHidePoint != hidePointId)
            {
                Debug.LogWarning("Hide interior mapping not found for hide point: " + hidePointId, this);
                lastMissingMappingHidePoint = hidePointId;
            }

            return string.Empty;
        }

        private void SetVisible(bool visible)
        {
            CacheReferences();
            isShowing = visible;

            if (targetImage != null)
            {
                targetImage.enabled = visible;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (rootCanvasGroup != null)
            {
                rootCanvasGroup.alpha = visible ? 1f : 0f;
                rootCanvasGroup.interactable = visible;
                rootCanvasGroup.blocksRaycasts = visible;
            }
        }

        private void CacheReferences()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (rootObject != null && rootCanvasGroup == null)
            {
                rootCanvasGroup = rootObject.GetComponent<CanvasGroup>();
            }
        }
    }
}
