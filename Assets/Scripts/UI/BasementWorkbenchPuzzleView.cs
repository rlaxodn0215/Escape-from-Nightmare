using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class BasementWorkbenchPuzzleView : PuzzleViewBase
	{
		private int progress;
		private readonly int[] answer = { 1, 3, 0, 2 };

		protected override string DefaultPuzzleId => "Basement_WorkbenchClueBox";
		protected override string DefaultTitle => "Workbench Box";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("WorkbenchPanel", root);
			CreateText("Hint", panel, "Compare the box marks with the final clue drawing.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < 4; i++)
			{
				int index = i;
				Button button = CreateButton("Mark_" + i, panel, "Mark " + (i + 1));
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-225f + i * 150f, 20f), new Vector2(120f, 170f));
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
				ShowMessage("The marks no longer line up.");
				return;
			}

			progress++;
			if (progress >= answer.Length)
			{
				CompletePuzzle();
			}
			else
			{
				ShowMessage("The lid loosens.");
			}
		}
	}
}
