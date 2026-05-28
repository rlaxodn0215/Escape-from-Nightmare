using UnityEngine;

namespace EscapeFromNightmare
{
    public class PuzzleUI_LockedRoomFinal : PuzzleSymbolCycleUIBase
    {
        private bool sequenceResolved;

        public override void Initialize(PuzzleRecord record)
        {
            sequenceResolved = false;
            base.Initialize(record);
            // TODO: Bind locked room final puzzle-specific visuals.
        }

        public void UseRequiredDevice()
        {
            if (!sequenceResolved)
            {
                SetMessage("Solve the symbols first.");
                return;
            }

            if (puzzleRecord == null || string.IsNullOrEmpty(puzzleRecord.requiredItemId))
            {
                Debug.LogWarning("LockedRoomFinal requiredItemId is empty.", this);
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            if (!InventoryManager.Instance.TryUseSelectedItem(puzzleRecord.requiredItemId))
            {
                SetMessage("Use the modified device.");
                return;
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SetFinalChaseStarted(true);
                SaveManager.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            Complete();
        }

        protected override void OnCorrectSequenceResolved()
        {
            sequenceResolved = true;
            SetMessage("Use the modified device.");
        }
    }
}
