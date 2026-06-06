using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class KitchenTableclothPuzzleView : PuzzleViewBase
	{
		private int progress;
		private readonly int[] answer = { 3, 0, 2, 1 };

		protected override string DefaultPuzzleId => "Kitchen_TableclothSymbol";
		protected override string DefaultTitle => "Folded Tablecloth";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("TableclothPanel", root);
			CreateText("Hint", panel, "Unfold the corners in the hidden symbol order.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < 4; i++)
			{
				int index = i;
				Button button = CreateButton("Corner_" + i, panel, "Corner " + (i + 1));
				float x = i % 2 == 0 ? -180f : 180f;
				float y = i < 2 ? 70f : -80f;
				SetRect(button.GetComponent<RectTransform>(), new Vector2(x, y), new Vector2(210f, 110f));
				button.onClick.AddListener(() => Press(index));
			}
		}

		protected override void OnOpened()
		{
			progress = 0;
		}

		private void Press(int index)
		{
			if (answer[progress] != index)
			{
				progress = 0;
				ShowMessage("The cloth folds itself back.");
				return;
			}

			progress++;
			if (progress >= answer.Length)
			{
				CompletePuzzle();
			}
			else
			{
				ShowMessage("A symbol edge appears.");
			}
		}
	}
}
