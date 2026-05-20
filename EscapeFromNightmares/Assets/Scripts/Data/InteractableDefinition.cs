using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Interactable Definition")]
    public sealed class InteractableDefinition : ScriptableObject
    {
        [SerializeField] private string roomId;
        [SerializeField] private string interactableId;
        [SerializeField] private InteractableType interactableType;
        [SerializeField] private Rect hitArea = new Rect(0f, 0f, 128f, 128f);
        [SerializeField] private InteractableRequirement[] requirements = new InteractableRequirement[0];
        [SerializeField] private string eventId;
        [SerializeField] private string targetRoomId;
        [SerializeField] private ItemDefinition itemReward;
        [SerializeField] private PuzzleDefinition puzzleDefinition;

        public string RoomId => roomId;
        public string InteractableId => interactableId;
        public InteractableType InteractableType => interactableType;
        public Rect HitArea => hitArea;
        public InteractableRequirement[] Requirements => requirements;
        public string EventId => eventId;
        public string TargetRoomId => targetRoomId;
        public ItemDefinition ItemReward => itemReward;
        public PuzzleDefinition PuzzleDefinition => puzzleDefinition;
    }
}
