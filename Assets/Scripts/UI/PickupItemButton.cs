using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class PickupItemButton : MonoBehaviour
	{
		[SerializeField] private ItemDefinition item;
		[SerializeField] private Button button;

		private void Awake()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}
		}

		private void OnEnable()
		{
			if (button != null)
			{
				button.onClick.AddListener(HandleClicked);
			}
		}

		private void Start()
		{
			RefreshCollectedState();
		}

		private void OnDisable()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(HandleClicked);
			}
		}

		private void HandleClicked()
		{
			if (item == null || InventoryManager.Instance == null)
			{
				Debug.LogWarning("PickupItemButton cannot collect because the item or InventoryManager is missing.", this);
				return;
			}

			if (!InventoryManager.Instance.AddItem(item))
			{
				RefreshCollectedState();
				return;
			}

			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlaySfx(AudioSoundId.ItemCollect);
			}

			gameObject.SetActive(false);
		}

		private void RefreshCollectedState()
		{
			if (item != null && InventoryManager.Instance != null && InventoryManager.Instance.HasItem(item.ItemId))
			{
				gameObject.SetActive(false);
			}
		}
	}
}
