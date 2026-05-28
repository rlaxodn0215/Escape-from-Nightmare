using System.Collections.Generic;

namespace EscapeFromNightmare
{
    public class PuzzleRetryLockManager : Singleton<PuzzleRetryLockManager>
    {
        private readonly HashSet<string> lockedPuzzleIds = new HashSet<string>();

        public bool IsLocked(string puzzleId)
        {
            return !string.IsNullOrEmpty(puzzleId) && lockedPuzzleIds.Contains(puzzleId);
        }

        public void LockPuzzle(string puzzleId)
        {
            if (!string.IsNullOrEmpty(puzzleId))
            {
                lockedPuzzleIds.Add(puzzleId);
            }
        }

        public void UnlockPuzzle(string puzzleId)
        {
            if (!string.IsNullOrEmpty(puzzleId))
            {
                lockedPuzzleIds.Remove(puzzleId);
            }
        }

        public void ClearAllLocks()
        {
            lockedPuzzleIds.Clear();
        }

        private void OnEnable()
        {
            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.GhostLeft += HandleGhostLeft;
            }
        }

        private void OnDisable()
        {
            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.GhostLeft -= HandleGhostLeft;
            }
        }

        private void HandleGhostLeft(string locationId)
        {
            ClearAllLocks();
        }
    }
}
