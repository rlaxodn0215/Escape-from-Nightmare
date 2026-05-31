// -----------------------------------------------------------------------------
// Codex comment pass: Hide Point Controller
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\HidePointController.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(ClickableButton))]
    // Scene interaction component for Hide Point Controller, converting player input into manager-level requests.
    public class HidePointController : MonoBehaviour
    {
        [SerializeField] private string hidePointId;
        [SerializeField] private GameObject rootObject;
        [SerializeField] private bool usable = true;

        // Stores the clickable Button value used by this script's runtime or editor workflow.
        private ClickableButton clickableButton;

        public string HidePointId
        {
            get { return hidePointId; }
        }

        public bool Usable
        {
            get { return usable; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            clickableButton = GetComponent<ClickableButton>();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            clickableButton = GetComponent<ClickableButton>();
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }

        // Keeps Inspector-edited values and cached references valid while working in the editor.
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

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetUsable(bool value)
        {
            usable = value;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
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
