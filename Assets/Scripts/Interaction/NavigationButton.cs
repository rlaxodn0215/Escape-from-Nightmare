using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        [SerializeField] private NavigationActionType actionType;
        [SerializeField] private string targetLocationId;
        [SerializeField] private string targetViewId;

        private Button button;

        public NavigationActionType ActionType
        {
            get { return actionType; }
        }

        public string TargetLocationId
        {
            get { return targetLocationId; }
        }

        public string TargetViewId
        {
            get { return targetViewId; }
        }

        private void Awake()
        {
            CacheButton();
        }

        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void Reset()
        {
            CacheButton();
        }

        private void HandleClick()
        {
            if (LocationManager.Instance == null)
            {
                Debug.LogWarning("LocationManager instance is missing.");
                return;
            }

            switch (actionType)
            {
                case NavigationActionType.RotateLeft:
                    LocationManager.Instance.RotateLeft();
                    break;
                case NavigationActionType.RotateRight:
                    LocationManager.Instance.RotateRight();
                    break;
                case NavigationActionType.SetLocation:
                    if (string.IsNullOrEmpty(targetLocationId))
                    {
                        Debug.LogWarning("NavigationButton targetLocationId is empty: " + name, this);
                        return;
                    }

                    LocationManager.Instance.SetLocation(targetLocationId, targetViewId);
                    break;
                case NavigationActionType.SetView:
                    if (string.IsNullOrEmpty(targetViewId))
                    {
                        Debug.LogWarning("NavigationButton targetViewId is empty: " + name, this);
                        return;
                    }

                    LocationManager.Instance.SetView(targetViewId);
                    break;
                default:
                    Debug.LogWarning("Unhandled navigation action type: " + actionType, this);
                    break;
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
