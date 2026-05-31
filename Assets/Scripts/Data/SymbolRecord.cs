// -----------------------------------------------------------------------------
// Codex comment pass: Symbol Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\SymbolRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Symbol Record data container whose field names intentionally match the project data files.
    public class SymbolRecord
    {
        // Stores the symbol Id value used by this script's runtime or editor workflow.
        public string symbolId;
        // Stores the display Name value used by this script's runtime or editor workflow.
        public string displayName;
        // Stores the sprite Path value used by this script's runtime or editor workflow.
        public string spritePath;
        // Stores the value value used by this script's runtime or editor workflow.
        public int value;
    }
}
