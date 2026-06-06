using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public abstract class BedroomStorageCodePuzzleView : PuzzleViewBase
	{
		private readonly string[] symbolNames = { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
		private readonly int[] selectedSymbolIndexes = new int[4];
		private readonly int[] selectedDigits = new int[4];
		private readonly Text[] symbolLabels = new Text[4];
		private readonly Text[] digitLabels = new Text[4];

		[SerializeField] private Sprite closeupImage;

		protected abstract int[] AnswerSymbolIndexes { get; }
		protected abstract int[] AnswerDigits { get; }

		protected override void OnOpened()
		{
			for (int i = 0; i < selectedSymbolIndexes.Length; i++)
			{
				selectedSymbolIndexes[i] = 0;
				selectedDigits[i] = 0;
			}

			RefreshLabels();
		}

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("StorageCodePanel", root);
			Image image = CreateImage("CloseupImage", panel, closeupImage, false);
			SetRect(image.GetComponent<RectTransform>(), new Vector2(0f, 62f), new Vector2(640f, 300f));
			CreateText("Hint", panel, "Set each slot to the matching symbol and number from the framed photos.", 22, TextAnchor.MiddleCenter);

			for (int i = 0; i < selectedSymbolIndexes.Length; i++)
			{
				int index = i;
				Button symbolButton = CreateButton("SymbolSlot_" + i, panel, string.Empty);
				Button digitButton = CreateButton("DigitSlot_" + i, panel, string.Empty);
				SetRect(symbolButton.GetComponent<RectTransform>(), new Vector2(-315f + i * 210f, -108f), new Vector2(150f, 70f));
				SetRect(digitButton.GetComponent<RectTransform>(), new Vector2(-315f + i * 210f, -188f), new Vector2(150f, 70f));
				symbolLabels[i] = symbolButton.GetComponentInChildren<Text>();
				digitLabels[i] = digitButton.GetComponentInChildren<Text>();
				symbolButton.onClick.AddListener(() => { selectedSymbolIndexes[index] = (selectedSymbolIndexes[index] + 1) % symbolNames.Length; RefreshLabels(); });
				digitButton.onClick.AddListener(() => { selectedDigits[index] = (selectedDigits[index] + 1) % 10; RefreshLabels(); });
			}

			Button submitButton = CreateButton("SubmitButton", panel, "Unlock");
			SetRect(submitButton.GetComponent<RectTransform>(), new Vector2(360f, -188f), new Vector2(180f, 70f));
			submitButton.onClick.AddListener(Check);
			RefreshLabels();
		}

		private void Check()
		{
			int[] answerSymbols = AnswerSymbolIndexes;
			int[] answerDigits = AnswerDigits;
			for (int i = 0; i < selectedSymbolIndexes.Length; i++)
			{
				if (selectedSymbolIndexes[i] != answerSymbols[i] || selectedDigits[i] != answerDigits[i])
				{
					ShowMessage("The lock clicks, then resets.");
					return;
				}
			}

			CompletePuzzle();
		}

		private void RefreshLabels()
		{
			for (int i = 0; i < selectedSymbolIndexes.Length; i++)
			{
				if (symbolLabels[i] != null)
				{
					symbolLabels[i].text = symbolNames[selectedSymbolIndexes[i]];
				}

				if (digitLabels[i] != null)
				{
					digitLabels[i].text = selectedDigits[i].ToString();
				}
			}
		}
	}
}
