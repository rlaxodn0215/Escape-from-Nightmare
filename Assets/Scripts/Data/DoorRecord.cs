// -----------------------------------------------------------------------------
// Codex comment pass: Door Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\DoorRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Door Record data container whose field names intentionally match the project data files.
    public class DoorRecord
    {
        // Stores the door Id value used by this script's runtime or editor workflow.
        public string doorId;
        // Stores the from Location Id value used by this script's runtime or editor workflow.
        public string fromLocationId;
        // Stores the from View Id value used by this script's runtime or editor workflow.
        public string fromViewId;
        // Stores the to Location Id value used by this script's runtime or editor workflow.
        public string toLocationId;
        // Stores the to View Id value used by this script's runtime or editor workflow.
        public string toViewId;
        // Stores the required Item Id value used by this script's runtime or editor workflow.
        public string requiredItemId;
        // Stores the required Puzzle Id value used by this script's runtime or editor workflow.
        public string requiredPuzzleId;
        // Stores the starts Locked value used by this script's runtime or editor workflow.
        public bool startsLocked;
        // Stores the stays Unlocked After Open value used by this script's runtime or editor workflow.
        public bool staysUnlockedAfterOpen;
    }
}
