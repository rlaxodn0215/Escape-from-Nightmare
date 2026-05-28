using System;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class NoiseManager : Singleton<NoiseManager>
    {
        [SerializeField] private bool noiseEnabled = true;
        [SerializeField] private float globalNoiseCooldownSeconds = 0.25f;

        private float lastNoiseTime = -999f;

        public event Action<string, string> NoiseMade;

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

        public void MakeNoiseAtCurrentLocation(string sourceId)
        {
            if (LocationManager.Instance != null)
            {
                MakeNoise(LocationManager.Instance.CurrentLocationId, sourceId);
                return;
            }

            MakeNoise(string.Empty, sourceId);
        }

        private bool CanMakeNoise()
        {
            if (globalNoiseCooldownSeconds <= 0f)
            {
                return true;
            }

            return Time.time - lastNoiseTime >= globalNoiseCooldownSeconds;
        }

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
