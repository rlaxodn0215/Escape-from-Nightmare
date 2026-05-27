using System.Collections.Generic;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Owns the mutable token input state for puzzle panels.
    /// </summary>
    public sealed class PuzzleFlowController
    {
        private readonly List<string> inputTokens;

        public PuzzleFlowController(List<string> inputTokens)
        {
            this.inputTokens = inputTokens;
        }

        public IReadOnlyList<string> InputTokens => inputTokens;

        public void Reset()
        {
            inputTokens.Clear();
        }

        public void AddToken(string token, int maxTokenCount)
        {
            if (maxTokenCount <= 0)
            {
                return;
            }

            if (inputTokens.Count >= maxTokenCount)
            {
                inputTokens.RemoveAt(inputTokens.Count - 1);
            }

            inputTokens.Add(token);
        }

        public string InputLabel()
        {
            return inputTokens.Count == 0 ? "Input: -" : "Input: " + PuzzlePresenter.FormatInput(inputTokens);
        }
    }
}
