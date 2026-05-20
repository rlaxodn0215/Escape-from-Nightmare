using System;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.UI;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class MapRuntimeSystem : MonoBehaviour
    {
        [SerializeField] private GameBootstrap bootstrap;
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private MapUI mapUI;
        [SerializeField] private MapRoomMarker[] markers = Array.Empty<MapRoomMarker>();

        private void Awake()
        {
            bootstrap ??= FindFirstObjectByType<GameBootstrap>();
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
            mapUI ??= FindFirstObjectByType<MapUI>(FindObjectsInactive.Include);
        }

        private void OnEnable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged += HandleRoomChanged;
            }
        }

        private void Start()
        {
            if (roomSystem != null && roomSystem.CurrentRoom != null)
            {
                ApplyRoom(roomSystem.CurrentRoom);
            }
            else
            {
                ApplyStartRoom();
            }
        }

        private void OnDisable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged -= HandleRoomChanged;
            }
        }

        private void HandleRoomChanged(RoomDefinition room)
        {
            ApplyRoom(room);
        }

        private void ApplyStartRoom()
        {
            StageDefinition stageDefinition = bootstrap != null ? bootstrap.StageDefinition : null;
            if (stageDefinition == null)
            {
                return;
            }

            foreach (RoomDefinition room in stageDefinition.Rooms)
            {
                if (room != null && room.RoomId == stageDefinition.StartRoomId)
                {
                    ApplyRoom(room);
                    return;
                }
            }
        }

        private void ApplyRoom(RoomDefinition room)
        {
            if (room == null || mapUI == null)
            {
                return;
            }

            Vector2 markerPosition = GetMarkerPosition(room.RoomId);
            string roomLabel = string.IsNullOrWhiteSpace(room.DisplayNameForDebug) ? room.RoomId : room.DisplayNameForDebug;
            mapUI.SetCurrentRoom(room.FloorId, markerPosition, roomLabel);
        }

        private Vector2 GetMarkerPosition(string roomId)
        {
            foreach (MapRoomMarker marker in markers)
            {
                if (marker.RoomId == roomId)
                {
                    return marker.MarkerAnchoredPosition;
                }
            }

            Debug.LogWarning($"Map marker for room '{roomId}' is not defined.");
            return Vector2.zero;
        }

        [Serializable]
        public struct MapRoomMarker
        {
            [SerializeField] private string roomId;
            [SerializeField] private Vector2 markerAnchoredPosition;

            public string RoomId => roomId;
            public Vector2 MarkerAnchoredPosition => markerAnchoredPosition;
        }
    }
}
