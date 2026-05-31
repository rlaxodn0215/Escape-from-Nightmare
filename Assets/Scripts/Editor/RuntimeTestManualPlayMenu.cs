// -----------------------------------------------------------------------------
// Codex comment pass: Runtime Test Manual Play Menu
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\RuntimeTestManualPlayMenu.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Editor utility for the Runtime Test Manual Play Menu workflow, exposed through menu items or called by other validation tools.
    public static class RuntimeTestManualPlayMenu
    {
        [MenuItem("Escape From Nightmare/Tests/Disable All Runtime Test Runners For Manual Play")]
        // Performs the Disable All Runtime Test Runners For Manual Play operation while keeping its implementation details inside this script.
        public static void DisableAllRuntimeTestRunnersForManualPlay()
        {
            DisableRunner<FirstFivePuzzleRuntimeTestRunner>();
            DisableRunner<RemainingPuzzleRuntimeTestRunner>();
            DisableRunner<FullGameRouteRuntimeTestRunner>();
            DisableRunner<GameSceneInteractionRuntimeTestRunner>();
            RuntimeTestLaunchGate.ClearRun(nameof(FirstFivePuzzleRuntimeTestRunner));
            RuntimeTestLaunchGate.ClearRun(nameof(RemainingPuzzleRuntimeTestRunner));
            RuntimeTestLaunchGate.ClearRun(nameof(FullGameRouteRuntimeTestRunner));
            RuntimeTestLaunchGate.ClearRun(nameof(GameSceneInteractionRuntimeTestRunner));
            Debug.Log("[RuntimeTestManualPlayMenu] Disabled runOnStart on all runtime test runners in the active scene.");
        }

        private static void DisableRunner<T>() where T : Object
        {
            T runner = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (runner == null)
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(runner);
            SerializedProperty property = serializedObject.FindProperty("runOnStart");
            if (property == null)
            {
                Debug.LogWarning("[RuntimeTestManualPlayMenu] Serialized field not found: runOnStart on " + typeof(T).Name);
                return;
            }

            property.boolValue = false;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
