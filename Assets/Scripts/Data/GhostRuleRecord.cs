// -----------------------------------------------------------------------------
// Codex comment pass: Ghost Rule Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\GhostRuleRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Ghost Rule Record data container whose field names intentionally match the project data files.
    public class GhostRuleRecord
    {
        // Stores the rule Id value used by this script's runtime or editor workflow.
        public string ruleId;
        // Stores the location Id value used by this script's runtime or editor workflow.
        public string locationId;
        // Stores the min Arrival Time value used by this script's runtime or editor workflow.
        public float minArrivalTime;
        // Stores the max Arrival Time value used by this script's runtime or editor workflow.
        public float maxArrivalTime;
        // Stores the min Leave Time value used by this script's runtime or editor workflow.
        public float minLeaveTime;
        // Stores the max Leave Time value used by this script's runtime or editor workflow.
        public float maxLeaveTime;
        // Stores the danger Increase Per Second value used by this script's runtime or editor workflow.
        public float dangerIncreasePerSecond;
    }
}
