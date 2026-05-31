// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Item Use UI Base
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleItemUseUIBase.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Puzzle Item Use UI Base screen, translating UI input into puzzle progress and completion.
    public class PuzzleItemUseUIBase : PuzzleUIBase
    {
        [SerializeField] protected Text messageText;
        [SerializeField] protected Button useSelectedItemButton;
        [SerializeField] protected Button closeButton;

        public string RequiredItemId
        {
            get { return puzzleRecord != null ? puzzleRecord.requiredItemId : string.Empty; }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        protected virtual void OnEnable()
        {
            HookButtons();
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        protected virtual void OnDisable()
        {
            UnhookButtons();
        }

        // Initializes local UI and state from an external record before the player can interact with it.
        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            SetMessage(string.Empty);
        }

        // Performs the Use Selected Item operation while keeping its implementation details inside this script.
        public virtual void UseSelectedItem()
        {
            if (puzzleRecord == null)
            {
                Debug.LogWarning("ItemUse puzzle has no PuzzleRecord: " + puzzleId, this);
                return;
            }

            if (string.IsNullOrEmpty(puzzleRecord.requiredItemId))
            {
                SetMessage("Correct.");
                Complete();
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            if (InventoryManager.Instance.TryUseSelectedItem(puzzleRecord.requiredItemId))
            {
                SetMessage("Correct.");
                Complete();
                return;
            }

            SetMessage("Required item is not selected.");
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        protected virtual void SetMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        // Performs the Hook Buttons operation while keeping its implementation details inside this script.
        protected virtual void HookButtons()
        {
            if (useSelectedItemButton != null)
            {
                useSelectedItemButton.onClick.RemoveListener(UseSelectedItem);
                useSelectedItemButton.onClick.AddListener(UseSelectedItem);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
                closeButton.onClick.AddListener(Close);
            }
        }

        // Performs the Unhook Buttons operation while keeping its implementation details inside this script.
        protected virtual void UnhookButtons()
        {
            if (useSelectedItemButton != null)
            {
                useSelectedItemButton.onClick.RemoveListener(UseSelectedItem);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
            }
        }
    }
}
