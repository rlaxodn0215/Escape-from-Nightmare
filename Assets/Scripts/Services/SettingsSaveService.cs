using System;
using System.IO;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 허용된 런타임 저장 파일인 settings.json과 clear_records.json만 읽고 씁니다.
    /// </summary>
    public sealed class SettingsSaveService
    {
        /// <summary>
        /// 오디오 설정 화면에서 조정하는 볼륨 값입니다. 모든 값은 0~1 범위로 사용합니다.
        /// </summary>
        [Serializable]
        public sealed class SettingsData
        {
            public float masterVolume = 0.8f;
            public float bgmVolume = 0.8f;
            public float sfxVolume = 0.8f;
            public float uiVolume = 0.8f;
        }

        /// <summary>
        /// 스테이지 클리어 여부처럼 진행 저장이 아닌 기록성 데이터만 담습니다.
        /// </summary>
        [Serializable]
        public sealed class ClearRecordsData
        {
            public bool stage1Clear;
        }

        private readonly string settingsPath;
        private readonly string recordsPath;

        /// <summary>
        /// 저장 루트 경로를 기준으로 허용된 두 저장 파일 경로를 구성합니다.
        /// </summary>
        public SettingsSaveService(string rootPath)
        {
            settingsPath = Path.Combine(rootPath, "settings.json");
            recordsPath = Path.Combine(rootPath, "clear_records.json");
        }

        /// <summary>settings.json을 읽고, 없으면 기본 설정을 반환합니다.</summary>
        public SettingsData LoadSettings()
        {
            return Load(settingsPath, new SettingsData());
        }

        /// <summary>settings.json에 오디오 설정을 저장합니다.</summary>
        public void SaveSettings(SettingsData data)
        {
            Save(settingsPath, data);
        }

        /// <summary>clear_records.json을 읽고, 없으면 기본 기록을 반환합니다.</summary>
        public ClearRecordsData LoadClearRecords()
        {
            return Load(recordsPath, new ClearRecordsData());
        }

        /// <summary>clear_records.json에 클리어 기록을 저장합니다.</summary>
        public void SaveClearRecords(ClearRecordsData data)
        {
            Save(recordsPath, data);
        }

        private static T Load<T>(string path, T fallback)
        {
            if (!File.Exists(path))
            {
                return fallback;
            }

            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        private static void Save<T>(string path, T data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
        }
    }
}
