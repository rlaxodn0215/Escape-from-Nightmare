// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Reward Type
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\PuzzleRewardType.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Serializable Puzzle Reward Type data container whose field names intentionally match the project data files.
    public static class PuzzleRewardType
    {
        // Stores the None value used by this script's runtime or editor workflow.
        public const string None = "None";
        // Stores the Item value used by this script's runtime or editor workflow.
        public const string Item = "Item";
        // Stores the Clue value used by this script's runtime or editor workflow.
        public const string Clue = "Clue";
        // Stores the Door Unlock value used by this script's runtime or editor workflow.
        public const string DoorUnlock = "DoorUnlock";
        // Stores the Final Chase value used by this script's runtime or editor workflow.
        public const string FinalChase = "FinalChase";
        // Stores the Ending value used by this script's runtime or editor workflow.
        public const string Ending = "Ending";
        // Stores the Custom value used by this script's runtime or editor workflow.
        public const string Custom = "Custom";
        // Stores the Start Final Chase value used by this script's runtime or editor workflow.
        public const string StartFinalChase = "StartFinalChase";
        // Stores the Item Transform value used by this script's runtime or editor workflow.
        public const string ItemTransform = "ItemTransform";
    }
}
