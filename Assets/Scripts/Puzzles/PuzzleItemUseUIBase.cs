using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class PuzzleItemUseUIBase : PuzzleUIBase
    {
        [SerializeField] protected Text messageText;
        [SerializeField] protected Button useSelectedItemButton;
        [SerializeField] protected Button closeButton;

        protected virtual void OnEnable()
        {
            HookButtons();
        }

        protected virtual void OnDisable()
        {
            UnhookButtons();
        }

        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            SetMessage(string.Empty);
        }

        public virtual void UseSelectedItem()
        {
            if (puzzleRecord == null || string.IsNullOrEmpty(puzzleRecord.requiredItemId))
            {
                Debug.LogWarning("ItemUse puzzle has no requiredItemId: " + puzzleId, this);
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

        protected virtual void SetMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

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
