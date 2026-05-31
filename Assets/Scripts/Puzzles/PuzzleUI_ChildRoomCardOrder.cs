// -----------------------------------------------------------------------------
// Codex comment pass: Child Room Card Order
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleUI_ChildRoomCardOrder.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Puzzle controller for the Child Room Card Order screen, translating UI input into puzzle progress and completion.
    public class PuzzleUI_ChildRoomCardOrder : PuzzleSequenceUIBase
    {
        // Initializes local UI and state from an external record before the player can interact with it.
        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            // TODO: Bind child room card order puzzle UI.
        }
    }
}
