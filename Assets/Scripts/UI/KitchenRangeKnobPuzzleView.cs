using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class KitchenRangeKnobPuzzleView : PuzzleViewBase
	{
		private readonly string[] directions = { "Up", "Right", "Down", "Left" };
		private readonly int[] knobs = new int[3];
		private readonly int[] answer = { 3, 1, 2 };
		private readonly Text[] labels = new Text[3];

		protected override string DefaultPuzzleId => "Kitchen_RangeKnobDirection";
		protected override string DefaultTitle => "Range Knobs";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("RangeKnobPanel", root);
			CreateText("Hint", panel, "Turn the knobs in the direction released by the basement lock.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < knobs.Length; i++)
			{
				int index = i;
				Button button = CreateButton("Knob_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-180f + i * 180f, 20f), new Vector2(150f, 150f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { knobs[index] = (knobs[index] + 1) % directions.Length; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Turn");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < knobs.Length; i++) knobs[i] = 0;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (knobs[i] != answer[i])
				{
					ShowMessage("Gas hisses, then stops.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = "Knob " + (i + 1) + "\n" + directions[knobs[i]];
			}
		}
	}
}
