// -----------------------------------------------------------------------------
// Codex comment pass: Game Scene Direct Play Menu
// Role: Provides editor-only manual play entry points without changing the shipped scene flow.
// Scope: This script belongs to Editor\GameSceneDirectPlayMenu.cs and keeps its behavior isolated to editor tooling.
// -----------------------------------------------------------------------------

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    [InitializeOnLoad]
    public static class GameSceneDirectPlayMenu
    {
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string GameSceneName = "GameScene";
        private const string DirectPlaySessionKey = "EscapeFromNightmare.GameSceneDirectPlay.StartFromBeginning";

        static GameSceneDirectPlayMenu()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        [MenuItem("Escape From Nightmare/Play/Start GameScene From Beginning")]
        public static void StartGameSceneFromBeginning()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("[GameSceneDirectPlayMenu] Stop Play Mode before starting a fresh GameScene manual play session.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            ClearRuntimeTestLaunchRequests();
            SessionState.SetBool(DirectPlaySessionKey, true);

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.path != GameScenePath)
            {
                EditorSceneManager.OpenScene(GameScenePath);
            }

            DisableRuntimeTestRunnersInOpenScene();
            EditorApplication.EnterPlaymode();
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredPlayMode)
            {
                return;
            }

            if (!SessionState.GetBool(DirectPlaySessionKey, false))
            {
                return;
            }

            EditorApplication.delayCall += ResetGameSceneAfterPlayModeStarts;
        }

        private static void ResetGameSceneAfterPlayModeStarts()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != GameSceneName)
            {
                Debug.LogWarning("[GameSceneDirectPlayMenu] Direct play reset skipped because the active scene is not GameScene.");
                SessionState.EraseBool(DirectPlaySessionKey);
                return;
            }

            ClearRuntimeTestLaunchRequests();
            DisableRuntimeTestRunnersInOpenScene();
            ResetRuntimeStateForBeginning();
            SessionState.EraseBool(DirectPlaySessionKey);
            Debug.Log("[GameSceneDirectPlayMenu] GameScene reset to the beginning for manual play.");
        }

        private static void ResetRuntimeStateForBeginning()
        {
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.LoadAllData();
            }

            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.StopEnding();
            }

            if (PuzzleManager.Instance != null)
            {
                PuzzleManager.Instance.CloseCurrentPuzzle();
            }

            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.HideCurrentImage();
            }

            HidePanel<GameOverPanelUI>();
            HidePanel<EndingPanelUI>();
            HidePanel<ClueImagePanelUI>();

            if (ChaseManager.Instance != null)
            {
                ChaseManager.Instance.ResetChase();
            }

            if (GhostManager.Instance != null)
            {
                GhostManager.Instance.StopChase();
                GhostManager.Instance.LeaveCurrentLocation();
            }

            if (HideManager.Instance != null && HideManager.Instance.IsHiding)
            {
                HideManager.Instance.ForceExitHidePoint();
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ResetDataForNewGameInMemory();
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ReplaceInventory(new string[0]);
                InventoryManager.Instance.ClearSelectedItem();
            }

            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.LoadUnlockedCluesFromSave();
            }

            if (LocationManager.Instance != null)
            {
                LocationManager.Instance.ResetToStartingLocation();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }
        }

        private static void HidePanel<T>() where T : MonoBehaviour
        {
            T panel = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (panel == null)
            {
                return;
            }

            if (panel is GameOverPanelUI gameOverPanel)
            {
                gameOverPanel.Hide();
            }
            else if (panel is EndingPanelUI endingPanel)
            {
                endingPanel.Hide();
            }
            else if (panel is ClueImagePanelUI clueImagePanel)
            {
                clueImagePanel.Hide();
            }
        }

        private static void DisableRuntimeTestRunnersInOpenScene()
        {
            DisableRunner<FirstFivePuzzleRuntimeTestRunner>();
            DisableRunner<RemainingPuzzleRuntimeTestRunner>();
            DisableRunner<FullGameRouteRuntimeTestRunner>();
            DisableRunner<GameSceneInteractionRuntimeTestRunner>();
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
                return;
            }

            property.boolValue = false;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ClearRuntimeTestLaunchRequests()
        {
            RuntimeTestLaunchGate.ClearRun(nameof(FirstFivePuzzleRuntimeTestRunner));
            RuntimeTestLaunchGate.ClearRun(nameof(RemainingPuzzleRuntimeTestRunner));
            RuntimeTestLaunchGate.ClearRun(nameof(FullGameRouteRuntimeTestRunner));
            RuntimeTestLaunchGate.ClearRun(nameof(GameSceneInteractionRuntimeTestRunner));
        }
    }
}
