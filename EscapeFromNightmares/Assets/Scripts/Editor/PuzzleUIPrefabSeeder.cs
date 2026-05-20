using EscapeFromNightmares.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class PuzzleUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/PuzzleUI.prefab";
        private const string PanelSpritePath = "Assets/Sprites/UI/ui_puzzle_panel.png";
        private const string CloseSpritePath = "Assets/Sprites/UI/ui_button_continue.png";
        private const string DigitSpritePath = "Assets/Sprites/UI/ui_number_lock_digit.png";
        private const string SymbolSpritePath = "Assets/Sprites/UI/ui_symbol_button.png";

        [MenuItem("Escape From Nightmares/Seed Puzzle UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildPuzzleUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            if (canvas == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas.");
                return;
            }

            GameObject existing = FindChild(canvas, "PuzzleUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "PuzzleUI";
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded PuzzleUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildPuzzleUI()
        {
            GameObject root = CreateRectObject("PuzzleUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            PuzzleUI puzzleUI = root.AddComponent<PuzzleUI>();
            Image backdrop = root.AddComponent<Image>();
            backdrop.color = new Color(0f, 0f, 0f, 0.62f);

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            GameObject panel = CreateRectObject("Panel", root.transform);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(720f, 470f);
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImage = panel.AddComponent<Image>();
            panelImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;

            TMP_Text titleLabel = CreateLabel("TitleLabel", panel.transform, new Vector2(0f, 190f), new Vector2(520f, 38f), 28f);
            titleLabel.text = "Puzzle";

            TMP_Text hintLabel = CreateLabel("HintLabel", panel.transform, new Vector2(0f, -178f), new Vector2(560f, 32f), 18f);
            hintLabel.text = "";

            RectTransform contentRoot = CreateContentRoot(panel.transform);
            CreatePreviewDigits(contentRoot);
            CreatePreviewSymbols(contentRoot);

            Button closeButton = CreateSpriteButton("CloseButton", panel.transform, CloseSpritePath, new Vector2(0f, -224f), new Vector2(230f, 58f));

            SerializedObject serialized = new SerializedObject(puzzleUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("closeButton").objectReferenceValue = closeButton;
            serialized.FindProperty("contentRoot").objectReferenceValue = contentRoot;
            serialized.FindProperty("titleLabel").objectReferenceValue = titleLabel;
            serialized.FindProperty("hintLabel").objectReferenceValue = hintLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        private static RectTransform CreateContentRoot(Transform parent)
        {
            GameObject content = CreateRectObject("ContentRoot", parent);
            RectTransform rect = content.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(560f, 250f);
            rect.anchoredPosition = new Vector2(0f, 8f);
            return rect;
        }

        private static void CreatePreviewDigits(RectTransform parent)
        {
            for (int index = 0; index < 4; index++)
            {
                Image image = CreateImage("DigitSlot" + (index + 1), parent, DigitSpritePath, new Vector2(-135f + index * 90f, 54f), new Vector2(62f, 82f));
                image.color = new Color(1f, 1f, 1f, 0.82f);
            }
        }

        private static void CreatePreviewSymbols(RectTransform parent)
        {
            for (int index = 0; index < 3; index++)
            {
                Image image = CreateImage("SymbolSlot" + (index + 1), parent, SymbolSpritePath, new Vector2(-90f + index * 90f, -58f), new Vector2(68f, 68f));
                image.color = new Color(1f, 1f, 1f, 0.72f);
            }
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
    }
}
