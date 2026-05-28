using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class PuzzleAnswerRecord
    {
        public string puzzleId;
        public string answerVariableName;
        public string answerText;
        public string[] answerSequence;
        public bool caseSensitive;
        public bool ignoreWhitespace;
    }
}
