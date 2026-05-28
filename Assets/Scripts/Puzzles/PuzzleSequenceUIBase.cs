using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class PuzzleSequenceUIBase : PuzzleUIBase
    {
        [SerializeField] protected Text sequenceText;
        [SerializeField] protected Text messageText;
        [SerializeField] protected Transform optionButtonRoot;
        [SerializeField] protected List<PuzzleSequenceOptionButton> optionButtons = new List<PuzzleSequenceOptionButton>();
        [SerializeField] protected Button submitButton;
        [SerializeField] protected Button resetButton;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected bool autoCollectOptionButtons = true;
        [SerializeField] protected bool autoSubmitWhenFull = true;
        [SerializeField] protected bool disableOptionAfterSelect = true;
        [SerializeField] protected bool refreshOptionsFromSymbolRecords = false;
        [SerializeField] protected string[] fallbackAnswerSequence;

        protected readonly List<string> currentSequence = new List<string>();
        protected string[] expectedSequence;
        protected PuzzleAnswerRecord answerRecord;

        protected virtual void Awake()
        {
            if (autoCollectOptionButtons)
            {
                CacheOptionButtons();
            }

            if (refreshOptionsFromSymbolRecords)
            {
                RefreshOptionSymbols();
            }
        }

        protected virtual void OnEnable()
        {
            HookButtons();
            RefreshDisplay();
            RefreshOptionButtons();
        }

        protected virtual void OnDisable()
        {
            UnhookButtons();
        }

        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);

            currentSequence.Clear();
            ResolveAnswer();
            RefreshDisplay();
            RefreshOptionButtons();
            SetMessage(string.Empty);
        }

        public void SelectOption(string optionId)
        {
            if (string.IsNullOrEmpty(optionId))
            {
                Debug.LogWarning("Cannot select an empty sequence option.", this);
                return;
            }

            if (expectedSequence == null || expectedSequence.Length == 0)
            {
                Debug.LogWarning("Expected sequence is empty for puzzle: " + puzzleId, this);
                return;
            }

            if (disableOptionAfterSelect && currentSequence.Contains(optionId))
            {
                return;
            }

            currentSequence.Add(optionId);
            RefreshDisplay();
            RefreshOptionButtons();

            if (autoSubmitWhenFull && currentSequence.Count >= expectedSequence.Length)
            {
                Submit();
            }
        }

        public void ResetInput()
        {
            currentSequence.Clear();
            RefreshDisplay();
            RefreshOptionButtons();
            SetMessage(string.Empty);
        }

        public void Submit()
        {
            if (expectedSequence == null || expectedSequence.Length == 0)
            {
                Debug.LogWarning("Expected sequence is empty for puzzle: " + puzzleId, this);
                return;
            }

            if (currentSequence.Count < expectedSequence.Length)
            {
                SetMessage("Not enough inputs.");
                return;
            }

            if (IsCorrectSequence())
            {
                SetMessage("Correct.");
                Complete();
                return;
            }

            SetMessage("Wrong.");
            RegisterFailure();
            currentSequence.Clear();
            RefreshDisplay();
            RefreshOptionButtons();
        }

        protected virtual void CacheOptionButtons()
        {
            Transform root = optionButtonRoot != null ? optionButtonRoot : transform;
            PuzzleSequenceOptionButton[] foundButtons = root.GetComponentsInChildren<PuzzleSequenceOptionButton>(true);

            optionButtons.Clear();
            for (int i = 0; i < foundButtons.Length; i++)
            {
                if (foundButtons[i] != null && !optionButtons.Contains(foundButtons[i]))
                {
                    optionButtons.Add(foundButtons[i]);
                    foundButtons[i].SetTarget(this);
                }
            }
        }

        protected virtual void ResolveAnswer()
        {
            answerRecord = null;
            expectedSequence = null;

            if (GameDataManager.Instance != null && puzzleRecord != null)
            {
                answerRecord = GameDataManager.Instance.GetPuzzleAnswer(puzzleRecord);
                expectedSequence = answerRecord != null ? answerRecord.answerSequence : GameDataManager.Instance.GetAnswerSequence(puzzleRecord);
            }

            if (expectedSequence == null || expectedSequence.Length == 0)
            {
                expectedSequence = fallbackAnswerSequence;
            }

            if (expectedSequence == null)
            {
                expectedSequence = new string[0];
            }

            if (expectedSequence.Length == 0)
            {
                Debug.LogWarning("Sequence answer is empty for puzzle: " + puzzleId, this);
            }
        }

        protected virtual bool IsCorrectSequence()
        {
            if (expectedSequence == null || currentSequence.Count != expectedSequence.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedSequence.Length; i++)
            {
                if (NormalizeOption(currentSequence[i]) != NormalizeOption(expectedSequence[i]))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual void RefreshDisplay()
        {
            if (sequenceText != null)
            {
                sequenceText.text = currentSequence.Count > 0 ? string.Join(" > ", currentSequence.ToArray()) : string.Empty;
            }
        }

        protected virtual void RefreshOptionButtons()
        {
            for (int i = 0; i < optionButtons.Count; i++)
            {
                PuzzleSequenceOptionButton optionButton = optionButtons[i];
                if (optionButton == null)
                {
                    continue;
                }

                bool selected = currentSequence.Contains(optionButton.OptionId);
                optionButton.SetSelected(selected);
                optionButton.SetInteractable(!disableOptionAfterSelect || !selected);
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
            if (autoCollectOptionButtons && optionButtons.Count == 0)
            {
                CacheOptionButtons();
            }

            for (int i = 0; i < optionButtons.Count; i++)
            {
                if (optionButtons[i] != null)
                {
                    optionButtons[i].SetTarget(this);
                }
            }

            if (refreshOptionsFromSymbolRecords)
            {
                RefreshOptionSymbols();
            }

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

        private string NormalizeOption(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            bool caseSensitive = answerRecord != null && answerRecord.caseSensitive;
            return caseSensitive ? value : value.ToUpperInvariant();
        }

        private void RefreshOptionSymbols()
        {
            for (int i = 0; i < optionButtons.Count; i++)
            {
                if (optionButtons[i] != null)
                {
                    optionButtons[i].RefreshVisualFromSymbolRecord();
                }
            }
        }
    }
}
