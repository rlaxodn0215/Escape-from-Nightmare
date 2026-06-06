using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class BedroomFrameCluePuzzleView : PuzzleViewBase
	{
		[SerializeField] private Sprite clueImage;
		[SerializeField] private string clueDescription;

		protected override string DefaultPuzzleId => "Bedroom_FrameClue";
		protected override string DefaultTitle => "Framed Photograph";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("FrameCluePanel", root);
			Image image = CreateImage("ClueImage", panel, clueImage, false);
			SetRect(image.GetComponent<RectTransform>(), Vector2.zero, new Vector2(768f, 512f));

			if (!string.IsNullOrWhiteSpace(clueDescription))
			{
				Text description = CreateText("Description", panel, clueDescription, 20, TextAnchor.MiddleCenter);
				SetRect(description.GetComponent<RectTransform>(), new Vector2(0f, -236f), new Vector2(720f, 42f));
			}
		}
	}
}
