using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	// 설정 창에서 Master/BGM/SFX/UI 음량 슬라이더를 AudioManager와 연결하는 UI 뷰입니다.
	// 슬라이더는 Fill 이미지를 가진 progress bar 형태로 배치하고, 값 변경 즉시 AudioMixer에 반영합니다.
	public class UIAudioSettingsView : MonoBehaviour
	{
		[SerializeField] private GameObject rootPanel;
		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider bgmVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;
		[SerializeField] private Slider uiVolumeSlider;
		[SerializeField] private Button closeButton;

		private bool isSyncing;

		private void Awake()
		{
			if (rootPanel == null)
			{
				rootPanel = gameObject;
			}

			RegisterEvents();
			Close();
		}

		private void OnDestroy()
		{
			UnregisterEvents();
		}

		public void Open()
		{
			SyncFromAudioManager();
			rootPanel.SetActive(true);
		}

		public void Close()
		{
			rootPanel.SetActive(false);
		}

		public void SyncFromAudioManager()
		{
			if (AudioManager.Instance == null)
			{
				return;
			}

			isSyncing = true;
			SetSliderValue(masterVolumeSlider, AudioManager.Instance.GetMasterVolume());
			SetSliderValue(bgmVolumeSlider, AudioManager.Instance.GetBgmVolume());
			SetSliderValue(sfxVolumeSlider, AudioManager.Instance.GetSfxVolume());
			SetSliderValue(uiVolumeSlider, AudioManager.Instance.GetUiVolume());
			isSyncing = false;
		}

		private void RegisterEvents()
		{
			if (masterVolumeSlider != null)
			{
				masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChanged);
			}

			if (bgmVolumeSlider != null)
			{
				bgmVolumeSlider.onValueChanged.AddListener(HandleBgmVolumeChanged);
			}

			if (sfxVolumeSlider != null)
			{
				sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChanged);
			}

			if (uiVolumeSlider != null)
			{
				uiVolumeSlider.onValueChanged.AddListener(HandleUiVolumeChanged);
			}

			if (closeButton != null)
			{
				closeButton.onClick.AddListener(Close);
			}
		}

		private void UnregisterEvents()
		{
			if (masterVolumeSlider != null)
			{
				masterVolumeSlider.onValueChanged.RemoveListener(HandleMasterVolumeChanged);
			}

			if (bgmVolumeSlider != null)
			{
				bgmVolumeSlider.onValueChanged.RemoveListener(HandleBgmVolumeChanged);
			}

			if (sfxVolumeSlider != null)
			{
				sfxVolumeSlider.onValueChanged.RemoveListener(HandleSfxVolumeChanged);
			}

			if (uiVolumeSlider != null)
			{
				uiVolumeSlider.onValueChanged.RemoveListener(HandleUiVolumeChanged);
			}

			if (closeButton != null)
			{
				closeButton.onClick.RemoveListener(Close);
			}
		}

		private void HandleMasterVolumeChanged(float value)
		{
			if (!isSyncing && AudioManager.Instance != null)
			{
				AudioManager.Instance.SetMasterVolume(value);
			}
		}

		private void HandleBgmVolumeChanged(float value)
		{
			if (!isSyncing && AudioManager.Instance != null)
			{
				AudioManager.Instance.SetBgmVolume(value);
			}
		}

		private void HandleSfxVolumeChanged(float value)
		{
			if (!isSyncing && AudioManager.Instance != null)
			{
				AudioManager.Instance.SetSfxVolume(value);
			}
		}

		private void HandleUiVolumeChanged(float value)
		{
			if (!isSyncing && AudioManager.Instance != null)
			{
				AudioManager.Instance.SetUiVolume(value);
			}
		}

		private static void SetSliderValue(Slider slider, float value)
		{
			if (slider != null)
			{
				slider.SetValueWithoutNotify(Mathf.Clamp01(value));
			}
		}
	}
}
