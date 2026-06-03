using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EscapeFromNightmare
{
	// Controls global audio volume and routes sounds through BGM, SFX, and UI channels.
	public class AudioManager : Singleton<AudioManager>
	{
		private const string MasterVolumeParameter = "MasterVolume";
		private const string BgmVolumeParameter = "BGMVolume";
		private const string SfxVolumeParameter = "SFXVolume";
		private const string UiVolumeParameter = "UIVolume";
		private const float MutedDecibel = -80f;
		private const int OneShotPoolSize = 8;

		[SerializeField] private AudioMixer audioMixer;
		[SerializeField] private AudioDatabase audioDatabase;
		[SerializeField] private AudioMixerGroup masterMixerGroup;
		[SerializeField] private AudioMixerGroup bgmMixerGroup;
		[SerializeField] private AudioMixerGroup sfxMixerGroup;
		[SerializeField] private AudioMixerGroup uiMixerGroup;
		[SerializeField] private AudioSource bgmSource;
		[SerializeField] private AudioSource sfxSource;
		[SerializeField] private AudioSource uiSource;

		private AudioSettingsData audioSettings = AudioSettingsData.Default;
		private readonly List<AudioSource> sfxOneShotSources = new List<AudioSource>();
		private readonly List<AudioSource> uiOneShotSources = new List<AudioSource>();
		private int sfxOneShotIndex;
		private int uiOneShotIndex;
		private float currentBgmVolumeScale = 1f;

		public AudioMixer AudioMixer => audioMixer;
		public AudioDatabase AudioDatabase => audioDatabase;
		public AudioMixerGroup MasterMixerGroup => masterMixerGroup;
		public AudioMixerGroup BgmMixerGroup => bgmMixerGroup;
		public AudioMixerGroup SfxMixerGroup => sfxMixerGroup;
		public AudioMixerGroup UiMixerGroup => uiMixerGroup;

		protected override void Awake()
		{
			base.Awake();

			if (Instance != this)
			{
				return;
			}

			EnsureAudioSources();
			LoadAndApplySettings();
		}

		public float GetMasterVolume()
		{
			return audioSettings.masterVolume;
		}

		public float GetBgmVolume()
		{
			return audioSettings.bgmVolume;
		}

		public float GetSfxVolume()
		{
			return audioSettings.sfxVolume;
		}

		public float GetUiVolume()
		{
			return audioSettings.uiVolume;
		}

		public void SetMasterVolume(float value, bool save = true)
		{
			audioSettings.masterVolume = ClampVolume(value);
			ApplyMixerVolume(MasterVolumeParameter, audioSettings.masterVolume);
			SaveSettingsIfNeeded(save);
		}

		public void SetBgmVolume(float value, bool save = true)
		{
			audioSettings.bgmVolume = ClampVolume(value);
			ApplyMixerVolume(BgmVolumeParameter, audioSettings.bgmVolume);
			SaveSettingsIfNeeded(save);
		}

		public void SetSfxVolume(float value, bool save = true)
		{
			audioSettings.sfxVolume = ClampVolume(value);
			ApplyMixerVolume(SfxVolumeParameter, audioSettings.sfxVolume);
			SaveSettingsIfNeeded(save);
		}

		public void SetUiVolume(float value, bool save = true)
		{
			audioSettings.uiVolume = ClampVolume(value);
			ApplyMixerVolume(UiVolumeParameter, audioSettings.uiVolume);
			SaveSettingsIfNeeded(save);
		}

		public bool Play(AudioSoundId id)
		{
			if (!TryGetSoundEntry(id, out AudioSoundEntry entry))
			{
				return false;
			}

			switch (entry.Channel)
			{
				case AudioChannel.Bgm:
					PlayBgm(entry.Clip, entry.Loop, entry.VolumeScale, entry.Pitch);
					return true;
				case AudioChannel.Sfx:
					PlaySfx(entry.Clip, entry.VolumeScale, entry.Pitch);
					return true;
				case AudioChannel.Ui:
					PlayUi(entry.Clip, entry.VolumeScale, entry.Pitch);
					return true;
				default:
					Debug.LogWarning($"AudioManager cannot play unsupported audio channel '{entry.Channel}' for '{id}'.", this);
					return false;
			}
		}

		public bool PlayBgm(AudioSoundId id)
		{
			return PlayExpectedChannel(id, AudioChannel.Bgm);
		}

		public bool PlaySfx(AudioSoundId id)
		{
			return PlayExpectedChannel(id, AudioChannel.Sfx);
		}

		public bool PlayUi(AudioSoundId id)
		{
			return PlayExpectedChannel(id, AudioChannel.Ui);
		}

		public void PlayBgm(AudioClip clip, bool loop = true)
		{
			PlayBgm(clip, loop, 1f, 1f);
		}

		public void PlaySfx(AudioClip clip)
		{
			PlaySfx(clip, 1f, 1f);
		}

		public void PlayUi(AudioClip clip)
		{
			PlayUi(clip, 1f, 1f);
		}

		private bool PlayExpectedChannel(AudioSoundId id, AudioChannel expectedChannel)
		{
			if (!TryGetSoundEntry(id, out AudioSoundEntry entry))
			{
				return false;
			}

			if (entry.Channel != expectedChannel)
			{
				Debug.LogWarning($"AudioManager expected '{id}' to be {expectedChannel}, but it is configured as {entry.Channel}.", this);
				return false;
			}

			return Play(id);
		}

		private bool TryGetSoundEntry(AudioSoundId id, out AudioSoundEntry entry)
		{
			entry = null;

			if (id == AudioSoundId.None)
			{
				Debug.LogWarning("AudioManager cannot play AudioSoundId.None.", this);
				return false;
			}

			if (audioDatabase == null)
			{
				Debug.LogWarning($"AudioManager has no AudioDatabase assigned for '{id}'.", this);
				return false;
			}

			if (!audioDatabase.TryGetSound(id, out entry))
			{
				Debug.LogWarning($"AudioDatabase does not contain an entry for '{id}'.", audioDatabase);
				return false;
			}

			if (entry.Clip == null)
			{
				Debug.LogWarning($"AudioDatabase entry '{id}' has no AudioClip assigned.", audioDatabase);
				return false;
			}

			return true;
		}

		private void PlayBgm(AudioClip clip, bool loop, float volumeScale, float pitch)
		{
			if (clip == null)
			{
				return;
			}

			EnsureAudioSources();
			currentBgmVolumeScale = Mathf.Max(0f, volumeScale);
			bgmSource.clip = clip;
			bgmSource.loop = loop;
			bgmSource.pitch = Mathf.Max(0.1f, pitch);
			bgmSource.volume = GetBgmSourceVolume();
			bgmSource.Play();
		}

		private void PlaySfx(AudioClip clip, float volumeScale, float pitch)
		{
			if (clip == null)
			{
				return;
			}

			EnsureAudioSources();
			PlayOneShot(sfxOneShotSources, ref sfxOneShotIndex, clip, volumeScale, pitch, GetSfxSourceVolume());
		}

		private void PlayUi(AudioClip clip, float volumeScale, float pitch)
		{
			if (clip == null)
			{
				return;
			}

			EnsureAudioSources();
			PlayOneShot(uiOneShotSources, ref uiOneShotIndex, clip, volumeScale, pitch, GetUiSourceVolume());
		}

		private void LoadAndApplySettings()
		{
			if (SaveManager.Instance != null)
			{
				audioSettings = SaveManager.Instance.LoadAudioSettings();
			}
			else
			{
				audioSettings = AudioSettingsData.Default;
			}

			ApplyAllMixerVolumes();
		}

		private void ApplyAllMixerVolumes()
		{
			ApplyMixerVolume(MasterVolumeParameter, audioSettings.masterVolume);
			ApplyMixerVolume(BgmVolumeParameter, audioSettings.bgmVolume);
			ApplyMixerVolume(SfxVolumeParameter, audioSettings.sfxVolume);
			ApplyMixerVolume(UiVolumeParameter, audioSettings.uiVolume);
			ApplySourceVolumes();
		}

		private void ApplyMixerVolume(string parameterName, float normalizedVolume)
		{
			if (audioMixer == null)
			{
				ApplySourceVolumes();
				return;
			}

			audioMixer.SetFloat(parameterName, ConvertNormalizedVolumeToDecibel(normalizedVolume));
			ApplySourceVolumes();
		}

		private void SaveSettingsIfNeeded(bool save)
		{
			if (!save || SaveManager.Instance == null)
			{
				return;
			}

			SaveManager.Instance.SaveAudioSettings(audioSettings);
		}

		private void EnsureAudioSources()
		{
			bgmSource = EnsureAudioSource(bgmSource, "BGM Audio Source", bgmMixerGroup);
			sfxSource = EnsureAudioSource(sfxSource, "SFX Audio Source", sfxMixerGroup);
			uiSource = EnsureAudioSource(uiSource, "UI Audio Source", uiMixerGroup);
			EnsureOneShotPool(sfxOneShotSources, "SFX One Shot Source", sfxMixerGroup);
			EnsureOneShotPool(uiOneShotSources, "UI One Shot Source", uiMixerGroup);
		}

		private AudioSource EnsureAudioSource(AudioSource source, string sourceName, AudioMixerGroup mixerGroup)
		{
			if (source == null)
			{
				Transform existing = transform.Find(sourceName);
				GameObject sourceObject = existing != null ? existing.gameObject : new GameObject(sourceName);
				sourceObject.transform.SetParent(transform);
				sourceObject.transform.localPosition = Vector3.zero;
				source = sourceObject.GetComponent<AudioSource>();

				if (source == null)
				{
					source = sourceObject.AddComponent<AudioSource>();
				}
			}

			source.playOnAwake = false;
			source.spatialBlend = 0f;
			source.outputAudioMixerGroup = mixerGroup != null ? mixerGroup : masterMixerGroup;
			ApplySourceVolumes();
			return source;
		}

		private void EnsureOneShotPool(List<AudioSource> pool, string sourceNamePrefix, AudioMixerGroup mixerGroup)
		{
			for (int i = 0; i < OneShotPoolSize; i++)
			{
				string sourceName = $"{sourceNamePrefix} {i + 1}";
				AudioSource source = null;
				Transform existing = transform.Find(sourceName);

				if (existing != null)
				{
					source = existing.GetComponent<AudioSource>();
				}

				source = EnsureAudioSource(source, sourceName, mixerGroup);

				if (!pool.Contains(source))
				{
					pool.Add(source);
				}
			}
		}

		private void PlayOneShot(List<AudioSource> pool, ref int poolIndex, AudioClip clip, float volumeScale, float pitch, float sourceVolume)
		{
			AudioSource source = GetAvailableOneShotSource(pool, ref poolIndex);
			if (source == null)
			{
				return;
			}

			source.volume = sourceVolume;
			source.pitch = Mathf.Max(0.1f, pitch);
			source.PlayOneShot(clip, Mathf.Max(0f, volumeScale));
		}

		private AudioSource GetAvailableOneShotSource(List<AudioSource> pool, ref int poolIndex)
		{
			foreach (AudioSource source in pool)
			{
				if (source != null && !source.isPlaying)
				{
					return source;
				}
			}

			if (pool.Count == 0)
			{
				return null;
			}

			poolIndex = (poolIndex + 1) % pool.Count;
			return pool[poolIndex];
		}

		private void ApplySourceVolumes()
		{
			if (bgmSource != null)
			{
				bgmSource.volume = GetBgmSourceVolume();
			}

			if (sfxSource != null)
			{
				sfxSource.volume = GetSfxSourceVolume();
			}

			if (uiSource != null)
			{
				uiSource.volume = GetUiSourceVolume();
			}

			ApplyPoolVolumes(sfxOneShotSources, GetSfxSourceVolume());
			ApplyPoolVolumes(uiOneShotSources, GetUiSourceVolume());
		}

		private void ApplyPoolVolumes(List<AudioSource> pool, float volume)
		{
			foreach (AudioSource source in pool)
			{
				if (source != null)
				{
					source.volume = volume;
				}
			}
		}

		private float GetBgmSourceVolume()
		{
			return audioSettings.masterVolume * audioSettings.bgmVolume * currentBgmVolumeScale;
		}

		private float GetSfxSourceVolume()
		{
			return audioSettings.masterVolume * audioSettings.sfxVolume;
		}

		private float GetUiSourceVolume()
		{
			return audioSettings.masterVolume * audioSettings.uiVolume;
		}

		private static float ClampVolume(float value)
		{
			return Mathf.Clamp01(value);
		}

		private static float ConvertNormalizedVolumeToDecibel(float normalizedVolume)
		{
			if (normalizedVolume <= 0.0001f)
			{
				return MutedDecibel;
			}

			return Mathf.Log10(normalizedVolume) * 20f;
		}
	}
}
