// -----------------------------------------------------------------------------
// Codex comment pass: Chase Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\ChaseManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Chase Manager system, keeping shared state and events behind one access point.
    public class ChaseManager : Singleton<ChaseManager>
    {
        [SerializeField] private int maxMovesBeforeCatch = 3;
        [SerializeField] private bool applySettingsOnStart = true;
        [SerializeField] private bool resetMoveCountOnStart = true;

        // Stores the is Chasing value used by this script's runtime or editor workflow.
        private bool isChasing;
        // Stores the move Count During Chase value used by this script's runtime or editor workflow.
        private int moveCountDuringChase;

        public bool IsChasing
        {
            get { return isChasing; }
        }

        public int MoveCountDuringChase
        {
            get { return moveCountDuringChase; }
        }

        public int MaxMovesBeforeCatch
        {
            get { return maxMovesBeforeCatch; }
        }

        public event Action ChaseStarted;
        public event Action ChaseEnded;
        public event Action ChaseFailed;
        public event Action<int> MoveCountChanged;

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (applySettingsOnStart)
            {
                ApplySettings();
            }
        }

        // Begins this system's runtime flow and initializes any timers, events, or counters it needs.
        public void StartChase()
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.DisableGhost)
            {
                Debug.Log("Chase start ignored because ghost is disabled for layout testing.");
                return;
            }

            if (isChasing)
            {
                return;
            }

            isChasing = true;
            if (resetMoveCountOnStart)
            {
                moveCountDuringChase = 0;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Chase);
            }

            if (ChaseStarted != null)
            {
                ChaseStarted();
            }

            RaiseMoveCountChanged();
            Debug.Log("Chase started.");
        }

        // Performs the Register Move operation while keeping its implementation details inside this script.
        public void RegisterMove()
        {
            if (!isChasing)
            {
                return;
            }

            if (HideManager.Instance != null && HideManager.Instance.IsHiding)
            {
                return;
            }

            moveCountDuringChase++;
            RaiseMoveCountChanged();
            Debug.Log("Chase move registered: " + moveCountDuringChase + "/" + maxMovesBeforeCatch);

            if (maxMovesBeforeCatch > 0 && moveCountDuringChase >= maxMovesBeforeCatch)
            {
                FailChase();
            }
        }

        // Performs the End Chase By Hide Success operation while keeping its implementation details inside this script.
        public void EndChaseByHideSuccess()
        {
            if (!isChasing)
            {
                return;
            }

            isChasing = false;
            moveCountDuringChase = 0;

            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.StopChase();
            }

            if (GameManager.Instance != null)
            {
                if (HideManager.Instance != null && HideManager.Instance.IsHiding)
                {
                    GameManager.Instance.SetState(GameState.Hiding);
                }
                else
                {
                    GameManager.Instance.SetState(GameState.Playing);
                }
            }

            if (ChaseEnded != null)
            {
                ChaseEnded();
            }

            RaiseMoveCountChanged();
            Debug.Log("Chase ended by successful hiding.");
        }

        // Performs the Fail Chase operation while keeping its implementation details inside this script.
        public void FailChase()
        {
            isChasing = false;
            moveCountDuringChase = 0;

            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.StopChase();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }

            if (ChaseFailed != null)
            {
                ChaseFailed();
            }

            RaiseMoveCountChanged();
            Debug.Log("Chase failed.");
        }

        // Returns runtime state to its defaults for a new game, retry, or clean test run.
        public void ResetChase()
        {
            isChasing = false;
            moveCountDuringChase = 0;
            RaiseMoveCountChanged();
        }

        // Applies calculated settings to Unity components or runtime state.
        private void ApplySettings()
        {
            if (GameDataManager.Instance == null || GameDataManager.Instance.Settings == null)
            {
                return;
            }

            if (GameDataManager.Instance.Settings.chaseMoveLimit > 0)
            {
                maxMovesBeforeCatch = GameDataManager.Instance.Settings.chaseMoveLimit;
            }
        }

        // Performs the Raise Move Count Changed operation while keeping its implementation details inside this script.
        private void RaiseMoveCountChanged()
        {
            if (MoveCountChanged != null)
            {
                MoveCountChanged(moveCountDuringChase);
            }
        }
    }
}
