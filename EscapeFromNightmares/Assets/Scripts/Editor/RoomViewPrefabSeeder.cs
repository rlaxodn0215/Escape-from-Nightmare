using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class RoomViewPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/RoomView.prefab";

        [MenuItem("Escape From Nightmares/Seed Room View Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildRoomView();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            if (systems == null)
            {
                Debug.LogError("Stage1 scene is missing Systems.");
                return;
            }

            GameObject existing = GameObject.Find("RoomView");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "RoomView";
            instance.transform.SetSiblingIndex(GetRootInsertIndex(scene));

            RoomViewPresenter presenter = instance.GetComponent<RoomViewPresenter>();
            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            if (roomSystem != null)
            {
                SetObjectReference(roomSystem, "roomViewPresenter", presenter);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded RoomView prefab and Stage1 scene instance.");
        }

        private static GameObject BuildRoomView()
        {
            GameObject root = new GameObject("RoomView");
            RoomViewPresenter presenter = root.AddComponent<RoomViewPresenter>();

            SpriteRenderer backgroundRenderer = CreateLayer("Background", root.transform, 0);
            SpriteRenderer midgroundRenderer = CreateLayer("MidgroundLayer", root.transform, 1);
            SpriteRenderer foregroundRenderer = CreateLayer("ForegroundLayer", root.transform, 2);
            SpriteRenderer effectRenderer = CreateLayer("EffectLayer", root.transform, 3);

            SerializedObject serialized = new SerializedObject(presenter);
            serialized.FindProperty("backgroundRenderer").objectReferenceValue = backgroundRenderer;
            SerializedProperty layerArray = serialized.FindProperty("visualLayerRenderers");
            layerArray.arraySize = 3;
            layerArray.GetArrayElementAtIndex(0).objectReferenceValue = midgroundRenderer;
            layerArray.GetArrayElementAtIndex(1).objectReferenceValue = foregroundRenderer;
            layerArray.GetArrayElementAtIndex(2).objectReferenceValue = effectRenderer;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static SpriteRenderer CreateLayer(string name, Transform parent, int sortingOrder)
        {
            GameObject layer = new GameObject(name);
            layer.transform.SetParent(parent, false);
            SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = sortingOrder;
            renderer.drawMode = SpriteDrawMode.Simple;
            renderer.enabled = false;
            return renderer;
        }

        private static int GetRootInsertIndex(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                if (roots[index].name == "Systems")
                {
                    return index + 1;
                }
            }

            return roots.Length;
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogError($"Missing serialized property '{propertyName}' on {target.name}.");
                return;
            }

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
