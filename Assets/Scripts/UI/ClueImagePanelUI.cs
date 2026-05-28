using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class ClueImagePanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Image clueImage;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button closeButton;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private string defaultLockedTitle = "Locked";
        [SerializeField] private string defaultLockedMessage = "You cannot examine this yet.";

        public bool IsVisible
        {
            get
            {
                GameObject target = rootObject != null ? rootObject : gameObject;
                return target.activeSelf;
            }
        }

        private void Awake()
        {
            EnsureRootObject();

            if (hideOnAwake)
            {
                Hide();
            }
        }

        private void OnEnable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
                closeButton.onClick.AddListener(HandleCloseClicked);
            }
        }

        private void OnDisable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        private void Reset()
        {
            EnsureRootObject();

            if (closeButton == null)
            {
                closeButton = GetComponentInChildren<Button>(true);
            }
        }

        public void ShowClue(ClueRecord record, Sprite sprite)
        {
            if (record == null)
            {
                ShowMessageOnly("Missing Clue", "Clue data was not found.");
                return;
            }

            SetRootActive(true);

            if (clueImage != null)
            {
                clueImage.sprite = sprite;
                clueImage.enabled = sprite != null;
            }

            if (titleText != null)
            {
                titleText.text = GetDisplayTitle(record);
            }

            if (descriptionText != null)
            {
                descriptionText.text = GetDescription(record);
            }

            if (messageText != null)
            {
                messageText.text = string.Empty;
            }
        }

        public void ShowLockedMessage(string message)
        {
            SetRootActive(true);

            if (clueImage != null)
            {
                clueImage.sprite = null;
                clueImage.enabled = false;
            }

            if (titleText != null)
            {
                titleText.text = defaultLockedTitle;
            }

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (messageText != null)
            {
                messageText.text = !string.IsNullOrEmpty(message) ? message : defaultLockedMessage;
            }
        }

        public void ShowMessageOnly(string title, string message)
        {
            SetRootActive(true);

            if (clueImage != null)
            {
                clueImage.sprite = null;
                clueImage.enabled = false;
            }

            if (titleText != null)
            {
                titleText.text = title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        public void Hide()
        {
            if (clueImage != null)
            {
                clueImage.sprite = null;
                clueImage.enabled = false;
            }

            if (titleText != null)
            {
                titleText.text = string.Empty;
            }

            if (descriptionText != null)
            {
                descriptionText.text = string.Empty;
            }

            if (messageText != null)
            {
                messageText.text = string.Empty;
            }

            SetRootActive(false);
        }

        private void HandleCloseClicked()
        {
            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.HideCurrentImage();
                return;
            }

            Hide();
        }

        private void SetRootActive(bool active)
        {
            EnsureRootObject();
            rootObject.SetActive(active);
        }

        private string GetDisplayTitle(ClueRecord record)
        {
            if (record == null)
            {
                return string.Empty;
            }

            return !string.IsNullOrEmpty(record.displayName) ? record.displayName : record.clueId;
        }

        private string GetDescription(ClueRecord record)
        {
            return record != null && !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
        }

        private void EnsureRootObject()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }
    }
}
