using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    public static class GameDataValidator
    {
        private static int errorCount;
        private static int warningCount;

        [MenuItem("Escape From Nightmare/Validate Game Data")]
        public static void ValidateGameData()
        {
            errorCount = 0;
            warningCount = 0;

            string dataPath = Path.Combine(Application.streamingAssetsPath, "Data");

            LocationRecordList locations = LoadJson<LocationRecordList>(dataPath, "locations.json");
            DoorRecordList doors = LoadJson<DoorRecordList>(dataPath, "doors.json");
            ItemRecordList items = LoadJson<ItemRecordList>(dataPath, "items.json");
            PuzzleRecordList puzzles = LoadJson<PuzzleRecordList>(dataPath, "puzzles.json");
            PuzzleAnswerRecordList answers = LoadJson<PuzzleAnswerRecordList>(dataPath, "puzzle_answers.json");
            ClueRecordList clues = LoadJson<ClueRecordList>(dataPath, "clues.json");
            SymbolRecordList symbols = LoadJson<SymbolRecordList>(dataPath, "symbols.json");
            GhostRuleRecordList ghostRules = LoadJson<GhostRuleRecordList>(dataPath, "ghost_rules.json");
            LoadJson<GameSettingsWrapper>(dataPath, "game_settings.json");

            List<LocationRecord> locationList = locations != null && locations.locations != null ? locations.locations : new List<LocationRecord>();
            List<DoorRecord> doorList = doors != null && doors.doors != null ? doors.doors : new List<DoorRecord>();
            List<ItemRecord> itemList = items != null && items.items != null ? items.items : new List<ItemRecord>();
            List<PuzzleRecord> puzzleList = puzzles != null && puzzles.puzzles != null ? puzzles.puzzles : new List<PuzzleRecord>();
            List<PuzzleAnswerRecord> answerList = answers != null && answers.answers != null ? answers.answers : new List<PuzzleAnswerRecord>();
            List<ClueRecord> clueList = clues != null && clues.clues != null ? clues.clues : new List<ClueRecord>();
            List<SymbolRecord> symbolList = symbols != null && symbols.symbols != null ? symbols.symbols : new List<SymbolRecord>();
            List<GhostRuleRecord> ghostRuleList = ghostRules != null && ghostRules.ghostRules != null ? ghostRules.ghostRules : new List<GhostRuleRecord>();

            HashSet<string> locationIds = BuildIdSet(locationList, "Location", record => record != null ? record.locationId : null);
            HashSet<string> doorIds = BuildIdSet(doorList, "Door", record => record != null ? record.doorId : null);
            HashSet<string> itemIds = BuildIdSet(itemList, "Item", record => record != null ? record.itemId : null);
            HashSet<string> puzzleIds = BuildIdSet(puzzleList, "Puzzle", record => record != null ? record.puzzleId : null);
            HashSet<string> clueIds = BuildIdSet(clueList, "Clue", record => record != null ? record.clueId : null);
            HashSet<string> symbolIds = BuildIdSet(symbolList, "Symbol", record => record != null ? record.symbolId : null);
            BuildIdSet(ghostRuleList, "GhostRule", record => record != null ? record.ruleId : null);
            HashSet<string> answerVariableNames = BuildIdSet(answerList, "PuzzleAnswer.answerVariableName", record => record != null ? record.answerVariableName : null);

            Dictionary<string, LocationRecord> locationMap = BuildLocationMap(locationList);
            Dictionary<string, PuzzleRecord> puzzleMap = BuildPuzzleMap(puzzleList);

            ValidateLocations(locationList);
            ValidateDoors(doorList, locationIds, locationMap, itemIds, puzzleIds);
            ValidateItems(itemList);
            ValidatePuzzles(puzzleList, locationIds, itemIds, doorIds, clueIds, answerVariableNames);
            ValidateClues(clueList, locationIds, itemIds, puzzleIds);
            ValidateSymbols(symbolList);
            ValidatePuzzleAnswers(answerList, puzzleIds, puzzleMap);
            ValidateGhostRules(ghostRuleList, locationIds);
            ValidateSymbolsUsedByAnswers(answerList, puzzleMap, symbolIds);
            ValidateSourceAlignment(locationList, doorList, puzzleList, symbolList);

            Debug.Log("Game data validation complete. Errors: " + errorCount + ", Warnings: " + warningCount);
            Debug.Log("[GameDataValidator] Next: run Escape From Nightmare / Validate Current Scene Wiring, then Validate Puzzle Prefab Contracts.");
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
                string json = File.ReadAllText(path);
                T result = JsonUtility.FromJson<T>(json);
                if (result == null)
                {
                    AddError("Failed to parse data file: " + fileName);
                }

                return result;
            }
            catch (Exception exception)
            {
                AddError("Exception while reading " + fileName + ": " + exception.Message);
                return null;
            }
        }

        private static HashSet<string> BuildIdSet<T>(IEnumerable<T> records, string label, Func<T, string> getId)
        {
            HashSet<string> ids = new HashSet<string>();
            if (records == null)
            {
                return ids;
            }

            foreach (T record in records)
            {
                string id = getId(record);
                if (string.IsNullOrEmpty(id))
                {
                    AddError(label + " has an empty ID.");
                    continue;
                }

                if (!ids.Add(id))
                {
                    AddError("Duplicate " + label + " ID: " + id);
                }
            }

            return ids;
        }

        private static Dictionary<string, LocationRecord> BuildLocationMap(IEnumerable<LocationRecord> locations)
        {
            Dictionary<string, LocationRecord> map = new Dictionary<string, LocationRecord>();
            foreach (LocationRecord location in locations)
            {
                if (location != null && !string.IsNullOrEmpty(location.locationId) && !map.ContainsKey(location.locationId))
                {
                    map.Add(location.locationId, location);
                }
            }

            return map;
        }

        private static Dictionary<string, PuzzleRecord> BuildPuzzleMap(IEnumerable<PuzzleRecord> puzzles)
        {
            Dictionary<string, PuzzleRecord> map = new Dictionary<string, PuzzleRecord>();
            foreach (PuzzleRecord puzzle in puzzles)
            {
                if (puzzle != null && !string.IsNullOrEmpty(puzzle.puzzleId) && !map.ContainsKey(puzzle.puzzleId))
                {
                    map.Add(puzzle.puzzleId, puzzle);
                }
            }

            return map;
        }

        private static Dictionary<string, PuzzleAnswerRecord> BuildAnswerByVariableMap(IEnumerable<PuzzleAnswerRecord> answers)
        {
            Dictionary<string, PuzzleAnswerRecord> map = new Dictionary<string, PuzzleAnswerRecord>();
            foreach (PuzzleAnswerRecord answer in answers)
            {
                if (answer != null && !string.IsNullOrEmpty(answer.answerVariableName) && !map.ContainsKey(answer.answerVariableName))
                {
                    map.Add(answer.answerVariableName, answer);
                }
            }

            return map;
        }

        private static void ValidateLocations(IEnumerable<LocationRecord> locations)
        {
            foreach (LocationRecord location in locations)
            {
                if (location == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(location.locationId))
                {
                    AddError("Location has an empty locationId.");
                    continue;
                }

                if (string.IsNullOrEmpty(location.defaultViewId))
                {
                    AddWarning("Location defaultViewId is empty: " + location.locationId);
                }

                if (location.viewIds == null || location.viewIds.Length == 0)
                {
                    AddWarning("Location has no viewIds: " + location.locationId);
                    continue;
                }

                HashSet<string> viewIds = new HashSet<string>();
                bool hasDefaultView = false;
                for (int i = 0; i < location.viewIds.Length; i++)
                {
                    string viewId = location.viewIds[i];
                    if (string.IsNullOrEmpty(viewId))
                    {
                        AddWarning("Location has an empty viewId: " + location.locationId);
                        continue;
                    }

                    if (!viewIds.Add(viewId))
                    {
                        AddWarning("Location has duplicate viewId: " + location.locationId + " / " + viewId);
                    }

                    if (viewId == location.defaultViewId)
                    {
                        hasDefaultView = true;
                    }
                }

                if (!string.IsNullOrEmpty(location.defaultViewId) && !hasDefaultView)
                {
                    AddWarning("Location defaultViewId is not in viewIds: " + location.locationId + " / " + location.defaultViewId);
                }
            }
        }

        private static void ValidateDoors(IEnumerable<DoorRecord> doors, HashSet<string> locationIds, Dictionary<string, LocationRecord> locationMap, HashSet<string> itemIds, HashSet<string> puzzleIds)
        {
            foreach (DoorRecord door in doors)
            {
                if (door == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(door.doorId))
                {
                    AddError("Door has an empty doorId.");
                    continue;
                }

                if (!locationIds.Contains(door.fromLocationId))
                {
                    AddError("Door fromLocationId does not exist: " + door.doorId + " / " + door.fromLocationId);
                }

                if (!locationIds.Contains(door.toLocationId))
                {
                    AddError("Door toLocationId does not exist: " + door.doorId + " / " + door.toLocationId);
                }

                CheckViewReference(locationMap, door.fromLocationId, door.fromViewId, "Door fromViewId", door.doorId);
                CheckViewReference(locationMap, door.toLocationId, door.toViewId, "Door toViewId", door.doorId);

                if (!string.IsNullOrEmpty(door.requiredItemId) && !itemIds.Contains(door.requiredItemId))
                {
                    AddError("Door requiredItemId does not exist: " + door.doorId + " / " + door.requiredItemId);
                }

                if (!string.IsNullOrEmpty(door.requiredPuzzleId) && !puzzleIds.Contains(door.requiredPuzzleId))
                {
                    AddError("Door requiredPuzzleId does not exist: " + door.doorId + " / " + door.requiredPuzzleId);
                }
            }
        }

        private static void ValidateItems(IEnumerable<ItemRecord> items)
        {
            foreach (ItemRecord item in items)
            {
                if (item == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(item.itemId))
                {
                    AddError("Item has an empty itemId.");
                    continue;
                }

                if (string.IsNullOrEmpty(item.displayName))
                {
                    AddWarning("Item displayName is empty: " + item.itemId);
                }

                if (string.IsNullOrEmpty(item.iconPath))
                {
                    AddWarning("Item iconPath is empty: " + item.itemId);
                }
                else if (Resources.Load<Sprite>(item.iconPath) == null)
                {
                    AddWarning("Item icon sprite not found at Resources path: " + item.iconPath);
                }
            }
        }

        private static void ValidatePuzzles(IEnumerable<PuzzleRecord> puzzles, HashSet<string> locationIds, HashSet<string> itemIds, HashSet<string> doorIds, HashSet<string> clueIds, HashSet<string> answerVariableNames)
        {
            foreach (PuzzleRecord puzzle in puzzles)
            {
                if (puzzle == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(puzzle.puzzleId))
                {
                    AddError("Puzzle has an empty puzzleId.");
                    continue;
                }

                if (!locationIds.Contains(puzzle.locationId))
                {
                    AddError("Puzzle locationId does not exist: " + puzzle.puzzleId + " / " + puzzle.locationId);
                }

                if (string.IsNullOrEmpty(puzzle.prefabPath))
                {
                    AddWarning("Puzzle prefabPath is empty: " + puzzle.puzzleId);
                }
                else if (Resources.Load<GameObject>(puzzle.prefabPath) == null)
                {
                    AddWarning("Puzzle prefab not found at Resources path: " + puzzle.prefabPath);
                }

                if (string.IsNullOrEmpty(puzzle.answerVariableName))
                {
                    AddWarning("Puzzle answerVariableName is empty: " + puzzle.puzzleId);
                }
                else if (!answerVariableNames.Contains(puzzle.answerVariableName))
                {
                    AddError("Puzzle answerVariableName has no answer record: " + puzzle.puzzleId + " / " + puzzle.answerVariableName);
                }

                if (!string.IsNullOrEmpty(puzzle.requiredItemId) && !itemIds.Contains(puzzle.requiredItemId))
                {
                    AddError("Puzzle requiredItemId does not exist: " + puzzle.puzzleId + " / " + puzzle.requiredItemId);
                }

                ValidatePuzzleReward(puzzle, itemIds, doorIds, clueIds);
            }
        }

        private static void ValidatePuzzleReward(PuzzleRecord puzzle, HashSet<string> itemIds, HashSet<string> doorIds, HashSet<string> clueIds)
        {
            if (string.IsNullOrEmpty(puzzle.rewardType) || puzzle.rewardType == PuzzleRewardType.None)
            {
                return;
            }

            switch (puzzle.rewardType)
            {
                case PuzzleRewardType.Item:
                    if (!itemIds.Contains(puzzle.rewardId))
                    {
                        AddError("Puzzle Item rewardId does not exist: " + puzzle.puzzleId + " / " + puzzle.rewardId);
                    }
                    break;
                case PuzzleRewardType.Clue:
                    if (!clueIds.Contains(puzzle.rewardId))
                    {
                        AddError("Puzzle Clue rewardId does not exist: " + puzzle.puzzleId + " / " + puzzle.rewardId);
                    }
                    break;
                case PuzzleRewardType.DoorUnlock:
                    if (!doorIds.Contains(puzzle.rewardId))
                    {
                        AddError("Puzzle DoorUnlock rewardId does not exist: " + puzzle.puzzleId + " / " + puzzle.rewardId);
                    }
                    break;
                case PuzzleRewardType.FinalChase:
                case PuzzleRewardType.Ending:
                case PuzzleRewardType.Custom:
                case PuzzleRewardType.StartFinalChase:
                case PuzzleRewardType.ItemTransform:
                    break;
                default:
                    AddWarning("Unknown puzzle rewardType: " + puzzle.puzzleId + " / " + puzzle.rewardType);
                    break;
            }
        }

        private static void ValidateClues(IEnumerable<ClueRecord> clues, HashSet<string> locationIds, HashSet<string> itemIds, HashSet<string> puzzleIds)
        {
            foreach (ClueRecord clue in clues)
            {
                if (clue == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(clue.clueId))
                {
                    AddError("Clue has an empty clueId.");
                    continue;
                }

                if (!string.IsNullOrEmpty(clue.locationId) && !locationIds.Contains(clue.locationId))
                {
                    AddError("Clue locationId does not exist: " + clue.clueId + " / " + clue.locationId);
                }

                if (!string.IsNullOrEmpty(clue.requiredPuzzleId) && !puzzleIds.Contains(clue.requiredPuzzleId))
                {
                    AddError("Clue requiredPuzzleId does not exist: " + clue.clueId + " / " + clue.requiredPuzzleId);
                }

                if (!string.IsNullOrEmpty(clue.requiredItemId) && !itemIds.Contains(clue.requiredItemId))
                {
                    AddError("Clue requiredItemId does not exist: " + clue.clueId + " / " + clue.requiredItemId);
                }

                if (string.IsNullOrEmpty(clue.imagePath))
                {
                    AddWarning("Clue imagePath is empty: " + clue.clueId);
                }
                else if (Resources.Load<Sprite>(clue.imagePath) == null)
                {
                    AddWarning("Clue image sprite not found at Resources path: " + clue.imagePath);
                }
            }
        }

        private static void ValidateSymbols(IEnumerable<SymbolRecord> symbols)
        {
            foreach (SymbolRecord symbol in symbols)
            {
                if (symbol == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(symbol.symbolId))
                {
                    AddError("Symbol has an empty symbolId.");
                    continue;
                }

                if (string.IsNullOrEmpty(symbol.displayName))
                {
                    AddWarning("Symbol displayName is empty: " + symbol.symbolId);
                }

                if (string.IsNullOrEmpty(symbol.spritePath))
                {
                    AddWarning("Symbol spritePath is empty: " + symbol.symbolId);
                }
                else if (Resources.Load<Sprite>(symbol.spritePath) == null)
                {
                    AddWarning("Symbol sprite not found at Resources path: " + symbol.spritePath);
                }
            }
        }

        private static void ValidatePuzzleAnswers(IEnumerable<PuzzleAnswerRecord> answers, HashSet<string> puzzleIds, Dictionary<string, PuzzleRecord> puzzleMap)
        {
            foreach (PuzzleAnswerRecord answer in answers)
            {
                if (answer == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(answer.puzzleId) && !puzzleIds.Contains(answer.puzzleId))
                {
                    AddError("PuzzleAnswer puzzleId does not exist: " + answer.puzzleId);
                }

                if (string.IsNullOrEmpty(answer.answerVariableName))
                {
                    AddWarning("PuzzleAnswer answerVariableName is empty for puzzle: " + answer.puzzleId);
                }

                bool hasText = !string.IsNullOrEmpty(answer.answerText);
                bool hasSequence = answer.answerSequence != null && answer.answerSequence.Length > 0;
                if (!hasText && !hasSequence)
                {
                    AddWarning("PuzzleAnswer has neither answerText nor answerSequence: " + answer.answerVariableName);
                }

                PuzzleRecord puzzle;
                if (!string.IsNullOrEmpty(answer.puzzleId) && puzzleMap.TryGetValue(answer.puzzleId, out puzzle))
                {
                    if (IsSequenceType(puzzle.type) && !hasSequence)
                    {
                        AddWarning("Sequence puzzle answerSequence is empty: " + puzzle.puzzleId);
                    }

                    if (IsNumberCodeType(puzzle.type) && !hasText)
                    {
                        AddWarning("NumberCode puzzle answerText is empty: " + puzzle.puzzleId);
                    }
                }
            }
        }

        private static void ValidateGhostRules(IEnumerable<GhostRuleRecord> ghostRules, HashSet<string> locationIds)
        {
            foreach (GhostRuleRecord rule in ghostRules)
            {
                if (rule == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(rule.ruleId))
                {
                    AddError("GhostRule has an empty ruleId.");
                    continue;
                }

                if (!locationIds.Contains(rule.locationId))
                {
                    AddError("GhostRule locationId does not exist: " + rule.ruleId + " / " + rule.locationId);
                }

                if (rule.minArrivalTime > rule.maxArrivalTime)
                {
                    AddWarning("GhostRule minArrivalTime is greater than maxArrivalTime: " + rule.ruleId);
                }

                if (rule.minLeaveTime > rule.maxLeaveTime)
                {
                    AddWarning("GhostRule minLeaveTime is greater than maxLeaveTime: " + rule.ruleId);
                }

                if (rule.dangerIncreasePerSecond <= 0f)
                {
                    AddWarning("GhostRule dangerIncreasePerSecond should be greater than 0: " + rule.ruleId);
                }
            }
        }

        private static void ValidateSymbolsUsedByAnswers(IEnumerable<PuzzleAnswerRecord> answers, Dictionary<string, PuzzleRecord> puzzleMap, HashSet<string> symbolIds)
        {
            foreach (PuzzleAnswerRecord answer in answers)
            {
                if (answer == null || answer.answerSequence == null || string.IsNullOrEmpty(answer.puzzleId))
                {
                    continue;
                }

                PuzzleRecord puzzle;
                if (!puzzleMap.TryGetValue(answer.puzzleId, out puzzle) || puzzle == null || (puzzle.type != "SymbolSequence" && puzzle.type != "SymbolCycle" && puzzle.type != "FinalSymbolItem"))
                {
                    continue;
                }

                for (int i = 0; i < answer.answerSequence.Length; i++)
                {
                    string optionId = answer.answerSequence[i];
                    if (!string.IsNullOrEmpty(optionId) && !symbolIds.Contains(optionId))
                    {
                        AddError("SymbolSequence answer option is not in symbols.json: " + answer.puzzleId + " / " + optionId);
                    }
                }
            }
        }

        private static void CheckViewReference(Dictionary<string, LocationRecord> locationMap, string locationId, string viewId, string label, string ownerId)
        {
            if (string.IsNullOrEmpty(viewId))
            {
                return;
            }

            LocationRecord location;
            if (!locationMap.TryGetValue(locationId, out location) || location == null || location.viewIds == null)
            {
                return;
            }

            for (int i = 0; i < location.viewIds.Length; i++)
            {
                if (location.viewIds[i] == viewId)
                {
                    return;
                }
            }

            AddWarning(label + " is not in location viewIds: " + ownerId + " / " + locationId + " / " + viewId);
        }

        private static bool IsSequenceType(string type)
        {
            return type == "Sequence" || type == "SymbolSequence" || type == "SymbolCycle" || type == "FinalSequence" || type == "FinalSymbolItem" || type == "PowerDevice";
        }

        private static bool IsNumberCodeType(string type)
        {
            return type == "NumberCode";
        }

        private static void AddError(string message)
        {
            errorCount++;
            Debug.LogError("[GameDataValidator] " + message);
        }

        private static void AddWarning(string message)
        {
            warningCount++;
            Debug.LogWarning("[GameDataValidator] " + message);
        }

        private static void ValidateSourceAlignment(IEnumerable<LocationRecord> locations, IEnumerable<DoorRecord> doors, IEnumerable<PuzzleRecord> puzzles, IEnumerable<SymbolRecord> symbols)
        {
            foreach (LocationRecord location in locations)
            {
                if (location == null)
                {
                    continue;
                }

                if (location.locationId == "Hallway")
                {
                    AddWarning("Legacy locationId found: Hallway. Use SecondFloorHallway.");
                }

                if (location.locationId == "Basement")
                {
                    AddWarning("Legacy locationId found: Basement. Use BasementStorage.");
                }
            }

            foreach (DoorRecord door in doors)
            {
                if (door == null || string.IsNullOrEmpty(door.doorId))
                {
                    continue;
                }

                if (door.doorId.Contains("Hallway"))
                {
                    AddWarning("Legacy doorId may reference Hallway. Use SecondFloorHallway naming: " + door.doorId);
                }

                if (door.doorId.Contains("Basement") && !door.doorId.Contains("BasementStorage"))
                {
                    AddWarning("Legacy doorId may reference Basement. Use BasementStorage naming: " + door.doorId);
                }
            }

            foreach (SymbolRecord symbol in symbols)
            {
                if (symbol == null)
                {
                    continue;
                }

                if (symbol.symbolId == "Symbol_Moon" || symbol.symbolId == "Symbol_Eye" || symbol.symbolId == "Symbol_Key")
                {
                    AddWarning("Legacy symbolId found: " + symbol.symbolId + ". Use Symbol_01 through Symbol_06 source-aligned IDs.");
                }
            }

            foreach (PuzzleRecord puzzle in puzzles)
            {
                if (puzzle == null)
                {
                    continue;
                }

                if (puzzle.puzzleId == "Puzzle_BasementPower_01")
                {
                    AddWarning("Legacy puzzleId found: Puzzle_BasementPower_01. Use Puzzle_BasementStorage_01.");
                }

                if (puzzle.puzzleId == "Puzzle_LockedRoomFinal_01")
                {
                    AddWarning("Legacy puzzleId found: Puzzle_LockedRoomFinal_01. Use Puzzle_LockedRoom_01.");
                }

                if (puzzle.puzzleId == "Puzzle_EntranceDoor_01")
                {
                    AddWarning("Legacy puzzleId found: Puzzle_EntranceDoor_01. Use Puzzle_Entrance_01.");
                }

                if (puzzle.puzzleId == "Puzzle_Kitchen_01" && puzzle.rewardId == "FrontDoorKey")
                {
                    AddError("Source-aligned Kitchen reward should be BasementFuse, not FrontDoorKey.");
                }

                if (puzzle.rewardId == "FrontDoorKey" && puzzle.puzzleId != "Puzzle_LockedRoom_01")
                {
                    AddWarning("FrontDoorKey should normally be rewarded by Puzzle_LockedRoom_01. Found on: " + puzzle.puzzleId);
                }

                if (puzzle.puzzleId == "Puzzle_BasementStorage_01" && puzzle.requiredItemId != "BasementFuse")
                {
                    AddWarning("Puzzle_BasementStorage_01 should require BasementFuse through requiredItemId.");
                }

                if (puzzle.puzzleId == "Puzzle_LockedRoom_01" && puzzle.requiredItemId != "ModifiedClockworkDevice")
                {
                    AddWarning("Puzzle_LockedRoom_01 should require ModifiedClockworkDevice.");
                }
            }
        }
    }
}
