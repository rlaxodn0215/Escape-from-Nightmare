using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class FinalFramePuzzleView : PuzzleViewBase
	{
		private readonly int[] pieces = new int[4];
		private readonly int[] answer = { 3, 1, 0, 2 };
		private readonly Text[] labels = new Text[4];

		protected override string DefaultPuzzleId => "Final_FramePieceOrder";
		protected override string DefaultTitle => "The Large Frame";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("FinalFramePanel", root);
			CreateText("Hint", panel, "Place the four pieces in the order revealed in the basement.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < pieces.Length; i++)
			{
				int index = i;
				Button button = CreateButton("FramePiece_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-225f + i * 150f, 20f), new Vector2(120f, 170f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { pieces[index] = (pieces[index] + 1) % 4; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Finish Frame");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(240f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < pieces.Length; i++) pieces[i] = i;
			RefreshLabels();
		}

		private void Check()
		{
			if (PuzzleManager.Instance == null || !PuzzleManager.Instance.HasFlag(PuzzleManager.FlagFinalPieceOrderHintKnown))
			{
				ShowMessage("The order is still missing.");
				return;
			}

			for (int i = 0; i < answer.Length; i++)
			{
				if (pieces[i] != answer[i])
				{
					ShowMessage("The frame rejects the arrangement.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = "Slot " + (i + 1) + "\nPiece " + (pieces[i] + 1);
			}
		}
	}
}
