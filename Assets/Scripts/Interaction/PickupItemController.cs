// -----------------------------------------------------------------------------
// Codex comment pass: Pickup Item Controller
// Role: Connects scene objects and UI buttons to player interactions such as movement, pickup, hiding, and puzzle access.
// Scope: This script belongs to Interaction\PickupItemController.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(ClickableButton))]
    // Scene interaction component for Pickup Item Controller, converting player input into manager-level requests.
    public class PickupItemController : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private bool hideWhenAlreadyOwned = true;

        // Stores the clickable Button value used by this script's runtime or editor workflow.
        private ClickableButton clickableButton;

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheReferences();
        }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            RefreshVisibility();
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheReferences();
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        public void RefreshVisibility()
        {
            if (!hideWhenAlreadyOwned)
            {
                return;
            }

            string itemId = GetLinkedItemId();
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            bool alreadyOwned = false;
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId))
            {
                alreadyOwned = true;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsItemOwned(itemId))
            {
                alreadyOwned = true;
            }

            if (alreadyOwned)
            {
                EnsureRootObject();
                rootObject.SetActive(false);
            }
        }

        // Performs the Mark Picked Up operation while keeping its implementation details inside this script.
        public void MarkPickedUp()
        {
            EnsureRootObject();
            rootObject.SetActive(false);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private string GetLinkedItemId()
        {
            CacheReferences();
            return clickableButton != null ? clickableButton.LinkedItemId : string.Empty;
        }

        // Performs the Cache References operation while keeping its implementation details inside this script.
        private void CacheReferences()
        {
            EnsureRootObject();

            if (clickableButton == null)
            {
                clickableButton = GetComponent<ClickableButton>();
            }
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private void EnsureRootObject()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }
    }
}
