// -----------------------------------------------------------------------------
// Codex comment pass: Art Resource Binding Validator
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\ArtResourceBindingValidator.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Editor utility for the Art Resource Binding Validator workflow, exposed through menu items or called by other validation tools.
    public static class ArtResourceBindingValidator
    {
        // Editor utility for the Binding Row workflow, exposed through menu items or called by other validation tools.
        private class BindingRow
        {
            // Stores the category value used by this script's runtime or editor workflow.
            public string category;
            // Stores the target value used by this script's runtime or editor workflow.
            public string target;
            // Stores the result value used by this script's runtime or editor workflow.
            public string result;
            // Stores the notes value used by this script's runtime or editor workflow.
            public string notes;
        }

        // Stores the rows value used by this script's runtime or editor workflow.
        private static readonly List<BindingRow> rows = new List<BindingRow>();
        // Stores the errors value used by this script's runtime or editor workflow.
        private static readonly List<string> errors = new List<string>();
        // Stores the warnings value used by this script's runtime or editor workflow.
        private static readonly List<string> warnings = new List<string>();

        [MenuItem("Escape From Nightmare/Art Resources/Validate Art Resource Bindings")]
        // Checks scene, prefab, resource, or data requirements and records any issues found.
        public static void ValidateArtResourceBindings()
        {
            Reset();
            ValidateViewBackgroundBindings();
            ValidateInventoryIcons();
            ValidateClueImages();
            ValidateSymbolBindings();
            ValidateHotspotButtons();
            ValidatePanelPresets();
            WriteReport();
            Debug.Log("[ArtResourceBindingValidator] Completed. Errors: " + errors.Count + ", Warnings: " + warnings.Count);
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateViewBackgroundBindings()
        {
            ViewBackgroundBinding[] bindings = Resources.FindObjectsOfTypeAll<ViewBackgroundBinding>();
            for (int i = 0; i < bindings.Length; i++)
            {
                ViewBackgroundBinding binding = bindings[i];
                if (!IsSceneObject(binding))
                {
                    continue;
                }

                LocationView view = binding.GetComponent<LocationView>();
                string viewId = view != null ? view.ViewId : binding.gameObject.name.Replace("View_", string.Empty);
                string expectedPath = "Backgrounds/" + viewId;
                string path = binding.ResourcesPath;

                if (string.IsNullOrEmpty(path))
                {
                    AddWarning("ViewBackgroundBinding resourcesPath is empty: " + GetPath(binding.gameObject));
                    AddRow("ViewBackgroundBinding", viewId, "Warning", "resourcesPath empty");
                    continue;
                }

                if (!string.Equals(path, expectedPath, StringComparison.Ordinal))
                {
                    AddWarning("ViewBackgroundBinding path mismatch for " + viewId + ": expected " + expectedPath + ", got " + path);
                }

                bool found = Resources.Load<Sprite>(path) != null;
                if (!found)
                {
                    AddWarning("Background sprite missing: " + path);
                }

                AddRow("ViewBackgroundBinding", viewId, found ? "Found" : "Missing Sprite", "path=" + path + ", expected=" + expectedPath);
            }
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateInventoryIcons()
        {
            ItemRecordList list = LoadJson<ItemRecordList>("items.json");
            if (list != null && list.items != null)
            {
                for (int i = 0; i < list.items.Count; i++)
                {
                    ItemRecord item = list.items[i];
                    if (item == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.iconPath))
                    {
                        AddWarning("Item iconPath is empty: " + item.itemId);
                        AddRow("Item Icon", item.itemId, "Warning", "iconPath empty");
                        continue;
                    }

                    bool found = Resources.Load<Sprite>(item.iconPath) != null;
                    if (!found)
                    {
                        AddWarning("Item icon missing: " + item.iconPath);
                    }

                    AddRow("Item Icon", item.itemId, found ? "Found" : "Missing Sprite", item.iconPath);
                }
            }

            InventorySlotUI[] slots = Resources.FindObjectsOfTypeAll<InventorySlotUI>();
            int sceneSlots = 0;
            int slotsWithIcon = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (!IsSceneObject(slots[i]))
                {
                    continue;
                }

                sceneSlots++;
                UnityEngine.Object iconImage = GetObjectReference(slots[i], "iconImage");
                if (iconImage != null)
                {
                    slotsWithIcon++;
                }
            }

            if (sceneSlots == 0)
            {
                AddWarning("No InventorySlotUI objects found in the active scene.");
            }
            else if (slotsWithIcon < sceneSlots)
            {
                AddWarning("Some InventorySlotUI objects have no iconImage connection. Connected " + slotsWithIcon + "/" + sceneSlots);
            }

            AddRow("InventorySlotUI", "Scene Slots", sceneSlots > 0 ? "Checked" : "Warning", "slots=" + sceneSlots + ", iconImage connected=" + slotsWithIcon);
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateClueImages()
        {
            ClueRecordList list = LoadJson<ClueRecordList>("clues.json");
            if (list != null && list.clues != null)
            {
                for (int i = 0; i < list.clues.Count; i++)
                {
                    ClueRecord clue = list.clues[i];
                    if (clue == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(clue.imagePath))
                    {
                        AddWarning("Clue imagePath is empty: " + clue.clueId);
                        AddRow("Clue Image", clue.clueId, "Warning", "imagePath empty");
                        continue;
                    }

                    bool found = Resources.Load<Sprite>(clue.imagePath) != null;
                    if (!found)
                    {
                        AddWarning("Clue image missing: " + clue.imagePath);
                    }

                    AddRow("Clue Image", clue.clueId, found ? "Found" : "Missing Sprite", clue.imagePath);
                }
            }

            ClueImagePanelUI panel = FindSceneObject<ClueImagePanelUI>();
            if (panel == null)
            {
                AddWarning("ClueImagePanelUI is missing in the active scene.");
                AddRow("ClueImagePanelUI", "Scene", "Warning", "Panel missing");
                return;
            }

            UnityEngine.Object clueImage = GetObjectReference(panel, "clueImage");
            if (clueImage == null)
            {
                AddWarning("ClueImagePanelUI.clueImage is not connected.");
            }

            AddRow("ClueImagePanelUI", GetPath(panel.gameObject), clueImage != null ? "Connected" : "Warning", "clueImage=" + (clueImage != null ? clueImage.name : "null"));
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateSymbolBindings()
        {
            SymbolRecordList list = LoadJson<SymbolRecordList>("symbols.json");
            if (list != null && list.symbols != null)
            {
                for (int i = 0; i < list.symbols.Count; i++)
                {
                    SymbolRecord symbol = list.symbols[i];
                    if (symbol == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(symbol.spritePath))
                    {
                        AddWarning("Symbol spritePath is empty: " + symbol.symbolId);
                        AddRow("Symbol Sprite", symbol.symbolId, "Warning", "spritePath empty");
                        continue;
                    }

                    bool found = Resources.Load<Sprite>(symbol.spritePath) != null;
                    if (!found)
                    {
                        AddWarning("Symbol sprite missing: " + symbol.spritePath);
                    }

                    AddRow("Symbol Sprite", symbol.symbolId, found ? "Found" : "Missing Sprite", symbol.spritePath);
                }
            }

            PuzzleSequenceOptionButton[] options = Resources.FindObjectsOfTypeAll<PuzzleSequenceOptionButton>();
            int sceneOptions = 0;
            int optionsWithLabel = 0;
            int optionsWithIcon = 0;
            for (int i = 0; i < options.Length; i++)
            {
                if (!IsSceneObject(options[i]))
                {
                    continue;
                }

                sceneOptions++;
                if (GetObjectReference(options[i], "labelText") != null)
                {
                    optionsWithLabel++;
                }

                if (GetObjectReference(options[i], "iconImage") != null)
                {
                    optionsWithIcon++;
                }
            }

            PuzzleSymbolCycleSlot[] slots = Resources.FindObjectsOfTypeAll<PuzzleSymbolCycleSlot>();
            int sceneSlots = 0;
            int slotsWithLabel = 0;
            int slotsWithImage = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (!IsSceneObject(slots[i]))
                {
                    continue;
                }

                sceneSlots++;
                if (GetObjectReference(slots[i], "labelText") != null)
                {
                    slotsWithLabel++;
                }

                if (GetObjectReference(slots[i], "symbolImage") != null)
                {
                    slotsWithImage++;
                }
            }

            if (sceneOptions > 0 && optionsWithLabel < sceneOptions)
            {
                AddWarning("Some PuzzleSequenceOptionButton objects do not have labelText fallback.");
            }

            if (sceneSlots > 0 && slotsWithLabel < sceneSlots)
            {
                AddWarning("Some PuzzleSymbolCycleSlot objects do not have labelText fallback.");
            }

            AddRow("Symbol UI", "Sequence Options", "Checked", "options=" + sceneOptions + ", labels=" + optionsWithLabel + ", icons=" + optionsWithIcon);
            AddRow("Symbol UI", "Cycle Slots", "Checked", "slots=" + sceneSlots + ", labels=" + slotsWithLabel + ", images=" + slotsWithImage);
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidateHotspotButtons()
        {
            ClickableButton[] clickables = Resources.FindObjectsOfTypeAll<ClickableButton>();
            int sceneClickables = 0;
            int withVisual = 0;
            for (int i = 0; i < clickables.Length; i++)
            {
                ClickableButton clickable = clickables[i];
                if (!IsSceneObject(clickable))
                {
                    continue;
                }

                sceneClickables++;
                HotspotButtonVisual visual = clickable.GetComponent<HotspotButtonVisual>();
                if (visual == null)
                {
                    AddWarning("ClickableButton has no HotspotButtonVisual: " + GetPath(clickable.gameObject));
                    AddRow("HotspotButtonVisual", GetPath(clickable.gameObject), "Warning", "Missing component");
                    continue;
                }

                withVisual++;
                Image image = GetObjectReference(visual, "buttonImage") as Image;
                bool keepRaycastTarget = GetBool(visual, "keepRaycastTarget", true);
                float hiddenAlpha = GetFloat(visual, "hiddenAlpha", 0.02f);
                if (image != null && hiddenAlpha <= 0f && (!image.raycastTarget || !keepRaycastTarget))
                {
                    AddError("Hotspot can become unclickable when hidden: " + GetPath(visual.gameObject));
                }

                AddRow("HotspotButtonVisual", GetPath(visual.gameObject), "Checked", "showDebugLabel=" + visual.ShowDebugLabel + ", hiddenAlpha=" + hiddenAlpha + ", raycastTarget=" + (image != null && image.raycastTarget));
            }

            if (sceneClickables == 0)
            {
                AddError("No ClickableButton objects found in the active scene.");
            }

            AddRow("Hotspot Summary", "Scene Clickables", withVisual == sceneClickables ? "Connected" : "Warning", "withVisual=" + withVisual + "/" + sceneClickables);
        }

        // Checks scene, prefab, resource, or data requirements and records any issues found.
        private static void ValidatePanelPresets()
        {
            CheckPanelPreset<ClueImagePanelUI>("ClueImagePanel");
            CheckPanelPreset<GameOverPanelUI>("GameOverPanel");
            CheckPanelPreset<EndingPanelUI>("EndingPanel");
            CheckPanelPreset<InventoryBarUI>("InventoryBar");
            CheckPanelPreset<GhostStatusUI>("GhostStatusPanel");
        }

        private static void CheckPanelPreset<T>(string label) where T : Component
        {
            T panel = FindSceneObject<T>();
            if (panel == null)
            {
                AddWarning(label + " component is missing.");
                AddRow("PanelVisualPreset", label, "Warning", "component missing");
                return;
            }

            PanelVisualPreset preset = panel.GetComponent<PanelVisualPreset>();
            if (preset == null)
            {
                AddWarning(label + " has no PanelVisualPreset.");
            }

            AddRow("PanelVisualPreset", GetPath(panel.gameObject), preset != null ? "Connected" : "Warning", label);
        }

        private static T FindSceneObject<T>() where T : Component
        {
            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]))
                {
                    return objects[i];
                }
            }

            return null;
        }

        private static T LoadJson<T>(string fileName) where T : class
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Data", fileName);
            if (!File.Exists(path))
            {
                AddError("Missing data file: " + fileName);
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch (Exception exception)
            {
                AddError("Could not parse " + fileName + ": " + exception.Message);
                return null;
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static UnityEngine.Object GetObjectReference(UnityEngine.Object target, string fieldName)
        {
            if (target == null)
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            return property != null ? property.objectReferenceValue : null;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static bool GetBool(UnityEngine.Object target, string fieldName, bool fallback)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            return property != null ? property.boolValue : fallback;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static float GetFloat(UnityEngine.Object target, string fieldName, float fallback)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            return property != null ? property.floatValue : fallback;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static bool IsSceneObject(UnityEngine.Object obj)
        {
            Component component = obj as Component;
            if (component == null)
            {
                return false;
            }

            return component.gameObject.scene.IsValid();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetPath(GameObject go)
        {
            if (go == null)
            {
                return string.Empty;
            }

            string path = go.name;
            Transform current = go.transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        // Performs the Add Row operation while keeping its implementation details inside this script.
        private static void AddRow(string category, string target, string result, string notes)
        {
            rows.Add(new BindingRow { category = category, target = target, result = result, notes = notes });
        }

        // Records a non-blocking validation concern for follow-up review.
        private static void AddWarning(string message)
        {
            warnings.Add(message);
            Debug.LogWarning("[ArtResourceBindingValidator] " + message);
        }

        // Records a blocking validation problem for the final report and console output.
        private static void AddError(string message)
        {
            errors.Add(message);
            Debug.LogError("[ArtResourceBindingValidator] " + message);
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
        private static void WriteReport()
        {
            string path = Path.Combine(Application.dataPath, "Docs/GeneratedArtBindingValidationReport.md");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Art Binding Validation Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Errors: " + errors.Count);
                builder.AppendLine("- Warnings: " + warnings.Count);
                builder.AppendLine();
                builder.AppendLine("## Binding Checks");
                builder.AppendLine();
                builder.AppendLine("| Category | Target | Result | Notes |");
                builder.AppendLine("|---|---|---|---|");
                for (int i = 0; i < rows.Count; i++)
                {
                    BindingRow row = rows[i];
                    builder.AppendLine("| " + ArtResourceCatalog.Escape(row.category) + " | " + ArtResourceCatalog.Escape(row.target) + " | " + ArtResourceCatalog.Escape(row.result) + " | " + ArtResourceCatalog.Escape(row.notes) + " |");
                }

                AppendList(builder, "Errors", errors);
                AppendList(builder, "Warnings", warnings);
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- Missing actual Sprite assets are Warnings until art files are delivered.");
                builder.AppendLine("- Hotspots that cannot receive raycasts are Errors.");
                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
            }
            catch (Exception exception)
            {
                Debug.LogError("[ArtResourceBindingValidator] Could not write report: " + exception.Message);
            }
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

        // Provides safe default Inspector values when the component is first attached.
        private static void Reset()
        {
            rows.Clear();
            errors.Clear();
            warnings.Clear();
        }
    }
}
