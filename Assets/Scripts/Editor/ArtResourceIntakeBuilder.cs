// -----------------------------------------------------------------------------
// Codex comment pass: Art Resource Requirement
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\ArtResourceIntakeBuilder.cs and keeps its behavior isolated to that folder's responsibility.
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
    // Editor utility for the Art Resource Requirement workflow, exposed through menu items or called by other validation tools.
    internal class ArtResourceRequirement
    {
        // Stores the type value used by this script's runtime or editor workflow.
        public string type;
        // Stores the id value used by this script's runtime or editor workflow.
        public string id;
        // Stores the resources Path value used by this script's runtime or editor workflow.
        public string resourcesPath;
        // Stores the intake Folder value used by this script's runtime or editor workflow.
        public string intakeFolder;
        // Stores the optional value used by this script's runtime or editor workflow.
        public bool optional;
    }

    // Editor utility for the Art Resource Catalog workflow, exposed through menu items or called by other validation tools.
    internal static class ArtResourceCatalog
    {
        // Stores the Art Intake Root value used by this script's runtime or editor workflow.
        public const string ArtIntakeRoot = "Assets/ArtIntake";
        // Stores the Resources Root value used by this script's runtime or editor workflow.
        public const string ResourcesRoot = "Assets/Resources";

        // Stores the Supported Extensions value used by this script's runtime or editor workflow.
        private static readonly string[] SupportedExtensions =
        {
            ".png", ".jpg", ".jpeg", ".psd", ".tga"
        };

        // Stores the Required Clue Ids value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredClueIds =
        {
            "BedroomPhotoCodeClue",
            "LivingRoomEntranceCodeClue",
            "ChildRoomCardSymbolClueImage",
            "StudyBookSymbolClueImage",
            "KitchenCodeClueImage",
            "KitchenFridgeSurfaceSymbolClue",
            "BasementPowerPatternClue",
            "BasementClueImage"
        };

        // Stores the Required Item Ids value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredItemIds =
        {
            "OldDrawerKey",
            "SmallClockworkDevice",
            "ModifiedClockworkDevice",
            "BasementFuse",
            "FrontDoorKey"
        };

        // Stores the Required Symbol Ids value used by this script's runtime or editor workflow.
        private static readonly string[] RequiredSymbolIds =
        {
            "Symbol_01",
            "Symbol_02",
            "Symbol_03",
            "Symbol_04",
            "Symbol_05",
            "Symbol_06"
        };

        // Stores the Optional Resource Paths value used by this script's runtime or editor workflow.
        private static readonly string[] OptionalResourcePaths =
        {
            "UI/Buttons/DefaultButton",
            "UI/Panels/DefaultPanel",
            "Ghost/Ghost_Default"
        };

        public static string[] Extensions
        {
            get { return SupportedExtensions; }
        }

        public static string[] IntakeFolders
        {
            get
            {
                return new string[]
                {
                    "Assets/ArtIntake",
                    "Assets/ArtIntake/Backgrounds",
                    "Assets/ArtIntake/ExamineImages",
                    "Assets/ArtIntake/ClueImages",
                    "Assets/ArtIntake/Items",
                    "Assets/ArtIntake/Symbols",
                    "Assets/ArtIntake/Ghost",
                    "Assets/ArtIntake/UI",
                    "Assets/ArtIntake/UI/Panels",
                    "Assets/ArtIntake/UI/Buttons"
                };
            }
        }

        public static string[] ResourceFolders
        {
            get
            {
                return new string[]
                {
                    "Assets/Resources/Backgrounds",
                    "Assets/Resources/ExamineImages",
                    "Assets/Resources/ClueImages",
                    "Assets/Resources/Items",
                    "Assets/Resources/Symbols",
                    "Assets/Resources/Ghost",
                    "Assets/Resources/UI",
                    "Assets/Resources/UI/Panels",
                    "Assets/Resources/UI/Buttons"
                };
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static List<ArtResourceRequirement> GetRequirements()
        {
            List<ArtResourceRequirement> requirements = new List<ArtResourceRequirement>();

            string[] viewIds = VisualResourceValidator.GetRequiredViewIds();
            for (int i = 0; i < viewIds.Length; i++)
            {
                requirements.Add(Create("Background", viewIds[i], "Backgrounds/" + viewIds[i], false));
            }

            Dictionary<string, string> cluePaths = LoadCluePaths();
            for (int i = 0; i < RequiredClueIds.Length; i++)
            {
                string id = RequiredClueIds[i];
                string path = cluePaths.ContainsKey(id) ? cluePaths[id] : "ClueImages/" + id;
                requirements.Add(Create("Clue Image", id, path, false));
            }

            Dictionary<string, string> itemPaths = LoadItemPaths();
            for (int i = 0; i < RequiredItemIds.Length; i++)
            {
                string id = RequiredItemIds[i];
                string path = itemPaths.ContainsKey(id) ? itemPaths[id] : "Items/" + id;
                requirements.Add(Create("Item Icon", id, path, false));
            }

            Dictionary<string, string> symbolPaths = LoadSymbolPaths();
            for (int i = 0; i < RequiredSymbolIds.Length; i++)
            {
                string id = RequiredSymbolIds[i];
                string path = symbolPaths.ContainsKey(id) ? symbolPaths[id] : "Symbols/" + id;
                requirements.Add(Create("Symbol Sprite", id, path, false));
            }

            for (int i = 0; i < OptionalResourcePaths.Length; i++)
            {
                requirements.Add(Create("Optional", OptionalResourcePaths[i], OptionalResourcePaths[i], true));
            }

            return requirements;
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        public static void EnsureFolders()
        {
            for (int i = 0; i < IntakeFolders.Length; i++)
            {
                EnsureFolder(IntakeFolders[i]);
            }

            for (int i = 0; i < ResourceFolders.Length; i++)
            {
                EnsureFolder(ResourceFolders[i]);
            }
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        public static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                return;
            }

            string normalized = assetPath.Replace("\\", "/").Trim('/');
            string[] parts = normalized.Split('/');
            if (parts.Length == 0 || parts[0] != "Assets")
            {
                return;
            }

            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static string FindExistingAssetPath(string resourcesPath)
        {
            if (string.IsNullOrEmpty(resourcesPath))
            {
                return string.Empty;
            }

            string basePath = ResourcesRoot + "/" + resourcesPath;
            for (int i = 0; i < SupportedExtensions.Length; i++)
            {
                string candidate = basePath + SupportedExtensions[i];
                if (File.Exists(candidate))
                {
                    return candidate.Replace("\\", "/");
                }
            }

            return string.Empty;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static string FindMatchingIntakePath(ArtResourceRequirement requirement)
        {
            if (requirement == null || string.IsNullOrEmpty(requirement.resourcesPath))
            {
                return string.Empty;
            }

            string fileName = Path.GetFileName(requirement.resourcesPath);
            string folder = ArtIntakeRoot + "/" + requirement.intakeFolder;
            for (int i = 0; i < SupportedExtensions.Length; i++)
            {
                string candidate = folder + "/" + fileName + SupportedExtensions[i];
                if (File.Exists(candidate))
                {
                    return candidate.Replace("\\", "/");
                }
            }

            return string.Empty;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static string GetTargetAssetPath(ArtResourceRequirement requirement, string sourcePath)
        {
            if (requirement == null || string.IsNullOrEmpty(requirement.resourcesPath))
            {
                return string.Empty;
            }

            string extension = !string.IsNullOrEmpty(sourcePath) ? Path.GetExtension(sourcePath) : ".png";
            return (ResourcesRoot + "/" + requirement.resourcesPath + extension).Replace("\\", "/");
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static bool IsSupportedImagePath(string assetPath)
        {
            string extension = Path.GetExtension(assetPath);
            for (int i = 0; i < SupportedExtensions.Length; i++)
            {
                if (string.Equals(extension, SupportedExtensions[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public static List<string> FindVisualResourceImageAssets()
        {
            List<string> assets = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", ResourceFolders);
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (IsSupportedImagePath(assetPath))
                {
                    assets.Add(assetPath);
                }
            }

            assets.Sort(StringComparer.OrdinalIgnoreCase);
            return assets;
        }

        // Performs the Escape operation while keeping its implementation details inside this script.
        public static string Escape(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static ArtResourceRequirement Create(string type, string id, string resourcesPath, bool optional)
        {
            return new ArtResourceRequirement
            {
                type = type,
                id = id,
                resourcesPath = resourcesPath,
                intakeFolder = GetIntakeFolder(resourcesPath),
                optional = optional
            };
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetIntakeFolder(string resourcesPath)
        {
            if (string.IsNullOrEmpty(resourcesPath))
            {
                return string.Empty;
            }

            string normalized = resourcesPath.Replace("\\", "/");
            if (normalized.StartsWith("Backgrounds/", StringComparison.OrdinalIgnoreCase))
            {
                return "Backgrounds";
            }

            if (normalized.StartsWith("ExamineImages/", StringComparison.OrdinalIgnoreCase))
            {
                return "ExamineImages";
            }

            if (normalized.StartsWith("ClueImages/", StringComparison.OrdinalIgnoreCase))
            {
                return "ClueImages";
            }

            if (normalized.StartsWith("Items/", StringComparison.OrdinalIgnoreCase))
            {
                return "Items";
            }

            if (normalized.StartsWith("Symbols/", StringComparison.OrdinalIgnoreCase))
            {
                return "Symbols";
            }

            if (normalized.StartsWith("Ghost/", StringComparison.OrdinalIgnoreCase))
            {
                return "Ghost";
            }

            if (normalized.StartsWith("UI/Panels/", StringComparison.OrdinalIgnoreCase))
            {
                return "UI/Panels";
            }

            if (normalized.StartsWith("UI/Buttons/", StringComparison.OrdinalIgnoreCase))
            {
                return "UI/Buttons";
            }

            if (normalized.StartsWith("UI/", StringComparison.OrdinalIgnoreCase))
            {
                return "UI";
            }

            return string.Empty;
        }

        private static Dictionary<string, string> LoadCluePaths()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            ClueRecordList list = LoadJson<ClueRecordList>("clues.json");
            if (list == null || list.clues == null)
            {
                return paths;
            }

            for (int i = 0; i < list.clues.Count; i++)
            {
                ClueRecord record = list.clues[i];
                if (record != null && !string.IsNullOrEmpty(record.clueId))
                {
                    paths[record.clueId] = record.imagePath;
                }
            }

            return paths;
        }

        private static Dictionary<string, string> LoadItemPaths()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            ItemRecordList list = LoadJson<ItemRecordList>("items.json");
            if (list == null || list.items == null)
            {
                return paths;
            }

            for (int i = 0; i < list.items.Count; i++)
            {
                ItemRecord record = list.items[i];
                if (record != null && !string.IsNullOrEmpty(record.itemId))
                {
                    paths[record.itemId] = record.iconPath;
                }
            }

            return paths;
        }

        private static Dictionary<string, string> LoadSymbolPaths()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            SymbolRecordList list = LoadJson<SymbolRecordList>("symbols.json");
            if (list == null || list.symbols == null)
            {
                return paths;
            }

            for (int i = 0; i < list.symbols.Count; i++)
            {
                SymbolRecord record = list.symbols[i];
                if (record != null && !string.IsNullOrEmpty(record.symbolId))
                {
                    paths[record.symbolId] = record.spritePath;
                }
            }

            return paths;
        }

        private static T LoadJson<T>(string fileName) where T : class
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Data", fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning("[ArtResourceCatalog] Missing data file: " + fileName);
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[ArtResourceCatalog] Could not parse " + fileName + ": " + exception.Message);
                return null;
            }
        }
    }

    // Editor utility for the Art Resource Intake Builder workflow, exposed through menu items or called by other validation tools.
    public static class ArtResourceIntakeBuilder
    {
        // Editor utility for the Intake Row workflow, exposed through menu items or called by other validation tools.
        private class IntakeRow
        {
            // Stores the requirement value used by this script's runtime or editor workflow.
            public ArtResourceRequirement requirement;
            // Stores the resources Asset Path value used by this script's runtime or editor workflow.
            public string resourcesAssetPath;
            // Stores the intake Asset Path value used by this script's runtime or editor workflow.
            public string intakeAssetPath;
            // Stores the ready To Copy value used by this script's runtime or editor workflow.
            public bool readyToCopy;
        }

        // Stores the copied value used by this script's runtime or editor workflow.
        private static readonly List<string> copied = new List<string>();
        // Stores the backups value used by this script's runtime or editor workflow.
        private static readonly List<string> backups = new List<string>();
        // Stores the warnings value used by this script's runtime or editor workflow.
        private static readonly List<string> warnings = new List<string>();
        // Stores the errors value used by this script's runtime or editor workflow.
        private static readonly List<string> errors = new List<string>();

        [MenuItem("Escape From Nightmare/Art Resources/Prepare Art Intake Folders")]
        // Performs the Prepare Art Intake Folders operation while keeping its implementation details inside this script.
        public static void PrepareArtIntakeFolders()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            WriteIntakeGuideSeed();
            AssetDatabase.Refresh();
            GenerateArtResourceIntakeReport();
            Debug.Log("[ArtResourceIntakeBuilder] ArtIntake and Resources folders are ready.");
        }

        [MenuItem("Escape From Nightmare/Art Resources/Generate Art Resource Intake Report")]
        // Performs the Generate Art Resource Intake Report operation while keeping its implementation details inside this script.
        public static void GenerateArtResourceIntakeReport()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            List<IntakeRow> rows = BuildRows();
            WriteReport(rows, false, string.Empty);
            Debug.Log("[ArtResourceIntakeBuilder] Art resource intake report generated. Ready to copy: " + CountReady(rows));
        }

        [MenuItem("Escape From Nightmare/Art Resources/Copy Matching ArtIntake Files To Resources With Backup")]
        // Performs the Copy Matching Art Intake Files To Resources With Backup operation while keeping its implementation details inside this script.
        public static void CopyMatchingArtIntakeFilesToResourcesWithBackup()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            List<IntakeRow> rows = BuildRows();
            string backupFolder = "Assets/Backups/ArtResources/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            int copyCount = 0;

            for (int i = 0; i < rows.Count; i++)
            {
                IntakeRow row = rows[i];
                if (row == null || !row.readyToCopy)
                {
                    continue;
                }

                string targetPath = ArtResourceCatalog.GetTargetAssetPath(row.requirement, row.intakeAssetPath);
                string targetFolder = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetFolder))
                {
                    ArtResourceCatalog.EnsureFolder(targetFolder.Replace("\\", "/"));
                }

                try
                {
                    if (File.Exists(targetPath))
                    {
                        ArtResourceCatalog.EnsureFolder(backupFolder);
                        string backupPath = backupFolder + "/" + Path.GetFileName(targetPath);
                        File.Copy(targetPath, backupPath, false);
                        backups.Add(targetPath + " -> " + backupPath);
                    }

                    File.Copy(row.intakeAssetPath, targetPath, true);
                    copied.Add(row.intakeAssetPath + " -> " + targetPath);
                    copyCount++;
                }
                catch (Exception exception)
                {
                    AddError("Could not copy " + row.intakeAssetPath + " to " + targetPath + ": " + exception.Message);
                }
            }

            AssetDatabase.Refresh();
            rows = BuildRows();
            WriteReport(rows, true, backupFolder);
            Debug.Log("[ArtResourceIntakeBuilder] Copy completed. Copied: " + copyCount + ", Errors: " + errors.Count);
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static List<IntakeRow> BuildRows()
        {
            List<ArtResourceRequirement> requirements = ArtResourceCatalog.GetRequirements();
            List<IntakeRow> rows = new List<IntakeRow>();
            for (int i = 0; i < requirements.Count; i++)
            {
                ArtResourceRequirement requirement = requirements[i];
                string resourcesAssetPath = ArtResourceCatalog.FindExistingAssetPath(requirement.resourcesPath);
                string intakeAssetPath = ArtResourceCatalog.FindMatchingIntakePath(requirement);
                rows.Add(new IntakeRow
                {
                    requirement = requirement,
                    resourcesAssetPath = resourcesAssetPath,
                    intakeAssetPath = intakeAssetPath,
                    readyToCopy = string.IsNullOrEmpty(resourcesAssetPath) && !string.IsNullOrEmpty(intakeAssetPath)
                });
            }

            return rows;
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
        private static void WriteReport(List<IntakeRow> rows, bool copyAttempted, string backupFolder)
        {
            string path = Path.Combine(Application.dataPath, "Docs/GeneratedArtResourceIntakeReport.md");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                int requiredTotal = CountRequired(rows);
                int resourcesFound = CountResourcesFound(rows, false);
                int artIntakeReady = CountReady(rows);
                int missing = CountMissing(rows, false);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Art Resource Intake Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Copy Attempted: " + (copyAttempted ? "Yes" : "No"));
                builder.AppendLine("- Backup Folder: " + (string.IsNullOrEmpty(backupFolder) ? "None" : backupFolder));
                builder.AppendLine("- Required Total: " + requiredTotal);
                builder.AppendLine("- Resources Found: " + resourcesFound);
                builder.AppendLine("- ArtIntake Ready: " + artIntakeReady);
                builder.AppendLine("- Missing: " + missing);
                builder.AppendLine("- Optional Found: " + CountResourcesFound(rows, true));
                builder.AppendLine();
                builder.AppendLine("## Intake Matches");
                builder.AppendLine();
                builder.AppendLine("| Type | ID | Resources Path | Resources Asset | ArtIntake Match | Status |");
                builder.AppendLine("|---|---|---|---|---|---|");
                for (int i = 0; i < rows.Count; i++)
                {
                    IntakeRow row = rows[i];
                    string status = GetStatus(row);
                    builder.AppendLine("| " + ArtResourceCatalog.Escape(row.requirement.type) + " | " + ArtResourceCatalog.Escape(row.requirement.id) + " | " + ArtResourceCatalog.Escape(row.requirement.resourcesPath) + " | " + ArtResourceCatalog.Escape(row.resourcesAssetPath) + " | " + ArtResourceCatalog.Escape(row.intakeAssetPath) + " | " + status + " |");
                }

                AppendList(builder, "Copied Files", copied);
                AppendList(builder, "Backups", backups);
                AppendList(builder, "Warnings", warnings);
                AppendList(builder, "Errors", errors);
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- Files are matched by the final Resources path file name.");
                builder.AppendLine("- Supported extensions: `.png`, `.jpg`, `.jpeg`, `.psd`, `.tga`.");
                builder.AppendLine("- Import settings are handled by `Apply Sprite Import Settings To Visual Resources`.");
                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
            }
            catch (Exception exception)
            {
                Debug.LogError("[ArtResourceIntakeBuilder] Could not write report: " + exception.Message);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetStatus(IntakeRow row)
        {
            if (row == null || row.requirement == null)
            {
                return "Invalid";
            }

            if (!string.IsNullOrEmpty(row.resourcesAssetPath))
            {
                return "Resources Ready";
            }

            if (!string.IsNullOrEmpty(row.intakeAssetPath))
            {
                return "Ready to copy";
            }

            return row.requirement.optional ? "Optional Missing" : "Missing";
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int CountRequired(List<IntakeRow> rows)
        {
            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].requirement != null && !rows[i].requirement.optional)
                {
                    count++;
                }
            }

            return count;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int CountResourcesFound(List<IntakeRow> rows, bool optional)
        {
            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].requirement != null && rows[i].requirement.optional == optional && !string.IsNullOrEmpty(rows[i].resourcesAssetPath))
                {
                    count++;
                }
            }

            return count;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int CountReady(List<IntakeRow> rows)
        {
            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].readyToCopy)
                {
                    count++;
                }
            }

            return count;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int CountMissing(List<IntakeRow> rows, bool optional)
        {
            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].requirement != null && rows[i].requirement.optional == optional && string.IsNullOrEmpty(rows[i].resourcesAssetPath) && string.IsNullOrEmpty(rows[i].intakeAssetPath))
                {
                    count++;
                }
            }

            return count;
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

        // Performs the Write Intake Guide Seed operation while keeping its implementation details inside this script.
        private static void WriteIntakeGuideSeed()
        {
            string docsFolder = Path.Combine(Application.dataPath, "Docs");
            Directory.CreateDirectory(docsFolder);
        }

        // Provides safe default Inspector values when the component is first attached.
        private static void Reset()
        {
            copied.Clear();
            backups.Clear();
            warnings.Clear();
            errors.Clear();
        }

        // Records a blocking validation problem for the final report and console output.
        private static void AddError(string message)
        {
            errors.Add(message);
            Debug.LogError("[ArtResourceIntakeBuilder] " + message);
        }
    }
}
