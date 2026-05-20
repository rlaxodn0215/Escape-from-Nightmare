using EscapeFromNightmares.Core;
using EscapeFromNightmares.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class SettingsUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/SettingsUI.prefab";
        private const string PanelSpritePath = "Assets/Sprites/UI/ui_puzzle_panel.png";

        [MenuItem("Escape From Nightmares/Seed Settings UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildSettingsUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            GameObject systems = GameObject.Find("Systems");
            if (canvas == null || systems == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas or Systems.");
                return;
            }

            GameObject existing = FindChild(canvas, "SettingsUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "SettingsUI";
            ConfigureSceneInstance(instance, systems.GetComponent<SaveManager>());
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded SettingsUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildSettingsUI()
        {
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);

            GameObject root = CreateRectObject("SettingsUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            SettingsUI settingsUI = root.AddComponent<SettingsUI>();
            Image panelImage = root.AddComponent<Image>();
            panelImage.sprite = panelSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(560f, 330f);
            rootRect.anchoredPosition = Vector2.zero;

            TMP_Text title = CreateLabel("Title", root.transform, new Vector2(0f, 116f), new Vector2(320f, 36f), 26f);
            title.text = "Settings";

            TMP_Text bgmLabel = CreateLabel("BgmLabel", root.transform, new Vector2(-170f, 44f), new Vector2(100f, 30f), 19f);
            bgmLabel.text = "BGM";
            TMP_Text sfxLabel = CreateLabel("SfxLabel", root.transform, new Vector2(-170f, -32f), new Vector2(100f, 30f), 19f);
            sfxLabel.text = "SFX";

            Slider bgmSlider = CreateSlider("BgmSlider", root.transform, new Vector2(25f, 44f));
            Slider sfxSlider = CreateSlider("SfxSlider", root.transform, new Vector2(25f, -32f));

            TMP_Text bgmValue = CreateLabel("BgmValue", root.transform, new Vector2(220f, 44f), new Vector2(70f, 30f), 18f);
            bgmValue.text = "80";
            TMP_Text sfxValue = CreateLabel("SfxValue", root.transform, new Vector2(220f, -32f), new Vector2(70f, 30f), 18f);
            sfxValue.text = "80";

            Button closeButton = CreateCloseButton(root.transform);

            SerializedObject serialized = new SerializedObject(settingsUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("closeButton").objectReferenceValue = closeButton;
            serialized.FindProperty("bgmSlider").objectReferenceValue = bgmSlider;
            serialized.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
            serialized.FindProperty("bgmValueLabel").objectReferenceValue = bgmValue;
            serialized.FindProperty("sfxValueLabel").objectReferenceValue = sfxValue;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        private static Slider CreateSlider(string name, Transform parent, Vector2 position)
        {
            GameObject root = CreateRectObject(name, parent);
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(300f, 26f);
            rootRect.anchoredPosition = position;

            Slider slider = root.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.8f;
            slider.transition = Selectable.Transition.None;

            GameObject background = CreateRectObject("Background", root.transform);
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 0.25f);
            backgroundRect.anchorMax = new Vector2(1f, 0.75f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.color = new Color(0.08f, 0.08f, 0.08f, 0.9f);

            GameObject fillArea = CreateRectObject("Fill Area", root.transform);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(6f, 0f);
            fillAreaRect.offsetMax = new Vector2(-6f, 0f);

            GameObject fill = CreateRectObject("Fill", fillArea.transform);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.5f, 0.05f, 0.08f, 0.95f);

            GameObject handleArea = CreateRectObject("Handle Slide Area", root.transform);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10f, 0f);
            handleAreaRect.offsetMax = new Vector2(-10f, 0f);

            GameObject handle = CreateRectObject("Handle", handleArea.transform);
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(18f, 28f);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = new Color(0.82f, 0.82f, 0.82f, 1f);

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            return slider;
        }

        private static Button CreateCloseButton(Transform parent)
        {
            GameObject close = CreateRectObject("CloseButton", parent);
            RectTransform rect = close.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-32f, -32f);
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

        private static void ConfigureSceneInstance(GameObject instance, SaveManager saveManager)
        {
            SettingsUI settingsUI = instance.GetComponent<SettingsUI>();
            if (settingsUI == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(settingsUI);
            serialized.FindProperty("saveManager").objectReferenceValue = saveManager;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(settingsUI);
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
