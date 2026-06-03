using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
	// 프로젝트에서 재생할 수 있는 사운드의 식별자입니다.
	public enum AudioSoundId
	{
		None = 0,
		TitleBgm = 1,
		MainBgm = 2,
		RoomTone = 3,
		WindChimes = 4,
		ButtonClick = 5,
		Confirm = 6,
		UiFail = 7,
		DoorOpen = 8,
		DoorLocked = 9,
		DoorSqueak = 10,
		ItemCollect = 11,
		DeepBoom = 12,
		LowWhoosh = 13
	}

	// 사운드를 출력할 오디오 채널입니다.
	public enum AudioChannel
	{
		Bgm = 0,
		Sfx = 1,
		Ui = 2
	}

	// 사운드 식별자와 실제 오디오 클립, 재생 설정을 묶어 둔 데이터입니다.
	[Serializable]
	public class AudioSoundEntry
	{
		[SerializeField] private AudioSoundId id = AudioSoundId.None;
		[SerializeField] private AudioChannel channel = AudioChannel.Sfx;
		[SerializeField] private AudioClip clip;
		[SerializeField, Range(0f, 2f)] private float volumeScale = 1f;
		[SerializeField, Range(0.1f, 3f)] private float pitch = 1f;
		[SerializeField] private bool loop;

		public AudioSoundId Id => id;
		public AudioChannel Channel => channel;
		public AudioClip Clip => clip;
		public float VolumeScale => volumeScale;
		public float Pitch => pitch;
		public bool Loop => loop;
	}

	// 오디오 매니저가 사운드 식별자로 오디오 클립을 찾을 수 있게 해 주는 데이터베이스 에셋입니다.
	[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Escape From Nightmare/Audio Database")]
	public class AudioDatabase : ScriptableObject
	{
		[SerializeField] private List<AudioSoundEntry> sounds = new();

		private Dictionary<AudioSoundId, AudioSoundEntry> soundLookup;

		public IReadOnlyList<AudioSoundEntry> Sounds => sounds;

		// 사운드 식별자에 해당하는 등록 정보를 조회합니다.
		public bool TryGetSound(AudioSoundId id, out AudioSoundEntry entry)
		{
			if (soundLookup == null)
			{
				BuildLookup();
			}

			return soundLookup.TryGetValue(id, out entry);
		}

		private void OnEnable()
		{
			BuildLookup();
		}

		private void OnValidate()
		{
			BuildLookup();
		}

		// 인스펙터 목록을 빠르게 조회할 수 있도록 딕셔너리로 다시 구성합니다.
		private void BuildLookup()
		{
			soundLookup = new Dictionary<AudioSoundId, AudioSoundEntry>();

			foreach (AudioSoundEntry sound in sounds)
			{
				if (sound == null || sound.Id == AudioSoundId.None)
				{
					continue;
				}

				soundLookup[sound.Id] = sound;
			}
		}
	}
}
