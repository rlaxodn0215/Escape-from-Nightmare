// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\PuzzleRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Puzzle Record data container whose field names intentionally match the project data files.
    public class PuzzleRecord
    {
        // Stores the puzzle Id value used by this script's runtime or editor workflow.
        public string puzzleId;
        // Stores the location Id value used by this script's runtime or editor workflow.
        public string locationId;
        // Stores the type value used by this script's runtime or editor workflow.
        public string type;
        // Stores the prefab Path value used by this script's runtime or editor workflow.
        public string prefabPath;
        // Stores the code Length value used by this script's runtime or editor workflow.
        public int codeLength;
        // Stores the answer Variable Name value used by this script's runtime or editor workflow.
        public string answerVariableName;
        // Stores the time Limit Seconds value used by this script's runtime or editor workflow.
        public int timeLimitSeconds;
        // Stores the fail Count To Noise value used by this script's runtime or editor workflow.
        public int failCountToNoise;
        // Stores the reward Type value used by this script's runtime or editor workflow.
        public string rewardType;
        // Stores the reward Id value used by this script's runtime or editor workflow.
        public string rewardId;
        // Stores the required Item Id value used by this script's runtime or editor workflow.
        public string requiredItemId;
        // Stores the starts Solved value used by this script's runtime or editor workflow.
        public bool startsSolved;
    }
}
