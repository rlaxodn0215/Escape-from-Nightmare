using EscapeFromNightmares.Core;
using EscapeFromNightmares.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1InventoryReferenceSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";

        [MenuItem("Escape From Nightmares/Seed Inventory Runtime")]
        public static void Seed()
        {
            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject systems = GameObject.Find("Systems");
            if (systems == null)
            {
                Debug.LogError("Stage1 scene is missing Systems.");
                return;
            }

            InventorySystem inventorySystem = systems.GetComponent<InventorySystem>();
            if (inventorySystem == null)
            {
                inventorySystem = systems.AddComponent<InventorySystem>();
            }

            GameStateManager gameStateManager = systems.GetComponent<GameStateManager>();
            SerializedObject serializedInventory = new SerializedObject(inventorySystem);
            serializedInventory.FindProperty("gameStateManager").objectReferenceValue = gameStateManager;
            serializedInventory.ApplyModifiedPropertiesWithoutUndo();

            InteractionSystem interactionSystem = systems.GetComponent<InteractionSystem>();
            if (interactionSystem != null)
            {
                SerializedObject serialized = new SerializedObject(interactionSystem);
                serialized.FindProperty("inventorySystem").objectReferenceValue = inventorySystem;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(interactionSystem);
            }

            EditorUtility.SetDirty(inventorySystem);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded inventory runtime.");
        }
    }
}
