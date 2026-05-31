// -----------------------------------------------------------------------------
// Codex comment pass: Scene Wiring Report Generator
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\SceneWiringReportGenerator.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Editor utility for the Scene Wiring Report Generator workflow, exposed through menu items or called by other validation tools.
    public static class SceneWiringReportGenerator
    {
        // Stores the Report Path value used by this script's runtime or editor workflow.
        private const string ReportPath = "Assets/Docs/GeneratedSceneWiringReport.md";

        [MenuItem("Escape From Nightmare/Generate Scene Wiring Report")]
        // Performs the Generate Scene Wiring Report operation while keeping its implementation details inside this script.
        public static void GenerateSceneWiringReport()
        {
            Scene scene = SceneManager.GetActiveScene();
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("# Generated Scene Wiring Report");
            builder.AppendLine();
            builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendLine("- Active Scene: " + scene.name);
            builder.AppendLine();

            AppendManagers(builder);
            AppendLocations(builder);
            AppendClickableButtons(builder);
            AppendNavigationButtons(builder);
            AppendInventoryUI(builder);
            AppendPanels(builder);
            AppendPuzzlePrefabs(builder);
            AppendWarnings(builder);

            try
            {
                string directory = Path.GetDirectoryName(ReportPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(ReportPath, builder.ToString());
                AssetDatabase.Refresh();
                Debug.Log("[SceneWiringReportGenerator] Report generated: " + ReportPath);
            }
            catch (Exception exception)
            {
                Debug.LogError("[SceneWiringReportGenerator] Failed to write report: " + exception.Message);
            }
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendManagers(StringBuilder builder)
        {
            builder.AppendLine("## Managers");
            AppendManager<GameManager>(builder, "GameManager");
            AppendManager<SaveManager>(builder, "SaveManager");
            AppendManager<GameDataManager>(builder, "GameDataManager");
            AppendManager<LocationManager>(builder, "LocationManager");
            AppendManager<InteractionManager>(builder, "InteractionManager");
            AppendManager<InventoryManager>(builder, "InventoryManager");
            AppendManager<PuzzleManager>(builder, "PuzzleManager");
            AppendManager<EndingManager>(builder, "EndingManager");
            AppendManager<NoiseManager>(builder, "NoiseManager");
            AppendManager<GhostManager>(builder, "GhostManager");
            AppendManager<HideManager>(builder, "HideManager");
            AppendManager<ChaseManager>(builder, "ChaseManager");
            AppendManager<ClueImageManager>(builder, "ClueImageManager");
            builder.AppendLine();
        }

        private static void AppendManager<T>(StringBuilder builder, string label) where T : Component
        {
            T[] managers = FindSceneObjects<T>();
            builder.AppendLine("- " + label + ": " + managers.Length);
            for (int i = 0; i < managers.Length; i++)
            {
                builder.AppendLine("  - " + GetHierarchyPath(managers[i].gameObject));
            }
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendLocations(StringBuilder builder)
        {
            builder.AppendLine("## Locations");
            LocationController[] controllers = FindSceneObjects<LocationController>();
            for (int i = 0; i < controllers.Length; i++)
            {
                LocationController controller = controllers[i];
                builder.AppendLine("### " + NullSafe(controller.LocationId));
                builder.AppendLine("- Path: " + GetHierarchyPath(controller.gameObject));
                builder.AppendLine("- Default View: " + NullSafe(controller.DefaultViewId));
                builder.AppendLine("- JSON Location Exists: " + LocationExists(controller.LocationId));
                builder.AppendLine("- Views:");
                LocationView[] views = controller.GetComponentsInChildren<LocationView>(true);
                for (int j = 0; j < views.Length; j++)
                {
                    builder.AppendLine("  - " + NullSafe(views[j].ViewId) + " (" + GetHierarchyPath(views[j].gameObject) + ")");
                }
                builder.AppendLine();
            }
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendClickableButtons(StringBuilder builder)
        {
            builder.AppendLine("## Clickable Buttons");
            builder.AppendLine("| Path | Type | clickableId | Door | Puzzle | Clue | Item | Required Item | Target | Location | View |");
            builder.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|");

            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                ClickableButton button = buttons[i];
                LocationController location = FindParent<LocationController>(button.transform);
                LocationView view = FindParent<LocationView>(button.transform);
                builder.AppendLine("| " + EscapeCell(GetHierarchyPath(button.gameObject)) +
                    " | " + button.ClickableType +
                    " | " + EscapeCell(button.ClickableId) +
                    " | " + EscapeCell(button.LinkedDoorId) +
                    " | " + EscapeCell(button.LinkedPuzzleId) +
                    " | " + EscapeCell(button.LinkedClueImageId) +
                    " | " + EscapeCell(button.LinkedItemId) +
                    " | " + EscapeCell(button.RequiredItemId) +
                    " | " + EscapeCell(button.TargetObjectId) +
                    " | " + EscapeCell(location != null ? location.LocationId : string.Empty) +
                    " | " + EscapeCell(view != null ? view.ViewId : string.Empty) +
                    " |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendNavigationButtons(StringBuilder builder)
        {
            builder.AppendLine("## Navigation Buttons");
            builder.AppendLine("| Path | actionType | targetLocationId | targetViewId |");
            builder.AppendLine("|---|---|---|---|");

            NavigationButton[] buttons = FindSceneObjects<NavigationButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                builder.AppendLine("| " + EscapeCell(GetHierarchyPath(buttons[i].gameObject)) +
                    " | " + EscapeCell(GetEnumField(buttons[i], "actionType")) +
                    " | " + EscapeCell(GetStringField(buttons[i], "targetLocationId")) +
                    " | " + EscapeCell(GetStringField(buttons[i], "targetViewId")) +
                    " |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendInventoryUI(StringBuilder builder)
        {
            InventoryBarUI[] bars = FindSceneObjects<InventoryBarUI>();
            InventorySlotUI[] slots = FindSceneObjects<InventorySlotUI>();
            builder.AppendLine("## Inventory UI");
            builder.AppendLine("- InventoryBarUI Count: " + bars.Length);
            builder.AppendLine("- InventorySlotUI Count: " + slots.Length);
            for (int i = 0; i < slots.Length; i++)
            {
                builder.AppendLine("  - " + GetHierarchyPath(slots[i].gameObject));
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendPanels(StringBuilder builder)
        {
            builder.AppendLine("## Panels");
            AppendPanel<ClueImagePanelUI>(builder, "ClueImagePanelUI");
            AppendPanel<GameOverPanelUI>(builder, "GameOverPanelUI");
            AppendPanel<EndingPanelUI>(builder, "EndingPanelUI");
            AppendPanel<GhostStatusUI>(builder, "GhostStatusUI");
            AppendPanel<HideExitButton>(builder, "HideExitButton");
            builder.AppendLine();
        }

        private static void AppendPanel<T>(StringBuilder builder, string label) where T : Component
        {
            T[] panels = FindSceneObjects<T>();
            builder.AppendLine("- " + label + ": " + panels.Length);
            for (int i = 0; i < panels.Length; i++)
            {
                builder.AppendLine("  - " + GetHierarchyPath(panels[i].gameObject));
            }
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendPuzzlePrefabs(StringBuilder builder)
        {
            builder.AppendLine("## Puzzle Prefabs");
            builder.AppendLine("| Puzzle ID | Type | Prefab Path | Load Result | PuzzleUIBase |");
            builder.AppendLine("|---|---|---|---|---|");

            PuzzleRecordList puzzles = LoadJson<PuzzleRecordList>("puzzles.json");
            List<PuzzleRecord> puzzleList = puzzles != null && puzzles.puzzles != null ? puzzles.puzzles : new List<PuzzleRecord>();
            for (int i = 0; i < puzzleList.Count; i++)
            {
                PuzzleRecord puzzle = puzzleList[i];
                GameObject prefab = !string.IsNullOrEmpty(puzzle.prefabPath) ? Resources.Load<GameObject>(puzzle.prefabPath) : null;
                bool hasBase = prefab != null && prefab.GetComponentInChildren<PuzzleUIBase>(true) != null;
                builder.AppendLine("| " + EscapeCell(puzzle.puzzleId) +
                    " | " + EscapeCell(puzzle.type) +
                    " | " + EscapeCell(puzzle.prefabPath) +
                    " | " + (prefab != null ? "Loaded" : "Missing") +
                    " | " + (hasBase ? "Yes" : "No") +
                    " |");
            }
            builder.AppendLine();
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private static void AppendWarnings(StringBuilder builder)
        {
            builder.AppendLine("## Warnings");
            builder.AppendLine("- This report is a snapshot for manual wiring review.");
            builder.AppendLine("- Run `Escape From Nightmare / Validate Current Scene Wiring` for detailed Scene errors.");
            builder.AppendLine("- Run `Escape From Nightmare / Validate Puzzle Prefab Contracts` for detailed Prefab errors.");
        }

        private static T LoadJson<T>(string fileName) where T : class
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Data", fileName);
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch
            {
                return null;
            }
        }

        // Performs the Location Exists operation while keeping its implementation details inside this script.
        private static bool LocationExists(string locationId)
        {
            LocationRecordList locations = LoadJson<LocationRecordList>("locations.json");
            if (locations == null || locations.locations == null)
            {
                return false;
            }

            for (int i = 0; i < locations.locations.Count; i++)
            {
                if (locations.locations[i] != null && locations.locations[i].locationId == locationId)
                {
                    return true;
                }
            }

            return false;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetStringField(UnityEngine.Object target, string fieldName)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            return property != null ? property.stringValue : string.Empty;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetEnumField(UnityEngine.Object target, string fieldName)
        {
            SerializedProperty property = GetProperty(target, fieldName);
            if (property == null || property.propertyType != SerializedPropertyType.Enum || property.enumValueIndex < 0 || property.enumValueIndex >= property.enumDisplayNames.Length)
            {
                return string.Empty;
            }

            return property.enumDisplayNames[property.enumValueIndex];
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static SerializedProperty GetProperty(UnityEngine.Object target, string fieldName)
        {
            if (target == null)
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            return serializedObject.FindProperty(fieldName);
        }

        private static T[] FindSceneObjects<T>() where T : UnityEngine.Object
        {
            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            List<T> sceneObjects = new List<T>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]))
                {
                    sceneObjects.Add(objects[i]);
                }
            }

            return sceneObjects.ToArray();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static bool IsSceneObject(UnityEngine.Object obj)
        {
            if (obj == null || EditorUtility.IsPersistent(obj))
            {
                return false;
            }

            Component component = obj as Component;
            if (component != null)
            {
                return component.gameObject.scene.IsValid();
            }

            GameObject gameObject = obj as GameObject;
            return gameObject != null && gameObject.scene.IsValid();
        }

        private static T FindParent<T>(Transform transform) where T : Component
        {
            Transform current = transform;
            while (current != null)
            {
                T component = current.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }

                current = current.parent;
            }

            return null;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static string GetHierarchyPath(GameObject obj)
        {
            if (obj == null)
            {
                return "(null)";
            }

            string path = obj.name;
            Transform current = obj.transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        // Performs the Escape Cell operation while keeping its implementation details inside this script.
        private static string EscapeCell(string value)
        {
            return NullSafe(value).Replace("|", "\\|");
        }

        // Performs the Null Safe operation while keeping its implementation details inside this script.
        private static string NullSafe(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value;
        }
    }
}
