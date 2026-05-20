using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class SoundRuntimeSystem : MonoBehaviour
    {
        [SerializeField] private GameBootstrap bootstrap;
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private AudioEmitter bgmEmitter;
        [SerializeField] private AudioEmitter ambienceEmitter;
        [SerializeField] private AudioEmitter sfxEmitter;
        [SerializeField] private AudioEmitter eventEmitter;
        [SerializeField] private AudioEmitter uiEmitter;

        private readonly Dictionary<string, SoundDefinition> soundsById = new Dictionary<string, SoundDefinition>();
        private StageDefinition stageDefinition;

        private void Awake()
        {
            bootstrap ??= FindFirstObjectByType<GameBootstrap>();
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
            stageDefinition = bootstrap != null ? bootstrap.StageDefinition : null;
            BuildSoundLookup();
        }

        private void OnEnable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged += HandleRoomChanged;
                HandleRoomChanged(roomSystem.CurrentRoom);
            }
        }

        private void OnDisable()
        {
            if (roomSystem != null)
            {
                roomSystem.RoomChanged -= HandleRoomChanged;
            }
        }

        public bool PlaySound(string soundId)
        {
            if (string.IsNullOrWhiteSpace(soundId) || !soundsById.TryGetValue(soundId, out SoundDefinition sound))
            {
                return false;
            }

            Play(sound);
            return true;
        }

        public void Play(SoundDefinition sound)
        {
            if (sound == null)
            {
                return;
            }

            SelectEmitter(sound.Category)?.Play(sound);
        }

        public void PlayPuzzleSuccess()
        {
            PlaySound("sfx_puzzle_success");
        }

        public void PlayPuzzleError()
        {
            PlaySound("sfx_puzzle_error");
        }

        public void PlayItemPickup()
        {
            PlaySound("sfx_item_pickup");
        }

        public void PlayDoorOpen()
        {
            PlaySound("sfx_door_open");
        }

        public void PlayUiClick()
        {
            PlaySound("ui_button_click");
        }

        private void HandleRoomChanged(RoomDefinition room)
        {
            SoundDefinition ambience = room != null ? room.AmbienceAudio : null;
            if (ambience == null && room != null)
            {
                soundsById.TryGetValue(GetAmbienceFallbackId(room.RoomId), out ambience);
            }

            if (ambience != null)
            {
                ambienceEmitter?.Play(ambience);
            }
        }

        private AudioEmitter SelectEmitter(AudioCategory category)
        {
            return category switch
            {
                AudioCategory.Bgm => bgmEmitter,
                AudioCategory.Ambience => ambienceEmitter,
                AudioCategory.Ui => uiEmitter != null ? uiEmitter : sfxEmitter,
                AudioCategory.Event => eventEmitter != null ? eventEmitter : sfxEmitter,
                AudioCategory.Monster => eventEmitter != null ? eventEmitter : sfxEmitter,
                _ => sfxEmitter
            };
        }

        private void BuildSoundLookup()
        {
            soundsById.Clear();
            if (stageDefinition == null)
            {
                Debug.LogWarning("SoundRuntimeSystem has no StageDefinition.");
                return;
            }

            foreach (SoundDefinition sound in stageDefinition.Sounds)
            {
                if (sound != null && !string.IsNullOrWhiteSpace(sound.SoundId))
                {
                    soundsById[sound.SoundId] = sound;
                }
            }
        }

        private static string GetAmbienceFallbackId(string roomId)
        {
            return roomId switch
            {
                "first_floor_hallway" => "amb_1f_hallway",
                "second_floor_hallway" => "amb_2f_hallway",
                "attic_main" or "attic_album_storage" or "attic_toy_storage" => "amb_attic",
                "basement_entry" or "basement_main" or "basement_storage" => "amb_basement",
                _ => $"amb_{roomId}"
            };
        }
    }
}
