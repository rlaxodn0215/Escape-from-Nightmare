using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 위험도와 은신 소음을 종합해 몬스터의 현재 압박 단계를 결정합니다.
    /// </summary>
    public sealed class MonsterAIController
    {
        /// <summary>현재 몬스터 상태입니다.</summary>
        public MonsterState State { get; private set; } = MonsterState.Disabled;
        private bool forcedChase;
        private bool debugStateOverride;
        private MonsterState debugState;

        /// <summary>몬스터 상태와 디버그 강제 상태를 초기 비활성 상태로 되돌립니다.</summary>
        public void Reset()
        {
            forcedChase = false;
            debugStateOverride = false;
            State = MonsterState.Disabled;
        }

        /// <summary>비활성 상태인 몬스터를 일반 상태로 켭니다.</summary>
        public void Enable()
        {
            if (State == MonsterState.Disabled)
            {
                State = MonsterState.Normal;
            }
        }

        /// <summary>최종 추격처럼 위험도와 무관하게 추격 상태를 유지하도록 강제합니다.</summary>
        public void ForceChase()
        {
            debugStateOverride = false;
            forcedChase = true;
            State = MonsterState.Chase;
        }

        /// <summary>
        /// 런타임 QA 입력에서 특정 몬스터 상태를 직접 확인할 수 있도록 상태를 고정합니다.
        /// </summary>
        public void ForceDebugState(MonsterState state)
        {
            forcedChase = false;
            debugStateOverride = true;
            debugState = state;
            State = state;
        }

        /// <summary>디버그 상태 고정을 해제하고 일반 위험도 계산으로 돌아갑니다.</summary>
        public void ClearDebugState()
        {
            debugStateOverride = false;
        }

        /// <summary>
        /// 위험도, 일반 소음, 은신 소음을 합산해 몬스터 상태 단계를 갱신합니다.
        /// </summary>
        public void Tick(DangerSystem dangerSystem, HidingSystem hidingSystem)
        {
            // 강제 추격과 디버그 상태는 플레이 흐름 검증을 위해 일반 AI 계산보다 우선합니다.
            if (forcedChase)
            {
                State = MonsterState.Chase;
                return;
            }

            if (debugStateOverride)
            {
                State = debugState;
                return;
            }

            if (State == MonsterState.Disabled)
            {
                return;
            }

            var danger = dangerSystem.DangerLevel + dangerSystem.NoiseLevel + hidingSystem.HideNoise;
            if (danger >= 130f)
            {
                State = MonsterState.Chase;
            }
            else if (danger >= 95f)
            {
                State = MonsterState.NearDetection;
            }
            else if (danger >= 65f)
            {
                State = MonsterState.Searching;
            }
            else if (danger >= 35f)
            {
                State = MonsterState.Approaching;
            }
            else
            {
                State = MonsterState.Normal;
            }
        }
    }
}
