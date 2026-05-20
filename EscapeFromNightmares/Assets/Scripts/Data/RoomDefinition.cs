using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Room Definition")]
    public sealed class RoomDefinition : ScriptableObject
    {
        [SerializeField] private string roomId;
        [SerializeField] private FloorId floorId;
        [SerializeField] private string displayNameForDebug;
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private SoundDefinition ambienceAudio;
        [SerializeField] private float dangerModifier = 1f;
        [SerializeField] private bool monsterCanEnter = true;
        [SerializeField] private RoomLink[] connectedRooms = new RoomLink[0];
        [SerializeField] private InteractableDefinition[] interactableDefinitions = new InteractableDefinition[0];

        public string RoomId => roomId;
        public FloorId FloorId => floorId;
        public string DisplayNameForDebug => displayNameForDebug;
        public Sprite BackgroundSprite => backgroundSprite;
        public SoundDefinition AmbienceAudio => ambienceAudio;
        public float DangerModifier => dangerModifier;
        public bool MonsterCanEnter => monsterCanEnter;
        public RoomLink[] ConnectedRooms => connectedRooms;
        public InteractableDefinition[] InteractableDefinitions => interactableDefinitions;
    }
}
