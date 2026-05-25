using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 방에 머무는 시간, 소음, 몬스터 상태를 바탕으로 플레이어의 위험도와 포획 게이지를 계산합니다.
    /// </summary>
    public sealed class DangerSystem
    {
        /// <summary>누적된 기본 위험도입니다. 방의 dangerModifier에 따라 증가 속도가 달라집니다.</summary>
        public float DangerLevel { get; private set; }
        /// <summary>상호작용이나 실패로 발생한 단기 소음 수치입니다.</summary>
        public float NoiseLevel { get; private set; }
        /// <summary>현재 방에 머문 시간입니다.</summary>
        public float StayTimer { get; private set; }
        /// <summary>추격 상태에서 올라가며 100에 도달하면 게임 오버로 이어지는 게이지입니다.</summary>
        public float CaptureGauge { get; private set; }

        /// <summary>새 게임 또는 재시작 시 위험 관련 누적값을 초기화합니다.</summary>
        public void Reset()
        {
            DangerLevel = 0f;
            NoiseLevel = 0f;
            StayTimer = 0f;
            CaptureGauge = 0f;
        }

        /// <summary>
        /// 매 프레임 위험도를 누적하고, 추격 여부에 따라 포획 게이지를 증가 또는 감소시킵니다.
        /// </summary>
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

        /// <summary>
        /// 퍼즐 실패나 큰 소리 이벤트처럼 즉시 위험도를 올리는 소음을 더합니다.
        /// </summary>
        public void AddNoise(float amount)
        {
            NoiseLevel = Mathf.Clamp(NoiseLevel + amount, 0f, 100f);
            DangerLevel = Mathf.Clamp(DangerLevel + amount * 0.5f, 0f, 100f);
        }

        /// <summary>
        /// 방 이동 직후 체류 시간을 초기화하고 포획 압박을 일부 낮춥니다.
        /// </summary>
        public void OnRoomChanged()
        {
            StayTimer = 0f;
            CaptureGauge = Mathf.Max(0f, CaptureGauge - 25f);
        }
    }
}
