using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class MonsterAIController
    {
        public MonsterState State { get; private set; } = MonsterState.Disabled;

        public void Reset()
        {
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
            State = MonsterState.Chase;
        }

        public void Tick(DangerSystem dangerSystem, HidingSystem hidingSystem)
        {
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
