using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class PickupItemButton : MonoBehaviour
	{
		[SerializeField] private ItemDefinition item;
		[SerializeField] private Button button;
		[SerializeField] private string[] requiredFlagIds = System.Array.Empty<string>();
		[SerializeField] private string[] requiredSolvedPuzzleIds = System.Array.Empty<string>();
		[SerializeField] private string[] requiredItemIds = System.Array.Empty<string>();

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

			Subscribe();
			RefreshAvailableState();
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

			Unsubscribe();
		}

		private void HandleClicked()
		{
			if (!MeetsConditions())
			{
				if (AudioManager.Instance != null)
				{
					AudioManager.Instance.PlaySfx(AudioSoundId.DoorLocked);
				}

				return;
			}

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

		private void RefreshAvailableState()
		{
			if (button != null)
			{
				button.interactable = MeetsConditions();
			}
		}

		private bool MeetsConditions()
		{
			if (PuzzleManager.Instance != null)
			{
				return PuzzleManager.Instance.MeetsConditions(requiredFlagIds, requiredSolvedPuzzleIds, requiredItemIds);
			}

			return IsEmpty(requiredFlagIds) && IsEmpty(requiredSolvedPuzzleIds) && IsEmpty(requiredItemIds);
		}

		private static bool IsEmpty(string[] values)
		{
			if (values == null || values.Length == 0)
			{
				return true;
			}

			foreach (string value in values)
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					return false;
				}
			}

			return true;
		}

		private void Subscribe()
		{
			if (PuzzleManager.Instance != null)
			{
				PuzzleManager.Instance.PuzzleStateChanged -= RefreshAvailableState;
				PuzzleManager.Instance.PuzzleStateChanged += RefreshAvailableState;
			}
		}

		private void Unsubscribe()
		{
			if (PuzzleManager.Instance != null)
			{
				PuzzleManager.Instance.PuzzleStateChanged -= RefreshAvailableState;
			}
		}
	}
}
