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

	[System.Serializable]
	public struct InventorySaveData
	{
		public string[] itemIds;
		public string selectedItemId;

		public static InventorySaveData Empty
		{
			get
			{
				return new InventorySaveData
				{
					itemIds = System.Array.Empty<string>(),
					selectedItemId = string.Empty
				};
			}
		}
	}

	[System.Serializable]
	public struct PuzzleSaveData
	{
		public string[] solvedPuzzleIds;
		public string[] flagIds;

		public static PuzzleSaveData Empty
		{
			get
			{
				return new PuzzleSaveData
				{
					solvedPuzzleIds = System.Array.Empty<string>(),
					flagIds = System.Array.Empty<string>()
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
		private const string InventoryDataKey = "Inventory.Data";
		private const string PuzzleDataKey = "Puzzle.Data";

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

		public InventorySaveData LoadInventoryData()
		{
			string json = PlayerPrefs.GetString(InventoryDataKey, string.Empty);
			if (string.IsNullOrWhiteSpace(json))
			{
				return InventorySaveData.Empty;
			}

			try
			{
				InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
				if (data.itemIds == null)
				{
					data.itemIds = System.Array.Empty<string>();
				}

				if (data.selectedItemId == null)
				{
					data.selectedItemId = string.Empty;
				}

				return data;
			}
			catch (System.ArgumentException)
			{
				return InventorySaveData.Empty;
			}
		}

		public void SaveInventoryData(InventorySaveData data)
		{
			if (data.itemIds == null)
			{
				data.itemIds = System.Array.Empty<string>();
			}

			if (data.selectedItemId == null)
			{
				data.selectedItemId = string.Empty;
			}

			PlayerPrefs.SetString(InventoryDataKey, JsonUtility.ToJson(data));
			PlayerPrefs.Save();
		}

		public void ResetInventoryData()
		{
			PlayerPrefs.DeleteKey(InventoryDataKey);
			PlayerPrefs.Save();
		}

		public PuzzleSaveData LoadPuzzleData()
		{
			string json = PlayerPrefs.GetString(PuzzleDataKey, string.Empty);
			if (string.IsNullOrWhiteSpace(json))
			{
				return PuzzleSaveData.Empty;
			}

			try
			{
				PuzzleSaveData data = JsonUtility.FromJson<PuzzleSaveData>(json);
				if (data.solvedPuzzleIds == null)
				{
					data.solvedPuzzleIds = System.Array.Empty<string>();
				}

				if (data.flagIds == null)
				{
					data.flagIds = System.Array.Empty<string>();
				}

				return data;
			}
			catch (System.ArgumentException)
			{
				return PuzzleSaveData.Empty;
			}
		}

		public void SavePuzzleData(PuzzleSaveData data)
		{
			if (data.solvedPuzzleIds == null)
			{
				data.solvedPuzzleIds = System.Array.Empty<string>();
			}

			if (data.flagIds == null)
			{
				data.flagIds = System.Array.Empty<string>();
			}

			PlayerPrefs.SetString(PuzzleDataKey, JsonUtility.ToJson(data));
			PlayerPrefs.Save();
		}

		public void ResetPuzzleData()
		{
			PlayerPrefs.DeleteKey(PuzzleDataKey);
			PlayerPrefs.Save();
		}
	}
}
