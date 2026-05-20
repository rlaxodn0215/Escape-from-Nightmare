using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class AudioEmitterPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/Audio/AudioEmitter.prefab";

        [MenuItem("Escape From Nightmares/Seed Audio Emitter Prefab")]
        public static void Seed()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Audio");

            GameObject prefabRoot = BuildAudioEmitter();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject audioRoot = GameObject.Find("AudioRoot");
            if (audioRoot == null)
            {
                Debug.LogError("Stage1 scene is missing AudioRoot.");
                return;
            }

            GameObject existing = FindChild(audioRoot, "AudioEmitter");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, audioRoot.transform);
            instance.name = "AudioEmitter";
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded AudioEmitter prefab and Stage1 scene instance.");
        }

        private static GameObject BuildAudioEmitter()
        {
            GameObject root = new GameObject("AudioEmitter");
            AudioSource source = root.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.volume = 1f;

            AudioEmitter emitter = root.AddComponent<AudioEmitter>();
            SerializedObject serialized = new SerializedObject(emitter);
            serialized.FindProperty("audioSource").objectReferenceValue = source;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folder = System.IO.Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parent, folder);
        }

        private static GameObject FindChild(GameObject parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            Transform[] children = parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child != parent.transform && child.name == childName)
                {
                    return child.gameObject;
                }
            }

            return null;
        }
    }
}
