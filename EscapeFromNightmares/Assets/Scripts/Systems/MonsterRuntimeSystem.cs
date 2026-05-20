using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class MonsterRuntimeSystem : MonoBehaviour
    {
        [SerializeField] private GameBootstrap bootstrap;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private HidingRuntimeSystem hidingRuntimeSystem;
        [SerializeField] private SoundRuntimeSystem soundRuntimeSystem;
        [SerializeField] private MonsterOverlay monsterOverlay;
        [SerializeField] private MonsterState currentState = MonsterState.Normal;
        [SerializeField, Min(0f)] private float nearCapturePressurePerSecond = 0.06f;
        [SerializeField, Min(0f)] private float chaseCapturePressurePerSecond = 0.18f;
        [SerializeField, Min(1)] private int chaseRoomMovesToEscape = 3;

        private readonly Dictionary<string, MonsterNodeDefinition> nodesByRoom = new Dictionary<string, MonsterNodeDefinition>();
        private StageDefinition stageDefinition;
        private string currentMonsterRoomId;
        private int chaseRoomMoveCount;

        public MonsterState CurrentState => currentState;
        public string CurrentMonsterRoomId => currentMonsterRoomId;

        private void Awake()
        {
            bootstrap ??= FindFirstObjectByType<GameBootstrap>();
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
            hidingRuntimeSystem ??= FindFirstObjectByType<HidingRuntimeSystem>();
            soundRuntimeSystem ??= FindFirstObjectByType<SoundRuntimeSystem>();
            monsterOverlay ??= FindFirstObjectByType<MonsterOverlay>(FindObjectsInactive.Include);
            stageDefinition = bootstrap != null ? bootstrap.StageDefinition : null;
            BuildNodeLookup();
            ApplyPresentation();
        }

        private void OnEnable()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            if (roomSystem != null)
            {
                roomSystem.RoomChanged += HandleRoomChanged;
            }

            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted += ResetRuntimeState;
            }

            if (hidingRuntimeSystem != null)
            {
                hidingRuntimeSystem.HidingCompleted += HandleHidingCompleted;
            }
        }

        private void OnDisable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged -= HandleRoomChanged;
            }

            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted -= ResetRuntimeState;
            }

            if (hidingRuntimeSystem != null)
            {
                hidingRuntimeSystem.HidingCompleted -= HandleHidingCompleted;
            }
        }

        private void Update()
        {
            if (hidingRuntimeSystem == null)
            {
                return;
            }

            if (currentState == MonsterState.NearDetection)
            {
                hidingRuntimeSystem.AddCapturePressure(nearCapturePressurePerSecond * Time.deltaTime);
            }
            else if (currentState == MonsterState.Chase)
            {
                hidingRuntimeSystem.AddCapturePressure(chaseCapturePressurePerSecond * Time.deltaTime);
            }
        }

        public void StartMonster(string triggerId = "")
        {
            currentMonsterRoomId = ResolveRoomForTrigger(triggerId);
            SetState(MonsterState.Approaching);
        }

        public void SetState(MonsterState nextState)
        {
            currentState = nextState;
            if (nextState == MonsterState.Chase || nextState == MonsterState.Normal)
            {
                chaseRoomMoveCount = 0;
            }

            ApplyPresentation();
        }

        public void ApplyEventRequest(string targetId, string value)
        {
            if (!string.IsNullOrWhiteSpace(targetId))
            {
                currentMonsterRoomId = ResolveRoomForTrigger(targetId);
            }

            if (TryParseState(value, out MonsterState nextState))
            {
                SetState(nextState);
            }
            else if (currentState == MonsterState.Normal)
            {
                SetState(MonsterState.Approaching);
            }
        }

        public void ResetRuntimeState()
        {
            currentMonsterRoomId = string.Empty;
            chaseRoomMoveCount = 0;
            SetState(MonsterState.Normal);
        }

        private void HandleRoomChanged(RoomDefinition room)
        {
            if (room == null || currentState == MonsterState.Normal)
            {
                return;
            }

            if (currentState == MonsterState.Chase && room.RoomId != currentMonsterRoomId)
            {
                chaseRoomMoveCount++;
                currentMonsterRoomId = room.RoomId;
                if (chaseRoomMoveCount >= chaseRoomMovesToEscape)
                {
                    SetState(MonsterState.Normal);
                }

                return;
            }

            if (room.RoomId == currentMonsterRoomId && currentState == MonsterState.Approaching)
            {
                SetState(MonsterState.NearDetection);
            }
        }

        private void HandleHidingCompleted(string hideSpotId)
        {
            if (currentState == MonsterState.NearDetection || currentState == MonsterState.Chase || currentState == MonsterState.Searching)
            {
                currentMonsterRoomId = string.Empty;
                SetState(MonsterState.Normal);
            }
        }

        private void ApplyPresentation()
        {
            if (monsterOverlay == null)
            {
                return;
            }

            switch (currentState)
            {
                case MonsterState.Normal:
                    monsterOverlay.Hide();
                    break;
                case MonsterState.Approaching:
                case MonsterState.Searching:
                    monsterOverlay.ShowSilhouette();
                    soundRuntimeSystem?.PlaySound("monster_footstep_far");
                    break;
                case MonsterState.NearDetection:
                    monsterOverlay.ShowNearDetection();
                    soundRuntimeSystem?.PlaySound("monster_breath_near");
                    break;
                case MonsterState.Chase:
                    monsterOverlay.ShowChase();
                    soundRuntimeSystem?.PlaySound("monster_chase_start");
                    break;
            }
        }

        private void BuildNodeLookup()
        {
            nodesByRoom.Clear();
            if (stageDefinition == null)
            {
                return;
            }

            foreach (MonsterNodeDefinition node in stageDefinition.MonsterNodes)
            {
                if (node != null && !string.IsNullOrWhiteSpace(node.RoomId))
                {
                    nodesByRoom[node.RoomId] = node;
                }
            }
        }

        private string ResolveRoomForTrigger(string triggerId)
        {
            if (roomSystem != null && roomSystem.CurrentRoom != null)
            {
                return roomSystem.CurrentRoom.RoomId;
            }

            if (!string.IsNullOrWhiteSpace(currentMonsterRoomId))
            {
                return currentMonsterRoomId;
            }

            return nodesByRoom.ContainsKey("kitchen") ? "kitchen" : string.Empty;
        }

        private static bool TryParseState(string value, out MonsterState state)
        {
            state = MonsterState.Approaching;
            return !string.IsNullOrWhiteSpace(value) && System.Enum.TryParse(value, true, out state);
        }
    }
}
