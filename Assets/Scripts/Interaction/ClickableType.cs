// -----------------------------------------------------------------------------
// Codex comment pass: Clickable Type
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\ClickableType.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

namespace EscapeFromNightmare
{
    // Lists the supported Clickable Type states so callers can branch without fragile string comparisons.
    public enum ClickableType
    {
        ExamineImage,
        Puzzle,
        Door,
        HidePoint,
        PickupItem,
        UseItemTarget,
        FinalDoor
    }
}
