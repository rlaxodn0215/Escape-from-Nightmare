// -----------------------------------------------------------------------------
// Codex comment pass: Hide Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\HideManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Hide Manager system, keeping shared state and events behind one access point.
    public class HideManager : Singleton<HideManager>
    {
        [SerializeField] private float minHideSeconds = 5f;
        [SerializeField] private float maxHideSeconds = 20f;
        [SerializeField] private bool applySettingsOnStart = true;
        [SerializeField] private bool allowExitBeforeSafe = false;
        [SerializeField] private bool failChaseWhenExitEarly = true;

        // Stores the is Hiding value used by this script's runtime or editor workflow.
        private bool isHiding;
        // Stores the can Exit Safely value used by this script's runtime or editor workflow.
        private bool canExitSafely;
        // Stores the current Hide Point Id value used by this script's runtime or editor workflow.
        private string currentHidePointId;
        private string returnLocationId;
        private string returnViewId;
        private bool hasReturnPosition;
        // Stores the hide Routine value used by this script's runtime or editor workflow.
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

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (applySettingsOnStart)
            {
                ApplySettings();
            }
        }

        // Performs the Enter Hide Point operation while keeping its implementation details inside this script.
        public void EnterHidePoint(string hidePointId)
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.DisableHiding)
            {
                Debug.Log("Hide point entry is disabled for layout testing: " + hidePointId);
                return;
            }

            string resolvedHidePointId = !string.IsNullOrEmpty(hidePointId) ? hidePointId : "UnknownHidePoint";

            if (isHiding)
            {
                Debug.Log("Already hiding at: " + currentHidePointId);
                return;
            }

            CaptureReturnPosition();

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

        // Performs the Exit Hide Point operation while keeping its implementation details inside this script.
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

        // Performs the Wait For Ghost To Leave operation while keeping its implementation details inside this script.
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

        // Performs the Force Exit Hide Point operation while keeping its implementation details inside this script.
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

            RestoreReturnPosition();

            Debug.Log("Exited hide point: " + exitedHidePointId);
        }

        private void CaptureReturnPosition()
        {
            if (LocationManager.Instance == null)
            {
                hasReturnPosition = false;
                returnLocationId = string.Empty;
                returnViewId = string.Empty;
                return;
            }

            returnLocationId = LocationManager.Instance.CurrentLocationId;
            returnViewId = LocationManager.Instance.CurrentViewId;
            hasReturnPosition = !string.IsNullOrEmpty(returnLocationId);
        }

        private void RestoreReturnPosition()
        {
            if (!hasReturnPosition)
            {
                return;
            }

            string targetLocationId = returnLocationId;
            string targetViewId = returnViewId;
            hasReturnPosition = false;
            returnLocationId = string.Empty;
            returnViewId = string.Empty;

            if (LocationManager.Instance == null)
            {
                return;
            }

            if (LocationManager.Instance.CurrentLocationId == targetLocationId && LocationManager.Instance.CurrentViewId == targetViewId)
            {
                return;
            }

            LocationManager.Instance.SetLocation(targetLocationId, targetViewId);
        }

        // Applies calculated settings to Unity components or runtime state.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsGhostThreatNearby()
        {
            return GhostManager.Instance != null && GhostManager.Instance.IsGhostThreateningCurrentLocation();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetHidingState(bool value)
        {
            isHiding = value;
        }
    }
}
