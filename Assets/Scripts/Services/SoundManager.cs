using UnityEngine;
using UnityEngine.Audio;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// BGM, SFX, UI 사운드용 AudioSource를 관리하고 저장된 볼륨 설정을 적용합니다.
    /// </summary>
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

        /// <summary>
        /// 리소스 매니저와 선택적 AudioMixer를 연결하고 재생용 AudioSource를 준비합니다.
        /// </summary>
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

        /// <summary>
        /// 저장된 볼륨 설정을 AudioSource 볼륨과 노출된 Mixer 파라미터에 반영합니다.
        /// </summary>
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

        /// <summary>지정한 Resources 경로의 BGM을 로드해 재생합니다.</summary>
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

        /// <summary>현재 재생 중인 BGM을 정지하고 클립 참조를 비웁니다.</summary>
        public void StopBgm()
        {
            EnsureSources();
            bgmSource.Stop();
            bgmSource.clip = null;
        }

        /// <summary>지정한 Resources 경로의 SFX를 한 번 재생합니다.</summary>
        public void PlaySfx(string path)
        {
            EnsureSources();
            PlayOneShot(sfxSource, path);
        }

        /// <summary>지정한 Resources 경로의 UI 사운드를 한 번 재생합니다.</summary>
        public void PlayUi(string path)
        {
            EnsureSources();
            PlayOneShot(uiSource, path);
        }

        /// <summary>
        /// SoundEntry의 카테고리에 따라 BGM, UI, SFX 재생 경로로 분기합니다.
        /// </summary>
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

        /// <summary>
        /// 0~1 범위 볼륨 값을 AudioMixer에서 사용하는 데시벨 값으로 변환합니다.
        /// </summary>
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
