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
        public Vector2 mapPosition;
        public string[] connectedRoomIds = Array.Empty<string>();
        public InteractableDefinition[] interactables = Array.Empty<InteractableDefinition>();
        public int hideSpotCount;
        public float dangerModifier = 1f;
        public bool allowMonster = true;
    }

    [Serializable]
    public sealed class InteractableDefinition
    {
        public string interactableId;
        public string displayName;
        public InteractableType type;
        public Rect normalizedHitbox = new Rect(0.4f, 0.4f, 0.2f, 0.2f);
        public string requiredItemId;
        public string grantsItemId;
        public string targetRoomId;
        public string puzzleId;
        public string eventId;
        public bool hiddenByDefault;
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
        public string successFlag;
        public string successEventId;
        public float failureDanger = 8f;
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
    }

    [Serializable]
    public sealed class SoundEntry
    {
        public string soundId;
        public string resourcePath;
        public bool loop;
    }
}
