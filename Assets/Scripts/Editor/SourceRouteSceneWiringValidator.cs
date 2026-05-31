// -----------------------------------------------------------------------------
// Codex comment pass: Scene Wiring Validator
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\SourceRouteSceneWiringValidator.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Editor utility for the Scene Wiring Validator workflow, exposed through menu items or called by other validation tools.
    public static class SourceRouteSceneWiringValidator
    {
        // Stores the errors value used by this script's runtime or editor workflow.
        private static readonly List<string> errors = new List<string>();
        // Stores the warnings value used by this script's runtime or editor workflow.
        private static readonly List<string> warnings = new List<string>();
        // Stores the infos value used by this script's runtime or editor workflow.
        private static readonly List<string> infos = new List<string>();
        private static readonly Dictionary<string, List<string>> doorButtonPaths = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<string>> puzzleButtonPaths = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<string>> clueButtonPaths = new Dictionary<string, List<string>>();
        // Stores the final Door Paths value used by this script's runtime or editor workflow.
        private static readonly List<string> finalDoorPaths = new List<string>();
        private static readonly Dictionary<string, LocationController> locationMap = new Dictionary<string, LocationController>();
        private static readonly Dictionary<string, HashSet<string>> locationViewMap = new Dictionary<string, HashSet<string>>();

        // Stores the door Ids value used by this script's runtime or editor workflow.
        private static HashSet<string> doorIds = new HashSet<string>();
        // Stores the puzzle Ids value used by this script's runtime or editor workflow.
        private static HashSet<string> puzzleIds = new HashSet<string>();
        // Stores the clue Ids value used by this script's runtime or editor workflow.
        private static HashSet<string> clueIds = new HashSet<string>();
        // Stores the item Ids value used by this script's runtime or editor workflow.
        private static HashSet<string> itemIds = new HashSet<string>();
        private static Dictionary<string, PuzzleRecord> puzzleMap = new Dictionary<string, PuzzleRecord>();

        // Stores the Required Managers value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredManagers =
        {
            "GameDataManager",
            "LocationManager",
            "InteractionManager",
            "InventoryManager",
            "PuzzleManager",
            "SaveManager",
            "GameManager",
            "EndingManager",
            "ClueImageManager",
            "NoiseManager",
            "GhostManager",
            "HideManager",
            "ChaseManager",
            "ScreenFadeManager"
        };

        // Stores the Required Locations value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredLocations =
        {
            "Bedroom",
            "ChildRoom",
            "Study",
            "SecondFloorHallway",
            "LivingRoom",
            "Entrance",
            "Kitchen",
            "BasementStorage",
            "LockedRoom"
        };

        // Stores the Required Views value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredViews =
        {
            "Bedroom_Front", "Bedroom_Back",
            "ChildRoom_Front", "ChildRoom_Back",
            "Study_Front", "Study_Right", "Study_Back", "Study_Left",
            "SecondFloorHallway_Front", "SecondFloorHallway_Back",
            "LivingRoom_Front", "LivingRoom_Back",
            "Entrance_Front",
            "Kitchen_Front",
            "BasementStorage_Front", "BasementStorage_Right", "BasementStorage_Back", "BasementStorage_Left",
            "LockedRoom_Front", "LockedRoom_Right", "LockedRoom_Back", "LockedRoom_Left"
        };

        // Stores the Required Doors value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredDoors =
        {
            "Door_Bedroom_SecondFloorHallway",
            "Door_SecondFloorHallway_Bedroom",
            "Door_SecondFloorHallway_ChildRoom",
            "Door_ChildRoom_SecondFloorHallway",
            "Door_SecondFloorHallway_Study",
            "Door_Study_SecondFloorHallway",
            "Door_SecondFloorHallway_LivingRoom",
            "Door_LivingRoom_SecondFloorHallway",
            "Door_LivingRoom_Kitchen",
            "Door_Kitchen_LivingRoom",
            "Door_Kitchen_BasementStorage",
            "Door_BasementStorage_Kitchen",
            "Door_BasementStorage_LockedRoom",
            "Door_LockedRoom_BasementStorage",
            "Door_LivingRoom_Entrance",
            "Door_Entrance_LivingRoom"
        };

        // Stores the Required Puzzles value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredPuzzles =
        {
            "Puzzle_Bedroom_01",
            "Puzzle_LivingRoom_01",
            "Puzzle_ChildRoom_01",
            "Puzzle_Study_01",
            "Puzzle_LivingRoom_02",
            "Puzzle_Kitchen_01",
            "Puzzle_BasementStorage_01",
            "Puzzle_LockedRoom_01",
            "Puzzle_Entrance_01"
        };

        // Stores the Recommended Clues value used by this script's runtime or editor workflow.
        private static readonly string[] RecommendedClues =
        {
            "BedroomPhotoCodeClue",
            "LivingRoomEntranceCodeClue",
            "ChildRoomCardSymbolClueImage",
            "StudyBookSymbolClueImage",
            "KitchenCodeClueImage",
            "KitchenFridgeSurfaceSymbolClue",
            "BasementPowerPatternClue",
            "BasementClueImage"
        };

        [MenuItem("Escape From Nightmare/Validate Source Route Scene Wiring")]
        // Checks scene, prefab, resource, or data requirements and records any issues found.
        public static void ValidateSourceRouteSceneWiring()
        {
            Reset();
            LoadData();
            CollectSceneMaps();
            ValidateActiveScene();
            ValidateManagers();
            ValidateRootsAndPanels();
            ValidateLocationsAndViews();
            ValidateButtons();
            ValidateVisualPolishComponents();
            ValidateRequiredDoors();
            ValidateRequiredPuzzles();
            ValidateRecommendedClues();
            ValidateFinalDoorOrEntrance();
            WriteReport();

            Debug.Log("Source route scene wiring validation completed. Errors: " + errors.Count + ", Warnings: " + warnings.Count);
        }

        // Provides safe default Inspector values when the component is first attached.
        private static void Reset()
        {
            errors.Clear();
            warnings.Clear();
            infos.Clear();
            doorButtonPaths.Clear();
            puzzleButtonPaths.Clear();
            clueButtonPaths.Clear();
            finalDoorPaths.Clear();
            locationMap.Clear();
            locationViewMap.Clear();
            doorIds = new HashSet<string>();
            puzzleIds = new HashSet<string>();
            clueIds = new HashSet<string>();
            itemIds = new HashSet<string>();
            puzzleMap = new Dictionary<string, PuzzleRecord>();
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        private static void LoadData()
        {
            string dataPath = Path.Combine(Application.streamingAssetsPath, "Data");
            DoorRecordList doors = LoadJson<DoorRecordList>(dataPath, "doors.json");
            PuzzleRecordList puzzles = LoadJson<PuzzleRecordList>(dataPath, "puzzles.json");
            ClueRecordList clues = LoadJson<ClueRecordList>(dataPath, "clues.json");
            ItemRecordList items = LoadJson<ItemRecordList>(dataPath, "items.json");

            if (doors != null && doors.doors != null)
            {
                for (int i = 0; i < doors.doors.Count; i++)
                {
                    if (doors.doors[i] != null && !string.IsNullOrEmpty(doors.doors[i].doorId))
                    {
                        doorIds.Add(doors.doors[i].doorId);
                    }
                }
            }

            if (puzzles != null && puzzles.puzzles != null)
            {
                for (int i = 0; i < puzzles.puzzles.Count; i++)
                {
                    PuzzleRecord puzzle = puzzles.puzzles[i];
                    if (puzzle != null && !string.IsNullOrEmpty(puzzle.puzzleId))
                    {
                        puzzleIds.Add(puzzle.puzzleId);
                        if (!puzzleMap.ContainsKey(puzzle.puzzleId))
                        {
                            puzzleMap.Add(puzzle.puzzleId, puzzle);
                        }
                    }
                }
            }

            if (clues != null && clues.clues != null)
            {
                for (int i = 0; i < clues.clues.Count; i++)
                {
                    if (clues.clues[i] != null && !string.IsNullOrEmpty(clues.clues[i].clueId))
                    {
                        clueIds.Add(clues.clues[i].clueId);
                    }
                }
            }

            if (items != null && items.items != null)
            {
                for (int i = 0; i < items.items.Count; i++)
                {
                    if (items.items[i] != null && !string.IsNullOrEmpty(items.items[i].itemId))
                    {
                        itemIds.Add(items.items[i].itemId);
                    }
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
                AddError("Could not parse " + fileName + ": " + exception.Message);
                return null;
            }
        }

        // Performs the Collect Scene Maps operation while keeping its implementation details inside this script.
        private static void CollectSceneMaps()
        {
            LocationController[] locations = FindSceneObjects<LocationController>();
            for (int i = 0; i < locations.Length; i++)
            {
                LocationController location = locations[i];
                if (location == null || string.IsNullOrEmpty(location.LocationId))
                {
                    continue;
                }

                if (!locationMap.ContainsKey(location.LocationId))
                {
                    locationMap.Add(location.LocationId, location);
                }

                HashSet<string> views = new HashSet<string>();
                LocationView[] foundViews = location.GetComponentsInChildren<LocationView>(true);
                for (int j = 0; j < foundViews.Length; j++)
                {
                    if (foundViews[j] != null && !string.IsNullOrEmpty(foundViews[j].ViewId))
                    {
                        views.Add(foundViews[j].ViewId);
                    }
                }

                locationViewMap[location.LocationId] = views;
            }

            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                ClickableButton button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                string path = GetHierarchyPath(button.gameObject);
                if (button.ClickableType == ClickableType.Door && !string.IsNullOrEmpty(button.LinkedDoorId))
                {
                    AddPath(doorButtonPaths, button.LinkedDoorId, path);
                    if (!doorIds.Contains(button.LinkedDoorId))
                    {
                        AddError("Door button references missing doorId: " + button.LinkedDoorId + " / " + path);
                    }
                }
                else if (button.ClickableType == ClickableType.Puzzle && !string.IsNullOrEmpty(button.LinkedPuzzleId))
                {
                    AddPath(puzzleButtonPaths, button.LinkedPuzzleId, path);
                    if (!puzzleIds.Contains(button.LinkedPuzzleId))
                    {
                        AddError("Puzzle button references missing puzzleId: " + button.LinkedPuzzleId + " / " + path);
                    }
                }
                else if (button.ClickableType == ClickableType.ExamineImage)
                {
                    string clueId = !string.IsNullOrEmpty(button.LinkedClueImageId) ? button.LinkedClueImageId : button.TargetObjectId;
                    if (!string.IsNullOrEmpty(clueId))
                    {
                        AddPath(clueButtonPaths, clueId, path);
                        if (!clueIds.Contains(clueId))
                        {
                            AddError("ExamineImage button references missing clueId: " + clueId + " / " + path);
                        }
                    }
                }
                else if (button.ClickableType == ClickableType.FinalDoor)
                {
                    finalDoorPaths.Add(path + " requiredItemId=" + button.RequiredItemId + " linkedPuzzleId=" + button.LinkedPuzzleId);
                    if (!string.IsNullOrEmpty(button.RequiredItemId) && !itemIds.Contains(button.RequiredItemId))
                    {
                        AddError("FinalDoor references missing requiredItemId: " + button.RequiredItemId + " / " + path);
                    }
                }
                else if (button.ClickableType == ClickableType.PickupItem && string.IsNullOrEmpty(button.LinkedItemId))
                {
                    AddError("PickupItem button has empty linkedItemId: " + path);
                }
                else if (button.ClickableType == ClickableType.UseItemTarget)
                {
                    ValidateUseItemTarget(button, path);
                }
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateUseItemTarget(ClickableButton button, string path)
        {
            if (string.IsNullOrEmpty(button.RequiredItemId))
            {
                AddError("UseItemTarget has empty requiredItemId: " + path);
            }
            else if (!itemIds.Contains(button.RequiredItemId))
            {
                AddError("UseItemTarget references missing itemId: " + button.RequiredItemId + " / " + path);
            }

            if (string.IsNullOrEmpty(button.LinkedPuzzleId)
                && string.IsNullOrEmpty(button.LinkedClueImageId)
                && string.IsNullOrEmpty(button.LinkedDoorId)
                && string.IsNullOrEmpty(button.LinkedItemId))
            {
                AddWarning("UseItemTarget has no linked result: " + path);
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != "GameScene")
            {
                AddWarning("Active Scene is not GameScene: " + activeScene.name);
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateManagers()
        {
            for (int i = 0; i < RequiredManagers.Length; i++)
            {
                int count = CountComponentByName(RequiredManagers[i]);
                if (count == 0)
                {
                    if (RequiredManagers[i] == "SaveManager" || RequiredManagers[i] == "GameManager")
                    {
                        AddWarning("Manager may be provided by TitleScene DontDestroyOnLoad, but is not in active scene: " + RequiredManagers[i]);
                    }
                    else
                    {
                        AddError("Required manager missing from active scene: " + RequiredManagers[i]);
                    }
                }
                else if (count > 1)
                {
                    AddWarning("Manager appears multiple times: " + RequiredManagers[i] + " / count=" + count);
                }
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateRootsAndPanels()
        {
            PuzzleManager[] puzzleManagers = FindSceneObjects<PuzzleManager>();
            if (puzzleManagers.Length == 0 || puzzleManagers[0].puzzleUiRoot == null)
            {
                AddError("PuzzleManager.puzzleUiRoot is missing.");
            }

            LocationManager[] locationManagers = FindSceneObjects<LocationManager>();
            if (locationManagers.Length == 0 || GetObjectField(locationManagers[0], "locationRoot") == null)
            {
                AddError("LocationManager.locationRoot is missing.");
            }

            RequireComponent<InventoryBarUI>("InventoryBarUI", true);
            RequireComponent<ClueImagePanelUI>("ClueImagePanelUI", true);
            RequireComponent<GameOverPanelUI>("GameOverPanelUI", true);
            RequireComponent<EndingPanelUI>("EndingPanelUI", true);
            RequireComponent<HideInteriorViewUI>("HideInteriorViewUI", true);
            RequireComponent<HideExitButton>("HideExitButton", true);
            RequireComponent<InteractionInputGate>("InteractionInputGate", true);
            RequireComponent<GhostStatusUI>("GhostStatusUI", false);

            ScreenFadeManager[] fadeManagers = FindSceneObjects<ScreenFadeManager>();
            if (fadeManagers.Length == 0 || GetObjectField(fadeManagers[0], "fadeCanvasGroup") == null)
            {
                AddError("ScreenFadeManager.fadeCanvasGroup is missing.");
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateLocationsAndViews()
        {
            for (int i = 0; i < RequiredLocations.Length; i++)
            {
                if (!locationMap.ContainsKey(RequiredLocations[i]))
                {
                    AddError("Required LocationController missing: " + RequiredLocations[i]);
                }
            }

            for (int i = 0; i < RequiredViews.Length; i++)
            {
                if (!HasView(RequiredViews[i]))
                {
                    AddError("Required LocationView missing: " + RequiredViews[i]);
                }
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static bool HasView(string viewId)
        {
            foreach (KeyValuePair<string, HashSet<string>> pair in locationViewMap)
            {
                if (pair.Value.Contains(viewId))
                {
                    return true;
                }
            }

            return false;
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateButtons()
        {
            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            if (buttons.Length == 0)
            {
                AddError("No ClickableButton components found in active scene.");
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateVisualPolishComponents()
        {
            LocationView[] views = FindSceneObjects<LocationView>();
            for (int i = 0; i < views.Length; i++)
            {
                if (views[i] != null && views[i].GetComponent<ViewBackgroundBinding>() == null)
                {
                    AddWarning("LocationView is missing ViewBackgroundBinding: " + views[i].ViewId + " / " + GetHierarchyPath(views[i].gameObject));
                }
            }

            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null && buttons[i].GetComponent<HotspotButtonVisual>() == null)
                {
                    AddWarning("ClickableButton is missing HotspotButtonVisual: " + GetHierarchyPath(buttons[i].gameObject));
                }
            }

            if (FindSceneObjects<DebugHotspotOverlay>().Length == 0)
            {
                AddWarning("DebugHotspotOverlay is missing from the active scene.");
            }

            if (!HasPanelPreset<ClueImagePanelUI>("ClueImagePanelUI"))
            {
                AddWarning("ClueImagePanelUI is missing PanelVisualPreset.");
            }
            if (!HasPanelPreset<GameOverPanelUI>("GameOverPanelUI"))
            {
                AddWarning("GameOverPanelUI is missing PanelVisualPreset.");
            }
            if (!HasPanelPreset<EndingPanelUI>("EndingPanelUI"))
            {
                AddWarning("EndingPanelUI is missing PanelVisualPreset.");
            }
        }

        private static bool HasPanelPreset<T>(string label) where T : Component
        {
            T[] panels = FindSceneObjects<T>();
            if (panels.Length == 0)
            {
                return true;
            }

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] != null && panels[i].GetComponent<PanelVisualPreset>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateRequiredDoors()
        {
            for (int i = 0; i < RequiredDoors.Length; i++)
            {
                if (!doorButtonPaths.ContainsKey(RequiredDoors[i]))
                {
                    AddError("Required Door button missing: " + RequiredDoors[i]);
                }
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateRequiredPuzzles()
        {
            for (int i = 0; i < RequiredPuzzles.Length; i++)
            {
                if (RequiredPuzzles[i] == "Puzzle_Entrance_01" && HasFinalDoorForEntrance())
                {
                    continue;
                }

                if (!puzzleButtonPaths.ContainsKey(RequiredPuzzles[i]))
                {
                    AddError("Required Puzzle button missing: " + RequiredPuzzles[i]);
                    continue;
                }

                PuzzleRecord puzzle;
                if (puzzleMap.TryGetValue(RequiredPuzzles[i], out puzzle))
                {
                    if (string.IsNullOrEmpty(puzzle.prefabPath) || Resources.Load<GameObject>(puzzle.prefabPath) == null)
                    {
                        AddError("Required Puzzle prefab does not load: " + RequiredPuzzles[i] + " / " + puzzle.prefabPath);
                    }
                }
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateRecommendedClues()
        {
            for (int i = 0; i < RecommendedClues.Length; i++)
            {
                if (!clueButtonPaths.ContainsKey(RecommendedClues[i]))
                {
                    AddWarning("Recommended ExamineImage button missing: " + RecommendedClues[i]);
                }
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateFinalDoorOrEntrance()
        {
            bool hasEntrancePuzzle = puzzleButtonPaths.ContainsKey("Puzzle_Entrance_01");
            bool hasFinalDoor = HasFinalDoorForEntrance();
            if (!hasEntrancePuzzle && !hasFinalDoor)
            {
                AddError("Entrance ending wiring missing: add Puzzle_Entrance_01 button or FinalDoor requiring FrontDoorKey.");
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static bool HasFinalDoorForEntrance()
        {
            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                ClickableButton button = buttons[i];
                if (button != null
                    && button.ClickableType == ClickableType.FinalDoor
                    && button.RequiredItemId == "FrontDoorKey"
                    && (string.IsNullOrEmpty(button.LinkedPuzzleId) || button.LinkedPuzzleId == "Puzzle_Entrance_01"))
                {
                    return true;
                }
            }

            return false;
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
        private static void WriteReport()
        {
            string path = Path.Combine(Application.dataPath, "Docs/GeneratedSourceRouteSceneWiringReport.md");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# Source Route Scene Wiring Report");
            builder.AppendLine();
            builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendLine("- Active Scene: " + SceneManager.GetActiveScene().name);
            builder.AppendLine("- Errors: " + errors.Count);
            builder.AppendLine("- Warnings: " + warnings.Count);
            builder.AppendLine();
            AppendManagers(builder);
            AppendLocations(builder);
            AppendButtonTable(builder, "Required Door Buttons", "Door ID", RequiredDoors, doorButtonPaths);
            AppendPuzzleButtons(builder);
            AppendButtonTable(builder, "Recommended ExamineImage Buttons", "Clue ID", RecommendedClues, clueButtonPaths);
            builder.AppendLine("## Final Door / Entrance");
            builder.AppendLine();
            builder.AppendLine("| Requirement | Found | Result | Notes |");
            builder.AppendLine("|---|---:|---|---|");
            builder.AppendLine("| Puzzle_Entrance_01 or FinalDoor FrontDoorKey | " + (HasFinalDoorForEntrance() || puzzleButtonPaths.ContainsKey("Puzzle_Entrance_01") ? "1" : "0") + " | " + (HasFinalDoorForEntrance() || puzzleButtonPaths.ContainsKey("Puzzle_Entrance_01") ? "OK" : "Missing") + " | " + EscapeMarkdown(string.Join("; ", finalDoorPaths.ToArray())) + " |");
            builder.AppendLine();
            builder.AppendLine("## Missing Wiring");
            builder.AppendLine();
            AppendList(builder, "Errors", errors);
            AppendList(builder, "Warnings", warnings);
            builder.AppendLine();
            builder.AppendLine("## Notes");
            builder.AppendLine();
            builder.AppendLine("- Runtime Full Route Test can pass even if Scene wiring is incomplete because it uses direct manager calls.");
            builder.AppendLine("- This report validates manual GameScene wiring readiness.");
            builder.AppendLine("- Do not auto-create Scene buttons from this report; wire them manually.");
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.Refresh();
            Debug.Log("[SourceRouteSceneWiringValidator] Report written: " + path);
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendManagers(StringBuilder builder)
        {
            builder.AppendLine("## Managers");
            builder.AppendLine();
            builder.AppendLine("| Manager | Count | Result | Notes |");
            builder.AppendLine("|---|---:|---|---|");
            for (int i = 0; i < RequiredManagers.Length; i++)
            {
                int count = CountComponentByName(RequiredManagers[i]);
                string result = count > 0 ? "Found" : "Missing";
                builder.AppendLine("| " + RequiredManagers[i] + " | " + count + " | " + result + " |  |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendLocations(StringBuilder builder)
        {
            builder.AppendLine("## Location Controllers");
            builder.AppendLine();
            builder.AppendLine("| Location ID | Found | Views Found | Result |");
            builder.AppendLine("|---|---:|---:|---|");
            for (int i = 0; i < RequiredLocations.Length; i++)
            {
                bool found = locationMap.ContainsKey(RequiredLocations[i]);
                int viewCount = locationViewMap.ContainsKey(RequiredLocations[i]) ? locationViewMap[RequiredLocations[i]].Count : 0;
                builder.AppendLine("| " + RequiredLocations[i] + " | " + (found ? "1" : "0") + " | " + viewCount + " | " + (found ? "Found" : "Missing") + " |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendButtonTable(StringBuilder builder, string title, string idLabel, string[] requiredIds, Dictionary<string, List<string>> paths)
        {
            builder.AppendLine("## " + title);
            builder.AppendLine();
            builder.AppendLine("| " + idLabel + " | Found | Button Path | Result |");
            builder.AppendLine("|---|---:|---|---|");
            for (int i = 0; i < requiredIds.Length; i++)
            {
                List<string> foundPaths;
                bool found = paths.TryGetValue(requiredIds[i], out foundPaths);
                builder.AppendLine("| " + requiredIds[i] + " | " + (found ? foundPaths.Count : 0) + " | " + EscapeMarkdown(found ? string.Join("; ", foundPaths.ToArray()) : string.Empty) + " | " + (found ? "Found" : "Missing") + " |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendPuzzleButtons(StringBuilder builder)
        {
            builder.AppendLine("## Required Puzzle Buttons");
            builder.AppendLine();
            builder.AppendLine("| Puzzle ID | Found | Button Path | Prefab Load | Result |");
            builder.AppendLine("|---|---:|---|---|---|");
            for (int i = 0; i < RequiredPuzzles.Length; i++)
            {
                List<string> foundPaths;
                bool found = puzzleButtonPaths.TryGetValue(RequiredPuzzles[i], out foundPaths);
                PuzzleRecord puzzle;
                string prefabLoad = "N/A";
                if (puzzleMap.TryGetValue(RequiredPuzzles[i], out puzzle))
                {
                    prefabLoad = !string.IsNullOrEmpty(puzzle.prefabPath) && Resources.Load<GameObject>(puzzle.prefabPath) != null ? "Loaded" : "Missing";
                }
                builder.AppendLine("| " + RequiredPuzzles[i] + " | " + (found ? foundPaths.Count : 0) + " | " + EscapeMarkdown(found ? string.Join("; ", foundPaths.ToArray()) : string.Empty) + " | " + prefabLoad + " | " + (found ? "Found" : "Missing") + " |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendList(StringBuilder builder, string title, List<string> values)
        {
            builder.AppendLine("### " + title);
            builder.AppendLine();
            if (values.Count == 0)
            {
                builder.AppendLine("- None");
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    builder.AppendLine("- " + values[i]);
                }
            }
            builder.AppendLine();
        }

        // Performs the Add Path operation while keeping its implementation details inside this script.
        private static void AddPath(Dictionary<string, List<string>> map, string key, string path)
        {
            if (!map.ContainsKey(key))
            {
                map.Add(key, new List<string>());
            }
            map[key].Add(path);
        }

        private static void RequireComponent<T>(string label, bool errorIfMissing) where T : UnityEngine.Object
        {
            if (FindSceneObjects<T>().Length > 0)
            {
                return;
            }
            if (errorIfMissing)
            {
                AddError(label + " is missing.");
            }
            else
            {
                AddWarning(label + " is missing.");
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int CountComponentByName(string typeName)
        {
            switch (typeName)
            {
                case "GameDataManager": return FindSceneObjects<GameDataManager>().Length;
                case "LocationManager": return FindSceneObjects<LocationManager>().Length;
                case "InteractionManager": return FindSceneObjects<InteractionManager>().Length;
                case "InventoryManager": return FindSceneObjects<InventoryManager>().Length;
                case "PuzzleManager": return FindSceneObjects<PuzzleManager>().Length;
                case "SaveManager": return FindSceneObjects<SaveManager>().Length;
                case "GameManager": return FindSceneObjects<GameManager>().Length;
                case "EndingManager": return FindSceneObjects<EndingManager>().Length;
                case "ClueImageManager": return FindSceneObjects<ClueImageManager>().Length;
                case "NoiseManager": return FindSceneObjects<NoiseManager>().Length;
                case "GhostManager": return FindSceneObjects<GhostManager>().Length;
                case "HideManager": return FindSceneObjects<HideManager>().Length;
                case "ChaseManager": return FindSceneObjects<ChaseManager>().Length;
                case "ScreenFadeManager": return FindSceneObjects<ScreenFadeManager>().Length;
                default: return 0;
            }
        }

        private static T[] FindSceneObjects<T>() where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            AddSceneObjects(UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None), result);
            AddSceneObjects(Resources.FindObjectsOfTypeAll<T>(), result);
            return result.ToArray();
        }

        private static void AddSceneObjects<T>(T[] objects, List<T> result) where T : UnityEngine.Object
        {
            if (objects == null)
            {
                return;
            }
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]) && !result.Contains(objects[i]))
                {
                    result.Add(objects[i]);
                }
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static bool IsSceneObject(UnityEngine.Object obj)
        {
            if (obj == null || EditorUtility.IsPersistent(obj))
            {
                return false;
            }
            Component component = obj as Component;
            if (component != null)
            {
                return component.gameObject.scene.IsValid();
            }
            GameObject gameObject = obj as GameObject;
            return gameObject != null && gameObject.scene.IsValid();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static UnityEngine.Object GetObjectField(UnityEngine.Object target, string fieldName)
        {
            if (target == null)
            {
                return null;
            }
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            return property != null ? property.objectReferenceValue : null;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Performs the Escape Markdown operation while keeping its implementation details inside this script.
        private static string EscapeMarkdown(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }

        // Records a blocking validation problem for the final report and console output.
        private static void AddError(string message)
        {
            errors.Add(message);
            Debug.LogError("[SourceRouteSceneWiringValidator] " + message);
        }

        // Records a non-blocking validation concern for follow-up review.
        private static void AddWarning(string message)
        {
            warnings.Add(message);
            Debug.LogWarning("[SourceRouteSceneWiringValidator] " + message);
        }
    }
}
