using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	// 설정 창의 마스터, 배경음, 효과음, 사용자 인터페이스 볼륨 슬라이더를 오디오 매니저와 연결합니다.
	// 슬라이더 값이 바뀌면 즉시 오디오 믹서와 저장 데이터에 반영됩니다.
	public class UIAudioSettingsView : MonoBehaviour
	{
		[SerializeField] private GameObject rootPanel;
		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider bgmVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;
		[SerializeField] private Slider uiVolumeSlider;
		[SerializeField] private Button closeButton;

		private bool isSyncing;

		// 패널 참조를 보정하고 이벤트를 등록한 뒤 기본 상태를 닫힘으로 둡니다.
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

		// 설정 창을 열고 현재 오디오 설정 값을 슬라이더에 반영합니다.
		public void Open()
		{
			SyncFromAudioManager();
			rootPanel.SetActive(true);
		}

		// 설정 창을 닫습니다.
		public void Close()
		{
			rootPanel.SetActive(false);
		}

		// 오디오 매니저의 현재 볼륨 값을 슬라이더 화면에 동기화합니다.
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

		// 슬라이더와 닫기 버튼 이벤트를 등록합니다.
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

		// 등록된 화면 이벤트를 해제합니다.
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
