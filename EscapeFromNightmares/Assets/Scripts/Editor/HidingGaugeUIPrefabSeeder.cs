using EscapeFromNightmares.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class HidingGaugeUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/HidingGaugeUI.prefab";
        private const string GaugeBackgroundSpritePath = "Assets/Sprites/UI/ui_hiding_danger_gauge_bg.png";
        private const string GaugeFillSpritePath = "Assets/Sprites/UI/ui_hiding_danger_gauge_fill.png";

        [MenuItem("Escape From Nightmares/Seed Hiding Gauge UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildHidingGaugeUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            if (canvas == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas.");
                return;
            }

            GameObject existing = FindChild(canvas, "HidingGaugeUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "HidingGaugeUI";
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded HidingGaugeUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildHidingGaugeUI()
        {
            GameObject root = CreateRectObject("HidingGaugeUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            HidingGaugeUI hidingGaugeUI = root.AddComponent<HidingGaugeUI>();

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0f);
            rootRect.anchorMax = new Vector2(0.5f, 0f);
            rootRect.pivot = new Vector2(0.5f, 0f);
            rootRect.sizeDelta = new Vector2(360f, 82f);
            rootRect.anchoredPosition = new Vector2(0f, 32f);

            Image backgroundImage = CreateImage("GaugeBackground", root.transform, GaugeBackgroundSpritePath, Vector2.zero, new Vector2(320f, 28f));
            backgroundImage.color = Color.white;

            Image fillImage = CreateImage("GaugeFill", backgroundImage.transform, GaugeFillSpritePath, Vector2.zero, new Vector2(320f, 28f));
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
            fillImage.fillAmount = 0f;
            fillImage.color = Color.white;

            TMP_Text label = CreateLabel("Label", root.transform, new Vector2(0f, 38f), new Vector2(280f, 24f), 16f);
            label.text = "Stay still";

            SerializedObject serialized = new SerializedObject(hidingGaugeUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("gaugeFillImage").objectReferenceValue = fillImage;
            serialized.FindProperty("label").objectReferenceValue = label;
            serialized.FindProperty("danger01").floatValue = 0f;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        private static Image CreateImage(string name, Transform parent, string spritePath, Vector2 position, Vector2 size)
        {
            GameObject root = CreateRectObject(name, parent);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            Image image = root.AddComponent<Image>();
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            image.preserveAspect = false;
            image.raycastTarget = false;
            return image;
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
            text.color = new Color(0.72f, 0.72f, 0.72f, 1f);
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
    }
}
