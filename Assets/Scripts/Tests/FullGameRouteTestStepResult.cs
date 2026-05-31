// -----------------------------------------------------------------------------
// Codex comment pass: Full Game Route Test Step Result
// Role: Runs play-mode route and puzzle checks, then records failures in a form that can be reported by editor tools.
// Scope: This script belongs to Tests\FullGameRouteTestStepResult.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    // Runtime test helper for the Full Game Route Test Step Result scenario, including setup, execution, and readable failure output.
    public class FullGameRouteTestStepResult
    {
        // Stores the step Index value used by this script's runtime or editor workflow.
        public int stepIndex;
        // Stores the step Name value used by this script's runtime or editor workflow.
        public string stepName;
        // Stores the target Id value used by this script's runtime or editor workflow.
        public string targetId;
        // Stores the passed value used by this script's runtime or editor workflow.
        public bool passed;
        // Stores the message value used by this script's runtime or editor workflow.
        public string message;
        // Stores the duration Seconds value used by this script's runtime or editor workflow.
        public float durationSeconds;
    }

    [Serializable]
    // Runtime test helper for the Full Game Route Test Report scenario, including setup, execution, and readable failure output.
    public class FullGameRouteTestReport
    {
        // Stores the generated At value used by this script's runtime or editor workflow.
        public string generatedAt;
        // Stores the active Scene value used by this script's runtime or editor workflow.
        public string activeScene;
        // Stores the total Steps value used by this script's runtime or editor workflow.
        public int totalSteps;
        // Stores the passed Steps value used by this script's runtime or editor workflow.
        public int passedSteps;
        // Stores the failed Steps value used by this script's runtime or editor workflow.
        public int failedSteps;
        // Stores the steps value used by this script's runtime or editor workflow.
        public List<FullGameRouteTestStepResult> steps = new List<FullGameRouteTestStepResult>();
    }
}
