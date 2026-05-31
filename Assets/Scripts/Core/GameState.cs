// -----------------------------------------------------------------------------
// Codex comment pass: Game State
// Role: Defines shared runtime states, start modes, and base infrastructure used across the project.
// Scope: This script belongs to Core\GameState.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Lists the supported Game State states so callers can branch without fragile string comparisons.
    public enum GameState
    {
        None,
        Title,
        Playing,
        Puzzle,
        Examine,
        Hiding,
        Chase,
        GameOver,
        Ending
    }
}
