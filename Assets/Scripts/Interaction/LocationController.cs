// -----------------------------------------------------------------------------
// Codex comment pass: Location Controller
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\LocationController.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Scene interaction component for Location Controller, converting player input into manager-level requests.
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private string locationId;
        [SerializeField] private string defaultViewId;
        [SerializeField] private List<LocationView> views = new List<LocationView>();

        // Stores the current View Index value used by this script's runtime or editor workflow.
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

        // Performs the Cache Views operation while keeping its implementation details inside this script.
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

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetLocationActive(bool active)
        {
            gameObject.SetActive(active);
        }

        // Performs the Activate Default View operation while keeping its implementation details inside this script.
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

        // Performs the Activate View operation while keeping its implementation details inside this script.
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

        // Performs the Rotate View operation while keeping its implementation details inside this script.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool HasView(string viewId)
        {
            return FindViewIndex(viewId) >= 0;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public LocationView GetView(string viewId)
        {
            int index = FindViewIndex(viewId);
            if (index < 0)
            {
                return null;
            }

            return views[index];
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Re-reads current game data and manager state, then redraws the visible UI.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheViews();
            ValidateIds();
        }

        // Keeps Inspector-edited values and cached references valid while working in the editor.
        private void OnValidate()
        {
            ValidateIds();
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
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

        // Checks scene, prefab, resource, or data requirements and records any issues found.
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
