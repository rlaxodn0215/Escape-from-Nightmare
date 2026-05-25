using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Top-level authored data for one stage.
    /// </summary>
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
}
