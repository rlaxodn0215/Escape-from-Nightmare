using EscapeFromNightmares.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class PauseUI : MonoBehaviour
    {
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private TitleUI titleUI;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button returnTitleButton;
        [SerializeField] private TMP_Text titleLabel;

        private void Awake()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            settingsUI ??= FindFirstObjectByType<SettingsUI>(FindObjectsInactive.Include);
            titleUI ??= FindFirstObjectByType<TitleUI>(FindObjectsInactive.Include);
            canvasGroup ??= GetComponent<CanvasGroup>();

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(ContinueGame);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OpenSettings);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.AddListener(ReturnToTitle);
            }
        }

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(ContinueGame);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(OpenSettings);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.RemoveListener(ReturnToTitle);
            }
        }

        public void Bind(GameStateManager nextGameStateManager, SettingsUI nextSettingsUI, TitleUI nextTitleUI)
        {
            gameStateManager = nextGameStateManager;
            settingsUI = nextSettingsUI;
            titleUI = nextTitleUI;
        }

        public void Show()
        {
            gameStateManager?.SetPaused(true);
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void ContinueGame()
        {
            gameStateManager?.SetPaused(false);
            Hide();
        }

        private void OpenSettings()
        {
            settingsUI?.Show();
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
