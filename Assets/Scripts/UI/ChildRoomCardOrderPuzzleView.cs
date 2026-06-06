using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class ChildRoomCardOrderPuzzleView : PuzzleViewBase
	{
		private readonly string[] symbols = { "Moon", "Star", "Key", "Eye" };
		private readonly int[] slots = new int[4];
		private readonly int[] answer = { 2, 0, 3, 1 };
		private readonly Text[] labels = new Text[4];

		protected override string DefaultPuzzleId => "ChildRoom_CardOrder";
		protected override string DefaultTitle => "Card Pattern Order";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("CardOrderPanel", root);
			CreateText("Hint", panel, "Place the cards in the order hinted by the child's room.", 24, TextAnchor.MiddleCenter);
			for (int i = 0; i < slots.Length; i++)
			{
				int index = i;
				Button button = CreateButton("CardSlot_" + i, panel, string.Empty);
				SetRect(button.GetComponent<RectTransform>(), new Vector2(-270f + i * 180f, 20f), new Vector2(150f, 180f));
				labels[i] = button.GetComponentInChildren<Text>();
				button.onClick.AddListener(() => { slots[index] = (slots[index] + 1) % symbols.Length; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Set Cards");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -190f), new Vector2(230f, 70f));
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
					ShowMessage("The cards are not telling the right story.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < labels.Length; i++)
			{
				if (labels[i] != null) labels[i].text = "Slot " + (i + 1) + "\n" + symbols[slots[i]];
			}
		}
	}
}
