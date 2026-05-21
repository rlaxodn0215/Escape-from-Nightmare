using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    public sealed class DangerSystem
    {
        public float DangerLevel { get; private set; }
        public float NoiseLevel { get; private set; }
        public float StayTimer { get; private set; }
        public float CaptureGauge { get; private set; }

        public void Reset()
        {
            DangerLevel = 0f;
            NoiseLevel = 0f;
            StayTimer = 0f;
            CaptureGauge = 0f;
        }

        public void Tick(float deltaTime, RoomDefinition room, MonsterState monsterState)
        {
            StayTimer += deltaTime;
            var roomModifier = room != null ? room.dangerModifier : 1f;
            DangerLevel = Mathf.Clamp(DangerLevel + deltaTime * 0.35f * roomModifier, 0f, 100f);
            NoiseLevel = Mathf.Max(0f, NoiseLevel - deltaTime * 5f);

            if (monsterState == MonsterState.Chase)
            {
                CaptureGauge = Mathf.Clamp(CaptureGauge + deltaTime * 18f, 0f, 100f);
            }
            else
            {
                CaptureGauge = Mathf.Max(0f, CaptureGauge - deltaTime * 10f);
            }
        }

        public void AddNoise(float amount)
        {
            NoiseLevel = Mathf.Clamp(NoiseLevel + amount, 0f, 100f);
            DangerLevel = Mathf.Clamp(DangerLevel + amount * 0.5f, 0f, 100f);
        }

        public void OnRoomChanged()
        {
            StayTimer = 0f;
            CaptureGauge = Mathf.Max(0f, CaptureGauge - 25f);
        }
    }
}
