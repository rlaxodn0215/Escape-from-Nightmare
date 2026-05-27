using System;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Authored puzzle rule, reward, and presentation data.
    /// </summary>
    [Serializable]
    public sealed class PuzzleDefinition
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
}
