using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class HallPortraitGazePuzzleView : PuzzleViewBase
	{
		private readonly int[] states = new int[4];
		private readonly Text[] labels = new Text[4];
		private readonly string[] gazeLabels = { "Left", "Right", "Up", "Down" };
		private readonly int[] answer = { 1, 0, 3, 2 };

		protected override string DefaultPuzzleId => "Hall_PortraitGazeCross";
		protected override string DefaultTitle => "Crossing Gazes";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("PortraitGazePanel", root);
			CreateText("Hint", panel, "Turn the portraits until their gazes cross in the hall.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < states.Length; i++)
			{
				int index = i;
				Button button = CreateButton("Portrait_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-270f + i * 180f, 25f), new Vector2(150f, 170f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { states[index] = (states[index] + 1) % gazeLabels.Length; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Align");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < states.Length; i++) states[i] = 0;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (states[i] != answer[i])
				{
					ShowMessage("Their eyes refuse to meet.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = "Portrait " + (i + 1) + "\n" + gazeLabels[states[i]];
			}
		}
	}
}
