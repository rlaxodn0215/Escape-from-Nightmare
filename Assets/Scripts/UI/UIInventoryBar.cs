using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class UIInventoryBar : MonoBehaviour
	{
		[SerializeField] private InventoryManager inventoryManager;
		[SerializeField] private Button toggleButton;
		[SerializeField] private GameObject windowRoot;
		[SerializeField] private UIInventorySlot[] slots;
		[SerializeField] private bool openOnStart;

		private bool isOpen;

		private void Awake()
		{
			FindReferencesIfNeeded();
			InitializeSlots();
			SetWindowOpen(openOnStart, false);
		}

		private void OnEnable()
		{
			FindReferencesIfNeeded();
			InitializeSlots();
			BindToggleButton();
			Subscribe();
			Refresh();
		}

		private void Start()
		{
			FindReferencesIfNeeded();
			InitializeSlots();
			BindToggleButton();
			SetWindowOpen(openOnStart, false);
			Subscribe();
			Refresh();
		}

		private void OnDisable()
		{
			UnbindToggleButton();
			Unsubscribe();
		}

		public void HandleSlotClicked(int slotIndex)
		{
			if (PuzzleManager.Instance != null && PuzzleManager.Instance.IsPuzzleOpen)
			{
				return;
			}

			if (inventoryManager == null || slots == null || slotIndex < 0 || slotIndex >= slots.Length)
			{
				return;
			}

			ItemDefinition item = slots[slotIndex] != null ? slots[slotIndex].Item : null;
			if (item == null)
			{
				return;
			}

			if (inventoryManager.SelectItem(item.ItemId) && AudioManager.Instance != null)
			{
				AudioManager.Instance.PlayUi(AudioSoundId.ButtonClick);
			}
		}

		public void ToggleWindow()
		{
			if (PuzzleManager.Instance != null && PuzzleManager.Instance.IsPuzzleOpen)
			{
				return;
			}

			SetWindowOpen(!isOpen, true);
		}

		public void SetWindowOpen(bool open)
		{
			SetWindowOpen(open, true);
		}

		private void SetWindowOpen(bool open, bool playSound)
		{
			isOpen = open;

			if (windowRoot != null)
			{
				windowRoot.SetActive(isOpen);
			}

			if (playSound && AudioManager.Instance != null)
			{
				AudioManager.Instance.PlayUi(AudioSoundId.ButtonClick);
			}

			if (isOpen)
			{
				Refresh();
			}
		}

		private void BindToggleButton()
		{
			if (toggleButton == null)
			{
				return;
			}

			toggleButton.onClick.RemoveListener(ToggleWindow);
			toggleButton.onClick.AddListener(ToggleWindow);
		}

		private void UnbindToggleButton()
		{
			if (toggleButton != null)
			{
				toggleButton.onClick.RemoveListener(ToggleWindow);
			}
		}

		private void Subscribe()
		{
			if (inventoryManager == null)
			{
				return;
			}

			inventoryManager.InventoryChanged -= Refresh;
			inventoryManager.SelectionChanged -= Refresh;
			inventoryManager.InventoryChanged += Refresh;
			inventoryManager.SelectionChanged += Refresh;
		}

		private void Unsubscribe()
		{
			if (inventoryManager == null)
			{
				return;
			}

			inventoryManager.InventoryChanged -= Refresh;
			inventoryManager.SelectionChanged -= Refresh;
		}

		private void Refresh()
		{
			if (inventoryManager == null || slots == null)
			{
				return;
			}

			for (int i = 0; i < slots.Length; i++)
			{
				ItemDefinition item = i < inventoryManager.Items.Count ? inventoryManager.Items[i] : null;
				bool selected = item != null && inventoryManager.SelectedItem == item;
				if (slots[i] != null)
				{
					slots[i].SetItem(item, selected);
				}
			}
		}

		private void FindReferencesIfNeeded()
		{
			if (inventoryManager == null)
			{
				inventoryManager = InventoryManager.Instance;
			}

			if (slots == null || slots.Length == 0)
			{
				slots = GetComponentsInChildren<UIInventorySlot>(true);
			}

			if (windowRoot == null)
			{
				Transform windowTransform = transform.Find("InventoryWindow");
				windowRoot = windowTransform != null ? windowTransform.gameObject : null;
			}

			if (toggleButton == null)
			{
				Transform toggleTransform = transform.Find("InventoryToggleButton");
				toggleButton = toggleTransform != null ? toggleTransform.GetComponent<Button>() : null;
			}
		}

		private void InitializeSlots()
		{
			if (slots == null)
			{
				return;
			}

			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i] != null)
				{
					slots[i].Initialize(this, i);
				}
			}
		}
	}
}
