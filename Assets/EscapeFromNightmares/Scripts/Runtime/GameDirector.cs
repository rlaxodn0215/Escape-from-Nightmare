using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    public sealed class GameDirector : MonoBehaviour
    {
        private StageDefinition stage;
        private GameSession session;
        private InteractionSystem interactionSystem;
        private InventoryService inventoryService;
        private PuzzleService puzzleService;
        private DangerSystem dangerSystem;
        private HidingSystem hidingSystem;
        private MonsterAIController monsterAI;
        private SettingsSaveService saveService;

        private readonly Dictionary<string, RoomDefinition> rooms = new Dictionary<string, RoomDefinition>();
        private readonly Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>();
        private readonly Dictionary<string, PuzzleDefinition> puzzles = new Dictionary<string, PuzzleDefinition>();

        private Canvas canvas;
        private RectTransform root;
        private Text titleText;
        private Text roomText;
        private Text statusText;
        private Text inventoryText;
        private Text logText;
        private RectTransform interactionPanel;
        private GameObject titlePanel;
        private GameObject gamePanel;
        private string latestLog = "Start를 눌러 악몽의 집에 들어가세요.";

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
            interactionSystem = new InteractionSystem(session);
            inventoryService = new InventoryService(session);
            puzzleService = new PuzzleService(session);
            dangerSystem = new DangerSystem();
            hidingSystem = new HidingSystem();
            monsterAI = new MonsterAIController();
            saveService = new SettingsSaveService(Application.persistentDataPath);

            EnsureEventSystem();
            BuildUi();
        }

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (gamePanel == null || !gamePanel.activeSelf)
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
                return;
            }

            RefreshStatus();
        }

        private void StartGame()
        {
            session.Start(stage);
            dangerSystem.Reset();
            hidingSystem.Exit();
            monsterAI.Reset();
            latestLog = "아이 방에서 깨어났습니다. 문밖은 지나치게 조용합니다.";
            titlePanel.SetActive(false);
            gamePanel.SetActive(true);
            OpenRoom(session.CurrentRoomId);
        }

        private void ShowTitle()
        {
            titlePanel.SetActive(true);
            gamePanel.SetActive(false);
        }

        private void OpenRoom(string roomId)
        {
            if (!rooms.ContainsKey(roomId))
            {
                latestLog = "아직 연결되지 않은 방입니다: " + roomId;
                return;
            }

            session.MoveTo(roomId);
            hidingSystem.Exit();
            dangerSystem.OnRoomChanged();
            RenderRoom();
        }

        private void RenderRoom()
        {
            var room = CurrentRoom();
            titleText.text = stage.displayName;
            roomText.text = room.displayName + "\n" + room.floorName + "\n\n" + PlaceholderDescription(room);
            ClearChildren(interactionPanel);

            AddPanelLabel(interactionPanel, "상호작용");
            foreach (var interactable in room.interactables)
            {
                AddPanelButton(interactionPanel, interactable.displayName, () => HandleInteractable(interactable));
            }

            AddPanelLabel(interactionPanel, "시스템");
            AddPanelButton(interactionPanel, "인벤토리", RefreshInventoryLog);
            AddPanelButton(interactionPanel, "지도", ShowMapLog);
            AddPanelButton(interactionPanel, "설정 저장", SaveSettings);
            AddPanelButton(interactionPanel, "타이틀로", ShowTitle);
            RefreshStatus();
        }

        private void HandleInteractable(InteractableDefinition interactable)
        {
            if (!session.HasItem(interactable.requiredItemId))
            {
                latestLog = "필요한 아이템이 없습니다: " + interactable.requiredItemId;
                return;
            }

            var result = interactionSystem.Resolve(interactable);
            switch (result.ResultType)
            {
                case InteractionResultType.MoveRoom:
                    OpenRoom(result.Value);
                    return;
                case InteractionResultType.AcquireItem:
                    AcquireItem(result.Value);
                    session.SetFlag(interactable.eventId);
                    break;
                case InteractionResultType.OpenPuzzle:
                    SolvePuzzle(result.Value);
                    break;
                case InteractionResultType.EnterHideSpot:
                    ToggleHiding();
                    break;
                case InteractionResultType.TriggerEvent:
                    TriggerEvent(result.Value);
                    break;
            }

            RenderRoom();
        }

        private void AcquireItem(string itemId)
        {
            if (!inventoryService.Acquire(itemId))
            {
                latestLog = "이미 확인했습니다.";
                return;
            }

            latestLog = ItemName(itemId) + " 획득.";
            if (itemId == "front_door_key")
            {
                monsterAI.Enable();
                monsterAI.ForceChase();
                latestLog += " 열쇠를 집는 순간 최종 추격이 시작됩니다.";
            }
        }

        private void SolvePuzzle(string puzzleId)
        {
            if (!puzzles.TryGetValue(puzzleId, out var puzzle))
            {
                latestLog = "퍼즐 데이터가 없습니다: " + puzzleId;
                return;
            }

            var solved = puzzleService.TrySolve(puzzle, puzzle.answerTokens);
            if (!solved)
            {
                dangerSystem.AddNoise(puzzle.failureDanger);
                latestLog = puzzle.displayName + " 해결 실패. 단서나 아이템이 부족합니다.";
                return;
            }

            latestLog = puzzle.displayName + " 해결.";
            if (puzzle.rewardItemIds.Length > 0)
            {
                latestLog += " 보상: " + string.Join(", ", puzzle.rewardItemIds.Select(ItemName));
            }

            if (puzzleId == "laundry_storage_box")
            {
                monsterAI.Enable();
                dangerSystem.AddNoise(20f);
                latestLog += " 멀리서 발소리가 들리기 시작합니다.";
            }
            else if (puzzleId == "basement_altar")
            {
                monsterAI.Enable();
                monsterAI.ForceChase();
                latestLog += " 현관 열쇠를 얻자 최종 추격이 시작됩니다.";
            }
            else if (puzzleId == "front_door_escape")
            {
                CompleteStage();
            }
        }

        private void TriggerEvent(string eventId)
        {
            session.SetFlag(eventId);
            if (eventId == "event_kitchen_first_appearance")
            {
                monsterAI.Enable();
                dangerSystem.AddNoise(25f);
                latestLog = "부엌 문 앞의 검은 형체가 사라졌습니다. 이제 집 안을 돌아다닙니다.";
            }
            else
            {
                latestLog = "단서를 확인했습니다: " + eventId;
            }
        }

        private void ToggleHiding()
        {
            if (hidingSystem.IsHiding)
            {
                hidingSystem.Exit();
                latestLog = "은신을 해제했습니다.";
            }
            else
            {
                hidingSystem.Enter(MousePosition());
                dangerSystem.AddNoise(-5f);
                latestLog = "숨었습니다. 마우스를 급하게 움직이면 들킬 수 있습니다.";
            }
        }

        private void CompleteStage()
        {
            session.SetFlag(stage.clearFlag);
            var records = saveService.LoadClearRecords();
            records.stage1Clear = true;
            saveService.SaveClearRecords(records);
            latestLog = "현관문이 열렸습니다. 바깥도 또 다른 악몽처럼 보입니다. Stage 1 Clear.";
            monsterAI.Reset();
            RenderRoom();
        }

        private void GameOver()
        {
            latestLog = "Game Over. 눈을 뜨자 다시 아이 방입니다.";
            session.Start(stage);
            dangerSystem.Reset();
            hidingSystem.Exit();
            monsterAI.Reset();
            OpenRoom(stage.startRoomId);
        }

        private void RefreshStatus()
        {
            var room = CurrentRoom();
            statusText.text =
                "Monster: " + monsterAI.State +
                "\nDanger: " + Mathf.RoundToInt(dangerSystem.DangerLevel) +
                "\nNoise: " + Mathf.RoundToInt(dangerSystem.NoiseLevel) +
                "\nCapture: " + Mathf.RoundToInt(dangerSystem.CaptureGauge) +
                "\nHiding: " + (hidingSystem.IsHiding ? "Yes (" + Mathf.RoundToInt(hidingSystem.HideNoise) + ")" : "No") +
                "\nRoom: " + room.roomId;
            inventoryText.text = "Inventory\n" + (session.InventoryItems.Count == 0 ? "- empty" : string.Join("\n", session.InventoryItems.Select(ItemName)));
            logText.text = latestLog;
        }

        private void RefreshInventoryLog()
        {
            latestLog = session.InventoryItems.Count == 0 ? "인벤토리가 비어 있습니다." : "소지품: " + string.Join(", ", session.InventoryItems.Select(ItemName));
            RefreshStatus();
        }

        private void ShowMapLog()
        {
            latestLog = "현재 위치: " + CurrentRoom().floorName + " / " + CurrentRoom().displayName + ". 연결: " + string.Join(", ", CurrentRoom().connectedRoomIds);
            RefreshStatus();
        }

        private void SaveSettings()
        {
            saveService.SaveSettings(new SettingsSaveService.SettingsData());
            latestLog = "설정값을 저장했습니다. 진행 상황은 저장하지 않습니다.";
            RefreshStatus();
        }

        private RoomDefinition CurrentRoom()
        {
            return rooms[session.CurrentRoomId];
        }

        private string ItemName(string itemId)
        {
            return items.TryGetValue(itemId, out var item) ? item.displayName : itemId;
        }

        private static string PlaceholderDescription(RoomDefinition room)
        {
            return "리소스 경로: " + room.backgroundResource + "\n실제 배경이 들어오기 전까지 어두운 더미 화면으로 사용합니다.";
        }

        private void BuildUi()
        {
            canvas = new GameObject("Escape UI").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvas.gameObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280f, 720f);
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            root = canvas.GetComponent<RectTransform>();

            titlePanel = Panel("Title", root, new Color(0.02f, 0.02f, 0.025f, 1f));
            var title = TextBlock("Escape From Nightmares", titlePanel.transform, 44, TextAnchor.MiddleCenter);
            Stretch(title.rectTransform, 0f, 0.62f, 1f, 0.86f);
            var subtitle = TextBlock("Unity 6 URP 2D prototype", titlePanel.transform, 18, TextAnchor.MiddleCenter);
            Stretch(subtitle.rectTransform, 0f, 0.52f, 1f, 0.62f);
            var startButton = Button("Start", titlePanel.transform, StartGame);
            Stretch(startButton.GetComponent<RectTransform>(), 0.42f, 0.38f, 0.58f, 0.47f);

            gamePanel = Panel("Game", root, new Color(0.015f, 0.015f, 0.018f, 1f));
            titleText = TextBlock("", gamePanel.transform, 22, TextAnchor.MiddleLeft);
            Stretch(titleText.rectTransform, 0.03f, 0.9f, 0.7f, 0.98f);
            roomText = TextBlock("", gamePanel.transform, 24, TextAnchor.UpperLeft);
            Stretch(roomText.rectTransform, 0.04f, 0.2f, 0.66f, 0.86f);
            statusText = TextBlock("", gamePanel.transform, 15, TextAnchor.UpperLeft);
            Stretch(statusText.rectTransform, 0.7f, 0.66f, 0.96f, 0.88f);
            inventoryText = TextBlock("", gamePanel.transform, 15, TextAnchor.UpperLeft);
            Stretch(inventoryText.rectTransform, 0.7f, 0.42f, 0.96f, 0.64f);
            logText = TextBlock("", gamePanel.transform, 16, TextAnchor.UpperLeft);
            Stretch(logText.rectTransform, 0.04f, 0.05f, 0.66f, 0.18f);

            interactionPanel = Panel("Interactions", gamePanel.transform, new Color(0f, 0f, 0f, 0f)).GetComponent<RectTransform>();
            Stretch(interactionPanel, 0.7f, 0.05f, 0.96f, 0.4f);
            var layout = interactionPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static GameObject Panel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var image = panel.AddComponent<Image>();
            image.color = color;
            Stretch(panel.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f);
            return panel;
        }

        private static Text TextBlock(string content, Transform parent, int size, TextAnchor anchor)
        {
            var textObject = new GameObject("Text");
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.color = new Color(0.88f, 0.86f, 0.82f, 1f);
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button Button(string label, Transform parent, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(label);
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.02f, 0.025f, 0.95f);
            var button = buttonObject.AddComponent<Button>();
            button.onClick.AddListener(action);
            var text = TextBlock(label, buttonObject.transform, 16, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, 0f, 0f, 1f, 1f);
            return button;
        }

        private static void AddPanelLabel(Transform parent, string label)
        {
            var text = TextBlock(label, parent, 14, TextAnchor.MiddleLeft);
            text.color = new Color(0.65f, 0.13f, 0.13f, 1f);
            text.rectTransform.sizeDelta = new Vector2(0f, 24f);
        }

        private static void AddPanelButton(Transform parent, string label, UnityEngine.Events.UnityAction action)
        {
            var button = Button(label, parent, action);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 34f);
        }

        private static void ClearChildren(Transform parent)
        {
            for (var index = parent.childCount - 1; index >= 0; index--)
            {
                Destroy(parent.GetChild(index).gameObject);
            }
        }

        private static void Stretch(RectTransform rectTransform, float minX, float minY, float maxX, float maxY)
        {
            rectTransform.anchorMin = new Vector2(minX, minY);
            rectTransform.anchorMax = new Vector2(maxX, maxY);
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
