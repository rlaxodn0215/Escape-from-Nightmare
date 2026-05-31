// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Number Code UI Base
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleNumberCodeUIBase.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Puzzle Number Code UI Base screen, translating UI input into puzzle progress and completion.
    public class PuzzleNumberCodeUIBase : PuzzleUIBase
    {
        [SerializeField] protected Text displayText;
        [SerializeField] protected Text messageText;
        [SerializeField] protected Transform numberButtonRoot;
        [SerializeField] protected List<PuzzleNumberButton> numberButtons = new List<PuzzleNumberButton>();
        [SerializeField] protected Button submitButton;
        [SerializeField] protected Button clearButton;
        [SerializeField] protected Button backspaceButton;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected bool autoCollectNumberButtons = true;
        [SerializeField] protected string fallbackAnswer;
        [SerializeField] protected int fallbackCodeLength = 4;
        [SerializeField] protected Text timerText;
        [SerializeField] protected bool useTimeLimit = true;

        // Stores the current Input value used by this script's runtime or editor workflow.
        protected string currentInput = string.Empty;
        // Stores the expected Answer value used by this script's runtime or editor workflow.
        protected string expectedAnswer = string.Empty;
        // Stores the max Length value used by this script's runtime or editor workflow.
        protected int maxLength = 4;
        // Stores the answer Record value used by this script's runtime or editor workflow.
        protected PuzzleAnswerRecord answerRecord;
        // Stores the remaining Time value used by this script's runtime or editor workflow.
        protected float remainingTime;
        // Stores the timer Routine value used by this script's runtime or editor workflow.
        protected Coroutine timerRoutine;
        // Stores the timer Running value used by this script's runtime or editor workflow.
        protected bool timerRunning;

        // Caches required component references and prepares this object before other startup code runs.
        protected virtual void Awake()
        {
            if (autoCollectNumberButtons)
            {
                CacheNumberButtons();
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        protected virtual void OnEnable()
        {
            HookButtons();
            RefreshDisplay();
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        protected virtual void OnDisable()
        {
            UnhookButtons();
            StopTimer();
        }

        // Initializes local UI and state from an external record before the player can interact with it.
        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);

            currentInput = string.Empty;
            ResolveAnswer();
            RefreshDisplay();
            SetMessage(string.Empty);
            StartTimerIfNeeded();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        public void AppendDigit(int digit)
        {
            digit = Mathf.Clamp(digit, 0, 9);

            if (currentInput.Length >= maxLength)
            {
                return;
            }

            currentInput += digit.ToString();
            RefreshDisplay();
        }

        // Performs the Clear Input operation while keeping its implementation details inside this script.
        public void ClearInput()
        {
            currentInput = string.Empty;
            RefreshDisplay();
            SetMessage(string.Empty);
        }

        // Performs the Backspace operation while keeping its implementation details inside this script.
        public void Backspace()
        {
            if (currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }

            RefreshDisplay();
        }

        // Performs the Submit operation while keeping its implementation details inside this script.
        public void Submit()
        {
            if (string.IsNullOrEmpty(expectedAnswer))
            {
                Debug.LogWarning("Expected answer is empty for puzzle: " + puzzleId, this);
                return;
            }

            if (currentInput.Length == 0)
            {
                SetMessage("Enter code.");
                return;
            }

            if (IsCorrectAnswer())
            {
                StopTimer();
                SetMessage("Correct.");
                Complete();
                return;
            }

            SetMessage("Wrong.");
            RegisterFailure();
            currentInput = string.Empty;
            RefreshDisplay();
        }

        // Closes the active UI or interaction and returns control to the normal game flow.
        public override void Close()
        {
            StopTimer();
            base.Close();
        }

        // Performs the Cache Number Buttons operation while keeping its implementation details inside this script.
        protected virtual void CacheNumberButtons()
        {
            Transform root = numberButtonRoot != null ? numberButtonRoot : transform;
            PuzzleNumberButton[] foundButtons = root.GetComponentsInChildren<PuzzleNumberButton>(true);

            numberButtons.Clear();
            for (int i = 0; i < foundButtons.Length; i++)
            {
                if (foundButtons[i] != null && !numberButtons.Contains(foundButtons[i]))
                {
                    numberButtons.Add(foundButtons[i]);
                    foundButtons[i].SetTarget(this);
                }
            }
        }

        // Performs the Resolve Answer operation while keeping its implementation details inside this script.
        protected virtual void ResolveAnswer()
        {
            answerRecord = null;
            expectedAnswer = string.Empty;

            if (GameDataManager.Instance != null && puzzleRecord != null)
            {
                answerRecord = GameDataManager.Instance.GetPuzzleAnswer(puzzleRecord);
                expectedAnswer = answerRecord != null ? answerRecord.answerText : GameDataManager.Instance.GetAnswerText(puzzleRecord);
            }

            if (string.IsNullOrEmpty(expectedAnswer))
            {
                expectedAnswer = fallbackAnswer;
            }

            if (puzzleRecord != null && puzzleRecord.codeLength > 0)
            {
                maxLength = puzzleRecord.codeLength;
            }
            else if (!string.IsNullOrEmpty(expectedAnswer))
            {
                maxLength = expectedAnswer.Length;
            }
            else
            {
                maxLength = fallbackCodeLength;
            }

            if (string.IsNullOrEmpty(expectedAnswer))
            {
                Debug.LogWarning("Number code answer is empty for puzzle: " + puzzleId, this);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        protected virtual bool IsCorrectAnswer()
        {
            return NormalizeAnswer(currentInput) == NormalizeAnswer(expectedAnswer);
        }

        // Performs the Normalize Answer operation while keeping its implementation details inside this script.
        protected virtual string NormalizeAnswer(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string normalized = value.Trim();
            bool ignoreWhitespace = answerRecord == null || answerRecord.ignoreWhitespace;
            bool caseSensitive = answerRecord != null && answerRecord.caseSensitive;

            if (ignoreWhitespace)
            {
                normalized = RemoveWhitespace(normalized);
            }

            if (!caseSensitive)
            {
                normalized = normalized.ToUpperInvariant();
            }

            return normalized;
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        protected virtual void RefreshDisplay()
        {
            if (displayText != null)
            {
                displayText.text = currentInput;
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

        // Begins this system's runtime flow and initializes any timers, events, or counters it needs.
        protected virtual void StartTimerIfNeeded()
        {
            StopTimer();

            if (!useTimeLimit || puzzleRecord == null || puzzleRecord.timeLimitSeconds <= 0f)
            {
                remainingTime = 0f;
                RefreshTimerText();
                return;
            }

            remainingTime = puzzleRecord.timeLimitSeconds;
            timerRunning = true;
            RefreshTimerText();
            timerRoutine = StartCoroutine(TimerRoutine());
        }

        // Performs the Timer Routine operation while keeping its implementation details inside this script.
        protected virtual IEnumerator TimerRoutine()
        {
            while (timerRunning && remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime < 0f)
                {
                    remainingTime = 0f;
                }

                RefreshTimerText();
                yield return null;
            }

            if (timerRunning)
            {
                HandleTimeOut();
            }
        }

        // Stops an active routine or state so the next run can start cleanly.
        protected virtual void StopTimer()
        {
            timerRunning = false;

            if (timerRoutine != null)
            {
                StopCoroutine(timerRoutine);
                timerRoutine = null;
            }
        }

        // Performs the Handle Time Out operation while keeping its implementation details inside this script.
        protected virtual void HandleTimeOut()
        {
            StopTimer();
            failedAttemptCount = 0;
            SetMessage("Time out.");

            if (NoiseManager.Instance != null)
            {
                string sourceId = !string.IsNullOrEmpty(puzzleId) ? puzzleId : gameObject.name;
                NoiseManager.Instance.MakeNoise(GetCurrentLocationId(), sourceId);
            }
            else
            {
                Debug.LogWarning("NoiseManager instance is missing.");
            }

            if (PuzzleRetryLockManager.Instance != null)
            {
                PuzzleRetryLockManager.Instance.LockPuzzle(puzzleId);
            }
            else
            {
                Debug.LogWarning("PuzzleRetryLockManager instance is missing.");
            }

            if (PuzzleManager.Instance != null)
            {
                PuzzleManager.Instance.CloseCurrentPuzzle();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        protected virtual void RefreshTimerText()
        {
            if (timerText != null)
            {
                timerText.text = remainingTime > 0f ? Mathf.CeilToInt(remainingTime).ToString() : string.Empty;
            }
        }

        // Performs the Hook Buttons operation while keeping its implementation details inside this script.
        protected virtual void HookButtons()
        {
            if (autoCollectNumberButtons && numberButtons.Count == 0)
            {
                CacheNumberButtons();
            }

            for (int i = 0; i < numberButtons.Count; i++)
            {
                if (numberButtons[i] != null)
                {
                    numberButtons[i].SetTarget(this);
                }
            }

            if (submitButton != null)
            {
                submitButton.onClick.RemoveListener(Submit);
                submitButton.onClick.AddListener(Submit);
            }

            if (clearButton != null)
            {
                clearButton.onClick.RemoveListener(ClearInput);
                clearButton.onClick.AddListener(ClearInput);
            }

            if (backspaceButton != null)
            {
                backspaceButton.onClick.RemoveListener(Backspace);
                backspaceButton.onClick.AddListener(Backspace);
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

            if (clearButton != null)
            {
                clearButton.onClick.RemoveListener(ClearInput);
            }

            if (backspaceButton != null)
            {
                backspaceButton.onClick.RemoveListener(Backspace);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
            }
        }

        // Performs the Remove Whitespace operation while keeping its implementation details inside this script.
        private string RemoveWhitespace(string value)
        {
            string result = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    result += value[i];
                }
            }

            return result;
        }
    }
}
