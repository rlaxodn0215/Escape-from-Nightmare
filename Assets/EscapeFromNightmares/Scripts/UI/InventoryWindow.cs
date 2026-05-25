using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    /// <summary>
    /// 인벤토리 슬롯 하나에 표시할 아이템 ID, 아이콘, 선택 여부를 담는 읽기 전용 모델입니다.
    /// </summary>
    public readonly struct InventorySlotModel
    {
        public InventorySlotModel(string itemId, Sprite icon, bool selected)
        {
            ItemId = itemId ?? string.Empty;
            Icon = icon;
            Selected = selected;
        }

        /// <summary>슬롯에 들어 있는 아이템 ID입니다.</summary>
        public string ItemId { get; }
        /// <summary>슬롯에 표시할 아이템 아이콘입니다.</summary>
        public Sprite Icon { get; }
        /// <summary>현재 선택된 아이템 슬롯인지 여부입니다.</summary>
        public bool Selected { get; }
        /// <summary>아이템 ID가 있는 실제 아이템 슬롯인지 여부입니다.</summary>
        public bool HasItem => !string.IsNullOrWhiteSpace(ItemId);
    }

    /// <summary>
    /// 인벤토리 패널을 열고 닫으며 보유 아이템을 슬롯 뷰에 바인딩합니다.
    /// </summary>
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

        /// <summary>인벤토리 패널이 현재 열려 있는지 여부입니다.</summary>
        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        /// <summary>
        /// 에디터 빌더나 런타임 생성 코드가 패널과 슬롯 참조를 직접 주입할 때 사용합니다.
        /// </summary>
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

        /// <summary>
        /// 인벤토리 패널을 열고 현재 아이템 목록, 선택 상태, 아이콘 해석 함수를 연결합니다.
        /// </summary>
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

        /// <summary>
        /// 열린 인벤토리의 슬롯 데이터를 새 인벤토리 상태로 갱신합니다.
        /// </summary>
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

        /// <summary>인벤토리 패널을 닫습니다.</summary>
        public void Close()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        /// <summary>현재 저장된 모델 정보를 슬롯 뷰들에 다시 바인딩합니다.</summary>
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

        /// <summary>
        /// 아이템 ID 목록을 고정된 슬롯 수에 맞춘 표시 모델 배열로 변환합니다.
        /// </summary>
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
