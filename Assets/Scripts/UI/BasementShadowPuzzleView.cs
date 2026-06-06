using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class BasementShadowPuzzleView : PuzzleViewBase
	{
		private readonly int[] objects = new int[3];
		private readonly int[] answer = { 2, 0, 1 };
		private readonly string[] labelsSource = { "Bottle", "Box", "Jar" };
		private readonly Text[] labels = new Text[3];

		protected override string DefaultPuzzleId => "Basement_ShadowBottleBox";
		protected override string DefaultTitle => "Shelf Shadows";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("ShadowPanel", root);
			CreateText("Hint", panel, "Arrange the shelf objects by their shadows.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < objects.Length; i++)
			{
				int index = i;
				Button button = CreateButton("Object_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-180f + i * 180f, 20f), new Vector2(150f, 170f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { objects[index] = (objects[index] + 1) % labelsSource.Length; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Compare");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < objects.Length; i++) objects[i] = 0;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (objects[i] != answer[i])
				{
					ShowMessage("The shadows stretch into the wrong shape.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = labelsSource[objects[i]];
			}
		}
	}
}
