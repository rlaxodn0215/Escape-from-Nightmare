using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.UI;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class PuzzleSystem : MonoBehaviour
    {
        [SerializeField] private PuzzleUI puzzleUI;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private EventRuntimeSystem eventRuntimeSystem;
        [SerializeField] private SoundRuntimeSystem soundRuntimeSystem;

        private PuzzleDefinition activePuzzle;

        private void Awake()
        {
            puzzleUI ??= FindFirstObjectByType<PuzzleUI>(FindObjectsInactive.Include);
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            inventorySystem ??= FindFirstObjectByType<InventorySystem>();
            eventRuntimeSystem ??= FindFirstObjectByType<EventRuntimeSystem>();
            soundRuntimeSystem ??= FindFirstObjectByType<SoundRuntimeSystem>();
        }

        private void OnEnable()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted += ResetRuntimeState;
            }

            if (puzzleUI != null)
            {
                puzzleUI.AnswerSubmitted += HandleAnswerSubmitted;
            }
        }

        private void OnDisable()
        {
            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted -= ResetRuntimeState;
            }

            if (puzzleUI != null)
            {
                puzzleUI.AnswerSubmitted -= HandleAnswerSubmitted;
            }
        }

        public void ResetRuntimeState()
        {
            activePuzzle = null;
            puzzleUI?.Hide();
        }

        public void OpenPuzzle(PuzzleDefinition puzzle)
        {
            if (puzzle == null)
            {
                Debug.LogWarning("Puzzle open ignored because definition is missing.");
                return;
            }

            activePuzzle = puzzle;

            if (puzzleUI == null)
            {
                Debug.LogWarning($"Cannot open puzzle '{puzzle.PuzzleId}' because PuzzleUI is missing.");
                return;
            }

            puzzleUI.Show(puzzle);
        }

        public bool TrySubmitAnswer(string answer)
        {
            if (activePuzzle == null)
            {
                return false;
            }

            bool isCorrect = IsCorrectAnswer(activePuzzle, answer);
            if (isCorrect)
            {
                GrantRewards(activePuzzle);
                Debug.Log($"Puzzle '{activePuzzle.PuzzleId}' solved. Success event: {activePuzzle.SuccessEventId}");
                soundRuntimeSystem?.PlayPuzzleSuccess();
                eventRuntimeSystem?.ExecuteEvent(activePuzzle.SuccessEventId);
                puzzleUI?.ShowFeedback("Solved");
                puzzleUI?.Hide();
                activePuzzle = null;
                return true;
            }

            Debug.Log($"Puzzle '{activePuzzle.PuzzleId}' failed. Failure event: {activePuzzle.FailureEventId}");
            soundRuntimeSystem?.PlayPuzzleError();
            eventRuntimeSystem?.ExecuteEvent(activePuzzle.FailureEventId);
            puzzleUI?.ShowFeedback("Wrong");
            return false;
        }

        private void HandleAnswerSubmitted(string answer)
        {
            TrySubmitAnswer(answer);
        }

        private bool IsCorrectAnswer(PuzzleDefinition puzzle, string answer)
        {
            if (puzzle.InputType == PuzzleInputType.ItemUse && inventorySystem != null && inventorySystem.IsSelected(puzzle.Answer))
            {
                return true;
            }

            return Normalize(answer) == Normalize(puzzle.Answer);
        }

        private void GrantRewards(PuzzleDefinition puzzle)
        {
            if (inventorySystem == null)
            {
                return;
            }

            foreach (ItemDefinition reward in puzzle.RewardItems)
            {
                if (reward != null)
                {
                    inventorySystem.AddItem(reward);
                }
            }
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToLowerInvariant().Replace(" ", string.Empty);
        }
    }
}
