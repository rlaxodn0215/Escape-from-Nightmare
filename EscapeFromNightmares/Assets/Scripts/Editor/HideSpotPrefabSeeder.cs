using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class HideSpotPrefabSeeder
    {
        private const string PrefabPath = "Assets/Prefabs/Interactables/HideSpot.prefab";

        [MenuItem("Escape From Nightmares/Seed Hide Spot Prefab")]
        public static void Seed()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Interactables");

            GameObject root = new GameObject(
                "HideSpot",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(InteractableHotspot),
                typeof(HideSpot));

            RectTransform rectTransform = root.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(220f, 180f);
            rectTransform.anchoredPosition = Vector2.zero;

            Image image = root.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;

            InteractableHotspot hotspot = root.GetComponent<InteractableHotspot>();
            SerializedObject hotspotSerialized = new SerializedObject(hotspot);
            hotspotSerialized.FindProperty("raycastImage").objectReferenceValue = image;
            hotspotSerialized.ApplyModifiedPropertiesWithoutUndo();

            HideSpot hideSpot = root.GetComponent<HideSpot>();
            SerializedObject hideSpotSerialized = new SerializedObject(hideSpot);
            hideSpotSerialized.FindProperty("hideSpotId").stringValue = "hide_spot";
            hideSpotSerialized.FindProperty("recommendedHoldSeconds").floatValue = 6f;
            hideSpotSerialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded HideSpot prefab.");
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
