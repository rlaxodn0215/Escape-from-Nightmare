using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Puzzle Definition")]
    public sealed class PuzzleDefinition : ScriptableObject
    {
        [SerializeField] private string puzzleId;
        [SerializeField] private string roomId;
        [SerializeField] private PuzzleInputType inputType;
        [SerializeField] private string answer;
        [SerializeField] private PuzzleAnswerEntry[] answerEntries = new PuzzleAnswerEntry[0];
        [SerializeField] private string successEventId;
        [SerializeField] private string failureEventId;
        [SerializeField] private ItemDefinition[] rewardItems = new ItemDefinition[0];
        [SerializeField] private float dangerOnFailure;

        public string PuzzleId => puzzleId;
        public string RoomId => roomId;
        public PuzzleInputType InputType => inputType;
        public string Answer => answer;
        public PuzzleAnswerEntry[] AnswerEntries => answerEntries;
        public string SuccessEventId => successEventId;
        public string FailureEventId => failureEventId;
        public ItemDefinition[] RewardItems => rewardItems;
        public float DangerOnFailure => dangerOnFailure;
    }
}
