using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class MonsterAIController
    {
        public MonsterState State { get; private set; } = MonsterState.Disabled;
        private bool forcedChase;
        private bool debugStateOverride;
        private MonsterState debugState;

        public void Reset()
        {
            forcedChase = false;
            debugStateOverride = false;
            State = MonsterState.Disabled;
        }

        public void Enable()
        {
            if (State == MonsterState.Disabled)
            {
                State = MonsterState.Normal;
            }
        }

        public void ForceChase()
        {
            debugStateOverride = false;
            forcedChase = true;
            State = MonsterState.Chase;
        }

        public void ForceDebugState(MonsterState state)
        {
            forcedChase = false;
            debugStateOverride = true;
            debugState = state;
            State = state;
        }

        public void ClearDebugState()
        {
            debugStateOverride = false;
        }

        public void Tick(DangerSystem dangerSystem, HidingSystem hidingSystem)
        {
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
