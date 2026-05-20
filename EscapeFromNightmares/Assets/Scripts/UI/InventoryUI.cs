using System.Collections.Generic;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button[] slotButtons = new Button[0];
        [SerializeField] private Image[] slotIcons = new Image[0];
        [SerializeField] private Image[] selectionFrames = new Image[0];
        [SerializeField] private TMP_Text selectedItemLabel;
        [SerializeField] private string emptySelectionLabel = "";

        private readonly List<ItemDefinition> visibleItems = new List<ItemDefinition>();

        private void Awake()
        {
            inventorySystem ??= FindFirstObjectByType<InventorySystem>();
            canvasGroup ??= GetComponent<CanvasGroup>();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            for (int index = 0; index < slotButtons.Length; index++)
            {
                int capturedIndex = index;
                if (slotButtons[index] != null)
                {
                    slotButtons[index].onClick.AddListener(() => SelectSlot(capturedIndex));
                }
            }
        }

        private void OnEnable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.InventoryChanged += Refresh;
                inventorySystem.SelectedItemChanged += RefreshSelection;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (inventorySystem != null)
            {
                inventorySystem.InventoryChanged -= Refresh;
                inventorySystem.SelectedItemChanged -= RefreshSelection;
            }
        }

        public void Bind(InventorySystem nextInventorySystem)
        {
            if (inventorySystem == nextInventorySystem)
            {
                return;
            }

            if (isActiveAndEnabled && inventorySystem != null)
            {
                inventorySystem.InventoryChanged -= Refresh;
                inventorySystem.SelectedItemChanged -= RefreshSelection;
            }

            inventorySystem = nextInventorySystem;

            if (isActiveAndEnabled && inventorySystem != null)
            {
                inventorySystem.InventoryChanged += Refresh;
                inventorySystem.SelectedItemChanged += RefreshSelection;
            }

            Refresh();
        }

        public void Show()
        {
            SetVisible(true);
            Refresh();
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void Toggle()
        {
            bool isVisible = canvasGroup == null ? gameObject.activeSelf : canvasGroup.alpha > 0.5f;
            SetVisible(!isVisible);
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(true);

            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private void Refresh()
        {
            visibleItems.Clear();

            if (inventorySystem != null)
            {
                visibleItems.AddRange(inventorySystem.Items);
            }

            for (int index = 0; index < slotButtons.Length; index++)
            {
                ItemDefinition item = index < visibleItems.Count ? visibleItems[index] : null;
                bool hasItem = item != null;

                if (slotButtons[index] != null)
                {
                    slotButtons[index].interactable = hasItem;
                }

                if (index < slotIcons.Length && slotIcons[index] != null)
                {
                    slotIcons[index].sprite = hasItem ? item.Icon : null;
                    slotIcons[index].color = hasItem ? Color.white : new Color(1f, 1f, 1f, 0f);
                    slotIcons[index].enabled = hasItem;
                }
            }

            RefreshSelection(inventorySystem != null ? inventorySystem.SelectedItem : null);
        }

        private void RefreshSelection(ItemDefinition selectedItem)
        {
            for (int index = 0; index < selectionFrames.Length; index++)
            {
                bool selected = selectedItem != null
                    && index < visibleItems.Count
                    && visibleItems[index] != null
                    && visibleItems[index].ItemId == selectedItem.ItemId;

                if (selectionFrames[index] != null)
                {
                    selectionFrames[index].enabled = selected;
                }
            }

            if (selectedItemLabel != null)
            {
                selectedItemLabel.text = selectedItem != null ? selectedItem.DisplayName : emptySelectionLabel;
            }
        }

        private void SelectSlot(int index)
        {
            if (inventorySystem == null || index < 0 || index >= visibleItems.Count)
            {
                return;
            }

            inventorySystem.SelectItem(visibleItems[index]);
        }
    }
}
