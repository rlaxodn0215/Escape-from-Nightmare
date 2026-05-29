using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    public static class FullGameRouteRuntimeTestMenu
    {
        private const string RunnerName = "FullGameRouteRuntimeTestRunner";

        [MenuItem("Escape From Nightmare/Tests/Prepare Full Game Route Runtime Test Runner")]
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

            SetBoolField(runner, "runOnStart", true);
            Selection.activeGameObject = runner.gameObject;
            Debug.Log("[FullGameRouteRuntimeTestMenu] Prepared FullGameRouteRuntimeTestRunner. Scene was not saved automatically.");
        }

        [MenuItem("Escape From Nightmare/Tests/Run Full Game Route Runtime Test")]
        public static void RunFullGameRouteRuntimeTest()
        {
            PrepareFullGameRouteRuntimeTestRunner();
            EditorApplication.isPlaying = true;
            Debug.Log("[FullGameRouteRuntimeTestMenu] Play Mode requested for full game route runtime test.");
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
                Debug.LogWarning("[FullGameRouteRuntimeTestMenu] Serialized field not found: " + fieldName);
                return;
            }

            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
