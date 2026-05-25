using EscapeFromNightmares.Services;
using EscapeFromNightmares.Data;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    /// <summary>
    /// 타이틀 설정 패널에서 오디오 슬라이더 값을 저장하고 SoundManager에 즉시 반영합니다.
    /// </summary>
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

        /// <summary>
        /// 저장 서비스, 사운드 매니저, 현재 설정을 연결하고 슬라이더 이벤트를 바인딩합니다.
        /// </summary>
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

        /// <summary>오디오 슬라이더 참조를 설정합니다.</summary>
        public void SetSliders(Slider master, Slider bgm, Slider sfx, Slider ui)
        {
            masterSlider = master;
            bgmSlider = bgm;
            sfxSlider = sfx;
            uiSlider = ui;
        }

        /// <summary>오디오 슬라이더와 닫기 버튼 참조를 설정합니다.</summary>
        public void SetControls(Slider master, Slider bgm, Slider sfx, Slider ui, Button close)
        {
            SetSliders(master, bgm, sfx, ui);
            closeButton = close;
        }

        /// <summary>설정 패널에 사용할 배경, 헤더, 라벨 이미지 참조를 설정합니다.</summary>
        public void SetVisuals(Image panelBackground, Image header, Image masterLabel, Image bgmLabel, Image sfxLabel, Image uiLabel)
        {
            panelBackgroundImage = panelBackground;
            headerImage = header;
            masterLabelImage = masterLabel;
            bgmLabelImage = bgmLabel;
            sfxLabelImage = sfxLabel;
            uiLabelImage = uiLabel;
        }

        /// <summary>
        /// ResourcePathCatalog의 경로를 사용해 패널 이미지와 슬라이더 이미지를 적용합니다.
        /// </summary>
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

        /// <summary>설정 패널을 닫습니다.</summary>
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
