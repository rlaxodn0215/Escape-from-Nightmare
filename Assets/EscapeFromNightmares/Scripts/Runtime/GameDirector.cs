using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using EscapeFromNightmares.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    public sealed class GameDirector : MonoBehaviour
    {
        [Header("Catalogs")]
        [SerializeField] private RoomSpriteCatalog spriteCatalog;

        [Header("Room View")]
        [SerializeField] private Image roomFaceImage;
        [SerializeField] private RectTransform roomObjectLayer;
        [SerializeField] private Button rotateLeftButton;
        [SerializeField] private Button rotateRightButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private InventoryWindow inventoryWindow;

        [Header("Close Up View")]
        [SerializeField] private GameObject closeUpPanel;
        [SerializeField] private Image closeUpImage;
        [SerializeField] private Button closeUpActionButton;
        [SerializeField] private Button closeUpItemButton;
        [SerializeField] private Button closeUpCloseButton;
        [SerializeField] private RectTransform closeUpItemHitbox;

        [Header("Hide View")]
        [SerializeField] private GameObject hideViewPanel;
        [SerializeField] private Image hideViewImage;
        [SerializeField] private Button hideExitButton;

        [Header("Puzzle View")]
        [SerializeField] private GameObject puzzlePanel;
        [SerializeField] private Text puzzleTitleText;
        [SerializeField] private Text puzzleInputText;
        [SerializeField] private Text puzzleLogText;
        [SerializeField] private Image puzzleCloseUpImage;
        [SerializeField] private RectTransform puzzleTokenPanel;
        [SerializeField] private Button puzzleBackButton;
        [SerializeField] private Image[] studySafeDigitImages = new Image[StudySafeDigitCount];
        [SerializeField] private Button[] studySafeDigitButtons = new Button[StudySafeDigitCount];

        [Header("Scene Transition")]
        [SerializeField] private CanvasGroup sceneTransitionOverlay;

        private StageDefinition stage;
        private GameSession session;
        private InventoryService inventoryService;
        private PuzzleService puzzleService;
        private FlagService flagService;
        private EscapeActionResolver actionResolver;
        private DangerSystem dangerSystem;
        private HidingSystem hidingSystem;
        private MonsterAIController monsterAI;
        private SettingsSaveService saveService;
        private ResourceManager resourceManager;
        private SoundManager soundManager;

        private readonly Dictionary<string, RoomDefinition> rooms = new Dictionary<string, RoomDefinition>();
        private readonly Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>();
        private readonly Dictionary<string, PuzzleDefinition> puzzles = new Dictionary<string, PuzzleDefinition>();
        private readonly List<string> puzzleInputTokens = new List<string>();
        private readonly int[] studySafeDigits = new int[StudySafeDigitCount];

        private PuzzleDefinition activePuzzle;
        private InteractableDefinition activeCloseUpInteractable;
        private Coroutine closeUpFadeRoutine;
        private Coroutine hideViewFadeRoutine;
        private Coroutine puzzleFadeRoutine;
        private Coroutine sceneTransitionRoutine;
        private bool sceneTransitionInProgress;
        private bool studySafeSolvedInPanel;
        private const float ModalFadeDuration = 0.22f;
        private const float SceneFadeDuration = 0.24f;
        private const string StudySafePuzzleId = "study_safe";
        private const string StudySafeUnlockedFlag = "study_safe_unlocked";
        private const string StudySafeOpenedFlag = "study_safe_opened";
        private const string StudySafeClearFlag = "puzzle_study_safe_clear";
        private const string StudySafeSuccessEventId = "event_study_safe_success";
        private const int StudySafeDigitCount = 4;
        private static readonly Rect[] StudySafeDigitRects =
        {
            new Rect(0.374f, 0.282f, 0.056f, 0.104f),
            new Rect(0.445f, 0.282f, 0.047f, 0.104f),
            new Rect(0.503f, 0.282f, 0.048f, 0.104f),
            new Rect(0.566f, 0.282f, 0.047f, 0.104f)
        };

        public void SetSceneReferences(
            RoomSpriteCatalog roomSprites,
            Image roomImage,
            RectTransform objectLayer,
            Button leftButton,
            Button rightButton,
            Button inventoryOpenButton,
            InventoryWindow inventory,
            GameObject drawerCloseUpPanel,
            Image drawerCloseUpImage,
            Button drawerOpenButton,
            Button drawerItemButton,
            Button drawerCloseButton,
            RectTransform drawerItemHitbox,
            GameObject bedHidePanel,
            Image bedHideImage,
            Button bedHideExitButton,
            GameObject commonPuzzlePanel,
            Text commonPuzzleTitle,
            Text commonPuzzleInput,
            Text commonPuzzleLog,
            Image commonPuzzleImage,
            RectTransform commonPuzzleTokens,
            Button commonPuzzleBackButton = null,
            Image[] safeDigitImages = null,
            Button[] safeDigitButtons = null,
            CanvasGroup transitionOverlay = null)
        {
            spriteCatalog = roomSprites;
            roomFaceImage = roomImage;
            roomObjectLayer = objectLayer;
            rotateLeftButton = leftButton;
            rotateRightButton = rightButton;
            inventoryButton = inventoryOpenButton;
            inventoryWindow = inventory;
            closeUpPanel = drawerCloseUpPanel;
            closeUpImage = drawerCloseUpImage;
            closeUpActionButton = drawerOpenButton;
            closeUpItemButton = drawerItemButton;
            closeUpCloseButton = drawerCloseButton;
            closeUpItemHitbox = drawerItemHitbox;
            hideViewPanel = bedHidePanel;
            hideViewImage = bedHideImage;
            hideExitButton = bedHideExitButton;
            puzzlePanel = commonPuzzlePanel;
            puzzleTitleText = commonPuzzleTitle;
            puzzleInputText = commonPuzzleInput;
            puzzleLogText = commonPuzzleLog;
            puzzleCloseUpImage = commonPuzzleImage;
            puzzleTokenPanel = commonPuzzleTokens;
            puzzleBackButton = commonPuzzleBackButton;
            studySafeDigitImages = NormalizeArray(safeDigitImages, StudySafeDigitCount);
            studySafeDigitButtons = NormalizeArray(safeDigitButtons, StudySafeDigitCount);
            sceneTransitionOverlay = transitionOverlay;
        }

        private void Awake()
        {
            stage = RuntimeStageFactory.CreateStage1();
            foreach (var room in stage.rooms)
            {
                rooms[room.roomId] = room;
            }

            foreach (var item in stage.items)
            {
                items[item.itemId] = item;
            }

            foreach (var puzzle in stage.puzzles)
            {
                puzzles[puzzle.puzzleId] = puzzle;
            }

            session = new GameSession();
            flagService = new FlagService(session);
            inventoryService = new InventoryService(session);
            puzzleService = new PuzzleService(session, flagService);
            actionResolver = new EscapeActionResolver(session, flagService, stage.soundCatalog);
            dangerSystem = new DangerSystem();
            hidingSystem = new HidingSystem();
            monsterAI = new MonsterAIController();
            saveService = new SettingsSaveService(Application.persistentDataPath);
            resourceManager = new ResourceManager(ResourcePathCatalog.CreateDefault());

            EnsureEventSystem();
            EnsureSoundManager();
            EnsureUi();
            BindUi();
        }

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (session == null || string.IsNullOrWhiteSpace(session.CurrentRoomId))
            {
                return;
            }

            var room = CurrentRoom();
            hidingSystem.Tick(Time.deltaTime, MousePosition());
            dangerSystem.Tick(Time.deltaTime, room, monsterAI.State);
            monsterAI.Tick(dangerSystem, hidingSystem);

            if (dangerSystem.CaptureGauge >= 100f)
            {
                GameOver();
            }
        }

        private void StartGame()
        {
            session.Start(stage);
            dangerSystem.Reset();
            hidingSystem.Exit();
            monsterAI.Reset();
            HideCloseUpPanelImmediate();
            HideHideViewImmediate();
            HidePuzzlePanelImmediate();
            inventoryWindow?.Close();
            PlaySoundById("bgm_stage1_ambient");
            OpenRoomImmediate(session.CurrentRoomId);
            Debug.Log("MainScene started: " + session.CurrentRoomId);
        }

        private void OpenRoom(string roomId)
        {
            StartSceneTransition(() => OpenRoomImmediate(roomId));
        }

        private void OpenRoomImmediate(string roomId)
        {
            if (!rooms.ContainsKey(roomId))
            {
                Debug.LogWarning("Room not found: " + roomId);
                return;
            }

            session.MoveTo(roomId);
            hidingSystem.Exit();
            HideHideViewImmediate();
            dangerSystem.OnRoomChanged();
            RenderRoom();
            Debug.Log("Room opened: " + roomId + " / " + session.CurrentFaceDirection);
        }

        private void RenderRoom()
        {
            var room = CurrentRoom();
            var face = CurrentFace(room);
            var faceInteractables = face.interactables ?? new InteractableDefinition[0];

            if (roomFaceImage != null)
            {
                roomFaceImage.sprite = ResolveSprite(ResolveRoomFaceBackgroundResource(face, flagService));
                roomFaceImage.color = Color.white;
                roomFaceImage.preserveAspect = true;
            }

            RenderRoomObjects(faceInteractables);
        }

        private void RenderRoomObjects(InteractableDefinition[] interactables)
        {
            if (roomObjectLayer == null)
            {
                return;
            }

            ClearChildren(roomObjectLayer);
            foreach (var interactable in interactables)
            {
                if (!ShouldRenderRoomHitbox(interactable, session))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(interactable.imageResource))
                {
                    continue;
                }

                var button = interactable.showWorldImage
                    ? CreateImageButton(interactable.interactableId, roomObjectLayer, ResolveSprite(interactable.imageResource), () => HandleInteractable(interactable))
                    : CreateTransparentButton(interactable.interactableId, roomObjectLayer, () => HandleInteractable(interactable));
                Stretch(button.GetComponent<RectTransform>(), interactable.normalizedHitbox);
            }
        }

        private void HandleInteractable(InteractableDefinition interactable)
        {
            if (ShouldOpenUnlockedStudySafe(interactable))
            {
                session.SetFlag(StudySafeOpenedFlag);
                OpenCloseUp(interactable.interactableId);
                RenderRoom();
                return;
            }

            var result = actionResolver.ResolveInteractable(interactable);
            if (!result.Succeeded)
            {
                Debug.Log(result.Message);
                return;
            }

            ExecuteActions(result.Actions);
            if (interactable.oneShot)
            {
                session.MarkInteractableUsed(interactable.interactableId);
            }

            if (interactable.consumeOnUse)
            {
                session.RemoveItem(interactable.requiredItemId);
            }

            if (!result.Actions.Any(IsRoomTransitionAction))
            {
                RenderRoom();
            }
        }

        private bool AcquireItem(string itemId)
        {
            if (!inventoryService.Acquire(itemId))
            {
                Debug.Log("Item already acquired or invalid: " + itemId);
                return false;
            }

            Debug.Log("Item acquired: " + ItemName(itemId));
            if (itemId == "front_door_key")
            {
                monsterAI.Enable();
                monsterAI.ForceChase();
                PlaySoundById("bgm_final_chase");
                Debug.Log("Final chase started.");
            }

            RefreshInventoryWindowIfOpen();
            return true;
        }

        private void OpenCloseUp(string interactableId)
        {
            activeCloseUpInteractable = FindInteractable(interactableId);
            if (activeCloseUpInteractable == null)
            {
                Debug.LogWarning("Close-up interactable not found: " + interactableId);
                return;
            }

            var state = GetCloseUpState(activeCloseUpInteractable);
            ApplyCloseUpState(state);
            if (closeUpPanel != null)
            {
                StartPanelFade(closeUpPanel, true, ref closeUpFadeRoutine);
            }

            Debug.Log("Close-up opened: " + interactableId + " / " + state);
        }

        private void OpenActiveDrawer()
        {
            if (activeCloseUpInteractable == null)
            {
                return;
            }

            var state = session.HasItem(activeCloseUpInteractable.closeUpItemId)
                ? CloseUpState.OpenEmpty
                : CloseUpState.OpenWithItem;
            ApplyCloseUpState(state);
            PlaySoundById(activeCloseUpInteractable.closeUpOpenSoundId);
            Debug.Log("Close-up state changed: " + state);
        }

        private void AcquireCloseUpItem()
        {
            if (activeCloseUpInteractable == null)
            {
                return;
            }

            var itemId = activeCloseUpInteractable.closeUpItemId;
            if (string.IsNullOrWhiteSpace(itemId))
            {
                itemId = activeCloseUpInteractable.grantsItemId;
            }

            var acquired = AcquireItem(itemId);
            if (acquired)
            {
                PlaySoundById("sfx_item_pickup");
            }

            session.MarkInteractableUsed(activeCloseUpInteractable.interactableId);
            if (activeCloseUpInteractable.puzzleId == StudySafePuzzleId)
            {
                session.MarkPuzzleSolved(StudySafePuzzleId);
                session.SetFlag(StudySafeClearFlag);
                session.SetFlag(StudySafeSuccessEventId);
                PlaySoundById("sfx_puzzle_success");
            }

            ApplyCloseUpState(CloseUpState.OpenEmpty);
            RenderRoom();
        }

        private CloseUpState GetCloseUpState(InteractableDefinition interactable)
        {
            if (!string.IsNullOrWhiteSpace(interactable.clueViewResource))
            {
                return CloseUpState.Closed;
            }

            if (interactable.puzzleId == StudySafePuzzleId && session.HasFlag(StudySafeOpenedFlag))
            {
                return session.HasItem(interactable.closeUpItemId) || session.HasUsedInteractable(interactable.interactableId)
                    ? CloseUpState.OpenEmpty
                    : CloseUpState.OpenWithItem;
            }

            return session.HasItem(interactable.closeUpItemId) || session.HasUsedInteractable(interactable.interactableId)
                ? CloseUpState.OpenEmpty
                : CloseUpState.Closed;
        }

        private void ApplyCloseUpState(CloseUpState state)
        {
            if (activeCloseUpInteractable == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(activeCloseUpInteractable.clueViewResource))
            {
                if (closeUpImage != null)
                {
                    closeUpImage.sprite = ResolveSprite(activeCloseUpInteractable.clueViewResource);
                    closeUpImage.color = Color.white;
                    closeUpImage.preserveAspect = true;
                }

                SetButtonVisible(closeUpActionButton, false);
                SetButtonVisible(closeUpItemButton, false);
                return;
            }

            if (closeUpImage != null)
            {
                closeUpImage.sprite = ResolveSprite(CloseUpSpriteResource(activeCloseUpInteractable, state));
                closeUpImage.color = Color.white;
                closeUpImage.preserveAspect = true;
            }

            SetButtonVisible(closeUpActionButton, state == CloseUpState.Closed);
            SetButtonVisible(closeUpItemButton, state == CloseUpState.OpenWithItem);
            if (closeUpItemHitbox != null)
            {
                Stretch(closeUpItemHitbox, activeCloseUpInteractable.closeUpItemHitbox);
            }
        }

        private void CloseCloseUpPanel()
        {
            if (closeUpPanel != null && closeUpPanel.activeSelf && activeCloseUpInteractable != null)
            {
                PlaySoundById(activeCloseUpInteractable.closeUpCloseSoundId);
            }

            activeCloseUpInteractable = null;
            if (closeUpPanel != null)
            {
                StartPanelFade(closeUpPanel, false, ref closeUpFadeRoutine);
            }
        }

        private void HideCloseUpPanelImmediate()
        {
            activeCloseUpInteractable = null;
            HidePanelImmediate(closeUpPanel, ref closeUpFadeRoutine);
        }

        private void OpenInventoryWindow()
        {
            if (inventoryWindow == null)
            {
                Debug.Log("Inventory: " + (session.InventoryItems.Count == 0 ? "empty" : string.Join(", ", session.InventoryItems.Select(ItemName))));
                return;
            }

            inventoryWindow.Open(session.InventoryItems, session.SelectedItemId, ResolveItemIcon, SelectInventoryItem);
            Debug.Log("Inventory opened.");
        }

        private void RefreshInventoryWindowIfOpen()
        {
            if (inventoryWindow != null && inventoryWindow.IsOpen)
            {
                inventoryWindow.Refresh(session.InventoryItems, session.SelectedItemId, ResolveItemIcon, SelectInventoryItem);
            }
        }

        private void SelectInventoryItem(string itemId)
        {
            session.SelectItem(itemId);
            Debug.Log("Selected item: " + ItemName(session.SelectedItemId));
            RefreshInventoryWindowIfOpen();
        }

        private void SolvePuzzle(string puzzleId)
        {
            if (!puzzles.TryGetValue(puzzleId, out var puzzle))
            {
                Debug.LogWarning("Puzzle not found: " + puzzleId);
                return;
            }

            OpenPuzzlePanel(puzzle);
        }

        private void SubmitActivePuzzle()
        {
            if (activePuzzle == null)
            {
                return;
            }

            var solved = puzzleService.TrySolve(activePuzzle, puzzleInputTokens);
            if (!solved)
            {
                ExecuteActions(actionResolver.ResolvePuzzleFailure(activePuzzle).Actions);
                dangerSystem.AddNoise(activePuzzle.failureDanger);
                if (puzzleLogText != null)
                {
                    puzzleLogText.text = "Failed. Try another input.";
                }

                Debug.Log("Puzzle failed: " + activePuzzle.puzzleId);
                return;
            }

            CompleteSolvedPuzzle(activePuzzle);
        }

        private void TriggerEvent(string eventId)
        {
            session.SetFlag(eventId);
            if (eventId == "event_kitchen_first_appearance")
            {
                monsterAI.Enable();
                dangerSystem.AddNoise(25f);
            }

            Debug.Log("Event triggered: " + eventId);
        }

        private void ToggleHiding()
        {
            if (hidingSystem.IsHiding)
            {
                hidingSystem.Exit();
                Debug.Log("Exit hide spot.");
            }
            else
            {
                hidingSystem.Enter(MousePosition());
                dangerSystem.AddNoise(-5f);
                Debug.Log("Entered hide spot.");
            }
        }

        private void OpenHideView(string interactableId)
        {
            var interactable = FindInteractable(interactableId);
            if (interactable == null || string.IsNullOrWhiteSpace(interactable.hideViewResource))
            {
                ToggleHiding();
                return;
            }

            hidingSystem.Enter(MousePosition());
            dangerSystem.AddNoise(-5f);
            if (hideViewImage != null)
            {
                hideViewImage.sprite = ResolveSprite(interactable.hideViewResource);
                hideViewImage.color = Color.white;
                hideViewImage.preserveAspect = true;
            }

            if (hideViewPanel != null)
            {
                StartPanelFade(hideViewPanel, true, ref hideViewFadeRoutine);
            }

            PlaySoundById(interactable.soundId);
            Debug.Log("Hide view opened: " + interactableId);
        }

        private void CloseHideView()
        {
            var wasOpen = hideViewPanel != null && hideViewPanel.activeSelf;
            if (hideViewPanel != null)
            {
                StartPanelFade(hideViewPanel, false, ref hideViewFadeRoutine);
            }

            if (hidingSystem != null && hidingSystem.IsHiding)
            {
                hidingSystem.Exit();
            }

            if (wasOpen)
            {
                Debug.Log("Hide view closed.");
            }
        }

        private void HideHideViewImmediate()
        {
            HidePanelImmediate(hideViewPanel, ref hideViewFadeRoutine);
            if (hidingSystem != null && hidingSystem.IsHiding)
            {
                hidingSystem.Exit();
            }
        }

        private void CompleteStage()
        {
            session.SetFlag(stage.clearFlag);
            var records = saveService.LoadClearRecords();
            records.stage1Clear = true;
            saveService.SaveClearRecords(records);
            monsterAI.Reset();
            Debug.Log("Stage clear: " + stage.stageId);
            RenderRoom();
        }

        private void GameOver()
        {
            Debug.Log("Game Over. Restarting stage.");
            session.Start(stage);
            dangerSystem.Reset();
            hidingSystem.Exit();
            monsterAI.Reset();
            OpenRoom(stage.startRoomId);
        }

        private RoomDefinition CurrentRoom()
        {
            return rooms[session.CurrentRoomId];
        }

        private RoomFaceDefinition CurrentFace(RoomDefinition room)
        {
            if (room.faces == null || room.faces.Length == 0)
            {
                return new RoomFaceDefinition
                {
                    direction = session.CurrentFaceDirection,
                    displayName = session.CurrentFaceDirection.ToString(),
                    backgroundResource = room.backgroundResource,
                    interactables = room.interactables
                };
            }

            foreach (var face in room.faces)
            {
                if (face.direction == session.CurrentFaceDirection)
                {
                    return face;
                }
            }

            return room.faces[0];
        }

        public static RoomFaceDirection NextFaceDirection(RoomDefinition room, RoomFaceDirection currentDirection, int offset)
        {
            if (room == null || room.faces == null || room.faces.Length == 0)
            {
                return (RoomFaceDirection)(((int)currentDirection + offset + 4) % 4);
            }

            var currentIndex = 0;
            for (var index = 0; index < room.faces.Length; index++)
            {
                if (room.faces[index].direction == currentDirection)
                {
                    currentIndex = index;
                    break;
                }
            }

            var nextIndex = (currentIndex + offset) % room.faces.Length;
            if (nextIndex < 0)
            {
                nextIndex += room.faces.Length;
            }

            return room.faces[nextIndex].direction;
        }

        private InteractableDefinition FindInteractable(string interactableId)
        {
            return CurrentRoom().faces
                .SelectMany(face => face.interactables ?? new InteractableDefinition[0])
                .FirstOrDefault(interactable => interactable.interactableId == interactableId);
        }

        private void RotateFace(int offset)
        {
            StartSceneTransition(() =>
            {
                session.SetFaceDirection(NextFaceDirection(CurrentRoom(), session.CurrentFaceDirection, offset));
                RenderRoom();
                Debug.Log("Face changed: " + session.CurrentFaceDirection);
            });
        }

        private string ItemName(string itemId)
        {
            return items.TryGetValue(itemId, out var item) ? item.displayName : itemId;
        }

        private void ExecuteActions(IReadOnlyList<EscapeAction> actions)
        {
            foreach (var action in actions)
            {
                ExecuteAction(action);
            }
        }

        private void ExecuteAction(EscapeAction action)
        {
            switch (action.Type)
            {
                case EscapeActionType.MoveRoom:
                    OpenRoom(action.Value);
                    break;
                case EscapeActionType.RotateFace:
                    RotateFace(action.IntValue);
                    break;
                case EscapeActionType.AcquireItem:
                    AcquireItem(action.Value);
                    break;
                case EscapeActionType.OpenCloseUp:
                    OpenCloseUp(action.Value);
                    break;
                case EscapeActionType.OpenPuzzle:
                    SolvePuzzle(action.Value);
                    break;
                case EscapeActionType.SetFlag:
                    flagService.Set(action.Value);
                    break;
                case EscapeActionType.ClearFlag:
                    flagService.Clear(action.Value);
                    break;
                case EscapeActionType.ShowClue:
                    TriggerEvent(action.Value);
                    break;
                case EscapeActionType.PlaySound:
                    soundManager.Play(action.SoundEntry);
                    break;
                case EscapeActionType.EnterHideSpot:
                    OpenHideView(action.Value);
                    break;
                case EscapeActionType.CompleteStage:
                    CompleteStage();
                    break;
            }
        }

        private void OpenPuzzlePanel(PuzzleDefinition puzzle)
        {
            activePuzzle = puzzle;
            puzzleInputTokens.Clear();
            studySafeSolvedInPanel = false;

            if (puzzleTitleText != null)
            {
                puzzleTitleText.text = puzzle.displayName + " / " + puzzle.puzzleType;
            }

            if (puzzleCloseUpImage != null)
            {
                puzzleCloseUpImage.sprite = ResolveSprite(puzzle.closeUpResource);
                puzzleCloseUpImage.color = Color.white;
                puzzleCloseUpImage.preserveAspect = true;
            }

            if (IsStudySafePuzzle(puzzle))
            {
                OpenStudySafePuzzlePanel();
                return;
            }

            SetLegacyPuzzleControlsVisible(true);
            SetStudySafeControlsVisible(false);
            if (puzzleLogText != null)
            {
                puzzleLogText.text = "Enter tokens, then submit.";
            }

            RefreshPuzzleInputText();
            BuildPuzzleTokens(puzzle);
            if (puzzlePanel != null)
            {
                StartPanelFade(puzzlePanel, true, ref puzzleFadeRoutine);
            }
        }

        private void ClosePuzzlePanel()
        {
            activePuzzle = null;
            puzzleInputTokens.Clear();
            studySafeSolvedInPanel = false;
            if (puzzlePanel != null)
            {
                StartPanelFade(puzzlePanel, false, ref puzzleFadeRoutine);
            }
        }

        private void HidePuzzlePanelImmediate()
        {
            activePuzzle = null;
            puzzleInputTokens.Clear();
            studySafeSolvedInPanel = false;
            HidePanelImmediate(puzzlePanel, ref puzzleFadeRoutine);
        }

        private void OpenStudySafePuzzlePanel()
        {
            EnsureStudySafePuzzleUi();
            SetLegacyPuzzleControlsVisible(false);
            SetStudySafeControlsVisible(true);
            for (var index = 0; index < StudySafeDigitCount; index++)
            {
                studySafeDigits[index] = 0;
                ApplyStudySafeDigitSprite(index);
            }

            if (puzzleBackButton != null)
            {
                StyleBackButton(puzzleBackButton);
            }

            if (puzzlePanel != null)
            {
                StartPanelFade(puzzlePanel, true, ref puzzleFadeRoutine);
            }
        }

        private void AdvanceStudySafeDigit(int index)
        {
            if (!IsStudySafePuzzle(activePuzzle) || index < 0 || index >= StudySafeDigitCount || studySafeSolvedInPanel)
            {
                return;
            }

            studySafeDigits[index] = NextStudySafeDigitValue(studySafeDigits[index]);
            ApplyStudySafeDigitSprite(index);
            if (StudySafeDigitsMatchAnswer(studySafeDigits, activePuzzle.answerTokens))
            {
                SubmitStudySafePuzzle();
            }
        }

        private void SubmitStudySafePuzzle()
        {
            if (!IsStudySafePuzzle(activePuzzle) || studySafeSolvedInPanel)
            {
                return;
            }

            if (!StudySafeDigitsMatchAnswer(studySafeDigits, activePuzzle.answerTokens))
            {
                return;
            }

            studySafeSolvedInPanel = true;
            ApplyStudySafeUnlockAndOpenState(session);
            Debug.Log("Study safe unlocked and opened.");
            RenderRoom();
            activePuzzle = null;
            puzzleInputTokens.Clear();
            StartPanelFade(puzzlePanel, false, ref puzzleFadeRoutine, () =>
            {
                PlaySoundById("sfx_drawer_open");
                OpenCloseUp("study_safe_obj");
            });
        }

        private void CompleteSolvedPuzzle(PuzzleDefinition solvedPuzzle)
        {
            ExecuteActions(actionResolver.ResolvePuzzleSuccess(solvedPuzzle).Actions);
            Debug.Log("Puzzle solved: " + solvedPuzzle.puzzleId);
            var solvedPuzzleId = solvedPuzzle.puzzleId;
            ClosePuzzlePanel();

            if (solvedPuzzleId == "laundry_storage_box")
            {
                monsterAI.Enable();
                dangerSystem.AddNoise(20f);
            }
            else if (solvedPuzzleId == "basement_altar")
            {
                monsterAI.Enable();
                monsterAI.ForceChase();
                PlaySoundById("bgm_final_chase");
            }
            else if (solvedPuzzleId == "front_door_escape")
            {
                ExecuteActions(actionResolver.ResolveCompleteStage(stage).Actions);
            }

            RenderRoom();
        }

        private void ApplyStudySafeDigitSprite(int index)
        {
            if (studySafeDigitImages == null || index < 0 || index >= studySafeDigitImages.Length || studySafeDigitImages[index] == null)
            {
                return;
            }

            var image = studySafeDigitImages[index];
            image.sprite = ResolveSprite(StudySafeDigitResource(studySafeDigits[index]));
            image.color = Color.white;
            image.preserveAspect = true;
        }

        private void SetLegacyPuzzleControlsVisible(bool visible)
        {
            SetGameObjectVisible(puzzleTitleText, visible);
            SetGameObjectVisible(puzzleInputText, visible);
            SetGameObjectVisible(puzzleLogText, visible);
            if (puzzleTokenPanel != null)
            {
                puzzleTokenPanel.gameObject.SetActive(visible);
            }
        }

        private void SetStudySafeControlsVisible(bool visible)
        {
            SetButtonVisible(puzzleBackButton, visible);
            SetImagesVisible(studySafeDigitImages, visible);
            SetButtonsVisible(studySafeDigitButtons, visible);
        }

        private static bool IsStudySafePuzzle(PuzzleDefinition puzzle)
        {
            return puzzle != null && puzzle.puzzleId == StudySafePuzzleId;
        }

        private bool ShouldOpenUnlockedStudySafe(InteractableDefinition interactable)
        {
            return interactable != null
                && interactable.puzzleId == StudySafePuzzleId
                && (session.HasFlag(StudySafeUnlockedFlag) || session.HasSolvedPuzzle(StudySafePuzzleId));
        }

        private void BuildPuzzleTokens(PuzzleDefinition puzzle)
        {
            if (puzzleTokenPanel == null)
            {
                return;
            }

            ClearChildren(puzzleTokenPanel);
            foreach (var token in TokenOptions(puzzle))
            {
                AddPanelButton(puzzleTokenPanel, token, () => AddPuzzleToken(token));
            }

            AddPanelButton(puzzleTokenPanel, "Clear", ClearPuzzleTokens);
            AddPanelButton(puzzleTokenPanel, "Submit", SubmitActivePuzzle);
            AddPanelButton(puzzleTokenPanel, "Close", ClosePuzzlePanel);
        }

        private void AddPuzzleToken(string token)
        {
            if (activePuzzle == null)
            {
                return;
            }

            if (puzzleInputTokens.Count >= activePuzzle.answerTokens.Length)
            {
                puzzleInputTokens.RemoveAt(puzzleInputTokens.Count - 1);
            }

            puzzleInputTokens.Add(token);
            RefreshPuzzleInputText();
        }

        private void ClearPuzzleTokens()
        {
            puzzleInputTokens.Clear();
            RefreshPuzzleInputText();
        }

        private void RefreshPuzzleInputText()
        {
            if (puzzleInputText != null)
            {
                puzzleInputText.text = puzzleInputTokens.Count == 0 ? "Input: -" : "Input: " + string.Join(" ", puzzleInputTokens);
            }
        }

        private static IEnumerable<string> TokenOptions(PuzzleDefinition puzzle)
        {
            switch (puzzle.puzzleType)
            {
                case PuzzleType.NumberLock:
                    return new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                case PuzzleType.ColorSequence:
                    return new[] { "black", "white", "red", "gray", "blue", "green" };
                case PuzzleType.SymbolSequence:
                    return new[] { "heart", "child_hand", "cracked_circle", "keyhole" };
                case PuzzleType.SilentSequence:
                    return new[] { "doll", "train", "block", "bell" };
                default:
                    return puzzle.answerTokens;
            }
        }

        public static int NextStudySafeDigitValue(int currentValue)
        {
            return currentValue < 0 || currentValue >= 9 ? 0 : currentValue + 1;
        }

        public static string StudySafeDigitResource(int digit)
        {
            return "EscapeFromNightmares/Puzzles/study_safe_digit_" + Mathf.Clamp(digit, 0, 9);
        }

        public static bool ShouldPreserveStudySafeDigitLayout(bool hasSerializedReference, bool foundSceneObject)
        {
            return hasSerializedReference || foundSceneObject;
        }

        public static void ApplyStudySafeUnlockAndOpenState(GameSession gameSession)
        {
            if (gameSession == null)
            {
                return;
            }

            gameSession.SetFlag(StudySafeUnlockedFlag);
            gameSession.SetFlag(StudySafeOpenedFlag);
        }

        public static bool StudySafeDigitsMatchAnswer(IReadOnlyList<int> digits, IReadOnlyList<string> answerTokens)
        {
            if (digits == null || answerTokens == null || digits.Count != StudySafeDigitCount || answerTokens.Count != StudySafeDigitCount)
            {
                return false;
            }

            for (var index = 0; index < StudySafeDigitCount; index++)
            {
                if (!int.TryParse(answerTokens[index], out var answerDigit) || digits[index] != answerDigit)
                {
                    return false;
                }
            }

            return true;
        }

        public static string ResolveRoomFaceBackgroundResource(RoomFaceDefinition face, FlagService flags)
        {
            if (face == null)
            {
                return string.Empty;
            }

            if (face.conditionalBackgrounds != null && flags != null)
            {
                foreach (var candidate in face.conditionalBackgrounds)
                {
                    if (candidate != null
                        && !string.IsNullOrWhiteSpace(candidate.backgroundResource)
                        && flags.ConditionsMet(candidate.conditions))
                    {
                        return candidate.backgroundResource;
                    }
                }
            }

            return face.backgroundResource;
        }

        private void PlaySoundById(string soundId)
        {
            if (stage.soundCatalog != null && stage.soundCatalog.TryFind(soundId, out var entry))
            {
                soundManager.Play(entry);
            }
        }

        private Sprite ResolveSprite(string resourcePathOrId)
        {
            var spriteId = SpriteId(resourcePathOrId);
            if (spriteCatalog != null && spriteCatalog.TryFind(spriteId, out var sprite))
            {
                return sprite;
            }

            return resourceManager.LoadSprite(resourcePathOrId);
        }

        private Sprite ResolveItemIcon(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return null;
            }

            if (items.TryGetValue(itemId, out var item))
            {
                return ResolveSprite(item.iconResource);
            }

            return ResolveSprite("EscapeFromNightmares/Items/item_" + itemId);
        }

        private static string CloseUpSpriteResource(InteractableDefinition interactable, CloseUpState state)
        {
            switch (state)
            {
                case CloseUpState.OpenWithItem:
                    return interactable.closeUpOpenWithItemResource;
                case CloseUpState.OpenEmpty:
                    return interactable.closeUpOpenEmptyResource;
                default:
                    return interactable.closeUpClosedResource;
            }
        }

        private static string SpriteId(string resourcePathOrId)
        {
            if (string.IsNullOrWhiteSpace(resourcePathOrId))
            {
                return string.Empty;
            }

            var slashIndex = resourcePathOrId.LastIndexOf('/');
            return slashIndex >= 0 ? resourcePathOrId.Substring(slashIndex + 1) : resourcePathOrId;
        }

        public static bool ShouldRenderRoomHitbox(InteractableDefinition interactable, GameSession currentSession)
        {
            return interactable != null
                && (!interactable.disableRoomHitboxWhenUsed
                    || currentSession == null
                    || !currentSession.HasUsedInteractable(interactable.interactableId));
        }

        private static bool IsRoomTransitionAction(EscapeAction action)
        {
            return action.Type == EscapeActionType.MoveRoom || action.Type == EscapeActionType.RotateFace;
        }

        private void StartSceneTransition(System.Action applyChange)
        {
            if (applyChange == null)
            {
                return;
            }

            if (sceneTransitionInProgress)
            {
                return;
            }

            if (sceneTransitionOverlay == null)
            {
                applyChange();
                return;
            }

            if (sceneTransitionRoutine != null)
            {
                StopCoroutine(sceneTransitionRoutine);
            }

            sceneTransitionRoutine = StartCoroutine(FadeSceneTransition(applyChange));
        }

        private IEnumerator FadeSceneTransition(System.Action applyChange)
        {
            sceneTransitionInProgress = true;
            var overlayObject = sceneTransitionOverlay.gameObject;
            overlayObject.SetActive(true);
            sceneTransitionOverlay.interactable = true;
            sceneTransitionOverlay.blocksRaycasts = true;

            yield return FadeCanvasGroup(sceneTransitionOverlay, 1f, SceneFadeDuration);
            applyChange();
            yield return null;
            yield return FadeCanvasGroup(sceneTransitionOverlay, 0f, SceneFadeDuration);

            sceneTransitionOverlay.interactable = false;
            sceneTransitionOverlay.blocksRaycasts = false;
            overlayObject.SetActive(false);
            sceneTransitionInProgress = false;
            sceneTransitionRoutine = null;
        }

        private static IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
        {
            var start = group.alpha;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            group.alpha = targetAlpha;
        }

        private void StartPanelFade(GameObject panel, bool visible, ref Coroutine routine, System.Action onComplete = null)
        {
            if (panel == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (routine != null)
            {
                StopCoroutine(routine);
            }

            routine = StartCoroutine(FadePanel(panel, visible, onComplete));
        }

        private void HidePanelImmediate(GameObject panel, ref Coroutine routine)
        {
            if (panel == null)
            {
                return;
            }

            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            var group = EnsureCanvasGroup(panel);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            panel.SetActive(false);
        }

        private IEnumerator FadePanel(GameObject panel, bool visible, System.Action onComplete)
        {
            var group = EnsureCanvasGroup(panel);
            if (visible)
            {
                panel.SetActive(true);
                group.alpha = 0f;
            }

            group.interactable = false;
            group.blocksRaycasts = true;

            var end = visible ? 1f : 0f;
            yield return FadeCanvasGroup(group, end, ModalFadeDuration);

            group.interactable = visible;
            group.blocksRaycasts = visible;
            if (!visible)
            {
                panel.SetActive(false);
            }

            onComplete?.Invoke();
        }

        private static CanvasGroup EnsureCanvasGroup(GameObject panel)
        {
            var group = panel.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = panel.AddComponent<CanvasGroup>();
            }

            return group;
        }

        private void EnsureSoundManager()
        {
            soundManager = Object.FindFirstObjectByType<SoundManager>();
            if (soundManager == null)
            {
                soundManager = new GameObject("Sound Manager").AddComponent<SoundManager>();
            }

            soundManager.Initialize(resourceManager);
            soundManager.ApplyVolumes(saveService.LoadSettings());
        }

        private void EnsureUi()
        {
            RectTransform root = null;
            if (roomFaceImage != null && roomFaceImage.canvas != null)
            {
                root = roomFaceImage.canvas.GetComponent<RectTransform>();
            }

            if (root == null)
            {
                var canvas = new GameObject("Escape UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
                root = canvas.GetComponent<RectTransform>();
            }

            if (roomFaceImage == null)
            {
                roomFaceImage = CreateImage("Room Face Image", root, Color.white);
                Stretch(roomFaceImage.rectTransform, new Rect(0.05f, 0.08f, 0.9f, 0.84f));
            }

            if (roomObjectLayer == null)
            {
                roomObjectLayer = CreatePanel("Room Object Layer", root, new Color(0f, 0f, 0f, 0f)).GetComponent<RectTransform>();
                Stretch(roomObjectLayer, new Rect(0.05f, 0.08f, 0.9f, 0.84f));
            }

            if (rotateLeftButton == null)
            {
                rotateLeftButton = CreateTextButton("Rotate Left", root, "<", () => RotateFace(-1));
                Stretch(rotateLeftButton.GetComponent<RectTransform>(), new Rect(0.02f, 0.41f, 0.08f, 0.18f));
            }

            if (rotateRightButton == null)
            {
                rotateRightButton = CreateTextButton("Rotate Right", root, ">", () => RotateFace(1));
                Stretch(rotateRightButton.GetComponent<RectTransform>(), new Rect(0.9f, 0.41f, 0.08f, 0.18f));
            }

            if (inventoryButton == null)
            {
                inventoryButton = CreateTextButton("Inventory Button", root, "Inventory", OpenInventoryWindow);
                Stretch(inventoryButton.GetComponent<RectTransform>(), new Rect(0.83f, 0.86f, 0.14f, 0.1f));
            }

            if (closeUpPanel == null)
            {
                BuildFallbackCloseUp(root);
            }

            if (hideViewPanel == null)
            {
                BuildFallbackHideView(root);
            }

            if (puzzlePanel == null)
            {
                BuildFallbackPuzzlePanel(root);
            }

            if (sceneTransitionOverlay == null)
            {
                sceneTransitionOverlay = CreateSceneTransitionOverlay(root);
            }

            NormalizeModalUi();
        }

        private void NormalizeModalUi()
        {
            if (closeUpPanel != null)
            {
                Stretch(closeUpPanel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
                var group = EnsureCanvasGroup(closeUpPanel);
                group.alpha = closeUpPanel.activeSelf ? 1f : 0f;
            }

            if (closeUpImage != null)
            {
                Stretch(closeUpImage.rectTransform, new Rect(0f, 0f, 1f, 1f));
            }

            StyleBackButton(closeUpCloseButton);

            if (hideViewPanel != null)
            {
                Stretch(hideViewPanel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
                var group = EnsureCanvasGroup(hideViewPanel);
                group.alpha = hideViewPanel.activeSelf ? 1f : 0f;
            }

            if (hideViewImage != null)
            {
                Stretch(hideViewImage.rectTransform, new Rect(0f, 0f, 1f, 1f));
            }

            StyleBackButton(hideExitButton);

            if (puzzlePanel != null)
            {
                var group = EnsureCanvasGroup(puzzlePanel);
                group.alpha = puzzlePanel.activeSelf ? 1f : 0f;
                EnsureStudySafePuzzleUi();
            }

            NormalizeSceneTransitionOverlay();
        }

        private void NormalizeSceneTransitionOverlay()
        {
            if (sceneTransitionOverlay == null)
            {
                return;
            }

            var rectTransform = sceneTransitionOverlay.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Stretch(rectTransform, new Rect(0f, 0f, 1f, 1f));
                rectTransform.SetAsLastSibling();
            }

            sceneTransitionOverlay.alpha = 0f;
            sceneTransitionOverlay.interactable = false;
            sceneTransitionOverlay.blocksRaycasts = false;
            sceneTransitionOverlay.gameObject.SetActive(false);
        }

        private void StyleBackButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = ResolveSprite("EscapeFromNightmares/UI/ui_back_arrow");
                image.color = Color.white;
                image.preserveAspect = true;
            }

            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = "<";
            }

            Stretch(button.GetComponent<RectTransform>(), new Rect(0.02f, 0.86f, 0.1f, 0.11f));
        }

        private void BindUi()
        {
            BindButton(rotateLeftButton, () => RotateFace(-1));
            BindButton(rotateRightButton, () => RotateFace(1));
            BindButton(inventoryButton, OpenInventoryWindow);
            BindButton(closeUpActionButton, OpenActiveDrawer);
            BindButton(closeUpItemButton, AcquireCloseUpItem);
            BindButton(closeUpCloseButton, CloseCloseUpPanel);
            BindButton(hideExitButton, CloseHideView);
            BindButton(puzzleBackButton, ClosePuzzlePanel);
            BindStudySafeDigitButtons();
        }

        private void BuildFallbackCloseUp(RectTransform root)
        {
            closeUpPanel = CreatePanel("Drawer Close Up Panel", root, new Color(0.01f, 0.01f, 0.012f, 0.96f));
            Stretch(closeUpPanel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
            closeUpImage = CreateImage("Drawer Close Up Image", closeUpPanel.transform, Color.white);
            Stretch(closeUpImage.rectTransform, new Rect(0f, 0f, 1f, 1f));
            closeUpActionButton = CreateTextButton("Open Drawer Button", closeUpPanel.transform, "Open", OpenActiveDrawer);
            Stretch(closeUpActionButton.GetComponent<RectTransform>(), new Rect(0.42f, 0.04f, 0.16f, 0.07f));
            closeUpItemButton = CreateTextButton("Drawer Item Button", closeUpPanel.transform, "", AcquireCloseUpItem);
            closeUpItemHitbox = closeUpItemButton.GetComponent<RectTransform>();
            closeUpCloseButton = CreateTextButton("Close Drawer Button", closeUpPanel.transform, "<", CloseCloseUpPanel);
            Stretch(closeUpCloseButton.GetComponent<RectTransform>(), new Rect(0.02f, 0.86f, 0.1f, 0.11f));
            closeUpPanel.SetActive(false);
        }

        private void BuildFallbackHideView(RectTransform root)
        {
            hideViewPanel = CreatePanel("Hide View Panel", root, new Color(0.005f, 0.005f, 0.006f, 0.98f));
            Stretch(hideViewPanel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
            hideViewImage = CreateImage("Hide View Image", hideViewPanel.transform, Color.white);
            Stretch(hideViewImage.rectTransform, new Rect(0f, 0f, 1f, 1f));
            hideExitButton = CreateTextButton("Hide Exit Button", hideViewPanel.transform, "<", CloseHideView);
            Stretch(hideExitButton.GetComponent<RectTransform>(), new Rect(0.02f, 0.86f, 0.1f, 0.11f));
            hideViewPanel.SetActive(false);
        }

        private void BuildFallbackPuzzlePanel(RectTransform root)
        {
            puzzlePanel = CreatePanel("Puzzle Close Up Panel", root, new Color(0.012f, 0.01f, 0.012f, 0.96f));
            Stretch(puzzlePanel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
            puzzleTitleText = CreateText("Puzzle Title", puzzlePanel.transform, 26, TextAnchor.MiddleCenter);
            Stretch(puzzleTitleText.rectTransform, new Rect(0.04f, 0.88f, 0.92f, 0.1f));
            puzzleCloseUpImage = CreateImage("Puzzle Close Up Image", puzzlePanel.transform, Color.white);
            puzzleCloseUpImage.preserveAspect = true;
            Stretch(puzzleCloseUpImage.rectTransform, new Rect(0f, 0f, 1f, 1f));
            puzzleInputText = CreateText("Puzzle Input", puzzlePanel.transform, 18, TextAnchor.MiddleLeft);
            Stretch(puzzleInputText.rectTransform, new Rect(0.66f, 0.74f, 0.28f, 0.1f));
            puzzleLogText = CreateText("Puzzle Log", puzzlePanel.transform, 16, TextAnchor.UpperLeft);
            Stretch(puzzleLogText.rectTransform, new Rect(0.66f, 0.62f, 0.28f, 0.1f));
            puzzleTokenPanel = CreatePanel("Puzzle Token Panel", puzzlePanel.transform, new Color(0f, 0f, 0f, 0f)).GetComponent<RectTransform>();
            Stretch(puzzleTokenPanel, new Rect(0.66f, 0.12f, 0.28f, 0.48f));
            var layout = puzzleTokenPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            EnsureStudySafePuzzleUi();
            puzzlePanel.SetActive(false);
        }

        private void EnsureStudySafePuzzleUi()
        {
            if (puzzlePanel == null)
            {
                return;
            }

            studySafeDigitImages = NormalizeArray(studySafeDigitImages, StudySafeDigitCount);
            studySafeDigitButtons = NormalizeArray(studySafeDigitButtons, StudySafeDigitCount);
            var parent = puzzlePanel.transform;

            if (puzzleBackButton == null)
            {
                puzzleBackButton = FindChildComponent<Button>(parent, "PuzzleBackButton");
                if (puzzleBackButton == null)
                {
                    puzzleBackButton = CreateImageButton("PuzzleBackButton", parent, ResolveSprite("EscapeFromNightmares/UI/ui_back_arrow"), ClosePuzzlePanel);
                }
            }

            StyleBackButton(puzzleBackButton);
            BindButton(puzzleBackButton, ClosePuzzlePanel);

            for (var index = 0; index < StudySafeDigitCount; index++)
            {
                var digitImageName = "StudySafeDigit" + index;
                var hasSerializedImage = studySafeDigitImages[index] != null;
                var foundSceneImage = false;
                if (studySafeDigitImages[index] == null)
                {
                    studySafeDigitImages[index] = FindChildComponent<Image>(parent, digitImageName);
                    foundSceneImage = studySafeDigitImages[index] != null;
                    if (studySafeDigitImages[index] == null)
                    {
                        studySafeDigitImages[index] = CreateImage(digitImageName, parent, Color.white);
                    }
                }

                studySafeDigitImages[index].preserveAspect = true;
                studySafeDigitImages[index].raycastTarget = false;
                if (!ShouldPreserveStudySafeDigitLayout(hasSerializedImage, foundSceneImage))
                {
                    Stretch(studySafeDigitImages[index].rectTransform, StudySafeDigitRects[index]);
                }

                var buttonName = "StudySafeDigitButton" + index;
                var hasSerializedButton = studySafeDigitButtons[index] != null;
                var foundSceneButton = false;
                if (studySafeDigitButtons[index] == null)
                {
                    studySafeDigitButtons[index] = FindChildComponent<Button>(parent, buttonName);
                    foundSceneButton = studySafeDigitButtons[index] != null;
                    if (studySafeDigitButtons[index] == null)
                    {
                        studySafeDigitButtons[index] = CreateTransparentButton(buttonName, parent, () => { });
                    }
                }

                if (!ShouldPreserveStudySafeDigitLayout(hasSerializedButton, foundSceneButton))
                {
                    Stretch(studySafeDigitButtons[index].GetComponent<RectTransform>(), StudySafeDigitRects[index]);
                }
            }

            BindStudySafeDigitButtons();
            SetStudySafeControlsVisible(false);
        }

        private void BindStudySafeDigitButtons()
        {
            if (studySafeDigitButtons == null)
            {
                return;
            }

            for (var index = 0; index < studySafeDigitButtons.Length; index++)
            {
                var capturedIndex = index;
                BindButton(studySafeDigitButtons[index], () => AdvanceStudySafeDigit(capturedIndex));
            }
        }

        private static CanvasGroup CreateSceneTransitionOverlay(RectTransform root)
        {
            var overlay = CreatePanel("SceneTransitionOverlay", root, Color.black);
            var group = overlay.AddComponent<CanvasGroup>();
            Stretch(overlay.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            overlay.SetActive(false);
            return group;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            var image = imageObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private Button CreateImageButton(string name, Transform parent, Sprite sprite, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.GetComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;
            var button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(action);
            return button;
        }

        private Button CreateTransparentButton(string name, Transform parent, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.001f);
            image.raycastTarget = true;
            var button = buttonObject.GetComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(action);
            return button;
        }

        private static Button CreateTextButton(string name, Transform parent, string label, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = new Color(0.02f, 0.018f, 0.015f, 0.96f);
            var button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(action);
            var text = CreateText("Label", buttonObject.transform, 24, TextAnchor.MiddleCenter);
            text.text = label;
            text.color = new Color(0.96f, 0.88f, 0.66f, 1f);
            Stretch(text.rectTransform, new Rect(0f, 0f, 1f, 1f));
            return button;
        }

        private static Text CreateText(string name, Transform parent, int size, TextAnchor anchor)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.color = new Color(0.88f, 0.86f, 0.82f, 1f);
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static void AddPanelButton(Transform parent, string label, UnityEngine.Events.UnityAction action)
        {
            var button = CreateTextButton(label, parent, label, action);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 34f);
        }

        private static void BindButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        private static void SetButtonVisible(Button button, bool visible)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
            }
        }

        private static void SetButtonsVisible(Button[] buttons, bool visible)
        {
            if (buttons == null)
            {
                return;
            }

            foreach (var button in buttons)
            {
                SetButtonVisible(button, visible);
            }
        }

        private static void SetImagesVisible(Image[] images, bool visible)
        {
            if (images == null)
            {
                return;
            }

            foreach (var image in images)
            {
                SetGameObjectVisible(image, visible);
            }
        }

        private static void SetGameObjectVisible(Component component, bool visible)
        {
            if (component != null)
            {
                component.gameObject.SetActive(visible);
            }
        }

        private static T[] NormalizeArray<T>(T[] values, int length)
        {
            if (values != null && values.Length == length)
            {
                return values;
            }

            var normalized = new T[length];
            if (values != null)
            {
                for (var index = 0; index < Mathf.Min(values.Length, length); index++)
                {
                    normalized[index] = values[index];
                }
            }

            return normalized;
        }

        private static T FindChildComponent<T>(Transform parent, string childName) where T : Component
        {
            if (parent == null)
            {
                return null;
            }

            var child = parent.Find(childName);
            return child == null ? null : child.GetComponent<T>();
        }

        private static void ClearChildren(Transform parent)
        {
            if (parent == null)
            {
                return;
            }

            for (var index = parent.childCount - 1; index >= 0; index--)
            {
                Destroy(parent.GetChild(index).gameObject);
            }
        }

        private static void Stretch(RectTransform rectTransform, Rect normalizedRect)
        {
            rectTransform.anchorMin = new Vector2(normalizedRect.xMin, normalizedRect.yMin);
            rectTransform.anchorMax = new Vector2(normalizedRect.xMax, normalizedRect.yMax);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        private static Vector2 MousePosition()
        {
            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        }
    }
}
