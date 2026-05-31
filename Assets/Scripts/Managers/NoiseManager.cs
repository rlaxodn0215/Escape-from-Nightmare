// -----------------------------------------------------------------------------
// Codex comment pass: Noise Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\NoiseManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Noise Manager system, keeping shared state and events behind one access point.
    public class NoiseManager : Singleton<NoiseManager>
    {
        [SerializeField] private bool noiseEnabled = true;
        [SerializeField] private float globalNoiseCooldownSeconds = 0.25f;

        // Stores the last Noise Time value used by this script's runtime or editor workflow.
        private float lastNoiseTime = -999f;

        public event Action<string, string> NoiseMade;

        // Performs the Make Noise operation while keeping its implementation details inside this script.
        public void MakeNoise(string locationId, string sourceId)
        {
            if (!noiseEnabled)
            {
                return;
            }

            if (!CanMakeNoise())
            {
                Debug.Log("Noise ignored because global cooldown is active.");
                return;
            }

            string resolvedLocationId = ResolveLocationId(locationId);
            if (string.IsNullOrEmpty(resolvedLocationId))
            {
                Debug.LogWarning("Cannot make noise because no locationId could be resolved.");
                return;
            }

            string resolvedSourceId = !string.IsNullOrEmpty(sourceId) ? sourceId : "UnknownSource";
            lastNoiseTime = Time.time;

            if (NoiseMade != null)
            {
                NoiseMade(resolvedLocationId, resolvedSourceId);
            }

            Debug.Log("Noise made at " + resolvedLocationId + " by " + resolvedSourceId);

            if (GhostManager.Instance == null)
            {
                Debug.LogWarning("GhostManager instance is missing.");
                return;
            }

            GhostManager.Instance.ReactToNoise(resolvedLocationId);
        }

        // Performs the Make Noise At Current Location operation while keeping its implementation details inside this script.
        public void MakeNoiseAtCurrentLocation(string sourceId)
        {
            if (LocationManager.Instance != null)
            {
                MakeNoise(LocationManager.Instance.CurrentLocationId, sourceId);
                return;
            }

            MakeNoise(string.Empty, sourceId);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool CanMakeNoise()
        {
            if (globalNoiseCooldownSeconds <= 0f)
            {
                return true;
            }

            return Time.time - lastNoiseTime >= globalNoiseCooldownSeconds;
        }

        // Performs the Resolve Location Id operation while keeping its implementation details inside this script.
        private string ResolveLocationId(string locationId)
        {
            if (!string.IsNullOrEmpty(locationId))
            {
                return locationId;
            }

            if (LocationManager.Instance != null)
            {
                return LocationManager.Instance.CurrentLocationId;
            }

            return string.Empty;
        }
    }
}
