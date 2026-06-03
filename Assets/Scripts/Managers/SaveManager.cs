using UnityEngine;

namespace EscapeFromNightmare
{
	// 오디오 설정 값을 저장하고 불러올 때 사용하는 데이터입니다.
	// 모든 볼륨 값은 0에서 1 사이의 정규화된 값입니다.
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

	// 게임 설정 저장과 불러오기를 담당하는 전역 매니저입니다.
	// 현재는 오디오 설정을 플레이어 설정 저장소에 저장합니다.
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
