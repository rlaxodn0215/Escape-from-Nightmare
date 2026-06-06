using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class KitchenCabinetPuzzleView : PuzzleViewBase
	{
		private readonly string[] contents = { "Plate", "Jar", "Bowl", "Cup" };
		private readonly int[] slots = new int[4];
		private readonly int[] answer = { 1, 2, 0, 3 };
		private readonly Text[] labels = new Text[4];

		protected override string DefaultPuzzleId => "Kitchen_CabinetArrangement";
		protected override string DefaultTitle => "Cabinet Arrangement";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("CabinetPanel", root);
			CreateText("Hint", panel, "Arrange the cabinet from the fireplace clue.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < slots.Length; i++)
			{
				int index = i;
				Button button = CreateButton("CabinetSlot_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-225f + i * 150f, 20f), new Vector2(120f, 170f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { slots[index] = (slots[index] + 1) % contents.Length; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Arrange");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			for (int i = 0; i < slots.Length; i++) slots[i] = 0;
			RefreshLabels();
		}

		private void Check()
		{
			for (int i = 0; i < answer.Length; i++)
			{
				if (slots[i] != answer[i])
				{
					ShowMessage("The cabinet rattles, then settles.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = contents[slots[i]];
			}
		}
	}
}
