using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public static class PuzzlePrefabBuilder
    {
        private const string PuzzleUiFolder = "Assets/Resources/PuzzleUI";
        private const string BackupFolder = "Assets/Backups/PuzzleUI";

        private static readonly string[] allSymbols =
        {
            "Symbol_01",
            "Symbol_02",
            "Symbol_03",
            "Symbol_04",
            "Symbol_05",
            "Symbol_06"
        };

        [MenuItem("Escape From Nightmare/Puzzle Prefabs/Create Missing First Five Puzzle Prefabs")]
        public static void CreateMissingFirstFivePuzzlePrefabs()
        {
            BuildFirstFivePrefabs(false);
        }

        [MenuItem("Escape From Nightmare/Puzzle Prefabs/Rebuild First Five Puzzle Prefabs With Backup")]
        public static void RebuildFirstFivePuzzlePrefabsWithBackup()
        {
            BuildFirstFivePrefabs(true);
        }

        private static void BuildFirstFivePrefabs(bool rebuild)
        {
            EnsureFolder(PuzzleUiFolder);

            int created = 0;
            int skipped = 0;
            int rebuilt = 0;

            BuildResult bedroom = BuildNumberCodePrefab(
                "PuzzleUI_BedroomNumberCode",
                typeof(PuzzleUI_BedroomNumberCode),
                "Bedroom Code",
                "7319",
                "60",
                rebuild);
            CountResult(bedroom, ref created, ref skipped, ref rebuilt);

            BuildResult kitchen = BuildNumberCodePrefab(
                "PuzzleUI_KitchenNumberCode",
                typeof(PuzzleUI_KitchenNumberCode),
                "Kitchen Code",
                "4826",
                "45",
                rebuild);
            CountResult(kitchen, ref created, ref skipped, ref rebuilt);

            BuildResult childRoom = BuildSequencePrefab(
                "PuzzleUI_ChildRoomCardOrder",
                typeof(PuzzleUI_ChildRoomCardOrder),
                "Child Room Card Order",
                new string[] { "Symbol_01", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" },
                rebuild);
            CountResult(childRoom, ref created, ref skipped, ref rebuilt);

            BuildResult study = BuildSequencePrefab(
                "PuzzleUI_StudyBookOrder",
                typeof(PuzzleUI_StudyBookOrder),
                "Study Book Order",
                allSymbols,
                rebuild);
            CountResult(study, ref created, ref skipped, ref rebuilt);

            BuildResult livingRoom = BuildSymbolCyclePrefab(rebuild);
            CountResult(livingRoom, ref created, ref skipped, ref rebuilt);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ValidateResourcesLoad("PuzzleUI/PuzzleUI_BedroomNumberCode");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_KitchenNumberCode");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_ChildRoomCardOrder");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_StudyBookOrder");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_LivingRoomSymbolSequence");

            Debug.Log("[PuzzlePrefabBuilder] Completed. Created: " + created + ", Rebuilt: " + rebuilt + ", Skipped: " + skipped);
        }

        private static BuildResult BuildNumberCodePrefab(string prefabName, Type componentType, string title, string fallbackAnswer, string timerInitialText, bool rebuild)
        {
            string prefabPath = GetPrefabPath(prefabName);
            if (File.Exists(prefabPath) && !rebuild)
            {
                Debug.Log("[PuzzlePrefabBuilder] Prefab already exists. Skipped: " + prefabPath);
                return BuildResult.Skipped;
            }

            if (File.Exists(prefabPath) && rebuild)
            {
                BackupExistingPrefab(prefabPath);
            }

            GameObject root = CreateUiRoot(prefabName, componentType);
            try
            {
                Component puzzleComponent = root.GetComponent(componentType);
                PuzzleNumberCodeUIBase puzzleUi = puzzleComponent as PuzzleNumberCodeUIBase;

                GameObject header = CreatePanel("Header", root.transform);
                SetRect(header.GetComponent<RectTransform>(), new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.95f), Vector2.zero, Vector2.zero);
                Text titleText = CreateText("TitleText", header.transform, title, 32);
                SetRect(titleText.rectTransform, new Vector2(0f, 0f), new Vector2(0.7f, 1f), Vector2.zero, Vector2.zero);
                Text timerText = CreateText("TimerText", header.transform, timerInitialText, 28);
                SetRect(timerText.rectTransform, new Vector2(0.72f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);

                GameObject displayPanel = CreatePanel("DisplayPanel", root.transform);
                SetRect(displayPanel.GetComponent<RectTransform>(), new Vector2(0.18f, 0.66f), new Vector2(0.82f, 0.78f), Vector2.zero, Vector2.zero);
                Text displayText = CreateText("DisplayText", displayPanel.transform, string.Empty, 36);
                SetRect(displayText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

                Text messageText = CreateText("MessageText", root.transform, string.Empty, 22);
                SetRect(messageText.rectTransform, new Vector2(0.14f, 0.58f), new Vector2(0.86f, 0.65f), Vector2.zero, Vector2.zero);

                GameObject numberButtonRoot = CreatePanel("NumberButtonRoot", root.transform);
                SetRect(numberButtonRoot.GetComponent<RectTransform>(), new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.56f), Vector2.zero, Vector2.zero);

                List<UnityEngine.Object> numberButtons = new List<UnityEngine.Object>();
                int[] digits = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
                for (int i = 0; i < digits.Length; i++)
                {
                    int row = i / 3;
                    int col = i % 3;
                    if (digits[i] == 0)
                    {
                        row = 3;
                        col = 1;
                    }

                    GameObject buttonObject = CreateButton("Button_" + digits[i], numberButtonRoot.transform, digits[i].ToString());
                    RectTransform rect = buttonObject.GetComponent<RectTransform>();
                    float cellWidth = 1f / 3f;
                    float cellHeight = 1f / 4f;
                    Vector2 anchorMin = new Vector2(col * cellWidth + 0.02f, 1f - (row + 1) * cellHeight + 0.02f);
                    Vector2 anchorMax = new Vector2((col + 1) * cellWidth - 0.02f, 1f - row * cellHeight - 0.02f);
                    SetRect(rect, anchorMin, anchorMax, Vector2.zero, Vector2.zero);

                    PuzzleNumberButton numberButton = buttonObject.AddComponent<PuzzleNumberButton>();
                    SetSerializedInt(numberButton, "digit", digits[i]);
                    SetSerializedObjectReference(numberButton, "target", puzzleUi);
                    numberButtons.Add(numberButton);
                }

                GameObject controlButtons = CreatePanel("ControlButtons", root.transform);
                SetRect(controlButtons.GetComponent<RectTransform>(), new Vector2(0.08f, 0.05f), new Vector2(0.92f, 0.17f), Vector2.zero, Vector2.zero);
                Button submitButton = CreateHorizontalControlButton("SubmitButton", controlButtons.transform, "Submit", 0, 4);
                Button clearButton = CreateHorizontalControlButton("ClearButton", controlButtons.transform, "Clear", 1, 4);
                Button backspaceButton = CreateHorizontalControlButton("BackspaceButton", controlButtons.transform, "Back", 2, 4);
                Button closeButton = CreateHorizontalControlButton("CloseButton", controlButtons.transform, "Close", 3, 4);

                SetSerializedObjectReference(puzzleComponent, "displayText", displayText);
                SetSerializedObjectReference(puzzleComponent, "messageText", messageText);
                SetSerializedObjectReference(puzzleComponent, "timerText", timerText);
                SetSerializedObjectReference(puzzleComponent, "numberButtonRoot", numberButtonRoot.transform);
                SetSerializedObjectList(puzzleComponent, "numberButtons", numberButtons);
                SetSerializedObjectReference(puzzleComponent, "submitButton", submitButton);
                SetSerializedObjectReference(puzzleComponent, "clearButton", clearButton);
                SetSerializedObjectReference(puzzleComponent, "backspaceButton", backspaceButton);
                SetSerializedObjectReference(puzzleComponent, "closeButton", closeButton);
                SetSerializedBool(puzzleComponent, "autoCollectNumberButtons", true);
                SetSerializedString(puzzleComponent, "fallbackAnswer", fallbackAnswer);
                SetSerializedInt(puzzleComponent, "fallbackCodeLength", 4);

                SavePrefab(root, prefabPath, rebuild);
                return rebuild ? BuildResult.Rebuilt : BuildResult.Created;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static BuildResult BuildSequencePrefab(string prefabName, Type componentType, string title, string[] optionIds, bool rebuild)
        {
            string prefabPath = GetPrefabPath(prefabName);
            if (File.Exists(prefabPath) && !rebuild)
            {
                Debug.Log("[PuzzlePrefabBuilder] Prefab already exists. Skipped: " + prefabPath);
                return BuildResult.Skipped;
            }

            if (File.Exists(prefabPath) && rebuild)
            {
                BackupExistingPrefab(prefabPath);
            }

            GameObject root = CreateUiRoot(prefabName, componentType);
            try
            {
                Component puzzleComponent = root.GetComponent(componentType);
                PuzzleSequenceUIBase puzzleUi = puzzleComponent as PuzzleSequenceUIBase;

                GameObject header = CreatePanel("Header", root.transform);
                SetRect(header.GetComponent<RectTransform>(), new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.95f), Vector2.zero, Vector2.zero);
                Text titleText = CreateText("TitleText", header.transform, title, 30);
                SetRect(titleText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

                Text sequenceText = CreateText("SequenceText", root.transform, string.Empty, 24);
                SetRect(sequenceText.rectTransform, new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.82f), Vector2.zero, Vector2.zero);

                Text messageText = CreateText("MessageText", root.transform, string.Empty, 22);
                SetRect(messageText.rectTransform, new Vector2(0.1f, 0.64f), new Vector2(0.9f, 0.71f), Vector2.zero, Vector2.zero);

                GameObject optionButtonRoot = CreatePanel("OptionButtonRoot", root.transform);
                SetRect(optionButtonRoot.GetComponent<RectTransform>(), new Vector2(0.08f, 0.2f), new Vector2(0.92f, 0.62f), Vector2.zero, Vector2.zero);

                List<UnityEngine.Object> optionButtons = new List<UnityEngine.Object>();
                for (int i = 0; i < optionIds.Length; i++)
                {
                    GameObject optionObject = CreateButton("Option_" + optionIds[i], optionButtonRoot.transform, optionIds[i]);
                    RectTransform rect = optionObject.GetComponent<RectTransform>();
                    float cellWidth = 1f / Mathf.Max(1, optionIds.Length);
                    SetRect(rect, new Vector2(i * cellWidth + 0.01f, 0.2f), new Vector2((i + 1) * cellWidth - 0.01f, 0.8f), Vector2.zero, Vector2.zero);

                    Image iconImage = CreateChildImage("IconImage", optionObject.transform, new Color(1f, 1f, 1f, 0.08f));
                    SetRect(iconImage.rectTransform, new Vector2(0.12f, 0.36f), new Vector2(0.88f, 0.92f), Vector2.zero, Vector2.zero);

                    Text labelText = optionObject.GetComponentInChildren<Text>();
                    GameObject selectedRoot = CreatePanel("SelectedIndicator", optionObject.transform);
                    Image selectedImage = selectedRoot.GetComponent<Image>();
                    selectedImage.color = new Color(0.3f, 0.8f, 1f, 0.35f);
                    SetRect(selectedRoot.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    selectedRoot.SetActive(false);

                    PuzzleSequenceOptionButton optionButton = optionObject.AddComponent<PuzzleSequenceOptionButton>();
                    SetSerializedString(optionButton, "optionId", optionIds[i]);
                    SetSerializedObjectReference(optionButton, "labelText", labelText);
                    SetSerializedObjectReference(optionButton, "iconImage", iconImage);
                    SetSerializedObjectReference(optionButton, "selectedRoot", selectedRoot);
                    SetSerializedObjectReference(optionButton, "target", puzzleUi);
                    optionButtons.Add(optionButton);
                }

                GameObject controlButtons = CreatePanel("ControlButtons", root.transform);
                SetRect(controlButtons.GetComponent<RectTransform>(), new Vector2(0.14f, 0.06f), new Vector2(0.86f, 0.16f), Vector2.zero, Vector2.zero);
                Button submitButton = CreateHorizontalControlButton("SubmitButton", controlButtons.transform, "Submit", 0, 3);
                Button resetButton = CreateHorizontalControlButton("ResetButton", controlButtons.transform, "Reset", 1, 3);
                Button closeButton = CreateHorizontalControlButton("CloseButton", controlButtons.transform, "Close", 2, 3);

                SetSerializedObjectReference(puzzleComponent, "sequenceText", sequenceText);
                SetSerializedObjectReference(puzzleComponent, "messageText", messageText);
                SetSerializedObjectReference(puzzleComponent, "optionButtonRoot", optionButtonRoot.transform);
                SetSerializedObjectList(puzzleComponent, "optionButtons", optionButtons);
                SetSerializedObjectReference(puzzleComponent, "submitButton", submitButton);
                SetSerializedObjectReference(puzzleComponent, "resetButton", resetButton);
                SetSerializedObjectReference(puzzleComponent, "closeButton", closeButton);
                SetSerializedBool(puzzleComponent, "autoCollectOptionButtons", true);
                SetSerializedBool(puzzleComponent, "autoSubmitWhenFull", true);
                SetSerializedBool(puzzleComponent, "disableOptionAfterSelect", true);
                SetSerializedBool(puzzleComponent, "refreshOptionsFromSymbolRecords", true);
                SetSerializedStringArray(puzzleComponent, "fallbackAnswerSequence", optionIds);

                SavePrefab(root, prefabPath, rebuild);
                return rebuild ? BuildResult.Rebuilt : BuildResult.Created;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static BuildResult BuildSymbolCyclePrefab(bool rebuild)
        {
            string prefabName = "PuzzleUI_LivingRoomSymbolSequence";
            string prefabPath = GetPrefabPath(prefabName);
            if (File.Exists(prefabPath) && !rebuild)
            {
                Debug.Log("[PuzzlePrefabBuilder] Prefab already exists. Skipped: " + prefabPath);
                return BuildResult.Skipped;
            }

            if (File.Exists(prefabPath) && rebuild)
            {
                BackupExistingPrefab(prefabPath);
            }

            GameObject root = CreateUiRoot(prefabName, typeof(PuzzleUI_LivingRoomSymbolSequence));
            try
            {
                PuzzleUI_LivingRoomSymbolSequence puzzleUi = root.GetComponent<PuzzleUI_LivingRoomSymbolSequence>();

                GameObject header = CreatePanel("Header", root.transform);
                SetRect(header.GetComponent<RectTransform>(), new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.95f), Vector2.zero, Vector2.zero);
                Text titleText = CreateText("TitleText", header.transform, "Living Room Symbol Sequence", 30);
                SetRect(titleText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

                Text sequenceText = CreateText("SequenceText", root.transform, string.Empty, 24);
                SetRect(sequenceText.rectTransform, new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.82f), Vector2.zero, Vector2.zero);

                Text messageText = CreateText("MessageText", root.transform, string.Empty, 22);
                SetRect(messageText.rectTransform, new Vector2(0.1f, 0.64f), new Vector2(0.9f, 0.71f), Vector2.zero, Vector2.zero);

                GameObject slotRoot = CreatePanel("SlotRoot", root.transform);
                SetRect(slotRoot.GetComponent<RectTransform>(), new Vector2(0.08f, 0.26f), new Vector2(0.92f, 0.62f), Vector2.zero, Vector2.zero);

                List<UnityEngine.Object> slots = new List<UnityEngine.Object>();
                for (int i = 0; i < 5; i++)
                {
                    GameObject slotObject = CreateButton("Slot_" + (i + 1).ToString("00"), slotRoot.transform, string.Empty);
                    RectTransform rect = slotObject.GetComponent<RectTransform>();
                    float cellWidth = 1f / 5f;
                    SetRect(rect, new Vector2(i * cellWidth + 0.012f, 0.16f), new Vector2((i + 1) * cellWidth - 0.012f, 0.84f), Vector2.zero, Vector2.zero);

                    Image symbolImage = slotObject.GetComponent<Image>();
                    Text labelText = slotObject.GetComponentInChildren<Text>();
                    labelText.text = "Empty";

                    PuzzleSymbolCycleSlot slot = slotObject.AddComponent<PuzzleSymbolCycleSlot>();
                    SetSerializedInt(slot, "slotIndex", i);
                    SetSerializedObjectReference(slot, "symbolImage", symbolImage);
                    SetSerializedObjectReference(slot, "labelText", labelText);
                    SetSerializedObjectReference(slot, "target", puzzleUi);
                    slots.Add(slot);
                }

                GameObject controlButtons = CreatePanel("ControlButtons", root.transform);
                SetRect(controlButtons.GetComponent<RectTransform>(), new Vector2(0.14f, 0.06f), new Vector2(0.86f, 0.16f), Vector2.zero, Vector2.zero);
                Button submitButton = CreateHorizontalControlButton("SubmitButton", controlButtons.transform, "Submit", 0, 3);
                Button resetButton = CreateHorizontalControlButton("ResetButton", controlButtons.transform, "Reset", 1, 3);
                Button closeButton = CreateHorizontalControlButton("CloseButton", controlButtons.transform, "Close", 2, 3);

                SetSerializedObjectReference(puzzleUi, "sequenceText", sequenceText);
                SetSerializedObjectReference(puzzleUi, "messageText", messageText);
                SetSerializedObjectReference(puzzleUi, "slotRoot", slotRoot.transform);
                SetSerializedObjectList(puzzleUi, "slots", slots);
                SetSerializedObjectReference(puzzleUi, "submitButton", submitButton);
                SetSerializedObjectReference(puzzleUi, "resetButton", resetButton);
                SetSerializedObjectReference(puzzleUi, "closeButton", closeButton);
                SetSerializedBool(puzzleUi, "autoCollectSlots", true);
                SetSerializedInt(puzzleUi, "expectedSlotCount", 5);
                SetSerializedStringArray(puzzleUi, "availableSymbolIds", allSymbols);

                SavePrefab(root, prefabPath, rebuild);
                return rebuild ? BuildResult.Rebuilt : BuildResult.Created;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static GameObject CreateUiRoot(string name, Type componentType)
        {
            GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = root.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Image image = root.GetComponent<Image>();
            image.color = new Color(0.03f, 0.035f, 0.04f, 0.92f);

            root.AddComponent(componentType);
            return root;
        }

        private static GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.04f);
            return panel;
        }

        private static Text CreateText(string name, Transform parent, string text, int fontSize)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text textComponent = textObject.GetComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = Color.white;
            textComponent.font = GetDefaultFont();
            textComponent.raycastTarget = false;
            return textComponent;
        }

        private static GameObject CreateButton(string name, Transform parent, string label)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.16f, 0.18f, 0.2f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.16f, 0.18f, 0.2f, 1f);
            colors.highlightedColor = new Color(0.24f, 0.28f, 0.32f, 1f);
            colors.pressedColor = new Color(0.08f, 0.1f, 0.12f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            Text labelText = CreateText("Text", buttonObject.transform, label, 20);
            SetRect(labelText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return buttonObject;
        }

        private static Button CreateHorizontalControlButton(string name, Transform parent, string label, int index, int total)
        {
            GameObject buttonObject = CreateButton(name, parent, label);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            float cellWidth = 1f / Mathf.Max(1, total);
            SetRect(rect, new Vector2(index * cellWidth + 0.01f, 0.12f), new Vector2((index + 1) * cellWidth - 0.01f, 0.88f), Vector2.zero, Vector2.zero);
            return buttonObject.GetComponent<Button>();
        }

        private static Image CreateChildImage(string name, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            rect.sizeDelta = Vector2.zero;
        }

        private static void SetSerializedObjectReference(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null)
            {
                return;
            }

            property.objectReferenceValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedString(UnityEngine.Object target, string fieldName, string value)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null)
            {
                return;
            }

            property.stringValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedInt(UnityEngine.Object target, string fieldName, int value)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null)
            {
                return;
            }

            property.intValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedBool(UnityEngine.Object target, string fieldName, bool value)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null)
            {
                return;
            }

            property.boolValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedObjectList(UnityEngine.Object target, string fieldName, IList<UnityEngine.Object> values)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null || !property.isArray)
            {
                return;
            }

            property.arraySize = values != null ? values.Count : 0;
            for (int i = 0; values != null && i < values.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedStringArray(UnityEngine.Object target, string fieldName, string[] values)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null || !property.isArray)
            {
                return;
            }

            property.arraySize = values != null ? values.Length : 0;
            for (int i = 0; values != null && i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i];
            }

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static SerializedProperty FindProperty(UnityEngine.Object target, string fieldName)
        {
            if (target == null)
            {
                Debug.LogWarning("[PuzzlePrefabBuilder] Cannot set serialized field on null target: " + fieldName);
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogWarning("[PuzzlePrefabBuilder] Serialized field not found: " + target.GetType().Name + "." + fieldName);
            }

            return property;
        }

        private static void SavePrefab(GameObject root, string path, bool overwrite)
        {
            if (!overwrite && File.Exists(path))
            {
                Debug.Log("[PuzzlePrefabBuilder] Prefab exists and overwrite is false: " + path);
                return;
            }

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            if (prefab == null)
            {
                Debug.LogError("[PuzzlePrefabBuilder] Failed to save prefab: " + path);
                return;
            }

            Debug.Log("[PuzzlePrefabBuilder] Saved prefab: " + path);
        }

        private static void BackupExistingPrefab(string prefabPath)
        {
            if (!File.Exists(prefabPath))
            {
                return;
            }

            EnsureFolder(BackupFolder);

            string fileName = Path.GetFileNameWithoutExtension(prefabPath);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = Path.Combine(BackupFolder, fileName + ".backup_" + timestamp + ".prefab").Replace("\\", "/");

            if (!AssetDatabase.CopyAsset(prefabPath, backupPath))
            {
                Debug.LogWarning("[PuzzlePrefabBuilder] Failed to backup prefab: " + prefabPath);
                return;
            }

            Debug.Log("[PuzzlePrefabBuilder] Backed up prefab: " + backupPath);
        }

        private static void EnsureFolder(string path)
        {
            string normalizedPath = path.Replace("\\", "/");
            string[] parts = normalizedPath.Split('/');
            if (parts.Length == 0 || parts[0] != "Assets")
            {
                Debug.LogWarning("[PuzzlePrefabBuilder] Folder path must start with Assets: " + path);
                return;
            }

            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static string GetPrefabPath(string prefabName)
        {
            return (PuzzleUiFolder + "/" + prefabName + ".prefab").Replace("\\", "/");
        }

        private static Font GetDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            if (font == null)
            {
                Debug.LogWarning("[PuzzlePrefabBuilder] Built-in UI font not found. Text components will have no font assigned.");
            }

            return font;
        }

        private static void ValidateResourcesLoad(string path)
        {
            GameObject loaded = Resources.Load<GameObject>(path);
            if (loaded == null)
            {
                Debug.LogError("[PuzzlePrefabBuilder] Resources.Load failed after save: " + path);
            }
            else
            {
                Debug.Log("[PuzzlePrefabBuilder] Resources.Load OK: " + path);
            }
        }

        private static void CountResult(BuildResult result, ref int created, ref int skipped, ref int rebuilt)
        {
            if (result == BuildResult.Created)
            {
                created++;
            }
            else if (result == BuildResult.Rebuilt)
            {
                rebuilt++;
            }
            else if (result == BuildResult.Skipped)
            {
                skipped++;
            }
        }

        private enum BuildResult
        {
            Created,
            Rebuilt,
            Skipped
        }
    }
}
