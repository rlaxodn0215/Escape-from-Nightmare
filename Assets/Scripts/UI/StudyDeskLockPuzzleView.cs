using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class StudyDeskLockPuzzleView : PuzzleViewBase
	{
		private readonly int[] digits = new int[4];
		private readonly int[] answer = { 1, 9, 3, 4 };
		private readonly Text[] labels = new Text[4];

		protected override string DefaultPuzzleId => "Study_DeskLockPassword";
		protected override string DefaultTitle => "Desk Lock";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("DeskLockPanel", root);
			CreateText("Hint", panel, "Set the lower desk lock from the hallway arrangement.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < digits.Length; i++)
			{
				int index = i;
				Button button = CreateButton("Digit_" + i, panel, "0");
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-225f + i * 150f, 20f), new Vector2(110f, 150f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { digits[index] = (digits[index] + 1) % 10; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Unlock");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < digits.Length; i++) digits[i] = 0;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (digits[i] != answer[i])
				{
					ShowMessage("The lock refuses the number.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = digits[i].ToString();
			}
		}
	}
}
