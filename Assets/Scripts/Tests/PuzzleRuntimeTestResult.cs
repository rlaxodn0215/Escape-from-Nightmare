using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    public class PuzzleRuntimeTestResult
    {
        public string testName;
        public string puzzleId;
        public bool passed;
        public string message;
        public float durationSeconds;
    }

    [Serializable]
    public class PuzzleRuntimeTestReport
    {
        public string generatedAt;
        public int totalCount;
        public int passedCount;
        public int failedCount;
        public List<PuzzleRuntimeTestResult> results = new List<PuzzleRuntimeTestResult>();
    }
}
