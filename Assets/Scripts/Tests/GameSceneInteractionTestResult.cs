using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    public class GameSceneInteractionTestResult
    {
        public int index;
        public string category;
        public string targetId;
        public bool passed;
        public string message;
        public float durationSeconds;
    }

    [Serializable]
    public class GameSceneInteractionTestReport
    {
        public string generatedAt;
        public string activeScene;
        public int totalCount;
        public int passedCount;
        public int failedCount;
        public List<GameSceneInteractionTestResult> results;
    }
}
