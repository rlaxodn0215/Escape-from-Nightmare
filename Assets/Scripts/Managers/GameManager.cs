// -----------------------------------------------------------------------------
// Codex comment pass: Game Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\GameManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Runtime owner for the Game Manager system, keeping shared state and events behind one access point.
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private string titleSceneName = "TitleScene";
        [SerializeField] private string gameSceneName = "GameScene";
        [SerializeField] private bool useSceneNamesFromSettings = true;
        [SerializeField] private bool loadTitleOnGameOverReturn = true;
        [SerializeField] private GameState currentState = GameState.None;

        // Stores the pending Start Mode value used by this script's runtime or editor workflow.
        private GameStartMode pendingStartMode = GameStartMode.None;

        protected override bool UseDontDestroyOnLoad
        {
            get { return true; }
        }

        public GameState CurrentState
        {
            get { return currentState; }
        }

        public GameStartMode PendingStartMode
        {
            get { return pendingStartMode; }
        }

        public string TitleSceneName
        {
            get { return titleSceneName; }
        }

        public string GameSceneName
        {
            get { return gameSceneName; }
        }

        public event Action<GameState> StateChanged;
        public event Action<GameStartMode> GameStartModeChanged;

        // Caches required component references and prepares this object before other startup code runs.
        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            ApplySettings();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (Instance != this)
            {
                return;
            }

            ApplySettings();

            Scene activeScene = SceneManager.GetActiveScene();
            if (IsCurrentScene(titleSceneName))
            {
                SetState(GameState.Title);
            }
            else if (activeScene.IsValid() && activeScene.name == gameSceneName && currentState == GameState.None)
            {
                SetState(GameState.Playing);
            }
        }

        // Performs the Handle Scene Loaded operation while keeping its implementation details inside this script.
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Instance != this)
            {
                return;
            }

            ApplySettings();

            if (scene.name == titleSceneName)
            {
                pendingStartMode = GameStartMode.None;
                RaiseGameStartModeChanged();
                SetState(GameState.Title);
                return;
            }

            if (scene.name == gameSceneName)
            {
                FinalizeGameSceneStart();
            }
        }

        // Applies calculated settings to Unity components or runtime state.
        private void ApplySettings()
        {
            if (!useSceneNamesFromSettings || GameDataManager.Instance == null || GameDataManager.Instance.Settings == null)
            {
                return;
            }

            GameSettingsRecord settings = GameDataManager.Instance.Settings;
            if (!string.IsNullOrEmpty(settings.titleSceneName))
            {
                titleSceneName = settings.titleSceneName;
            }

            if (!string.IsNullOrEmpty(settings.gameSceneName))
            {
                gameSceneName = settings.gameSceneName;
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetState(GameState newState)
        {
            if (currentState == newState)
            {
                return;
            }

            currentState = newState;

            if (StateChanged != null)
            {
                StateChanged(currentState);
            }

            Debug.Log("Game state changed: " + newState);
        }

        // Begins this system's runtime flow and initializes any timers, events, or counters it needs.
        public void StartGame()
        {
            StartNewGame();
        }

        // Begins this system's runtime flow and initializes any timers, events, or counters it needs.
        public void StartNewGame()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ResetDataForNewGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            LoadGameScene(GameStartMode.NewGame);
        }

        // Performs the Continue Game operation while keeping its implementation details inside this script.
        public bool ContinueGame()
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("SaveManager instance is missing.");
                return false;
            }

            if (!SaveManager.Instance.HasSaveData())
            {
                Debug.LogWarning("Cannot continue because no save data exists.");
                return false;
            }

            LoadGameScene(GameStartMode.Continue);
            return true;
        }

        // Performs the Restart From Checkpoint operation while keeping its implementation details inside this script.
        public void RestartFromCheckpoint()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
            {
                LoadGameScene(GameStartMode.RestartFromCheckpoint);
                return;
            }

            Debug.LogWarning("No checkpoint save found. Starting a new game.");
            StartNewGame();
        }

        // Performs the Game Over operation while keeping its implementation details inside this script.
        public void GameOver()
        {
            SetState(GameState.GameOver);
            Debug.Log("Game over.");
        }

        // Performs the Enter Ending operation while keeping its implementation details inside this script.
        public void EnterEnding()
        {
            SetState(GameState.Ending);

            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.PlayEnding();
            }
            else
            {
                Debug.LogWarning("EndingManager instance is missing.");
            }
        }

        // Performs the Return To Title operation while keeping its implementation details inside this script.
        public void ReturnToTitle()
        {
            pendingStartMode = GameStartMode.None;
            RaiseGameStartModeChanged();

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.AutoSaveIfPossible();
            }

            if (loadTitleOnGameOverReturn)
            {
                LoadTitleScene();
            }
            else
            {
                SetState(GameState.Title);
            }
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        public void LoadTitleScene()
        {
            string sceneName = !string.IsNullOrEmpty(titleSceneName) ? titleSceneName : "TitleScene";
            Debug.Log("Loading title scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        public void LoadGameScene(GameStartMode mode)
        {
            PrepareGameSceneStart(mode);

            string sceneName = !string.IsNullOrEmpty(gameSceneName) ? gameSceneName : "GameScene";
            Debug.Log("Loading game scene: " + sceneName + ", Mode: " + mode);
            SceneManager.LoadScene(sceneName);
        }

        // Performs the Quit Game operation while keeping its implementation details inside this script.
        public void QuitGame()
        {
            Debug.Log("QuitGame requested.");
            Application.Quit();
        }

        // Performs the Prepare Game Scene Start operation while keeping its implementation details inside this script.
        private void PrepareGameSceneStart(GameStartMode mode)
        {
            pendingStartMode = mode;
            RaiseGameStartModeChanged();
        }

        // Performs the Finalize Game Scene Start operation while keeping its implementation details inside this script.
        private void FinalizeGameSceneStart()
        {
            ApplySettings();

            if (SaveManager.Instance != null)
            {
                if (pendingStartMode == GameStartMode.Continue || pendingStartMode == GameStartMode.RestartFromCheckpoint)
                {
                    if (!SaveManager.Instance.TryLoadGame())
                    {
                        Debug.LogWarning("Save load failed. Resetting to new game data.");
                        SaveManager.Instance.ResetDataForNewGame();
                    }
                }
                else if (pendingStartMode == GameStartMode.NewGame)
                {
                    SaveManager.Instance.MarkGameStarted();
                }
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            SetState(GameState.Playing);

            if (LocationManager.Instance != null)
            {
                LocationManager.Instance.ApplySavedPositionOrStartingLocation();
            }
            else
            {
                Debug.LogWarning("LocationManager instance is missing.");
            }

            pendingStartMode = GameStartMode.None;
            RaiseGameStartModeChanged();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsCurrentScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            return activeScene.IsValid() && activeScene.name == sceneName;
        }

        // Performs the Raise Game Start Mode Changed operation while keeping its implementation details inside this script.
        private void RaiseGameStartModeChanged()
        {
            if (GameStartModeChanged != null)
            {
                GameStartModeChanged(pendingStartMode);
            }
        }
    }
}
