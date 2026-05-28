using UnityEngine;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(ClickableButton))]
    public class HidePointController : MonoBehaviour
    {
        [SerializeField] private string hidePointId;
        [SerializeField] private GameObject rootObject;
        [SerializeField] private bool usable = true;

        private ClickableButton clickableButton;

        public string HidePointId
        {
            get { return hidePointId; }
        }

        public bool Usable
        {
            get { return usable; }
        }

        private void Awake()
        {
            clickableButton = GetComponent<ClickableButton>();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        private void Reset()
        {
            clickableButton = GetComponent<ClickableButton>();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        private void OnValidate()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            if (clickableButton == null)
            {
                clickableButton = GetComponent<ClickableButton>();
            }

            if (clickableButton != null && clickableButton.ClickableType != ClickableType.HidePoint)
            {
                Debug.LogWarning("HidePointController should be used with ClickableType.HidePoint.", this);
            }
        }

        public void SetUsable(bool value)
        {
            usable = value;
        }

        public string GetResolvedHidePointId()
        {
            if (!string.IsNullOrEmpty(hidePointId))
            {
                return hidePointId;
            }

            if (clickableButton == null)
            {
                clickableButton = GetComponent<ClickableButton>();
            }

            if (clickableButton != null)
            {
                if (!string.IsNullOrEmpty(clickableButton.TargetObjectId))
                {
                    return clickableButton.TargetObjectId;
                }

                if (!string.IsNullOrEmpty(clickableButton.ClickableId))
                {
                    return clickableButton.ClickableId;
                }
            }

            return gameObject.name;
        }
    }
}
