// -----------------------------------------------------------------------------
// Codex comment pass: Visual Resource Validator
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\VisualResourceValidator.cs and keeps its behavior isolated to that folder's responsibility.
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
    // Editor utility for the Visual Resource Validator workflow, exposed through menu items or called by other validation tools.
    public static class VisualResourceValidator
    {
        // Editor utility for the Resource Check workflow, exposed through menu items or called by other validation tools.
        private class ResourceCheck
        {
            // Stores the requirement value used by this script's runtime or editor workflow.
            public ArtResourceRequirement requirement;
            // Stores the resources Found value used by this script's runtime or editor workflow.
            public bool resourcesFound;
            // Stores the art Intake Found value used by this script's runtime or editor workflow.
            public bool artIntakeFound;
            // Stores the resources Load Found value used by this script's runtime or editor workflow.
            public bool resourcesLoadFound;
            // Stores the dimensions Valid value used by this script's runtime or editor workflow.
            public bool dimensionsValid;
            // Stores the import Valid value used by this script's runtime or editor workflow.
            public bool importValid;
            // Stores the alpha Valid value used by this script's runtime or editor workflow.
            public bool alphaValid;
            // Stores the resources Asset Path value used by this script's runtime or editor workflow.
            public string resourcesAssetPath;
            // Stores the art Intake Asset Path value used by this script's runtime or editor workflow.
            public string artIntakeAssetPath;
            // Stores the dimensions value used by this script's runtime or editor workflow.
            public string dimensions;
            // Stores the expected Dimensions value used by this script's runtime or editor workflow.
            public string expectedDimensions;
            // Stores the import Notes value used by this script's runtime or editor workflow.
            public string importNotes;
            // Stores the duplicate Notes value used by this script's runtime or editor workflow.
            public string duplicateNotes;
            // Stores the status value used by this script's runtime or editor workflow.
            public string status;
            // Stores the notes value used by this script's runtime or editor workflow.
            public string notes;
        }

        // Stores the errors value used by this script's runtime or editor workflow.
        private static readonly List<string> errors = new List<string>();
        // Stores the warnings value used by this script's runtime or editor workflow.
        private static readonly List<string> warnings = new List<string>();
        // Stores the infos value used by this script's runtime or editor workflow.
        private static readonly List<string> infos = new List<string>();
        // Stores the checks value used by this script's runtime or editor workflow.
        private static readonly List<ResourceCheck> checks = new List<ResourceCheck>();

        // Stores the Required View Ids value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredViewIds =
        {
            "Bedroom_Front", "Bedroom_Back",
            "SecondFloorHallway_Front", "SecondFloorHallway_Back",
            "ChildRoom_Front", "ChildRoom_Back",
            "Study_Front", "Study_Back",
            "SecondFloorStairs_Front", "SecondFloorStairs_Back",
            "FirstFloorHall_Front", "FirstFloorHall_Back",
            "Entrance_Front", "Entrance_Back",
            "SmallLivingRoom_Front", "SmallLivingRoom_Back",
            "LivingRoom_Front", "LivingRoom_Back",
            "Kitchen_Front", "Kitchen_Back",
            "BasementStairs_Front", "BasementStairs_Back",
            "BasementStorage_Front", "BasementStorage_Back"
        };

        [MenuItem("Escape From Nightmare/Visual Polish/Validate Visual Resources")]
        // Checks scene, prefab, resource, or data requirements and records any issues found.
        public static void ValidateVisualResources()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            ValidateRequirements();
            WriteValidationReport();
            Debug.Log("Visual resource validation completed. Errors: " + errors.Count + ", Warnings: " + warnings.Count + ", Info: " + infos.Count);
        }

        [MenuItem("Escape From Nightmare/Visual Resources/Validate Required Image Resources")]
        // Checks scene, prefab, resource, or data requirements and records any issues found.
        public static void ValidateRequiredImageResources()
        {
            ValidateVisualResources();
        }

        [MenuItem("Escape From Nightmare/Visual Resources/Generate Image Resource Report")]
        // Performs the Generate Image Resource Report operation while keeping its implementation details inside this script.
        public static void GenerateImageResourceReport()
        {
            ValidateVisualResources();
        }

        // Performs the Generate Visual Resource Requirement Report Menu operation while keeping its implementation details inside this script.
        public static void GenerateVisualResourceRequirementReportMenu()
        {
            GenerateRequirementReport();
        }

        // Performs the Generate Requirement Report operation while keeping its implementation details inside this script.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static string[] GetRequiredViewIds()
        {
            return RequiredViewIds;
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
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
                    else
                    {
                        if (!check.dimensionsValid)
                        {
                            AddError(requirement.type + " dimensions invalid: " + requirement.id + " / " + check.dimensions + " expected " + check.expectedDimensions);
                        }

                        if (!check.importValid)
                        {
                            AddError(requirement.type + " import settings invalid: " + requirement.id + " / " + check.importNotes);
                        }

                        if (!check.alphaValid)
                        {
                            AddError(requirement.type + " alpha validation failed: " + requirement.id);
                        }
                    }

                    if (!string.IsNullOrEmpty(check.duplicateNotes))
                    {
                        AddError(requirement.type + " duplicate filename variant found: " + requirement.id + " / " + check.duplicateNotes);
                    }
                }
            }
        }

        // Performs the Check Requirement operation while keeping its implementation details inside this script.
        private static ResourceCheck CheckRequirement(ArtResourceRequirement requirement)
        {
            string resourcesAssetPath = ArtResourceCatalog.FindExistingAssetPath(requirement.resourcesPath);
            string intakeAssetPath = ArtResourceCatalog.FindMatchingIntakePath(requirement);
            bool resourcesLoadFound = !string.IsNullOrEmpty(requirement.resourcesPath) && Resources.Load<Sprite>(requirement.resourcesPath) != null;
            bool resourcesFound = !string.IsNullOrEmpty(resourcesAssetPath) && resourcesLoadFound;
            bool artIntakeFound = !string.IsNullOrEmpty(intakeAssetPath);
            string dimensions;
            string expectedDimensions;
            bool dimensionsValid = ValidateDimensions(requirement, resourcesAssetPath, out dimensions, out expectedDimensions);
            string importNotes;
            bool importValid = ValidateImport(resourcesAssetPath, out importNotes);
            bool alphaValid = ValidateAlpha(requirement, resourcesAssetPath);
            string duplicateNotes = FindDuplicateVariants(requirement);
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
                resourcesLoadFound = resourcesLoadFound,
                dimensionsValid = dimensionsValid,
                importValid = importValid,
                alphaValid = alphaValid,
                resourcesAssetPath = resourcesAssetPath,
                artIntakeAssetPath = intakeAssetPath,
                dimensions = dimensions,
                expectedDimensions = expectedDimensions,
                importNotes = importNotes,
                duplicateNotes = duplicateNotes,
                status = status,
                notes = notes
            };
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static bool ValidateDimensions(ArtResourceRequirement requirement, string assetPath, out string actual, out string expected)
        {
            actual = "Missing";
            expected = GetExpectedDimensions(requirement);
            if (string.IsNullOrEmpty(assetPath))
            {
                return requirement != null && requirement.optional;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                actual = "Texture missing";
                return false;
            }

            actual = texture.width + "x" + texture.height;
            if (requirement == null || requirement.optional)
            {
                return true;
            }

            switch (requirement.type)
            {
                case "Background":
                    return texture.width == 1920 && texture.height == 1080;
                case "Item Icon":
                case "Symbol Sprite":
                    return texture.width == 512 && texture.height == 512;
                case "Clue Image":
                    return texture.width == texture.height && texture.width == 1024;
                default:
                    return true;
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetExpectedDimensions(ArtResourceRequirement requirement)
        {
            if (requirement == null)
            {
                return "Unknown";
            }

            switch (requirement.type)
            {
                case "Background":
                    return "1920x1080";
                case "Item Icon":
                case "Symbol Sprite":
                    return "512x512";
                case "Clue Image":
                    return "1024x1024";
                default:
                    return requirement.optional ? "Optional" : "Any";
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static bool ValidateImport(string assetPath, out string notes)
        {
            notes = "Missing";
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                notes = "TextureImporter missing";
                return false;
            }

            notes = "type=" + importer.textureType
                + ", alpha=" + importer.alphaIsTransparency
                + ", compression=" + importer.textureCompression
                + ", max=" + importer.maxTextureSize;
            return importer.textureType == TextureImporterType.Sprite;
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static bool ValidateAlpha(ArtResourceRequirement requirement, string assetPath)
        {
            if (requirement == null || requirement.optional)
            {
                return true;
            }

            bool requiresAlpha = requirement.type == "Item Icon" || requirement.type == "Symbol Sprite";
            if (!requiresAlpha)
            {
                return true;
            }

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            return importer != null && importer.alphaIsTransparency;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string FindDuplicateVariants(ArtResourceRequirement requirement)
        {
            if (requirement == null || string.IsNullOrEmpty(requirement.resourcesPath))
            {
                return string.Empty;
            }

            string folder = Path.GetDirectoryName("Assets/Resources/" + requirement.resourcesPath);
            string exactName = Path.GetFileName(requirement.resourcesPath);
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return string.Empty;
            }

            string normalizedExact = NormalizeVariantName(exactName);
            List<string> variants = new List<string>();
            string[] files = Directory.GetFiles(folder);
            for (int i = 0; i < files.Length; i++)
            {
                if (!ArtResourceCatalog.IsSupportedImagePath(files[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                if (!string.Equals(fileName, exactName, StringComparison.Ordinal)
                    && string.Equals(NormalizeVariantName(fileName), normalizedExact, StringComparison.Ordinal))
                {
                    variants.Add(Path.GetFileName(files[i]));
                }
            }

            return variants.Count == 0 ? string.Empty : string.Join(", ", variants.ToArray());
        }

        // Performs the Normalize Variant Name operation while keeping its implementation details inside this script.
        private static string NormalizeVariantName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
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
                string report = builder.ToString();
                File.WriteAllText(path, report);
                string generatedReportPath = Path.Combine(Application.dataPath, "GeneratedReports/GeneratedImageResourceReport.md");
                Directory.CreateDirectory(Path.GetDirectoryName(generatedReportPath));
                File.WriteAllText(generatedReportPath, report);
                AssetDatabase.Refresh();
                Debug.Log("[VisualResourceValidator] Report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogError("[VisualResourceValidator] Could not write report: " + exception.Message);
            }
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendResourceTable(StringBuilder builder, string title, string idLabel, string type)
        {
            builder.AppendLine("## " + title);
            builder.AppendLine();
            builder.AppendLine("| " + idLabel + " | Resources Path | Load | Size | Import | Alpha | Duplicates | Status | Notes |");
            builder.AppendLine("|---|---|---:|---|---|---|---|---|---|");
            for (int i = 0; i < checks.Count; i++)
            {
                ResourceCheck check = checks[i];
                if (check.requirement.type != type)
                {
                    continue;
                }

                builder.AppendLine("| " + Escape(check.requirement.id)
                    + " | " + Escape(check.requirement.resourcesPath)
                    + " | " + (check.resourcesLoadFound ? "Yes" : "No")
                    + " | " + Escape(check.dimensions + " / " + check.expectedDimensions)
                    + " | " + Escape(check.importNotes)
                    + " | " + (check.alphaValid ? "OK" : "Check")
                    + " | " + Escape(check.duplicateNotes)
                    + " | " + Escape(check.status)
                    + " | " + Escape(check.notes) + " |");
            }

            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Queries current data or scene state and returns a value used by the caller's next branch.
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

        // Records a blocking validation problem for the final report and console output.
        private static void AddError(string message)
        {
            errors.Add(message);
            Debug.LogError("[VisualResourceValidator] " + message);
        }

        // Records a non-blocking validation concern for follow-up review.
        private static void AddWarning(string message)
        {
            warnings.Add(message);
            Debug.LogWarning("[VisualResourceValidator] " + message);
        }

        // Records contextual validation information that helps explain the current setup.
        private static void AddInfo(string message)
        {
            infos.Add(message);
            Debug.Log("[VisualResourceValidator] " + message);
        }

        // Provides safe default Inspector values when the component is first attached.
        private static void Reset()
        {
            errors.Clear();
            warnings.Clear();
            infos.Clear();
            checks.Clear();
        }

        // Performs the Escape operation while keeping its implementation details inside this script.
        private static string Escape(string value)
        {
            return ArtResourceCatalog.Escape(value);
        }
    }
}
