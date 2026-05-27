using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    public enum InteractableType
    {
        Door,
        ItemPickup,
        PuzzleObject,
        HideSpot,
        LockedDoor,
        EscapeDoor,
        ClueObject,
        EventTrigger
    }

    public enum ItemType
    {
        ClueItem,
        MechanicalPart,
        AltarObject,
        KeyItem
    }

    public enum PuzzleType
    {
        NumberLock,
        SymbolSequence,
        ColorSequence,
        SilentSequence,
        SymbolItemMatching,
        ItemUse
    }

    public enum MonsterState
    {
        Disabled,
        Normal,
        Approaching,
        Searching,
        NearDetection,
        Chase
    }

    public enum RoomFaceDirection
    {
        North,
        East,
        South,
        West
    }

    public enum SoundCategory
    {
        Bgm,
        Sfx,
        Ui,
        Ambience,
        Monster
    }

    [Serializable]
    public sealed class SoundEntry
    {
        public string soundId;
        public AudioClip clip;
        public SoundCategory category = SoundCategory.Sfx;
        public bool loop;
    }

    [Serializable]
    public sealed class SpriteBinding
    {
        public string spriteId;
        public Sprite sprite;
    }

    public static class BindingLookup
    {
        public static bool TryFindSprite(IEnumerable<SpriteBinding> bindings, string spriteId, out Sprite sprite)
        {
            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (binding != null && binding.spriteId == spriteId && binding.sprite != null)
                    {
                        sprite = binding.sprite;
                        return true;
                    }
                }
            }

            sprite = null;
            return false;
        }

        public static bool TryFindSound(IEnumerable<SoundEntry> entries, string soundId, out SoundEntry entry)
        {
            if (entries != null)
            {
                foreach (var candidate in entries)
                {
                    if (candidate != null && candidate.soundId == soundId)
                    {
                        entry = candidate;
                        return true;
                    }
                }
            }

            entry = null;
            return false;
        }
    }

    [Serializable]
    public sealed class RoomFaceDefinition
    {
        public RoomFaceDirection direction;
        public string displayName;
        public string backgroundResource;
        public ConditionalBackgroundDefinition[] conditionalBackgrounds = Array.Empty<ConditionalBackgroundDefinition>();
        public InteractableDefinition[] interactables = Array.Empty<InteractableDefinition>();
    }

    [Serializable]
    public sealed class ConditionalBackgroundDefinition
    {
        public string backgroundResource;
        public ConditionDefinition conditions = new ConditionDefinition();
    }

    [Serializable]
    public sealed class InteractableDefinition
    {
        public string interactableId;
        public string displayName;
        public InteractableType type;
        public Rect normalizedHitbox = new Rect(0.4f, 0.4f, 0.2f, 0.2f);
        public ConditionDefinition conditions = new ConditionDefinition();
        public string requiredItemId;
        public string grantsItemId;
        public string targetRoomId;
        public string puzzleId;
        public string solvesPuzzleId;
        public string eventId;
        public string imageResource;
        public string closeUpClosedResource;
        public string closeUpOpenWithItemResource;
        public string closeUpOpenEmptyResource;
        public string closeUpItemId;
        public Rect closeUpItemHitbox = new Rect(0.42f, 0.35f, 0.22f, 0.24f);
        public string closeUpOpenSoundId;
        public string closeUpCloseSoundId;
        public string hideViewResource;
        public string clueViewResource;
        public string[] setFlagIds = Array.Empty<string>();
        public string soundId;
        public bool consumeOnUse;
        public bool oneShot;
        public bool hiddenByDefault;
        public bool showWorldImage = true;
        public bool disableRoomHitboxWhenUsed;
    }

    [Serializable]
    public sealed class ConditionDefinition
    {
        public string[] requiredItemIds = Array.Empty<string>();
        public string[] requiredFlagIds = Array.Empty<string>();
        public string[] anyFlagIds = Array.Empty<string>();
        public string[] forbiddenFlagIds = Array.Empty<string>();
        public string[] solvedPuzzleIds = Array.Empty<string>();
    }
}
