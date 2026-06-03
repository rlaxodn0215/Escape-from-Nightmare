using UnityEngine;

namespace EscapeFromNightmare
{
	// 오디오 설정 값을 저장하고 불러오는 데이터 구조입니다.
	// 값은 모두 0~1 범위이며 AudioManager가 AudioMixer dB 값으로 변환해 적용합니다.
	[System.Serializable]
	public struct AudioSettingsData
	{
		public float masterVolume;
		public float bgmVolume;
		public float sfxVolume;
		public float uiVolume;

		public static AudioSettingsData Default
		{
			get
			{
				return new AudioSettingsData
				{
					masterVolume = 1f,
					bgmVolume = 1f,
					sfxVolume = 1f,
					uiVolume = 1f
				};
			}
		}
	}

	// 게임 저장과 불러오기 기능을 담당할 매니저입니다.
	// 현재는 오디오 설정 저장 API를 제공하며, 실제 저장 매체는 PlayerPrefs를 사용합니다.
	public class SaveManager : Singleton<SaveManager>
	{
		private const string MasterVolumeKey = "Audio.MasterVolume";
		private const string BgmVolumeKey = "Audio.BGMVolume";
		private const string SfxVolumeKey = "Audio.SFXVolume";
		private const string UiVolumeKey = "Audio.UIVolume";

		public AudioSettingsData LoadAudioSettings()
		{
			AudioSettingsData defaultSettings = AudioSettingsData.Default;

			return new AudioSettingsData
			{
				masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, defaultSettings.masterVolume),
				bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, defaultSettings.bgmVolume),
				sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, defaultSettings.sfxVolume),
				uiVolume = PlayerPrefs.GetFloat(UiVolumeKey, defaultSettings.uiVolume)
			};
		}

		public void SaveAudioSettings(AudioSettingsData settings)
		{
			PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(settings.masterVolume));
			PlayerPrefs.SetFloat(BgmVolumeKey, Mathf.Clamp01(settings.bgmVolume));
			PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(settings.sfxVolume));
			PlayerPrefs.SetFloat(UiVolumeKey, Mathf.Clamp01(settings.uiVolume));
			PlayerPrefs.Save();
		}

		public void ResetAudioSettings()
		{
			SaveAudioSettings(AudioSettingsData.Default);
		}
	}
}
