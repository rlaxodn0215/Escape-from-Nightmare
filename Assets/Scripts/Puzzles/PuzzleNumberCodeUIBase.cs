using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
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

        protected string currentInput = string.Empty;
        protected string expectedAnswer = string.Empty;
        protected int maxLength = 4;
        protected PuzzleAnswerRecord answerRecord;
        protected float remainingTime;
        protected Coroutine timerRoutine;
        protected bool timerRunning;

        protected virtual void Awake()
        {
            if (autoCollectNumberButtons)
            {
                CacheNumberButtons();
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
            StopTimer();
        }

        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);

            currentInput = string.Empty;
            ResolveAnswer();
            RefreshDisplay();
            SetMessage(string.Empty);
            StartTimerIfNeeded();
        }

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

        public void ClearInput()
        {
            currentInput = string.Empty;
            RefreshDisplay();
            SetMessage(string.Empty);
        }

        public void Backspace()
        {
            if (currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }

            RefreshDisplay();
        }

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

        public override void Close()
        {
            StopTimer();
            base.Close();
        }

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

        protected virtual bool IsCorrectAnswer()
        {
            return NormalizeAnswer(currentInput) == NormalizeAnswer(expectedAnswer);
        }

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

        protected virtual void RefreshDisplay()
        {
            if (displayText != null)
            {
                displayText.text = currentInput;
            }
        }

        protected virtual void SetMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

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

        protected virtual void StopTimer()
        {
            timerRunning = false;

            if (timerRoutine != null)
            {
                StopCoroutine(timerRoutine);
                timerRoutine = null;
            }
        }

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

        protected virtual void RefreshTimerText()
        {
            if (timerText != null)
            {
                timerText.text = remainingTime > 0f ? Mathf.CeilToInt(remainingTime).ToString() : string.Empty;
            }
        }

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
