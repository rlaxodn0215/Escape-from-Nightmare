// -----------------------------------------------------------------------------
// Codex comment pass: Game Settings Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\GameSettingsRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Game Settings Record data container whose field names intentionally match the project data files.
    public class GameSettingsRecord
    {
        // Stores the reference Width value used by this script's runtime or editor workflow.
        public int referenceWidth;
        // Stores the reference Height value used by this script's runtime or editor workflow.
        public int referenceHeight;
        // Stores the hide Min Seconds value used by this script's runtime or editor workflow.
        public float hideMinSeconds;
        // Stores the hide Max Seconds value used by this script's runtime or editor workflow.
        public float hideMaxSeconds;
        // Stores the title Scene Name value used by this script's runtime or editor workflow.
        public string titleSceneName;
        // Stores the game Scene Name value used by this script's runtime or editor workflow.
        public string gameSceneName;

        // Stores the ghost Default Min Arrival Time value used by this script's runtime or editor workflow.
        public float ghostDefaultMinArrivalTime;
        // Stores the ghost Default Max Arrival Time value used by this script's runtime or editor workflow.
        public float ghostDefaultMaxArrivalTime;
        // Stores the ghost Default Min Leave Time value used by this script's runtime or editor workflow.
        public float ghostDefaultMinLeaveTime;
        // Stores the ghost Default Max Leave Time value used by this script's runtime or editor workflow.
        public float ghostDefaultMaxLeaveTime;
        // Stores the ghost Danger Threshold value used by this script's runtime or editor workflow.
        public float ghostDangerThreshold;
        // Stores the chase Move Limit value used by this script's runtime or editor workflow.
        public int chaseMoveLimit;
    }
}
