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

    [CreateAssetMenu(menuName = "Escape From Nightmares/Stage Definition")]
    public sealed class StageDefinition : ScriptableObject
    {
        public string stageId = "stage1";
        public string displayName = "Stage 1";
        public string startRoomId = "child_room";
        public string clearFlag = "stage1_clear";
        public List<RoomDefinition> rooms = new List<RoomDefinition>();
        public List<ItemDefinition> items = new List<ItemDefinition>();
        public List<PuzzleDefinition> puzzles = new List<PuzzleDefinition>();
        public MonsterNodeGraph monsterNodeGraph;
        public SoundCatalog soundCatalog;
    }

    [CreateAssetMenu(menuName = "Escape From Nightmares/Room Definition")]
    public sealed class RoomDefinition : ScriptableObject
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

    [CreateAssetMenu(menuName = "Escape From Nightmares/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public string description;
        public string iconResource;
        public ItemType itemType;
        public string altarSymbol;
    }

    [CreateAssetMenu(menuName = "Escape From Nightmares/Puzzle Definition")]
    public sealed class PuzzleDefinition : ScriptableObject
    {
        public string puzzleId;
        public string displayName;
        public PuzzleType puzzleType;
        public string[] answerTokens = Array.Empty<string>();
        public string[] requiredItemIds = Array.Empty<string>();
        public string[] rewardItemIds = Array.Empty<string>();
        public ConditionDefinition conditions = new ConditionDefinition();
        public string successFlag;
        public string successEventId;
        public string successSoundId;
        public string failureSoundId;
        public string closeUpResource;
        public float failureDanger = 8f;
        public bool consumeRequiredItems;
        public bool oneShot;
        public bool deferSolvedUntilRewardPickup;
    }

    [CreateAssetMenu(menuName = "Escape From Nightmares/Monster Node Graph")]
    public sealed class MonsterNodeGraph : ScriptableObject
    {
        public List<MonsterNode> nodes = new List<MonsterNode>();
    }

    [Serializable]
    public sealed class MonsterNode
    {
        public string nodeId;
        public string roomId;
        public string[] connectedNodeIds = Array.Empty<string>();
    }

    [CreateAssetMenu(menuName = "Escape From Nightmares/Sound Catalog")]
    public sealed class SoundCatalog : ScriptableObject
    {
        public List<SoundEntry> entries = new List<SoundEntry>();

        public bool TryFind(string soundId, out SoundEntry entry)
        {
            foreach (var candidate in entries)
            {
                if (candidate != null && candidate.soundId == soundId)
                {
                    entry = candidate;
                    return true;
                }
            }

            entry = null;
            return false;
        }
    }

    [Serializable]
    public sealed class SoundEntry
    {
        public string soundId;
        public string resourcePath;
        public SoundCategory category = SoundCategory.Sfx;
        public bool loop;
    }
}
