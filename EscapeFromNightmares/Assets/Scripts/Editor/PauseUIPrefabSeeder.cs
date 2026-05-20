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
    public static class PauseUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/PauseUI.prefab";
        private const string ContinueSpritePath = "Assets/Sprites/UI/ui_button_continue.png";
        private const string SettingsSpritePath = "Assets/Sprites/UI/ui_button_settings.png";
        private const string ReturnTitleSpritePath = "Assets/Sprites/UI/ui_button_return_title.png";
        private const string PanelSpritePath = "Assets/Sprites/UI/ui_puzzle_panel.png";

        [MenuItem("Escape From Nightmares/Seed Pause UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildPauseUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            GameObject systems = GameObject.Find("Systems");
            GameObject settingsObject = FindChild(canvas, "SettingsUI");
            GameObject titleObject = FindChild(canvas, "TitleUI");
            if (canvas == null || systems == null || settingsObject == null || titleObject == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas, Systems, SettingsUI, or TitleUI.");
                return;
            }

            GameObject existing = FindChild(canvas, "PauseUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "PauseUI";
            ConfigureSceneInstance(
                instance,
                systems.GetComponent<GameStateManager>(),
                settingsObject.GetComponent<SettingsUI>(),
                titleObject.GetComponent<TitleUI>());
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded PauseUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildPauseUI()
        {
            GameObject root = CreateRectObject("PauseUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            PauseUI pauseUI = root.AddComponent<PauseUI>();
            Image panelImage = root.AddComponent<Image>();
            panelImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(500f, 380f);
            rootRect.anchoredPosition = Vector2.zero;

            TMP_Text titleLabel = CreateLabel("TitleLabel", root.transform, new Vector2(0f, 130f), new Vector2(280f, 40f), 28f);
            titleLabel.text = "Pause";

            Button continueButton = CreateSpriteButton("ContinueButton", root.transform, ContinueSpritePath, new Vector2(0f, 42f), new Vector2(260f, 66f));
            Button settingsButton = CreateSpriteButton("SettingsButton", root.transform, SettingsSpritePath, new Vector2(0f, -42f), new Vector2(260f, 66f));
            Button returnTitleButton = CreateSpriteButton("ReturnTitleButton", root.transform, ReturnTitleSpritePath, new Vector2(0f, -126f), new Vector2(260f, 66f));

            SerializedObject serialized = new SerializedObject(pauseUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("continueButton").objectReferenceValue = continueButton;
            serialized.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            serialized.FindProperty("returnTitleButton").objectReferenceValue = returnTitleButton;
            serialized.FindProperty("titleLabel").objectReferenceValue = titleLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        private static Button CreateSpriteButton(string name, Transform parent, string spritePath, Vector2 position, Vector2 size)
        {
            GameObject root = CreateRectObject(name, parent);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            Image image = root.AddComponent<Image>();
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            image.preserveAspect = true;

            Button button = root.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
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

        private static void ConfigureSceneInstance(GameObject instance, GameStateManager gameStateManager, SettingsUI settingsUI, TitleUI titleUI)
        {
            PauseUI pauseUI = instance.GetComponent<PauseUI>();
            if (pauseUI == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(pauseUI);
            serialized.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serialized.FindProperty("settingsUI").objectReferenceValue = settingsUI;
            serialized.FindProperty("titleUI").objectReferenceValue = titleUI;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pauseUI);
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
