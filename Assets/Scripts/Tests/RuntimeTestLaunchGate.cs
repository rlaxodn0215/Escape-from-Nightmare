// -----------------------------------------------------------------------------
// Codex comment pass: Runtime Test Launch Gate
// Role: Runs play-mode route and puzzle checks, then records failures in a form that can be reported by editor tools.
// Scope: This script belongs to Tests\RuntimeTestLaunchGate.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EscapeFromNightmare
{
    // Runtime test helper for the Runtime Test Launch Gate scenario, including setup, execution, and readable failure output.
    public static class RuntimeTestLaunchGate
    {
        // Stores the Session Key Prefix value used by this script's runtime or editor workflow.
        private const string SessionKeyPrefix = "EscapeFromNightmare.RuntimeTestLaunchGate.";

        // Performs the Request Run operation while keeping its implementation details inside this script.
        public static void RequestRun(string runnerName)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(runnerName))
            {
                return;
            }

            SessionState.SetBool(SessionKeyPrefix + runnerName, true);
#endif
        }

        // Performs the Consume Run operation while keeping its implementation details inside this script.
        public static bool ConsumeRun(string runnerName)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(runnerName))
            {
                return false;
            }

            string key = SessionKeyPrefix + runnerName;
            bool shouldRun = SessionState.GetBool(key, false);
            if (shouldRun)
            {
                SessionState.EraseBool(key);
            }

            return shouldRun;
#else
            return false;
#endif
        }

        // Performs the Clear Run operation while keeping its implementation details inside this script.
        public static void ClearRun(string runnerName)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(runnerName))
            {
                return;
            }

            SessionState.EraseBool(SessionKeyPrefix + runnerName);
#endif
        }
    }
}
