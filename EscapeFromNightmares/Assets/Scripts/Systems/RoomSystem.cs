using System;
using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class RoomSystem : MonoBehaviour
    {
        [SerializeField] private GameBootstrap bootstrap;
        [SerializeField] private RoomViewPresenter roomViewPresenter;

        private readonly Dictionary<string, RoomDefinition> roomsById = new Dictionary<string, RoomDefinition>();
        private StageDefinition stageDefinition;
        private RoomDefinition currentRoom;

        public event Action<RoomDefinition> RoomChanged;

        public RoomDefinition CurrentRoom => currentRoom;

        private void Awake()
        {
            bootstrap ??= FindFirstObjectByType<GameBootstrap>();
            roomViewPresenter ??= FindFirstObjectByType<RoomViewPresenter>();
            stageDefinition = bootstrap != null ? bootstrap.StageDefinition : null;
            BuildRoomLookup();
        }

        private void Start()
        {
            string startRoomId = stageDefinition != null ? stageDefinition.StartRoomId : "child_room";
            ChangeRoom(startRoomId);
        }

        private void OnEnable()
        {
            bootstrap ??= FindFirstObjectByType<GameBootstrap>();
            if (bootstrap != null && bootstrap.GameStateManager != null)
            {
                bootstrap.GameStateManager.Stage1RunStarted += HandleStage1RunStarted;
            }
        }

        private void OnDisable()
        {
            if (bootstrap != null && bootstrap.GameStateManager != null)
            {
                bootstrap.GameStateManager.Stage1RunStarted -= HandleStage1RunStarted;
            }
        }

        public bool ChangeRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                Debug.LogWarning("Room change ignored because room id is empty.");
                return false;
            }

            if (!roomsById.TryGetValue(roomId, out RoomDefinition nextRoom))
            {
                Debug.LogWarning($"Room '{roomId}' is not defined in StageDefinition.");
                return false;
            }

            currentRoom = nextRoom;
            bootstrap?.GameStateManager?.SetCurrentRoom(roomId);
            roomViewPresenter?.ShowRoom(currentRoom);
            RoomChanged?.Invoke(currentRoom);
            return true;
        }

        private void BuildRoomLookup()
        {
            roomsById.Clear();
            if (stageDefinition == null)
            {
                Debug.LogWarning("RoomSystem has no StageDefinition.");
                return;
            }

            foreach (RoomDefinition room in stageDefinition.Rooms)
            {
                if (room == null || string.IsNullOrWhiteSpace(room.RoomId))
                {
                    continue;
                }

                roomsById[room.RoomId] = room;
            }
        }

        private void HandleStage1RunStarted()
        {
            string startRoomId = stageDefinition != null ? stageDefinition.StartRoomId : "child_room";
            ChangeRoom(startRoomId);
        }
    }
}
