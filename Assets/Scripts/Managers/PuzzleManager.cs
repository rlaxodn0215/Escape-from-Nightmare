// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\PuzzleManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Puzzle Manager system, keeping shared state and events behind one access point.
    public class PuzzleManager : Singleton<PuzzleManager>
    {
        // Stores the puzzle Ui Root value used by this script's runtime or editor workflow.
        public Transform puzzleUiRoot;
        // Stores the current Puzzle Instance value used by this script's runtime or editor workflow.
        public GameObject currentPuzzleInstance;
        // Stores the current Puzzle value used by this script's runtime or editor workflow.
        public PuzzleRecord currentPuzzle;

        public GameObject CurrentPuzzleInstance
        {
            get { return currentPuzzleInstance; }
        }

        public PuzzleRecord CurrentPuzzle
        {
            get { return currentPuzzle; }
        }

        public bool HasOpenPuzzle
        {
            get { return currentPuzzleInstance != null; }
        }

        // Opens the requested puzzle, clue, screen, or navigation target for the player.
        public void OpenPuzzle(string puzzleId)
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.DisablePuzzles)
            {
                Debug.Log("Puzzle opening is disabled for layout testing: " + puzzleId);
                return;
            }

            if (string.IsNullOrEmpty(puzzleId))
            {
                Debug.LogWarning("Cannot open puzzle with an empty puzzleId.");
                return;
            }

            if (PuzzleRetryLockManager.Instance != null && PuzzleRetryLockManager.Instance.IsLocked(puzzleId))
            {
                Debug.LogWarning("Puzzle is temporarily locked until the ghost leaves: " + puzzleId);
                return;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsPuzzleCompleted(puzzleId))
            {
                Debug.Log("Puzzle is already completed: " + puzzleId);
                return;
            }

            if (GameDataManager.Instance == null)
            {
                Debug.LogWarning("GameDataManager instance is missing.");
                return;
            }

            PuzzleRecord puzzle = GameDataManager.Instance.GetPuzzleById(puzzleId);
            if (puzzle == null)
            {
                Debug.LogWarning("Puzzle not found: " + puzzleId);
                return;
            }

            if (string.IsNullOrEmpty(puzzle.prefabPath))
            {
                Debug.LogWarning("Puzzle prefab path is empty: " + puzzleId);
                return;
            }

            GameObject prefab = Resources.Load<GameObject>(puzzle.prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning("Puzzle prefab not found at Resources path: " + puzzle.prefabPath);
                return;
            }

            CloseCurrentPuzzle();

            if (puzzleUiRoot != null)
            {
                currentPuzzleInstance = Instantiate(prefab, puzzleUiRoot);
            }
            else
            {
                Debug.LogWarning("Puzzle UI root is missing. Instantiating puzzle without an assigned UI root.");
                currentPuzzleInstance = Instantiate(prefab);
            }

            currentPuzzle = puzzle;

            PuzzleUIBase puzzleUi = currentPuzzleInstance.GetComponentInChildren<PuzzleUIBase>(true);
            if (puzzleUi != null)
            {
                puzzleUi.Initialize(puzzle);
            }
            else
            {
                Debug.LogWarning("Puzzle prefab is missing a PuzzleUIBase component: " + puzzle.prefabPath);
            }
        }

        // Closes the active UI or interaction and returns control to the normal game flow.
        public void CloseCurrentPuzzle()
        {
            if (currentPuzzleInstance != null)
            {
                Destroy(currentPuzzleInstance);
            }

            currentPuzzleInstance = null;
            currentPuzzle = null;

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Puzzle)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }
        }

        // Performs the Complete Puzzle operation while keeping its implementation details inside this script.
        public void CompletePuzzle(string puzzleId)
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.DisablePuzzles)
            {
                Debug.Log("Puzzle completion is disabled for layout testing: " + puzzleId);
                CloseCurrentPuzzle();
                return;
            }

            if (string.IsNullOrEmpty(puzzleId))
            {
                Debug.LogWarning("Cannot complete puzzle with an empty puzzleId.");
                return;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsPuzzleCompleted(puzzleId))
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayUi(AudioCue.UiConfirm);
                }

                Debug.Log("Puzzle already completed: " + puzzleId);
                CloseCurrentPuzzle();
                return;
            }
            else if (SaveManager.Instance == null)
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkPuzzleCompleted(puzzleId);
            }

            PuzzleRecord puzzle = currentPuzzle;

            if (puzzle == null || puzzle.puzzleId != puzzleId)
            {
                if (GameDataManager.Instance == null)
                {
                    Debug.LogWarning("GameDataManager instance is missing.");
                    return;
                }

                puzzle = GameDataManager.Instance.GetPuzzleById(puzzleId);
            }

            if (puzzle != null)
            {
                ApplyPuzzleReward(puzzle);
            }
            else
            {
                Debug.LogWarning("Completed puzzle not found: " + puzzleId);
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiConfirm);
            }

            CloseCurrentPuzzle();
        }

        // Applies calculated settings to Unity components or runtime state.
        public void ApplyPuzzleReward(PuzzleRecord puzzle)
        {
            if (puzzle == null)
            {
                Debug.LogWarning("Cannot apply reward for null puzzle.");
                return;
            }

            if (string.IsNullOrEmpty(puzzle.rewardType))
            {
                return;
            }

            string rewardType = puzzle.rewardType.Trim();
            if (rewardType == PuzzleRewardType.None)
            {
                return;
            }

            bool needsRewardId = rewardType == PuzzleRewardType.Item || rewardType == PuzzleRewardType.Clue || rewardType == PuzzleRewardType.DoorUnlock || rewardType == PuzzleRewardType.ItemTransform;
            if (needsRewardId && string.IsNullOrEmpty(puzzle.rewardId))
            {
                Debug.LogWarning("Puzzle rewardId is empty. Puzzle: " + puzzle.puzzleId + ", RewardType: " + rewardType);
                return;
            }

            switch (rewardType)
            {
                case PuzzleRewardType.Item:
                    if (InventoryManager.Instance != null)
                    {
                        if (InventoryManager.Instance.TryAddItem(puzzle.rewardId) && AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlaySfx(AudioCue.ItemPickup);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("InventoryManager instance is missing.");
                    }
                    break;
                case PuzzleRewardType.Clue:
                    if (ClueImageManager.Instance != null)
                    {
                        ClueImageManager.Instance.UnlockClue(puzzle.rewardId);
                    }
                    else
                    {
                        Debug.LogWarning("ClueImageManager instance is missing.");
                    }
                    break;
                case PuzzleRewardType.DoorUnlock:
                    if (SaveManager.Instance != null)
                    {
                        SaveManager.Instance.MarkDoorOpened(puzzle.rewardId);

                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlaySfx(AudioCue.DoorUnlock);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("SaveManager instance is missing.");
                    }
                    break;
                case PuzzleRewardType.FinalChase:
                case PuzzleRewardType.StartFinalChase:
                    if (GameDataManager.Instance != null && GameDataManager.Instance.DisableGhost)
                    {
                        Debug.Log("Final chase reward ignored because ghost is disabled for layout testing: " + puzzle.puzzleId);
                        break;
                    }

                    if (SaveManager.Instance != null)
                    {
                        SaveManager.Instance.SetFinalChaseStarted(true);
                        SaveManager.Instance.SaveGame();
                    }
                    else
                    {
                        Debug.LogWarning("SaveManager instance is missing.");
                    }

                    if (GhostManager.Instance != null)
                    {
                        GhostManager.Instance.StartChase();
                    }
                    else if (ChaseManager.Instance != null)
                    {
                        ChaseManager.Instance.StartChase();
                    }
                    break;
                case PuzzleRewardType.Custom:
                    Debug.Log("Custom puzzle reward handled by puzzle UI or later game logic. Puzzle: " + puzzle.puzzleId + ", RewardId: " + puzzle.rewardId);
                    break;
                case PuzzleRewardType.ItemTransform:
                    Debug.Log("ItemTransform reward requires puzzle-specific handling. Puzzle: " + puzzle.puzzleId + ", RewardId: " + puzzle.rewardId);
                    break;
                case PuzzleRewardType.Ending:
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EnterEnding();
                    }
                    else
                    {
                        Debug.LogWarning("GameManager instance is missing.");
                    }
                    break;
                default:
                    Debug.LogWarning("Unknown puzzle rewardType: " + rewardType + ", Puzzle: " + puzzle.puzzleId);
                    break;
            }
        }
    }
}
