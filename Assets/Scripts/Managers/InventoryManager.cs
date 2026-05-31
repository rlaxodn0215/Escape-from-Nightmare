// -----------------------------------------------------------------------------
// Codex comment pass: Inventory Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\InventoryManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Inventory Manager system, keeping shared state and events behind one access point.
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private bool loadFromSaveOnStart = true;

        // Stores the owned Item Ids value used by this script's runtime or editor workflow.
        private readonly List<string> ownedItemIds = new List<string>();
        // Stores the selected Item Id value used by this script's runtime or editor workflow.
        private string selectedItemId;

        public event Action InventoryChanged;
        public event Action<string> SelectedItemChanged;

        public IReadOnlyList<string> OwnedItemIds
        {
            get { return ownedItemIds; }
        }

        public string SelectedItemId
        {
            get { return selectedItemId; }
        }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (loadFromSaveOnStart)
            {
                LoadFromSave();
            }
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        public void LoadFromSave()
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("SaveManager instance is missing.");
                NotifyInventoryChanged();
                return;
            }

            ownedItemIds.Clear();
            IReadOnlyList<string> savedItemIds = SaveManager.Instance.GetOwnedItemIds();
            for (int i = 0; i < savedItemIds.Count; i++)
            {
                string itemId = savedItemIds[i];
                if (!string.IsNullOrEmpty(itemId) && !ownedItemIds.Contains(itemId))
                {
                    ownedItemIds.Add(itemId);
                }
            }

            NotifyInventoryChanged();
        }

        // Performs the Replace Inventory operation while keeping its implementation details inside this script.
        public void ReplaceInventory(IEnumerable<string> itemIds)
        {
            ownedItemIds.Clear();

            if (itemIds != null)
            {
                foreach (string itemId in itemIds)
                {
                    if (!string.IsNullOrEmpty(itemId) && !ownedItemIds.Contains(itemId))
                    {
                        ownedItemIds.Add(itemId);
                    }
                }
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ReplaceOwnedItems(ownedItemIds);
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            NotifyInventoryChanged();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool HasItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            if (ownedItemIds.Contains(itemId))
            {
                return true;
            }

            return SaveManager.Instance != null && SaveManager.Instance.IsItemOwned(itemId);
        }

        // Performs the Add Item operation while keeping its implementation details inside this script.
        public void AddItem(string itemId)
        {
            TryAddItem(itemId);
        }

        // Performs the Try Add Item operation while keeping its implementation details inside this script.
        public bool TryAddItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("Cannot add an empty item id.");
                return false;
            }

            if (ownedItemIds.Contains(itemId))
            {
                return false;
            }

            ownedItemIds.Add(itemId);

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkItemOwned(itemId);
                SaveManager.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            NotifyInventoryChanged();
            return true;
        }

        // Performs the Remove Item operation while keeping its implementation details inside this script.
        public void RemoveItem(string itemId)
        {
            TryRemoveItem(itemId);
        }

        // Performs the Try Remove Item operation while keeping its implementation details inside this script.
        public bool TryRemoveItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            if (!ownedItemIds.Remove(itemId))
            {
                return false;
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.RemoveOwnedItem(itemId);
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            if (selectedItemId == itemId)
            {
                ClearSelectedItem();
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }

            NotifyInventoryChanged();
            return true;
        }

        // Performs the Try Transform Item operation while keeping its implementation details inside this script.
        public bool TryTransformItem(string fromItemId, string toItemId)
        {
            if (string.IsNullOrEmpty(fromItemId) || string.IsNullOrEmpty(toItemId))
            {
                return false;
            }

            if (!HasItem(fromItemId))
            {
                return false;
            }

            TryRemoveItem(fromItemId);
            TryAddItem(toItemId);

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }

            return true;
        }

        // Performs the Select Item operation while keeping its implementation details inside this script.
        public void SelectItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                ClearSelectedItem();
                return;
            }

            if (!HasItem(itemId))
            {
                Debug.LogWarning("Cannot select unowned item: " + itemId);
                return;
            }

            selectedItemId = selectedItemId == itemId ? string.Empty : itemId;
            NotifySelectedItemChanged();
        }

        // Performs the Try Use Selected Item operation while keeping its implementation details inside this script.
        public bool TryUseSelectedItem(string requiredItemId)
        {
            if (string.IsNullOrEmpty(requiredItemId))
            {
                return true;
            }

            if (string.IsNullOrEmpty(selectedItemId) || selectedItemId != requiredItemId || !HasItem(requiredItemId))
            {
                return false;
            }

            ItemRecord item = GetItemRecord(requiredItemId);
            if (item != null && item.consumable)
            {
                TryRemoveItem(requiredItemId);
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkItemUsed(requiredItemId);
                SaveManager.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            ClearSelectedItem();
            NotifyInventoryChanged();
            return true;
        }

        // Performs the Clear Selected Item operation while keeping its implementation details inside this script.
        public void ClearSelectedItem()
        {
            selectedItemId = string.Empty;
            NotifySelectedItemChanged();
        }

        // Performs the Notify Inventory Changed operation while keeping its implementation details inside this script.
        private void NotifyInventoryChanged()
        {
            if (InventoryChanged != null)
            {
                InventoryChanged.Invoke();
            }
        }

        // Performs the Notify Selected Item Changed operation while keeping its implementation details inside this script.
        private void NotifySelectedItemChanged()
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged.Invoke(selectedItemId);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private ItemRecord GetItemRecord(string itemId)
        {
            if (GameDataManager.Instance == null)
            {
                Debug.LogWarning("GameDataManager instance is missing.");
                return null;
            }

            return GameDataManager.Instance.GetItemById(itemId);
        }
    }
}
