using System;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Authored room data, including per-face backgrounds, movement links, and interactions.
    /// </summary>
    [Serializable]
    public sealed class RoomDefinition
    {
        public string roomId;
        public string displayName;
        public string floorName;
        public string backgroundResource;
        public RoomFaceDefinition[] faces = Array.Empty<RoomFaceDefinition>();
        public Vector2 mapPosition;
        public string[] connectedRoomIds = Array.Empty<string>();
        public InteractableDefinition[] interactables = Array.Empty<InteractableDefinition>();
        public int hideSpotCount;
        public float dangerModifier = 1f;
        public bool allowMonster = true;
    }
}
