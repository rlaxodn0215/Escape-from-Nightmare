using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    public static class PostVisualPolishRegressionRunner
    {
        private class ReportStatus
        {
            public string name;
            public string path;
            public bool exists;
            public string summary;
        }

        [MenuItem("Escape From Nightmare/Tests/Run Post Visual Polish Regression Suite")]
        public static void RunPostVisualPolishRegressionSuite()
        {
            Debug.Log("[PostVisualPolishRegressionRunner] Running safe editor-side visual checks. Play Mode runtime tests must still be launched through their test menus.");

            VisualResourceValidator.ValidateVisualResources();
            ArtResourceBindingValidator.ValidateArtResourceBindings();
            WriteReport();
        }

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

        private static string Escape(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
