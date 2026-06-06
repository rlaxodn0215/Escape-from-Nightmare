using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class LivingRoomClockCandlePuzzleView : PuzzleViewBase
	{
		private int hour;
		private readonly int[] candles = new int[3];
		private readonly Text[] labels = new Text[4];

		protected override string DefaultPuzzleId => "LivingRoom_ClockCandleOrder";
		protected override string DefaultTitle => "Clock and Candles";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("ClockCandlePanel", root);
			CreateText("Hint", panel, "Set the clock and candle order from the missing family photo.", 24, TextAnchor.MiddleCenter);
			Button clock = CreateButton("ClockButton", panel, string.Empty);
			SetRect(clock.GetComponent<RectTransform>(), new Vector2(-270f, 20f), new Vector2(170f, 170f));
			labels[0] = clock.GetComponentInChildren<Text>();
			clock.onClick.AddListener(() => { hour = hour % 12 + 1; RefreshLabels(); });
			for (int i = 0; i < candles.Length; i++)
			{
				int index = i;
				Button candle = CreateButton("Candle_" + i, panel, string.Empty);
				SetRect(candle.GetComponent<RectTransform>(), new Vector2(-60f + i * 160f, 20f), new Vector2(130f, 170f));
				labels[i + 1] = candle.GetComponentInChildren<Text>();
				candle.onClick.AddListener(() => { candles[index] = (candles[index] + 1) % 3; RefreshLabels(); });
			}

			Button submit = CreateButton("SubmitButton", panel, "Light");
			SetRect(submit.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(220f, 70f));
			submit.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			hour = 1;
			for (int i = 0; i < candles.Length; i++) candles[i] = 0;
			RefreshLabels();
		}

		private void Check()
		{
			if (hour == 7 && candles[0] == 2 && candles[1] == 0 && candles[2] == 1)
			{
				CompletePuzzle();
			}
			else
			{
				ShowMessage("The fireplace remains cold.");
			}
		}

		private void RefreshLabels()
		{
			if (labels[0] != null) labels[0].text = "Clock\n" + hour + ":00";
			for (int i = 0; i < candles.Length; i++)
			{
				if (labels[i + 1] != null) labels[i + 1].text = "Candle\n" + (candles[i] + 1);
			}
		}
	}
}
