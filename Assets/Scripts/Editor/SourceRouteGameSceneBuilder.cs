using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public static class SourceRouteGameSceneBuilder
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string ReportPath = "Assets/Docs/GeneratedSourceRouteGameSceneBuilderReport.md";

        private static readonly Dictionary<string, SourceRouteGameSceneBuilderCategoryStats> stats = new Dictionary<string, SourceRouteGameSceneBuilderCategoryStats>();
        private static readonly List<string> createdObjects = new List<string>();
        private static readonly List<string> reusedObjects = new List<string>();
        private static readonly List<string> linkedFields = new List<string>();
        private static readonly List<string> warnings = new List<string>();
        private static readonly List<string> errors = new List<string>();

        private static Canvas canvas;
        private static Transform locationRoot;
        private static Transform navigationRoot;
        private static Transform inventoryRoot;
        private static Transform puzzleUiRoot;
        private static Transform clueImagePanelRoot;
        private static Transform gameOverPanelRoot;
        private static Transform endingPanelRoot;
        private static Transform ghostStatusRoot;

        private static GameDataManager gameDataManager;
        private static LocationManager locationManager;
        private static InteractionManager interactionManager;
        private static InventoryManager inventoryManager;
        private static PuzzleManager puzzleManager;
        private static SaveManager saveManager;
        private static GameManager gameManager;
        private static EndingManager endingManager;
        private static ClueImageManager clueImageManager;
        private static NoiseManager noiseManager;
        private static GhostManager ghostManager;
        private static HideManager hideManager;
        private static ChaseManager chaseManager;

        private static readonly Dictionary<string, LocationController> locationsById = new Dictionary<string, LocationController>();
        private static readonly Dictionary<string, LocationView> viewsById = new Dictionary<string, LocationView>();
        private static readonly Dictionary<string, DoorRecord> doorsById = new Dictionary<string, DoorRecord>();
        private static readonly Dictionary<string, PuzzleRecord> puzzlesById = new Dictionary<string, PuzzleRecord>();
        private static readonly Dictionary<string, ClueRecord> cluesById = new Dictionary<string, ClueRecord>();

        private static bool lastRunSaved;
        private static string lastBackupPath = string.Empty;

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

        private static readonly string[,] LocationViews =
        {
            { "Bedroom", "Bedroom_Front,Bedroom_Right,Bedroom_Back,Bedroom_Left", "Bedroom_Front" },
            { "ChildRoom", "ChildRoom_Front,ChildRoom_Right,ChildRoom_Back,ChildRoom_Left", "ChildRoom_Front" },
            { "Study", "Study_Front,Study_Right,Study_Back,Study_Left", "Study_Front" },
            { "SecondFloorHallway", "SecondFloorHallway_Front,SecondFloorHallway_Back", "SecondFloorHallway_Front" },
            { "LivingRoom", "LivingRoom_Front,LivingRoom_Back", "LivingRoom_Front" },
            { "Entrance", "Entrance_Front", "Entrance_Front" },
            { "Kitchen", "Kitchen_Front", "Kitchen_Front" },
            { "BasementStorage", "BasementStorage_Front,BasementStorage_Right,BasementStorage_Back,BasementStorage_Left", "BasementStorage_Front" },
            { "LockedRoom", "LockedRoom_Front,LockedRoom_Right,LockedRoom_Back,LockedRoom_Left", "LockedRoom_Front" }
        };

        [MenuItem("Escape From Nightmare/Scene Setup/Build Missing Source Route GameScene Wiring")]
        public static void BuildMissingSourceRouteGameSceneWiring()
        {
            Build(false);
        }

        [MenuItem("Escape From Nightmare/Scene Setup/Build Missing Source Route GameScene Wiring And Save With Backup")]
        public static void BuildMissingSourceRouteGameSceneWiringAndSaveWithBackup()
        {
            Build(true);
        }

        [MenuItem("Escape From Nightmare/Scene Setup/Generate Source Route GameScene Builder Report")]
        public static void GenerateSourceRouteGameSceneBuilderReport()
        {
            ResetRunState();
            lastRunSaved = false;
            lastBackupPath = string.Empty;
            if (!EnsureGameSceneOpen())
            {
                WriteBuilderReport();
                return;
            }

            LoadData();
            CacheSceneObjects();
            WriteBuilderReport();
        }

        private static void Build(bool saveWithBackup)
        {
            ResetRunState();
            lastRunSaved = false;
            lastBackupPath = string.Empty;

            if (!EnsureGameSceneOpen())
            {
                WriteBuilderReport();
                return;
            }

            if (saveWithBackup && !BackupGameScene())
            {
                AddError("Build Settings", "GameScene backup failed. Scene save was skipped.");
                WriteBuilderReport();
                return;
            }

            LoadData();
            EnsureCanvasAndEventSystem();
            EnsureManagers();
            EnsureUiRoots();
            EnsureInventoryUi();
            EnsurePanels();
            EnsureNavigationButtons();
            EnsureLocationsAndViews();
            EnsureDoorButtons();
            EnsurePuzzleButtons();
            EnsureClueButtons();
            EnsureHidePoints();
            EnsureFinalDoor();
            EnsureBuildSettings();
            CacheSceneObjects();

            if (saveWithBackup && errors.Count == 0)
            {
                bool saved = EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                lastRunSaved = saved;
                if (!saved)
                {
                    AddError("Build Settings", "EditorSceneManager.SaveScene returned false.");
                }
            }

            WriteStaticDocs();
            WriteBuilderReport();
            AssetDatabase.Refresh();
            Debug.Log("Source route GameScene builder completed. Errors: " + errors.Count + ", Warnings: " + warnings.Count + ", Saved: " + lastRunSaved);
        }

        private static void ResetRunState()
        {
            stats.Clear();
            createdObjects.Clear();
            reusedObjects.Clear();
            linkedFields.Clear();
            warnings.Clear();
            errors.Clear();
            locationsById.Clear();
            viewsById.Clear();
            doorsById.Clear();
            puzzlesById.Clear();
            cluesById.Clear();
            canvas = null;
            locationRoot = null;
            navigationRoot = null;
            inventoryRoot = null;
            puzzleUiRoot = null;
            clueImagePanelRoot = null;
            gameOverPanelRoot = null;
            endingPanelRoot = null;
            ghostStatusRoot = null;
        }

        private static bool EnsureGameSceneOpen()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid() && activeScene.path == GameScenePath)
            {
                return true;
            }

            if (!File.Exists(GameScenePath))
            {
                AddError("Build Settings", "GameScene file not found: " + GameScenePath);
                return false;
            }

            EditorSceneManager.OpenScene(GameScenePath);
            AddInfo("Build Settings", "Opened GameScene: " + GameScenePath);
            return true;
        }

        private static bool BackupGameScene()
        {
            try
            {
                if (!File.Exists(GameScenePath))
                {
                    AddError("Build Settings", "Cannot back up missing scene: " + GameScenePath);
                    return false;
                }

                EnsureFolder("Assets/Backups/Scenes");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                lastBackupPath = "Assets/Backups/Scenes/GameScene.backup_before_source_route_wiring_" + timestamp + ".unity";
                File.Copy(GameScenePath, lastBackupPath, false);
                AddInfo("Build Settings", "Backed up GameScene to " + lastBackupPath);
                return true;
            }
            catch (Exception exception)
            {
                AddError("Build Settings", "Could not back up GameScene: " + exception.Message);
                return false;
            }
        }

        private static void LoadData()
        {
            string dataPath = Path.Combine(Application.streamingAssetsPath, "Data");
            DoorRecordList doors = LoadJson<DoorRecordList>(dataPath, "doors.json");
            PuzzleRecordList puzzles = LoadJson<PuzzleRecordList>(dataPath, "puzzles.json");
            ClueRecordList clues = LoadJson<ClueRecordList>(dataPath, "clues.json");

            if (doors != null && doors.doors != null)
            {
                for (int i = 0; i < doors.doors.Count; i++)
                {
                    DoorRecord record = doors.doors[i];
                    if (record != null && !string.IsNullOrEmpty(record.doorId) && !doorsById.ContainsKey(record.doorId))
                    {
                        doorsById.Add(record.doorId, record);
                    }
                }
            }

            if (puzzles != null && puzzles.puzzles != null)
            {
                for (int i = 0; i < puzzles.puzzles.Count; i++)
                {
                    PuzzleRecord record = puzzles.puzzles[i];
                    if (record != null && !string.IsNullOrEmpty(record.puzzleId) && !puzzlesById.ContainsKey(record.puzzleId))
                    {
                        puzzlesById.Add(record.puzzleId, record);
                    }
                }
            }

            if (clues != null && clues.clues != null)
            {
                for (int i = 0; i < clues.clues.Count; i++)
                {
                    ClueRecord record = clues.clues[i];
                    if (record != null && !string.IsNullOrEmpty(record.clueId) && !cluesById.ContainsKey(record.clueId))
                    {
                        cluesById.Add(record.clueId, record);
                    }
                }
            }
        }

        private static T LoadJson<T>(string dataPath, string fileName) where T : class
        {
            string path = Path.Combine(dataPath, fileName);
            if (!File.Exists(path))
            {
                AddError("Build Settings", "Missing data file: " + fileName);
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch (Exception exception)
            {
                AddError("Build Settings", "Could not parse " + fileName + ": " + exception.Message);
                return null;
            }
        }

        private static void CacheSceneObjects()
        {
            locationsById.Clear();
            viewsById.Clear();

            LocationController[] locationControllers = FindComponentsInScene<LocationController>();
            for (int i = 0; i < locationControllers.Length; i++)
            {
                if (locationControllers[i] != null && !string.IsNullOrEmpty(locationControllers[i].LocationId) && !locationsById.ContainsKey(locationControllers[i].LocationId))
                {
                    locationsById.Add(locationControllers[i].LocationId, locationControllers[i]);
                }
            }

            LocationView[] views = FindComponentsInScene<LocationView>();
            for (int i = 0; i < views.Length; i++)
            {
                if (views[i] != null && !string.IsNullOrEmpty(views[i].ViewId) && !viewsById.ContainsKey(views[i].ViewId))
                {
                    viewsById.Add(views[i].ViewId, views[i]);
                }
            }
        }

        private static void EnsureCanvasAndEventSystem()
        {
            canvas = FindComponentInScene<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = GetOrCreateGameObject("Canvas", null, "UI Roots");
                canvas = GetOrCreateComponent<Canvas>(canvasObject, "UI Roots");
                GetOrCreateComponent<CanvasScaler>(canvasObject, "UI Roots");
                GetOrCreateComponent<GraphicRaycaster>(canvasObject, "UI Roots");
                AddCreated("UI Roots", canvasObject);
            }
            else
            {
                AddReused("UI Roots", canvas.gameObject, "Existing Canvas");
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = GetOrCreateComponent<CanvasScaler>(canvas.gameObject, "UI Roots");
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            GetOrCreateComponent<GraphicRaycaster>(canvas.gameObject, "UI Roots");

            EventSystem eventSystem = FindComponentInScene<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObject = GetOrCreateGameObject("EventSystem", null, "UI Roots");
                GetOrCreateComponent<EventSystem>(eventSystemObject, "UI Roots");
                GetOrCreateComponent<StandaloneInputModule>(eventSystemObject, "UI Roots");
                AddCreated("UI Roots", eventSystemObject);
            }
            else
            {
                AddReused("UI Roots", eventSystem.gameObject, "Existing EventSystem");
                GetOrCreateComponent<StandaloneInputModule>(eventSystem.gameObject, "UI Roots");
            }
        }

        private static void EnsureManagers()
        {
            Transform managersRoot = GetOrCreateGameObject("Managers", null, "Managers").transform;
            gameManager = EnsureManager<GameManager>("GameManager", managersRoot);
            saveManager = EnsureManager<SaveManager>("SaveManager", managersRoot);
            gameDataManager = EnsureManager<GameDataManager>("GameDataManager", managersRoot);
            locationManager = EnsureManager<LocationManager>("LocationManager", managersRoot);
            interactionManager = EnsureManager<InteractionManager>("InteractionManager", managersRoot);
            inventoryManager = EnsureManager<InventoryManager>("InventoryManager", managersRoot);
            puzzleManager = EnsureManager<PuzzleManager>("PuzzleManager", managersRoot);
            endingManager = EnsureManager<EndingManager>("EndingManager", managersRoot);
            clueImageManager = EnsureManager<ClueImageManager>("ClueImageManager", managersRoot);
            noiseManager = EnsureManager<NoiseManager>("NoiseManager", managersRoot);
            ghostManager = EnsureManager<GhostManager>("GhostManager", managersRoot);
            hideManager = EnsureManager<HideManager>("HideManager", managersRoot);
            chaseManager = EnsureManager<ChaseManager>("ChaseManager", managersRoot);
        }

        private static T EnsureManager<T>(string objectName, Transform parent) where T : Component
        {
            T existing = FindComponentInScene<T>();
            if (existing != null)
            {
                AddReused("Managers", existing.gameObject, "Existing " + typeof(T).Name);
                return existing;
            }

            GameObject managerObject = GetOrCreateGameObject(objectName, parent, "Managers");
            T component = GetOrCreateComponent<T>(managerObject, "Managers");
            AddCreated("Managers", managerObject);
            return component;
        }

        private static void EnsureUiRoots()
        {
            Transform canvasTransform = canvas != null ? canvas.transform : null;
            locationRoot = CreatePanel("LocationRoot", canvasTransform, "UI Roots").transform;
            navigationRoot = CreatePanel("NavigationButtons", canvasTransform, "UI Roots").transform;
            inventoryRoot = CreatePanel("InventoryBar", canvasTransform, "UI Roots").transform;
            puzzleUiRoot = CreatePanel("PuzzleUIRoot", canvasTransform, "UI Roots").transform;
            clueImagePanelRoot = CreatePanel("ClueImagePanel", canvasTransform, "UI Roots").transform;
            gameOverPanelRoot = CreatePanel("GameOverPanel", canvasTransform, "UI Roots").transform;
            endingPanelRoot = CreatePanel("EndingPanel", canvasTransform, "UI Roots").transform;
            CreateButton("HideExitButton", canvasTransform, "Exit Hide", "UI Roots");
            ghostStatusRoot = CreatePanel("GhostStatusPanel", canvasTransform, "UI Roots").transform;

            SetSerializedObjectReference(locationManager, "locationRoot", locationRoot, "UI Roots");
            if (puzzleManager != null && puzzleManager.puzzleUiRoot != puzzleUiRoot)
            {
                puzzleManager.puzzleUiRoot = puzzleUiRoot;
                AddLinked("UI Roots", puzzleManager, "puzzleUiRoot", puzzleUiRoot);
                EditorUtility.SetDirty(puzzleManager);
            }
        }

        private static void EnsureInventoryUi()
        {
            InventoryBarUI bar = GetOrCreateComponent<InventoryBarUI>(inventoryRoot.gameObject, "Inventory UI");
            List<UnityEngine.Object> slots = new List<UnityEngine.Object>();
            for (int i = 1; i <= 6; i++)
            {
                Transform slot = CreateButton("Slot_" + i.ToString("00"), inventoryRoot, string.Empty, "Inventory UI").transform;
                InventorySlotUI slotUi = GetOrCreateComponent<InventorySlotUI>(slot.gameObject, "Inventory UI");
                GameObject emptyRoot = GetOrCreateGameObject("EmptyRoot", slot, "Inventory UI");
                GameObject filledRoot = GetOrCreateGameObject("FilledRoot", slot, "Inventory UI");
                Image iconImage = GetOrCreateImage("IconImage", filledRoot.transform, "Inventory UI");
                Text labelText = CreateText("LabelText", filledRoot.transform, string.Empty, 18, "Inventory UI");
                GameObject selectedIndicator = GetOrCreateGameObject("SelectedIndicator", slot, "Inventory UI");
                GetOrCreateComponent<Image>(selectedIndicator, "Inventory UI").color = new Color(1f, 0.9f, 0.2f, 0.35f);

                SetSerializedObjectReference(slotUi, "iconImage", iconImage, "Inventory UI");
                SetSerializedObjectReference(slotUi, "labelText", labelText, "Inventory UI");
                SetSerializedObjectReference(slotUi, "selectedIndicator", selectedIndicator, "Inventory UI");
                SetSerializedObjectReference(slotUi, "emptyRoot", emptyRoot, "Inventory UI");
                SetSerializedObjectReference(slotUi, "filledRoot", filledRoot, "Inventory UI");
                slots.Add(slotUi);
            }

            SetSerializedObjectReference(bar, "slotRoot", inventoryRoot, "Inventory UI");
            SetSerializedObjectList(bar, "slots", slots, "Inventory UI");
            SetSerializedBool(bar, "autoCollectSlots", true, "Inventory UI");
        }

        private static void EnsurePanels()
        {
            ClueImagePanelUI cluePanel = GetOrCreateComponent<ClueImagePanelUI>(clueImagePanelRoot.gameObject, "Panels");
            GameObject background = GetOrCreateGameObject("Background", clueImagePanelRoot, "Panels");
            GetOrCreateComponent<Image>(background, "Panels").color = new Color(0f, 0f, 0f, 0.65f);
            Image clueImage = GetOrCreateImage("ClueImage", clueImagePanelRoot, "Panels");
            Text clueTitle = CreateText("TitleText", clueImagePanelRoot, "Clue", 32, "Panels");
            Text clueDescription = CreateText("DescriptionText", clueImagePanelRoot, string.Empty, 22, "Panels");
            Text clueMessage = CreateText("MessageText", clueImagePanelRoot, string.Empty, 22, "Panels");
            Button clueClose = CreateButton("CloseButton", clueImagePanelRoot, "Close", "Panels").GetComponent<Button>();
            SetSerializedObjectReference(cluePanel, "rootObject", clueImagePanelRoot.gameObject, "Panels");
            SetSerializedObjectReference(cluePanel, "clueImage", clueImage, "Panels");
            SetSerializedObjectReference(cluePanel, "titleText", clueTitle, "Panels");
            SetSerializedObjectReference(cluePanel, "descriptionText", clueDescription, "Panels");
            SetSerializedObjectReference(cluePanel, "messageText", clueMessage, "Panels");
            SetSerializedObjectReference(cluePanel, "closeButton", clueClose, "Panels");
            SetSerializedBool(cluePanel, "hideOnAwake", true, "Panels");
            SetSerializedObjectReference(clueImageManager, "clueImagePanel", cluePanel, "Panels");

            GameOverPanelUI gameOverPanel = GetOrCreateComponent<GameOverPanelUI>(gameOverPanelRoot.gameObject, "Panels");
            Text gameOverMessage = CreateText("MessageText", gameOverPanelRoot, "You were caught.", 30, "Panels");
            Button restartButton = CreateButton("RestartButton", gameOverPanelRoot, "Restart", "Panels").GetComponent<Button>();
            Button returnTitleButton = CreateButton("ReturnTitleButton", gameOverPanelRoot, "Return Title", "Panels").GetComponent<Button>();
            SetSerializedObjectReference(gameOverPanel, "rootObject", gameOverPanelRoot.gameObject, "Panels");
            SetSerializedObjectReference(gameOverPanel, "messageText", gameOverMessage, "Panels");
            SetSerializedObjectReference(gameOverPanel, "restartButton", restartButton, "Panels");
            SetSerializedObjectReference(gameOverPanel, "returnTitleButton", returnTitleButton, "Panels");
            SetSerializedBool(gameOverPanel, "hideOnAwake", true, "Panels");

            EndingPanelUI endingPanelUi = GetOrCreateComponent<EndingPanelUI>(endingPanelRoot.gameObject, "Panels");
            Text endingTitle = CreateText("TitleText", endingPanelRoot, "Ending", 34, "Panels");
            Text endingMessage = CreateText("MessageText", endingPanelRoot, "You escaped from the nightmare.", 24, "Panels");
            Button skipButton = CreateButton("SkipButton", endingPanelRoot, "Skip", "Panels").GetComponent<Button>();
            SetSerializedObjectReference(endingPanelUi, "rootObject", endingPanelRoot.gameObject, "Panels");
            SetSerializedObjectReference(endingPanelUi, "titleText", endingTitle, "Panels");
            SetSerializedObjectReference(endingPanelUi, "messageText", endingMessage, "Panels");
            SetSerializedObjectReference(endingPanelUi, "skipButton", skipButton, "Panels");
            SetSerializedBool(endingPanelUi, "hideOnAwake", true, "Panels");
            SetSerializedObjectReference(endingManager, "endingPanel", endingPanelUi, "Panels");

            GameObject hideExit = FindObjectInSceneByName("HideExitButton");
            HideExitButton hideExitButton = GetOrCreateComponent<HideExitButton>(hideExit, "Panels");
            SetSerializedObjectReference(hideExitButton, "rootObject", hideExit, "Panels");
            SetSerializedBool(hideExitButton, "showOnlyWhileHiding", true, "Panels");

            GhostStatusUI ghostStatus = GetOrCreateComponent<GhostStatusUI>(ghostStatusRoot.gameObject, "Panels");
            Text stateText = CreateText("StateText", ghostStatusRoot, "Ghost: N/A", 18, "Panels");
            Text dangerText = CreateText("DangerText", ghostStatusRoot, "Danger: N/A", 18, "Panels");
            Text chaseText = CreateText("ChaseText", ghostStatusRoot, "Chase: N/A", 18, "Panels");
            Text hideText = CreateText("HideText", ghostStatusRoot, "Hide: N/A", 18, "Panels");
            SetSerializedObjectReference(ghostStatus, "rootObject", ghostStatusRoot.gameObject, "Panels");
            SetSerializedObjectReference(ghostStatus, "stateText", stateText, "Panels");
            SetSerializedObjectReference(ghostStatus, "dangerText", dangerText, "Panels");
            SetSerializedObjectReference(ghostStatus, "chaseText", chaseText, "Panels");
            SetSerializedObjectReference(ghostStatus, "hideText", hideText, "Panels");
        }

        private static void EnsureNavigationButtons()
        {
            GameObject left = CreateButton("Button_RotateLeft", navigationRoot, "<", "Navigation");
            NavigationButton leftNav = GetOrCreateComponent<NavigationButton>(left, "Navigation");
            SetSerializedInt(leftNav, "actionType", (int)NavigationActionType.RotateLeft, "Navigation");

            GameObject right = CreateButton("Button_RotateRight", navigationRoot, ">", "Navigation");
            NavigationButton rightNav = GetOrCreateComponent<NavigationButton>(right, "Navigation");
            SetSerializedInt(rightNav, "actionType", (int)NavigationActionType.RotateRight, "Navigation");
        }

        private static void EnsureLocationsAndViews()
        {
            CacheSceneObjects();
            List<UnityEngine.Object> locationControllers = new List<UnityEngine.Object>();

            for (int i = 0; i < LocationViews.GetLength(0); i++)
            {
                string locationId = LocationViews[i, 0];
                string[] views = LocationViews[i, 1].Split(',');
                string defaultViewId = LocationViews[i, 2];

                LocationController controller;
                if (!locationsById.TryGetValue(locationId, out controller) || controller == null)
                {
                    GameObject locationObject = GetOrCreateGameObject("Location_" + locationId, locationRoot, "Locations");
                    controller = GetOrCreateComponent<LocationController>(locationObject, "Locations");
                    SetSerializedString(controller, "locationId", locationId, "Locations");
                    AddCreated("Locations", locationObject);
                }
                else
                {
                    AddReused("Locations", controller.gameObject, "Location ID " + locationId);
                }

                SetSerializedString(controller, "locationId", locationId, "Locations");
                SetSerializedString(controller, "defaultViewId", defaultViewId, "Locations");

                List<UnityEngine.Object> viewList = new List<UnityEngine.Object>();
                for (int j = 0; j < views.Length; j++)
                {
                    string viewId = views[j];
                    LocationView view;
                    if (!viewsById.TryGetValue(viewId, out view) || view == null)
                    {
                        GameObject viewObject = CreatePanel("View_" + viewId, controller.transform, "Views");
                        view = GetOrCreateComponent<LocationView>(viewObject, "Views");
                        AddCreated("Views", viewObject);
                    }
                    else
                    {
                        AddReused("Views", view.gameObject, "View ID " + viewId);
                        if (view.transform.parent != controller.transform)
                        {
                            view.transform.SetParent(controller.transform, false);
                        }
                    }

                    SetSerializedString(view, "viewId", viewId, "Views");
                    SetSerializedObjectReference(view, "rootObject", view.gameObject, "Views");
                    GetOrCreateComponent<Image>(view.gameObject, "Views").color = new Color(0.08f, 0.08f, 0.08f, 0.22f);
                    GetOrCreateGameObject("ButtonLayer", view.transform, "Views");
                    viewList.Add(view);
                    viewsById[viewId] = view;
                }

                SetSerializedObjectList(controller, "views", viewList, "Locations");
                locationsById[locationId] = controller;
                locationControllers.Add(controller);
            }

            SetSerializedObjectReference(locationManager, "locationRoot", locationRoot, "Locations");
            SetSerializedObjectList(locationManager, "locationControllers", locationControllers, "Locations");
            SetSerializedBool(locationManager, "collectLocationsFromRoot", true, "Locations");
            SetSerializedString(locationManager, "startingLocationId", "Bedroom", "Locations");
            SetSerializedString(locationManager, "startingViewId", "Bedroom_Front", "Locations");
        }

        private static void EnsureDoorButtons()
        {
            for (int i = 0; i < RequiredDoors.Length; i++)
            {
                string doorId = RequiredDoors[i];
                if (FindClickableButton(b => b.ClickableType == ClickableType.Door && b.LinkedDoorId == doorId) != null)
                {
                    AddReused("Door Buttons", FindClickableButton(b => b.ClickableType == ClickableType.Door && b.LinkedDoorId == doorId).gameObject, "Door button " + doorId);
                    continue;
                }

                DoorRecord record;
                if (!doorsById.TryGetValue(doorId, out record))
                {
                    AddError("Door Buttons", "DoorRecord not found: " + doorId);
                    continue;
                }

                Transform parent = ResolveViewParent(record.fromViewId, record.fromLocationId, "Door Buttons");
                GameObject buttonObject = CreateButton("Button_Door_" + doorId, parent, "Door " + doorId, "Door Buttons");
                ClickableButton clickable = GetOrCreateComponent<ClickableButton>(buttonObject, "Door Buttons");
                clickable.clickableId = doorId;
                clickable.clickableType = ClickableType.Door;
                clickable.linkedDoorId = doorId;
                EditorUtility.SetDirty(clickable);
            }
        }

        private static void EnsurePuzzleButtons()
        {
            for (int i = 0; i < RequiredPuzzles.Length; i++)
            {
                string puzzleId = RequiredPuzzles[i];
                if (FindClickableButton(b => b.ClickableType == ClickableType.Puzzle && b.LinkedPuzzleId == puzzleId) != null)
                {
                    AddReused("Puzzle Buttons", FindClickableButton(b => b.ClickableType == ClickableType.Puzzle && b.LinkedPuzzleId == puzzleId).gameObject, "Puzzle button " + puzzleId);
                    continue;
                }

                PuzzleRecord record;
                if (!puzzlesById.TryGetValue(puzzleId, out record))
                {
                    AddError("Puzzle Buttons", "PuzzleRecord not found: " + puzzleId);
                    continue;
                }

                if (string.IsNullOrEmpty(record.prefabPath) || Resources.Load<GameObject>(record.prefabPath) == null)
                {
                    AddError("Puzzle Buttons", "Puzzle prefab does not load: " + puzzleId + " / " + record.prefabPath);
                }

                Transform parent = ResolveViewParent(GetPuzzleView(puzzleId, record.locationId), record.locationId, "Puzzle Buttons");
                GameObject buttonObject = CreateButton("Button_Puzzle_" + puzzleId, parent, "Puzzle " + puzzleId, "Puzzle Buttons");
                ClickableButton clickable = GetOrCreateComponent<ClickableButton>(buttonObject, "Puzzle Buttons");
                clickable.clickableId = puzzleId;
                clickable.clickableType = ClickableType.Puzzle;
                clickable.linkedPuzzleId = puzzleId;
                EditorUtility.SetDirty(clickable);
            }
        }

        private static void EnsureClueButtons()
        {
            for (int i = 0; i < RecommendedClues.Length; i++)
            {
                string clueId = RecommendedClues[i];
                if (FindClickableButton(b => b.ClickableType == ClickableType.ExamineImage && (b.LinkedClueImageId == clueId || b.TargetObjectId == clueId)) != null)
                {
                    AddReused("Clue Buttons", FindClickableButton(b => b.ClickableType == ClickableType.ExamineImage && (b.LinkedClueImageId == clueId || b.TargetObjectId == clueId)).gameObject, "Clue button " + clueId);
                    continue;
                }

                ClueRecord record;
                cluesById.TryGetValue(clueId, out record);
                string viewId = GetClueView(clueId, record != null ? record.locationId : string.Empty);
                Transform parent = ResolveViewParent(viewId, record != null ? record.locationId : string.Empty, "Clue Buttons");
                if (parent == null)
                {
                    AddWarning("Clue Buttons", "Could not resolve parent view for clue: " + clueId);
                    continue;
                }

                GameObject buttonObject = CreateButton("Button_Clue_" + clueId, parent, "Clue " + clueId, "Clue Buttons");
                ClickableButton clickable = GetOrCreateComponent<ClickableButton>(buttonObject, "Clue Buttons");
                clickable.clickableId = clueId;
                clickable.clickableType = ClickableType.ExamineImage;
                clickable.linkedClueImageId = clueId;
                EditorUtility.SetDirty(clickable);
            }
        }

        private static void EnsureHidePoints()
        {
            CreateHidePoint("HidePoint_Bedroom_Closet", "Bedroom_Back");
            CreateHidePoint("HidePoint_SecondFloorHallway_Cabinet", "SecondFloorHallway_Back");
            CreateHidePoint("HidePoint_Study_Desk", "Study_Left");
        }

        private static void CreateHidePoint(string hidePointId, string viewId)
        {
            if (FindClickableButton(b => b.ClickableType == ClickableType.HidePoint && (b.TargetObjectId == hidePointId || b.ClickableId == hidePointId)) != null)
            {
                AddReused("HidePoints", FindClickableButton(b => b.ClickableType == ClickableType.HidePoint && (b.TargetObjectId == hidePointId || b.ClickableId == hidePointId)).gameObject, "HidePoint " + hidePointId);
                return;
            }

            Transform parent = ResolveViewParent(viewId, string.Empty, "HidePoints");
            GameObject buttonObject = CreateButton("Button_HidePoint_" + hidePointId, parent, "Hide", "HidePoints");
            ClickableButton clickable = GetOrCreateComponent<ClickableButton>(buttonObject, "HidePoints");
            clickable.clickableId = hidePointId;
            clickable.clickableType = ClickableType.HidePoint;
            clickable.targetObjectId = hidePointId;
            HidePointController controller = GetOrCreateComponent<HidePointController>(buttonObject, "HidePoints");
            SetSerializedString(controller, "hidePointId", hidePointId, "HidePoints");
            SetSerializedObjectReference(controller, "rootObject", buttonObject, "HidePoints");
            SetSerializedBool(controller, "usable", true, "HidePoints");
            EditorUtility.SetDirty(clickable);
        }

        private static void EnsureFinalDoor()
        {
            if (FindClickableButton(b => b.ClickableType == ClickableType.FinalDoor && b.RequiredItemId == "FrontDoorKey") != null)
            {
                AddReused("FinalDoor", FindClickableButton(b => b.ClickableType == ClickableType.FinalDoor && b.RequiredItemId == "FrontDoorKey").gameObject, "FinalDoor FrontDoorKey");
                return;
            }

            Transform parent = ResolveViewParent("Entrance_Front", "Entrance", "FinalDoor");
            GameObject buttonObject = CreateButton("Button_FinalDoor_FrontDoorKey", parent, "Final Door", "FinalDoor");
            ClickableButton clickable = GetOrCreateComponent<ClickableButton>(buttonObject, "FinalDoor");
            clickable.clickableId = "FinalDoor_FrontDoorKey";
            clickable.clickableType = ClickableType.FinalDoor;
            clickable.requiredItemId = "FrontDoorKey";
            clickable.linkedPuzzleId = "Puzzle_Entrance_01";
            clickable.linkedDoorId = string.Empty;
            EditorUtility.SetDirty(clickable);
        }

        private static void EnsureBuildSettings()
        {
            EnsureSceneInBuildSettings("Assets/Scenes/TitleScene.unity");
            EnsureSceneInBuildSettings(GameScenePath);
        }

        private static void EnsureSceneInBuildSettings(string scenePath)
        {
            if (!File.Exists(scenePath))
            {
                AddWarning("Build Settings", "Scene file not found, cannot add to Build Settings: " + scenePath);
                return;
            }

            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].path == scenePath)
                {
                    AddReused("Build Settings", null, "Scene already in Build Settings: " + scenePath);
                    return;
                }
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            AddInfo("Build Settings", "Added scene to Build Settings: " + scenePath);
        }

        private static Transform ResolveViewParent(string viewId, string locationId, string category)
        {
            CacheSceneObjects();
            LocationView view;
            if (!string.IsNullOrEmpty(viewId) && viewsById.TryGetValue(viewId, out view) && view != null)
            {
                Transform layer = view.transform.Find("ButtonLayer");
                return layer != null ? layer : view.transform;
            }

            if (!string.IsNullOrEmpty(locationId))
            {
                string fallbackViewId = GetDefaultViewForLocation(locationId);
                if (!string.IsNullOrEmpty(fallbackViewId) && viewsById.TryGetValue(fallbackViewId, out view) && view != null)
                {
                    AddWarning(category, "Could not find view " + viewId + ". Used default view " + fallbackViewId + ".");
                    Transform layer = view.transform.Find("ButtonLayer");
                    return layer != null ? layer : view.transform;
                }
            }

            AddWarning(category, "Could not resolve parent view: " + viewId + " / location=" + locationId);
            return locationRoot;
        }

        private static string GetDefaultViewForLocation(string locationId)
        {
            for (int i = 0; i < LocationViews.GetLength(0); i++)
            {
                if (LocationViews[i, 0] == locationId)
                {
                    return LocationViews[i, 2];
                }
            }

            return string.Empty;
        }

        private static string GetPuzzleView(string puzzleId, string locationId)
        {
            switch (puzzleId)
            {
                case "Puzzle_Bedroom_01": return "Bedroom_Front";
                case "Puzzle_LivingRoom_01": return "LivingRoom_Front";
                case "Puzzle_ChildRoom_01": return "ChildRoom_Front";
                case "Puzzle_Study_01": return "Study_Front";
                case "Puzzle_LivingRoom_02": return "LivingRoom_Back";
                case "Puzzle_Kitchen_01": return "Kitchen_Front";
                case "Puzzle_BasementStorage_01": return "BasementStorage_Front";
                case "Puzzle_LockedRoom_01": return "LockedRoom_Front";
                case "Puzzle_Entrance_01": return "Entrance_Front";
                default: return GetDefaultViewForLocation(locationId);
            }
        }

        private static string GetClueView(string clueId, string locationId)
        {
            switch (clueId)
            {
                case "BedroomPhotoCodeClue": return "Bedroom_Left";
                case "LivingRoomEntranceCodeClue": return "LivingRoom_Front";
                case "ChildRoomCardSymbolClueImage": return "ChildRoom_Right";
                case "StudyBookSymbolClueImage": return "Study_Right";
                case "KitchenCodeClueImage": return "LivingRoom_Back";
                case "KitchenFridgeSurfaceSymbolClue": return "Kitchen_Front";
                case "BasementPowerPatternClue": return "BasementStorage_Left";
                case "BasementClueImage": return "BasementStorage_Back";
                default: return GetDefaultViewForLocation(locationId);
            }
        }

        private static ClickableButton FindClickableButton(Predicate<ClickableButton> predicate)
        {
            ClickableButton[] buttons = FindComponentsInScene<ClickableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null && predicate(buttons[i]))
                {
                    return buttons[i];
                }
            }

            return null;
        }

        private static void WriteStaticDocs()
        {
            WriteGeneratedWiringMap();
            WriteManualPolishChecklist();
            WriteUsageDoc();
        }

        private static void WriteGeneratedWiringMap()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# GameScene Generated Wiring Map");
            builder.AppendLine();
            builder.AppendLine("## Managers");
            builder.AppendLine();
            builder.AppendLine("- GameManager");
            builder.AppendLine("- SaveManager");
            builder.AppendLine("- GameDataManager");
            builder.AppendLine("- LocationManager");
            builder.AppendLine("- InteractionManager");
            builder.AppendLine("- InventoryManager");
            builder.AppendLine("- PuzzleManager");
            builder.AppendLine("- EndingManager");
            builder.AppendLine("- ClueImageManager");
            builder.AppendLine("- NoiseManager");
            builder.AppendLine("- GhostManager");
            builder.AppendLine("- HideManager");
            builder.AppendLine("- ChaseManager");
            builder.AppendLine();
            builder.AppendLine("## UI Roots");
            builder.AppendLine();
            builder.AppendLine("- LocationRoot");
            builder.AppendLine("- NavigationButtons");
            builder.AppendLine("- InventoryBar");
            builder.AppendLine("- PuzzleUIRoot");
            builder.AppendLine("- ClueImagePanel");
            builder.AppendLine("- GameOverPanel");
            builder.AppendLine("- EndingPanel");
            builder.AppendLine("- HideExitButton");
            builder.AppendLine("- GhostStatusPanel");
            builder.AppendLine();
            AppendDoorMap(builder);
            AppendPuzzleMap(builder);
            AppendClueMap(builder);
            builder.AppendLine("## HidePoints");
            builder.AppendLine();
            builder.AppendLine("| HidePoint ID | Parent View | Button Name |");
            builder.AppendLine("|---|---|---|");
            builder.AppendLine("| HidePoint_Bedroom_Closet | Bedroom_Back | Button_HidePoint_HidePoint_Bedroom_Closet |");
            builder.AppendLine("| HidePoint_SecondFloorHallway_Cabinet | SecondFloorHallway_Back | Button_HidePoint_HidePoint_SecondFloorHallway_Cabinet |");
            builder.AppendLine("| HidePoint_Study_Desk | Study_Left | Button_HidePoint_HidePoint_Study_Desk |");
            builder.AppendLine();
            builder.AppendLine("## FinalDoor");
            builder.AppendLine();
            builder.AppendLine("| Parent View | Button Name | Required Item | Linked Puzzle |");
            builder.AppendLine("|---|---|---|---|");
            builder.AppendLine("| Entrance_Front | Button_FinalDoor_FrontDoorKey | FrontDoorKey | Puzzle_Entrance_01 |");
            WriteTextAsset("Assets/Docs/GameSceneGeneratedWiringMap.md", builder.ToString());
        }

        private static void AppendDoorMap(StringBuilder builder)
        {
            builder.AppendLine("## Door Buttons");
            builder.AppendLine();
            builder.AppendLine("| Door ID | Parent View | Button Name |");
            builder.AppendLine("|---|---|---|");
            for (int i = 0; i < RequiredDoors.Length; i++)
            {
                DoorRecord record;
                string view = doorsById.TryGetValue(RequiredDoors[i], out record) ? record.fromViewId : string.Empty;
                builder.AppendLine("| " + RequiredDoors[i] + " | " + view + " | Button_Door_" + RequiredDoors[i] + " |");
            }
            builder.AppendLine();
        }

        private static void AppendPuzzleMap(StringBuilder builder)
        {
            builder.AppendLine("## Puzzle Buttons");
            builder.AppendLine();
            builder.AppendLine("| Puzzle ID | Parent View | Button Name |");
            builder.AppendLine("|---|---|---|");
            for (int i = 0; i < RequiredPuzzles.Length; i++)
            {
                PuzzleRecord record;
                puzzlesById.TryGetValue(RequiredPuzzles[i], out record);
                builder.AppendLine("| " + RequiredPuzzles[i] + " | " + GetPuzzleView(RequiredPuzzles[i], record != null ? record.locationId : string.Empty) + " | Button_Puzzle_" + RequiredPuzzles[i] + " |");
            }
            builder.AppendLine();
        }

        private static void AppendClueMap(StringBuilder builder)
        {
            builder.AppendLine("## Clue Buttons");
            builder.AppendLine();
            builder.AppendLine("| Clue ID | Parent View | Button Name |");
            builder.AppendLine("|---|---|---|");
            for (int i = 0; i < RecommendedClues.Length; i++)
            {
                ClueRecord record;
                cluesById.TryGetValue(RecommendedClues[i], out record);
                builder.AppendLine("| " + RecommendedClues[i] + " | " + GetClueView(RecommendedClues[i], record != null ? record.locationId : string.Empty) + " | Button_Clue_" + RecommendedClues[i] + " |");
            }
            builder.AppendLine();
        }

        private static void WriteManualPolishChecklist()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# GameScene Manual Polish Checklist");
            builder.AppendLine();
            builder.AppendLine("- [ ] Replace placeholder backgrounds");
            builder.AppendLine("- [ ] Adjust button positions");
            builder.AppendLine("- [ ] Adjust button sizes");
            builder.AppendLine("- [ ] Place Door buttons over actual door art");
            builder.AppendLine("- [ ] Place Puzzle buttons over actual objects");
            builder.AppendLine("- [ ] Place ExamineImage buttons over clue objects");
            builder.AppendLine("- [ ] Place HidePoint buttons over hiding objects");
            builder.AppendLine("- [ ] Polish ClueImagePanel");
            builder.AppendLine("- [ ] Polish InventoryBar");
            builder.AppendLine("- [ ] Polish GameOverPanel");
            builder.AppendLine("- [ ] Polish EndingPanel");
            builder.AppendLine("- [ ] Hide or remove GhostStatusUI before final release");
            builder.AppendLine("- [ ] Run Source Route Scene Wiring Validator");
            builder.AppendLine("- [ ] Run full manual playtest");
            WriteTextAsset("Assets/Docs/GameSceneManualPolishChecklist.md", builder.ToString());
        }

        private static void WriteUsageDoc()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# Source Route GameScene Builder Usage");
            builder.AppendLine();
            builder.AppendLine("## Purpose");
            builder.AppendLine();
            builder.AppendLine("Creates placeholder GameScene wiring for the source-aligned route. This is an Editor-only builder and does not create runtime auto-layout.");
            builder.AppendLine();
            builder.AppendLine("## Menus");
            builder.AppendLine();
            builder.AppendLine("- `Escape From Nightmare / Scene Setup / Build Missing Source Route GameScene Wiring`");
            builder.AppendLine("- `Escape From Nightmare / Scene Setup / Build Missing Source Route GameScene Wiring And Save With Backup`");
            builder.AppendLine("- `Escape From Nightmare / Scene Setup / Generate Source Route GameScene Builder Report`");
            builder.AppendLine();
            builder.AppendLine("## Backup");
            builder.AppendLine();
            builder.AppendLine("The save menu backs up `Assets/Scenes/GameScene.unity` into `Assets/Backups/Scenes` before saving.");
            builder.AppendLine();
            builder.AppendLine("## Recommended Validation Order");
            builder.AppendLine();
            builder.AppendLine("1. Validate Game Data");
            builder.AppendLine("2. Validate Puzzle Prefab Contracts");
            builder.AppendLine("3. Build Missing Source Route GameScene Wiring And Save With Backup");
            builder.AppendLine("4. Validate Source Route Scene Wiring");
            builder.AppendLine("5. Generate Scene Wiring Report");
            builder.AppendLine("6. Run First Five Runtime Tests");
            builder.AppendLine("7. Run Remaining Runtime Tests");
            builder.AppendLine("8. Run Full Game Route Runtime Test");
            WriteTextAsset("Assets/Docs/SourceRouteGameSceneBuilderUsage.md", builder.ToString());
        }

        private static void WriteBuilderReport()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# Source Route GameScene Builder Report");
            builder.AppendLine();
            builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendLine("- Scene: " + SceneManager.GetActiveScene().path);
            builder.AppendLine("- Saved: " + lastRunSaved);
            builder.AppendLine("- Backup Path: " + lastBackupPath);
            builder.AppendLine();
            builder.AppendLine("## Summary");
            builder.AppendLine();
            builder.AppendLine("| Category | Created | Reused | Linked | Warnings | Errors |");
            builder.AppendLine("|---|---:|---:|---:|---:|---:|");
            foreach (SourceRouteGameSceneBuilderCategoryStats entry in stats.Values)
            {
                builder.AppendLine("| " + entry.category + " | " + entry.created + " | " + entry.reused + " | " + entry.linked + " | " + entry.warnings + " | " + entry.errors + " |");
            }
            builder.AppendLine();
            AppendReportList(builder, "Created Objects", createdObjects);
            AppendReportList(builder, "Reused Objects", reusedObjects);
            AppendReportList(builder, "Linked Fields", linkedFields);
            AppendReportList(builder, "Warnings", warnings);
            AppendReportList(builder, "Errors", errors);
            builder.AppendLine("## Next Validation");
            builder.AppendLine();
            builder.AppendLine("- Run Escape From Nightmare / Validate Source Route Scene Wiring");
            builder.AppendLine("- Run Escape From Nightmare / Generate Scene Wiring Report");
            builder.AppendLine("- Run Full Game Route Runtime Test");
            WriteTextAsset(ReportPath, builder.ToString());
        }

        private static void AppendReportList(StringBuilder builder, string title, List<string> values)
        {
            builder.AppendLine("## " + title);
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

        private static void WriteTextAsset(string assetPath, string contents)
        {
            string directory = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(assetPath, contents);
        }

        private static GameObject GetOrCreateGameObject(string name, Transform parent = null, string category = "UI Roots")
        {
            GameObject existing = null;
            if (parent != null)
            {
                Transform child = parent.Find(name);
                if (child != null)
                {
                    existing = child.gameObject;
                }
            }
            else
            {
                existing = FindObjectInSceneByName(name);
            }

            if (existing != null)
            {
                AddReused(category, existing, "Object exists");
                return existing;
            }

            GameObject created = new GameObject(name, typeof(RectTransform));
            if (parent != null)
            {
                created.transform.SetParent(parent, false);
            }
            SetRectStretch(created.GetComponent<RectTransform>());
            AddCreated(category, created);
            return created;
        }

        private static T GetOrCreateComponent<T>(GameObject go, string category = "UI Roots") where T : Component
        {
            if (go == null)
            {
                AddError(category, "Cannot add component to null GameObject: " + typeof(T).Name);
                return null;
            }

            T component = go.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            component = go.AddComponent<T>();
            AddInfo(category, "Added component " + typeof(T).Name + " to " + GetHierarchyPath(go));
            return component;
        }

        private static GameObject CreatePanel(string name, Transform parent, string category)
        {
            GameObject panel = GetOrCreateGameObject(name, parent, category);
            Image image = GetOrCreateComponent<Image>(panel, category);
            image.color = new Color(0.05f, 0.05f, 0.05f, 0.12f);
            SetRectStretch(panel.GetComponent<RectTransform>());
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string label, string category)
        {
            GameObject buttonObject = GetOrCreateGameObject(name, parent, category);
            Image image = GetOrCreateComponent<Image>(buttonObject, category);
            image.color = new Color(0.18f, 0.2f, 0.24f, 0.82f);
            GetOrCreateComponent<Button>(buttonObject, category);
            Text text = CreateText("Text", buttonObject.transform, label, 18, category);
            text.alignment = TextAnchor.MiddleCenter;
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(220f, 48f);
            }
            return buttonObject;
        }

        private static Text CreateText(string name, Transform parent, string text, int fontSize, string category)
        {
            GameObject textObject = GetOrCreateGameObject(name, parent, category);
            Text uiText = GetOrCreateComponent<Text>(textObject, category);
            uiText.text = text;
            uiText.fontSize = fontSize;
            uiText.color = Color.white;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.font = GetBuiltinFont();
            SetRectStretch(textObject.GetComponent<RectTransform>());
            return uiText;
        }

        private static Image GetOrCreateImage(string name, Transform parent, string category)
        {
            GameObject imageObject = GetOrCreateGameObject(name, parent, category);
            Image image = GetOrCreateComponent<Image>(imageObject, category);
            image.color = new Color(1f, 1f, 1f, 0.12f);
            return image;
        }

        private static Font GetBuiltinFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            if (font == null)
            {
                AddWarning("UI Roots", "Builtin UI font not found. Text components will use null font.");
            }
            return font;
        }

        private static void SetRectStretch(RectTransform rect)
        {
            if (rect == null)
            {
                return;
            }
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        private static void SetSerializedObjectReference(UnityEngine.Object target, string fieldName, UnityEngine.Object value, string category)
        {
            SerializedProperty property = FindProperty(target, fieldName, category);
            if (property == null)
            {
                return;
            }
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                AddWarning(category, "Field is not an object reference: " + target.name + "." + fieldName);
                return;
            }
            property.objectReferenceValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AddLinked(category, target, fieldName, value);
        }

        private static void SetSerializedString(UnityEngine.Object target, string fieldName, string value, string category)
        {
            SerializedProperty property = FindProperty(target, fieldName, category);
            if (property == null)
            {
                return;
            }
            property.stringValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AddLinked(category, target, fieldName, null);
        }

        private static void SetSerializedBool(UnityEngine.Object target, string fieldName, bool value, string category)
        {
            SerializedProperty property = FindProperty(target, fieldName, category);
            if (property == null)
            {
                return;
            }
            property.boolValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AddLinked(category, target, fieldName, null);
        }

        private static void SetSerializedInt(UnityEngine.Object target, string fieldName, int value, string category)
        {
            SerializedProperty property = FindProperty(target, fieldName, category);
            if (property == null)
            {
                return;
            }
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                property.enumValueIndex = value;
            }
            else
            {
                property.intValue = value;
            }
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AddLinked(category, target, fieldName, null);
        }

        private static void SetSerializedObjectList(UnityEngine.Object target, string fieldName, IList<UnityEngine.Object> values, string category)
        {
            SerializedProperty property = FindProperty(target, fieldName, category);
            if (property == null || !property.isArray)
            {
                AddWarning(category, "Field is not an array/list: " + (target != null ? target.name : "(null)") + "." + fieldName);
                return;
            }
            property.arraySize = values != null ? values.Count : 0;
            for (int i = 0; values != null && i < values.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AddLinked(category, target, fieldName, null);
        }

        private static SerializedProperty FindProperty(UnityEngine.Object target, string fieldName, string category)
        {
            if (target == null)
            {
                AddWarning(category, "Cannot set field on null target: " + fieldName);
                return null;
            }
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                AddWarning(category, "Serialized field not found: " + target.name + "." + fieldName);
            }
            return property;
        }

        private static GameObject FindObjectInSceneByName(string name)
        {
            GameObject[] objects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]) && objects[i].name == name)
                {
                    return objects[i];
                }
            }
            return null;
        }

        private static T FindComponentInScene<T>() where T : Component
        {
            T[] components = FindComponentsInScene<T>();
            return components.Length > 0 ? components[0] : null;
        }

        private static T[] FindComponentsInScene<T>() where T : Component
        {
            List<T> result = new List<T>();
            T[] components = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < components.Length; i++)
            {
                if (IsSceneObject(components[i]))
                {
                    result.Add(components[i]);
                }
            }
            return result.ToArray();
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

        private static string GetHierarchyPath(GameObject go)
        {
            if (go == null)
            {
                return "(null)";
            }
            string path = go.name;
            Transform current = go.transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            return path;
        }

        private static void EnsureFolder(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || Directory.Exists(assetPath))
            {
                return;
            }
            Directory.CreateDirectory(assetPath);
        }

        private static SourceRouteGameSceneBuilderCategoryStats GetStats(string category)
        {
            if (!stats.ContainsKey(category))
            {
                stats.Add(category, new SourceRouteGameSceneBuilderCategoryStats { category = category });
            }
            return stats[category];
        }

        private static void AddCreated(string category, GameObject go)
        {
            GetStats(category).created++;
            if (go != null)
            {
                createdObjects.Add(GetHierarchyPath(go));
                EditorUtility.SetDirty(go);
            }
        }

        private static void AddReused(string category, GameObject go, string reason)
        {
            GetStats(category).reused++;
            reusedObjects.Add((go != null ? GetHierarchyPath(go) : "(asset)") + " - " + reason);
        }

        private static void AddLinked(string category, UnityEngine.Object target, string fieldName, UnityEngine.Object value)
        {
            GetStats(category).linked++;
            string valueName = value != null ? value.name : "(value)";
            linkedFields.Add((target != null ? target.name : "(null)") + "." + fieldName + " = " + valueName);
            if (target != null)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private static void AddInfo(string category, string message)
        {
            GetStats(category);
            Debug.Log("[SourceRouteGameSceneBuilder] " + message);
        }

        private static void AddWarning(string category, string message)
        {
            GetStats(category).warnings++;
            warnings.Add(message);
            Debug.LogWarning("[SourceRouteGameSceneBuilder] " + message);
        }

        private static void AddError(string category, string message)
        {
            GetStats(category).errors++;
            errors.Add(message);
            Debug.LogError("[SourceRouteGameSceneBuilder] " + message);
        }
    }
}
