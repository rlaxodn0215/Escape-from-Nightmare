// -----------------------------------------------------------------------------
// Codex comment pass: Ghost Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\GhostManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Ghost Manager system, keeping shared state and events behind one access point.
    public class GhostManager : Singleton<GhostManager>
    {
        [SerializeField] private bool autoStartPatrolOnStart = true;
        [SerializeField] private float defaultMinArrivalTime = 5f;
        [SerializeField] private float defaultMaxArrivalTime = 12f;
        [SerializeField] private float defaultMinLeaveTime = 5f;
        [SerializeField] private float defaultMaxLeaveTime = 20f;
        [SerializeField] private float defaultDangerIncreasePerSecond = 0.25f;
        [SerializeField] private float dangerThreshold = 1f;
        [SerializeField] private bool resetDangerWhenGhostLeaves = true;
        [SerializeField] private bool ignoreNoiseWhileChasing = true;

        // Stores the runtime State value used by this script's runtime or editor workflow.
        private GhostRuntimeState runtimeState = GhostRuntimeState.Inactive;
        // Stores the current Ghost Location Id value used by this script's runtime or editor workflow.
        private string currentGhostLocationId;
        // Stores the target Noise Location Id value used by this script's runtime or editor workflow.
        private string targetNoiseLocationId;
        // Stores the danger Level value used by this script's runtime or editor workflow.
        private float dangerLevel;
        // Stores the noise Response Routine value used by this script's runtime or editor workflow.
        private Coroutine noiseResponseRoutine;

        public GhostRuntimeState RuntimeState
        {
            get { return runtimeState; }
        }

        public string CurrentGhostLocationId
        {
            get { return currentGhostLocationId; }
        }

        public string TargetNoiseLocationId
        {
            get { return targetNoiseLocationId; }
        }

        public float DangerLevel
        {
            get { return dangerLevel; }
        }

        public float DangerThreshold
        {
            get { return dangerThreshold; }
        }

        public bool IsGhostPresent
        {
            get { return !string.IsNullOrEmpty(currentGhostLocationId); }
        }

        public bool IsChasing
        {
            get { return runtimeState == GhostRuntimeState.Chasing; }
        }

        public event Action<GhostRuntimeState> StateChanged;
        public event Action<string> GhostArrived;
        public event Action<string> GhostLeft;
        public event Action<float> DangerChanged;
        public event Action ChaseStarted;
        public event Action ChaseStopped;

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            ApplySettings();

            if (autoStartPatrolOnStart)
            {
                StartPatrol();
            }
        }

        // Refreshes frame-dependent input, timers, animation, or visual state.
        private void Update()
        {
            if (runtimeState == GhostRuntimeState.SearchingLocation || runtimeState == GhostRuntimeState.Chasing)
            {
                UpdateDanger();
            }
        }

        // Begins this system's runtime flow and initializes any timers, events, or counters it needs.
        public void StartPatrol()
        {
            targetNoiseLocationId = string.Empty;
            ResetDanger();
            SetRuntimeState(GhostRuntimeState.Patrolling);
            Debug.Log("Ghost patrol started.");
        }

        // Stops an active routine or state so the next run can start cleanly.
        public void StopPatrol()
        {
            if (noiseResponseRoutine != null)
            {
                StopCoroutine(noiseResponseRoutine);
                noiseResponseRoutine = null;
            }

            targetNoiseLocationId = string.Empty;
            SetRuntimeState(GhostRuntimeState.Inactive);
            Debug.Log("Ghost patrol stopped.");
        }

        // Performs the React To Noise operation while keeping its implementation details inside this script.
        public void ReactToNoise(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                Debug.LogWarning("Ghost cannot react to noise without a locationId.");
                return;
            }

            if (ignoreNoiseWhileChasing && runtimeState == GhostRuntimeState.Chasing)
            {
                Debug.Log("Ghost ignored noise while chasing: " + locationId);
                return;
            }

            if (noiseResponseRoutine != null)
            {
                StopCoroutine(noiseResponseRoutine);
            }

            targetNoiseLocationId = locationId;
            noiseResponseRoutine = StartCoroutine(NoiseResponseRoutine(locationId));
            SetRuntimeState(GhostRuntimeState.RespondingToNoise);
            Debug.Log("Ghost reacts to noise at location: " + locationId);
        }

        // Performs the Enter Location operation while keeping its implementation details inside this script.
        public void EnterLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                Debug.LogWarning("Ghost cannot enter an empty locationId.");
                return;
            }

            currentGhostLocationId = locationId;
            SetRuntimeState(GhostRuntimeState.SearchingLocation);

            if (GhostArrived != null)
            {
                GhostArrived(locationId);
            }

            Debug.Log("Ghost enters location: " + locationId);
        }

        // Performs the Leave Current Location operation while keeping its implementation details inside this script.
        public void LeaveCurrentLocation()
        {
            string previousLocationId = currentGhostLocationId;
            SetRuntimeState(GhostRuntimeState.LeavingLocation);

            currentGhostLocationId = string.Empty;
            targetNoiseLocationId = string.Empty;

            if (resetDangerWhenGhostLeaves)
            {
                ResetDanger();
            }

            SetRuntimeState(GhostRuntimeState.Patrolling);

            if (GhostLeft != null)
            {
                GhostLeft(previousLocationId);
            }

            Debug.Log("Ghost leaves current location: " + previousLocationId);
        }

        // Begins this system's runtime flow and initializes any timers, events, or counters it needs.
        public void StartChase()
        {
            if (runtimeState == GhostRuntimeState.Chasing)
            {
                return;
            }

            SetRuntimeState(GhostRuntimeState.Chasing);
            dangerLevel = dangerThreshold;
            RaiseDangerChanged();

            if (ChaseStarted != null)
            {
                ChaseStarted();
            }

            if (ChaseManager.Instance != null)
            {
                ChaseManager.Instance.StartChase();
            }
            else
            {
                Debug.LogWarning("ChaseManager instance is missing.");
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Chase);
            }

            Debug.Log("Ghost chase started.");
        }

        // Stops an active routine or state so the next run can start cleanly.
        public void StopChase()
        {
            if (noiseResponseRoutine != null)
            {
                StopCoroutine(noiseResponseRoutine);
                noiseResponseRoutine = null;
            }

            ResetDanger();
            currentGhostLocationId = string.Empty;
            targetNoiseLocationId = string.Empty;

            if (ChaseStopped != null)
            {
                ChaseStopped();
            }

            SetRuntimeState(GhostRuntimeState.Patrolling);
            Debug.Log("Ghost chase stopped.");
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsGhostInLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                return false;
            }

            return currentGhostLocationId == locationId;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsGhostThreateningCurrentLocation()
        {
            if (LocationManager.Instance == null)
            {
                return false;
            }

            return IsGhostInLocation(LocationManager.Instance.CurrentLocationId);
        }

        // Returns runtime state to its defaults for a new game, retry, or clean test run.
        public void ResetDanger()
        {
            dangerLevel = 0f;
            RaiseDangerChanged();
        }

        // Performs the Noise Response Routine operation while keeping its implementation details inside this script.
        private IEnumerator NoiseResponseRoutine(string locationId)
        {
            GhostRuleRecord rule = GetRuleForLocation(locationId);
            yield return new WaitForSeconds(GetArrivalDelay(rule));

            EnterLocation(locationId);
            yield return new WaitForSeconds(GetLeaveDelay(rule));

            if (runtimeState != GhostRuntimeState.Chasing)
            {
                LeaveCurrentLocation();
            }

            noiseResponseRoutine = null;
        }

        // Performs the Update Danger operation while keeping its implementation details inside this script.
        private void UpdateDanger()
        {
            if (runtimeState == GhostRuntimeState.Chasing)
            {
                return;
            }

            if (string.IsNullOrEmpty(currentGhostLocationId) || LocationManager.Instance == null)
            {
                return;
            }

            if (LocationManager.Instance.CurrentLocationId != currentGhostLocationId)
            {
                return;
            }

            if (HideManager.Instance != null && HideManager.Instance.IsHiding)
            {
                return;
            }

            GhostRuleRecord rule = GetRuleForLocation(currentGhostLocationId);
            float previousDanger = dangerLevel;
            dangerLevel += GetDangerIncreasePerSecond(rule) * Time.deltaTime;

            if (!Mathf.Approximately(previousDanger, dangerLevel))
            {
                RaiseDangerChanged();
            }

            if (dangerLevel >= dangerThreshold)
            {
                StartChase();
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetRuntimeState(GhostRuntimeState newState)
        {
            if (runtimeState == newState)
            {
                return;
            }

            runtimeState = newState;

            if (StateChanged != null)
            {
                StateChanged(runtimeState);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private GhostRuleRecord GetRuleForLocation(string locationId)
        {
            if (GameDataManager.Instance == null)
            {
                return null;
            }

            return GameDataManager.Instance.GetGhostRuleForLocation(locationId);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private float GetArrivalDelay(GhostRuleRecord rule)
        {
            float min = rule != null && rule.minArrivalTime > 0f ? rule.minArrivalTime : defaultMinArrivalTime;
            float max = rule != null && rule.maxArrivalTime > 0f ? rule.maxArrivalTime : defaultMaxArrivalTime;
            return RandomRangeSafe(min, max);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private float GetLeaveDelay(GhostRuleRecord rule)
        {
            float min = rule != null && rule.minLeaveTime > 0f ? rule.minLeaveTime : defaultMinLeaveTime;
            float max = rule != null && rule.maxLeaveTime > 0f ? rule.maxLeaveTime : defaultMaxLeaveTime;
            return RandomRangeSafe(min, max);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private float GetDangerIncreasePerSecond(GhostRuleRecord rule)
        {
            if (rule != null && rule.dangerIncreasePerSecond > 0f)
            {
                return rule.dangerIncreasePerSecond;
            }

            return defaultDangerIncreasePerSecond;
        }

        // Applies calculated settings to Unity components or runtime state.
        private void ApplySettings()
        {
            if (GameDataManager.Instance == null || GameDataManager.Instance.Settings == null)
            {
                return;
            }

            GameSettingsRecord settings = GameDataManager.Instance.Settings;
            if (settings.ghostDefaultMinArrivalTime > 0f)
            {
                defaultMinArrivalTime = settings.ghostDefaultMinArrivalTime;
            }

            if (settings.ghostDefaultMaxArrivalTime > 0f)
            {
                defaultMaxArrivalTime = settings.ghostDefaultMaxArrivalTime;
            }

            if (settings.ghostDefaultMinLeaveTime > 0f)
            {
                defaultMinLeaveTime = settings.ghostDefaultMinLeaveTime;
            }

            if (settings.ghostDefaultMaxLeaveTime > 0f)
            {
                defaultMaxLeaveTime = settings.ghostDefaultMaxLeaveTime;
            }

            if (settings.ghostDangerThreshold > 0f)
            {
                dangerThreshold = settings.ghostDangerThreshold;
            }
        }

        // Performs the Random Range Safe operation while keeping its implementation details inside this script.
        private float RandomRangeSafe(float min, float max)
        {
            min = Mathf.Max(0f, min);
            max = Mathf.Max(0f, max);

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

        // Performs the Raise Danger Changed operation while keeping its implementation details inside this script.
        private void RaiseDangerChanged()
        {
            if (DangerChanged != null)
            {
                DangerChanged(dangerLevel);
            }
        }
    }
}
