using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public static class RemainingPuzzlePrefabBuilder
    {
        private const string PuzzleUiFolder = "Assets/Resources/PuzzleUI";
        private const string BackupFolder = "Assets/Backups/PuzzleUI";

        [MenuItem("Escape From Nightmare/Puzzle Prefabs/Create Missing Remaining Puzzle Prefabs")]
        public static void CreateMissingRemainingPuzzlePrefabs()
        {
            BuildAll(false);
        }

        [MenuItem("Escape From Nightmare/Puzzle Prefabs/Rebuild Remaining Puzzle Prefabs With Backup")]
        public static void RebuildRemainingPuzzlePrefabsWithBackup()
        {
            BuildAll(true);
        }

        private static void BuildAll(bool rebuild)
        {
            EnsureFolder(PuzzleUiFolder);
            EnsureFolder(BackupFolder);

            int created = 0;
            int rebuilt = 0;
            int skipped = 0;

            BuildResult livingRoom = BuildItemUsePrefab(
                "PuzzleUI_LivingRoomItemUse",
                typeof(PuzzleUI_LivingRoomItemUse),
                "Living Room Item Use",
                rebuild);
            Count(livingRoom, ref created, ref rebuilt, ref skipped);

            BuildResult basement = BuildBasementPowerDevicePrefab(rebuild);
            Count(basement, ref created, ref rebuilt, ref skipped);

            BuildResult lockedRoom = BuildLockedRoomFinalPrefab(rebuild);
            Count(lockedRoom, ref created, ref rebuilt, ref skipped);

            BuildResult entrance = BuildItemUsePrefab(
                "PuzzleUI_EntranceDoor",
                typeof(PuzzleUI_EntranceDoor),
                "Entrance Door",
                rebuild);
            Count(entrance, ref created, ref rebuilt, ref skipped);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ValidateResourcesLoad("PuzzleUI/PuzzleUI_LivingRoomItemUse");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_BasementPowerDevice");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_LockedRoomFinal");
            ValidateResourcesLoad("PuzzleUI/PuzzleUI_EntranceDoor");

            Debug.Log("[RemainingPuzzlePrefabBuilder] Completed. Created: " + created + ", Rebuilt: " + rebuilt + ", Skipped: " + skipped);
        }

        private static BuildResult BuildItemUsePrefab(string prefabName, Type componentType, string title, bool rebuild)
        {
            string prefabPath = PuzzleUiFolder + "/" + prefabName + ".prefab";
            if (File.Exists(prefabPath) && !rebuild)
            {
                Debug.Log("[RemainingPuzzlePrefabBuilder] Prefab already exists. Skipped: " + prefabPath);
                return BuildResult.Skipped;
            }

            if (File.Exists(prefabPath))
            {
                BackupExistingPrefab(prefabPath);
            }

            GameObject root = CreateUiRoot(prefabName, componentType);
            try
            {
                Component ui = root.GetComponent(componentType);
                CreateText("TitleText", root.transform, title, 30);
                Text messageText = CreateText("MessageText", root.transform, string.Empty, 20);
                Button useButton = CreateButton("UseSelectedItemButton", root.transform, "Use Item");
                Button closeButton = CreateButton("CloseButton", root.transform, "Close");

                SetSerializedObjectReference(ui, "messageText", messageText);
                SetSerializedObjectReference(ui, "useSelectedItemButton", useButton);
                SetSerializedObjectReference(ui, "closeButton", closeButton);

                SavePrefab(root, prefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            Debug.Log("[RemainingPuzzlePrefabBuilder] Saved prefab: " + prefabPath);
            return rebuild ? BuildResult.Rebuilt : BuildResult.Created;
        }

        private static BuildResult BuildBasementPowerDevicePrefab(bool rebuild)
        {
            const string prefabName = "PuzzleUI_BasementPowerDevice";
            string prefabPath = PuzzleUiFolder + "/" + prefabName + ".prefab";
            if (File.Exists(prefabPath) && !rebuild)
            {
                Debug.Log("[RemainingPuzzlePrefabBuilder] Prefab already exists. Skipped: " + prefabPath);
                return BuildResult.Skipped;
            }

            if (File.Exists(prefabPath))
            {
                BackupExistingPrefab(prefabPath);
            }

            GameObject root = CreateUiRoot(prefabName, typeof(PuzzleUI_BasementPowerDevice));
            try
            {
                PuzzleUI_BasementPowerDevice ui = root.GetComponent<PuzzleUI_BasementPowerDevice>();
                CreateText("TitleText", root.transform, "Basement Power Device", 30);
                Text inputText = CreateText("InputText", root.transform, string.Empty, 20);
                Text messageText = CreateText("MessageText", root.transform, string.Empty, 20);
                GameObject switchRoot = CreatePanel("SwitchButtonRoot", root.transform);

                List<UnityEngine.Object> switches = new List<UnityEngine.Object>();
                switches.Add(CreateSwitchButton("Switch_Left_Button", switchRoot.transform, "Switch_Left", ui));
                switches.Add(CreateSwitchButton("Switch_Right_Button", switchRoot.transform, "Switch_Right", ui));
                switches.Add(CreateSwitchButton("Switch_Center_Button", switchRoot.transform, "Switch_Center", ui));

                Button powerButton = CreateButton("PowerButton", root.transform, "Power");
                Button resetButton = CreateButton("ResetButton", root.transform, "Reset");
                Button closeButton = CreateButton("CloseButton", root.transform, "Close");

                SetSerializedObjectReference(ui, "inputText", inputText);
                SetSerializedObjectReference(ui, "messageText", messageText);
                SetSerializedObjectReference(ui, "switchButtonRoot", switchRoot.transform);
                SetSerializedObjectList(ui, "switchButtons", switches);
                SetSerializedObjectReference(ui, "powerButton", powerButton);
                SetSerializedObjectReference(ui, "resetButton", resetButton);
                SetSerializedObjectReference(ui, "closeButton", closeButton);
                SetSerializedInt(ui, "requiredInputLength", 5);
                SetSerializedString(ui, "requiredSecondItemId", "SmallClockworkDevice");
                SetSerializedString(ui, "transformedItemId", "ModifiedClockworkDevice");
                SetSerializedString(ui, "unlockDoorId", "Door_BasementStorage_LockedRoom");
                SetSerializedString(ui, "unlockClueId", "BasementClueImage");

                SavePrefab(root, prefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            Debug.Log("[RemainingPuzzlePrefabBuilder] Saved prefab: " + prefabPath);
            return rebuild ? BuildResult.Rebuilt : BuildResult.Created;
        }

        private static BuildResult BuildLockedRoomFinalPrefab(bool rebuild)
        {
            const string prefabName = "PuzzleUI_LockedRoomFinal";
            string prefabPath = PuzzleUiFolder + "/" + prefabName + ".prefab";
            if (File.Exists(prefabPath) && !rebuild)
            {
                Debug.Log("[RemainingPuzzlePrefabBuilder] Prefab already exists. Skipped: " + prefabPath);
                return BuildResult.Skipped;
            }

            if (File.Exists(prefabPath))
            {
                BackupExistingPrefab(prefabPath);
            }

            GameObject root = CreateUiRoot(prefabName, typeof(PuzzleUI_LockedRoomFinal));
            try
            {
                PuzzleUI_LockedRoomFinal ui = root.GetComponent<PuzzleUI_LockedRoomFinal>();
                CreateText("TitleText", root.transform, "Locked Room Final", 30);
                Text sequenceText = CreateText("SequenceText", root.transform, string.Empty, 20);
                Text messageText = CreateText("MessageText", root.transform, string.Empty, 20);
                GameObject slotRoot = CreatePanel("SlotRoot", root.transform);

                List<UnityEngine.Object> slots = new List<UnityEngine.Object>();
                for (int i = 0; i < 5; i++)
                {
                    slots.Add(CreateSymbolSlot("Slot_0" + (i + 1), slotRoot.transform, i, ui));
                }

                Text itemUseMessageText = CreateText("ItemUseMessageText", root.transform, string.Empty, 18);
                Button submitButton = CreateButton("SubmitButton", root.transform, "Submit");
                Button resetButton = CreateButton("ResetButton", root.transform, "Reset");
                Button useButton = CreateButton("UseClockworkDeviceButton", root.transform, "Use Device");
                Button closeButton = CreateButton("CloseButton", root.transform, "Close");

                SetSerializedObjectReference(ui, "sequenceText", sequenceText);
                SetSerializedObjectReference(ui, "messageText", messageText);
                SetSerializedObjectReference(ui, "slotRoot", slotRoot.transform);
                SetSerializedObjectList(ui, "slots", slots);
                SetSerializedObjectReference(ui, "submitButton", submitButton);
                SetSerializedObjectReference(ui, "resetButton", resetButton);
                SetSerializedObjectReference(ui, "closeButton", closeButton);
                SetSerializedInt(ui, "expectedSlotCount", 5);
                SetSerializedStringArray(ui, "availableSymbolIds", new[] { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" });
                SetSerializedObjectReference(ui, "itemUseMessageText", itemUseMessageText);
                SetSerializedObjectReference(ui, "useClockworkDeviceButton", useButton);
                SetSerializedString(ui, "requiredFinalItemId", "ModifiedClockworkDevice");

                SavePrefab(root, prefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            Debug.Log("[RemainingPuzzlePrefabBuilder] Saved prefab: " + prefabPath);
            return rebuild ? BuildResult.Rebuilt : BuildResult.Created;
        }

        private static PuzzlePowerSwitchButton CreateSwitchButton(string name, Transform parent, string switchId, PuzzlePowerDeviceUIBase target)
        {
            Button button = CreateButton(name, parent, switchId);
            PuzzlePowerSwitchButton switchButton = button.gameObject.AddComponent<PuzzlePowerSwitchButton>();
            Text labelText = button.GetComponentInChildren<Text>(true);
            SetSerializedString(switchButton, "switchId", switchId);
            SetSerializedObjectReference(switchButton, "labelText", labelText);
            SetSerializedObjectReference(switchButton, "target", target);
            return switchButton;
        }

        private static PuzzleSymbolCycleSlot CreateSymbolSlot(string name, Transform parent, int slotIndex, PuzzleSymbolCycleUIBase target)
        {
            GameObject slot = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(PuzzleSymbolCycleSlot));
            slot.transform.SetParent(parent, false);
            Image image = slot.GetComponent<Image>();
            image.color = new Color(0.18f, 0.18f, 0.2f, 0.9f);
            Text labelText = CreateText("LabelText", slot.transform, string.Empty, 18);
            PuzzleSymbolCycleSlot cycleSlot = slot.GetComponent<PuzzleSymbolCycleSlot>();
            SetSerializedObjectReference(cycleSlot, "symbolImage", image);
            SetSerializedObjectReference(cycleSlot, "labelText", labelText);
            SetSerializedInt(cycleSlot, "slotIndex", slotIndex);
            SetSerializedObjectReference(cycleSlot, "target", target);
            return cycleSlot;
        }

        private static GameObject CreateUiRoot(string name, Type componentType)
        {
            GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = root.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image image = root.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.78f);
            root.AddComponent(componentType);
            return root;
        }

        private static GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
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
            return textComponent;
        }

        private static Button CreateButton(string name, Transform parent, string label)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.32f, 0.32f, 0.36f, 0.95f);
            Text labelText = CreateText("Text", buttonObject.transform, label, 18);
            labelText.raycastTarget = false;
            return buttonObject.GetComponent<Button>();
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

        private static Font GetDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            if (font == null)
            {
                Debug.LogWarning("[RemainingPuzzlePrefabBuilder] Could not load built-in UI font.");
            }

            return font;
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

        private static void SetSerializedObjectList(UnityEngine.Object target, string fieldName, IList<UnityEngine.Object> values)
        {
            SerializedProperty property = FindProperty(target, fieldName);
            if (property == null || !property.isArray)
            {
                return;
            }

            property.arraySize = values != null ? values.Count : 0;
            for (int i = 0; i < property.arraySize; i++)
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
            for (int i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i];
            }

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static SerializedProperty FindProperty(UnityEngine.Object target, string fieldName)
        {
            if (target == null || string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogWarning("[RemainingPuzzlePrefabBuilder] Serialized field not found: " + target.GetType().Name + "." + fieldName);
            }

            return property;
        }

        private static void SavePrefab(GameObject root, string path)
        {
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, path);
            if (saved == null)
            {
                Debug.LogError("[RemainingPuzzlePrefabBuilder] Failed to save prefab: " + path);
            }
        }

        private static void BackupExistingPrefab(string prefabPath)
        {
            try
            {
                EnsureFolder(BackupFolder);
                string fileName = Path.GetFileNameWithoutExtension(prefabPath);
                string backupPath = BackupFolder + "/" + fileName + ".backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".prefab";
                File.Copy(prefabPath, backupPath, true);
                Debug.Log("[RemainingPuzzlePrefabBuilder] Backed up prefab: " + backupPath);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[RemainingPuzzlePrefabBuilder] Could not back up prefab: " + prefabPath + " / " + exception.Message);
            }
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path);
                string folder = Path.GetFileName(path);
                if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                {
                    EnsureFolder(parent.Replace('\\', '/'));
                }

                AssetDatabase.CreateFolder(parent != null ? parent.Replace('\\', '/') : "Assets", folder);
            }
        }

        private static void ValidateResourcesLoad(string resourcesPath)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcesPath);
            if (prefab == null)
            {
                Debug.LogError("[RemainingPuzzlePrefabBuilder] Resources.Load failed: " + resourcesPath);
            }
            else
            {
                Debug.Log("[RemainingPuzzlePrefabBuilder] Resources.Load OK: " + resourcesPath);
            }
        }

        private static void Count(BuildResult result, ref int created, ref int rebuilt, ref int skipped)
        {
            if (result == BuildResult.Created)
            {
                created++;
            }
            else if (result == BuildResult.Rebuilt)
            {
                rebuilt++;
            }
            else
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
