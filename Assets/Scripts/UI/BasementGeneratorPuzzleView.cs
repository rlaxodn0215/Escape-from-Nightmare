using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class BasementGeneratorPuzzleView : PuzzleViewBase
	{
		private readonly bool[] lights = new bool[6];
		private readonly bool[] answer = { true, false, true, true, false, true };
		private readonly Text[] labels = new Text[6];

		protected override string DefaultPuzzleId => "Basement_GeneratorRedLight";
		protected override string DefaultTitle => "Generator Lights";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("GeneratorPanel", root);
			CreateText("Hint", panel, "Match the red light pattern.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < lights.Length; i++)
			{
				int index = i;
				Button button = CreateButton("Light_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-300f + i * 120f, 20f), new Vector2(90f, 120f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { lights[index] = !lights[index]; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Start");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < lights.Length; i++) lights[i] = false;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (lights[i] != answer[i])
				{
					ShowMessage("The generator coughs out.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = lights[i] ? "Red" : "Off";
			}
		}
	}
}
