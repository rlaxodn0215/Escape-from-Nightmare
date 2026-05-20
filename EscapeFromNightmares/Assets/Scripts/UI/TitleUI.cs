using EscapeFromNightmares.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class TitleUI : MonoBehaviour
    {
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private SceneFlowController sceneFlowController;
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text titleLabel;

        private void Awake()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            sceneFlowController ??= FindFirstObjectByType<SceneFlowController>();
            settingsUI ??= FindFirstObjectByType<SettingsUI>(FindObjectsInactive.Include);
            canvasGroup ??= GetComponent<CanvasGroup>();

            if (startButton != null)
            {
                startButton.onClick.AddListener(StartGame);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OpenSettings);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }

            gameStateManager?.SetState(GameRunState.Title);
        }

        private void OnDestroy()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(StartGame);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(OpenSettings);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(QuitGame);
            }
        }

        public void Bind(GameStateManager nextGameStateManager, SceneFlowController nextSceneFlowController, SettingsUI nextSettingsUI)
        {
            gameStateManager = nextGameStateManager;
            sceneFlowController = nextSceneFlowController;
            settingsUI = nextSettingsUI;
        }

        public void Show()
        {
            SetVisible(true);
            gameStateManager?.SetState(GameRunState.Title);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void StartGame()
        {
            gameStateManager?.StartStage1Run();
            Hide();
        }

        private void OpenSettings()
        {
            settingsUI?.Show();
        }

        private void QuitGame()
        {
            sceneFlowController?.QuitGame();
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(true);

            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
