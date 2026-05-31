// -----------------------------------------------------------------------------
// Codex comment pass: Clue Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\ClueRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Clue Record data container whose field names intentionally match the project data files.
    public class ClueRecord
    {
        // Stores the clue Id value used by this script's runtime or editor workflow.
        public string clueId;
        // Stores the location Id value used by this script's runtime or editor workflow.
        public string locationId;
        // Stores the image Path value used by this script's runtime or editor workflow.
        public string imagePath;
        // Stores the required Puzzle Id value used by this script's runtime or editor workflow.
        public string requiredPuzzleId;
        // Stores the required Item Id value used by this script's runtime or editor workflow.
        public string requiredItemId;
        // Stores the starts Unlocked value used by this script's runtime or editor workflow.
        public bool startsUnlocked;
        // Stores the display Name value used by this script's runtime or editor workflow.
        public string displayName;
        // Stores the description value used by this script's runtime or editor workflow.
        public string description;
        // Stores the locked Message value used by this script's runtime or editor workflow.
        public string lockedMessage;
    }
}
