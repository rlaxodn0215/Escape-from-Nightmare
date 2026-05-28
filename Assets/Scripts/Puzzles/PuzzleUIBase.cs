using UnityEngine;

namespace EscapeFromNightmare
{
    public class PuzzleUIBase : MonoBehaviour
    {
        [SerializeField] protected string puzzleId;
        protected PuzzleRecord puzzleRecord;
        protected int failedAttemptCount;

        public string PuzzleId
        {
            get { return puzzleId; }
        }

        public PuzzleRecord PuzzleRecord
        {
            get { return puzzleRecord; }
        }

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

        protected virtual void RegisterFailure()
        {
            failedAttemptCount++;
            Debug.Log("Puzzle failed. Puzzle: " + puzzleId + ", Failed attempts: " + failedAttemptCount);
            TriggerNoiseIfNeeded();
        }

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

        protected virtual string GetCurrentLocationId()
        {
            if (LocationManager.Instance != null)
            {
                return LocationManager.Instance.CurrentLocationId;
            }

            return puzzleRecord != null ? puzzleRecord.locationId : string.Empty;
        }

        protected virtual void Complete()
        {
            if (PuzzleManager.Instance == null)
            {
                Debug.LogWarning("PuzzleManager instance is missing.");
                return;
            }

            PuzzleManager.Instance.CompletePuzzle(puzzleId);
        }

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

        public void CompleteFromButton()
        {
            Complete();
        }
    }
}
