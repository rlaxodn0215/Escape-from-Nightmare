// -----------------------------------------------------------------------------
// Codex comment pass: Remaining Puzzle Runtime Test Menu
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\RemainingPuzzleRuntimeTestMenu.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Editor utility for the Remaining Puzzle Runtime Test Menu workflow, exposed through menu items or called by other validation tools.
    public static class RemainingPuzzleRuntimeTestMenu
    {
        // Stores the Runner Name value used by this script's runtime or editor workflow.
        private const string RunnerName = "RemainingPuzzleRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare Remaining Puzzle Runtime Test Runner")]
        // Performs the Prepare Remaining Puzzle Runtime Test Runner operation while keeping its implementation details inside this script.
        public static void PrepareRemainingPuzzleRuntimeTestRunner()
        {
            FirstFivePuzzleRuntimeTestRunner firstFiveRunner = Object.FindFirstObjectByType<FirstFivePuzzleRuntimeTestRunner>(FindObjectsInactive.Include);
            if (firstFiveRunner != null)
            {
                SetBoolField(firstFiveRunner, "runOnStart", false);
            }

            FullGameRouteRuntimeTestRunner fullRouteRunner = Object.FindFirstObjectByType<FullGameRouteRuntimeTestRunner>(FindObjectsInactive.Include);
            if (fullRouteRunner != null)
            {
                SetBoolField(fullRouteRunner, "runOnStart", false);
            }

            GameSceneInteractionRuntimeTestRunner interactionRunner = Object.FindFirstObjectByType<GameSceneInteractionRuntimeTestRunner>(FindObjectsInactive.Include);
            if (interactionRunner != null)
            {
                SetBoolField(interactionRunner, "runOnStart", false);
            }

            RemainingPuzzleRuntimeTestRunner runner = Object.FindFirstObjectByType<RemainingPuzzleRuntimeTestRunner>(FindObjectsInactive.Include);
            if (runner == null)
            {
                GameObject runnerObject = new GameObject(RunnerName);
                runner = runnerObject.AddComponent<RemainingPuzzleRuntimeTestRunner>();
                Undo.RegisterCreatedObjectUndo(runnerObject, "Create Remaining Puzzle Runtime Test Runner");
            }

            SetBoolField(runner, "runOnStart", false);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[RemainingPuzzleRuntimeTestMenu] Prepared RemainingPuzzleRuntimeTestRunner for manual selection. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run Remaining Puzzle Runtime Tests")]
        // Performs the Run Remaining Puzzle Runtime Tests operation while keeping its implementation details inside this script.
        public static void RunRemainingPuzzleRuntimeTests()
        {
            PrepareRemainingPuzzleRuntimeTestRunner();
            RuntimeTestLaunchGate.RequestRun(RunnerName);
            EditorApplication.isPlaying = true;
            Debug.Log("[RemainingPuzzleRuntimeTestMenu] Play Mode requested for remaining puzzle runtime tests.");
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
                Debug.LogWarning("[RemainingPuzzleRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
