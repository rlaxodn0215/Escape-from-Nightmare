using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Sound Definition")]
    public sealed class SoundDefinition : ScriptableObject
    {
        [SerializeField] private string soundId;
        [SerializeField] private AudioCategory category;
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool loop;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;

        public string SoundId => soundId;
        public AudioCategory Category => category;
        public AudioClip Clip => clip;
        public bool Loop => loop;
        public float Volume => volume;
    }
}
