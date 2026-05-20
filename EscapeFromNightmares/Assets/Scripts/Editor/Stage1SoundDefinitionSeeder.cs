using System.IO;
using EscapeFromNightmares.Data;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1SoundDefinitionSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Sound Definitions";
        private const string AutoSeedSessionKey = "EscapeFromNightmares.Stage1SoundDefinitionSeeder.AutoSeeded";
        private const string AudioRoot = "Assets/Audio";
        private const string OutputRoot = "Assets/ScriptableObjects/Stage1/Sounds";
        private const string StageDefinitionPath = "Assets/ScriptableObjects/Stage1/Stage1Definition.asset";
        private static readonly string[] AudioSearchFolders =
        {
            $"{AudioRoot}/BGM",
            $"{AudioRoot}/Ambience",
            $"{AudioRoot}/Events",
            $"{AudioRoot}/Monster",
            $"{AudioRoot}/SFX",
            $"{AudioRoot}/UI"
        };

        [InitializeOnLoadMethod]
        private static void AutoSeedAfterReload()
        {
            if (SessionState.GetBool(AutoSeedSessionKey, false))
            {
                return;
            }

            SessionState.SetBool(AutoSeedSessionKey, true);
            EditorApplication.delayCall += Seed;
        }

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            Directory.CreateDirectory(OutputRoot);

            string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", AudioSearchFolders);
            var soundDefinitions = new SoundDefinition[audioGuids.Length];

            for (int i = 0; i < audioGuids.Length; i++)
            {
                string audioPath = AssetDatabase.GUIDToAssetPath(audioGuids[i]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);
                if (clip == null)
                {
                    continue;
                }

                string soundId = Path.GetFileNameWithoutExtension(audioPath);
                string folder = GetOutputFolder(audioPath);
                Directory.CreateDirectory(folder);

                string assetPath = $"{folder}/{soundId}.asset";
                SoundDefinition definition = AssetDatabase.LoadAssetAtPath<SoundDefinition>(assetPath);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<SoundDefinition>();
                    AssetDatabase.CreateAsset(definition, assetPath);
                }

                SerializedObject serialized = new SerializedObject(definition);
                serialized.FindProperty("soundId").stringValue = soundId;
                serialized.FindProperty("category").enumValueIndex = (int)GetCategory(audioPath);
                serialized.FindProperty("clip").objectReferenceValue = clip;
                serialized.FindProperty("loop").boolValue = audioPath.Contains("/BGM/") || audioPath.Contains("/Ambience/");
                serialized.FindProperty("volume").floatValue = audioPath.Contains("/BGM/") ? 0.7f : 1f;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(definition);

                soundDefinitions[i] = definition;
            }

            StageDefinition stageDefinition = AssetDatabase.LoadAssetAtPath<StageDefinition>(StageDefinitionPath);
            if (stageDefinition != null)
            {
                SerializedObject serializedStage = new SerializedObject(stageDefinition);
                SerializedProperty sounds = serializedStage.FindProperty("sounds");
                sounds.arraySize = soundDefinitions.Length;

                for (int i = 0; i < soundDefinitions.Length; i++)
                {
                    sounds.GetArrayElementAtIndex(i).objectReferenceValue = soundDefinitions[i];
                }

                serializedStage.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(stageDefinition);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Seeded {audioGuids.Length} Stage 1 sound definitions.");
        }

        private static string GetOutputFolder(string audioPath)
        {
            if (audioPath.Contains("/BGM/"))
            {
                return $"{OutputRoot}/BGM";
            }

            if (audioPath.Contains("/Ambience/"))
            {
                return $"{OutputRoot}/Ambience";
            }

            if (audioPath.Contains("/Monster/"))
            {
                return $"{OutputRoot}/Monster";
            }

            if (audioPath.Contains("/UI/"))
            {
                return $"{OutputRoot}/UI";
            }

            if (audioPath.Contains("/Events/"))
            {
                return $"{OutputRoot}/Events";
            }

            return $"{OutputRoot}/SFX";
        }

        private static AudioCategory GetCategory(string audioPath)
        {
            if (audioPath.Contains("/BGM/"))
            {
                return AudioCategory.Bgm;
            }

            if (audioPath.Contains("/Ambience/"))
            {
                return AudioCategory.Ambience;
            }

            if (audioPath.Contains("/Monster/"))
            {
                return AudioCategory.Monster;
            }

            if (audioPath.Contains("/UI/"))
            {
                return AudioCategory.Ui;
            }

            if (audioPath.Contains("/Events/"))
            {
                return AudioCategory.Event;
            }

            return AudioCategory.Sfx;
        }
    }
}
