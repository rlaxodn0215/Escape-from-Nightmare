using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.Services;
using EscapeFromNightmares.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// 메인 게임 씬의 화면 구성, 입력 바인딩, 방 이동, 퍼즐, 인벤토리, 몬스터 상태를 총괄합니다.
    /// </summary>
    public sealed class GameDirector : MonoBehaviour
    {
        [Header("Catalogs")]
        [SerializeField] private StageDefinition stageDefinition;
        [SerializeField] private RoomSpriteCatalog spriteCatalog;
        [SerializeField] private MonsterPlacementCatalog monsterPlacementCatalog;

        [Header("Room View")]
        [SerializeField] private Image roomFaceImage;
        [SerializeField] private RectTransform roomObjectLayer;
        [SerializeField] private Image monsterImage;
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

        [Header("Stage Clear")]
        [SerializeField] private GameObject stageClearPanel;
        [SerializeField] private Image stageClearBackgroundImage;
        [SerializeField] private Button stageClearTitleButton;

        [Header("Runtime QA")]
        [SerializeField] private MonsterRuntimeQaPanel monsterQaPanel;

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
        private StageLookup stageLookup;
        private EscapeActionExecutor actionExecutor;

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
        private bool monsterQaPanelVisible;
        /// <summary>스테이지 클리어 화면 배경으로 사용하는 Resources 경로입니다.</summary>
        public const string StageClearBackgroundResource = "EscapeFromNightmares/Endings/stage1_clear_background";
        /// <summary>몬스터 그림자 스프라이트로 사용하는 Resources 경로입니다.</summary>
        public const string MonsterShadowResource = "EscapeFromNightmares/Monster/monster_shadow";
        /// <summary>부엌 첫 등장 연출을 한 번만 처리하기 위한 이벤트 플래그입니다.</summary>
        public const string KitchenFirstAppearanceEventFlag = "event_kitchen_first_appearance";
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

        /// <summary>
        /// 에디터 빌더가 만든 씬 UI와 카탈로그 참조를 런타임 컨트롤러에 주입합니다.
        /// </summary>
        public void SetSceneReferences(
            RoomSpriteCatalog roomSprites,
            MonsterPlacementCatalog monsterPlacements,
            Image roomImage,
            RectTransform objectLayer,
            Image monsterObjectImage,
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
            CanvasGroup transitionOverlay = null,
            GameObject clearPanel = null,
            Image clearBackgroundImage = null,
            Button clearTitleButton = null,
            MonsterRuntimeQaPanel monsterRuntimeQaPanel = null,
            StageDefinition mainStage = null)
        {
            stageDefinition = mainStage;
            spriteCatalog = roomSprites;
            monsterPlacementCatalog = monsterPlacements;
            roomFaceImage = roomImage;
            roomObjectLayer = objectLayer;
            monsterImage = monsterObjectImage;
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
            stageClearPanel = clearPanel;
            stageClearBackgroundImage = clearBackgroundImage;
            stageClearTitleButton = clearTitleButton;
            monsterQaPanel = monsterRuntimeQaPanel;
        }

        private void Awake()
        {
            // 현재 프로토타입은 ScriptableObject 에셋 대신 코드 팩토리에서 Stage 1 데이터를 조립한다.
            stage = StageRepository.LoadStage1(stageDefinition);
            if (stage == null)
            {
                enabled = false;
                return;
            }

            stageLookup = new StageLookup(stage);
            rooms.Clear();
            items.Clear();
            puzzles.Clear();
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

            if (monsterPlacementCatalog == null)
            {
                monsterPlacementCatalog = MonsterPlacementCatalog.CreateDefault(stage);
            }

            // 서비스 계층은 순수 상태/규칙을 담당하고, GameDirector는 Unity UI와 실행 순서를 연결한다.
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
            actionExecutor = new EscapeActionExecutor(
                OpenRoom,
                RotateFace,
                itemId => AcquireItem(itemId),
                OpenCloseUp,
                SolvePuzzle,
                flagService,
                TriggerEvent,
                soundManager,
                OpenHideView,
                session,
                CompleteStage);
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
            RenderMonster();
            HandleMonsterQaInput();

            if (dangerSystem.CaptureGauge >= 100f)
            {
                GameOver();
            }
        }

        private void StartGame()
        {
            // 새 게임 시작 시 화면 모달, 위험도, 몬스터, 인벤토리 선택 상태를 모두 초기 상태로 맞춘다.
            session.Start(stage);
            dangerSystem.Reset();
            hidingSystem.Exit();
            monsterAI.Reset();
            HideCloseUpPanelImmediate();
            HideHideViewImmediate();
            HidePuzzlePanelImmediate();
            HideStageClearPanelImmediate();
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
            if (stageLookup == null || !stageLookup.TryGetRoom(roomId, out _))
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
            // 현재 방/방향 데이터가 화면의 배경 이미지와 클릭 가능한 상호작용 레이어의 원천이다.
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
            RenderMonster();
        }

        private void RenderRoomObjects(InteractableDefinition[] interactables)
        {
            if (roomObjectLayer == null)
            {
                return;
            }

            ClearChildrenExcept(roomObjectLayer, monsterImage == null ? null : monsterImage.transform);
            // 조건을 만족하는 상호작용만 버튼으로 만들고, 이미지가 없는 항목은 투명 히트박스로 처리한다.
            foreach (var interactable in interactables)
            {
                if (!ShouldRenderRoomHitbox(interactable, session, flagService))
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

        private void RenderMonster()
        {
            if (session == null || string.IsNullOrWhiteSpace(session.CurrentRoomId))
            {
                HideMonsterImage();
                return;
            }

            if (!TryResolveMonsterPlacement(monsterPlacementCatalog, session.CurrentRoomId, session.CurrentFaceDirection, monsterAI.State, out var placementRect))
            {
                HideMonsterImage();
                return;
            }

            // 몬스터는 방 오브젝트 레이어의 가장 위에 배치해 상호작용 이미지와 겹쳐도 항상 보이게 한다.
            var image = EnsureMonsterImage();
            if (image == null)
            {
                return;
            }

            image.sprite = ResolveSprite(MonsterShadowResource);
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            ApplyMonsterPlacement(image.rectTransform, placementRect);
            image.transform.SetAsLastSibling();
            image.gameObject.SetActive(true);
        }

        private Image EnsureMonsterImage()
        {
            if (monsterImage != null)
            {
                return monsterImage;
            }

            if (roomObjectLayer == null)
            {
                return null;
            }

            monsterImage = CreateImage("MonsterImage", roomObjectLayer, Color.white);
            monsterImage.raycastTarget = false;
            monsterImage.preserveAspect = true;
            monsterImage.gameObject.SetActive(false);
            return monsterImage;
        }

        private void HideMonsterImage()
        {
            if (monsterImage != null)
            {
                monsterImage.gameObject.SetActive(false);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void HandleMonsterQaInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.f9Key.wasPressedThisFrame)
            {
                monsterQaPanelVisible = !monsterQaPanelVisible;
                EnsureMonsterQaPanel();
                monsterQaPanel?.SetVisible(monsterQaPanelVisible);
            }

            if (keyboard.f10Key.wasPressedThisFrame)
            {
                monsterAI.ForceDebugState(NextMonsterQaState(monsterAI.State));
                RenderMonster();
            }

            if (keyboard.f11Key.wasPressedThisFrame)
            {
                if (monsterAI.State == MonsterState.Disabled)
                {
                    monsterAI.ClearDebugState();
                    monsterAI.Enable();
                }
                else
                {
                    monsterAI.Reset();
                }

                RenderMonster();
            }

            RefreshMonsterQaPanel();
        }

        private MonsterRuntimeQaPanel EnsureMonsterQaPanel()
        {
            if (monsterQaPanel != null)
            {
                return monsterQaPanel;
            }

            var root = ResolveUiRoot();
            if (root == null)
            {
                return null;
            }

            monsterQaPanel = root.GetComponentInChildren<MonsterRuntimeQaPanel>(true);
            if (monsterQaPanel != null)
            {
                monsterQaPanel.SetVisible(monsterQaPanelVisible);
                return monsterQaPanel;
            }

            monsterQaPanel = MonsterRuntimeQaPanel.Create(root);
            monsterQaPanel.SetVisible(monsterQaPanelVisible);
            return monsterQaPanel;
        }

        private RectTransform ResolveUiRoot()
        {
            if (roomFaceImage != null && roomFaceImage.canvas != null)
            {
                return roomFaceImage.canvas.GetComponent<RectTransform>();
            }

            if (roomObjectLayer != null)
            {
                return roomObjectLayer.GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
            }

            return null;
        }

        private void RefreshMonsterQaPanel()
        {
            if (!monsterQaPanelVisible)
            {
                return;
            }

            var panel = EnsureMonsterQaPanel();
            if (panel == null || session == null)
            {
                return;
            }

            var snapshot = CreateMonsterQaSnapshot(
                monsterPlacementCatalog,
                session.CurrentRoomId,
                session.CurrentFaceDirection,
                monsterAI.State,
                monsterImage != null && monsterImage.gameObject.activeSelf);
            panel.Refresh(snapshot);
        }

        private void HandleInteractable(InteractableDefinition interactable)
        {
            // 서재 금고는 퍼즐 성공 후 보상 획득 전후 이미지가 달라 별도 상태 전환을 먼저 처리한다.
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
                // 최종 열쇠 획득은 즉시 추격 상태와 배경 전환 플래그를 켜는 진행 분기점이다.
                ApplyFinalChaseStartState(session, monsterAI, flagService);
                PlaySoundById("bgm_final_chase");
                Debug.Log("Front door key acquired; final chase started.");
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
            return CloseUpPresenter.ResolveState(interactable, session, StudySafePuzzleId, StudySafeOpenedFlag);
        }

        private void ApplyCloseUpState(CloseUpState state)
        {
            CloseUpPresenter.ApplyState(
                activeCloseUpInteractable,
                state,
                closeUpImage,
                closeUpActionButton,
                closeUpItemButton,
                closeUpItemHitbox,
                ResolveSprite,
                SetButtonVisible,
                Stretch);
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
            if (stageLookup == null || !stageLookup.TryGetPuzzle(puzzleId, out var puzzle))
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
            if (!HideViewPresenter.CanOpen(interactable))
            {
                ToggleHiding();
                return;
            }

            hidingSystem.Enter(MousePosition());
            dangerSystem.AddNoise(-5f);
            HideViewPresenter.Apply(interactable, hideViewImage, ResolveSprite);

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
            ApplyStageClearState(session, stage, saveService, monsterAI);
            soundManager?.StopBgm();
            ShowStageClearPanel();
            Debug.Log("Stage clear: " + stage.stageId);
        }

        private void GameOver()
        {
            Debug.Log("Game Over. Restarting stage.");
            session.Start(stage);
            dangerSystem.Reset();
            hidingSystem.Exit();
            monsterAI.Reset();
            HideStageClearPanelImmediate();
            OpenRoom(stage.startRoomId);
        }

        private RoomDefinition CurrentRoom()
        {
            return stageLookup.RequireRoom(session.CurrentRoomId);
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

        /// <summary>
        /// 현재 방이 가진 방향 목록 안에서 offset만큼 회전한 다음 방향을 계산합니다.
        /// </summary>
        public static RoomFaceDirection NextFaceDirection(RoomDefinition room, RoomFaceDirection currentDirection, int offset)
        {
            return RoomPresenter.NextFaceDirection(room, currentDirection, offset);
        }

        private InteractableDefinition FindInteractable(string interactableId)
        {
            return stageLookup.FindInteractable(interactableId);
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
            return stageLookup != null && stageLookup.TryGetItem(itemId, out var item) ? item.displayName : itemId;
        }

        private void ExecuteActions(IReadOnlyList<EscapeAction> actions)
        {
            // Resolver는 의도만 반환하므로 실제 세션 변경, UI 열기, 사운드 재생은 이곳에서 순서대로 실행한다.
            foreach (var action in actions)
            {
                ExecuteAction(action);
            }
        }

        private void ExecuteAction(EscapeAction action)
        {
            actionExecutor.Execute(action);
        }

        private void OpenPuzzlePanel(PuzzleDefinition puzzle)
        {
            activePuzzle = puzzle;
            puzzleInputTokens.Clear();
            studySafeSolvedInPanel = false;

            PuzzlePresenter.ApplyHeader(puzzle, puzzleTitleText, puzzleInputText, puzzleLogText, puzzleCloseUpImage, ResolveSprite);

            if (IsStudySafePuzzle(puzzle))
            {
                // 금고 퍼즐은 숫자 이미지와 버튼을 실제 금고 위치에 겹쳐야 하므로 전용 UI 상태를 준비한다.
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
                ApplyLaundryStorageBoxMonsterStartState(session, monsterAI, flagService, dangerSystem);
            }
            else if (solvedPuzzleId == "basement_altar")
            {
                Debug.Log("Basement altar solved; front door key pickup spawned.");
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
            return PuzzlePresenter.IsStudySafePuzzle(puzzle, StudySafePuzzleId);
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
                puzzleInputText.text = puzzleInputTokens.Count == 0 ? "Input: -" : "Input: " + PuzzlePresenter.FormatInput(puzzleInputTokens);
            }
        }

        private static IEnumerable<string> TokenOptions(PuzzleDefinition puzzle)
        {
            return PuzzlePresenter.TokenOptions(puzzle);
        }

        /// <summary>서재 금고 숫자를 0~9 범위에서 다음 값으로 순환시킵니다.</summary>
        public static int NextStudySafeDigitValue(int currentValue)
        {
            return StudySafePuzzleRules.NextDigitValue(currentValue);
        }

        /// <summary>서재 금고 숫자 이미지의 Resources 경로를 반환합니다.</summary>
        public static string StudySafeDigitResource(int digit)
        {
            return StudySafePuzzleRules.DigitResource(Mathf.Clamp(digit, 0, 9));
        }

        /// <summary>
        /// 씬에 이미 배치된 금고 숫자 UI의 레이아웃을 코드가 덮어쓰지 않아야 하는지 판단합니다.
        /// </summary>
        public static bool ShouldPreserveStudySafeDigitLayout(bool hasSerializedReference, bool foundSceneObject)
        {
            return StudySafePuzzleRules.ShouldPreserveDigitLayout(hasSerializedReference, foundSceneObject);
        }

        /// <summary>
        /// 서재 금고 퍼즐 성공 후 금고가 열려 있고 보상 회수 단계로 들어갔음을 세션에 기록합니다.
        /// </summary>
        public static void ApplyStudySafeUnlockAndOpenState(GameSession gameSession)
        {
            if (gameSession == null)
            {
                return;
            }

            ProgressionFlow.ApplyStudySafeUnlockAndOpenState(gameSession);
        }

        /// <summary>
        /// 현관 열쇠 획득 후 최종 추격 상태와 관련 플래그를 적용합니다.
        /// </summary>
        public static void ApplyFinalChaseStartState(GameSession gameSession, MonsterAIController monsterAIController, FlagService flags)
        {
            if (gameSession == null || monsterAIController == null || flags == null)
            {
                return;
            }

            ProgressionFlow.ApplyFinalChaseStartState(gameSession, monsterAIController, flags);
        }

        /// <summary>
        /// 세탁실 보관함 퍼즐 성공 후 몬스터 시스템을 켜고 첫 등장 압박을 부여합니다.
        /// </summary>
        public static void ApplyLaundryStorageBoxMonsterStartState(GameSession gameSession, MonsterAIController monsterAIController, FlagService flags, DangerSystem danger)
        {
            if (gameSession == null || monsterAIController == null || flags == null || danger == null)
            {
                return;
            }

            ProgressionFlow.ApplyLaundryStorageBoxMonsterStartState(gameSession, monsterAIController, flags, danger);
        }

        /// <summary>
        /// 스테이지 클리어 플래그, 클리어 기록 저장, 몬스터 정지를 한 번에 적용합니다.
        /// </summary>
        public static void ApplyStageClearState(GameSession gameSession, StageDefinition stageDefinition, SettingsSaveService settingsSaveService, MonsterAIController monsterAIController)
        {
            if (gameSession == null || stageDefinition == null || settingsSaveService == null)
            {
                return;
            }

            ProgressionFlow.ApplyStageClearState(gameSession, stageDefinition, settingsSaveService, monsterAIController);
        }

        /// <summary>
        /// 현재 몬스터 상태에서 화면에 몬스터 그림자를 표시해야 하는지 판단합니다.
        /// </summary>
        public static bool ShouldShowMonster(MonsterState state)
        {
            return MonsterPresenter.ShouldShowMonster(state);
        }

        /// <summary>
        /// QA 단축키로 순환할 다음 몬스터 상태를 반환합니다.
        /// </summary>
        public static MonsterState NextMonsterQaState(MonsterState state)
        {
            return MonsterPresenter.NextQaState(state);
        }

        /// <summary>
        /// 몬스터 상태와 배치 카탈로그를 확인해 현재 방/방향에서 사용할 표시 영역을 구합니다.
        /// </summary>
        public static bool TryResolveMonsterPlacement(MonsterPlacementCatalog catalog, string roomId, RoomFaceDirection faceDirection, MonsterState state, out Rect normalizedRect)
        {
            return MonsterPresenter.TryResolvePlacement(catalog, roomId, faceDirection, state, out normalizedRect);
        }

        /// <summary>
        /// 런타임 QA 패널에 표시할 몬스터 배치/상태 진단 정보를 생성합니다.
        /// </summary>
        public static MonsterQaSnapshot CreateMonsterQaSnapshot(MonsterPlacementCatalog catalog, string roomId, RoomFaceDirection faceDirection, MonsterState state, bool monsterImageActive)
        {
            return MonsterPresenter.CreateQaSnapshot(catalog, roomId, faceDirection, state, monsterImageActive);
        }

        /// <summary>
        /// 정규화된 Rect를 UI 앵커 값으로 변환해 몬스터 이미지 위치를 적용합니다.
        /// </summary>
        public static void ApplyMonsterPlacement(RectTransform rectTransform, Rect normalizedRect)
        {
            MonsterPresenter.ApplyPlacement(rectTransform, normalizedRect);
        }

        /// <summary>
        /// 서재 금고 UI 숫자 배열이 퍼즐 정답 토큰과 일치하는지 확인합니다.
        /// </summary>
        public static bool StudySafeDigitsMatchAnswer(IReadOnlyList<int> digits, IReadOnlyList<string> answerTokens)
        {
            return StudySafePuzzleRules.DigitsMatchAnswer(digits, answerTokens);
        }

        /// <summary>
        /// 몬스터 QA 패널에서 배치가 보이지 않는 이유를 분류합니다.
        /// </summary>
        public enum MonsterQaStatus
        {
            StateHidden,
            CatalogMissing,
            PlacementMissing,
            PlacementDisabled,
            PlacementEmpty,
            PlacementReady
        }

        /// <summary>
        /// 런타임 QA 패널에 표시할 현재 몬스터 배치 진단값입니다.
        /// </summary>
        public struct MonsterQaSnapshot
        {
            public string RoomId;
            public RoomFaceDirection FaceDirection;
            public MonsterState State;
            public bool HasPlacement;
            public bool PlacementEnabled;
            public Rect NormalizedRect;
            public bool ShouldShowMonster;
            public bool MonsterImageActive;
            public MonsterQaStatus Status;
            public string StatusText;
        }

        /// <summary>
        /// 조건부 배경 중 현재 플래그 조건을 만족하는 리소스를 우선 반환합니다.
        /// </summary>
        public static string ResolveRoomFaceBackgroundResource(RoomFaceDefinition face, FlagService flags)
        {
            return RoomPresenter.ResolveRoomFaceBackgroundResource(face, flags);
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

            if (stageLookup != null && stageLookup.TryGetItem(itemId, out var item))
            {
                return ResolveSprite(item.iconResource);
            }

            return ResolveSprite("EscapeFromNightmares/Items/item_" + itemId);
        }

        private static string CloseUpSpriteResource(InteractableDefinition interactable, CloseUpState state)
        {
            return CloseUpPresenter.ResolveSpriteResource(interactable, state);
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

        /// <summary>
        /// 현재 세션 기준으로 상호작용 히트박스를 표시할지 판단합니다.
        /// </summary>
        public static bool ShouldRenderRoomHitbox(InteractableDefinition interactable, GameSession currentSession)
        {
            return RoomPresenter.ShouldRenderRoomHitbox(interactable, currentSession);
        }

        /// <summary>
        /// 조건 판정 서비스까지 주입해 상호작용 히트박스 표시 여부를 판단합니다.
        /// </summary>
        public static bool ShouldRenderRoomHitbox(InteractableDefinition interactable, GameSession currentSession, FlagService flags)
        {
            return RoomPresenter.ShouldRenderRoomHitbox(interactable, currentSession, flags);
        }

        private static bool IsRoomTransitionAction(EscapeAction action)
        {
            return action.Type == EscapeActionType.MoveRoom || action.Type == EscapeActionType.RotateFace;
        }

        private void ShowStageClearPanel()
        {
            if (stageClearPanel == null)
            {
                return;
            }

            if (stageClearBackgroundImage != null)
            {
                stageClearBackgroundImage.sprite = ResolveSprite(StageClearBackgroundResource);
                stageClearBackgroundImage.color = Color.white;
                stageClearBackgroundImage.preserveAspect = true;
            }

            stageClearPanel.SetActive(true);
            var group = EnsureCanvasGroup(stageClearPanel);
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        private void HideStageClearPanelImmediate()
        {
            if (stageClearPanel == null)
            {
                return;
            }

            var group = EnsureCanvasGroup(stageClearPanel);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            stageClearPanel.SetActive(false);
        }

        private static void ReturnToTitleScene()
        {
            SceneManager.LoadScene("TitleScene");
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
            return PanelFader.FadeCanvasGroup(group, targetAlpha, duration);
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
            return PanelFader.FadePanel(panel, visible, ModalFadeDuration, onComplete);
        }

        private static CanvasGroup EnsureCanvasGroup(GameObject panel)
        {
            return PanelFader.EnsureCanvasGroup(panel);
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

            if (stageClearPanel == null)
            {
                var stageClear = CreateStageClearPanel(root, null);
                stageClearPanel = stageClear.panel;
                stageClearBackgroundImage = stageClear.backgroundImage;
                stageClearTitleButton = stageClear.titleButton;
            }

            if (monsterQaPanel == null)
            {
                monsterQaPanel = root.GetComponentInChildren<MonsterRuntimeQaPanel>(true);
                if (monsterQaPanel == null)
                {
                    monsterQaPanel = MonsterRuntimeQaPanel.Create(root);
                }
            }

            monsterQaPanel.SetVisible(false);
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

            if (stageClearPanel != null)
            {
                Stretch(stageClearPanel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));
                var group = EnsureCanvasGroup(stageClearPanel);
                group.alpha = stageClearPanel.activeSelf ? 1f : 0f;
                group.interactable = stageClearPanel.activeSelf;
                group.blocksRaycasts = stageClearPanel.activeSelf;
            }

            if (stageClearBackgroundImage != null)
            {
                Stretch(stageClearBackgroundImage.rectTransform, new Rect(0f, 0f, 1f, 1f));
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
            BindButton(stageClearTitleButton, ReturnToTitleScene);
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

        /// <summary>
        /// 씬에 클리어 패널이 없을 때 사용할 기본 Stage Clear UI를 런타임으로 생성합니다.
        /// </summary>
        public static StageClearUi CreateStageClearPanel(RectTransform root, Sprite backgroundSprite)
        {
            return StageClearPresenter.CreatePanel(root, backgroundSprite, ReturnToTitleScene);
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

        /// <summary>
        /// 런타임으로 만든 Stage Clear 패널과 핵심 UI 참조를 묶습니다.
        /// </summary>
        public readonly struct StageClearUi
        {
            /// <summary>Stage Clear UI 참조 묶음을 생성합니다.</summary>
            public StageClearUi(GameObject panel, Image backgroundImage, Button titleButton)
            {
                this.panel = panel;
                this.backgroundImage = backgroundImage;
                this.titleButton = titleButton;
            }

            public readonly GameObject panel;
            public readonly Image backgroundImage;
            public readonly Button titleButton;
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

        private static void ClearChildrenExcept(Transform parent, Transform preservedChild)
        {
            if (parent == null)
            {
                return;
            }

            for (var index = parent.childCount - 1; index >= 0; index--)
            {
                var child = parent.GetChild(index);
                if (child == preservedChild)
                {
                    continue;
                }

                Destroy(child.gameObject);
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

namespace EscapeFromNightmares.UI
{
    /// <summary>
    /// 개발 빌드와 에디터에서 몬스터 상태/배치 문제를 빠르게 확인하기 위한 런타임 QA 패널입니다.
    /// </summary>
    public sealed class MonsterRuntimeQaPanel : MonoBehaviour
    {
        [SerializeField] private Text infoText;
        [SerializeField] private CanvasGroup canvasGroup;

        /// <summary>QA 정보를 표시하는 텍스트 컴포넌트입니다.</summary>
        public Text InfoText => infoText;
        /// <summary>패널 표시 상태를 제어하는 CanvasGroup입니다.</summary>
        public CanvasGroup CanvasGroup => canvasGroup;

        /// <summary>
        /// 패널 표시 여부를 바꾸되 게임 입력을 가로막지 않도록 raycast 차단은 끕니다.
        /// </summary>
        public void SetVisible(bool visible)
        {
            EnsureReferences();
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 최신 몬스터 QA 스냅샷을 사람이 읽을 수 있는 텍스트로 갱신합니다.
        /// </summary>
        public void Refresh(GameDirector.MonsterQaSnapshot snapshot)
        {
            EnsureReferences();
            infoText.text =
                "Monster QA\n"
                + "F9 toggle  F10 state  F11 reset\n"
                + "room: " + snapshot.RoomId + "\n"
                + "face: " + snapshot.FaceDirection + "\n"
                + "state: " + snapshot.State + "\n"
                + "entry: " + (snapshot.HasPlacement ? "found" : "missing") + "\n"
                + "enabled: " + snapshot.PlacementEnabled + "\n"
                + "rect: " + RectText(snapshot.NormalizedRect) + "\n"
                + "target: " + snapshot.ShouldShowMonster + "\n"
                + "image: " + snapshot.MonsterImageActive + "\n"
                + "status: " + snapshot.StatusText;
        }

        /// <summary>
        /// 별도 프리팹 없이 현재 Canvas 아래에 기본 QA 패널을 생성합니다.
        /// </summary>
        public static MonsterRuntimeQaPanel Create(RectTransform root)
        {
            var panelObject = new GameObject("MonsterRuntimeQaPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(MonsterRuntimeQaPanel));
            panelObject.transform.SetParent(root, false);
            var rectTransform = panelObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.67f, 0.58f);
            rectTransform.anchorMax = new Vector2(0.98f, 0.96f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var background = panelObject.GetComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.78f);
            background.raycastTarget = false;

            var textObject = new GameObject("MonsterRuntimeQaText", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(panelObject.transform, false);
            var textTransform = textObject.GetComponent<RectTransform>();
            textTransform.anchorMin = new Vector2(0.04f, 0.04f);
            textTransform.anchorMax = new Vector2(0.96f, 0.96f);
            textTransform.offsetMin = Vector2.zero;
            textTransform.offsetMax = Vector2.zero;

            var text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 15;
            text.color = new Color(0.88f, 0.95f, 0.86f, 1f);
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            var panel = panelObject.GetComponent<MonsterRuntimeQaPanel>();
            panel.infoText = text;
            panel.canvasGroup = panelObject.GetComponent<CanvasGroup>();
            panel.SetVisible(false);
            return panel;
        }

        private void EnsureReferences()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            if (infoText == null)
            {
                infoText = GetComponentInChildren<Text>(true);
            }
        }

        private static string RectText(Rect rect)
        {
            return rect.x.ToString("0.###") + ", "
                + rect.y.ToString("0.###") + ", "
                + rect.width.ToString("0.###") + ", "
                + rect.height.ToString("0.###");
        }
    }
}
