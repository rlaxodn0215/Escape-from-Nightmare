using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class EventRuntimeSystem : MonoBehaviour
    {
        [SerializeField] private GameBootstrap bootstrap;
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private RoomSystem roomSystem;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private AudioEmitter audioEmitter;
        [SerializeField] private SoundRuntimeSystem soundRuntimeSystem;
        [SerializeField] private MonsterRuntimeSystem monsterRuntimeSystem;

        private readonly Dictionary<string, EventDefinition> eventsById = new Dictionary<string, EventDefinition>();
        private readonly Dictionary<string, SoundDefinition> soundsById = new Dictionary<string, SoundDefinition>();
        private readonly Dictionary<string, ItemDefinition> itemsById = new Dictionary<string, ItemDefinition>();
        private readonly HashSet<string> flags = new HashSet<string>();
        private StageDefinition stageDefinition;
        private float dangerLevel;

        public float DangerLevel => dangerLevel;

        private void Awake()
        {
            bootstrap ??= FindFirstObjectByType<GameBootstrap>();
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            saveManager ??= FindFirstObjectByType<SaveManager>();
            roomSystem ??= FindFirstObjectByType<RoomSystem>();
            inventorySystem ??= FindFirstObjectByType<InventorySystem>();
            audioEmitter ??= FindFirstObjectByType<AudioEmitter>(FindObjectsInactive.Include);
            soundRuntimeSystem ??= FindFirstObjectByType<SoundRuntimeSystem>();
            monsterRuntimeSystem ??= FindFirstObjectByType<MonsterRuntimeSystem>();
            stageDefinition = bootstrap != null ? bootstrap.StageDefinition : null;
            BuildLookups();
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

        public void ResetRuntimeState()
        {
            dangerLevel = 0f;
            BuildLookups();
        }

        public bool HasFlag(string flagId)
        {
            return !string.IsNullOrWhiteSpace(flagId) && flags.Contains(flagId);
        }

        public bool ExecuteEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                return false;
            }

            if (!eventsById.TryGetValue(eventId, out EventDefinition eventDefinition))
            {
                Debug.LogWarning($"Event '{eventId}' is not defined.");
                return false;
            }

            ExecuteEvent(eventDefinition);
            return true;
        }

        public void ExecuteEvent(EventDefinition eventDefinition)
        {
            if (eventDefinition == null)
            {
                return;
            }

            if (eventDefinition.AudioCue != null)
            {
                PlaySound(eventDefinition.AudioCue);
            }

            if (!Mathf.Approximately(eventDefinition.DangerDelta, 0f))
            {
                dangerLevel = Mathf.Max(0f, dangerLevel + eventDefinition.DangerDelta);
            }

            foreach (EventEffect effect in eventDefinition.Effects)
            {
                ExecuteEffect(effect);
            }

            Debug.Log($"Executed event '{eventDefinition.EventId}'.");
        }

        private void ExecuteEffect(EventEffect effect)
        {
            switch (effect.effectType)
            {
                case EventEffectType.SetFlag:
                    SetFlag(effect);
                    break;
                case EventEffectType.GiveItem:
                    GiveItem(effect.targetId);
                    break;
                case EventEffectType.PlayAudio:
                    PlayAudio(effect);
                    break;
                case EventEffectType.ChangeRoom:
                    ChangeRoom(effect);
                    break;
                case EventEffectType.GameOver:
                    gameStateManager?.TriggerGameOver();
                    break;
                case EventEffectType.StageClear:
                    gameStateManager?.MarkStage1Clear();
                    saveManager?.SaveStage1Clear();
                    break;
                case EventEffectType.StartMonster:
                    monsterRuntimeSystem?.StartMonster(effect.targetId);
                    break;
                case EventEffectType.ChangeMonsterState:
                    monsterRuntimeSystem?.ApplyEventRequest(effect.targetId, effect.value);
                    break;
                case EventEffectType.ShowUi:
                case EventEffectType.SpawnInteractable:
                    Debug.Log($"Event effect '{effect.effectType}' for '{effect.targetId}' is queued for a later dedicated system.");
                    break;
            }
        }

        private void SetFlag(EventEffect effect)
        {
            string flag = !string.IsNullOrWhiteSpace(effect.targetId) ? effect.targetId : effect.value;
            if (!string.IsNullOrWhiteSpace(flag))
            {
                flags.Add(flag);
            }
        }

        private void GiveItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId) || inventorySystem == null)
            {
                return;
            }

            if (itemsById.TryGetValue(itemId, out ItemDefinition item))
            {
                inventorySystem.AddItem(item);
            }
            else
            {
                Debug.LogWarning($"Event tried to give missing item '{itemId}'.");
            }
        }

        private void PlayAudio(EventEffect effect)
        {
            string soundId = !string.IsNullOrWhiteSpace(effect.targetId) ? effect.targetId : effect.value;
            if (!string.IsNullOrWhiteSpace(soundId) && soundsById.TryGetValue(soundId, out SoundDefinition sound))
            {
                PlaySound(sound);
            }
        }

        private void PlaySound(SoundDefinition sound)
        {
            if (soundRuntimeSystem != null)
            {
                soundRuntimeSystem.Play(sound);
            }
            else
            {
                audioEmitter?.Play(sound);
            }
        }

        private void ChangeRoom(EventEffect effect)
        {
            string roomId = !string.IsNullOrWhiteSpace(effect.targetId) ? effect.targetId : effect.value;
            if (!string.IsNullOrWhiteSpace(roomId))
            {
                roomSystem?.ChangeRoom(roomId);
            }
        }

        private void BuildLookups()
        {
            eventsById.Clear();
            soundsById.Clear();
            itemsById.Clear();
            flags.Clear();

            if (stageDefinition == null)
            {
                Debug.LogWarning("EventRuntimeSystem has no StageDefinition.");
                return;
            }

            foreach (string flag in stageDefinition.InitialFlags)
            {
                if (!string.IsNullOrWhiteSpace(flag))
                {
                    flags.Add(flag);
                }
            }

            foreach (EventDefinition eventDefinition in stageDefinition.Events)
            {
                if (eventDefinition != null && !string.IsNullOrWhiteSpace(eventDefinition.EventId))
                {
                    eventsById[eventDefinition.EventId] = eventDefinition;
                }
            }

            foreach (SoundDefinition sound in stageDefinition.Sounds)
            {
                if (sound != null && !string.IsNullOrWhiteSpace(sound.SoundId))
                {
                    soundsById[sound.SoundId] = sound;
                }
            }

            foreach (ItemDefinition item in stageDefinition.Items)
            {
                if (item != null && !string.IsNullOrWhiteSpace(item.ItemId))
                {
                    itemsById[item.ItemId] = item;
                }
            }
        }
    }
}
