using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Centralizes modal panel fade and immediate visibility behavior.
    /// </summary>
    public sealed class ModalViewController
    {
        private readonly MonoBehaviour owner;
        private readonly float fadeDuration;

        public ModalViewController(MonoBehaviour owner, float fadeDuration)
        {
            this.owner = owner;
            this.fadeDuration = fadeDuration;
        }

        public void StartPanelFade(GameObject panel, bool visible, ref Coroutine routine, System.Action onComplete = null)
        {
            if (panel == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (routine != null)
            {
                owner.StopCoroutine(routine);
            }

            routine = owner.StartCoroutine(PanelFader.FadePanel(panel, visible, fadeDuration, onComplete));
        }

        public void HidePanelImmediate(GameObject panel, ref Coroutine routine)
        {
            if (panel == null)
            {
                return;
            }

            if (routine != null)
            {
                owner.StopCoroutine(routine);
                routine = null;
            }

            var group = PanelFader.EnsureCanvasGroup(panel);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            panel.SetActive(false);
        }

        public static CanvasGroup EnsureCanvasGroup(GameObject panel)
        {
            return PanelFader.EnsureCanvasGroup(panel);
        }
    }
}
