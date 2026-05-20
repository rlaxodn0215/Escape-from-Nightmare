using EscapeFromNightmares.Core;
using EscapeFromNightmares.Systems;
using EscapeFromNightmares.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1PuzzleRuntimeSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";

        [MenuItem("Escape From Nightmares/Seed Stage1 Puzzle Runtime")]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            GameObject puzzleUIObject = FindSceneObject("PuzzleUI");

            if (systems == null || puzzleUIObject == null)
            {
                Debug.LogError("Stage1 scene is missing Systems or PuzzleUI.");
                return;
            }

            PuzzleUI puzzleUI = puzzleUIObject.GetComponent<PuzzleUI>();
            if (puzzleUI == null)
            {
                Debug.LogError("PuzzleUI scene object is missing PuzzleUI component.");
                return;
            }

            EnsurePuzzleUIControls(puzzleUIObject, puzzleUI);

            PuzzleSystem puzzleSystem = systems.GetComponent<PuzzleSystem>();
            if (puzzleSystem == null)
            {
                puzzleSystem = systems.AddComponent<PuzzleSystem>();
            }

            InventorySystem inventorySystem = systems.GetComponent<InventorySystem>();
            GameStateManager gameStateManager = systems.GetComponent<GameStateManager>();
            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();

            SerializedObject serializedPuzzleSystem = new SerializedObject(puzzleSystem);
            serializedPuzzleSystem.FindProperty("puzzleUI").objectReferenceValue = puzzleUI;
            serializedPuzzleSystem.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serializedPuzzleSystem.FindProperty("inventorySystem").objectReferenceValue = inventorySystem;
            serializedPuzzleSystem.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(puzzleSystem);

            if (interactionSystem != null)
            {
                SerializedObject serializedInteraction = new SerializedObject(interactionSystem);
                serializedInteraction.FindProperty("puzzleSystem").objectReferenceValue = puzzleSystem;
                serializedInteraction.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(interactionSystem);
            }

            puzzleUIObject.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 puzzle runtime.");
        }

        private static void EnsurePuzzleUIControls(GameObject puzzleUIObject, PuzzleUI puzzleUI)
        {
            Transform panel = FindChild(puzzleUIObject.transform, "Panel");
            Transform contentRoot = FindChild(puzzleUIObject.transform, "ContentRoot");
            if (panel == null || contentRoot == null)
            {
                Debug.LogError("PuzzleUI is missing Panel or ContentRoot.");
                return;
            }

            TMP_InputField answerInput = EnsureInputField(contentRoot);
            TMP_Text feedbackLabel = EnsureLabel(panel, "FeedbackLabel", new Vector2(0f, -140f), new Vector2(520f, 28f), 18f);
            Button submitButton = EnsureButton(panel, "SubmitButton", "Submit", new Vector2(-92f, -224f), new Vector2(150f, 48f));
            Button clearButton = EnsureButton(panel, "ClearButton", "Clear", new Vector2(92f, -224f), new Vector2(150f, 48f));

            SerializedObject serialized = new SerializedObject(puzzleUI);
            serialized.FindProperty("feedbackLabel").objectReferenceValue = feedbackLabel;
            serialized.FindProperty("answerInput").objectReferenceValue = answerInput;
            serialized.FindProperty("submitButton").objectReferenceValue = submitButton;
            serialized.FindProperty("clearButton").objectReferenceValue = clearButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(puzzleUI);
        }

        private static TMP_InputField EnsureInputField(Transform parent)
        {
            Transform existing = FindChild(parent, "AnswerInput");
            GameObject root = existing != null ? existing.gameObject : CreateRectObject("AnswerInput", parent);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -12f);
            rect.sizeDelta = new Vector2(430f, 54f);

            Image image = root.GetComponent<Image>();
            if (image == null)
            {
                image = root.AddComponent<Image>();
            }

            image.color = new Color(0.05f, 0.05f, 0.05f, 0.86f);

            TMP_Text text = EnsureLabel(root.transform, "Text", Vector2.zero, new Vector2(390f, 38f), 22f);
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = true;

            TMP_Text placeholder = EnsureLabel(root.transform, "Placeholder", Vector2.zero, new Vector2(390f, 38f), 18f);
            placeholder.text = "";
            placeholder.color = new Color(0.55f, 0.55f, 0.55f, 0.72f);

            TMP_InputField input = root.GetComponent<TMP_InputField>();
            if (input == null)
            {
                input = root.AddComponent<TMP_InputField>();
            }

            input.textComponent = text;
            input.placeholder = placeholder;
            input.transition = Selectable.Transition.None;
            input.lineType = TMP_InputField.LineType.SingleLine;
            return input;
        }

        private static Button EnsureButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
        {
            Transform existing = FindChild(parent, name);
            GameObject root = existing != null ? existing.gameObject : CreateRectObject(name, parent);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = root.GetComponent<Image>();
            if (image == null)
            {
                image = root.AddComponent<Image>();
            }

            image.color = new Color(0.12f, 0.12f, 0.12f, 0.92f);

            TMP_Text text = EnsureLabel(root.transform, "Label", Vector2.zero, size, 18f);
            text.text = label;

            Button button = root.GetComponent<Button>();
            if (button == null)
            {
                button = root.AddComponent<Button>();
            }

            button.transition = Selectable.Transition.None;
            return button;
        }

        private static TMP_Text EnsureLabel(Transform parent, string name, Vector2 position, Vector2 size, float fontSize)
        {
            Transform existing = FindChild(parent, name);
            GameObject root = existing != null ? existing.gameObject : CreateRectObject(name, parent);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI text = root.GetComponent<TextMeshProUGUI>();
            if (text == null)
            {
                text = root.AddComponent<TextMeshProUGUI>();
            }

            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.82f, 0.82f, 0.82f, 1f);
            text.raycastTarget = false;
            return text;
        }

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static Transform FindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child != parent && child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }

        private static GameObject FindSceneObject(string name)
        {
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                Transform[] children = root.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child.name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }
    }
}
