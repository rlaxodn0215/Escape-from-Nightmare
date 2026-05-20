using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1EventRuntimeSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string EventRoot = "Assets/ScriptableObjects/Stage1/Events";
        private const string StageDefinitionPath = "Assets/ScriptableObjects/Stage1/Stage1Definition.asset";

        [MenuItem("Escape From Nightmares/Seed Stage1 Event Runtime")]
        public static void Seed()
        {
            SeedEventEffects();
            BindSceneRuntime();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 event runtime.");
        }

        private static void SeedEventEffects()
        {
            Dictionary<string, EventDefinition> events = LoadEvents();
            SetEffects(events, "event_open_study_safe",
                Effect(EventEffectType.SetFlag, "study_safe_opened"),
                Effect(EventEffectType.GiveItem, "fuse_holder"));
            SetEffects(events, "event_open_hidden_photo_drawer",
                Effect(EventEffectType.SetFlag, "hidden_photo_drawer_opened"));
            SetEffects(events, "event_open_laundry_storage_box",
                Effect(EventEffectType.SetFlag, "laundry_storage_box_opened"),
                Effect(EventEffectType.GiveItem, "fuse"));
            SetEffects(events, "event_restore_electricity",
                Effect(EventEffectType.SetFlag, "electricity_restored"),
                Effect(EventEffectType.GiveItem, "old_keychain"),
                Effect(EventEffectType.ChangeMonsterState, "electricity_noise", "approaching"));
            SetEffects(events, "event_break_mirror",
                Effect(EventEffectType.SetFlag, "mirror_broken"),
                Effect(EventEffectType.GiveItem, "broken_hand_mirror"));
            SetEffects(events, "event_open_master_bedroom_drawer",
                Effect(EventEffectType.SetFlag, "master_bedroom_drawer_opened"),
                Effect(EventEffectType.GiveItem, "old_necklace"));
            SetEffects(events, "event_open_attic_toy_box",
                Effect(EventEffectType.SetFlag, "attic_toy_box_opened"),
                Effect(EventEffectType.GiveItem, "small_doll"),
                Effect(EventEffectType.GiveItem, "symbol_fragment"));
            SetEffects(events, "event_front_door_key_appears",
                Effect(EventEffectType.SetFlag, "front_door_key_appeared"));
            SetEffects(events, "event_final_chase_trigger",
                Effect(EventEffectType.SetFlag, "final_chase_started"),
                Effect(EventEffectType.StartMonster, "final_chase"));
            SetEffects(events, "event_stage1_clear",
                Effect(EventEffectType.StageClear, "stage1_clear"));
            SetEffects(events, "event_front_door_locked",
                Effect(EventEffectType.SetFlag, "front_door_locked_checked"));
            SetEffects(events, "event_player_captured",
                Effect(EventEffectType.GameOver, "player_captured"));
            SetEffects(events, "event_puzzle_error_soft",
                Effect(EventEffectType.SetFlag, "last_puzzle_error"));
            SetEffects(events, "event_toy_wrong_sound",
                Effect(EventEffectType.SetFlag, "toy_sequence_wrong"));
            SetEffects(events, "event_window_silhouette",
                Effect(EventEffectType.SetFlag, "window_silhouette_seen"),
                Effect(EventEffectType.ChangeMonsterState, "window_silhouette", "approaching"));
            SetEffects(events, "event_kitchen_first_appearance",
                Effect(EventEffectType.SetFlag, "kitchen_first_appearance_seen"),
                Effect(EventEffectType.StartMonster, "kitchen_first_appearance"));
            SetEffects(events, "event_basement_door_unlocked",
                Effect(EventEffectType.SetFlag, "basement_door_unlocked"));
            SetEffects(events, "event_electric_noise_attracts_monster",
                Effect(EventEffectType.ChangeMonsterState, "electric_noise", "approaching"));
        }

        private static void BindSceneRuntime()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            GameObject stageRoot = GameObject.Find("StageRoot");
            GameObject audioEmitterObject = FindSceneObject("AudioEmitter");

            if (systems == null || stageRoot == null)
            {
                Debug.LogError("Stage1 scene is missing Systems or StageRoot.");
                return;
            }

            EventRuntimeSystem eventRuntimeSystem = systems.GetComponent<EventRuntimeSystem>();
            if (eventRuntimeSystem == null)
            {
                eventRuntimeSystem = systems.AddComponent<EventRuntimeSystem>();
            }

            GameBootstrap bootstrap = stageRoot.GetComponent<GameBootstrap>();
            GameStateManager gameStateManager = systems.GetComponent<GameStateManager>();
            SaveManager saveManager = systems.GetComponent<SaveManager>();
            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            InventorySystem inventorySystem = systems.GetComponent<InventorySystem>();
            AudioEmitter audioEmitter = audioEmitterObject != null ? audioEmitterObject.GetComponent<AudioEmitter>() : null;

            SerializedObject serializedEventSystem = new SerializedObject(eventRuntimeSystem);
            serializedEventSystem.FindProperty("bootstrap").objectReferenceValue = bootstrap;
            serializedEventSystem.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serializedEventSystem.FindProperty("saveManager").objectReferenceValue = saveManager;
            serializedEventSystem.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            serializedEventSystem.FindProperty("inventorySystem").objectReferenceValue = inventorySystem;
            serializedEventSystem.FindProperty("audioEmitter").objectReferenceValue = audioEmitter;
            serializedEventSystem.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(eventRuntimeSystem);

            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();
            if (interactionSystem != null)
            {
                SerializedObject serializedInteraction = new SerializedObject(interactionSystem);
                serializedInteraction.FindProperty("eventRuntimeSystem").objectReferenceValue = eventRuntimeSystem;
                serializedInteraction.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(interactionSystem);
            }

            PuzzleSystem puzzleSystem = systems.GetComponent<PuzzleSystem>();
            if (puzzleSystem != null)
            {
                SerializedObject serializedPuzzle = new SerializedObject(puzzleSystem);
                serializedPuzzle.FindProperty("eventRuntimeSystem").objectReferenceValue = eventRuntimeSystem;
                serializedPuzzle.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(puzzleSystem);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static Dictionary<string, EventDefinition> LoadEvents()
        {
            Dictionary<string, EventDefinition> results = new Dictionary<string, EventDefinition>();
            string[] guids = AssetDatabase.FindAssets("t:EventDefinition", new[] { EventRoot });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EventDefinition eventDefinition = AssetDatabase.LoadAssetAtPath<EventDefinition>(path);
                if (eventDefinition != null && !string.IsNullOrWhiteSpace(eventDefinition.EventId))
                {
                    results[eventDefinition.EventId] = eventDefinition;
                }
            }

            return results;
        }

        private static void SetEffects(Dictionary<string, EventDefinition> events, string eventId, params EventEffect[] effects)
        {
            if (!events.TryGetValue(eventId, out EventDefinition eventDefinition))
            {
                Debug.LogWarning($"Cannot seed missing event '{eventId}'.");
                return;
            }

            SerializedObject serialized = new SerializedObject(eventDefinition);
            SerializedProperty effectsProperty = serialized.FindProperty("effects");
            effectsProperty.arraySize = effects.Length;
            for (int i = 0; i < effects.Length; i++)
            {
                SerializedProperty effect = effectsProperty.GetArrayElementAtIndex(i);
                effect.FindPropertyRelative("effectType").enumValueIndex = (int)effects[i].effectType;
                effect.FindPropertyRelative("targetId").stringValue = effects[i].targetId;
                effect.FindPropertyRelative("value").stringValue = effects[i].value;
                effect.FindPropertyRelative("intValue").intValue = effects[i].intValue;
                effect.FindPropertyRelative("floatValue").floatValue = effects[i].floatValue;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(eventDefinition);
        }

        private static EventEffect Effect(EventEffectType type, string targetId, string value = "", int intValue = 0, float floatValue = 0f)
        {
            return new EventEffect
            {
                effectType = type,
                targetId = targetId,
                value = value,
                intValue = intValue,
                floatValue = floatValue
            };
        }

        private static GameObject FindSceneObject(string name)
        {
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }
    }
}
