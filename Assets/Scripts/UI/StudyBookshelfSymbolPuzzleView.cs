using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class StudyBookshelfSymbolPuzzleView : PuzzleViewBase
	{
		private int progress;
		private readonly int[] answer = { 0, 2, 4, 1 };

		protected override string DefaultPuzzleId => "Study_BookshelfSymbol";
		protected override string DefaultTitle => "Bookshelf Symbols";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("BookshelfPanel", root);
			CreateText("Hint", panel, "Press the hidden book symbols in the order from the clue.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < 5; i++)
			{
				int index = i;
				Button button = CreateButton("Symbol_" + i, panel, "Symbol " + (i + 1));
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-320f + i * 160f, 20f), new Vector2(130f, 170f));
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
				ShowMessage("The shelf goes still.");
				return;
			}

			progress++;
			ShowMessage("A book shifts.");
			if (progress >= answer.Length)
			{
				CompletePuzzle();
			}
		}
	}
}
