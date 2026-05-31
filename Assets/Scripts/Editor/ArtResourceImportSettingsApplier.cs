// -----------------------------------------------------------------------------
// Codex comment pass: Art Resource Import Settings Applier
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\ArtResourceImportSettingsApplier.cs and keeps its behavior isolated to that folder's responsibility.
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
    // Editor utility for the Art Resource Import Settings Applier workflow, exposed through menu items or called by other validation tools.
    public static class ArtResourceImportSettingsApplier
    {
        // Editor utility for the Import Row workflow, exposed through menu items or called by other validation tools.
        private class ImportRow
        {
            // Stores the asset Path value used by this script's runtime or editor workflow.
            public string assetPath;
            // Stores the category value used by this script's runtime or editor workflow.
            public string category;
            // Stores the max Texture Size value used by this script's runtime or editor workflow.
            public int maxTextureSize;
            // Stores the changed value used by this script's runtime or editor workflow.
            public bool changed;
            // Stores the notes value used by this script's runtime or editor workflow.
            public string notes;
        }

        // Stores the rows value used by this script's runtime or editor workflow.
        private static readonly List<ImportRow> rows = new List<ImportRow>();
        // Stores the errors value used by this script's runtime or editor workflow.
        private static readonly List<string> errors = new List<string>();

        [MenuItem("Escape From Nightmare/Art Resources/Apply Sprite Import Settings To Visual Resources")]
        // Applies calculated settings to Unity components or runtime state.
        public static void ApplySpriteImportSettingsToVisualResources()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            List<string> assets = ArtResourceCatalog.FindVisualResourceImageAssets();

            if (assets.Count == 0)
            {
                WriteReport("No visual resource images found.");
                Debug.Log("[ArtResourceImportSettingsApplier] No visual resource images found.");
                return;
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < assets.Count; i++)
                {
                    ApplySettings(assets[i]);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();
            WriteReport(string.Empty);
            Debug.Log("[ArtResourceImportSettingsApplier] Sprite import settings applied. Assets: " + rows.Count + ", Changed: " + CountChanged() + ", Errors: " + errors.Count);
        }

        [MenuItem("Escape From Nightmare/Art Resources/Generate Art Import Settings Report")]
        // Performs the Generate Art Import Settings Report operation while keeping its implementation details inside this script.
        public static void GenerateArtImportSettingsReport()
        {
            Reset();
            ArtResourceCatalog.EnsureFolders();
            List<string> assets = ArtResourceCatalog.FindVisualResourceImageAssets();
            for (int i = 0; i < assets.Count; i++)
            {
                TextureImporter importer = AssetImporter.GetAtPath(assets[i]) as TextureImporter;
                string category = GetCategory(assets[i]);
                rows.Add(new ImportRow
                {
                    assetPath = assets[i],
                    category = category,
                    maxTextureSize = importer != null ? importer.maxTextureSize : 0,
                    changed = false,
                    notes = importer != null ? DescribeImporter(importer) : "TextureImporter missing"
                });
            }

            WriteReport(assets.Count == 0 ? "No visual resource images found." : string.Empty);
            Debug.Log("[ArtResourceImportSettingsApplier] Art import settings report generated.");
        }

        // Applies calculated settings to Unity components or runtime state.
        private static void ApplySettings(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            string category = GetCategory(assetPath);
            int maxTextureSize = GetMaxTextureSize(category);

            if (importer == null)
            {
                errors.Add("TextureImporter missing for " + assetPath);
                rows.Add(new ImportRow { assetPath = assetPath, category = category, maxTextureSize = maxTextureSize, changed = false, notes = "TextureImporter missing" });
                return;
            }

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (!importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear;
                changed = true;
            }

            if (importer.maxTextureSize != maxTextureSize)
            {
                importer.maxTextureSize = maxTextureSize;
                changed = true;
            }

            if (Math.Abs(importer.spritePixelsPerUnit - 100f) > 0.01f)
            {
                importer.spritePixelsPerUnit = 100f;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }

            rows.Add(new ImportRow
            {
                assetPath = assetPath,
                category = category,
                maxTextureSize = maxTextureSize,
                changed = changed,
                notes = DescribeImporter(importer)
            });
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetCategory(string assetPath)
        {
            string normalized = assetPath.Replace("\\", "/");
            if (normalized.Contains("/Backgrounds/"))
            {
                return "Backgrounds";
            }

            if (normalized.Contains("/ClueImages/") || normalized.Contains("/ExamineImages/"))
            {
                return "ClueImages";
            }

            if (normalized.Contains("/Items/"))
            {
                return "Items";
            }

            if (normalized.Contains("/Symbols/"))
            {
                return "Symbols";
            }

            if (normalized.Contains("/Ghost/"))
            {
                return "Ghost";
            }

            if (normalized.Contains("/UI/"))
            {
                return "UI";
            }

            return "Other";
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int GetMaxTextureSize(string category)
        {
            switch (category)
            {
                case "Backgrounds":
                    return 4096;
                case "ClueImages":
                    return 2048;
                case "Items":
                case "Symbols":
                    return 512;
                case "UI":
                    return 1024;
                case "Ghost":
                    return 2048;
                default:
                    return 2048;
            }
        }

        // Performs the Describe Importer operation while keeping its implementation details inside this script.
        private static string DescribeImporter(TextureImporter importer)
        {
            if (importer == null)
            {
                return "TextureImporter missing";
            }

            return "type=" + importer.textureType
                   + ", mode=" + importer.spriteImportMode
                   + ", mipmaps=" + importer.mipmapEnabled
                   + ", alpha=" + importer.alphaIsTransparency
                   + ", compression=" + importer.textureCompression
                   + ", filter=" + importer.filterMode
                   + ", max=" + importer.maxTextureSize
                   + ", ppu=" + importer.spritePixelsPerUnit;
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
        private static void WriteReport(string note)
        {
            string path = Path.Combine(Application.dataPath, "Docs/GeneratedArtImportSettingsReport.md");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Art Import Settings Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Processed Images: " + rows.Count);
                builder.AppendLine("- Changed TextureImporters: " + CountChanged());
                builder.AppendLine("- Errors: " + errors.Count);
                if (!string.IsNullOrEmpty(note))
                {
                    builder.AppendLine("- Note: " + note);
                }

                builder.AppendLine();
                builder.AppendLine("## Settings");
                builder.AppendLine();
                builder.AppendLine("| Asset | Category | Max Size | Changed | Notes |");
                builder.AppendLine("|---|---|---:|---:|---|");
                for (int i = 0; i < rows.Count; i++)
                {
                    ImportRow row = rows[i];
                    builder.AppendLine("| " + ArtResourceCatalog.Escape(row.assetPath) + " | " + row.category + " | " + row.maxTextureSize + " | " + (row.changed ? "Yes" : "No") + " | " + ArtResourceCatalog.Escape(row.notes) + " |");
                }

                builder.AppendLine();
                builder.AppendLine("## Errors");
                builder.AppendLine();
                if (errors.Count == 0)
                {
                    builder.AppendLine("- None");
                }
                else
                {
                    for (int i = 0; i < errors.Count; i++)
                    {
                        builder.AppendLine("- " + errors[i]);
                    }
                }

                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
            }
            catch (Exception exception)
            {
                Debug.LogError("[ArtResourceImportSettingsApplier] Could not write report: " + exception.Message);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static int CountChanged()
        {
            int count = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].changed)
                {
                    count++;
                }
            }

            return count;
        }

        // Provides safe default Inspector values when the component is first attached.
        private static void Reset()
        {
            rows.Clear();
            errors.Clear();
        }
    }
}
