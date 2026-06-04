using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
	public class InventoryManager : Singleton<InventoryManager>
	{
		[SerializeField, Min(1)] private int maxItems = 6;
		[SerializeField] private ItemDefinition[] itemCatalog = Array.Empty<ItemDefinition>();

		private readonly List<ItemDefinition> items = new List<ItemDefinition>();
		private readonly Dictionary<string, ItemDefinition> catalogById = new Dictionary<string, ItemDefinition>(StringComparer.Ordinal);
		private ItemDefinition selectedItem;
		private bool isLoaded;

		public event Action InventoryChanged;
		public event Action SelectionChanged;

		public IReadOnlyList<ItemDefinition> Items => items;
		public ItemDefinition SelectedItem => selectedItem;

		protected override bool DontDestroy => false;

		protected override void Awake()
		{
			base.Awake();

			if (Instance != this)
			{
				return;
			}

			BuildCatalogLookup();
			LoadInventory();
		}

		public bool AddItem(ItemDefinition item)
		{
			if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
			{
				Debug.LogWarning("InventoryManager cannot add an empty item.", this);
				return false;
			}

			if (HasItem(item.ItemId))
			{
				return false;
			}

			if (items.Count >= maxItems)
			{
				Debug.LogWarning($"InventoryManager cannot add '{item.ItemId}' because the inventory is full.", this);
				return false;
			}

			items.Add(item);

			if (selectedItem == null)
			{
				SetSelectedItem(item, false);
			}

			SaveInventory();
			InventoryChanged?.Invoke();
			return true;
		}

		public bool RemoveItem(string itemId)
		{
			if (string.IsNullOrWhiteSpace(itemId))
			{
				return false;
			}

			int index = items.FindIndex(item => item != null && string.Equals(item.ItemId, itemId, StringComparison.Ordinal));
			if (index < 0)
			{
				return false;
			}

			bool removedSelectedItem = selectedItem != null && string.Equals(selectedItem.ItemId, itemId, StringComparison.Ordinal);
			items.RemoveAt(index);

			if (removedSelectedItem)
			{
				SetSelectedItem(items.Count > 0 ? items[0] : null, false);
			}

			SaveInventory();
			InventoryChanged?.Invoke();
			return true;
		}

		public bool SelectItem(string itemId)
		{
			if (string.IsNullOrWhiteSpace(itemId))
			{
				return false;
			}

			ItemDefinition item = FindOwnedItem(itemId);
			if (item == null)
			{
				return false;
			}

			if (selectedItem == item)
			{
				return true;
			}

			SetSelectedItem(item, true);
			return true;
		}

		public bool HasItem(string itemId)
		{
			return FindOwnedItem(itemId) != null;
		}

		private void BuildCatalogLookup()
		{
			catalogById.Clear();

			foreach (ItemDefinition item in itemCatalog)
			{
				if (item == null || string.IsNullOrWhiteSpace(item.ItemId))
				{
					continue;
				}

				catalogById[item.ItemId] = item;
			}
		}

		private void LoadInventory()
		{
			if (isLoaded)
			{
				return;
			}

			isLoaded = true;
			items.Clear();

			if (SaveManager.Instance == null)
			{
				return;
			}

			InventorySaveData saveData = SaveManager.Instance.LoadInventoryData();
			if (saveData.itemIds != null)
			{
				foreach (string itemId in saveData.itemIds)
				{
					if (string.IsNullOrWhiteSpace(itemId) || items.Count >= maxItems || HasItem(itemId))
					{
						continue;
					}

					if (catalogById.TryGetValue(itemId, out ItemDefinition item) && item != null)
					{
						items.Add(item);
					}
				}
			}

			selectedItem = FindOwnedItem(saveData.selectedItemId);
			if (selectedItem == null && items.Count > 0)
			{
				selectedItem = items[0];
			}
		}

		private void SaveInventory()
		{
			if (SaveManager.Instance == null)
			{
				return;
			}

			string[] itemIds = new string[items.Count];
			for (int i = 0; i < items.Count; i++)
			{
				itemIds[i] = items[i] != null ? items[i].ItemId : string.Empty;
			}

			SaveManager.Instance.SaveInventoryData(new InventorySaveData
			{
				itemIds = itemIds,
				selectedItemId = selectedItem != null ? selectedItem.ItemId : string.Empty
			});
		}

		private ItemDefinition FindOwnedItem(string itemId)
		{
			if (string.IsNullOrWhiteSpace(itemId))
			{
				return null;
			}

			foreach (ItemDefinition item in items)
			{
				if (item != null && string.Equals(item.ItemId, itemId, StringComparison.Ordinal))
				{
					return item;
				}
			}

			return null;
		}

		private void SetSelectedItem(ItemDefinition item, bool save)
		{
			selectedItem = item;

			if (save)
			{
				SaveInventory();
			}

			SelectionChanged?.Invoke();
		}
	}
}
