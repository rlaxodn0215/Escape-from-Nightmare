using EscapeFromNightmares.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class SettingsUI : MonoBehaviour
    {
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button closeButton;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TMP_Text bgmValueLabel;
        [SerializeField] private TMP_Text sfxValueLabel;

        private void Awake()
        {
            saveManager ??= FindFirstObjectByType<SaveManager>();
            canvasGroup ??= GetComponent<CanvasGroup>();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (bgmSlider != null)
            {
                bgmSlider.onValueChanged.AddListener(_ => SaveCurrentValues());
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.AddListener(_ => SaveCurrentValues());
            }

            RefreshFromSave();
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public void Bind(SaveManager nextSaveManager)
        {
            saveManager = nextSaveManager;
            RefreshFromSave();
        }

        public void Show()
        {
            RefreshFromSave();
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void Toggle()
        {
            bool isVisible = canvasGroup == null ? gameObject.activeSelf : canvasGroup.alpha > 0.5f;
            SetVisible(!isVisible);
        }

        private void RefreshFromSave()
        {
            float bgm = saveManager != null ? saveManager.BgmVolume : 0.8f;
            float sfx = saveManager != null ? saveManager.SfxVolume : 0.8f;
            SetSliderValueWithoutNotify(bgmSlider, bgm);
            SetSliderValueWithoutNotify(sfxSlider, sfx);
            RefreshLabels();
        }

        private void SaveCurrentValues()
        {
            float bgm = bgmSlider != null ? bgmSlider.value : 0.8f;
            float sfx = sfxSlider != null ? sfxSlider.value : 0.8f;
            saveManager?.SaveSettings(bgm, sfx);
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            if (bgmValueLabel != null && bgmSlider != null)
            {
                bgmValueLabel.text = Mathf.RoundToInt(bgmSlider.value * 100f).ToString();
            }

            if (sfxValueLabel != null && sfxSlider != null)
            {
                sfxValueLabel.text = Mathf.RoundToInt(sfxSlider.value * 100f).ToString();
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

        private static void SetSliderValueWithoutNotify(Slider slider, float value)
        {
            if (slider != null)
            {
                slider.SetValueWithoutNotify(Mathf.Clamp01(value));
            }
        }
    }
}
