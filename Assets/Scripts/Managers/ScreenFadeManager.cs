using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class ScreenFadeManager : Singleton<ScreenFadeManager>
    {
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeOutSeconds = 0.6f;
        [SerializeField] private float fadeInSeconds = 0.6f;
        [SerializeField] private Color fadeColor = Color.black;

        private Image fadeImage;
        private Coroutine transitionRoutine;
        private bool isTransitioning;

        public bool IsTransitioning
        {
            get { return isTransitioning; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this)
            {
                return;
            }

            CacheReferences();
            SetFadeAlpha(0f, false);
        }

        private void OnValidate()
        {
            CacheReferences();
            ApplyFadeColor();
        }

        public void PlayTransition(Action action)
        {
            if (isTransitioning)
            {
                return;
            }

            if (fadeCanvasGroup == null || !gameObject.activeInHierarchy)
            {
                if (action != null)
                {
                    action();
                }

                return;
            }

            transitionRoutine = StartCoroutine(PlayTransitionRoutine(action));
        }

        private IEnumerator PlayTransitionRoutine(Action action)
        {
            isTransitioning = true;
            SetFadeAlpha(fadeCanvasGroup.alpha, true);

            yield return FadeTo(1f, fadeOutSeconds);

            if (action != null)
            {
                action();
            }

            yield return null;
            yield return FadeTo(0f, fadeInSeconds);

            isTransitioning = false;
            transitionRoutine = null;
            SetFadeAlpha(0f, false);
        }

        private IEnumerator FadeTo(float targetAlpha, float seconds)
        {
            float duration = Mathf.Max(0f, seconds);
            float startAlpha = fadeCanvasGroup != null ? fadeCanvasGroup.alpha : 0f;

            if (Mathf.Approximately(duration, 0f))
            {
                SetFadeAlpha(targetAlpha, true);
                yield break;
            }

            for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
            {
                float t = Mathf.Clamp01(time / duration);
                SetFadeAlpha(Mathf.Lerp(startAlpha, targetAlpha, t), true);
                yield return null;
            }

            SetFadeAlpha(targetAlpha, true);
        }

        private void SetFadeAlpha(float alpha, bool blockRaycasts)
        {
            CacheReferences();
            if (fadeCanvasGroup == null)
            {
                return;
            }

            fadeCanvasGroup.alpha = alpha;
            fadeCanvasGroup.interactable = blockRaycasts;
            fadeCanvasGroup.blocksRaycasts = blockRaycasts;
            ApplyFadeColor();
        }

        private void CacheReferences()
        {
            if (fadeCanvasGroup == null)
            {
                fadeCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (fadeCanvasGroup != null && fadeImage == null)
            {
                fadeImage = fadeCanvasGroup.GetComponent<Image>();
            }
        }

        private void ApplyFadeColor()
        {
            if (fadeImage != null)
            {
                fadeImage.color = fadeColor;
                fadeImage.raycastTarget = true;
            }
        }
    }
}
