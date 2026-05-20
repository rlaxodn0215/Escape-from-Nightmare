using EscapeFromNightmares.Core;
using EscapeFromNightmares.Systems;
using EscapeFromNightmares.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1HidingRuntimeSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Hiding Runtime";
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            GameObject hidingGaugeObject = FindSceneObject("HidingGaugeUI");

            if (systems == null || hidingGaugeObject == null)
            {
                Debug.LogError("Stage1 scene is missing Systems or HidingGaugeUI.");
                return;
            }

            HidingGaugeUI hidingGaugeUI = hidingGaugeObject.GetComponent<HidingGaugeUI>();
            if (hidingGaugeUI == null)
            {
                Debug.LogError("HidingGaugeUI scene object is missing HidingGaugeUI component.");
                return;
            }

            HidingRuntimeSystem hidingRuntimeSystem = systems.GetComponent<HidingRuntimeSystem>();
            if (hidingRuntimeSystem == null)
            {
                hidingRuntimeSystem = systems.AddComponent<HidingRuntimeSystem>();
            }

            EventRuntimeSystem eventRuntimeSystem = systems.GetComponent<EventRuntimeSystem>();
            GameStateManager gameStateManager = systems.GetComponent<GameStateManager>();
            SoundRuntimeSystem soundRuntimeSystem = systems.GetComponent<SoundRuntimeSystem>();

            SerializedObject serializedHiding = new SerializedObject(hidingRuntimeSystem);
            serializedHiding.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serializedHiding.FindProperty("hidingGaugeUI").objectReferenceValue = hidingGaugeUI;
            serializedHiding.FindProperty("eventRuntimeSystem").objectReferenceValue = eventRuntimeSystem;
            serializedHiding.FindProperty("soundRuntimeSystem").objectReferenceValue = soundRuntimeSystem;
            serializedHiding.FindProperty("defaultHideSeconds").floatValue = 5f;
            serializedHiding.FindProperty("captureDecayPerSecond").floatValue = 0.34f;
            serializedHiding.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(hidingRuntimeSystem);

            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();
            if (interactionSystem != null)
            {
                SerializedObject serializedInteraction = new SerializedObject(interactionSystem);
                serializedInteraction.FindProperty("hidingRuntimeSystem").objectReferenceValue = hidingRuntimeSystem;
                serializedInteraction.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(interactionSystem);
            }

            hidingGaugeUI.SetDanger01(0f);
            hidingGaugeObject.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded Stage1 hiding runtime.");
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
