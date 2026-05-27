using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// Manages AudioSources for BGM, SFX, and UI playback with direct AudioClip references.
    /// </summary>
    public sealed class SoundManager : MonoBehaviour
    {
        private AudioSource bgmSource;
        private AudioSource sfxSource;
        private AudioSource uiSource;

        public void Initialize()
        {
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
        }

        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            EnsureSources();
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

        public void PlaySfx(AudioClip clip)
        {
            EnsureSources();
            PlayOneShot(sfxSource, clip);
        }

        public void PlayUi(AudioClip clip)
        {
            EnsureSources();
            PlayOneShot(uiSource, clip);
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
                    PlayBgm(entry.clip, entry.loop);
                    break;
                case SoundCategory.Ui:
                    PlayUi(entry.clip);
                    break;
                case SoundCategory.Sfx:
                case SoundCategory.Monster:
                    PlaySfx(entry.clip);
                    break;
            }
        }

        private void PlayOneShot(AudioSource source, AudioClip clip)
        {
            EnsureSources();
            if (clip != null)
            {
                source.PlayOneShot(clip);
            }
        }

        private void EnsureSources()
        {
            if (bgmSource == null)
            {
                bgmSource = CreateSource("BGM Source", true);
            }

            if (sfxSource == null)
            {
                sfxSource = CreateSource("SFX Source", false);
            }

            if (uiSource == null)
            {
                uiSource = CreateSource("UI Source", false);
            }
        }

        private AudioSource CreateSource(string sourceName, bool loop)
        {
            var child = new GameObject(sourceName);
            child.transform.SetParent(transform, false);
            var source = child.AddComponent<AudioSource>();
            source.loop = loop;
            source.playOnAwake = false;
            return source;
        }
    }
}
