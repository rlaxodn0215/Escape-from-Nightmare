using EscapeFromNightmares.Services;
using EscapeFromNightmares.Data;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class SettingsAudioPanel : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider uiSlider;
        [SerializeField] private Button closeButton;
        [SerializeField] private Image panelBackgroundImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private Image masterLabelImage;
        [SerializeField] private Image bgmLabelImage;
        [SerializeField] private Image sfxLabelImage;
        [SerializeField] private Image uiLabelImage;

        private SettingsSaveService settingsSaveService;
        private SoundManager soundManager;
        private SettingsSaveService.SettingsData settings;

        public void Initialize(SettingsSaveService saveService, SoundManager manager, SettingsSaveService.SettingsData initialSettings)
        {
            settingsSaveService = saveService;
            soundManager = manager;
            settings = initialSettings ?? new SettingsSaveService.SettingsData();

            BindSlider(masterSlider, settings.masterVolume, OnMasterChanged);
            BindSlider(bgmSlider, settings.bgmVolume, OnBgmChanged);
            BindSlider(sfxSlider, settings.sfxVolume, OnSfxChanged);
            BindSlider(uiSlider, settings.uiVolume, OnUiChanged);
            BindCloseButton();

            ApplyAndSave();
        }

        public void SetSliders(Slider master, Slider bgm, Slider sfx, Slider ui)
        {
            masterSlider = master;
            bgmSlider = bgm;
            sfxSlider = sfx;
            uiSlider = ui;
        }

        public void SetControls(Slider master, Slider bgm, Slider sfx, Slider ui, Button close)
        {
            SetSliders(master, bgm, sfx, ui);
            closeButton = close;
        }

        public void SetVisuals(Image panelBackground, Image header, Image masterLabel, Image bgmLabel, Image sfxLabel, Image uiLabel)
        {
            panelBackgroundImage = panelBackground;
            headerImage = header;
            masterLabelImage = masterLabel;
            bgmLabelImage = bgmLabel;
            sfxLabelImage = sfxLabel;
            uiLabelImage = uiLabel;
        }

        public void ApplySprites(ResourceManager resourceManager, ResourcePathCatalog catalog)
        {
            if (resourceManager == null || catalog == null)
            {
                return;
            }

            SetImageSprite(resourceManager, panelBackgroundImage, catalog.settingsPanelBackgroundPath);
            SetImageSprite(resourceManager, headerImage, catalog.settingsHeaderPath);
            SetImageSprite(resourceManager, masterLabelImage, catalog.settingsMasterLabelPath);
            SetImageSprite(resourceManager, bgmLabelImage, catalog.settingsBgmLabelPath);
            SetImageSprite(resourceManager, sfxLabelImage, catalog.settingsSfxLabelPath);
            SetImageSprite(resourceManager, uiLabelImage, catalog.settingsUiLabelPath);
            SetButtonSprite(resourceManager, closeButton, catalog.titleCloseButtonPath);
            ApplySliderSprites(resourceManager, catalog, masterSlider);
            ApplySliderSprites(resourceManager, catalog, bgmSlider);
            ApplySliderSprites(resourceManager, catalog, sfxSlider);
            ApplySliderSprites(resourceManager, catalog, uiSlider);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnMasterChanged(float value)
        {
            settings.masterVolume = value;
            ApplyAndSave();
        }

        private void OnBgmChanged(float value)
        {
            settings.bgmVolume = value;
            ApplyAndSave();
        }

        private void OnSfxChanged(float value)
        {
            settings.sfxVolume = value;
            ApplyAndSave();
        }

        private void OnUiChanged(float value)
        {
            settings.uiVolume = value;
            ApplyAndSave();
        }

        private void ApplyAndSave()
        {
            soundManager?.ApplyVolumes(settings);
            settingsSaveService?.SaveSettings(settings);
        }

        private static void BindSlider(Slider slider, float value, UnityEngine.Events.UnityAction<float> callback)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.SetValueWithoutNotify(Mathf.Clamp01(value));
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(callback);
        }

        private void BindCloseButton()
        {
            if (closeButton == null)
            {
                closeButton = FindCloseButton();
            }

            if (closeButton == null)
            {
                return;
            }

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        private Button FindCloseButton()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            for (var index = 0; index < buttons.Length; index++)
            {
                if (buttons[index].name == "CloseButton")
                {
                    return buttons[index];
                }
            }

            return null;
        }

        private static void ApplySliderSprites(ResourceManager resourceManager, ResourcePathCatalog catalog, Slider slider)
        {
            if (slider == null)
            {
                return;
            }

            var trackImage = slider.transform.Find("Background")?.GetComponent<Image>();
            SetImageSprite(resourceManager, trackImage, catalog.settingsSliderTrackPath);

            var fillImage = slider.fillRect != null ? slider.fillRect.GetComponent<Image>() : null;
            SetImageSprite(resourceManager, fillImage, catalog.settingsSliderFillPath);

            var handleImage = slider.handleRect != null ? slider.handleRect.GetComponent<Image>() : null;
            SetImageSprite(resourceManager, handleImage, catalog.settingsSliderHandlePath);
        }

        private static void SetButtonSprite(ResourceManager resourceManager, Button button, string path)
        {
            if (button == null)
            {
                return;
            }

            SetImageSprite(resourceManager, button.GetComponent<Image>(), path);
        }

        private static void SetImageSprite(ResourceManager resourceManager, Image image, string path)
        {
            if (image == null)
            {
                return;
            }

            if (image.sprite == null)
            {
                image.sprite = resourceManager.LoadSprite(path);
            }

            image.color = Color.white;
        }
    }
}
