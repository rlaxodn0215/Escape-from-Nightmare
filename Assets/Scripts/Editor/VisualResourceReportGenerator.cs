using UnityEditor;

namespace EscapeFromNightmare
{
    public static class VisualResourceReportGenerator
    {
        [MenuItem("Escape From Nightmare/Visual Polish/Generate Visual Resource Requirement Report", false, 100)]
        public static void GenerateVisualResourceRequirementReport()
        {
            VisualResourceValidator.GenerateRequirementReport();
        }
    }
}
