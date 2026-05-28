using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private string locationId;
        [SerializeField] private string defaultViewId;
        [SerializeField] private List<LocationView> views = new List<LocationView>();

        private int currentViewIndex = -1;

        public string LocationId
        {
            get { return locationId; }
        }

        public string DefaultViewId
        {
            get { return defaultViewId; }
        }

        public string CurrentViewId
        {
            get
            {
                if (currentViewIndex < 0 || currentViewIndex >= views.Count || views[currentViewIndex] == null)
                {
                    return null;
                }

                return views[currentViewIndex].ViewId;
            }
        }

        public IReadOnlyList<LocationView> Views
        {
            get { return views; }
        }

        public void CacheViews()
        {
            LocationView[] foundViews = GetComponentsInChildren<LocationView>(true);
            views.Clear();

            for (int i = 0; i < foundViews.Length; i++)
            {
                if (foundViews[i] != null && !views.Contains(foundViews[i]))
                {
                    views.Add(foundViews[i]);
                }
            }

            ValidateIds();
        }

        public void SetLocationActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public bool ActivateDefaultView()
        {
            EnsureViews();

            if (!string.IsNullOrEmpty(defaultViewId) && HasView(defaultViewId))
            {
                return ActivateView(defaultViewId);
            }

            if (views.Count > 0 && views[0] != null)
            {
                currentViewIndex = 0;
                RefreshViewVisibility();
                return true;
            }

            Debug.LogWarning("Location has no views: " + locationId, this);
            return false;
        }

        public bool ActivateView(string viewId)
        {
            EnsureViews();

            int index = FindViewIndex(viewId);
            if (index < 0)
            {
                Debug.LogWarning("View not found. Location: " + locationId + ", View: " + viewId, this);
                return false;
            }

            currentViewIndex = index;
            RefreshViewVisibility();
            return true;
        }

        public bool RotateView(int delta)
        {
            EnsureViews();

            if (views.Count == 0)
            {
                Debug.LogWarning("Cannot rotate a location with no views: " + locationId, this);
                return false;
            }

            if (currentViewIndex < 0 || currentViewIndex >= views.Count)
            {
                currentViewIndex = 0;
            }
            else
            {
                currentViewIndex = (currentViewIndex + delta) % views.Count;
                if (currentViewIndex < 0)
                {
                    currentViewIndex += views.Count;
                }
            }

            RefreshViewVisibility();
            return true;
        }

        public bool HasView(string viewId)
        {
            return FindViewIndex(viewId) >= 0;
        }

        public LocationView GetView(string viewId)
        {
            int index = FindViewIndex(viewId);
            if (index < 0)
            {
                return null;
            }

            return views[index];
        }

        public string GetViewIdByOffset(int delta)
        {
            EnsureViews();

            if (views.Count == 0)
            {
                return null;
            }

            int baseIndex = currentViewIndex;
            if (baseIndex < 0 || baseIndex >= views.Count)
            {
                baseIndex = 0;
            }

            int nextIndex = (baseIndex + delta) % views.Count;
            if (nextIndex < 0)
            {
                nextIndex += views.Count;
            }

            return views[nextIndex] != null ? views[nextIndex].ViewId : null;
        }

        public void RefreshViewVisibility()
        {
            EnsureViews();

            for (int i = 0; i < views.Count; i++)
            {
                if (views[i] != null)
                {
                    views[i].SetActive(i == currentViewIndex);
                }
            }
        }

        private int FindViewIndex(string viewId)
        {
            if (string.IsNullOrEmpty(viewId))
            {
                return -1;
            }

            EnsureViews();

            for (int i = 0; i < views.Count; i++)
            {
                if (views[i] != null && views[i].Matches(viewId))
                {
                    return i;
                }
            }

            return -1;
        }

        private void Reset()
        {
            CacheViews();
            ValidateIds();
        }

        private void OnValidate()
        {
            ValidateIds();
        }

        private void EnsureViews()
        {
            if (views == null)
            {
                views = new List<LocationView>();
            }

            if (views.Count == 0)
            {
                CacheViews();
            }
        }

        private void ValidateIds()
        {
            if (string.IsNullOrEmpty(locationId))
            {
                Debug.LogWarning("LocationController has an empty locationId: " + name, this);
            }

            HashSet<string> usedViewIds = new HashSet<string>();
            for (int i = 0; i < views.Count; i++)
            {
                if (views[i] == null || string.IsNullOrEmpty(views[i].ViewId))
                {
                    continue;
                }

                if (!usedViewIds.Add(views[i].ViewId))
                {
                    Debug.LogWarning("Duplicate viewId in location " + locationId + ": " + views[i].ViewId, this);
                }
            }
        }
    }
}
