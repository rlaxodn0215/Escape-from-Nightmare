using System;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [Serializable]
    public struct RoomLink
    {
        public string targetRoomId;
        public string viaInteractableId;
    }

    [Serializable]
    public struct InteractableRequirement
    {
        public string requiredItemId;
        public string requiredFlag;
        public bool consumeItem;
    }

    [Serializable]
    public struct EventEffect
    {
        public EventEffectType effectType;
        public string targetId;
        public string value;
        public int intValue;
        public float floatValue;
    }

    [Serializable]
    public struct PuzzleAnswerEntry
    {
        public string key;
        public string value;
    }

    [Serializable]
    public struct RectHitArea
    {
        public Rect rect;

        public RectHitArea(Rect rect)
        {
            this.rect = rect;
        }
    }
}
