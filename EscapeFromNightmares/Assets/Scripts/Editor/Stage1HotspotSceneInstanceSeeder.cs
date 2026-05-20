using System.Collections.Generic;
using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1HotspotSceneInstanceSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Hotspot Scene Instances";
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string InteractableRoot = "Assets/ScriptableObjects/Stage1/Interactables";
        private const string InteractableHotspotPrefabPath = "Assets/Prefabs/Interactables/InteractableHotspot.prefab";
        private const string ScreenEdgeHotspotPrefabPath = "Assets/Prefabs/Interactables/ScreenEdgeHotspot.prefab";
        private const string HideSpotPrefabPath = "Assets/Prefabs/Interactables/HideSpot.prefab";
        private const string HotspotCanvasName = "HotspotCanvas";

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject interactableLayer = GameObject.Find("InteractableLayer");
            GameObject systems = GameObject.Find("Systems");

            if (interactableLayer == null || systems == null)
            {
                Debug.LogError("Stage1 scene is missing InteractableLayer or Systems.");
                return;
            }

            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();
            if (roomSystem == null || interactionSystem == null)
            {
                Debug.LogError("Stage1 scene is missing RoomSystem or InteractionSystem.");
                return;
            }

            GameObject hotspotCanvas = EnsureHotspotCanvas(interactableLayer.transform, roomSystem);
            ClearExistingRoomGroups(hotspotCanvas.transform);

            GameObject interactablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(InteractableHotspotPrefabPath);
            GameObject screenEdgePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ScreenEdgeHotspotPrefabPath);
            GameObject hideSpotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HideSpotPrefabPath);
            if (interactablePrefab == null || screenEdgePrefab == null || hideSpotPrefab == null)
            {
                Debug.LogError("One or more hotspot prefabs are missing.");
                return;
            }

            InteractableDefinition[] definitions = LoadInteractables();
            Dictionary<string, List<InteractableDefinition>> byRoom = definitions
                .Where(definition => definition != null && !string.IsNullOrWhiteSpace(definition.RoomId))
                .GroupBy(definition => definition.RoomId)
                .ToDictionary(group => group.Key, group => group.OrderBy(definition => definition.InteractableId).ToList());

            List<GameObject> roomRoots = new List<GameObject>();
            int instanceCount = 0;
            foreach (KeyValuePair<string, List<InteractableDefinition>> entry in byRoom.OrderBy(pair => pair.Key))
            {
                GameObject roomRoot = CreateRoomRoot(entry.Key, hotspotCanvas.transform);
                roomRoots.Add(roomRoot);

                foreach (InteractableDefinition definition in entry.Value)
                {
                    GameObject prefab = SelectPrefab(definition, interactablePrefab, screenEdgePrefab, hideSpotPrefab);
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, roomRoot.transform);
                    instance.name = definition.InteractableId;
                    ConfigureHotspotInstance(instance, definition, interactionSystem);
                    instanceCount++;
                }

                roomRoot.SetActive(false);
            }

            SetRoomGroups(hotspotCanvas.GetComponent<RoomHotspotLayer>(), roomSystem, roomRoots);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Seeded {instanceCount} Stage1 hotspot scene instances across {roomRoots.Count} room groups.");
        }

        private static GameObject EnsureHotspotCanvas(Transform parent, RoomSystem roomSystem)
        {
            Transform existing = parent.Find(HotspotCanvasName);
            GameObject hotspotCanvas = existing != null ? existing.gameObject : new GameObject(HotspotCanvasName, typeof(RectTransform));
            hotspotCanvas.transform.SetParent(parent, false);

            RectTransform rectTransform = hotspotCanvas.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Canvas canvas = hotspotCanvas.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = hotspotCanvas.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scaler = hotspotCanvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = hotspotCanvas.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (hotspotCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                hotspotCanvas.AddComponent<GraphicRaycaster>();
            }

            RoomHotspotLayer layer = hotspotCanvas.GetComponent<RoomHotspotLayer>();
            if (layer == null)
            {
                layer = hotspotCanvas.AddComponent<RoomHotspotLayer>();
            }

            SerializedObject serializedLayer = new SerializedObject(layer);
            serializedLayer.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            serializedLayer.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(layer);
            return hotspotCanvas;
        }

        private static void ClearExistingRoomGroups(Transform hotspotCanvas)
        {
            for (int i = hotspotCanvas.childCount - 1; i >= 0; i--)
            {
                Transform child = hotspotCanvas.GetChild(i);
                if (child.name.StartsWith("Room_", System.StringComparison.Ordinal))
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        private static InteractableDefinition[] LoadInteractables()
        {
            string[] guids = AssetDatabase.FindAssets("t:InteractableDefinition", new[] { InteractableRoot });
            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<InteractableDefinition>)
                .Where(definition => definition != null)
                .OrderBy(definition => definition.RoomId)
                .ThenBy(definition => definition.InteractableId)
                .ToArray();
        }

        private static GameObject CreateRoomRoot(string roomId, Transform parent)
        {
            GameObject roomRoot = new GameObject($"Room_{roomId}", typeof(RectTransform));
            roomRoot.transform.SetParent(parent, false);

            RectTransform rectTransform = roomRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            return roomRoot;
        }

        private static GameObject SelectPrefab(InteractableDefinition definition, GameObject interactablePrefab, GameObject screenEdgePrefab, GameObject hideSpotPrefab)
        {
            return definition.InteractableType switch
            {
                InteractableType.HideSpot => hideSpotPrefab,
                InteractableType.ScreenEdge => screenEdgePrefab,
                _ => interactablePrefab
            };
        }

        private static void ConfigureHotspotInstance(GameObject instance, InteractableDefinition definition, InteractionSystem interactionSystem)
        {
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                ApplyHitArea(rectTransform, definition.HitArea);
            }

            Image image = instance.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0f, 0f, 0f, 0f);
                image.raycastTarget = true;
            }

            InteractableHotspot hotspot = instance.GetComponent<InteractableHotspot>();
            if (hotspot != null)
            {
                SerializedObject serializedHotspot = new SerializedObject(hotspot);
                serializedHotspot.FindProperty("definition").objectReferenceValue = definition;
                serializedHotspot.FindProperty("interactionSystem").objectReferenceValue = interactionSystem;
                serializedHotspot.FindProperty("raycastImage").objectReferenceValue = image;
                serializedHotspot.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(hotspot);
            }

            HideSpot hideSpot = instance.GetComponent<HideSpot>();
            if (hideSpot != null)
            {
                SerializedObject serializedHideSpot = new SerializedObject(hideSpot);
                serializedHideSpot.FindProperty("hideSpotId").stringValue = definition.InteractableId;
                serializedHideSpot.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(hideSpot);
            }

            ScreenEdgeHotspot edgeHotspot = instance.GetComponent<ScreenEdgeHotspot>();
            if (edgeHotspot != null)
            {
                SerializedObject serializedEdge = new SerializedObject(edgeHotspot);
                serializedEdge.FindProperty("edge").enumValueIndex = (int)GetScreenEdge(definition.HitArea);
                serializedEdge.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(edgeHotspot);
            }
        }

        private static void ApplyHitArea(RectTransform rectTransform, Rect hitArea)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(hitArea.width, hitArea.height);
            rectTransform.anchoredPosition = new Vector2(hitArea.x + hitArea.width * 0.5f - 640f, 360f - hitArea.y - hitArea.height * 0.5f);
        }

        private static ScreenEdge GetScreenEdge(Rect hitArea)
        {
            float leftDistance = hitArea.xMin;
            float rightDistance = 1280f - hitArea.xMax;
            float topDistance = hitArea.yMin;
            float bottomDistance = 720f - hitArea.yMax;
            float minDistance = Mathf.Min(leftDistance, rightDistance, topDistance, bottomDistance);

            if (Mathf.Approximately(minDistance, leftDistance))
            {
                return ScreenEdge.Left;
            }

            if (Mathf.Approximately(minDistance, rightDistance))
            {
                return ScreenEdge.Right;
            }

            if (Mathf.Approximately(minDistance, topDistance))
            {
                return ScreenEdge.Top;
            }

            return ScreenEdge.Bottom;
        }

        private static void SetRoomGroups(RoomHotspotLayer layer, RoomSystem roomSystem, List<GameObject> roomRoots)
        {
            SerializedObject serializedLayer = new SerializedObject(layer);
            serializedLayer.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            SerializedProperty groups = serializedLayer.FindProperty("roomGroups");
            groups.arraySize = roomRoots.Count;

            for (int i = 0; i < roomRoots.Count; i++)
            {
                GameObject root = roomRoots[i];
                string roomId = root.name.StartsWith("Room_", System.StringComparison.Ordinal)
                    ? root.name.Substring("Room_".Length)
                    : root.name;
                SerializedProperty group = groups.GetArrayElementAtIndex(i);
                group.FindPropertyRelative("roomId").stringValue = roomId;
                group.FindPropertyRelative("root").objectReferenceValue = root;
            }

            serializedLayer.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(layer);
        }
    }
}
