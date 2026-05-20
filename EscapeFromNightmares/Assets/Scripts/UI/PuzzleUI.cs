using System;
using EscapeFromNightmares.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    public sealed class PuzzleUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text hintLabel;
        [SerializeField] private TMP_Text feedbackLabel;
        [SerializeField] private TMP_InputField answerInput;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button clearButton;

        public event Action<string> AnswerSubmitted;
        public RectTransform ContentRoot => contentRoot;

        private void Awake()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (submitButton != null)
            {
                submitButton.onClick.AddListener(SubmitAnswer);
            }

            if (clearButton != null)
            {
                clearButton.onClick.AddListener(ClearAnswer);
            }

            if (answerInput != null)
            {
                answerInput.onSubmit.AddListener(SubmitAnswer);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }

            if (submitButton != null)
            {
                submitButton.onClick.RemoveListener(SubmitAnswer);
            }

            if (clearButton != null)
            {
                clearButton.onClick.RemoveListener(ClearAnswer);
            }

            if (answerInput != null)
            {
                answerInput.onSubmit.RemoveListener(SubmitAnswer);
            }
        }

        public void Show(PuzzleDefinition puzzle)
        {
            if (puzzle == null)
            {
                return;
            }

            Show(FormatTitle(puzzle), FormatHint(puzzle));
            ClearAnswer();
        }

        public void Show(string title = null, string hint = null)
        {
            if (!string.IsNullOrWhiteSpace(title) && titleLabel != null)
            {
                titleLabel.text = title;
            }

            if (hint != null && hintLabel != null)
            {
                hintLabel.text = hint;
            }

            SetVisible(true);
            answerInput?.ActivateInputField();
        }

        public void ShowFeedback(string message)
        {
            if (feedbackLabel != null)
            {
                feedbackLabel.text = message ?? string.Empty;
            }
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void SubmitAnswer()
        {
            SubmitAnswer(answerInput != null ? answerInput.text : string.Empty);
        }

        private void SubmitAnswer(string answer)
        {
            AnswerSubmitted?.Invoke(answer ?? string.Empty);
        }

        private void ClearAnswer()
        {
            if (answerInput != null)
            {
                answerInput.text = string.Empty;
            }

            ShowFeedback(string.Empty);
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(true);

            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private static string FormatTitle(PuzzleDefinition puzzle)
        {
            return string.IsNullOrWhiteSpace(puzzle.PuzzleId) ? "Puzzle" : puzzle.PuzzleId.Replace('_', ' ');
        }

        private static string FormatHint(PuzzleDefinition puzzle)
        {
            return puzzle.InputType switch
            {
                PuzzleInputType.NumberLock => "Enter the number sequence.",
                PuzzleInputType.SymbolSequence => "Enter symbols joined by >.",
                PuzzleInputType.SilentSequence => "Enter the silent sequence joined by >.",
                PuzzleInputType.ColorSequence => "Enter colors joined by >.",
                PuzzleInputType.SymbolItemMatching => "Enter each match as key=value; separate with ;.",
                PuzzleInputType.ItemUse => "Use the matching item id.",
                _ => string.Empty
            };
        }
    }
}
