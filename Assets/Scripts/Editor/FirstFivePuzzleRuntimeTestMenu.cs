// -----------------------------------------------------------------------------
// Codex comment pass: First Five Puzzle Runtime Test Menu
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\FirstFivePuzzleRuntimeTestMenu.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Editor utility for the First Five Puzzle Runtime Test Menu workflow, exposed through menu items or called by other validation tools.
    public static class FirstFivePuzzleRuntimeTestMenu
    {
        // Stores the Runner Name value used by this script's runtime or editor workflow.
        private const string RunnerName = "FirstFivePuzzleRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare First Five Puzzle Runtime Test Runner")]
        // Performs the Prepare First Five Puzzle Runtime Test Runner operation while keeping its implementation details inside this script.
        public static void PrepareFirstFivePuzzleRuntimeTestRunner()
        {
            RemainingPuzzleRuntimeTestRunner remainingRunner = Object.FindFirstObjectByType<RemainingPuzzleRuntimeTestRunner>(FindObjectsInactive.Include);
            if (remainingRunner != null)
            {
                SetBoolField(remainingRunner, "runOnStart", false);
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

            FirstFivePuzzleRuntimeTestRunner runner = Object.FindFirstObjectByType<FirstFivePuzzleRuntimeTestRunner>(FindObjectsInactive.Include);
            if (runner == null)
            {
                GameObject runnerObject = new GameObject(RunnerName);
                runner = runnerObject.AddComponent<FirstFivePuzzleRuntimeTestRunner>();
                Undo.RegisterCreatedObjectUndo(runnerObject, "Create First Five Puzzle Runtime Test Runner");
            }

            SetBoolField(runner, "runOnStart", false);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[FirstFivePuzzleRuntimeTestMenu] Prepared FirstFivePuzzleRuntimeTestRunner for manual selection. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run First Five Puzzle Runtime Tests")]
        // Performs the Run First Five Puzzle Runtime Tests operation while keeping its implementation details inside this script.
        public static void RunFirstFivePuzzleRuntimeTests()
        {
            PrepareFirstFivePuzzleRuntimeTestRunner();
            RuntimeTestLaunchGate.RequestRun(RunnerName);
            EditorApplication.isPlaying = true;
            Debug.Log("[FirstFivePuzzleRuntimeTestMenu] Play Mode requested for first five puzzle runtime tests.");
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
                Debug.LogWarning("[FirstFivePuzzleRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
