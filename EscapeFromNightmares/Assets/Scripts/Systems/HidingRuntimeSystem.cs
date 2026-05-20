using System;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.UI;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class HidingRuntimeSystem : MonoBehaviour
    {
        private const string PlayerCapturedEventId = "event_player_captured";

        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private HidingGaugeUI hidingGaugeUI;
        [SerializeField] private EventRuntimeSystem eventRuntimeSystem;
        [SerializeField] private SoundRuntimeSystem soundRuntimeSystem;
        [SerializeField, Min(0.1f)] private float defaultHideSeconds = 6f;
        [SerializeField, Min(0f)] private float captureDecayPerSecond = 0.25f;

        private bool isHiding;
        private float hideTimer;
        private float hideDuration;
        private float captureGauge01;
        private string activeHideSpotId;

        public bool IsHiding => isHiding;
        public float CaptureGauge01 => captureGauge01;
        public string ActiveHideSpotId => activeHideSpotId;
        public event Action<string> HidingCompleted;

        private void Awake()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            hidingGaugeUI ??= FindFirstObjectByType<HidingGaugeUI>(FindObjectsInactive.Include);
            eventRuntimeSystem ??= FindFirstObjectByType<EventRuntimeSystem>();
            soundRuntimeSystem ??= FindFirstObjectByType<SoundRuntimeSystem>();
            hidingGaugeUI?.Hide();
        }

        private void OnEnable()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted += ResetRuntimeState;
            }
        }

        private void OnDisable()
        {
            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted -= ResetRuntimeState;
            }
        }

        private void Update()
        {
            if (!isHiding)
            {
                return;
            }

            hideTimer += Time.deltaTime;
            captureGauge01 = Mathf.Max(0f, captureGauge01 - captureDecayPerSecond * Time.deltaTime);
            UpdateGauge();

            if (hideTimer >= hideDuration && captureGauge01 <= 0f)
            {
                StopHiding(true);
            }
        }

        public void EnterHideSpot(string hideSpotId, float recommendedHoldSeconds = 0f)
        {
            isHiding = true;
            hideTimer = 0f;
            hideDuration = recommendedHoldSeconds > 0f ? recommendedHoldSeconds : defaultHideSeconds;
            activeHideSpotId = string.IsNullOrWhiteSpace(hideSpotId) ? "hide_spot" : hideSpotId;
            hidingGaugeUI?.Show();
            hidingGaugeUI?.SetLabel("Hiding");
            soundRuntimeSystem?.PlaySound("monster_breath_far");
            UpdateGauge();
        }

        public void StopHiding()
        {
            StopHiding(false);
        }

        private void StopHiding(bool completed)
        {
            string completedHideSpotId = activeHideSpotId;
            isHiding = false;
            activeHideSpotId = string.Empty;
            hidingGaugeUI?.SetDanger01(0f);
            hidingGaugeUI?.Hide();

            if (completed)
            {
                HidingCompleted?.Invoke(completedHideSpotId);
            }
        }

        public void AddCapturePressure(float amount01)
        {
            captureGauge01 = Mathf.Clamp01(captureGauge01 + amount01);
            UpdateGauge();

            if (captureGauge01 >= 1f)
            {
                eventRuntimeSystem?.ExecuteEvent(PlayerCapturedEventId);
            }
            else if (!isHiding)
            {
                hidingGaugeUI?.Show();
            }
        }

        public void SetCaptureGauge01(float value)
        {
            captureGauge01 = Mathf.Clamp01(value);
            UpdateGauge();
        }

        public void ResetRuntimeState()
        {
            isHiding = false;
            hideTimer = 0f;
            hideDuration = 0f;
            captureGauge01 = 0f;
            activeHideSpotId = string.Empty;
            hidingGaugeUI?.SetDanger01(0f);
            hidingGaugeUI?.Hide();
        }

        private void UpdateGauge()
        {
            hidingGaugeUI?.SetDanger01(captureGauge01);
        }
    }
}
