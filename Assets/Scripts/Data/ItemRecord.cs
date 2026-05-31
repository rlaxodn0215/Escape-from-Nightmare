// -----------------------------------------------------------------------------
// Codex comment pass: Item Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\ItemRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Item Record data container whose field names intentionally match the project data files.
    public class ItemRecord
    {
        // Stores the item Id value used by this script's runtime or editor workflow.
        public string itemId;
        // Stores the display Name value used by this script's runtime or editor workflow.
        public string displayName;
        // Stores the description value used by this script's runtime or editor workflow.
        public string description;
        // Stores the icon Path value used by this script's runtime or editor workflow.
        public string iconPath;
        // Stores the consumable value used by this script's runtime or editor workflow.
        public bool consumable;
    }
}
