using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class DollhouseWindowPuzzleView : PuzzleViewBase
	{
		private readonly bool[] windows = new bool[9];
		private readonly bool[] answer = { true, false, true, false, true, false, true, false, true };
		private readonly Text[] labels = new Text[9];

		protected override string DefaultPuzzleId => "ChildRoom_DollhouseWindows";
		protected override string DefaultTitle => "Dollhouse Windows";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("DollhousePanel", root);
			CreateText("Hint", panel, "Click the windows until the lit rooms match the tiny house.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < windows.Length; i++)
			{
				int index = i;
				Button button = CreateButton("Window_" + i, panel, string.Empty);
				int column = i % 3;
				int row = i / 3;
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-150f + column * 150f, 100f - row * 105f), new Vector2(110f, 80f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { windows[index] = !windows[index]; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Latch Windows");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(310f, -165f), new Vector2(230f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < windows.Length; i++) windows[i] = false;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (windows[i] != answer[i])
				{
					ShowMessage("A window clicks back out of place.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = windows[i] ? "Lit" : "Dark";
			}
		}
	}
}
