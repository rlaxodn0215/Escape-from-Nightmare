// -----------------------------------------------------------------------------
// Codex comment pass: Game Start Mode
// Role: Defines shared runtime states, start modes, and base infrastructure used across the project.
// Scope: This script belongs to Core\GameStartMode.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Lists the supported Game Start Mode states so callers can branch without fragile string comparisons.
    public enum GameStartMode
    {
        None,
        NewGame,
        Continue,
        RestartFromCheckpoint
    }
}
