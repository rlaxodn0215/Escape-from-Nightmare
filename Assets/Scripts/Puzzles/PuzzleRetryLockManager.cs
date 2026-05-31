// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Retry Lock Manager
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleRetryLockManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Puzzle Retry Lock Manager screen, translating UI input into puzzle progress and completion.
    public class PuzzleRetryLockManager : Singleton<PuzzleRetryLockManager>
    {
        // Stores the locked Puzzle Ids value used by this script's runtime or editor workflow.
        private readonly HashSet<string> lockedPuzzleIds = new HashSet<string>();

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsLocked(string puzzleId)
        {
            return !string.IsNullOrEmpty(puzzleId) && lockedPuzzleIds.Contains(puzzleId);
        }

        // Performs the Lock Puzzle operation while keeping its implementation details inside this script.
        public void LockPuzzle(string puzzleId)
        {
            if (!string.IsNullOrEmpty(puzzleId))
            {
                lockedPuzzleIds.Add(puzzleId);
            }
        }

        // Performs the Unlock Puzzle operation while keeping its implementation details inside this script.
        public void UnlockPuzzle(string puzzleId)
        {
            if (!string.IsNullOrEmpty(puzzleId))
            {
                lockedPuzzleIds.Remove(puzzleId);
            }
        }

        // Performs the Clear All Locks operation while keeping its implementation details inside this script.
        public void ClearAllLocks()
        {
            lockedPuzzleIds.Clear();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.GhostLeft += HandleGhostLeft;
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.GhostLeft -= HandleGhostLeft;
            }
        }

        // Performs the Handle Ghost Left operation while keeping its implementation details inside this script.
        private void HandleGhostLeft(string locationId)
        {
            ClearAllLocks();
        }
    }
}
