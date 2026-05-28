using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class GameOverPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Text messageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnTitleButton;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private string defaultMessage = "You were caught.";

        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
        }

        private void Awake()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            if (hideOnAwake)
            {
                Hide();
            }
        }

        private void OnEnable()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
                restartButton.onClick.AddListener(HandleRestartClicked);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.RemoveListener(HandleReturnTitleClicked);
                returnTitleButton.onClick.AddListener(HandleReturnTitleClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged -= HandleGameStateChanged;
                GameManager.Instance.StateChanged += HandleGameStateChanged;
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void OnDisable()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
            }

            if (returnTitleButton != null)
            {
                returnTitleButton.onClick.RemoveListener(HandleReturnTitleClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged -= HandleGameStateChanged;
            }
        }

        private void Reset()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        public void Show()
        {
            SetRootActive(true);

            if (messageText != null)
            {
                messageText.text = defaultMessage;
            }
        }

        public void Hide()
        {
            SetRootActive(false);
        }

        private void HandleRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartFromCheckpoint();
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }
        }

        private void HandleReturnTitleClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToTitle();
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                Show();
                return;
            }

            Hide();
        }

        private void SetRootActive(bool active)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            isVisible = active;
            if (rootObject == gameObject)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(active);
                }

                return;
            }

            rootObject.SetActive(active);
        }
    }
}
