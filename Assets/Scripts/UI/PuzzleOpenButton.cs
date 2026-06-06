using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class PuzzleOpenButton : MonoBehaviour
	{
		[SerializeField] private string puzzleId;
		[SerializeField] private Button button;
		[SerializeField] private string[] requiredFlagIds = System.Array.Empty<string>();
		[SerializeField] private string[] requiredSolvedPuzzleIds = System.Array.Empty<string>();
		[SerializeField] private string[] requiredItemIds = System.Array.Empty<string>();
		[SerializeField] private bool hideWhenSolved;

		private void Awake()
		{
			FindReferencesIfNeeded();
		}

		private void OnEnable()
		{
			FindReferencesIfNeeded();
			Bind();
			Subscribe();
			Refresh();
		}

		private void OnDisable()
		{
			Unbind();
			Unsubscribe();
		}

		private void HandleClicked()
		{
			if (PuzzleManager.Instance == null)
			{
				return;
			}

			if (!MeetsConditions())
			{
				PlayLockedSound();
				return;
			}

			PuzzleManager.Instance.OpenPuzzle(puzzleId);
		}

		private void Refresh()
		{
			bool available = MeetsConditions();
			if (button != null)
			{
				button.interactable = available;
			}

			if (hideWhenSolved && PuzzleManager.Instance != null && PuzzleManager.Instance.HasSolved(puzzleId))
			{
				gameObject.SetActive(false);
			}
		}

		private bool MeetsConditions()
		{
			return PuzzleManager.Instance != null
				&& !string.IsNullOrWhiteSpace(puzzleId)
				&& PuzzleManager.Instance.MeetsConditions(requiredFlagIds, requiredSolvedPuzzleIds, requiredItemIds);
		}

		private void Subscribe()
		{
			if (PuzzleManager.Instance != null)
			{
				PuzzleManager.Instance.PuzzleStateChanged -= Refresh;
				PuzzleManager.Instance.PuzzleStateChanged += Refresh;
			}
		}

		private void Unsubscribe()
		{
			if (PuzzleManager.Instance != null)
			{
				PuzzleManager.Instance.PuzzleStateChanged -= Refresh;
			}
		}

		private void Bind()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(HandleClicked);
				button.onClick.AddListener(HandleClicked);
			}
		}

		private void Unbind()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(HandleClicked);
			}
		}

		private void FindReferencesIfNeeded()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}
		}

		private static void PlayLockedSound()
		{
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlaySfx(AudioSoundId.DoorLocked);
			}
		}
	}
}
