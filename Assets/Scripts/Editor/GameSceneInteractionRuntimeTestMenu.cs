// -----------------------------------------------------------------------------
// Codex comment pass: Game Scene Interaction Runtime Test Menu
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\GameSceneInteractionRuntimeTestMenu.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Editor utility for the Game Scene Interaction Runtime Test Menu workflow, exposed through menu items or called by other validation tools.
    public static class GameSceneInteractionRuntimeTestMenu
    {
        // Stores the Game Scene Path value used by this script's runtime or editor workflow.
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        // Stores the Runner Name value used by this script's runtime or editor workflow.
        private const string RunnerName = "GameSceneInteractionRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare GameScene Interaction Runtime Test Runner")]
        // Performs the Prepare Game Scene Interaction Runtime Test Runner operation while keeping its implementation details inside this script.
        public static void PrepareGameSceneInteractionRuntimeTestRunner()
        {
            EnsureGameSceneOpen();
            DisableOtherRuntimeTestRunners();

            GameSceneInteractionRuntimeTestRunner runner = Object.FindFirstObjectByType<GameSceneInteractionRuntimeTestRunner>(FindObjectsInactive.Include);
            if (runner == null)
            {
                GameObject runnerObject = new GameObject(RunnerName);
                runner = runnerObject.AddComponent<GameSceneInteractionRuntimeTestRunner>();
                Undo.RegisterCreatedObjectUndo(runnerObject, "Create GameScene Interaction Runtime Test Runner");
            }

            SetBoolField(runner, "runOnStart", false);
            SetBoolField(runner, "testHidePointButtons", true);
            SetFloatField(runner, "waitAfterClickSeconds", 1.35f);
            SetFloatField(runner, "waitAfterOpenSeconds", 1.35f);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[GameSceneInteractionRuntimeTestMenu] Prepared GameSceneInteractionRuntimeTestRunner for manual selection. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run GameScene Interaction Runtime Tests")]
        // Performs the Run Game Scene Interaction Runtime Tests operation while keeping its implementation details inside this script.
        public static void RunGameSceneInteractionRuntimeTests()
        {
            PrepareGameSceneInteractionRuntimeTestRunner();
            RuntimeTestLaunchGate.RequestRun(RunnerName);
            EditorApplication.isPlaying = true;
            Debug.Log("[GameSceneInteractionRuntimeTestMenu] Play Mode requested for GameScene interaction runtime tests.");
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private static void EnsureGameSceneOpen()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid() && activeScene.name == "GameScene")
            {
                return;
            }

            if (!System.IO.File.Exists(GameScenePath))
            {
                Debug.LogError("[GameSceneInteractionRuntimeTestMenu] GameScene was not found: " + GameScenePath);
                return;
            }

            EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
        }

        // Performs the Disable Other Runtime Test Runners operation while keeping its implementation details inside this script.
        private static void DisableOtherRuntimeTestRunners()
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

            FullGameRouteRuntimeTestRunner fullRouteRunner = Object.FindFirstObjectByType<FullGameRouteRuntimeTestRunner>(FindObjectsInactive.Include);
            if (fullRouteRunner != null)
            {
                SetBoolField(fullRouteRunner, "runOnStart", false);
            }
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
                Debug.LogWarning("[GameSceneInteractionRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloatField(Object target, string fieldName, float value)
        {
            if (target == null || string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogWarning("[GameSceneInteractionRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
