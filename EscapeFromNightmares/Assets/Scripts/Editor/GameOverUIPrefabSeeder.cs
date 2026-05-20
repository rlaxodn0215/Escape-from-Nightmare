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
    public static class GameOverUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/GameOverUI.prefab";
        private const string GameOverTextSpritePath = "Assets/Sprites/UI/ui_gameover_text.png";
        private const string RestartSpritePath = "Assets/Sprites/UI/ui_button_continue.png";
        private const string ReturnTitleSpritePath = "Assets/Sprites/UI/ui_button_return_title.png";

        [MenuItem("Escape From Nightmares/Seed Game Over UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildGameOverUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            GameObject systems = GameObject.Find("Systems");
            GameObject titleObject = FindChild(canvas, "TitleUI");
            if (canvas == null || systems == null || titleObject == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas, Systems, or TitleUI.");
                return;
            }

            GameObject existing = FindChild(canvas, "GameOverUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "GameOverUI";
            ConfigureSceneInstance(
                instance,
                systems.GetComponent<GameStateManager>(),
                titleObject.GetComponent<TitleUI>());
            instance.SetActive(true);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded GameOverUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildGameOverUI()
        {
            GameObject root = CreateRectObject("GameOverUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            GameOverUI gameOverUI = root.AddComponent<GameOverUI>();
            Image backdrop = root.AddComponent<Image>();
            backdrop.color = new Color(0f, 0f, 0f, 0.88f);

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            Image gameOverTextImage = CreateImage("GameOverText", root.transform, GameOverTextSpritePath, new Vector2(0f, 112f), new Vector2(460f, 132f));
            TMP_Text hintLabel = CreateLabel("HintLabel", root.transform, new Vector2(0f, 12f), new Vector2(520f, 36f), 20f);
            hintLabel.text = "It begins again in the child's room.";

            Button restartButton = CreateSpriteButton("RestartButton", root.transform, RestartSpritePath, new Vector2(0f, -76f), new Vector2(260f, 66f));
            Button returnTitleButton = CreateSpriteButton("ReturnTitleButton", root.transform, ReturnTitleSpritePath, new Vector2(0f, -158f), new Vector2(260f, 66f));

            SerializedObject serialized = new SerializedObject(gameOverUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("restartButton").objectReferenceValue = restartButton;
            serialized.FindProperty("returnTitleButton").objectReferenceValue = returnTitleButton;
            serialized.FindProperty("gameOverTextImage").objectReferenceValue = gameOverTextImage;
            serialized.FindProperty("hintLabel").objectReferenceValue = hintLabel;
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
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
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
            text.color = new Color(0.72f, 0.72f, 0.72f, 1f);
            text.raycastTarget = false;
            return text;
        }

        private static void ConfigureSceneInstance(GameObject instance, GameStateManager gameStateManager, TitleUI titleUI)
        {
            GameOverUI gameOverUI = instance.GetComponent<GameOverUI>();
            if (gameOverUI == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(gameOverUI);
            serialized.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serialized.FindProperty("titleUI").objectReferenceValue = titleUI;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(gameOverUI);
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
