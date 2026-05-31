// -----------------------------------------------------------------------------
// Codex comment pass: Navigation Action Type
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\NavigationActionType.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Lists the supported Navigation Action Type states so callers can branch without fragile string comparisons.
    public enum NavigationActionType
    {
        RotateLeft,
        RotateRight,
        SetLocation,
        SetView
    }
}
