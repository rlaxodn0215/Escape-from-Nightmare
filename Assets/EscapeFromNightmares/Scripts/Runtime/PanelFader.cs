using System.Collections;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Shared CanvasGroup fade helpers for modal and scene transition panels.
    /// </summary>
    public static class PanelFader
    {
        public static IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
        {
            var start = group.alpha;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            group.alpha = targetAlpha;
        }

        public static IEnumerator FadePanel(GameObject panel, bool visible, float duration, System.Action onComplete)
        {
            var group = EnsureCanvasGroup(panel);
            if (visible)
            {
                panel.SetActive(true);
                group.alpha = 0f;
            }

            group.interactable = false;
            group.blocksRaycasts = true;

            var end = visible ? 1f : 0f;
            yield return FadeCanvasGroup(group, end, duration);

            group.interactable = visible;
            group.blocksRaycasts = visible;
            if (!visible)
            {
                panel.SetActive(false);
            }

            onComplete?.Invoke();
        }

        public static CanvasGroup EnsureCanvasGroup(GameObject panel)
        {
            var group = panel.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = panel.AddComponent<CanvasGroup>();
            }

            return group;
        }
    }
}
