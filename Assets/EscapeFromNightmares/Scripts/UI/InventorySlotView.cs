using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;

        public string ItemId { get; private set; }

        public void SetReferences(Image frame, Image icon, Button slotButton)
        {
            frameImage = frame;
            iconImage = icon;
            button = slotButton;
        }

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
