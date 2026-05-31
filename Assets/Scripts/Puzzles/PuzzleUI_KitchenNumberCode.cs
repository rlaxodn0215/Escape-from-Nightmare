// -----------------------------------------------------------------------------
// Codex comment pass: Kitchen Number Code
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleUI_KitchenNumberCode.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Puzzle controller for the Kitchen Number Code screen, translating UI input into puzzle progress and completion.
    public class PuzzleUI_KitchenNumberCode : PuzzleNumberCodeUIBase
    {
        // Initializes local UI and state from an external record before the player can interact with it.
        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            // TODO: Bind kitchen number code puzzle UI.
        }
    }
}
