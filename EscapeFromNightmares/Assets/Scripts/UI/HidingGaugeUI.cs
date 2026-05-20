using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class HidingGaugeUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image gaugeFillImage;
        [SerializeField] private TMP_Text label;
        [SerializeField, Range(0f, 1f)] private float danger01;

        public float Danger01 => danger01;

        private void Awake()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
            ApplyDanger();
        }

        private void OnValidate()
        {
            danger01 = Mathf.Clamp01(danger01);
            ApplyDanger();
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void SetDanger01(float value)
        {
            danger01 = Mathf.Clamp01(value);
            ApplyDanger();
        }

        public void SetLabel(string value)
        {
            if (label != null)
            {
                label.text = value ?? string.Empty;
            }
        }

        private void ApplyDanger()
        {
            if (gaugeFillImage != null)
            {
                gaugeFillImage.fillAmount = danger01;
            }

            if (label != null)
            {
                label.text = danger01 > 0f ? $"{Mathf.RoundToInt(danger01 * 100f)}%" : string.Empty;
            }
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(true);

            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
