using System;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class EndingPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Text titleText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button skipButton;
        [SerializeField] private bool hideOnAwake = true;
        [SerializeField] private string defaultTitle = "Ending";
        [SerializeField] private string defaultMessage = "You escaped from the nightmare.";

        public event Action SkipRequested;

        public bool IsVisible
        {
            get { return rootObject != null && rootObject.activeSelf; }
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
            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(HandleSkipClicked);
                skipButton.onClick.AddListener(HandleSkipClicked);
            }
        }

        private void OnDisable()
        {
            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(HandleSkipClicked);
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
            Show(defaultTitle, defaultMessage);
        }

        public void Show(string title, string message)
        {
            SetRootActive(true);

            if (titleText != null)
            {
                titleText.text = !string.IsNullOrEmpty(title) ? title : defaultTitle;
            }

            if (messageText != null)
            {
                messageText.text = !string.IsNullOrEmpty(message) ? message : defaultMessage;
            }
        }

        public void Hide()
        {
            SetRootActive(false);
        }

        private void HandleSkipClicked()
        {
            if (SkipRequested != null)
            {
                SkipRequested();
            }
        }

        private void SetRootActive(bool active)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            rootObject.SetActive(active);
        }
    }
}
