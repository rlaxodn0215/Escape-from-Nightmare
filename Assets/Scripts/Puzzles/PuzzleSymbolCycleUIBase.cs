using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class PuzzleSymbolCycleUIBase : PuzzleUIBase
    {
        [SerializeField] protected Text sequenceText;
        [SerializeField] protected Text messageText;
        [SerializeField] protected Transform slotRoot;
        [SerializeField] protected List<PuzzleSymbolCycleSlot> slots = new List<PuzzleSymbolCycleSlot>();
        [SerializeField] protected Button submitButton;
        [SerializeField] protected Button resetButton;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected bool autoCollectSlots = true;
        [SerializeField] protected int expectedSlotCount = 5;
        [SerializeField] protected string[] availableSymbolIds;

        protected string[] expectedSequence;

        public string[] AvailableSymbolIds
        {
            get { return availableSymbolIds; }
        }

        protected virtual void Awake()
        {
            ResolveAvailableSymbols();

            if (autoCollectSlots)
            {
                CacheSlots();
            }
        }

        protected virtual void OnEnable()
        {
            HookButtons();
            RefreshDisplay();
        }

        protected virtual void OnDisable()
        {
            UnhookButtons();
        }

        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            ResolveAvailableSymbols();
            CacheSlots();
            ResolveAnswer();
            ResetInput();
            SetMessage(string.Empty);
        }

        public virtual int IndexOfAvailableSymbol(string symbolId)
        {
            if (availableSymbolIds == null || string.IsNullOrEmpty(symbolId))
            {
                return -1;
            }

            for (int i = 0; i < availableSymbolIds.Length; i++)
            {
                if (availableSymbolIds[i] == symbolId)
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual void CycleSlot(PuzzleSymbolCycleSlot slot)
        {
            if (slot == null)
            {
                return;
            }

            slot.CycleNext();
            RefreshDisplay();
        }

        public virtual void ResetInput()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].Clear();
                }
            }

            RefreshDisplay();
            SetMessage(string.Empty);
        }

        public virtual void Submit()
        {
            if (expectedSequence == null || expectedSequence.Length == 0)
            {
                Debug.LogWarning("Expected symbol sequence is empty for puzzle: " + puzzleId, this);
                return;
            }

            if (!IsCompleteInput())
            {
                SetMessage("Not enough inputs.");
                return;
            }

            if (IsCorrectSequence())
            {
                SetMessage("Correct.");
                OnCorrectSequenceResolved();
                return;
            }

            SetMessage("Wrong.");
            RegisterFailure();
        }

        protected virtual void CacheSlots()
        {
            Transform root = slotRoot != null ? slotRoot : transform;
            PuzzleSymbolCycleSlot[] foundSlots = root.GetComponentsInChildren<PuzzleSymbolCycleSlot>(true);

            slots.Clear();
            for (int i = 0; i < foundSlots.Length; i++)
            {
                if (foundSlots[i] != null && !slots.Contains(foundSlots[i]))
                {
                    slots.Add(foundSlots[i]);
                    foundSlots[i].SetTarget(this);
                    foundSlots[i].SetSlotIndex(slots.Count - 1);
                }
            }

            if (slots.Count < expectedSlotCount)
            {
                Debug.LogWarning("Symbol cycle slot count is lower than expected. Puzzle: " + puzzleId + ", Expected: " + expectedSlotCount + ", Found: " + slots.Count, this);
            }
        }

        protected virtual void ResolveAvailableSymbols()
        {
            if (availableSymbolIds != null && availableSymbolIds.Length > 0)
            {
                return;
            }

            List<string> ids = new List<string>();
            if (GameDataManager.Instance != null && GameDataManager.Instance.Symbols != null)
            {
                IReadOnlyList<SymbolRecord> symbols = GameDataManager.Instance.Symbols;
                for (int i = 0; i < symbols.Count; i++)
                {
                    if (symbols[i] != null && !string.IsNullOrEmpty(symbols[i].symbolId))
                    {
                        ids.Add(symbols[i].symbolId);
                    }
                }
            }

            availableSymbolIds = ids.Count > 0
                ? ids.ToArray()
                : new string[] { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
        }

        protected virtual void ResolveAnswer()
        {
            expectedSequence = null;

            if (GameDataManager.Instance != null && puzzleRecord != null)
            {
                expectedSequence = GameDataManager.Instance.GetAnswerSequence(puzzleRecord);
            }

            if (expectedSequence == null)
            {
                expectedSequence = new string[0];
            }

            if (expectedSequence.Length == 0)
            {
                Debug.LogWarning("Symbol cycle answer is empty for puzzle: " + puzzleId, this);
            }
        }

        protected virtual bool IsCompleteInput()
        {
            if (slots.Count < expectedSlotCount)
            {
                return false;
            }

            for (int i = 0; i < expectedSlotCount; i++)
            {
                if (slots[i] == null || string.IsNullOrEmpty(slots[i].CurrentSymbolId))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual bool IsCorrectSequence()
        {
            string[] currentSequence = GetCurrentSequence();
            if (expectedSequence == null || currentSequence.Length != expectedSequence.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedSequence.Length; i++)
            {
                string current = currentSequence[i] != null ? currentSequence[i] : string.Empty;
                string expected = expectedSequence[i] != null ? expectedSequence[i] : string.Empty;
                if (current.ToUpperInvariant() != expected.ToUpperInvariant())
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual string[] GetCurrentSequence()
        {
            int count = Mathf.Min(expectedSlotCount, slots.Count);
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = slots[i] != null ? slots[i].CurrentSymbolId : string.Empty;
            }

            return result;
        }

        protected virtual void OnCorrectSequenceResolved()
        {
            Complete();
        }

        protected virtual void RefreshDisplay()
        {
            if (sequenceText != null)
            {
                sequenceText.text = string.Join(" > ", GetCurrentSequence());
            }
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
            if (submitButton != null)
            {
                submitButton.onClick.RemoveListener(Submit);
                submitButton.onClick.AddListener(Submit);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveListener(ResetInput);
                resetButton.onClick.AddListener(ResetInput);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
                closeButton.onClick.AddListener(Close);
            }
        }

        protected virtual void UnhookButtons()
        {
            if (submitButton != null)
            {
                submitButton.onClick.RemoveListener(Submit);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveListener(ResetInput);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
            }
        }
    }
}
