// -----------------------------------------------------------------------------
// Codex comment pass: Game Scene Builder Category Stats
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\SourceRouteGameSceneBuilderReport.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Editor utility for the Game Scene Builder Category Stats workflow, exposed through menu items or called by other validation tools.
    public class SourceRouteGameSceneBuilderCategoryStats
    {
        // Stores the category value used by this script's runtime or editor workflow.
        public string category;
        // Stores the created value used by this script's runtime or editor workflow.
        public int created;
        // Stores the reused value used by this script's runtime or editor workflow.
        public int reused;
        // Stores the linked value used by this script's runtime or editor workflow.
        public int linked;
        // Stores the warnings value used by this script's runtime or editor workflow.
        public int warnings;
        // Stores the errors value used by this script's runtime or editor workflow.
        public int errors;
    }
}
