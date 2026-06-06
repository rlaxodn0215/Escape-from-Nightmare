using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class KitchenBasementLockPuzzleView : PuzzleViewBase
	{
		private bool metalInstalled;
		private bool fuseInstalled;
		private Text metalText;
		private Text fuseText;

		protected override string DefaultPuzzleId => "Kitchen_BasementDoorLock";
		protected override string DefaultTitle => "Basement Door Lock";

		protected override void BuildPuzzle(RectTransform root)
		{
			RectTransform panel = CreatePanel("BasementLockPanel", root);
			CreateText("Hint", panel, "Mount both pieces into the basement lock.", 24, TextAnchor.MiddleCenter);
			Button metalButton = CreateButton("MetalButton", panel, "Metal Piece");
			Button fuseButton = CreateButton("FuseButton", panel, "Fuse Part");
			Button submitButton = CreateButton("SubmitButton", panel, "Power Lock");
			SetRect(metalButton.GetComponent<RectTransform>(), new Vector2(-190f, 20f), new Vector2(230f, 150f));
			SetRect(fuseButton.GetComponent<RectTransform>(), new Vector2(190f, 20f), new Vector2(230f, 150f));
			SetRect(submitButton.GetComponent<RectTransform>(), new Vector2(0f, -185f), new Vector2(230f, 70f));
			metalText = metalButton.GetComponentInChildren<Text>();
			fuseText = fuseButton.GetComponentInChildren<Text>();
			metalButton.onClick.AddListener(InstallMetal);
			fuseButton.onClick.AddListener(InstallFuse);
			submitButton.onClick.AddListener(Check);
			RefreshLabels();
		}

		protected override void OnOpened()
		{
			metalInstalled = PuzzleManager.Instance != null && PuzzleManager.Instance.HasFlag(PuzzleManager.FlagBasementMetalPieceInstalled);
			fuseInstalled = PuzzleManager.Instance != null && PuzzleManager.Instance.HasFlag(PuzzleManager.FlagBasementFuseInstalled);
			RefreshLabels();
		}

		private void InstallMetal()
		{
			if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(PuzzleManager.ItemBasementMetalPiece))
			{
				metalInstalled = true;
				RefreshLabels();
			}
			else
			{
				ShowMessage("The metal slot is still empty.");
			}
		}

		private void InstallFuse()
		{
			if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem("BasementFuse"))
			{
				fuseInstalled = true;
				RefreshLabels();
			}
			else
			{
				ShowMessage("A fuse part is missing.");
			}
		}

		private void Check()
		{
			if (metalInstalled && fuseInstalled)
			{
				CompletePuzzle();
			}
			else
			{
				ShowMessage("The lock needs both parts.");
			}
		}

		private void RefreshLabels()
		{
			if (metalText != null) metalText.text = metalInstalled ? "Metal\nInstalled" : "Metal\nMissing";
			if (fuseText != null) fuseText.text = fuseInstalled ? "Fuse\nInstalled" : "Fuse\nMissing";
		}
	}
}
