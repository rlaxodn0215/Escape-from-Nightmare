// -----------------------------------------------------------------------------
// Codex comment pass: Locked Room Final
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleUI_LockedRoomFinal.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Locked Room Final screen, translating UI input into puzzle progress and completion.
    public class PuzzleUI_LockedRoomFinal : PuzzleSymbolCycleUIBase
    {
        [SerializeField] private Text itemUseMessageText;
        [SerializeField] private Button useClockworkDeviceButton;
        [SerializeField] private string requiredFinalItemId = "ModifiedClockworkDevice";

        // Stores the sequence Solved value used by this script's runtime or editor workflow.
        private bool sequenceSolved;

        public bool IsSequenceSolvedForTest
        {
            get { return sequenceSolved; }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        protected override void OnEnable()
        {
            base.OnEnable();

            if (useClockworkDeviceButton != null)
            {
                useClockworkDeviceButton.onClick.RemoveListener(UseRequiredItem);
                useClockworkDeviceButton.onClick.AddListener(UseRequiredItem);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        protected override void OnDisable()
        {
            if (useClockworkDeviceButton != null)
            {
                useClockworkDeviceButton.onClick.RemoveListener(UseRequiredItem);
            }

            base.OnDisable();
        }

        // Initializes local UI and state from an external record before the player can interact with it.
        public override void Initialize(PuzzleRecord record)
        {
            sequenceSolved = false;
            base.Initialize(record);
            SetItemUseMessage(string.Empty);
            // TODO: Bind locked room final puzzle-specific visuals.
        }

        // Performs the Use Required Item operation while keeping its implementation details inside this script.
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

        // Performs the Use Required Device operation while keeping its implementation details inside this script.
        public void UseRequiredDevice()
        {
            UseRequiredItem();
        }

        // Performs the On Correct Sequence Resolved operation while keeping its implementation details inside this script.
        protected override void OnCorrectSequenceResolved()
        {
            sequenceSolved = true;
            SetMessage("Use the modified device.");
            SetItemUseMessage("Use the modified clockwork device.");
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetItemUseMessage(string message)
        {
            if (itemUseMessageText != null)
            {
                itemUseMessageText.text = message;
            }
        }
    }
}
