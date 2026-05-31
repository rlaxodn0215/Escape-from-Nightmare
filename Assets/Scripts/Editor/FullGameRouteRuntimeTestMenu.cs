// -----------------------------------------------------------------------------
// Codex comment pass: Full Game Route Runtime Test Menu
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\FullGameRouteRuntimeTestMenu.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Editor utility for the Full Game Route Runtime Test Menu workflow, exposed through menu items or called by other validation tools.
    public static class FullGameRouteRuntimeTestMenu
    {
        // Stores the Runner Name value used by this script's runtime or editor workflow.
        private const string RunnerName = "FullGameRouteRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare Full Game Route Runtime Test Runner")]
        // Performs the Prepare Full Game Route Runtime Test Runner operation while keeping its implementation details inside this script.
        public static void PrepareFullGameRouteRuntimeTestRunner()
        {
            FirstFivePuzzleRuntimeTestRunner firstFiveRunner = Object.FindFirstObjectByType<FirstFivePuzzleRuntimeTestRunner>(FindObjectsInactive.Include);
            if (firstFiveRunner != null)
            {
                SetBoolField(firstFiveRunner, "runOnStart", false);
            }

            RemainingPuzzleRuntimeTestRunner remainingRunner = Object.FindFirstObjectByType<RemainingPuzzleRuntimeTestRunner>(FindObjectsInactive.Include);
            if (remainingRunner != null)
            {
                SetBoolField(remainingRunner, "runOnStart", false);
            }

            GameSceneInteractionRuntimeTestRunner interactionRunner = Object.FindFirstObjectByType<GameSceneInteractionRuntimeTestRunner>(FindObjectsInactive.Include);
            if (interactionRunner != null)
            {
                SetBoolField(interactionRunner, "runOnStart", false);
            }

            FullGameRouteRuntimeTestRunner runner = Object.FindFirstObjectByType<FullGameRouteRuntimeTestRunner>(FindObjectsInactive.Include);
            if (runner == null)
            {
                GameObject runnerObject = new GameObject(RunnerName);
                runner = runnerObject.AddComponent<FullGameRouteRuntimeTestRunner>();
                Undo.RegisterCreatedObjectUndo(runnerObject, "Create Full Game Route Runtime Test Runner");
            }

            SetBoolField(runner, "runOnStart", false);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[FullGameRouteRuntimeTestMenu] Prepared FullGameRouteRuntimeTestRunner for manual selection. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run Full Game Route Runtime Test")]
        // Performs the Run Full Game Route Runtime Test operation while keeping its implementation details inside this script.
        public static void RunFullGameRouteRuntimeTest()
        {
            PrepareFullGameRouteRuntimeTestRunner();
            RuntimeTestLaunchGate.RequestRun(RunnerName);
            EditorApplication.isPlaying = true;
            Debug.Log("[FullGameRouteRuntimeTestMenu] Play Mode requested for full game route runtime test.");
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private static void SetBoolField(Object target, string fieldName, bool value)
        {
            if (target == null || string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogWarning("[FullGameRouteRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
