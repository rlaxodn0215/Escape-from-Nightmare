namespace EscapeFromNightmares.Data
{
    public enum FloorId
    {
        FirstFloor,
        SecondFloor,
        Basement,
        Attic
    }

    public enum InteractableType
    {
        Door,
        ScreenEdge,
        Container,
        ItemPickup,
        PuzzleObject,
        ClueObject,
        HideSpot,
        LockedDoor,
        MonsterSpawnMarker,
        EscapeDoor
    }

    public enum PuzzleInputType
    {
        NumberLock,
        SymbolSequence,
        SilentSequence,
        ColorSequence,
        SymbolItemMatching,
        ItemUse
    }

    public enum ItemCategory
    {
        ClueItem,
        MechanicalPart,
        AltarObject,
        KeyItem
    }

    public enum EventEffectType
    {
        SetFlag,
        GiveItem,
        SpawnInteractable,
        PlayAudio,
        ChangeRoom,
        StartMonster,
        ChangeMonsterState,
        ShowUi,
        GameOver,
        StageClear
    }

    public enum MonsterState
    {
        Normal,
        Approaching,
        Searching,
        NearDetection,
        Chase
    }

    public enum AudioCategory
    {
        Bgm,
        Ambience,
        Sfx,
        Monster,
        Ui,
        Event
    }
}
