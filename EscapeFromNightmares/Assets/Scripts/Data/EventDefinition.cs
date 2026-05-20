using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Event Definition")]
    public sealed class EventDefinition : ScriptableObject
    {
        [SerializeField] private string eventId;
        [SerializeField] private EventEffect[] effects = new EventEffect[0];
        [SerializeField] private SoundDefinition audioCue;
        [SerializeField] private MonsterState monsterStateAfterEvent = MonsterState.Normal;
        [SerializeField] private float dangerDelta;

        public string EventId => eventId;
        public EventEffect[] Effects => effects;
        public SoundDefinition AudioCue => audioCue;
        public MonsterState MonsterStateAfterEvent => monsterStateAfterEvent;
        public float DangerDelta => dangerDelta;
    }
}
