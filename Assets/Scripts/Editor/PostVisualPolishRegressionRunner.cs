// -----------------------------------------------------------------------------
// Codex comment pass: Post Visual Polish Regression Runner
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\PostVisualPolishRegressionRunner.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Editor utility for the Post Visual Polish Regression Runner workflow, exposed through menu items or called by other validation tools.
    public static class PostVisualPolishRegressionRunner
    {
        // Editor utility for the Report Status workflow, exposed through menu items or called by other validation tools.
        private class ReportStatus
        {
            // Stores the name value used by this script's runtime or editor workflow.
            public string name;
            // Stores the path value used by this script's runtime or editor workflow.
            public string path;
            // Stores the exists value used by this script's runtime or editor workflow.
            public bool exists;
            // Stores the summary value used by this script's runtime or editor workflow.
            public string summary;
        }

        [MenuItem("Escape From Nightmare/Tests/Run Post Visual Polish Regression Suite")]
        // Performs the Run Post Visual Polish Regression Suite operation while keeping its implementation details inside this script.
        public static void RunPostVisualPolishRegressionSuite()
        {
            Debug.Log("[PostVisualPolishRegressionRunner] Running safe editor-side visual checks. Play Mode runtime tests must still be launched through their test menus.");

            VisualResourceValidator.ValidateVisualResources();
            ArtResourceBindingValidator.ValidateArtResourceBindings();
            WriteReport();
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
        private static void WriteReport()
        {
            string path = Path.Combine(Application.dataPath, "Docs/GeneratedPostVisualPolishRegressionReport.md");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                List<ReportStatus> statuses = BuildReportStatuses();

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Post Visual Polish Regression Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Editor Safe Checks Invoked: Visual Resource Validator, Art Resource Binding Validator");
                builder.AppendLine("- Play Mode Tests: Not chained by this menu; run through UnityMCP or the individual test menus.");
                builder.AppendLine();
                builder.AppendLine("## Required Execution Order");
                builder.AppendLine();
                builder.AppendLine("1. Escape From Nightmare / Validate Game Data");
                builder.AppendLine("2. Escape From Nightmare / Validate Puzzle Prefab Contracts");
                builder.AppendLine("3. Escape From Nightmare / Validate Source Route Scene Wiring");
                builder.AppendLine("4. Escape From Nightmare / Visual Polish / Validate Visual Resources");
                builder.AppendLine("5. Escape From Nightmare / Art Resources / Validate Art Resource Bindings");
                builder.AppendLine("6. Escape From Nightmare / Tests / Run First Five Puzzle Runtime Tests");
                builder.AppendLine("7. Escape From Nightmare / Tests / Run Remaining Puzzle Runtime Tests");
                builder.AppendLine("8. Escape From Nightmare / Tests / Run Full Game Route Runtime Test");
                builder.AppendLine("9. Escape From Nightmare / Tests / Run GameScene Interaction Runtime Tests");
                builder.AppendLine();
                builder.AppendLine("## Latest Report Files");
                builder.AppendLine();
                builder.AppendLine("| Report | Exists | Summary |");
                builder.AppendLine("|---|---:|---|");
                for (int i = 0; i < statuses.Count; i++)
                {
                    ReportStatus status = statuses[i];
                    builder.AppendLine("| " + status.name + " | " + (status.exists ? "Yes" : "No") + " | " + Escape(status.summary) + " |");
                }

                builder.AppendLine();
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- Runtime test menus enter Play Mode, so they are safer to run one at a time through UnityMCP.");
                builder.AppendLine("- Missing Sprite warnings are expected before actual art files are placed in `Assets/ArtIntake` or `Assets/Resources`.");
                builder.AppendLine("- This report intentionally does not modify Scene, Prefab, or JSON data.");

                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
                Debug.Log("[PostVisualPolishRegressionRunner] Report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogError("[PostVisualPolishRegressionRunner] Could not write report: " + exception.Message);
            }
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static List<ReportStatus> BuildReportStatuses()
        {
            List<ReportStatus> statuses = new List<ReportStatus>();
            AddStatus(statuses, "Game Data", "GeneratedGameDataValidationReport.md");
            AddStatus(statuses, "Puzzle Prefab Contracts", "GeneratedPuzzlePrefabContractReport.md");
            AddStatus(statuses, "Source Route Scene Wiring", "GeneratedSourceRouteSceneWiringReport.md");
            AddStatus(statuses, "Visual Resources", "GeneratedVisualResourceReport.md");
            AddStatus(statuses, "Art Resource Bindings", "GeneratedArtBindingValidationReport.md");
            AddStatus(statuses, "First Five Runtime", "GeneratedFirstFivePuzzleRuntimeTestReport.md");
            AddStatus(statuses, "Remaining Runtime", "GeneratedRemainingPuzzleRuntimeTestReport.md");
            AddStatus(statuses, "Full Route Runtime", "GeneratedFullGameRouteRuntimeTestReport.md");
            AddStatus(statuses, "GameScene Interaction Runtime", "GeneratedGameSceneInteractionRuntimeTestReport.md");
            return statuses;
        }

        // Performs the Add Status operation while keeping its implementation details inside this script.
        private static void AddStatus(List<ReportStatus> statuses, string name, string fileName)
        {
            string fullPath = Path.Combine(Application.dataPath, "Docs", fileName);
            bool exists = File.Exists(fullPath);
            statuses.Add(new ReportStatus
            {
                name = name,
                path = fullPath,
                exists = exists,
                summary = exists ? SummarizeReport(fullPath) : "Report file is missing."
            });
        }

        // Performs the Summarize Report operation while keeping its implementation details inside this script.
        private static string SummarizeReport(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);
                List<string> summary = new List<string>();
                for (int i = 0; i < lines.Length && summary.Count < 5; i++)
                {
                    string line = lines[i].Trim();
                    if (line.StartsWith("- Errors:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Warnings:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Total:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Total Steps:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Passed:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Failed:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Required Missing:", StringComparison.OrdinalIgnoreCase)
                        || line.StartsWith("- Required Found:", StringComparison.OrdinalIgnoreCase))
                    {
                        summary.Add(line.TrimStart('-', ' '));
                    }
                }

                return summary.Count > 0 ? string.Join("; ", summary.ToArray()) : "Report exists.";
            }
            catch (Exception exception)
            {
                return "Could not summarize: " + exception.Message;
            }
        }

        // Performs the Escape operation while keeping its implementation details inside this script.
        private static string Escape(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
