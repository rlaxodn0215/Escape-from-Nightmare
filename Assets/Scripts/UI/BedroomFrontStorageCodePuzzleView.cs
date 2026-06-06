namespace EscapeFromNightmare
{
	public class BedroomFrontStorageCodePuzzleView : BedroomStorageCodePuzzleView
	{
		private static readonly int[] Symbols = { 0, 2, 3, 1 };
		private static readonly int[] Digits = { 6, 2, 5, 7 };

		protected override string DefaultPuzzleId => "Bedroom_FrontStorageCode";
		protected override string DefaultTitle => "Front Storage Lock";
		protected override int[] AnswerSymbolIndexes => Symbols;
		protected override int[] AnswerDigits => Digits;
	}
}
