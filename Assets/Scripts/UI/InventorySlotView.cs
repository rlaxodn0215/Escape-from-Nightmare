using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    /// <summary>
    /// 인벤토리 슬롯 하나의 프레임, 아이콘, 클릭 가능 상태를 화면에 반영합니다.
    /// </summary>
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        /// <summary>현재 슬롯에 표시된 아이템 ID입니다. 빈 슬롯이면 빈 문자열입니다.</summary>
        public string ItemId { get; private set; }

        /// <summary>
        /// 에디터 빌더나 런타임 생성 코드가 슬롯 구성 요소를 직접 주입할 때 사용합니다.
        /// </summary>
        public void SetReferences(Image frame, Image icon, Button slotButton)
        {
            frameImage = frame;
            iconImage = icon;
            button = slotButton;
        }

        /// <summary>
        /// 슬롯 모델에 맞춰 선택 프레임, 아이콘, 버튼 이벤트를 한 번에 갱신합니다.
        /// </summary>
        public void Bind(
            InventorySlotModel model,
            Sprite emptyFrame,
            Sprite selectedFrame,
            UnityAction onClick)
        {
            ItemId = model.ItemId;
            if (frameImage != null)
            {
                frameImage.sprite = model.Selected && selectedFrame != null ? selectedFrame : emptyFrame;
                frameImage.color = Color.white;
            }

            if (iconImage != null)
            {
                iconImage.sprite = model.Icon;
                iconImage.enabled = model.Icon != null;
                iconImage.color = Color.white;
            }

            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.interactable = model.HasItem;
            if (model.HasItem && onClick != null)
            {
                button.onClick.AddListener(onClick);
            }
        }
    }
}
