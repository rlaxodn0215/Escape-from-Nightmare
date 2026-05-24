using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public readonly struct InventorySlotModel
    {
        public InventorySlotModel(string itemId, Sprite icon, bool selected)
        {
            ItemId = itemId ?? string.Empty;
            Icon = icon;
            Selected = selected;
        }

        public string ItemId { get; }
        public Sprite Icon { get; }
        public bool Selected { get; }
        public bool HasItem => !string.IsNullOrWhiteSpace(ItemId);
    }

    public sealed class InventoryWindow : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button closeButton;
        [SerializeField] private InventorySlotView[] slots = Array.Empty<InventorySlotView>();
        [SerializeField] private Sprite emptySlotSprite;
        [SerializeField] private Sprite selectedSlotSprite;

        private IReadOnlyList<string> currentItemIds = Array.Empty<string>();
        private string currentSelectedItemId = string.Empty;
        private Func<string, Sprite> iconResolver;
        private Action<string> onSelectItem;

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        public void SetReferences(
            GameObject root,
            Button close,
            InventorySlotView[] slotViews,
            Sprite emptySlot,
            Sprite selectedSlot)
        {
            panelRoot = root;
            closeButton = close;
            slots = slotViews ?? Array.Empty<InventorySlotView>();
            emptySlotSprite = emptySlot;
            selectedSlotSprite = selectedSlot;
        }

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Close);
            }
        }

        public void Open(
            IReadOnlyCollection<string> itemIds,
            string selectedItemId,
            Func<string, Sprite> resolveIcon,
            Action<string> selectItem)
        {
            BindCloseButton();
            currentItemIds = itemIds?.ToArray() ?? Array.Empty<string>();
            currentSelectedItemId = selectedItemId ?? string.Empty;
            iconResolver = resolveIcon;
            onSelectItem = selectItem;

            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }

            Refresh();
        }

        public void Refresh(
            IReadOnlyCollection<string> itemIds,
            string selectedItemId,
            Func<string, Sprite> resolveIcon,
            Action<string> selectItem)
        {
            BindCloseButton();
            currentItemIds = itemIds?.ToArray() ?? Array.Empty<string>();
            currentSelectedItemId = selectedItemId ?? string.Empty;
            iconResolver = resolveIcon;
            onSelectItem = selectItem;
            Refresh();
        }

        public void Close()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        public void Refresh()
        {
            var models = BuildSlotModels(currentItemIds, currentSelectedItemId, slots.Length, iconResolver);
            for (var index = 0; index < slots.Length; index++)
            {
                var slot = slots[index];
                if (slot == null)
                {
                    continue;
                }

                var model = models[index];
                slot.Bind(model, emptySlotSprite, selectedSlotSprite, () => SelectItem(model.ItemId));
            }
        }

        public static InventorySlotModel[] BuildSlotModels(
            IReadOnlyCollection<string> itemIds,
            string selectedItemId,
            int slotCount,
            Func<string, Sprite> resolveIcon)
        {
            var safeSlotCount = Mathf.Max(0, slotCount);
            var models = new InventorySlotModel[safeSlotCount];
            var ids = itemIds?.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray() ?? Array.Empty<string>();

            for (var index = 0; index < safeSlotCount; index++)
            {
                if (index >= ids.Length)
                {
                    models[index] = new InventorySlotModel(string.Empty, null, false);
                    continue;
                }

                var itemId = ids[index];
                models[index] = new InventorySlotModel(
                    itemId,
                    resolveIcon?.Invoke(itemId),
                    itemId == selectedItemId);
            }

            return models;
        }

        private void SelectItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return;
            }

            currentSelectedItemId = itemId;
            onSelectItem?.Invoke(itemId);
            Refresh();
        }

        private void BindCloseButton()
        {
            if (closeButton == null)
            {
                return;
            }

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }
    }
}
