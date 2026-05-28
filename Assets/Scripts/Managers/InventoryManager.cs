using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private bool loadFromSaveOnStart = true;

        private readonly List<string> ownedItemIds = new List<string>();
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

        private void Start()
        {
            if (loadFromSaveOnStart)
            {
                LoadFromSave();
            }
        }

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

        public void AddItem(string itemId)
        {
            TryAddItem(itemId);
        }

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

        public void RemoveItem(string itemId)
        {
            TryRemoveItem(itemId);
        }

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

        public void ClearSelectedItem()
        {
            selectedItemId = string.Empty;
            NotifySelectedItemChanged();
        }

        private void NotifyInventoryChanged()
        {
            if (InventoryChanged != null)
            {
                InventoryChanged.Invoke();
            }
        }

        private void NotifySelectedItemChanged()
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged.Invoke(selectedItemId);
            }
        }

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
