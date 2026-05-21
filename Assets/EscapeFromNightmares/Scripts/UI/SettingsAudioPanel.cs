using EscapeFromNightmares.Services;
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
    }
}
