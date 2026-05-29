using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    public static class GameSceneInteractionRuntimeTestMenu
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string RunnerName = "GameSceneInteractionRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare GameScene Interaction Runtime Test Runner")]
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

            SetBoolField(runner, "runOnStart", true);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[GameSceneInteractionRuntimeTestMenu] Prepared GameSceneInteractionRuntimeTestRunner. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run GameScene Interaction Runtime Tests")]
        public static void RunGameSceneInteractionRuntimeTests()
        {
            PrepareGameSceneInteractionRuntimeTestRunner();
            EditorApplication.isPlaying = true;
            Debug.Log("[GameSceneInteractionRuntimeTestMenu] Play Mode requested for GameScene interaction runtime tests.");
        }

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
    }
}
