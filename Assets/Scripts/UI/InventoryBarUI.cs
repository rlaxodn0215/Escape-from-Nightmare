using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class InventoryBarUI : MonoBehaviour
    {
        [SerializeField] private Transform slotRoot;
        [SerializeField] private List<InventorySlotUI> slots = new List<InventorySlotUI>();
        [SerializeField] private bool autoCollectSlots = true;

        private bool subscribed;

        private void Awake()
        {
            if (autoCollectSlots)
            {
                CacheSlots();
            }
        }

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Start()
        {
            Refresh();
        }

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

        private void HandleInventoryChanged()
        {
            Refresh();
        }

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
    }
}
