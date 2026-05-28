using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class HideExitButton : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private bool showOnlyWhileHiding = true;

        private Button button;

        private void Awake()
        {
            CacheButton();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        private void OnEnable()
        {
            CacheButton();
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }

            RefreshVisibility();
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void Update()
        {
            RefreshVisibility();
        }

        private void Reset()
        {
            CacheButton();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        private void HandleClick()
        {
            if (HideManager.Instance == null)
            {
                Debug.LogWarning("HideManager instance is missing.");
                return;
            }

            HideManager.Instance.ExitHidePoint();
        }

        private void RefreshVisibility()
        {
            if (rootObject == null || !showOnlyWhileHiding)
            {
                return;
            }

            bool shouldShow = HideManager.Instance != null && HideManager.Instance.IsHiding;
            if (rootObject.activeSelf != shouldShow)
            {
                rootObject.SetActive(shouldShow);
            }
        }

        private void CacheButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }
    }
}
