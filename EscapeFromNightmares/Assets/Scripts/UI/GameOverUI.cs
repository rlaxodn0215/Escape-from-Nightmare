using EscapeFromNightmares.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private TitleUI titleUI;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnTitleButton;
        [SerializeField] private Image gameOverTextImage;
        [SerializeField] private TMP_Text hintLabel;

        private void Awake()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            titleUI ??= FindFirstObjectByType<TitleUI>(FindObjectsInactive.Include);
            canvasGroup ??= GetComponent<CanvasGroup>();

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(Restart);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.AddListener(ReturnToTitle);
            }

            if (gameStateManager != null)
            {
                gameStateManager.StateChanged += HandleStateChanged;
                HandleStateChanged(gameStateManager.CurrentState);
            }
        }

        private void OnDestroy()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(Restart);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.RemoveListener(ReturnToTitle);
            }

            if (gameStateManager != null)
            {
                gameStateManager.StateChanged -= HandleStateChanged;
            }
        }

        public void Bind(GameStateManager nextGameStateManager, TitleUI nextTitleUI)
        {
            if (gameStateManager != null)
            {
                gameStateManager.StateChanged -= HandleStateChanged;
            }

            gameStateManager = nextGameStateManager;
            titleUI = nextTitleUI;

            if (gameStateManager != null)
            {
                gameStateManager.StateChanged += HandleStateChanged;
                HandleStateChanged(gameStateManager.CurrentState);
            }
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void HandleStateChanged(GameRunState state)
        {
            SetVisible(state == GameRunState.GameOver);
        }

        private void Restart()
        {
            gameStateManager?.StartStage1Run();
            Hide();
        }

        private void ReturnToTitle()
        {
            Hide();
            titleUI?.Show();
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
