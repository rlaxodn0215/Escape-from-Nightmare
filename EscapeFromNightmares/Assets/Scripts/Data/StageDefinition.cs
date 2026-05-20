using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Stage Definition")]
    public sealed class StageDefinition : ScriptableObject
    {
        [SerializeField] private string stageId = "stage1";
        [SerializeField] private string startRoomId = "child_room";
        [SerializeField] private string clearEventId = "event_stage1_clear";
        [SerializeField] private string failEventId = "event_player_captured";
        [SerializeField] private string[] initialFlags = new string[0];
        [SerializeField] private RoomDefinition[] rooms = new RoomDefinition[0];
        [SerializeField] private ItemDefinition[] items = new ItemDefinition[0];
        [SerializeField] private PuzzleDefinition[] puzzles = new PuzzleDefinition[0];
        [SerializeField] private EventDefinition[] events = new EventDefinition[0];
        [SerializeField] private MonsterNodeDefinition[] monsterNodes = new MonsterNodeDefinition[0];
        [SerializeField] private SoundDefinition[] sounds = new SoundDefinition[0];

        public string StageId => stageId;
        public string StartRoomId => startRoomId;
        public string ClearEventId => clearEventId;
        public string FailEventId => failEventId;
        public string[] InitialFlags => initialFlags;
        public RoomDefinition[] Rooms => rooms;
        public ItemDefinition[] Items => items;
        public PuzzleDefinition[] Puzzles => puzzles;
        public EventDefinition[] Events => events;
        public MonsterNodeDefinition[] MonsterNodes => monsterNodes;
        public SoundDefinition[] Sounds => sounds;
    }
}
