using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class TitleMenuUI : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button deleteSaveButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Text statusText;
        [SerializeField] private bool refreshOnEnable = true;
        [SerializeField] private string noSaveMessage = "No save data found.";
        [SerializeField] private string saveDeletedMessage = "Save data deleted.";

        private void OnEnable()
        {
            if (newGameButton != null)
            {
                newGameButton.onClick.RemoveListener(HandleNewGameClicked);
                newGameButton.onClick.AddListener(HandleNewGameClicked);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueClicked);
                continueButton.onClick.AddListener(HandleContinueClicked);
            }

            if (deleteSaveButton != null)
            {
                deleteSaveButton.onClick.RemoveListener(HandleDeleteSaveClicked);
                deleteSaveButton.onClick.AddListener(HandleDeleteSaveClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(HandleQuitClicked);
                quitButton.onClick.AddListener(HandleQuitClicked);
            }

            if (refreshOnEnable)
            {
                Refresh();
            }
        }

        private void OnDisable()
        {
            if (newGameButton != null)
            {
                newGameButton.onClick.RemoveListener(HandleNewGameClicked);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueClicked);
            }

            if (deleteSaveButton != null)
            {
                deleteSaveButton.onClick.RemoveListener(HandleDeleteSaveClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(HandleQuitClicked);
            }
        }

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            bool hasSaveData = HasSaveData();

            if (continueButton != null)
            {
                continueButton.interactable = hasSaveData;
            }

            if (deleteSaveButton != null)
            {
                deleteSaveButton.interactable = hasSaveData;
            }

            SetStatus(string.Empty);
        }

        private void HandleNewGameClicked()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager instance is missing.");
                return;
            }

            GameManager.Instance.StartNewGame();
        }

        private void HandleContinueClicked()
        {
            if (!HasSaveData())
            {
                SetStatus(noSaveMessage);
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager instance is missing.");
                return;
            }

            if (!GameManager.Instance.ContinueGame())
            {
                SetStatus(noSaveMessage);
            }
        }

        private void HandleDeleteSaveClicked()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.DeleteSave();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            Refresh();
            SetStatus(saveDeletedMessage);
        }

        private void HandleQuitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
                return;
            }

            Debug.Log("QuitGame requested.");
            Application.Quit();
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private bool HasSaveData()
        {
            return SaveManager.Instance != null && SaveManager.Instance.HasSaveData();
        }
    }
}
