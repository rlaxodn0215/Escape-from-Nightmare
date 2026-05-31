// -----------------------------------------------------------------------------
// Codex comment pass: Game Scene Interaction Test Result
// Role: Runs play-mode route and puzzle checks, then records failures in a form that can be reported by editor tools.
// Scope: This script belongs to Tests\GameSceneInteractionTestResult.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EscapeFromNightmare
{
    [Serializable]
    // Runtime test helper for the Game Scene Interaction Test Result scenario, including setup, execution, and readable failure output.
    public class GameSceneInteractionTestResult
    {
        // Stores the index value used by this script's runtime or editor workflow.
        public int index;
        // Stores the category value used by this script's runtime or editor workflow.
        public string category;
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
    // Runtime test helper for the Game Scene Interaction Test Report scenario, including setup, execution, and readable failure output.
    public class GameSceneInteractionTestReport
    {
        // Stores the generated At value used by this script's runtime or editor workflow.
        public string generatedAt;
        // Stores the active Scene value used by this script's runtime or editor workflow.
        public string activeScene;
        // Stores the total Count value used by this script's runtime or editor workflow.
        public int totalCount;
        // Stores the passed Count value used by this script's runtime or editor workflow.
        public int passedCount;
        // Stores the failed Count value used by this script's runtime or editor workflow.
        public int failedCount;
        // Stores the results value used by this script's runtime or editor workflow.
        public List<GameSceneInteractionTestResult> results;
    }
}
