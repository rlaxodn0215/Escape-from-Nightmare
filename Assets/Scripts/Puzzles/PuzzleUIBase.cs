// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle UI Base
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleUIBase.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Puzzle UI Base screen, translating UI input into puzzle progress and completion.
    public class PuzzleUIBase : MonoBehaviour
    {
        [SerializeField] protected string puzzleId;
        // Stores the puzzle Record value used by this script's runtime or editor workflow.
        protected PuzzleRecord puzzleRecord;
        // Stores the failed Attempt Count value used by this script's runtime or editor workflow.
        protected int failedAttemptCount;

        public string PuzzleId
        {
            get { return puzzleId; }
        }

        public PuzzleRecord PuzzleRecord
        {
            get { return puzzleRecord; }
        }

        // Initializes local UI and state from an external record before the player can interact with it.
        public virtual void Initialize(PuzzleRecord record)
        {
            puzzleRecord = record;

            if (record == null)
            {
                Debug.LogWarning("Cannot initialize puzzle UI with a null PuzzleRecord.", this);
                return;
            }

            puzzleId = record.puzzleId;
            failedAttemptCount = 0;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Puzzle);
            }
        }

        // Performs the Register Failure operation while keeping its implementation details inside this script.
        protected virtual void RegisterFailure()
        {
            failedAttemptCount++;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUi(AudioCue.UiFail);
            }

            Debug.Log("Puzzle failed. Puzzle: " + puzzleId + ", Failed attempts: " + failedAttemptCount);
            TriggerNoiseIfNeeded();
        }

        // Performs the Trigger Noise If Needed operation while keeping its implementation details inside this script.
        protected virtual void TriggerNoiseIfNeeded()
        {
            if (puzzleRecord == null || puzzleRecord.failCountToNoise <= 0 || failedAttemptCount < puzzleRecord.failCountToNoise)
            {
                return;
            }

            if (NoiseManager.Instance != null)
            {
                string sourceId = !string.IsNullOrEmpty(puzzleId) ? puzzleId : gameObject.name;
                NoiseManager.Instance.MakeNoise(GetCurrentLocationId(), sourceId);
            }
            else
            {
                Debug.LogWarning("NoiseManager instance is missing.");
            }

            failedAttemptCount = 0;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        protected virtual string GetCurrentLocationId()
        {
            if (LocationManager.Instance != null)
            {
                return LocationManager.Instance.CurrentLocationId;
            }

            return puzzleRecord != null ? puzzleRecord.locationId : string.Empty;
        }

        // Performs the Complete operation while keeping its implementation details inside this script.
        protected virtual void Complete()
        {
            if (PuzzleManager.Instance == null)
            {
                Debug.LogWarning("PuzzleManager instance is missing.");
                return;
            }

            PuzzleManager.Instance.CompletePuzzle(puzzleId);
        }

        // Closes the active UI or interaction and returns control to the normal game flow.
        public virtual void Close()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }

            if (PuzzleManager.Instance != null)
            {
                PuzzleManager.Instance.CloseCurrentPuzzle();
                return;
            }

            Debug.LogWarning("PuzzleManager instance is missing.");
            gameObject.SetActive(false);
        }

        // Performs the Complete From Button operation while keeping its implementation details inside this script.
        public void CompleteFromButton()
        {
            Complete();
        }
    }
}
