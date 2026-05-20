using System;
using System.Collections.Generic;
using EscapeFromNightmares.Core;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class InventorySystem : MonoBehaviour
    {
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private List<ItemDefinition> startingItems = new List<ItemDefinition>();

        private readonly Dictionary<string, ItemDefinition> itemsById = new Dictionary<string, ItemDefinition>();
        private readonly HashSet<string> consumedItemIds = new HashSet<string>();
        private ItemDefinition selectedItem;

        public event Action InventoryChanged;
        public event Action<ItemDefinition> SelectedItemChanged;

        public ItemDefinition SelectedItem => selectedItem;
        public IReadOnlyCollection<ItemDefinition> Items => itemsById.Values;

        private void Awake()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            ResetInventory();
        }

        private void OnEnable()
        {
            gameStateManager ??= FindFirstObjectByType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted += ResetInventory;
            }
        }

        private void OnDisable()
        {
            if (gameStateManager != null)
            {
                gameStateManager.Stage1RunStarted -= ResetInventory;
            }
        }

        public void ResetInventory()
        {
            itemsById.Clear();
            consumedItemIds.Clear();
            selectedItem = null;

            foreach (ItemDefinition item in startingItems)
            {
                AddItem(item);
            }

            InventoryChanged?.Invoke();
            SelectedItemChanged?.Invoke(selectedItem);
        }

        public bool AddItem(ItemDefinition item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
            {
                Debug.LogWarning("Ignored inventory add because item or item id is missing.");
                return false;
            }

            if (consumedItemIds.Contains(item.ItemId) || itemsById.ContainsKey(item.ItemId))
            {
                return false;
            }

            itemsById.Add(item.ItemId, item);
            InventoryChanged?.Invoke();
            return true;
        }

        public bool HasItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && itemsById.ContainsKey(itemId);
        }

        public bool SelectItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                ClearSelection();
                return false;
            }

            if (!itemsById.TryGetValue(itemId, out ItemDefinition item))
            {
                Debug.Log($"Cannot select missing item '{itemId}'.");
                return false;
            }

            selectedItem = item;
            SelectedItemChanged?.Invoke(selectedItem);
            return true;
        }

        public void SelectItem(ItemDefinition item)
        {
            if (item == null)
            {
                ClearSelection();
                return;
            }

            SelectItem(item.ItemId);
        }

        public void ClearSelection()
        {
            if (selectedItem == null)
            {
                return;
            }

            selectedItem = null;
            SelectedItemChanged?.Invoke(selectedItem);
        }

        public bool IsSelected(string itemId)
        {
            return selectedItem != null && selectedItem.ItemId == itemId;
        }

        public bool TryConsumeSelected(string requiredItemId)
        {
            if (!IsSelected(requiredItemId) || !itemsById.Remove(requiredItemId))
            {
                return false;
            }

            consumedItemIds.Add(requiredItemId);
            selectedItem = null;
            InventoryChanged?.Invoke();
            SelectedItemChanged?.Invoke(selectedItem);
            return true;
        }
    }
}
