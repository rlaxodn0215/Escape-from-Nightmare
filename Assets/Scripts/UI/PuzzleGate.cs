using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class PuzzleGate : MonoBehaviour
	{
		[SerializeField] private string[] requiredFlagIds = System.Array.Empty<string>();
		[SerializeField] private string[] requiredSolvedPuzzleIds = System.Array.Empty<string>();
		[SerializeField] private string[] requiredItemIds = System.Array.Empty<string>();
		[SerializeField] private bool invertCondition;
		[SerializeField] private bool controlGameObjectActive;
		[SerializeField] private Button targetButton;

		private void Awake()
		{
			FindReferencesIfNeeded();
		}

		private void OnEnable()
		{
			FindReferencesIfNeeded();
			Subscribe();
			Refresh();
		}

		private void OnDisable()
		{
			Unsubscribe();
		}

		public void Refresh()
		{
			bool open = PuzzleManager.Instance != null
				&& PuzzleManager.Instance.MeetsConditions(requiredFlagIds, requiredSolvedPuzzleIds, requiredItemIds);

			if (invertCondition)
			{
				open = !open;
			}

			if (targetButton != null)
			{
				targetButton.interactable = open;
			}

			if (controlGameObjectActive)
			{
				gameObject.SetActive(open);
			}
		}

		private void Subscribe()
		{
			if (PuzzleManager.Instance != null)
			{
				PuzzleManager.Instance.PuzzleStateChanged -= Refresh;
				PuzzleManager.Instance.PuzzleStateChanged += Refresh;
			}

			if (InventoryManager.Instance != null)
			{
				InventoryManager.Instance.InventoryChanged -= Refresh;
				InventoryManager.Instance.InventoryChanged += Refresh;
			}
		}

		private void Unsubscribe()
		{
			if (PuzzleManager.Instance != null)
			{
				PuzzleManager.Instance.PuzzleStateChanged -= Refresh;
			}

			if (InventoryManager.Instance != null)
			{
				InventoryManager.Instance.InventoryChanged -= Refresh;
			}
		}

		private void FindReferencesIfNeeded()
		{
			if (targetButton == null)
			{
				targetButton = GetComponent<Button>();
			}
		}
	}
}
