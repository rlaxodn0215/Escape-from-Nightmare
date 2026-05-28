using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public static class SceneWiringValidator
    {
        private static int errorCount;
        private static int warningCount;

        private static readonly HashSet<string> locationIds = new HashSet<string>();
        private static readonly Dictionary<string, LocationRecord> locationMap = new Dictionary<string, LocationRecord>();
        private static readonly HashSet<string> viewIds = new HashSet<string>();
        private static readonly Dictionary<string, DoorRecord> doorMap = new Dictionary<string, DoorRecord>();
        private static readonly Dictionary<string, ItemRecord> itemMap = new Dictionary<string, ItemRecord>();
        private static readonly Dictionary<string, PuzzleRecord> puzzleMap = new Dictionary<string, PuzzleRecord>();
        private static readonly Dictionary<string, ClueRecord> clueMap = new Dictionary<string, ClueRecord>();
        private static readonly Dictionary<string, SymbolRecord> symbolMap = new Dictionary<string, SymbolRecord>();
        private static readonly Dictionary<string, PuzzleAnswerRecord> answerByVariableName = new Dictionary<string, PuzzleAnswerRecord>();
        private static readonly Dictionary<string, PuzzleAnswerRecord> answerByPuzzleId = new Dictionary<string, PuzzleAnswerRecord>();

        private static string titleSceneName = "TitleScene";
        private static string gameSceneName = "GameScene";

        [MenuItem("Escape From Nightmare/Validate Current Scene Wiring")]
        public static void ValidateCurrentSceneWiring()
        {
            errorCount = 0;
            warningCount = 0;

            LoadGameData();

            Scene scene = SceneManager.GetActiveScene();
            AddInfo("Validating active scene: " + scene.name);

            if (scene.name == titleSceneName)
            {
                ValidateTitleScene();
            }
            else if (scene.name == gameSceneName)
            {
                ValidateGameScene();
            }
            else
            {
                AddWarning("Active Scene is not " + titleSceneName + " or " + gameSceneName + ". Running common wiring checks only: " + scene.name);
                ValidateCommonSceneObjects();
            }

            Debug.Log("Scene wiring validation completed. Errors: " + errorCount + ", Warnings: " + warningCount);
        }

        private static void LoadGameData()
        {
            locationIds.Clear();
            locationMap.Clear();
            viewIds.Clear();
            doorMap.Clear();
            itemMap.Clear();
            puzzleMap.Clear();
            clueMap.Clear();
            symbolMap.Clear();
            answerByVariableName.Clear();
            answerByPuzzleId.Clear();
            titleSceneName = "TitleScene";
            gameSceneName = "GameScene";

            string dataPath = Path.Combine(Application.streamingAssetsPath, "Data");

            LocationRecordList locations = LoadJson<LocationRecordList>(dataPath, "locations.json");
            DoorRecordList doors = LoadJson<DoorRecordList>(dataPath, "doors.json");
            ItemRecordList items = LoadJson<ItemRecordList>(dataPath, "items.json");
            PuzzleRecordList puzzles = LoadJson<PuzzleRecordList>(dataPath, "puzzles.json");
            ClueRecordList clues = LoadJson<ClueRecordList>(dataPath, "clues.json");
            SymbolRecordList symbols = LoadJson<SymbolRecordList>(dataPath, "symbols.json");
            PuzzleAnswerRecordList answers = LoadJson<PuzzleAnswerRecordList>(dataPath, "puzzle_answers.json");
            LoadJson<GhostRuleRecordList>(dataPath, "ghost_rules.json");
            GameSettingsWrapper settings = LoadJson<GameSettingsWrapper>(dataPath, "game_settings.json");

            if (settings != null && settings.settings != null)
            {
                if (!string.IsNullOrEmpty(settings.settings.titleSceneName))
                {
                    titleSceneName = settings.settings.titleSceneName;
                }

                if (!string.IsNullOrEmpty(settings.settings.gameSceneName))
                {
                    gameSceneName = settings.settings.gameSceneName;
                }
            }

            if (locations != null && locations.locations != null)
            {
                for (int i = 0; i < locations.locations.Count; i++)
                {
                    LocationRecord location = locations.locations[i];
                    if (location == null || string.IsNullOrEmpty(location.locationId))
                    {
                        continue;
                    }

                    locationIds.Add(location.locationId);
                    if (!locationMap.ContainsKey(location.locationId))
                    {
                        locationMap.Add(location.locationId, location);
                    }

                    if (location.viewIds != null)
                    {
                        for (int j = 0; j < location.viewIds.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(location.viewIds[j]))
                            {
                                viewIds.Add(location.viewIds[j]);
                            }
                        }
                    }
                }
            }

            AddToMap(doors != null ? doors.doors : null, doorMap, record => record.doorId);
            AddToMap(items != null ? items.items : null, itemMap, record => record.itemId);
            AddToMap(puzzles != null ? puzzles.puzzles : null, puzzleMap, record => record.puzzleId);
            AddToMap(clues != null ? clues.clues : null, clueMap, record => record.clueId);
            AddToMap(symbols != null ? symbols.symbols : null, symbolMap, record => record.symbolId);

            if (answers != null && answers.answers != null)
            {
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
                T result = JsonUtility.FromJson<T>(File.ReadAllText(path));
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

        private static void AddToMap<T>(IEnumerable<T> records, Dictionary<string, T> map, Func<T, string> getId) where T : class
        {
            if (records == null)
            {
                return;
            }

            foreach (T record in records)
            {
                if (record == null)
                {
                    continue;
                }

                string id = getId(record);
                if (!string.IsNullOrEmpty(id) && !map.ContainsKey(id))
                {
                    map.Add(id, record);
                }
            }
        }

        private static void ValidateTitleScene()
        {
            ValidateComponentCount<GameManager>("GameManager", true, true);
            ValidateComponentCount<SaveManager>("SaveManager", true, true);

            TitleMenuUI[] titleMenus = FindSceneObjects<TitleMenuUI>();
            if (titleMenus.Length == 0)
            {
                AddError("TitleMenuUI is missing from TitleScene.");
            }
            else if (titleMenus.Length > 1)
            {
                AddWarning("Multiple TitleMenuUI components found: " + titleMenus.Length);
            }

            for (int i = 0; i < titleMenus.Length; i++)
            {
                ValidateTitleMenuUI(titleMenus[i]);
            }

            ValidateBuildSettingsScene(titleSceneName, true);
            ValidateBuildSettingsScene(gameSceneName, true);
            ValidateCommonSceneObjects();
        }

        private static void ValidateTitleMenuUI(TitleMenuUI titleMenu)
        {
            if (titleMenu == null)
            {
                return;
            }

            string path = GetHierarchyPath(titleMenu.gameObject);
            CheckObjectField(titleMenu, "newGameButton", "TitleMenuUI.newGameButton", path, true);
            CheckObjectField(titleMenu, "continueButton", "TitleMenuUI.continueButton", path, false);
            CheckObjectField(titleMenu, "deleteSaveButton", "TitleMenuUI.deleteSaveButton", path, false);
            CheckObjectField(titleMenu, "quitButton", "TitleMenuUI.quitButton", path, false);
            CheckObjectField(titleMenu, "statusText", "TitleMenuUI.statusText", path, false);
        }

        private static void ValidateBuildSettingsScene(string sceneName, bool errorIfMissing)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                AddError("Scene name is empty in settings.");
                return;
            }

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                string buildSceneName = Path.GetFileNameWithoutExtension(scenes[i].path);
                if (buildSceneName == sceneName)
                {
                    return;
                }
            }

            if (errorIfMissing)
            {
                AddError("Scene is not registered in Build Settings: " + sceneName);
            }
            else
            {
                AddWarning("Scene is not registered in Build Settings: " + sceneName);
            }
        }

        private static void ValidateGameScene()
        {
            ValidateComponentCount<GameDataManager>("GameDataManager", true, true);
            ValidateComponentCount<LocationManager>("LocationManager", true, true);
            ValidateComponentCount<InteractionManager>("InteractionManager", true, true);
            ValidateComponentCount<InventoryManager>("InventoryManager", true, true);
            ValidateComponentCount<PuzzleManager>("PuzzleManager", true, true);
            ValidateComponentCount<SaveManager>("SaveManager", false, true);
            ValidateComponentCount<GameManager>("GameManager", false, true);
            ValidateComponentCount<EndingManager>("EndingManager", false, true);
            ValidateComponentCount<NoiseManager>("NoiseManager", false, true);
            ValidateComponentCount<GhostManager>("GhostManager", false, true);
            ValidateComponentCount<HideManager>("HideManager", false, true);
            ValidateComponentCount<ChaseManager>("ChaseManager", false, true);
            ValidateComponentCount<ClueImageManager>("ClueImageManager", false, true);

            ValidateLocationManager();
            ValidatePuzzleManager();
            ValidateClueImageManager();
            ValidateEndingManager();
            ValidateCommonSceneObjects();
            ValidateInventoryUI();
            ValidateClueImagePanel();
            ValidateGameOverAndEndingPanels();
            ValidateGhostHideChaseUI();
        }

        private static void ValidateCommonSceneObjects()
        {
            ValidateLocationControllersAndViews();
            ValidateNavigationButtons();
            ValidateClickableButtons();
        }

        private static void ValidateLocationManager()
        {
            LocationManager[] managers = FindSceneObjects<LocationManager>();
            for (int i = 0; i < managers.Length; i++)
            {
                LocationManager manager = managers[i];
                string path = GetHierarchyPath(manager.gameObject);
                UnityEngine.Object root = GetObjectField(manager, "locationRoot");
                bool collectFromRoot = GetBoolField(manager, "collectLocationsFromRoot", true);
                SerializedProperty controllers = GetProperty(manager, "locationControllers");
                int controllerCount = controllers != null && controllers.isArray ? controllers.arraySize : 0;

                if (collectFromRoot && root == null)
                {
                    AddError("LocationManager.locationRoot is required when collectLocationsFromRoot is true: " + path);
                }

                if (root == null && controllerCount == 0)
                {
                    AddError("LocationManager has no locationRoot and no locationControllers: " + path);
                }

                string startingLocationId = GetStringField(manager, "startingLocationId");
                string startingViewId = GetStringField(manager, "startingViewId");
                if (!string.IsNullOrEmpty(startingLocationId) && !locationIds.Contains(startingLocationId))
                {
                    AddError("LocationManager.startingLocationId does not exist in locations.json: " + startingLocationId);
                }

                if (!string.IsNullOrEmpty(startingLocationId) && !string.IsNullOrEmpty(startingViewId))
                {
                    CheckLocationViewReference(startingLocationId, startingViewId, "LocationManager.startingViewId", path, false);
                }
            }
        }

        private static void ValidatePuzzleManager()
        {
            PuzzleManager[] managers = FindSceneObjects<PuzzleManager>();
            for (int i = 0; i < managers.Length; i++)
            {
                if (GetObjectField(managers[i], "puzzleUiRoot") == null)
                {
                    AddError("PuzzleManager.puzzleUiRoot is not assigned: " + GetHierarchyPath(managers[i].gameObject));
                }
            }
        }

        private static void ValidateClueImageManager()
        {
            ClueImageManager[] managers = FindSceneObjects<ClueImageManager>();
            ClueImagePanelUI[] panels = FindSceneObjects<ClueImagePanelUI>();
            for (int i = 0; i < managers.Length; i++)
            {
                UnityEngine.Object panel = GetObjectField(managers[i], "clueImagePanel");
                if (panel == null)
                {
                    AddWarning("ClueImageManager.clueImagePanel is not assigned: " + GetHierarchyPath(managers[i].gameObject));
                }
                else if (panels.Length > 0 && !ContainsReference(panels, panel))
                {
                    AddWarning("ClueImageManager.clueImagePanel does not reference a ClueImagePanelUI in the active scene: " + GetHierarchyPath(managers[i].gameObject));
                }
            }
        }

        private static void ValidateEndingManager()
        {
            EndingManager[] managers = FindSceneObjects<EndingManager>();
            EndingPanelUI[] panels = FindSceneObjects<EndingPanelUI>();
            for (int i = 0; i < managers.Length; i++)
            {
                UnityEngine.Object panel = GetObjectField(managers[i], "endingPanel");
                if (panel == null)
                {
                    AddWarning("EndingManager.endingPanel is not assigned: " + GetHierarchyPath(managers[i].gameObject));
                }
                else if (panels.Length > 0 && !ContainsReference(panels, panel))
                {
                    AddWarning("EndingManager.endingPanel does not reference an EndingPanelUI in the active scene: " + GetHierarchyPath(managers[i].gameObject));
                }
            }
        }

        private static void ValidateLocationControllersAndViews()
        {
            LocationController[] controllers = FindSceneObjects<LocationController>();
            Dictionary<string, LocationController> sceneLocations = new Dictionary<string, LocationController>();
            HashSet<string> requiredLocations = new HashSet<string> { "Bedroom", "ChildRoom", "Study", "SecondFloorHallway", "LivingRoom", "Entrance", "Kitchen", "BasementStorage", "LockedRoom" };

            for (int i = 0; i < controllers.Length; i++)
            {
                LocationController controller = controllers[i];
                string path = GetHierarchyPath(controller.gameObject);
                string locationId = controller.LocationId;

                if (string.IsNullOrEmpty(locationId))
                {
                    AddError("LocationController.locationId is empty: " + path);
                    continue;
                }

                if (!locationIds.Contains(locationId))
                {
                    AddError("LocationController.locationId does not exist in locations.json: " + locationId + " at " + path);
                }

                if (sceneLocations.ContainsKey(locationId))
                {
                    AddError("Duplicate LocationController.locationId in scene: " + locationId + " at " + path);
                }
                else
                {
                    sceneLocations.Add(locationId, controller);
                }

                if (string.IsNullOrEmpty(controller.DefaultViewId))
                {
                    AddWarning("LocationController.defaultViewId is empty: " + locationId + " at " + path);
                }
                else
                {
                    CheckLocationViewReference(locationId, controller.DefaultViewId, "LocationController.defaultViewId", path, false);
                }

                LocationView[] childViews = controller.GetComponentsInChildren<LocationView>(true);
                if (childViews.Length == 0)
                {
                    AddError("LocationController has no child LocationView: " + locationId + " at " + path);
                }

                if ((controller.Views == null || controller.Views.Count == 0) && childViews.Length > 0)
                {
                    AddWarning("LocationController.Views list is empty, but child LocationView objects exist: " + locationId + " at " + path);
                }

                HashSet<string> localViewIds = new HashSet<string>();
                for (int j = 0; j < childViews.Length; j++)
                {
                    ValidateLocationView(childViews[j], controller, locationId, localViewIds);
                }
            }

            foreach (string locationId in locationIds)
            {
                if (sceneLocations.ContainsKey(locationId))
                {
                    continue;
                }

                if (requiredLocations.Contains(locationId))
                {
                    AddError("Required main-route Location is not placed in the scene: " + locationId);
                }
                else
                {
                    AddWarning("Location from locations.json is not placed in the scene: " + locationId);
                }
            }
        }

        private static void ValidateLocationView(LocationView view, LocationController parent, string parentLocationId, HashSet<string> localViewIds)
        {
            if (view == null)
            {
                return;
            }

            string path = GetHierarchyPath(view.gameObject);
            string viewId = view.ViewId;
            if (string.IsNullOrEmpty(viewId))
            {
                AddError("LocationView.viewId is empty: " + path);
                return;
            }

            if (!viewIds.Contains(viewId))
            {
                AddWarning("LocationView.viewId is not found in any locations.json viewIds: " + viewId + " at " + path);
            }

            if (!localViewIds.Add(viewId))
            {
                AddError("Duplicate LocationView.viewId under LocationController " + parentLocationId + ": " + viewId);
            }

            CheckLocationViewReference(parentLocationId, viewId, "LocationView under " + parentLocationId, path, false);

            if (GetObjectField(view, "rootObject") == null)
            {
                AddWarning("LocationView.rootObject is not assigned: " + viewId + " at " + path);
            }
        }

        private static void ValidateNavigationButtons()
        {
            NavigationButton[] buttons = FindSceneObjects<NavigationButton>();
            int rotateLeftCount = 0;
            int rotateRightCount = 0;

            for (int i = 0; i < buttons.Length; i++)
            {
                NavigationButton button = buttons[i];
                string path = GetHierarchyPath(button.gameObject);
                if (button.GetComponent<Button>() == null)
                {
                    AddError("NavigationButton is missing UnityEngine.UI.Button: " + path);
                }

                string actionType = GetEnumNameField(button, "actionType");
                string targetLocationId = GetStringField(button, "targetLocationId");
                string targetViewId = GetStringField(button, "targetViewId");

                if (actionType == "RotateLeft")
                {
                    rotateLeftCount++;
                }
                else if (actionType == "RotateRight")
                {
                    rotateRightCount++;
                }
                else if (actionType == "SetLocation")
                {
                    if (string.IsNullOrEmpty(targetLocationId))
                    {
                        AddError("NavigationButton SetLocation targetLocationId is empty: " + path);
                    }
                    else if (!locationIds.Contains(targetLocationId))
                    {
                        AddError("NavigationButton targetLocationId does not exist: " + targetLocationId + " at " + path);
                    }

                    if (!string.IsNullOrEmpty(targetLocationId) && !string.IsNullOrEmpty(targetViewId))
                    {
                        CheckLocationViewReference(targetLocationId, targetViewId, "NavigationButton targetViewId", path, false);
                    }
                }
                else if (actionType == "SetView")
                {
                    if (string.IsNullOrEmpty(targetViewId))
                    {
                        AddError("NavigationButton SetView targetViewId is empty: " + path);
                    }
                    else if (!viewIds.Contains(targetViewId))
                    {
                        AddWarning("NavigationButton targetViewId is not found in any locations.json viewIds: " + targetViewId + " at " + path);
                    }
                }
            }

            if (rotateLeftCount == 0)
            {
                AddWarning("No NavigationButton with actionType RotateLeft found in the active scene.");
            }

            if (rotateRightCount == 0)
            {
                AddWarning("No NavigationButton with actionType RotateRight found in the active scene.");
            }
        }

        private static void ValidateClickableButtons()
        {
            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                ValidateClickableButton(buttons[i]);
            }
        }

        private static void ValidateClickableButton(ClickableButton button)
        {
            if (button == null)
            {
                return;
            }

            string path = GetHierarchyPath(button.gameObject);
            if (button.GetComponent<Button>() == null)
            {
                AddError("ClickableButton is missing UnityEngine.UI.Button: " + path);
            }

            if (string.IsNullOrEmpty(button.ClickableId))
            {
                AddWarning("ClickableButton.clickableId is empty: " + path);
            }

            LocationController parentLocation = FindParent<LocationController>(button.transform);
            LocationView parentView = FindParent<LocationView>(button.transform);
            string parentLocationId = parentLocation != null ? parentLocation.LocationId : string.Empty;
            string parentViewId = parentView != null ? parentView.ViewId : string.Empty;

            switch (button.ClickableType)
            {
                case ClickableType.ExamineImage:
                    ValidateExamineImageButton(button, path);
                    break;
                case ClickableType.Puzzle:
                    ValidatePuzzleButton(button, path, parentLocationId);
                    break;
                case ClickableType.Door:
                    ValidateDoorButton(button, path, parentLocationId, parentViewId);
                    break;
                case ClickableType.HidePoint:
                    ValidateHidePointButton(button, path);
                    break;
                case ClickableType.PickupItem:
                    ValidatePickupItemButton(button, path);
                    break;
                case ClickableType.UseItemTarget:
                    ValidateUseItemTargetButton(button, path);
                    break;
                case ClickableType.FinalDoor:
                    ValidateFinalDoorButton(button, path);
                    break;
                default:
                    AddWarning("Unhandled ClickableType: " + button.ClickableType + " at " + path);
                    break;
            }
        }

        private static void ValidateExamineImageButton(ClickableButton button, string path)
        {
            string clueId = FirstNotEmpty(button.LinkedClueImageId, button.TargetObjectId, button.ClickableId);
            if (string.IsNullOrEmpty(clueId))
            {
                AddError("ExamineImage ClickableButton needs linkedClueImageId, targetObjectId, or clickableId: " + path);
            }
            else if (!clueMap.ContainsKey(clueId))
            {
                AddError("ExamineImage clueId does not exist in clues.json: " + clueId + " at " + path);
            }
        }

        private static void ValidatePuzzleButton(ClickableButton button, string path, string parentLocationId)
        {
            if (string.IsNullOrEmpty(button.LinkedPuzzleId))
            {
                AddError("Puzzle ClickableButton.linkedPuzzleId is empty: " + path);
                return;
            }

            PuzzleRecord puzzle;
            if (!puzzleMap.TryGetValue(button.LinkedPuzzleId, out puzzle))
            {
                AddError("Puzzle linkedPuzzleId does not exist in puzzles.json: " + button.LinkedPuzzleId + " at " + path);
                return;
            }

            if (string.IsNullOrEmpty(puzzle.prefabPath))
            {
                AddWarning("PuzzleRecord.prefabPath is empty for button puzzle: " + button.LinkedPuzzleId + " at " + path);
            }
            else if (Resources.Load<GameObject>(puzzle.prefabPath) == null)
            {
                AddWarning("Puzzle prefab not found at Resources path: " + puzzle.prefabPath + " for " + button.LinkedPuzzleId);
            }

            if (!string.IsNullOrEmpty(parentLocationId) && !string.IsNullOrEmpty(puzzle.locationId) && parentLocationId != puzzle.locationId)
            {
                AddWarning("Puzzle button parent Location does not match PuzzleRecord.locationId: button at " + path + ", parent " + parentLocationId + ", puzzle " + puzzle.locationId);
            }
        }

        private static void ValidateDoorButton(ClickableButton button, string path, string parentLocationId, string parentViewId)
        {
            if (string.IsNullOrEmpty(button.LinkedDoorId))
            {
                if (!string.IsNullOrEmpty(button.LinkedLocationId))
                {
                    AddWarning("Door ClickableButton uses linkedLocationId fallback instead of linkedDoorId: " + path);
                    return;
                }

                AddError("Door ClickableButton needs linkedDoorId or linkedLocationId: " + path);
                return;
            }

            DoorRecord door;
            if (!doorMap.TryGetValue(button.LinkedDoorId, out door))
            {
                AddError("Door linkedDoorId does not exist in doors.json: " + button.LinkedDoorId + " at " + path);
                return;
            }

            if (!string.IsNullOrEmpty(parentLocationId) && !string.IsNullOrEmpty(door.fromLocationId) && parentLocationId != door.fromLocationId)
            {
                AddWarning("Door button parent Location differs from DoorRecord.fromLocationId: " + path + ", parent " + parentLocationId + ", door " + door.fromLocationId);
            }

            if (!string.IsNullOrEmpty(parentViewId) && !string.IsNullOrEmpty(door.fromViewId) && parentViewId != door.fromViewId)
            {
                AddWarning("Door button parent View differs from DoorRecord.fromViewId: " + path + ", parent " + parentViewId + ", door " + door.fromViewId);
            }

            CheckOptionalReference(door.requiredItemId, itemMap, "Door.requiredItemId", button.LinkedDoorId, true);
            CheckOptionalReference(door.requiredPuzzleId, puzzleMap, "Door.requiredPuzzleId", button.LinkedDoorId, true);
        }

        private static void ValidateHidePointButton(ClickableButton button, string path)
        {
            HidePointController hidePoint = button.GetComponent<HidePointController>();
            if (hidePoint == null)
            {
                AddWarning("HidePoint ClickableButton has no HidePointController: " + path);
            }
            else if (!hidePoint.Usable)
            {
                AddInfo("HidePointController is currently marked unusable: " + path);
            }

            string resolvedId = hidePoint != null ? hidePoint.GetResolvedHidePointId() : FirstNotEmpty(button.TargetObjectId, button.ClickableId, button.gameObject.name);
            if (string.IsNullOrEmpty(resolvedId))
            {
                AddWarning("HidePoint has no hidePointId, targetObjectId, clickableId, or GameObject name: " + path);
            }
        }

        private static void ValidatePickupItemButton(ClickableButton button, string path)
        {
            if (string.IsNullOrEmpty(button.LinkedItemId))
            {
                AddError("PickupItem ClickableButton.linkedItemId is empty: " + path);
                return;
            }

            if (!itemMap.ContainsKey(button.LinkedItemId))
            {
                AddError("PickupItem linkedItemId does not exist in items.json: " + button.LinkedItemId + " at " + path);
            }

            if (button.GetComponent<PickupItemController>() == null)
            {
                AddWarning("PickupItem ClickableButton has no PickupItemController: " + path);
            }
        }

        private static void ValidateUseItemTargetButton(ClickableButton button, string path)
        {
            if (string.IsNullOrEmpty(button.RequiredItemId))
            {
                AddError("UseItemTarget ClickableButton.requiredItemId is empty: " + path);
            }
            else if (!itemMap.ContainsKey(button.RequiredItemId))
            {
                AddError("UseItemTarget requiredItemId does not exist in items.json: " + button.RequiredItemId + " at " + path);
            }

            CheckOptionalReference(button.LinkedPuzzleId, puzzleMap, "UseItemTarget.linkedPuzzleId", path, true);
            CheckOptionalReference(button.LinkedClueImageId, clueMap, "UseItemTarget.linkedClueImageId", path, true);
            CheckOptionalReference(button.LinkedDoorId, doorMap, "UseItemTarget.linkedDoorId", path, true);
            CheckOptionalReference(button.LinkedItemId, itemMap, "UseItemTarget.linkedItemId", path, true);

            if (string.IsNullOrEmpty(button.LinkedPuzzleId) && string.IsNullOrEmpty(button.LinkedClueImageId) && string.IsNullOrEmpty(button.LinkedDoorId) && string.IsNullOrEmpty(button.LinkedItemId))
            {
                AddWarning("UseItemTarget has no linked result field, so item use may do nothing: " + path);
            }
        }

        private static void ValidateFinalDoorButton(ClickableButton button, string path)
        {
            CheckOptionalReference(button.RequiredItemId, itemMap, "FinalDoor.requiredItemId", path, true);
            CheckOptionalReference(button.LinkedPuzzleId, puzzleMap, "FinalDoor.linkedPuzzleId", path, true);
            CheckOptionalReference(button.LinkedDoorId, doorMap, "FinalDoor.linkedDoorId", path, true);

            if (string.IsNullOrEmpty(button.RequiredItemId) && string.IsNullOrEmpty(button.LinkedPuzzleId) && string.IsNullOrEmpty(button.LinkedDoorId))
            {
                AddWarning("FinalDoor has no requiredItemId, linkedPuzzleId, or linkedDoorId; it may enter Ending immediately: " + path);
            }
        }

        private static void ValidateInventoryUI()
        {
            InventoryBarUI[] bars = FindSceneObjects<InventoryBarUI>();
            InventorySlotUI[] slots = FindSceneObjects<InventorySlotUI>();

            if (bars.Length == 0)
            {
                AddWarning("InventoryBarUI is missing from the active scene.");
            }

            for (int i = 0; i < bars.Length; i++)
            {
                string path = GetHierarchyPath(bars[i].gameObject);
                if (GetObjectField(bars[i], "slotRoot") == null)
                {
                    AddWarning("InventoryBarUI.slotRoot is not assigned: " + path);
                }

                SerializedProperty slotList = GetProperty(bars[i], "slots");
                if (slotList == null || !slotList.isArray || slotList.arraySize == 0)
                {
                    AddWarning("InventoryBarUI.slots list is empty: " + path);
                }

                InventorySlotUI[] childSlots = bars[i].GetComponentsInChildren<InventorySlotUI>(true);
                if (childSlots.Length == 0)
                {
                    AddError("InventoryBarUI has no child InventorySlotUI: " + path);
                }
                else if (childSlots.Length < 5)
                {
                    AddWarning("InventoryBarUI has fewer than 5 slots: " + childSlots.Length + " at " + path);
                }
            }

            for (int i = 0; i < slots.Length; i++)
            {
                ValidateInventorySlot(slots[i]);
            }
        }

        private static void ValidateInventorySlot(InventorySlotUI slot)
        {
            string path = GetHierarchyPath(slot.gameObject);
            if (slot.GetComponent<Button>() == null)
            {
                AddError("InventorySlotUI is missing UnityEngine.UI.Button: " + path);
            }

            CheckObjectField(slot, "iconImage", "InventorySlotUI.iconImage", path, false);
            CheckObjectField(slot, "labelText", "InventorySlotUI.labelText", path, false);
            CheckObjectField(slot, "selectedIndicator", "InventorySlotUI.selectedIndicator", path, false);
            CheckObjectField(slot, "emptyRoot", "InventorySlotUI.emptyRoot", path, false);
            CheckObjectField(slot, "filledRoot", "InventorySlotUI.filledRoot", path, false);
        }

        private static void ValidateClueImagePanel()
        {
            ClueImagePanelUI[] panels = FindSceneObjects<ClueImagePanelUI>();
            if (panels.Length == 0)
            {
                AddWarning("ClueImagePanelUI is missing from the active scene.");
            }

            for (int i = 0; i < panels.Length; i++)
            {
                string path = GetHierarchyPath(panels[i].gameObject);
                CheckObjectField(panels[i], "rootObject", "ClueImagePanelUI.rootObject", path, false);
                CheckObjectField(panels[i], "clueImage", "ClueImagePanelUI.clueImage", path, false);
                CheckObjectField(panels[i], "titleText", "ClueImagePanelUI.titleText", path, false);
                CheckObjectField(panels[i], "descriptionText", "ClueImagePanelUI.descriptionText", path, false);
                CheckObjectField(panels[i], "messageText", "ClueImagePanelUI.messageText", path, false);
                CheckObjectField(panels[i], "closeButton", "ClueImagePanelUI.closeButton", path, false);
            }
        }

        private static void ValidateGameOverAndEndingPanels()
        {
            GameOverPanelUI[] gameOverPanels = FindSceneObjects<GameOverPanelUI>();
            if (gameOverPanels.Length == 0)
            {
                AddWarning("GameOverPanelUI is missing from the active scene.");
            }

            for (int i = 0; i < gameOverPanels.Length; i++)
            {
                string path = GetHierarchyPath(gameOverPanels[i].gameObject);
                CheckObjectField(gameOverPanels[i], "rootObject", "GameOverPanelUI.rootObject", path, false);
                CheckObjectField(gameOverPanels[i], "messageText", "GameOverPanelUI.messageText", path, false);
                CheckObjectField(gameOverPanels[i], "restartButton", "GameOverPanelUI.restartButton", path, true);
                CheckObjectField(gameOverPanels[i], "returnTitleButton", "GameOverPanelUI.returnTitleButton", path, true);
            }

            EndingPanelUI[] endingPanels = FindSceneObjects<EndingPanelUI>();
            if (endingPanels.Length == 0)
            {
                AddWarning("EndingPanelUI is missing from the active scene.");
            }

            for (int i = 0; i < endingPanels.Length; i++)
            {
                string path = GetHierarchyPath(endingPanels[i].gameObject);
                CheckObjectField(endingPanels[i], "rootObject", "EndingPanelUI.rootObject", path, false);
                CheckObjectField(endingPanels[i], "titleText", "EndingPanelUI.titleText", path, false);
                CheckObjectField(endingPanels[i], "messageText", "EndingPanelUI.messageText", path, false);
                CheckObjectField(endingPanels[i], "skipButton", "EndingPanelUI.skipButton", path, false);
            }
        }

        private static void ValidateGhostHideChaseUI()
        {
            GhostStatusUI[] statusUis = FindSceneObjects<GhostStatusUI>();
            if (statusUis.Length == 0)
            {
                AddWarning("GhostStatusUI is missing from the active scene.");
            }

            for (int i = 0; i < statusUis.Length; i++)
            {
                string path = GetHierarchyPath(statusUis[i].gameObject);
                CheckObjectField(statusUis[i], "stateText", "GhostStatusUI.stateText", path, false);
                CheckObjectField(statusUis[i], "dangerText", "GhostStatusUI.dangerText", path, false);
                CheckObjectField(statusUis[i], "chaseText", "GhostStatusUI.chaseText", path, false);
                CheckObjectField(statusUis[i], "hideText", "GhostStatusUI.hideText", path, false);
            }

            HideExitButton[] exitButtons = FindSceneObjects<HideExitButton>();
            if (exitButtons.Length == 0)
            {
                AddWarning("HideExitButton is missing from the active scene.");
            }

            for (int i = 0; i < exitButtons.Length; i++)
            {
                string path = GetHierarchyPath(exitButtons[i].gameObject);
                if (exitButtons[i].GetComponent<Button>() == null)
                {
                    AddError("HideExitButton is missing UnityEngine.UI.Button: " + path);
                }

                CheckObjectField(exitButtons[i], "rootObject", "HideExitButton.rootObject", path, false);
            }

            HidePointController[] hidePoints = FindSceneObjects<HidePointController>();
            if (hidePoints.Length == 0)
            {
                AddWarning("No HidePointController found in the active scene.");
            }

            for (int i = 0; i < hidePoints.Length; i++)
            {
                string path = GetHierarchyPath(hidePoints[i].gameObject);
                ClickableButton clickable = hidePoints[i].GetComponent<ClickableButton>();
                if (clickable == null)
                {
                    AddError("HidePointController is missing ClickableButton: " + path);
                    continue;
                }

                if (clickable.ClickableType != ClickableType.HidePoint)
                {
                    AddWarning("HidePointController ClickableButton.clickableType is not HidePoint: " + path);
                }

                if (string.IsNullOrEmpty(hidePoints[i].HidePointId) && string.IsNullOrEmpty(clickable.TargetObjectId) && string.IsNullOrEmpty(clickable.ClickableId))
                {
                    AddWarning("HidePointController has no hidePointId, targetObjectId, or clickableId: " + path);
                }
            }
        }

        private static void ValidateComponentCount<T>(string label, bool missingIsError, bool duplicateIsError) where T : Component
        {
            T[] components = FindSceneObjects<T>();
            if (components.Length == 0)
            {
                if (missingIsError)
                {
                    AddError(label + " is missing from the active scene.");
                }
                else
                {
                    AddWarning(label + " is missing from the active scene.");
                }
            }
            else if (components.Length > 1)
            {
                string message = "Multiple " + label + " components found in the active scene: " + components.Length;
                if (duplicateIsError && missingIsError)
                {
                    AddError(message);
                }
                else
                {
                    AddWarning(message);
                }
            }
        }

        private static void CheckLocationViewReference(string locationId, string viewId, string label, string owner, bool errorIfInvalid)
        {
            if (string.IsNullOrEmpty(viewId))
            {
                return;
            }

            LocationRecord location;
            if (!locationMap.TryGetValue(locationId, out location) || location == null || location.viewIds == null)
            {
                if (errorIfInvalid)
                {
                    AddError(label + " cannot be checked because location is missing: " + owner + " / " + locationId);
                }

                return;
            }

            for (int i = 0; i < location.viewIds.Length; i++)
            {
                if (location.viewIds[i] == viewId)
                {
                    return;
                }
            }

            if (errorIfInvalid)
            {
                AddError(label + " is not in the LocationRecord.viewIds: " + owner + " / " + locationId + " / " + viewId);
            }
            else
            {
                AddWarning(label + " is not in the LocationRecord.viewIds: " + owner + " / " + locationId + " / " + viewId);
            }
        }

        private static void CheckOptionalReference<T>(string id, Dictionary<string, T> map, string label, string owner, bool errorIfInvalid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (!map.ContainsKey(id))
            {
                if (errorIfInvalid)
                {
                    AddError(label + " does not exist: " + owner + " / " + id);
                }
                else
                {
                    AddWarning(label + " does not exist: " + owner + " / " + id);
                }
            }
        }

        private static void CheckObjectField(UnityEngine.Object target, string fieldName, string label, string path, bool errorIfNull)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            if (property == null)
            {
                AddWarning(label + " field was not found on " + target.GetType().Name + ": " + path);
                return;
            }

            if (property.objectReferenceValue != null)
            {
                return;
            }

            if (errorIfNull)
            {
                AddError(label + " is not assigned: " + path);
            }
            else
            {
                AddWarning(label + " is not assigned: " + path);
            }
        }

        private static UnityEngine.Object GetObjectField(UnityEngine.Object target, string fieldName)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            return property != null ? property.objectReferenceValue : null;
        }

        private static string GetStringField(UnityEngine.Object target, string fieldName)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            return property != null ? property.stringValue : string.Empty;
        }

        private static bool GetBoolField(UnityEngine.Object target, string fieldName, bool defaultValue)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            return property != null ? property.boolValue : defaultValue;
        }

        private static string GetEnumNameField(UnityEngine.Object target, string fieldName)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                return string.Empty;
            }

            if (property.enumValueIndex < 0 || property.enumValueIndex >= property.enumDisplayNames.Length)
            {
                return string.Empty;
            }

            return property.enumDisplayNames[property.enumValueIndex];
        }

        private static SerializedProperty GetProperty(UnityEngine.Object target, string fieldName)
        {
            if (target == null || string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            return serializedObject.FindProperty(fieldName);
        }

        private static T[] FindSceneObjects<T>() where T : UnityEngine.Object
        {
            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            List<T> sceneObjects = new List<T>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]))
                {
                    sceneObjects.Add(objects[i]);
                }
            }

            return sceneObjects.ToArray();
        }

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

        private static T FindParent<T>(Transform transform) where T : Component
        {
            Transform current = transform;
            while (current != null)
            {
                T component = current.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }

                current = current.parent;
            }

            return null;
        }

        private static bool ContainsReference<T>(T[] objects, UnityEngine.Object reference) where T : UnityEngine.Object
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == reference)
                {
                    return true;
                }
            }

            return false;
        }

        private static string FirstNotEmpty(params string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!string.IsNullOrEmpty(values[i]))
                {
                    return values[i];
                }
            }

            return string.Empty;
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
            Debug.LogError("[SceneWiringValidator] " + message);
        }

        private static void AddWarning(string message)
        {
            warningCount++;
            Debug.LogWarning("[SceneWiringValidator] " + message);
        }

        private static void AddInfo(string message)
        {
            Debug.Log("[SceneWiringValidator] " + message);
        }
    }
}
