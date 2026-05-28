using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public static class PuzzlePrefabContractValidator
    {
        private static int errorCount;
        private static int warningCount;

        private static readonly Dictionary<string, PuzzleAnswerRecord> answerByVariableName = new Dictionary<string, PuzzleAnswerRecord>();
        private static readonly Dictionary<string, PuzzleAnswerRecord> answerByPuzzleId = new Dictionary<string, PuzzleAnswerRecord>();
        private static readonly HashSet<string> symbolIds = new HashSet<string>();

        [MenuItem("Escape From Nightmare/Validate Puzzle Prefab Contracts")]
        public static void ValidatePuzzlePrefabContracts()
        {
            errorCount = 0;
            warningCount = 0;

            string dataPath = Path.Combine(Application.streamingAssetsPath, "Data");
            PuzzleRecordList puzzles = LoadJson<PuzzleRecordList>(dataPath, "puzzles.json");
            PuzzleAnswerRecordList answers = LoadJson<PuzzleAnswerRecordList>(dataPath, "puzzle_answers.json");
            SymbolRecordList symbols = LoadJson<SymbolRecordList>(dataPath, "symbols.json");

            BuildAnswerMaps(answers);
            BuildSymbolIds(symbols);

            List<PuzzleRecord> puzzleList = puzzles != null && puzzles.puzzles != null ? puzzles.puzzles : new List<PuzzleRecord>();
            for (int i = 0; i < puzzleList.Count; i++)
            {
                ValidatePuzzlePrefab(puzzleList[i]);
            }

            Debug.Log("Puzzle prefab contract validation completed. Errors: " + errorCount + ", Warnings: " + warningCount);
        }

        private static void ValidatePuzzlePrefab(PuzzleRecord puzzle)
        {
            if (puzzle == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(puzzle.prefabPath))
            {
                AddWarning("Puzzle prefabPath is empty: " + puzzle.puzzleId);
                return;
            }

            GameObject prefab = Resources.Load<GameObject>(puzzle.prefabPath);
            if (prefab == null)
            {
                if (IsFirstFivePuzzle(puzzle))
                {
                    AddError("Required first-five puzzle prefab not found at Resources path: " + puzzle.prefabPath + " (" + puzzle.puzzleId + ")");
                }
                else
                {
                    AddWarning("Puzzle prefab not found at Resources path: " + puzzle.prefabPath + " (" + puzzle.puzzleId + ")");
                }

                return;
            }

            PuzzleUIBase baseUi = prefab.GetComponentInChildren<PuzzleUIBase>(true);
            if (baseUi == null)
            {
                AddError("Puzzle prefab has no PuzzleUIBase component: " + puzzle.puzzleId + " / " + puzzle.prefabPath);
                return;
            }

            if (puzzle.type == "NumberCode")
            {
                ValidateNumberCodePrefab(puzzle, prefab);
            }
            else if (puzzle.type == "Sequence" || puzzle.type == "FinalSequence")
            {
                ValidateSequencePrefab(puzzle, prefab, false);
            }
            else if (puzzle.type == "SymbolSequence")
            {
                ValidateSequencePrefab(puzzle, prefab, true);
            }
            else if (puzzle.type == "SymbolCycle")
            {
                ValidateSymbolCyclePrefab(puzzle, prefab);
            }
            else if (puzzle.type == "PowerDevice")
            {
                ValidatePowerDevicePrefab(puzzle, prefab);
            }
            else if (puzzle.type == "ItemUse")
            {
                ValidateItemUsePrefab(puzzle, prefab);
            }
            else if (puzzle.type == "FinalSymbolItem")
            {
                ValidateFinalSymbolItemPrefab(puzzle, prefab);
            }
            else
            {
                AddInfo("TODO puzzle type only checked for PuzzleUIBase: " + puzzle.puzzleId + " / " + puzzle.type);
            }
        }

        private static void ValidateNumberCodePrefab(PuzzleRecord puzzle, GameObject prefab)
        {
            PuzzleNumberCodeUIBase ui = prefab.GetComponentInChildren<PuzzleNumberCodeUIBase>(true);
            if (ui == null)
            {
                AddError("NumberCode puzzle prefab needs PuzzleNumberCodeUIBase: " + puzzle.puzzleId);
                return;
            }

            string path = puzzle.puzzleId + " / " + puzzle.prefabPath;
            CheckObjectField(ui, "displayText", "displayText", path, true);
            CheckObjectField(ui, "messageText", "messageText", path, true);
            CheckObjectField(ui, "timerText", "timerText", path, true);
            CheckObjectField(ui, "submitButton", "submitButton", path, true);
            CheckObjectField(ui, "clearButton", "clearButton", path, true);
            CheckObjectField(ui, "backspaceButton", "backspaceButton", path, true);
            CheckObjectField(ui, "closeButton", "closeButton", path, true);

            int fallbackCodeLength = GetIntField(ui, "fallbackCodeLength", 0);
            if (fallbackCodeLength != 4)
            {
                AddWarning("NumberCode fallbackCodeLength should be 4: " + path + " / " + fallbackCodeLength);
            }

            PuzzleNumberButton[] numberButtons = prefab.GetComponentsInChildren<PuzzleNumberButton>(true);
            if (numberButtons.Length < 10)
            {
                AddError("NumberCode prefab should have at least 10 PuzzleNumberButton components: " + path + " / found " + numberButtons.Length);
            }

            HashSet<int> digits = new HashSet<int>();
            for (int i = 0; i < numberButtons.Length; i++)
            {
                if (numberButtons[i].GetComponent<Button>() == null)
                {
                    AddError("PuzzleNumberButton is missing UnityEngine.UI.Button: " + path + " / " + GetHierarchyPath(numberButtons[i].gameObject));
                }

                if (!digits.Add(numberButtons[i].Digit))
                {
                    AddWarning("Duplicate PuzzleNumberButton digit: " + path + " / " + numberButtons[i].Digit);
                }
            }

            for (int digit = 0; digit <= 9; digit++)
            {
                if (!digits.Contains(digit))
                {
                    AddError("NumberCode prefab is missing digit button: " + path + " / " + digit);
                }
            }

            PuzzleAnswerRecord answer = ResolveAnswer(puzzle);
            if (answer == null || string.IsNullOrEmpty(answer.answerText))
            {
                AddError("NumberCode puzzle has no answerText in puzzle_answers.json: " + puzzle.puzzleId);
            }
            else if (puzzle.codeLength > 0 && answer.answerText.Length != puzzle.codeLength)
            {
                AddWarning("NumberCode codeLength differs from answerText length: " + puzzle.puzzleId + " / codeLength " + puzzle.codeLength + " / answer length " + answer.answerText.Length);
            }
        }

        private static void ValidateSequencePrefab(PuzzleRecord puzzle, GameObject prefab, bool symbolSequence)
        {
            PuzzleSequenceUIBase ui = prefab.GetComponentInChildren<PuzzleSequenceUIBase>(true);
            if (ui == null)
            {
                AddError("Sequence puzzle prefab needs PuzzleSequenceUIBase: " + puzzle.puzzleId);
                return;
            }

            string path = puzzle.puzzleId + " / " + puzzle.prefabPath;
            CheckObjectField(ui, "sequenceText", "sequenceText", path, true);
            CheckObjectField(ui, "messageText", "messageText", path, true);
            CheckObjectField(ui, "submitButton", "submitButton", path, true);
            CheckObjectField(ui, "resetButton", "resetButton", path, true);
            CheckObjectField(ui, "closeButton", "closeButton", path, true);

            PuzzleSequenceOptionButton[] optionButtons = prefab.GetComponentsInChildren<PuzzleSequenceOptionButton>(true);
            if (optionButtons.Length == 0)
            {
                AddError("Sequence prefab has no PuzzleSequenceOptionButton: " + path);
            }

            HashSet<string> optionIds = new HashSet<string>();
            for (int i = 0; i < optionButtons.Length; i++)
            {
                PuzzleSequenceOptionButton option = optionButtons[i];
                if (option.GetComponent<Button>() == null)
                {
                    AddError("PuzzleSequenceOptionButton is missing UnityEngine.UI.Button: " + path + " / " + GetHierarchyPath(option.gameObject));
                }

                if (string.IsNullOrEmpty(option.OptionId))
                {
                    AddError("PuzzleSequenceOptionButton.optionId is empty: " + path + " / " + GetHierarchyPath(option.gameObject));
                    continue;
                }

                if (!optionIds.Add(option.OptionId))
                {
                    AddWarning("Duplicate PuzzleSequenceOptionButton.optionId: " + path + " / " + option.OptionId);
                }

                if (symbolSequence && !symbolIds.Contains(option.OptionId))
                {
                    AddWarning("SymbolSequence optionId is not in symbols.json: " + path + " / " + option.OptionId);
                }
            }

            PuzzleAnswerRecord answer = ResolveAnswer(puzzle);
            if (answer == null || answer.answerSequence == null || answer.answerSequence.Length == 0)
            {
                AddError("Sequence puzzle has no answerSequence in puzzle_answers.json: " + puzzle.puzzleId);
            }
            else
            {
                for (int i = 0; i < answer.answerSequence.Length; i++)
                {
                    string expectedOption = answer.answerSequence[i];
                    if (!optionIds.Contains(expectedOption))
                    {
                        AddError("Sequence answer option is missing from prefab buttons: " + path + " / " + expectedOption);
                    }

                    if (symbolSequence && !symbolIds.Contains(expectedOption))
                    {
                        AddError("SymbolSequence answer option is not in symbols.json: " + path + " / " + expectedOption);
                    }
                }
            }

            if (symbolSequence && !GetBoolField(ui, "refreshOptionsFromSymbolRecords", false))
            {
                AddWarning("SymbolSequence prefab may want refreshOptionsFromSymbolRecords enabled: " + path);
            }

            ValidateExpectedOptionsForKnownSequence(puzzle, optionIds, path);
        }

        private static PuzzleAnswerRecord ResolveAnswer(PuzzleRecord puzzle)
        {
            if (puzzle == null)
            {
                return null;
            }

            PuzzleAnswerRecord answer;
            if (!string.IsNullOrEmpty(puzzle.answerVariableName) && answerByVariableName.TryGetValue(puzzle.answerVariableName, out answer))
            {
                return answer;
            }

            if (!string.IsNullOrEmpty(puzzle.puzzleId) && answerByPuzzleId.TryGetValue(puzzle.puzzleId, out answer))
            {
                return answer;
            }

            return null;
        }

        private static void ValidateSymbolCyclePrefab(PuzzleRecord puzzle, GameObject prefab)
        {
            PuzzleSymbolCycleUIBase ui = prefab.GetComponentInChildren<PuzzleSymbolCycleUIBase>(true);
            if (ui == null)
            {
                AddError("SymbolCycle puzzle prefab needs PuzzleSymbolCycleUIBase: " + puzzle.puzzleId);
                return;
            }

            string path = puzzle.puzzleId + " / " + puzzle.prefabPath;
            CheckObjectField(ui, "sequenceText", "sequenceText", path, true);
            CheckObjectField(ui, "messageText", "messageText", path, true);
            CheckObjectField(ui, "submitButton", "submitButton", path, true);
            CheckObjectField(ui, "resetButton", "resetButton", path, true);
            CheckObjectField(ui, "closeButton", "closeButton", path, true);

            int expectedSlotCount = GetIntField(ui, "expectedSlotCount", 5);
            PuzzleSymbolCycleSlot[] slots = prefab.GetComponentsInChildren<PuzzleSymbolCycleSlot>(true);
            if (slots.Length < expectedSlotCount)
            {
                AddError("SymbolCycle prefab has fewer slots than expected: " + path + " / expected " + expectedSlotCount + " / found " + slots.Length);
            }

            HashSet<int> slotIndices = new HashSet<int>();
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].GetComponent<Button>() == null)
                {
                    AddError("PuzzleSymbolCycleSlot is missing UnityEngine.UI.Button: " + path + " / " + GetHierarchyPath(slots[i].gameObject));
                }

                int slotIndex = GetIntField(slots[i], "slotIndex", -1);
                if (!slotIndices.Add(slotIndex))
                {
                    AddWarning("Duplicate PuzzleSymbolCycleSlot.slotIndex: " + path + " / " + slotIndex);
                }
            }

            for (int i = 0; i < expectedSlotCount; i++)
            {
                if (!slotIndices.Contains(i))
                {
                    AddError("SymbolCycle prefab is missing slotIndex: " + path + " / " + i);
                }
            }

            string[] availableSymbolIds = GetStringArrayField(ui, "availableSymbolIds");
            if (availableSymbolIds == null || availableSymbolIds.Length == 0)
            {
                AddWarning("SymbolCycle availableSymbolIds is empty. Runtime can fall back to symbols.json: " + path);
            }
            else
            {
                RequireStringInArray(availableSymbolIds, "Symbol_01", "availableSymbolIds", path);
                RequireStringInArray(availableSymbolIds, "Symbol_02", "availableSymbolIds", path);
                RequireStringInArray(availableSymbolIds, "Symbol_03", "availableSymbolIds", path);
                RequireStringInArray(availableSymbolIds, "Symbol_04", "availableSymbolIds", path);
                RequireStringInArray(availableSymbolIds, "Symbol_05", "availableSymbolIds", path);
                RequireStringInArray(availableSymbolIds, "Symbol_06", "availableSymbolIds", path);
            }

            PuzzleAnswerRecord answer = ResolveAnswer(puzzle);
            if (answer == null || answer.answerSequence == null || answer.answerSequence.Length == 0)
            {
                AddError("SymbolCycle puzzle has no answerSequence: " + puzzle.puzzleId);
            }
            else if (answer.answerSequence.Length != expectedSlotCount)
            {
                AddWarning("SymbolCycle answerSequence length differs from expectedSlotCount: " + path);
            }
        }

        private static bool IsFirstFivePuzzle(PuzzleRecord puzzle)
        {
            if (puzzle == null)
            {
                return false;
            }

            return puzzle.puzzleId == "Puzzle_Bedroom_01"
                || puzzle.puzzleId == "Puzzle_Kitchen_01"
                || puzzle.puzzleId == "Puzzle_ChildRoom_01"
                || puzzle.puzzleId == "Puzzle_Study_01"
                || puzzle.puzzleId == "Puzzle_LivingRoom_02";
        }

        private static void ValidateExpectedOptionsForKnownSequence(PuzzleRecord puzzle, HashSet<string> optionIds, string path)
        {
            if (puzzle == null || optionIds == null)
            {
                return;
            }

            if (puzzle.puzzleId == "Puzzle_ChildRoom_01")
            {
                RequireOption(optionIds, "Symbol_01", path);
                RequireOption(optionIds, "Symbol_03", path);
                RequireOption(optionIds, "Symbol_04", path);
                RequireOption(optionIds, "Symbol_05", path);
                RequireOption(optionIds, "Symbol_06", path);
            }
            else if (puzzle.puzzleId == "Puzzle_Study_01")
            {
                RequireOption(optionIds, "Symbol_01", path);
                RequireOption(optionIds, "Symbol_02", path);
                RequireOption(optionIds, "Symbol_03", path);
                RequireOption(optionIds, "Symbol_04", path);
                RequireOption(optionIds, "Symbol_05", path);
                RequireOption(optionIds, "Symbol_06", path);
            }
        }

        private static void RequireOption(HashSet<string> optionIds, string optionId, string owner)
        {
            if (!optionIds.Contains(optionId))
            {
                AddError("Sequence prefab is missing required optionId: " + owner + " / " + optionId);
            }
        }

        private static void ValidatePowerDevicePrefab(PuzzleRecord puzzle, GameObject prefab)
        {
            PuzzlePowerDeviceUIBase ui = prefab.GetComponentInChildren<PuzzlePowerDeviceUIBase>(true);
            if (ui == null)
            {
                AddError("PowerDevice puzzle prefab needs PuzzlePowerDeviceUIBase: " + puzzle.puzzleId);
                return;
            }

            string path = puzzle.puzzleId + " / " + puzzle.prefabPath;
            CheckObjectField(ui, "powerButton", "powerButton", path, true);
            CheckObjectField(ui, "resetButton", "resetButton", path, false);
            CheckObjectField(ui, "closeButton", "closeButton", path, false);

            PuzzlePowerSwitchButton[] buttons = prefab.GetComponentsInChildren<PuzzlePowerSwitchButton>(true);
            if (buttons.Length < 3)
            {
                AddError("PowerDevice prefab needs at least 3 PuzzlePowerSwitchButton components: " + path);
            }

            HashSet<string> switchIds = new HashSet<string>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (!string.IsNullOrEmpty(buttons[i].SwitchId))
                {
                    switchIds.Add(buttons[i].SwitchId);
                }
            }

            RequireSwitchId(switchIds, "Switch_Left", path);
            RequireSwitchId(switchIds, "Switch_Right", path);
            RequireSwitchId(switchIds, "Switch_Center", path);

            PuzzleAnswerRecord answer = ResolveAnswer(puzzle);
            if (answer == null || answer.answerSequence == null || answer.answerSequence.Length == 0)
            {
                AddError("PowerDevice puzzle has no answerSequence: " + puzzle.puzzleId);
            }
            else if (answer.answerSequence.Length != 5)
            {
                AddWarning("PowerDevice answerSequence should have 5 inputs: " + path);
            }
        }

        private static void ValidateItemUsePrefab(PuzzleRecord puzzle, GameObject prefab)
        {
            PuzzleItemUseUIBase ui = prefab.GetComponentInChildren<PuzzleItemUseUIBase>(true);
            if (ui == null)
            {
                AddError("ItemUse puzzle prefab needs PuzzleItemUseUIBase when prefabPath is assigned: " + puzzle.puzzleId);
                return;
            }

            string path = puzzle.puzzleId + " / " + puzzle.prefabPath;
            CheckObjectField(ui, "useSelectedItemButton", "useSelectedItemButton", path, true);
            CheckObjectField(ui, "closeButton", "closeButton", path, false);
        }

        private static void ValidateFinalSymbolItemPrefab(PuzzleRecord puzzle, GameObject prefab)
        {
            PuzzleSymbolCycleUIBase ui = prefab.GetComponentInChildren<PuzzleSymbolCycleUIBase>(true);
            if (ui == null)
            {
                AddError("FinalSymbolItem puzzle prefab needs PuzzleSymbolCycleUIBase or PuzzleUI_LockedRoomFinal: " + puzzle.puzzleId);
                return;
            }

            if (puzzle.requiredItemId != "ModifiedClockworkDevice")
            {
                AddWarning("FinalSymbolItem should require ModifiedClockworkDevice: " + puzzle.puzzleId);
            }

            string path = puzzle.puzzleId + " / " + puzzle.prefabPath;
            PuzzleSymbolCycleSlot[] slots = prefab.GetComponentsInChildren<PuzzleSymbolCycleSlot>(true);
            if (slots.Length < 5)
            {
                AddError("FinalSymbolItem prefab needs at least 5 PuzzleSymbolCycleSlot components: " + path);
            }

            PuzzleAnswerRecord answer = ResolveAnswer(puzzle);
            if (answer == null || answer.answerSequence == null || answer.answerSequence.Length != 5)
            {
                AddWarning("FinalSymbolItem answerSequence should have 5 symbols: " + puzzle.puzzleId);
            }
        }

        private static void RequireSwitchId(HashSet<string> switchIds, string switchId, string owner)
        {
            if (!switchIds.Contains(switchId))
            {
                AddError("PowerDevice prefab is missing switchId: " + owner + " / " + switchId);
            }
        }

        private static void BuildAnswerMaps(PuzzleAnswerRecordList answers)
        {
            answerByVariableName.Clear();
            answerByPuzzleId.Clear();

            if (answers == null || answers.answers == null)
            {
                return;
            }

            for (int i = 0; i < answers.answers.Count; i++)
            {
                PuzzleAnswerRecord answer = answers.answers[i];
                if (answer == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(answer.answerVariableName) && !answerByVariableName.ContainsKey(answer.answerVariableName))
                {
                    answerByVariableName.Add(answer.answerVariableName, answer);
                }

                if (!string.IsNullOrEmpty(answer.puzzleId) && !answerByPuzzleId.ContainsKey(answer.puzzleId))
                {
                    answerByPuzzleId.Add(answer.puzzleId, answer);
                }
            }
        }

        private static void BuildSymbolIds(SymbolRecordList symbols)
        {
            symbolIds.Clear();
            if (symbols == null || symbols.symbols == null)
            {
                return;
            }

            for (int i = 0; i < symbols.symbols.Count; i++)
            {
                if (symbols.symbols[i] != null && !string.IsNullOrEmpty(symbols.symbols[i].symbolId))
                {
                    symbolIds.Add(symbols.symbols[i].symbolId);
                }
            }
        }

        private static T LoadJson<T>(string dataPath, string fileName) where T : class
        {
            string path = Path.Combine(dataPath, fileName);
            if (!File.Exists(path))
            {
                AddError("Missing data file: " + fileName);
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch (Exception exception)
            {
                AddError("Exception while reading " + fileName + ": " + exception.Message);
                return null;
            }
        }

        private static void CheckObjectField(UnityEngine.Object target, string fieldName, string label, string owner, bool errorIfNull)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            if (property == null)
            {
                AddWarning("Field not found for contract check: " + target.GetType().Name + "." + fieldName + " / " + owner);
                return;
            }

            if (property.objectReferenceValue != null)
            {
                return;
            }

            if (errorIfNull)
            {
                AddError("Required field is not assigned: " + label + " / " + owner);
            }
            else
            {
                AddWarning("Field is not assigned: " + label + " / " + owner);
            }
        }

        private static bool GetBoolField(UnityEngine.Object target, string fieldName, bool defaultValue)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            return property != null ? property.boolValue : defaultValue;
        }

        private static int GetIntField(UnityEngine.Object target, string fieldName, int defaultValue)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            return property != null ? property.intValue : defaultValue;
        }

        private static string[] GetStringArrayField(UnityEngine.Object target, string fieldName)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            if (property == null || !property.isArray)
            {
                return new string[0];
            }

            string[] values = new string[property.arraySize];
            for (int i = 0; i < property.arraySize; i++)
            {
                values[i] = property.GetArrayElementAtIndex(i).stringValue;
            }

            return values;
        }

        private static void RequireStringInArray(string[] values, string requiredValue, string label, string owner)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == requiredValue)
                {
                    return;
                }
            }

            AddError(label + " is missing required value: " + owner + " / " + requiredValue);
        }

        private static SerializedProperty GetProperty(UnityEngine.Object target, string fieldName)
        {
            if (target == null)
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            return serializedObject.FindProperty(fieldName);
        }

        private static string GetHierarchyPath(GameObject obj)
        {
            if (obj == null)
            {
                return "(null)";
            }

            string path = obj.name;
            Transform current = obj.transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private static void AddError(string message)
        {
            errorCount++;
            Debug.LogError("[PuzzlePrefabContractValidator] " + message);
        }

        private static void AddWarning(string message)
        {
            warningCount++;
            Debug.LogWarning("[PuzzlePrefabContractValidator] " + message);
        }

        private static void AddInfo(string message)
        {
            Debug.Log("[PuzzlePrefabContractValidator] " + message);
        }
    }
}
