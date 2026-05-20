using UnityEngine;

namespace EscapeFromNightmares.Core
{
    public sealed class SaveManager : MonoBehaviour
    {
        private const string BgmVolumeKey = "settings_bgm_volume";
        private const string SfxVolumeKey = "settings_sfx_volume";
        private const string Stage1ClearKey = "stage1_clear";

        [Range(0f, 1f)]
        [SerializeField] private float bgmVolume = 0.8f;

        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 0.8f;

        [SerializeField] private bool stage1Clear;

        public float BgmVolume => bgmVolume;
        public float SfxVolume => sfxVolume;
        public bool Stage1Clear => stage1Clear;

        public void Load()
        {
            bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, bgmVolume);
            sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, sfxVolume);
            stage1Clear = PlayerPrefs.GetInt(Stage1ClearKey, 0) == 1;
        }

        public void SaveSettings(float nextBgmVolume, float nextSfxVolume)
        {
            bgmVolume = Mathf.Clamp01(nextBgmVolume);
            sfxVolume = Mathf.Clamp01(nextSfxVolume);

            PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
            PlayerPrefs.Save();
        }

        public void SaveStage1Clear()
        {
            stage1Clear = true;
            PlayerPrefs.SetInt(Stage1ClearKey, 1);
            PlayerPrefs.Save();
        }
    }
}
