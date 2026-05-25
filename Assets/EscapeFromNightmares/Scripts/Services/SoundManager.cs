using UnityEngine;
using UnityEngine.Audio;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class SoundManager : MonoBehaviour
    {
        public const string MasterVolumeParameter = "MasterVolume";
        public const string BgmVolumeParameter = "BgmVolume";
        public const string SfxVolumeParameter = "SfxVolume";
        public const string UiVolumeParameter = "UiVolume";

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup bgmGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup uiGroup;
        [SerializeField] private bool useExposedMixerParameters;

        private AudioSource bgmSource;
        private AudioSource sfxSource;
        private AudioSource uiSource;
        private ResourceManager resourceManager;
        private bool masterMixerFailed;
        private bool bgmMixerFailed;
        private bool sfxMixerFailed;
        private bool uiMixerFailed;

        public void Initialize(ResourceManager resources, AudioMixer mixer = null)
        {
            resourceManager = resources;
            if (mixer != null)
            {
                audioMixer = mixer;
            }

            ResolveMixerGroups();
            ResetMixerFailures();
            EnsureSources();
        }

        public void ApplyVolumes(SettingsSaveService.SettingsData settings)
        {
            if (settings == null)
            {
                return;
            }

            EnsureSources();
            bgmSource.volume = Mathf.Clamp01(settings.masterVolume * settings.bgmVolume);
            sfxSource.volume = Mathf.Clamp01(settings.masterVolume * settings.sfxVolume);
            uiSource.volume = Mathf.Clamp01(settings.masterVolume * settings.uiVolume);

            if (useExposedMixerParameters && audioMixer != null)
            {
                TrySetMixerVolume(MasterVolumeParameter, settings.masterVolume, ref masterMixerFailed);
                TrySetMixerVolume(BgmVolumeParameter, settings.bgmVolume, ref bgmMixerFailed);
                TrySetMixerVolume(SfxVolumeParameter, settings.sfxVolume, ref sfxMixerFailed);
                TrySetMixerVolume(UiVolumeParameter, settings.uiVolume, ref uiMixerFailed);
            }
        }

        public void PlayBgm(string path, bool loop = true)
        {
            EnsureSources();
            var clip = resourceManager?.LoadAudioClip(path);
            if (clip == null)
            {
                return;
            }

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBgm()
        {
            EnsureSources();
            bgmSource.Stop();
            bgmSource.clip = null;
        }

        public void PlaySfx(string path)
        {
            EnsureSources();
            PlayOneShot(sfxSource, path);
        }

        public void PlayUi(string path)
        {
            EnsureSources();
            PlayOneShot(uiSource, path);
        }

        public void Play(SoundEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            switch (entry.category)
            {
                case SoundCategory.Bgm:
                case SoundCategory.Ambience:
                    PlayBgm(entry.resourcePath, entry.loop);
                    break;
                case SoundCategory.Ui:
                    PlayUi(entry.resourcePath);
                    break;
                case SoundCategory.Sfx:
                case SoundCategory.Monster:
                    PlaySfx(entry.resourcePath);
                    break;
            }
        }

        public static float VolumeToDecibels(float normalizedVolume)
        {
            var clamped = Mathf.Clamp01(normalizedVolume);
            return clamped <= 0.0001f ? -80f : Mathf.Log10(clamped) * 20f;
        }

        private void PlayOneShot(AudioSource source, string path)
        {
            EnsureSources();
            var clip = resourceManager?.LoadAudioClip(path);
            if (clip != null)
            {
                source.PlayOneShot(clip);
            }
        }

        private void EnsureSources()
        {
            ResolveMixerGroups();
            if (bgmSource == null)
            {
                bgmSource = CreateSource("BGM Source", bgmGroup, true);
            }

            if (sfxSource == null)
            {
                sfxSource = CreateSource("SFX Source", sfxGroup, false);
            }

            if (uiSource == null)
            {
                uiSource = CreateSource("UI Source", uiGroup, false);
            }
        }

        private AudioSource CreateSource(string sourceName, AudioMixerGroup group, bool loop)
        {
            var child = new GameObject(sourceName);
            child.transform.SetParent(transform, false);
            var source = child.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = group;
            source.loop = loop;
            source.playOnAwake = false;
            return source;
        }

        private void ResolveMixerGroups()
        {
            if (audioMixer == null)
            {
                return;
            }

            bgmGroup = bgmGroup != null ? bgmGroup : FindMixerGroup("BGM");
            sfxGroup = sfxGroup != null ? sfxGroup : FindMixerGroup("SFX");
            uiGroup = uiGroup != null ? uiGroup : FindMixerGroup("UI");

            if (bgmSource != null)
            {
                bgmSource.outputAudioMixerGroup = bgmGroup;
            }

            if (sfxSource != null)
            {
                sfxSource.outputAudioMixerGroup = sfxGroup;
            }

            if (uiSource != null)
            {
                uiSource.outputAudioMixerGroup = uiGroup;
            }
        }

        private AudioMixerGroup FindMixerGroup(string groupName)
        {
            var groups = audioMixer.FindMatchingGroups(groupName);
            return groups != null && groups.Length > 0 ? groups[0] : null;
        }

        private void TrySetMixerVolume(string parameterName, float value, ref bool failed)
        {
            if (failed)
            {
                return;
            }

            failed = !audioMixer.SetFloat(parameterName, VolumeToDecibels(value));
        }

        private void ResetMixerFailures()
        {
            masterMixerFailed = false;
            bgmMixerFailed = false;
            sfxMixerFailed = false;
            uiMixerFailed = false;
        }
    }
}
