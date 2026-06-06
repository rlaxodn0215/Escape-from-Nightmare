namespace EscapeFromNightmare
{
	public class BedroomBackStorageCodePuzzleView : BedroomStorageCodePuzzleView
	{
		private static readonly int[] Symbols = { 4, 1, 0, 5 };
		private static readonly int[] Digits = { 1, 0, 6, 5 };

		protected override string DefaultPuzzleId => "Bedroom_BackStorageCode";
		protected override string DefaultTitle => "Back Storage Lock";
		protected override int[] AnswerSymbolIndexes => Symbols;
		protected override int[] AnswerDigits => Digits;
	}
}
