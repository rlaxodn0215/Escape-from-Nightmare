using EscapeFromNightmares.Core;
using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1MonsterRuntimeSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Monster Runtime";
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            GameObject stageRoot = GameObject.Find("StageRoot");
            GameObject monsterOverlayObject = FindSceneObject("MonsterOverlay");

            if (systems == null || stageRoot == null || monsterOverlayObject == null)
            {
                Debug.LogError("Stage1 scene is missing Systems, StageRoot, or MonsterOverlay.");
                return;
            }

            MonsterRuntimeSystem monsterRuntimeSystem = systems.GetComponent<MonsterRuntimeSystem>();
            if (monsterRuntimeSystem == null)
            {
                monsterRuntimeSystem = systems.AddComponent<MonsterRuntimeSystem>();
            }

            GameBootstrap bootstrap = stageRoot.GetComponent<GameBootstrap>();
            GameStateManager gameStateManager = systems.GetComponent<GameStateManager>();
            RoomSystem roomSystem = systems.GetComponent<RoomSystem>();
            HidingRuntimeSystem hidingRuntimeSystem = systems.GetComponent<HidingRuntimeSystem>();
            SoundRuntimeSystem soundRuntimeSystem = systems.GetComponent<SoundRuntimeSystem>();
            MonsterOverlay monsterOverlay = monsterOverlayObject.GetComponent<MonsterOverlay>();

            SerializedObject serializedMonster = new SerializedObject(monsterRuntimeSystem);
            serializedMonster.FindProperty("bootstrap").objectReferenceValue = bootstrap;
            serializedMonster.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serializedMonster.FindProperty("roomSystem").objectReferenceValue = roomSystem;
            serializedMonster.FindProperty("hidingRuntimeSystem").objectReferenceValue = hidingRuntimeSystem;
            serializedMonster.FindProperty("soundRuntimeSystem").objectReferenceValue = soundRuntimeSystem;
            serializedMonster.FindProperty("monsterOverlay").objectReferenceValue = monsterOverlay;
            serializedMonster.FindProperty("nearCapturePressurePerSecond").floatValue = 0.06f;
            serializedMonster.FindProperty("chaseCapturePressurePerSecond").floatValue = 0.18f;
            serializedMonster.FindProperty("chaseRoomMovesToEscape").intValue = 3;
            serializedMonster.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(monsterRuntimeSystem);

            EventRuntimeSystem eventRuntimeSystem = systems.GetComponent<EventRuntimeSystem>();
            if (eventRuntimeSystem != null)
            {
                SerializedObject serializedEvent = new SerializedObject(eventRuntimeSystem);
                serializedEvent.FindProperty("monsterRuntimeSystem").objectReferenceValue = monsterRuntimeSystem;
                serializedEvent.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(eventRuntimeSystem);
            }

            if (monsterOverlayObject != null)
            {
                monsterOverlayObject.SetActive(true);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 monster runtime.");
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
