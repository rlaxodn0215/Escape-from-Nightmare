using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    public static class FirstFivePuzzleRuntimeTestMenu
    {
        private const string RunnerName = "FirstFivePuzzleRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare First Five Puzzle Runtime Test Runner")]
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

            SetBoolField(runner, "runOnStart", true);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[FirstFivePuzzleRuntimeTestMenu] Prepared FirstFivePuzzleRuntimeTestRunner. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run First Five Puzzle Runtime Tests")]
        public static void RunFirstFivePuzzleRuntimeTests()
        {
            PrepareFirstFivePuzzleRuntimeTestRunner();
            EditorApplication.isPlaying = true;
            Debug.Log("[FirstFivePuzzleRuntimeTestMenu] Play Mode requested for first five puzzle runtime tests.");
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
                Debug.LogWarning("[FirstFivePuzzleRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
