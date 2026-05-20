using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1InteractionReferenceSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string InteractableHotspotPrefabPath = "Assets/Prefabs/Interactables/InteractableHotspot.prefab";
        private const string ScreenEdgeHotspotPrefabPath = "Assets/Prefabs/Interactables/ScreenEdgeHotspot.prefab";

        [MenuItem("Escape From Nightmares/Seed Interaction Runtime")]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            if (systems == null)
            {
                Debug.LogError("Stage1 scene is missing Systems.");
                return;
            }

            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();
            if (interactionSystem == null)
            {
                interactionSystem = systems.AddComponent<InteractionSystem>();
            }

            SerializedObject serialized = new SerializedObject(interactionSystem);
            serialized.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(interactionSystem);

            CreateHotspotPrefab(InteractableHotspotPrefabPath, "InteractableHotspot");
            CreateHotspotPrefab(ScreenEdgeHotspotPrefabPath, "ScreenEdgeHotspot");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded interaction runtime and hotspot prefabs.");
        }

        private static void CreateHotspotPrefab(string path, string name)
        {
            GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InteractableHotspot));
            RectTransform rectTransform = root.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(160f, 120f);

            Image image = root.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
