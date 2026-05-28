using System;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class ChaseManager : Singleton<ChaseManager>
    {
        [SerializeField] private int maxMovesBeforeCatch = 3;
        [SerializeField] private bool applySettingsOnStart = true;
        [SerializeField] private bool resetMoveCountOnStart = true;

        private bool isChasing;
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

        private void Start()
        {
            if (applySettingsOnStart)
            {
                ApplySettings();
            }
        }

        public void StartChase()
        {
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

        public void ResetChase()
        {
            isChasing = false;
            moveCountDuringChase = 0;
            RaiseMoveCountChanged();
        }

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

        private void RaiseMoveCountChanged()
        {
            if (MoveCountChanged != null)
            {
                MoveCountChanged(moveCountDuringChase);
            }
        }
    }
}
