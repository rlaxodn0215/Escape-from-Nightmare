using System.Collections.Generic;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Top-level authored data for one stage.
    /// </summary>
    [System.Serializable]
    public sealed class StageDefinition
    {
        public string stageId = "stage1";
        public string displayName = "Stage 1";
        public string startRoomId = "child_room";
        public string clearFlag = "stage1_clear";
        public List<RoomDefinition> rooms = new List<RoomDefinition>();
        public List<ItemDefinition> items = new List<ItemDefinition>();
        public List<PuzzleDefinition> puzzles = new List<PuzzleDefinition>();
        public MonsterNodeGraph monsterNodeGraph;
        public List<SoundEntry> sounds = new List<SoundEntry>();
    }
}
