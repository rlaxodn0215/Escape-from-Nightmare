using System;
using System.IO;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    public sealed class SettingsSaveService
    {
        [Serializable]
        public sealed class SettingsData
        {
            public float masterVolume = 0.8f;
            public float bgmVolume = 0.8f;
            public float sfxVolume = 0.8f;
            public float uiVolume = 0.8f;
        }

        [Serializable]
        public sealed class ClearRecordsData
        {
            public bool stage1Clear;
        }

        private readonly string settingsPath;
        private readonly string recordsPath;

        public SettingsSaveService(string rootPath)
        {
            settingsPath = Path.Combine(rootPath, "settings.json");
            recordsPath = Path.Combine(rootPath, "clear_records.json");
        }

        public SettingsData LoadSettings()
        {
            return Load(settingsPath, new SettingsData());
        }

        public void SaveSettings(SettingsData data)
        {
            Save(settingsPath, data);
        }

        public ClearRecordsData LoadClearRecords()
        {
            return Load(recordsPath, new ClearRecordsData());
        }

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
