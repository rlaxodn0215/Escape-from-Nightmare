using System;
using System.Collections;
using System.Linq;
using EscapeFromNightmares.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Core
{
    public sealed class GeneratedPlayerSmokeRunner : MonoBehaviour
    {
        public const string SmokeArgument = "--stage1-smoke";
        public const string SuccessLogLine = "STAGE1_SMOKE_SUCCESS";
        public const string FailureLogPrefix = "STAGE1_SMOKE_FAILED";

        private const string Stage1SceneName = "Stage1";
        private const string StartRoomId = "child_room";
        private const float TimeoutSeconds = 10f;

        private static bool created;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateForCommandLine()
        {
            if (created || !HasSmokeArgument(Environment.GetCommandLineArgs()))
            {
                return;
            }

            created = true;
            GameObject runnerObject = new GameObject(nameof(GeneratedPlayerSmokeRunner));
            DontDestroyOnLoad(runnerObject);
            runnerObject.hideFlags = HideFlags.HideAndDontSave;
            runnerObject.AddComponent<GeneratedPlayerSmokeRunner>();
        }

        public static bool HasSmokeArgument(string[] args)
        {
            return args != null && args.Any(arg => string.Equals(arg, SmokeArgument, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TryValidateLoadedStage1(out string details)
        {
            return TryValidateLoadedStage1(startRun: true, out details);
        }

        public static bool TryValidateLoadedStage1(bool startRun, out string details)
        {
            string[] requiredRoots =
            {
                "StageRoot",
                "Systems",
                "RoomView",
                "InteractableLayer",
                "MonsterLayer",
                "AudioRoot",
                "UICanvas",
                "EventSystem"
            };

            foreach (string root in requiredRoots)
            {
                if (GameObject.Find(root) == null)
                {
                    details = $"Missing required root '{root}'.";
                    return false;
                }
            }

            GameStateManager gameStateManager = FindFirstObjectByType<GameStateManager>();
            RoomSystem roomSystem = FindFirstObjectByType<RoomSystem>();
            if (gameStateManager == null || roomSystem == null)
            {
                details = "Missing GameStateManager or RoomSystem.";
                return false;
            }

            if (startRun)
            {
                gameStateManager.StartStage1Run();
            }

            string currentRoomId = roomSystem.CurrentRoom != null ? roomSystem.CurrentRoom.RoomId : null;
            if (!string.Equals(gameStateManager.CurrentRoomId, StartRoomId, StringComparison.Ordinal) ||
                !string.Equals(currentRoomId, StartRoomId, StringComparison.Ordinal))
            {
                details = $"Expected '{StartRoomId}', got GameStateManager='{gameStateManager.CurrentRoomId}', RoomSystem='{currentRoomId ?? "<null>"}'.";
                return false;
            }

            string[] requiredSystems =
            {
                nameof(GameBootstrap),
                nameof(SceneFlowController),
                nameof(InputRouter),
                nameof(SaveManager),
                nameof(InteractionSystem),
                nameof(InventorySystem),
                nameof(PuzzleSystem),
                nameof(EventRuntimeSystem),
                nameof(SoundRuntimeSystem),
                nameof(HidingRuntimeSystem),
                nameof(MonsterRuntimeSystem),
                nameof(MapRuntimeSystem)
            };

            foreach (string systemName in requiredSystems)
            {
                if (FindComponentByTypeName(systemName) == null)
                {
                    details = $"Missing required system '{systemName}'.";
                    return false;
                }
            }

            details = $"Stage1 initialized in '{StartRoomId}' with required roots and systems present.";
            return true;
        }

        private IEnumerator Start()
        {
            if (SceneManager.GetActiveScene().name != Stage1SceneName)
            {
                SceneManager.LoadScene(Stage1SceneName);
                yield return null;
            }

            float deadline = Time.realtimeSinceStartup + TimeoutSeconds;
            string details = "Stage1 validation did not run.";
            while (Time.realtimeSinceStartup <= deadline)
            {
                yield return null;
                yield return null;

                if (TryValidateLoadedStage1(out details))
                {
                    Debug.Log($"{SuccessLogLine}: {details}");
                    Quit(0);
                    yield break;
                }
            }

            Debug.LogError($"{FailureLogPrefix}: {details}");
            Quit(1);
        }

        private static Component FindComponentByTypeName(string typeName)
        {
            Component[] components = FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            return components.FirstOrDefault(component => component != null && component.GetType().Name == typeName);
        }

        private static void Quit(int exitCode)
        {
#if UNITY_EDITOR
            Debug.Log($"Generated player smoke requested quit with exit code {exitCode}.");
#else
            Application.Quit(exitCode);
#endif
        }
    }
}
