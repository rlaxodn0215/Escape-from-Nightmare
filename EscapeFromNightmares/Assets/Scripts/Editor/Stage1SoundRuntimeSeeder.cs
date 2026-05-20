using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1SoundRuntimeSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Sound Runtime";
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string StageDefinitionPath = "Assets/ScriptableObjects/Stage1/Stage1Definition.asset";

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            StageDefinition stageDefinition = AssetDatabase.LoadAssetAtPath<StageDefinition>(StageDefinitionPath);
            if (stageDefinition == null)
            {
                Debug.LogError($"Missing StageDefinition at {StageDefinitionPath}");
                return;
            }

            Dictionary<string, SoundDefinition> sounds = stageDefinition.Sounds
                .Where(sound => sound != null && !string.IsNullOrWhiteSpace(sound.SoundId))
                .ToDictionary(sound => sound.SoundId, sound => sound);

            BindRoomAmbience(stageDefinition, sounds);
            BindEventAudio(stageDefinition, sounds);
            BindSceneRuntime(sounds);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 sound runtime.");
        }

        private static void BindRoomAmbience(StageDefinition stageDefinition, Dictionary<string, SoundDefinition> sounds)
        {
            foreach (RoomDefinition room in stageDefinition.Rooms)
            {
                if (room == null || string.IsNullOrWhiteSpace(room.RoomId))
                {
                    continue;
                }

                if (sounds.TryGetValue(GetAmbienceId(room.RoomId), out SoundDefinition ambience))
                {
                    SerializedObject serializedRoom = new SerializedObject(room);
                    serializedRoom.FindProperty("ambienceAudio").objectReferenceValue = ambience;
                    serializedRoom.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(room);
                }
            }
        }

        private static void BindEventAudio(StageDefinition stageDefinition, Dictionary<string, SoundDefinition> sounds)
        {
            foreach (EventDefinition eventDefinition in stageDefinition.Events)
            {
                if (eventDefinition == null || string.IsNullOrWhiteSpace(eventDefinition.EventId))
                {
                    continue;
                }

                string soundId = GetEventSoundId(eventDefinition.EventId);
                if (string.IsNullOrWhiteSpace(soundId) || !sounds.TryGetValue(soundId, out SoundDefinition sound))
                {
                    continue;
                }

                SerializedObject serializedEvent = new SerializedObject(eventDefinition);
                serializedEvent.FindProperty("audioCue").objectReferenceValue = sound;
                serializedEvent.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(eventDefinition);
            }
        }

        private static void BindSceneRuntime(Dictionary<string, SoundDefinition> sounds)
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            GameObject stageRoot = GameObject.Find("StageRoot");
            GameObject audioRoot = GameObject.Find("AudioRoot");

            if (systems == null || stageRoot == null || audioRoot == null)
            {
                Debug.LogError("Stage1 scene is missing Systems, StageRoot, or AudioRoot.");
                return;
            }

            AudioEmitter bgmEmitter = EnsureEmitter(audioRoot.transform, "BgmEmitter", true);
            AudioEmitter ambienceEmitter = EnsureEmitter(audioRoot.transform, "AmbienceEmitter", true);
            AudioEmitter sfxEmitter = EnsureEmitter(audioRoot.transform, "SfxEmitter", false);
            AudioEmitter eventEmitter = EnsureEmitter(audioRoot.transform, "EventEmitter", false);
            AudioEmitter uiEmitter = EnsureEmitter(audioRoot.transform, "UiEmitter", false);

            SoundRuntimeSystem soundRuntimeSystem = systems.GetComponent<SoundRuntimeSystem>();
            if (soundRuntimeSystem == null)
            {
                soundRuntimeSystem = systems.AddComponent<SoundRuntimeSystem>();
            }

            GameBootstrap bootstrap = stageRoot.GetComponent<GameBootstrap>();
            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();

            SerializedObject serializedSound = new SerializedObject(soundRuntimeSystem);
            serializedSound.FindProperty("bootstrap").objectReferenceValue = bootstrap;
            serializedSound.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            serializedSound.FindProperty("bgmEmitter").objectReferenceValue = bgmEmitter;
            serializedSound.FindProperty("ambienceEmitter").objectReferenceValue = ambienceEmitter;
            serializedSound.FindProperty("sfxEmitter").objectReferenceValue = sfxEmitter;
            serializedSound.FindProperty("eventEmitter").objectReferenceValue = eventEmitter;
            serializedSound.FindProperty("uiEmitter").objectReferenceValue = uiEmitter;
            serializedSound.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(soundRuntimeSystem);

            EventRuntimeSystem eventRuntimeSystem = systems.GetComponent<EventRuntimeSystem>();
            if (eventRuntimeSystem != null)
            {
                SerializedObject serializedEventRuntime = new SerializedObject(eventRuntimeSystem);
                serializedEventRuntime.FindProperty("soundRuntimeSystem").objectReferenceValue = soundRuntimeSystem;
                serializedEventRuntime.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(eventRuntimeSystem);
            }

            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();
            if (interactionSystem != null)
            {
                SerializedObject serializedInteraction = new SerializedObject(interactionSystem);
                serializedInteraction.FindProperty("soundRuntimeSystem").objectReferenceValue = soundRuntimeSystem;
                serializedInteraction.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(interactionSystem);
            }

            PuzzleSystem puzzleSystem = systems.GetComponent<PuzzleSystem>();
            if (puzzleSystem != null)
            {
                SerializedObject serializedPuzzle = new SerializedObject(puzzleSystem);
                serializedPuzzle.FindProperty("soundRuntimeSystem").objectReferenceValue = soundRuntimeSystem;
                serializedPuzzle.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(puzzleSystem);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static AudioEmitter EnsureEmitter(Transform parent, string name, bool loop)
        {
            Transform existing = parent.Find(name);
            GameObject root = existing != null ? existing.gameObject : new GameObject(name);
            root.transform.SetParent(parent, false);
            root.SetActive(true);

            AudioSource source = root.GetComponent<AudioSource>();
            if (source == null)
            {
                source = root.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;

            AudioEmitter emitter = root.GetComponent<AudioEmitter>();
            if (emitter == null)
            {
                emitter = root.AddComponent<AudioEmitter>();
            }

            SerializedObject serializedEmitter = new SerializedObject(emitter);
            serializedEmitter.FindProperty("audioSource").objectReferenceValue = source;
            serializedEmitter.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(emitter);
            return emitter;
        }

        private static string GetAmbienceId(string roomId)
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

        private static string GetEventSoundId(string eventId)
        {
            return eventId switch
            {
                "event_window_silhouette" => "event_window_silhouette",
                "event_kitchen_first_appearance" => "event_kitchen_first_appearance",
                "event_final_chase_trigger" => "event_final_chase_trigger",
                "event_player_captured" => "monster_capture",
                "event_stage1_clear" => "bgm_ending_hint",
                "event_open_study_safe" => "sfx_safe_open",
                "event_open_laundry_storage_box" => "sfx_box_open",
                "event_restore_electricity" => "sfx_electricity_restore",
                "event_break_mirror" => "sfx_mirror_crack",
                "event_open_master_bedroom_drawer" => "sfx_drawer_open",
                "event_open_attic_toy_box" => "sfx_toy_box_open",
                "event_front_door_key_appears" => "sfx_key_appear",
                "event_front_door_locked" => "sfx_door_locked",
                "event_puzzle_error_soft" => "sfx_puzzle_error",
                "event_toy_wrong_sound" => "sfx_toy_wrong_sound",
                _ => string.Empty
            };
        }
    }
}
