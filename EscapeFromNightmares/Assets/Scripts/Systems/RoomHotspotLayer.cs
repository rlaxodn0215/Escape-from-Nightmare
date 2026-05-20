using System;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class RoomHotspotLayer : MonoBehaviour
    {
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private RoomHotspotGroup[] roomGroups = Array.Empty<RoomHotspotGroup>();

        private void Awake()
        {
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
        }

        private void OnEnable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged += HandleRoomChanged;
                ApplyRoom(roomSystem.CurrentRoom);
            }
        }

        private void OnDisable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged -= HandleRoomChanged;
            }
        }

        private void Start()
        {
            ApplyRoom(roomSystem != null ? roomSystem.CurrentRoom : null);
        }

        private void HandleRoomChanged(RoomDefinition room)
        {
            ApplyRoom(room);
        }

        private void ApplyRoom(RoomDefinition room)
        {
            string activeRoomId = room != null ? room.RoomId : string.Empty;

            foreach (RoomHotspotGroup group in roomGroups)
            {
                if (group.Root == null)
                {
                    continue;
                }

                group.Root.SetActive(!string.IsNullOrWhiteSpace(activeRoomId) && group.RoomId == activeRoomId);
            }
        }
    }

    [Serializable]
    public struct RoomHotspotGroup
    {
        [SerializeField] private string roomId;
        [SerializeField] private GameObject root;

        public string RoomId => roomId;
        public GameObject Root => root;
    }
}
