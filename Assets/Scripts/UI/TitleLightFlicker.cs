// -----------------------------------------------------------------------------
// Codex comment pass: Title Light Flicker
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\TitleLightFlicker.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Title Light Flicker UI elements, keeping references cached and visuals synchronized.
    public class TitleLightFlicker : MonoBehaviour
    {
        [SerializeField] private Image targetOverlay;
        [SerializeField, Range(0f, 1f)] private float baseAlpha = 0.28f;
        [SerializeField, Range(0f, 1f)] private float minAlpha = 0.22f;
        [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.38f;
        [SerializeField, Range(0.02f, 1f)] private float minInterval = 0.08f;
        [SerializeField, Range(0.02f, 1.5f)] private float maxInterval = 0.45f;
        [SerializeField, Range(0f, 1f)] private float rareDarkPulseChance = 0.08f;
        [SerializeField, Range(0f, 1f)] private float rareDarkPulseAlpha = 0.42f;
        [SerializeField, Range(0.1f, 20f)] private float smoothSpeed = 7f;

        // Stores the current Alpha value used by this script's runtime or editor workflow.
        private float currentAlpha;
        // Stores the target Alpha value used by this script's runtime or editor workflow.
        private float targetAlpha;
        // Stores the next Sample Time value used by this script's runtime or editor workflow.
        private float nextSampleTime;

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            if (targetOverlay == null)
            {
                targetOverlay = GetComponent<Image>();
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            currentAlpha = baseAlpha;
            targetAlpha = baseAlpha;
            nextSampleTime = Time.unscaledTime;
            ApplyAlpha(currentAlpha);
        }

        // Refreshes frame-dependent input, timers, animation, or visual state.
        private void Update()
        {
            if (targetOverlay == null)
            {
                return;
            }

            float now = Time.unscaledTime;
            if (now >= nextSampleTime)
            {
                SampleNextTarget(now);
            }

            float delta = Time.unscaledDeltaTime * smoothSpeed;
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, delta);
            ApplyAlpha(currentAlpha);
        }

        // Performs the Sample Next Target operation while keeping its implementation details inside this script.
        private void SampleNextTarget(float now)
        {
            targetAlpha = Random.value < rareDarkPulseChance
                ? rareDarkPulseAlpha
                : Random.Range(minAlpha, maxAlpha);

            nextSampleTime = now + Random.Range(minInterval, maxInterval);
        }

        // Applies calculated settings to Unity components or runtime state.
        private void ApplyAlpha(float alpha)
        {
            if (targetOverlay == null)
            {
                return;
            }

            targetOverlay.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha));
        }

        // Keeps Inspector-edited values and cached references valid while working in the editor.
        private void OnValidate()
        {
            if (maxAlpha < minAlpha)
            {
                maxAlpha = minAlpha;
            }

            if (maxInterval < minInterval)
            {
                maxInterval = minInterval;
            }

            rareDarkPulseAlpha = Mathf.Max(rareDarkPulseAlpha, maxAlpha);
        }
    }
}
