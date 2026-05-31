// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Sequence UI Base
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleSequenceUIBase.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Puzzle Sequence UI Base screen, translating UI input into puzzle progress and completion.
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

        // Stores the current Sequence value used by this script's runtime or editor workflow.
        protected readonly List<string> currentSequence = new List<string>();
        // Stores the expected Sequence value used by this script's runtime or editor workflow.
        protected string[] expectedSequence;
        // Stores the answer Record value used by this script's runtime or editor workflow.
        protected PuzzleAnswerRecord answerRecord;

        // Caches required component references and prepares this object before other startup code runs.
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

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        protected virtual void OnEnable()
        {
            HookButtons();
            RefreshDisplay();
            RefreshOptionButtons();
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

            currentSequence.Clear();
            ResolveAnswer();
            RefreshDisplay();
            RefreshOptionButtons();
            SetMessage(string.Empty);
        }

        // Performs the Select Option operation while keeping its implementation details inside this script.
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

        // Returns runtime state to its defaults for a new game, retry, or clean test run.
        public void ResetInput()
        {
            currentSequence.Clear();
            RefreshDisplay();
            RefreshOptionButtons();
            SetMessage(string.Empty);
        }

        // Performs the Submit operation while keeping its implementation details inside this script.
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

        // Performs the Cache Option Buttons operation while keeping its implementation details inside this script.
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

        // Performs the Resolve Answer operation while keeping its implementation details inside this script.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Re-reads current game data and manager state, then redraws the visible UI.
        protected virtual void RefreshDisplay()
        {
            if (sequenceText != null)
            {
                sequenceText.text = currentSequence.Count > 0 ? string.Join(" > ", currentSequence.ToArray()) : string.Empty;
            }
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
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

        // Performs the Unhook Buttons operation while keeping its implementation details inside this script.
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

        // Performs the Normalize Option operation while keeping its implementation details inside this script.
        private string NormalizeOption(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            bool caseSensitive = answerRecord != null && answerRecord.caseSensitive;
            return caseSensitive ? value : value.ToUpperInvariant();
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
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
