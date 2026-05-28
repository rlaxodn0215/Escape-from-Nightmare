using UnityEngine;

namespace EscapeFromNightmare
{
    public class PuzzleManager : Singleton<PuzzleManager>
    {
        public Transform puzzleUiRoot;
        public GameObject currentPuzzleInstance;
        public PuzzleRecord currentPuzzle;

        public void OpenPuzzle(string puzzleId)
        {
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

        public void CloseCurrentPuzzle()
        {
            if (currentPuzzleInstance != null)
            {
                Destroy(currentPuzzleInstance);
            }

            currentPuzzleInstance = null;
            currentPuzzle = null;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }
        }

        public void CompletePuzzle(string puzzleId)
        {
            if (string.IsNullOrEmpty(puzzleId))
            {
                Debug.LogWarning("Cannot complete puzzle with an empty puzzleId.");
                return;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsPuzzleCompleted(puzzleId))
            {
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

            CloseCurrentPuzzle();
        }

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
                        InventoryManager.Instance.TryAddItem(puzzle.rewardId);
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
                    }
                    else
                    {
                        Debug.LogWarning("SaveManager instance is missing.");
                    }
                    break;
                case PuzzleRewardType.FinalChase:
                case PuzzleRewardType.StartFinalChase:
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
