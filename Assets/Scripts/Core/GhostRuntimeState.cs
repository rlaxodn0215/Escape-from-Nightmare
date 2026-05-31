// -----------------------------------------------------------------------------
// Codex comment pass: Ghost Runtime State
// Role: Defines shared runtime states, start modes, and base infrastructure used across the project.
// Scope: This script belongs to Core\GhostRuntimeState.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Lists the supported Ghost Runtime State states so callers can branch without fragile string comparisons.
    public enum GhostRuntimeState
    {
        Inactive,
        Patrolling,
        RespondingToNoise,
        SearchingLocation,
        LeavingLocation,
        Chasing
    }
}
