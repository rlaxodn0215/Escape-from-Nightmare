using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmare
{
    public static class VisualResourceValidator
    {
        private class ResourceCheck
        {
            public ArtResourceRequirement requirement;
            public bool resourcesFound;
            public bool artIntakeFound;
            public string resourcesAssetPath;
            public string artIntakeAssetPath;
            public string status;
            public string notes;
        }

        private static readonly List<string> errors = new List<string>();
        private static readonly List<string> warnings = new List<string>();
        private static readonly List<string> infos = new List<string>();
        private static readonly List<ResourceCheck> checks = new List<ResourceCheck>();

        private static readonly string[] RequiredViewIds =
        {
            "Bedroom_Front", "Bedroom_Right", "Bedroom_Back", "Bedroom_Left",
            "ChildRoom_Front", "ChildRoom_Right", "ChildRoom_Back", "ChildRoom_Left",
            "Study_Front", "Study_Right", "Study_Back", "Study_Left",
            "SecondFloorHallway_Front", "SecondFloorHallway_Back",
            "LivingRoom_Front", "LivingRoom_Back",
            "Entrance_Front",
            "Kitchen_Front",
            "BasementStorage_Front", "BasementStorage_Right", "BasementStorage_Back", "BasementStorage_Left",
            "LockedRoom_Front", "LockedRoom_Right", "LockedRoom_Back", "LockedRoom_Left"
        };

        [MenuItem("Escape From Nightmare/Visual Polish/Validate Visual Resources")]
        public static void ValidateVisualResources()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            ValidateRequirements();
            WriteValidationReport();
            Debug.Log("Visual resource validation completed. Errors: " + errors.Count + ", Warnings: " + warnings.Count + ", Info: " + infos.Count);
        }

        public static void GenerateVisualResourceRequirementReportMenu()
        {
            GenerateRequirementReport();
        }

        public static void GenerateRequirementReport()
        {
            string path = Path.Combine(Application.dataPath, "Docs/VisualResourceRequirements.md");
            try
            {
                ArtResourceCatalog.EnsureFolders();
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                List<ArtResourceRequirement> requirements = ArtResourceCatalog.GetRequirements();
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Visual Resource Requirements");
                builder.AppendLine();
                builder.AppendLine("All paths below are `Resources.Load<Sprite>()` paths. Do not include file extensions in JSON or component fields.");
                builder.AppendLine();
                builder.AppendLine("## Import Settings");
                builder.AppendLine();
                builder.AppendLine("| Type | Recommended Resolution | Transparency | Max Size | Notes |");
                builder.AppendLine("|---|---|---|---:|---|");
                builder.AppendLine("| Backgrounds | 1920x1080 or 2048+ 16:9 | No | 4096 | Full location view backgrounds |");
                builder.AppendLine("| Clue / Examine Images | 1920x1080 or high panel resolution | Optional | 2048 | Used in clue viewer panels |");
                builder.AppendLine("| Item Icons | 512x512 | Yes, PNG recommended | 512 | Inventory icons |");
                builder.AppendLine("| Symbols | 512x512 | Yes, PNG recommended | 512 | Puzzle symbol buttons and slots |");
                builder.AppendLine("| Ghost | 1024-2048 | Yes, PNG recommended | 2048 | Optional ghost visual |");
                builder.AppendLine("| UI Buttons / Panels | 512-1024 | Optional | 1024 | Optional skin sprites; 9-slice can be configured later |");
                builder.AppendLine();
                builder.AppendLine("## Required And Optional Assets");
                builder.AppendLine();
                builder.AppendLine("| Type | ID | Resources Path | ArtIntake Path | Example Resources File |");
                builder.AppendLine("|---|---|---|---|---|");
                for (int i = 0; i < requirements.Count; i++)
                {
                    ArtResourceRequirement requirement = requirements[i];
                    string type = requirement.optional ? requirement.type + " (Optional)" : requirement.type;
                    string fileName = Path.GetFileName(requirement.resourcesPath);
                    string artIntakePath = "Assets/ArtIntake/" + requirement.intakeFolder + "/" + fileName + ".png";
                    string resourcesFile = "Assets/Resources/" + requirement.resourcesPath + ".png";
                    builder.AppendLine("| " + Escape(type) + " | " + Escape(requirement.id) + " | " + Escape(requirement.resourcesPath) + " | " + Escape(artIntakePath) + " | " + Escape(resourcesFile) + " |");
                }

                builder.AppendLine();
                builder.AppendLine("## Naming Rules");
                builder.AppendLine();
                builder.AppendLine("- File names must match the final segment of the Resources path.");
                builder.AppendLine("- Correct: `Bedroom_Front.png`, `OldDrawerKey.png`, `Symbol_01.png`.");
                builder.AppendLine("- Incorrect: `bedroom front.png`, `Bedroom-Front.png`, `old_drawer_key.png`, `Symbol1.png`.");
                builder.AppendLine("- `Resources.Load` paths never include file extensions.");
                builder.AppendLine("- Unity Texture Type must be `Sprite`.");
                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
                Debug.Log("[VisualResourceReportGenerator] Requirement report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogError("[VisualResourceReportGenerator] Could not write requirement report: " + exception.Message);
            }
        }

        public static string[] GetRequiredViewIds()
        {
            return RequiredViewIds;
        }

        private static void ValidateRequirements()
        {
            List<ArtResourceRequirement> requirements = ArtResourceCatalog.GetRequirements();
            for (int i = 0; i < requirements.Count; i++)
            {
                ArtResourceRequirement requirement = requirements[i];
                ResourceCheck check = CheckRequirement(requirement);
                checks.Add(check);

                if (requirement.optional)
                {
                    if (!check.resourcesFound)
                    {
                        AddInfo("Optional visual resource missing: " + requirement.resourcesPath);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(requirement.resourcesPath))
                    {
                        AddWarning(requirement.type + " Resources path is empty: " + requirement.id);
                    }
                    else if (Path.HasExtension(requirement.resourcesPath))
                    {
                        AddWarning(requirement.type + " Resources path should be extensionless: " + requirement.id + " / " + requirement.resourcesPath);
                    }

                    if (!check.resourcesFound)
                    {
                        AddWarning(requirement.type + " sprite missing at Resources path: " + requirement.resourcesPath);
                    }
                }
            }
        }

        private static ResourceCheck CheckRequirement(ArtResourceRequirement requirement)
        {
            string resourcesAssetPath = ArtResourceCatalog.FindExistingAssetPath(requirement.resourcesPath);
            string intakeAssetPath = ArtResourceCatalog.FindMatchingIntakePath(requirement);
            bool resourcesFound = !string.IsNullOrEmpty(requirement.resourcesPath) && Resources.Load<Sprite>(requirement.resourcesPath) != null;
            bool artIntakeFound = !string.IsNullOrEmpty(intakeAssetPath);
            string status;
            string notes;

            if (resourcesFound)
            {
                status = "Resources Ready";
                notes = resourcesAssetPath;
            }
            else if (artIntakeFound)
            {
                status = "Ready to copy";
                notes = intakeAssetPath;
            }
            else
            {
                status = requirement.optional ? "Optional Missing" : "Missing";
                notes = "No matching Sprite in Resources" + (requirement.optional ? string.Empty : " or ArtIntake");
            }

            return new ResourceCheck
            {
                requirement = requirement,
                resourcesFound = resourcesFound,
                artIntakeFound = artIntakeFound,
                resourcesAssetPath = resourcesAssetPath,
                artIntakeAssetPath = intakeAssetPath,
                status = status,
                notes = notes
            };
        }

        private static void WriteValidationReport()
        {
            string path = Path.Combine(Application.dataPath, "Docs/GeneratedVisualResourceReport.md");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Visual Resource Validation Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Errors: " + errors.Count);
                builder.AppendLine("- Warnings: " + warnings.Count);
                builder.AppendLine("- Info: " + infos.Count);
                builder.AppendLine("- Required Found: " + CountRequiredFound());
                builder.AppendLine("- Required Missing: " + CountRequiredMissing());
                builder.AppendLine("- Optional Found: " + CountOptionalFound());
                builder.AppendLine("- Optional Missing: " + CountOptionalMissing());
                builder.AppendLine("- ArtIntake Ready: " + CountArtIntakeReady());
                builder.AppendLine("- Resources Ready: " + CountResourcesReady());
                builder.AppendLine();
                AppendResourceTable(builder, "Backgrounds", "View ID", "Background");
                AppendResourceTable(builder, "Clue Images", "Clue ID", "Clue Image");
                AppendResourceTable(builder, "Item Icons", "Item ID", "Item Icon");
                AppendResourceTable(builder, "Symbols", "Symbol ID", "Symbol Sprite");
                AppendResourceTable(builder, "Optional UI / Ghost", "Asset", "Optional");
                builder.AppendLine("## Next Steps");
                builder.AppendLine();
                builder.AppendLine("- Missing background images should be placed under `Assets/ArtIntake/Backgrounds` or `Assets/Resources/Backgrounds`.");
                builder.AppendLine("- Missing clue images should be placed under `Assets/ArtIntake/ClueImages`, `Assets/ArtIntake/ExamineImages`, or matching Resources folders.");
                builder.AppendLine("- Missing item icons should be placed under `Assets/ArtIntake/Items` or `Assets/Resources/Items`.");
                builder.AppendLine("- Missing symbols should be placed under `Assets/ArtIntake/Symbols` or `Assets/Resources/Symbols`.");
                builder.AppendLine("- Run `Copy Matching ArtIntake Files To Resources With Backup`, then `Apply Sprite Import Settings To Visual Resources`.");
                AppendList(builder, "Errors", errors);
                AppendList(builder, "Warnings", warnings);
                AppendList(builder, "Info", infos);
                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
                Debug.Log("[VisualResourceValidator] Report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogError("[VisualResourceValidator] Could not write report: " + exception.Message);
            }
        }

        private static void AppendResourceTable(StringBuilder builder, string title, string idLabel, string type)
        {
            builder.AppendLine("## " + title);
            builder.AppendLine();
            builder.AppendLine("| " + idLabel + " | Resources Path | Resources Found | ArtIntake Match | Status | Notes |");
            builder.AppendLine("|---|---|---:|---|---|---|");
            for (int i = 0; i < checks.Count; i++)
            {
                ResourceCheck check = checks[i];
                if (check.requirement.type != type)
                {
                    continue;
                }

                builder.AppendLine("| " + Escape(check.requirement.id) + " | " + Escape(check.requirement.resourcesPath) + " | " + (check.resourcesFound ? "Yes" : "No") + " | " + Escape(check.artIntakeAssetPath) + " | " + Escape(check.status) + " | " + Escape(check.notes) + " |");
            }
            builder.AppendLine();
        }

        private static void AppendList(StringBuilder builder, string title, List<string> values)
        {
            builder.AppendLine();
            builder.AppendLine("## " + title);
            builder.AppendLine();
            if (values.Count == 0)
            {
                builder.AppendLine("- None");
                return;
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.AppendLine("- " + values[i]);
            }
        }

        private static int CountRequiredFound()
        {
            int count = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (!checks[i].requirement.optional && checks[i].resourcesFound)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountRequiredMissing()
        {
            int count = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (!checks[i].requirement.optional && !checks[i].resourcesFound)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountOptionalFound()
        {
            int count = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (checks[i].requirement.optional && checks[i].resourcesFound)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountOptionalMissing()
        {
            int count = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (checks[i].requirement.optional && !checks[i].resourcesFound)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountArtIntakeReady()
        {
            int count = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (!checks[i].resourcesFound && checks[i].artIntakeFound)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountResourcesReady()
        {
            int count = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (checks[i].resourcesFound)
                {
                    count++;
                }
            }

            return count;
        }

        private static void AddError(string message)
        {
            errors.Add(message);
            Debug.LogError("[VisualResourceValidator] " + message);
        }

        private static void AddWarning(string message)
        {
            warnings.Add(message);
            Debug.LogWarning("[VisualResourceValidator] " + message);
        }

        private static void AddInfo(string message)
        {
            infos.Add(message);
            Debug.Log("[VisualResourceValidator] " + message);
        }

        private static void Reset()
        {
            errors.Clear();
            warnings.Clear();
            infos.Clear();
            checks.Clear();
        }

        private static string Escape(string value)
        {
            return ArtResourceCatalog.Escape(value);
        }
    }
}
