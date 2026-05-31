// -----------------------------------------------------------------------------
// Codex comment pass: Location View
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\LocationView.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Scene interaction component for Location View, converting player input into manager-level requests.
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

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetActive(bool active)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            rootObject.SetActive(active);
        }

        // Performs the Matches operation while keeping its implementation details inside this script.
        public bool Matches(string targetViewId)
        {
            if (string.IsNullOrEmpty(targetViewId))
            {
                return false;
            }

            return viewId == targetViewId;
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            EnsureRootObject();
            WarnIfMissingViewId();
        }

        // Keeps Inspector-edited values and cached references valid while working in the editor.
        private void OnValidate()
        {
            EnsureRootObject();
            WarnIfMissingViewId();
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private void EnsureRootObject()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Performs the Warn If Missing View Id operation while keeping its implementation details inside this script.
        private void WarnIfMissingViewId()
        {
            if (string.IsNullOrEmpty(viewId))
            {
                Debug.LogWarning("LocationView has an empty viewId: " + name, this);
            }
        }
    }
}
