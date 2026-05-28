using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private string titleSceneName = "TitleScene";
        [SerializeField] private string gameSceneName = "GameScene";
        [SerializeField] private bool useSceneNamesFromSettings = true;
        [SerializeField] private bool loadTitleOnGameOverReturn = true;
        [SerializeField] private GameState currentState = GameState.None;

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

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            ApplySettings();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

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

        public void StartGame()
        {
            StartNewGame();
        }

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

        public void GameOver()
        {
            SetState(GameState.GameOver);
            Debug.Log("Game over.");
        }

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

        public void LoadTitleScene()
        {
            string sceneName = !string.IsNullOrEmpty(titleSceneName) ? titleSceneName : "TitleScene";
            Debug.Log("Loading title scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }

        public void LoadGameScene(GameStartMode mode)
        {
            PrepareGameSceneStart(mode);

            string sceneName = !string.IsNullOrEmpty(gameSceneName) ? gameSceneName : "GameScene";
            Debug.Log("Loading game scene: " + sceneName + ", Mode: " + mode);
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Debug.Log("QuitGame requested.");
            Application.Quit();
        }

        private void PrepareGameSceneStart(GameStartMode mode)
        {
            pendingStartMode = mode;
            RaiseGameStartModeChanged();
        }

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

        private bool IsCurrentScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            return activeScene.IsValid() && activeScene.name == sceneName;
        }

        private void RaiseGameStartModeChanged()
        {
            if (GameStartModeChanged != null)
            {
                GameStartModeChanged(pendingStartMode);
            }
        }
    }
}
