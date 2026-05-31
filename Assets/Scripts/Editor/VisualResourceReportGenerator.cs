// -----------------------------------------------------------------------------
// Codex comment pass: Visual Resource Report Generator
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\VisualResourceReportGenerator.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEditor;

namespace EscapeFromNightmare
{
    // Editor utility for the Visual Resource Report Generator workflow, exposed through menu items or called by other validation tools.
    public static class VisualResourceReportGenerator
    {
        [MenuItem("Escape From Nightmare/Visual Polish/Generate Visual Resource Requirement Report", false, 100)]
        // Performs the Generate Visual Resource Requirement Report operation while keeping its implementation details inside this script.
        public static void GenerateVisualResourceRequirementReport()
        {
            VisualResourceValidator.GenerateRequirementReport();
        }
    }
}
