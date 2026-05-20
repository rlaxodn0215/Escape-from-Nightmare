using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class InteractableHotspotPrefabSeeder
    {
        private const string PrefabPath = "Assets/Prefabs/Interactables/InteractableHotspot.prefab";

        [MenuItem("Escape From Nightmares/Seed Interactable Hotspot Prefab")]
        public static void Seed()
        {
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Interactables");

            GameObject root = new GameObject("InteractableHotspot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InteractableHotspot));
            RectTransform rectTransform = root.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(160f, 120f);
            rectTransform.anchoredPosition = Vector2.zero;

            Image image = root.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;

            InteractableHotspot hotspot = root.GetComponent<InteractableHotspot>();
            SerializedObject serialized = new SerializedObject(hotspot);
            serialized.FindProperty("raycastImage").objectReferenceValue = image;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded InteractableHotspot prefab.");
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
