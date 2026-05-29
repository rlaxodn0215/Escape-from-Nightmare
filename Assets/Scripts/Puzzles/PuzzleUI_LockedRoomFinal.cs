using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class PuzzleUI_LockedRoomFinal : PuzzleSymbolCycleUIBase
    {
        [SerializeField] private Text itemUseMessageText;
        [SerializeField] private Button useClockworkDeviceButton;
        [SerializeField] private string requiredFinalItemId = "ModifiedClockworkDevice";

        private bool sequenceSolved;

        public bool IsSequenceSolvedForTest
        {
            get { return sequenceSolved; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (useClockworkDeviceButton != null)
            {
                useClockworkDeviceButton.onClick.RemoveListener(UseRequiredItem);
                useClockworkDeviceButton.onClick.AddListener(UseRequiredItem);
            }
        }

        protected override void OnDisable()
        {
            if (useClockworkDeviceButton != null)
            {
                useClockworkDeviceButton.onClick.RemoveListener(UseRequiredItem);
            }

            base.OnDisable();
        }

        public override void Initialize(PuzzleRecord record)
        {
            sequenceSolved = false;
            base.Initialize(record);
            SetItemUseMessage(string.Empty);
            // TODO: Bind locked room final puzzle-specific visuals.
        }

        public void UseRequiredItem()
        {
            if (!sequenceSolved)
            {
                SetItemUseMessage("Solve the symbols first.");
                return;
            }

            string requiredItemId = !string.IsNullOrEmpty(requiredFinalItemId)
                ? requiredFinalItemId
                : (puzzleRecord != null ? puzzleRecord.requiredItemId : string.Empty);

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            if (!InventoryManager.Instance.TryUseSelectedItem(requiredItemId))
            {
                SetItemUseMessage("Use the modified clockwork device.");
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

        public void UseRequiredDevice()
        {
            UseRequiredItem();
        }

        protected override void OnCorrectSequenceResolved()
        {
            sequenceSolved = true;
            SetMessage("Use the modified device.");
            SetItemUseMessage("Use the modified clockwork device.");
        }

        private void SetItemUseMessage(string message)
        {
            if (itemUseMessageText != null)
            {
                itemUseMessageText.text = message;
            }
        }
    }
}
