using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class PuzzleRecord
    {
        public string puzzleId;
        public string locationId;
        public string type;
        public string prefabPath;
        public int codeLength;
        public string answerVariableName;
        public int timeLimitSeconds;
        public int failCountToNoise;
        public string rewardType;
        public string rewardId;
        public string requiredItemId;
        public bool startsSolved;
    }
}
