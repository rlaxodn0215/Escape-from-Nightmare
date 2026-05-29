using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    public static class RemainingPuzzleRuntimeTestMenu
    {
        private const string RunnerName = "RemainingPuzzleRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare Remaining Puzzle Runtime Test Runner")]
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

            SetBoolField(runner, "runOnStart", true);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[RemainingPuzzleRuntimeTestMenu] Prepared RemainingPuzzleRuntimeTestRunner. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run Remaining Puzzle Runtime Tests")]
        public static void RunRemainingPuzzleRuntimeTests()
        {
            PrepareRemainingPuzzleRuntimeTestRunner();
            EditorApplication.isPlaying = true;
            Debug.Log("[RemainingPuzzleRuntimeTestMenu] Play Mode requested for remaining puzzle runtime tests.");
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
                Debug.LogWarning("[RemainingPuzzleRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
