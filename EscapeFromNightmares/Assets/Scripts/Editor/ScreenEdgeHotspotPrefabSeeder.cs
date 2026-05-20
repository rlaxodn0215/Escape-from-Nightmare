using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class ScreenEdgeHotspotPrefabSeeder
    {
        private const string PrefabPath = "Assets/Prefabs/Interactables/ScreenEdgeHotspot.prefab";

        [MenuItem("Escape From Nightmares/Seed Screen Edge Hotspot Prefab")]
        public static void Seed()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Interactables");

            GameObject root = new GameObject(
                "ScreenEdgeHotspot",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(InteractableHotspot),
                typeof(ScreenEdgeHotspot));

            RectTransform rectTransform = root.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1f, 0.5f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f);
            rectTransform.pivot = new Vector2(1f, 0.5f);
            rectTransform.sizeDelta = new Vector2(96f, 720f);
            rectTransform.anchoredPosition = Vector2.zero;

            Image image = root.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;

            InteractableHotspot hotspot = root.GetComponent<InteractableHotspot>();
            SerializedObject hotspotSerialized = new SerializedObject(hotspot);
            hotspotSerialized.FindProperty("raycastImage").objectReferenceValue = image;
            hotspotSerialized.ApplyModifiedPropertiesWithoutUndo();

            ScreenEdgeHotspot screenEdgeHotspot = root.GetComponent<ScreenEdgeHotspot>();
            SerializedObject edgeSerialized = new SerializedObject(screenEdgeHotspot);
            edgeSerialized.FindProperty("edge").enumValueIndex = (int)ScreenEdge.Right;
            edgeSerialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded ScreenEdgeHotspot prefab.");
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
    }
}
