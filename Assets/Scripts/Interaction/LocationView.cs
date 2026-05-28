using UnityEngine;

namespace EscapeFromNightmare
{
    public class LocationView : MonoBehaviour
    {
        [SerializeField] private string viewId;
        [SerializeField] private GameObject rootObject;

        public string ViewId
        {
            get { return viewId; }
        }

        public bool IsActive
        {
            get
            {
                GameObject target = rootObject != null ? rootObject : gameObject;
                return target.activeSelf;
            }
        }

        public void SetActive(bool active)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            rootObject.SetActive(active);
        }

        public bool Matches(string targetViewId)
        {
            if (string.IsNullOrEmpty(targetViewId))
            {
                return false;
            }

            return viewId == targetViewId;
        }

        private void Reset()
        {
            EnsureRootObject();
            WarnIfMissingViewId();
        }

        private void OnValidate()
        {
            EnsureRootObject();
            WarnIfMissingViewId();
        }

        private void EnsureRootObject()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        private void WarnIfMissingViewId()
        {
            if (string.IsNullOrEmpty(viewId))
            {
                Debug.LogWarning("LocationView has an empty viewId: " + name, this);
            }
        }
    }
}
