using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    public class FullGameRouteTestStepResult
    {
        public int stepIndex;
        public string stepName;
        public string targetId;
        public bool passed;
        public string message;
        public float durationSeconds;
    }

    [Serializable]
    public class FullGameRouteTestReport
    {
        public string generatedAt;
        public string activeScene;
        public int totalSteps;
        public int passedSteps;
        public int failedSteps;
        public List<FullGameRouteTestStepResult> steps = new List<FullGameRouteTestStepResult>();
    }
}
