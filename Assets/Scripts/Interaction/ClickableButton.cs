using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class ClickableButton : MonoBehaviour
    {
        public string clickableId;
        public ClickableType clickableType;
        public string linkedLocationId;
        public string linkedViewId;
        public string linkedDoorId;
        public string linkedPuzzleId;
        public string linkedClueImageId;
        public string linkedItemId;
        public string requiredItemId;
        public string targetObjectId;

        private Button button;

        public string ClickableId
        {
            get { return clickableId; }
        }

        public ClickableType ClickableType
        {
            get { return clickableType; }
        }

        public string LinkedLocationId
        {
            get { return linkedLocationId; }
        }

        public string LinkedViewId
        {
            get { return linkedViewId; }
        }

        public string LinkedDoorId
        {
            get { return linkedDoorId; }
        }

        public string LinkedPuzzleId
        {
            get { return linkedPuzzleId; }
        }

        public string LinkedClueImageId
        {
            get { return linkedClueImageId; }
        }

        public string LinkedItemId
        {
            get { return linkedItemId; }
        }

        public string RequiredItemId
        {
            get { return requiredItemId; }
        }

        public string TargetObjectId
        {
            get { return targetObjectId; }
        }

        private void Awake()
        {
            CacheButton();
        }

        private void Reset()
        {
            CacheButton();
        }

        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
                button.onClick.AddListener(OnClicked);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
            }
        }

        private void CacheButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnClicked()
        {
            if (InteractionManager.Instance == null)
            {
                Debug.LogWarning("InteractionManager instance is missing.");
                return;
            }

            InteractionManager.Instance.HandleClick(this);
        }
    }
}
