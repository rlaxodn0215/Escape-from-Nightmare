using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioEmitter : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public AudioSource Source => audioSource;

        private void Awake()
        {
            audioSource ??= GetComponent<AudioSource>();
        }

        public void Play(SoundDefinition soundDefinition)
        {
            if (soundDefinition == null || soundDefinition.Clip == null || audioSource == null)
            {
                return;
            }

            audioSource.clip = soundDefinition.Clip;
            audioSource.loop = soundDefinition.Loop;
            audioSource.volume = soundDefinition.Volume;
            audioSource.Play();
        }

        public void Stop()
        {
            audioSource?.Stop();
        }
    }
}
