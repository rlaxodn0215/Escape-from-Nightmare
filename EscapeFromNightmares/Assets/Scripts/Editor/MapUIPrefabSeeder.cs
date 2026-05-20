using EscapeFromNightmares.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class MapUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/MapUI.prefab";
        private const string PanelSpritePath = "Assets/Sprites/UI/ui_map_panel.png";
        private const string MarkerSpritePath = "Assets/Sprites/UI/ui_map_current_room_marker.png";

        private static readonly string[] FloorSpritePaths =
        {
            "Assets/Sprites/UI/ui_map_floor_1f.png",
            "Assets/Sprites/UI/ui_map_floor_2f.png",
            "Assets/Sprites/UI/ui_map_floor_basement.png",
            "Assets/Sprites/UI/ui_map_floor_attic.png"
        };

        private static readonly string[] FloorNames =
        {
            "1F",
            "2F",
            "B1",
            "Attic"
        };

        [MenuItem("Escape From Nightmares/Seed Map UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildMapUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            if (canvas == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas.");
                return;
            }

            GameObject existing = FindChild(canvas, "MapUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "MapUI";
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded MapUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildMapUI()
        {
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
            Sprite markerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MarkerSpritePath);

            GameObject root = CreateRectObject("MapUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            MapUI mapUI = root.AddComponent<MapUI>();
            Image panelImage = root.AddComponent<Image>();
            panelImage.sprite = panelSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(820f, 560f);
            rootRect.anchoredPosition = Vector2.zero;

            TMP_Text floorLabel = CreateLabel("FloorLabel", root.transform, new Vector2(0f, 230f), new Vector2(420f, 36f), 24f);
            Image[] floorImages = new Image[FloorSpritePaths.Length];
            Button[] floorButtons = new Button[FloorSpritePaths.Length];

            for (int index = 0; index < FloorSpritePaths.Length; index++)
            {
                GameObject floor = CreateRectObject($"FloorImage{index + 1}", root.transform);
                RectTransform floorRect = floor.GetComponent<RectTransform>();
                floorRect.anchorMin = new Vector2(0.5f, 0.5f);
                floorRect.anchorMax = new Vector2(0.5f, 0.5f);
                floorRect.sizeDelta = new Vector2(640f, 400f);
                floorRect.anchoredPosition = new Vector2(0f, -8f);

                Image floorImage = floor.AddComponent<Image>();
                floorImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(FloorSpritePaths[index]);
                floorImage.preserveAspect = true;
                floorImage.raycastTarget = false;
                floorImage.enabled = index == 0;
                floorImages[index] = floorImage;

                Button floorButton = CreateFloorButton(root.transform, FloorNames[index], index);
                floorButtons[index] = floorButton;
            }

            GameObject marker = CreateRectObject("CurrentRoomMarker", root.transform);
            RectTransform markerRect = marker.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0.5f, 0.5f);
            markerRect.anchorMax = new Vector2(0.5f, 0.5f);
            markerRect.sizeDelta = new Vector2(28f, 28f);
            markerRect.anchoredPosition = new Vector2(-230f, -70f);
            Image markerImage = marker.AddComponent<Image>();
            markerImage.sprite = markerSprite;
            markerImage.preserveAspect = true;
            markerImage.raycastTarget = false;

            Button closeButton = CreateCloseButton(root.transform);

            SerializedObject serialized = new SerializedObject(mapUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("closeButton").objectReferenceValue = closeButton;
            SetArray(serialized.FindProperty("floorButtons"), floorButtons);
            SetArray(serialized.FindProperty("floorImages"), floorImages);
            serialized.FindProperty("currentRoomMarker").objectReferenceValue = markerImage;
            serialized.FindProperty("floorLabel").objectReferenceValue = floorLabel;
            SerializedProperty floorNames = serialized.FindProperty("floorNames");
            floorNames.arraySize = FloorNames.Length;
            for (int index = 0; index < FloorNames.Length; index++)
            {
                floorNames.GetArrayElementAtIndex(index).stringValue = FloorNames[index];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        private static Button CreateFloorButton(Transform parent, string labelText, int index)
        {
            GameObject buttonObject = CreateRectObject($"FloorButton{labelText}", parent);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(70f, 34f);
            rect.anchoredPosition = new Vector2(95f + index * 78f, -78f);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.05f, 0.05f, 0.05f, 0.82f);

            Button button = buttonObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            TMP_Text label = CreateLabel("Label", buttonObject.transform, Vector2.zero, new Vector2(64f, 28f), 18f);
            label.text = labelText;
            return button;
        }

        private static Button CreateCloseButton(Transform parent)
        {
            GameObject close = CreateRectObject("CloseButton", parent);
            RectTransform rect = close.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-34f, -34f);
            rect.sizeDelta = new Vector2(38f, 38f);

            Image image = close.AddComponent<Image>();
            image.color = new Color(0.04f, 0.04f, 0.04f, 0.72f);

            Button button = close.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            TMP_Text label = CreateLabel("Label", close.transform, Vector2.zero, new Vector2(32f, 32f), 22f);
            label.text = "X";
            return button;
        }

        private static TMP_Text CreateLabel(string name, Transform parent, Vector2 position, Vector2 size, float fontSize)
        {
            GameObject label = CreateRectObject(name, parent);
            RectTransform rect = label.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI text = label.AddComponent<TextMeshProUGUI>();
            text.text = "";
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.82f, 0.82f, 0.82f, 1f);
            text.raycastTarget = false;
            return text;
        }

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            if (parent != null)
            {
                gameObject.transform.SetParent(parent, false);
            }

            return gameObject;
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

        private static void SetArray<T>(SerializedProperty property, T[] values) where T : Object
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }
    }
}
