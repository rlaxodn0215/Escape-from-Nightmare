using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
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

	public enum AudioChannel
	{
		Bgm = 0,
		Sfx = 1,
		Ui = 2
	}

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

	[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Escape From Nightmare/Audio Database")]
	public class AudioDatabase : ScriptableObject
	{
		[SerializeField] private List<AudioSoundEntry> sounds = new();

		private Dictionary<AudioSoundId, AudioSoundEntry> soundLookup;

		public IReadOnlyList<AudioSoundEntry> Sounds => sounds;

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
