// -----------------------------------------------------------------------------
// Codex comment pass: Inventory Bar UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\InventoryBarUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Inventory Bar UI UI elements, keeping references cached and visuals synchronized.
    public class InventoryBarUI : MonoBehaviour
    {
        [SerializeField] private Transform slotRoot;
        [SerializeField] private List<InventorySlotUI> slots = new List<InventorySlotUI>();
        [SerializeField] private bool autoCollectSlots = true;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool hideOnAwake = true;

        // Stores the subscribed value used by this script's runtime or editor workflow.
        private bool subscribed;
        private bool visible;

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheCanvasGroup();

            if (autoCollectSlots)
            {
                CacheSlots();
            }

            SetVisible(!hideOnAwake);
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            CacheCanvasGroup();
            Subscribe();
            Refresh();
            SetVisible(visible && !hideOnAwake);
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            Unsubscribe();
        }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            Refresh();
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void ToggleVisibility()
        {
            SetVisible(!visible);
        }

        // Performs the Cache Slots operation while keeping its implementation details inside this script.
        public void CacheSlots()
        {
            Transform root = slotRoot != null ? slotRoot : transform;
            InventorySlotUI[] foundSlots = root.GetComponentsInChildren<InventorySlotUI>(true);

            slots.Clear();
            for (int i = 0; i < foundSlots.Length; i++)
            {
                if (foundSlots[i] != null && !slots.Contains(foundSlots[i]))
                {
                    slots.Add(foundSlots[i]);
                }
            }
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        public void Refresh()
        {
            if (autoCollectSlots && slots.Count == 0)
            {
                CacheSlots();
            }

            if (InventoryManager.Instance == null)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i] != null)
                    {
                        slots[i].Clear();
                    }
                }

                return;
            }

            IReadOnlyList<string> itemIds = InventoryManager.Instance.OwnedItemIds;
            string selectedItemId = InventoryManager.Instance.SelectedItemId;

            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlotUI slot = slots[i];
                if (slot == null)
                {
                    continue;
                }

                if (i < itemIds.Count)
                {
                    slot.SetItem(itemIds[i]);
                    slot.SetSelected(itemIds[i] == selectedItemId);
                }
                else
                {
                    slot.Clear();
                }
            }

            if (itemIds.Count > slots.Count)
            {
                Debug.LogWarning("Inventory has more items than visible slots. Items: " + itemIds.Count + ", Slots: " + slots.Count);
            }
        }

        // Performs the Subscribe operation while keeping its implementation details inside this script.
        private void Subscribe()
        {
            if (subscribed || InventoryManager.Instance == null)
            {
                return;
            }

            InventoryManager.Instance.InventoryChanged += HandleInventoryChanged;
            InventoryManager.Instance.SelectedItemChanged += HandleSelectedItemChanged;
            subscribed = true;
        }

        // Performs the Unsubscribe operation while keeping its implementation details inside this script.
        private void Unsubscribe()
        {
            if (!subscribed || InventoryManager.Instance == null)
            {
                subscribed = false;
                return;
            }

            InventoryManager.Instance.InventoryChanged -= HandleInventoryChanged;
            InventoryManager.Instance.SelectedItemChanged -= HandleSelectedItemChanged;
            subscribed = false;
        }

        // Performs the Handle Inventory Changed operation while keeping its implementation details inside this script.
        private void HandleInventoryChanged()
        {
            Refresh();
        }

        // Performs the Handle Selected Item Changed operation while keeping its implementation details inside this script.
        private void HandleSelectedItemChanged(string selectedItemId)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].SetSelected(!string.IsNullOrEmpty(slots[i].ItemId) && slots[i].ItemId == selectedItemId);
                }
            }
        }

        private void CacheCanvasGroup()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        private void SetVisible(bool newVisible)
        {
            visible = newVisible;

            CacheCanvasGroup();
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = newVisible ? 1f : 0f;
            canvasGroup.interactable = newVisible;
            canvasGroup.blocksRaycasts = newVisible;
        }
    }
}
