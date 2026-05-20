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
    public static class TitleUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/TitleUI.prefab";
        private const string StartSpritePath = "Assets/Sprites/UI/ui_button_start.png";
        private const string SettingsSpritePath = "Assets/Sprites/UI/ui_button_settings.png";
        private const string QuitSpritePath = "Assets/Sprites/UI/ui_button_quit.png";

        [MenuItem("Escape From Nightmares/Seed Title UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildTitleUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            GameObject systems = GameObject.Find("Systems");
            GameObject settingsObject = FindChild(canvas, "SettingsUI");
            if (canvas == null || systems == null || settingsObject == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas, Systems, or UICanvas/SettingsUI.");
                return;
            }

            GameObject existing = FindChild(canvas, "TitleUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "TitleUI";
            ConfigureSceneInstance(
                instance,
                systems.GetComponent<GameStateManager>(),
                systems.GetComponent<SceneFlowController>(),
                settingsObject.GetComponent<SettingsUI>());

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded TitleUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildTitleUI()
        {
            GameObject root = CreateRectObject("TitleUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            TitleUI titleUI = root.AddComponent<TitleUI>();
            Image backdrop = root.AddComponent<Image>();
            backdrop.color = new Color(0f, 0f, 0f, 0.82f);

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            TMP_Text titleLabel = CreateLabel("TitleLabel", root.transform, new Vector2(0f, 168f), new Vector2(720f, 58f), 40f);
            titleLabel.text = "Escape From Nightmares";

            Button startButton = CreateSpriteButton("StartButton", root.transform, StartSpritePath, new Vector2(0f, 44f), new Vector2(260f, 72f));
            Button settingsButton = CreateSpriteButton("SettingsButton", root.transform, SettingsSpritePath, new Vector2(0f, -48f), new Vector2(260f, 72f));
            Button quitButton = CreateSpriteButton("QuitButton", root.transform, QuitSpritePath, new Vector2(0f, -140f), new Vector2(260f, 72f));

            SerializedObject serialized = new SerializedObject(titleUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("startButton").objectReferenceValue = startButton;
            serialized.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            serialized.FindProperty("quitButton").objectReferenceValue = quitButton;
            serialized.FindProperty("titleLabel").objectReferenceValue = titleLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

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

        private static void ConfigureSceneInstance(GameObject instance, GameStateManager gameStateManager, SceneFlowController sceneFlowController, SettingsUI settingsUI)
        {
            TitleUI titleUI = instance.GetComponent<TitleUI>();
            if (titleUI == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(titleUI);
            serialized.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serialized.FindProperty("sceneFlowController").objectReferenceValue = sceneFlowController;
            serialized.FindProperty("settingsUI").objectReferenceValue = settingsUI;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(titleUI);
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
