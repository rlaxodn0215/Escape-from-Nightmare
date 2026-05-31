// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Answer Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\PuzzleAnswerRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Puzzle Answer Record data container whose field names intentionally match the project data files.
    public class PuzzleAnswerRecord
    {
        // Stores the puzzle Id value used by this script's runtime or editor workflow.
        public string puzzleId;
        // Stores the answer Variable Name value used by this script's runtime or editor workflow.
        public string answerVariableName;
        // Stores the answer Text value used by this script's runtime or editor workflow.
        public string answerText;
        // Stores the answer Sequence value used by this script's runtime or editor workflow.
        public string[] answerSequence;
        // Stores the case Sensitive value used by this script's runtime or editor workflow.
        public bool caseSensitive;
        // Stores the ignore Whitespace value used by this script's runtime or editor workflow.
        public bool ignoreWhitespace;
    }
}
