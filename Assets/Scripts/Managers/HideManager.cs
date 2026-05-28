using System;
using System.Collections;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class HideManager : Singleton<HideManager>
    {
        [SerializeField] private float minHideSeconds = 5f;
        [SerializeField] private float maxHideSeconds = 20f;
        [SerializeField] private bool applySettingsOnStart = true;
        [SerializeField] private bool allowExitBeforeSafe = false;
        [SerializeField] private bool failChaseWhenExitEarly = true;

        private bool isHiding;
        private bool canExitSafely;
        private string currentHidePointId;
        private Coroutine hideRoutine;

        public bool IsHiding
        {
            get { return isHiding; }
        }

        public bool CanExitSafely
        {
            get { return canExitSafely; }
        }

        public string CurrentHidePointId
        {
            get { return currentHidePointId; }
        }

        public event Action<string> HideEntered;
        public event Action<string> HideExited;
        public event Action<string> HideBecameSafe;

        private void Start()
        {
            if (applySettingsOnStart)
            {
                ApplySettings();
            }
        }

        public void EnterHidePoint(string hidePointId)
        {
            string resolvedHidePointId = !string.IsNullOrEmpty(hidePointId) ? hidePointId : "UnknownHidePoint";

            if (isHiding)
            {
                Debug.Log("Already hiding at: " + currentHidePointId);
                return;
            }

            currentHidePointId = resolvedHidePointId;
            canExitSafely = false;
            SetHidingState(true);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Hiding);
            }

            if (HideEntered != null)
            {
                HideEntered(currentHidePointId);
            }

            Debug.Log("Entered hide point: " + currentHidePointId);

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
            }

            hideRoutine = StartCoroutine(WaitForGhostToLeave());
        }

        public void ExitHidePoint()
        {
            if (!isHiding)
            {
                return;
            }

            if (!canExitSafely && !allowExitBeforeSafe)
            {
                Debug.Log("Cannot exit hide point safely yet: " + currentHidePointId);

                if (failChaseWhenExitEarly && ChaseManager.Instance != null && ChaseManager.Instance.IsChasing)
                {
                    ChaseManager.Instance.FailChase();
                }

                return;
            }

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            ForceExitHidePoint();
        }

        public IEnumerator WaitForGhostToLeave()
        {
            yield return new WaitForSeconds(GetHideWaitSeconds());

            if (GhostManager.Instance != null && GhostManager.Instance.IsGhostThreateningCurrentLocation())
            {
                GhostManager.Instance.LeaveCurrentLocation();
            }

            canExitSafely = true;

            if (HideBecameSafe != null)
            {
                HideBecameSafe(currentHidePointId);
            }

            if (ChaseManager.Instance != null && ChaseManager.Instance.IsChasing)
            {
                ChaseManager.Instance.EndChaseByHideSuccess();
            }

            hideRoutine = null;
            Debug.Log("Hide point is now safe to exit: " + currentHidePointId);
        }

        public void ForceExitHidePoint()
        {
            string exitedHidePointId = currentHidePointId;
            SetHidingState(false);
            canExitSafely = false;
            currentHidePointId = string.Empty;

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Hiding)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }

            if (HideExited != null)
            {
                HideExited(exitedHidePointId);
            }

            Debug.Log("Exited hide point: " + exitedHidePointId);
        }

        private void ApplySettings()
        {
            if (GameDataManager.Instance == null || GameDataManager.Instance.Settings == null)
            {
                return;
            }

            GameSettingsRecord settings = GameDataManager.Instance.Settings;
            if (settings.hideMinSeconds > 0f)
            {
                minHideSeconds = settings.hideMinSeconds;
            }

            if (settings.hideMaxSeconds > 0f)
            {
                maxHideSeconds = settings.hideMaxSeconds;
            }
        }

        private float GetHideWaitSeconds()
        {
            float min = Mathf.Max(0f, minHideSeconds);
            float max = Mathf.Max(0f, maxHideSeconds);

            if (max < min)
            {
                float temp = min;
                min = max;
                max = temp;
            }

            if (Mathf.Approximately(min, max))
            {
                return min;
            }

            return UnityEngine.Random.Range(min, max);
        }

        private bool IsGhostThreatNearby()
        {
            return GhostManager.Instance != null && GhostManager.Instance.IsGhostThreateningCurrentLocation();
        }

        private void SetHidingState(bool value)
        {
            isHiding = value;
        }
    }
}
