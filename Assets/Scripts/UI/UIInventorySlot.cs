using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class UIInventorySlot : MonoBehaviour
	{
		[SerializeField] private Button button;
		[SerializeField] private Image frameImage;
		[SerializeField] private Image iconImage;
		[SerializeField] private Sprite emptySprite;
		[SerializeField] private Sprite selectedSprite;

		private UIInventoryBar inventoryBar;
		private int slotIndex;
		private ItemDefinition item;

		public ItemDefinition Item => item;

		private void Awake()
		{
			FindReferencesIfNeeded();
		}

		private void OnEnable()
		{
			FindReferencesIfNeeded();

			if (button != null)
			{
				button.onClick.AddListener(HandleClicked);
			}
		}

		private void OnDisable()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(HandleClicked);
			}
		}

		public void Initialize(UIInventoryBar owner, int index)
		{
			inventoryBar = owner;
			slotIndex = index;
		}

		public void SetItem(ItemDefinition nextItem, bool selected)
		{
			item = nextItem;
			FindReferencesIfNeeded();

			if (frameImage != null)
			{
				frameImage.sprite = selected && selectedSprite != null ? selectedSprite : emptySprite;
			}

			if (iconImage != null)
			{
				iconImage.sprite = item != null ? item.Icon : null;
				iconImage.enabled = item != null && item.Icon != null;
				iconImage.preserveAspect = true;
			}

			if (button != null)
			{
				button.interactable = item != null;
			}
		}

		private void HandleClicked()
		{
			inventoryBar?.HandleSlotClicked(slotIndex);
		}

		private void FindReferencesIfNeeded()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}

			if (frameImage == null)
			{
				frameImage = GetComponent<Image>();
			}

			if (iconImage == null)
			{
				Transform iconTransform = transform.Find("Icon");
				iconImage = iconTransform != null ? iconTransform.GetComponent<Image>() : null;
			}
		}
	}
}
