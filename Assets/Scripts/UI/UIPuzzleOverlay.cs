using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class UIPuzzleOverlay : MonoBehaviour
	{
		[SerializeField] private GameObject rootPanel;
		[SerializeField] private Transform contentRoot;
		[SerializeField] private Text titleText;
		[SerializeField] private Text messageText;
		[SerializeField] private Button closeButton;
		[SerializeField] private PuzzleViewBase[] puzzleViews = Array.Empty<PuzzleViewBase>();

		private readonly Dictionary<string, PuzzleViewBase> viewsById = new Dictionary<string, PuzzleViewBase>(StringComparer.Ordinal);
		private PuzzleViewBase currentView;

		public bool IsOpen => rootPanel != null && rootPanel.activeSelf;

		private void Awake()
		{
			FindReferencesIfNeeded();
			BuildViewLookup();
			SetOpen(false);
		}

		private void OnEnable()
		{
			BindCloseButton();
		}

		private void OnDisable()
		{
			UnbindCloseButton();
		}

		public bool OpenPuzzle(string puzzleId)
		{
			if (string.IsNullOrWhiteSpace(puzzleId))
			{
				return false;
			}

			FindReferencesIfNeeded();
			BuildViewLookup();

			if (!viewsById.TryGetValue(puzzleId, out PuzzleViewBase view) || view == null)
			{
				Debug.LogWarning($"UIPuzzleOverlay could not find puzzle view '{puzzleId}'.", this);
				return false;
			}

			SetOpen(true);
			ShowView(view);
			return true;
		}

		public void Close()
		{
			if (currentView != null)
			{
				currentView.Close();
				currentView.gameObject.SetActive(false);
				currentView = null;
			}

			SetOpen(false);
		}

		public void CompleteCurrentPuzzle()
		{
			if (currentView == null)
			{
				return;
			}

			bool newlySolved = PuzzleManager.Instance != null && PuzzleManager.Instance.SolvePuzzle(currentView.PuzzleId);
			SetMessage(newlySolved ? "Solved." : "Already solved.");
		}

		public void SetMessage(string message)
		{
			if (messageText != null)
			{
				messageText.text = message ?? string.Empty;
			}
		}

		private void ShowView(PuzzleViewBase view)
		{
			if (currentView != null)
			{
				currentView.Close();
				currentView.gameObject.SetActive(false);
			}

			currentView = view;
			foreach (PuzzleViewBase puzzleView in puzzleViews)
			{
				if (puzzleView != null)
				{
					puzzleView.gameObject.SetActive(puzzleView == view);
				}
			}

			if (titleText != null)
			{
				titleText.text = currentView.Title;
			}

			SetMessage(string.Empty);
			currentView.Open(this);
		}

		private void SetOpen(bool open)
		{
			if (rootPanel != null)
			{
				rootPanel.SetActive(open);
			}
		}

		private void BuildViewLookup()
		{
			viewsById.Clear();
			if (puzzleViews == null || puzzleViews.Length == 0)
			{
				puzzleViews = contentRoot != null ? contentRoot.GetComponentsInChildren<PuzzleViewBase>(true) : GetComponentsInChildren<PuzzleViewBase>(true);
			}

			foreach (PuzzleViewBase view in puzzleViews)
			{
				if (view == null || string.IsNullOrWhiteSpace(view.PuzzleId))
				{
					continue;
				}

				viewsById[view.PuzzleId] = view;
				view.gameObject.SetActive(false);
			}
		}

		private void FindReferencesIfNeeded()
		{
			if (rootPanel == null)
			{
				rootPanel = gameObject;
			}

			if (contentRoot == null)
			{
				Transform found = transform.Find("ContentRoot");
				contentRoot = found != null ? found : transform;
			}

			if (closeButton == null)
			{
				Transform closeTransform = transform.Find("CloseButton");
				closeButton = closeTransform != null ? closeTransform.GetComponent<Button>() : null;
			}
		}

		private void BindCloseButton()
		{
			if (closeButton != null)
			{
				closeButton.onClick.RemoveListener(Close);
				closeButton.onClick.AddListener(Close);
			}
		}

		private void UnbindCloseButton()
		{
			if (closeButton != null)
			{
				closeButton.onClick.RemoveListener(Close);
			}
		}
	}
}
