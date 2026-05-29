using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public static class PlaceholderVisualPolishBuilder
    {
        private class CategoryStats
        {
            public string name;
            public int processed;
            public int added;
            public int reused;
            public int warnings;
            public int errors;
        }

        private class ViewBindingRow
        {
            public string viewId;
            public string gameObjectPath;
            public string resourcesPath;
            public bool componentAdded;
        }

        private class HotspotRow
        {
            public string type;
            public string targetId;
            public string gameObjectPath;
            public string label;
            public bool debugVisible;
        }

        private static readonly Dictionary<string, CategoryStats> stats = new Dictionary<string, CategoryStats>();
        private static readonly List<string> warnings = new List<string>();
        private static readonly List<string> errors = new List<string>();
        private static readonly List<ViewBindingRow> viewBindingRows = new List<ViewBindingRow>();
        private static readonly List<HotspotRow> hotspotRows = new List<HotspotRow>();
        private static readonly List<string> panelRows = new List<string>();

        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string ReportPath = "Assets/Docs/GeneratedPlaceholderVisualPolishReport.md";
        private static string lastBackupPath = string.Empty;
        private static bool lastSaved;

        [MenuItem("Escape From Nightmare/Visual Polish/Apply Placeholder Visual Polish")]
        public static void ApplyPlaceholderVisualPolish()
        {
            RunBuilder(false);
        }

        [MenuItem("Escape From Nightmare/Visual Polish/Apply Placeholder Visual Polish And Save With Backup")]
        public static void ApplyPlaceholderVisualPolishAndSaveWithBackup()
        {
            RunBuilder(true);
        }

        [MenuItem("Escape From Nightmare/Visual Polish/Generate Placeholder Visual Polish Report")]
        public static void GeneratePlaceholderVisualPolishReport()
        {
            ResetReportState();
            WritePolishReport();
        }

        private static void RunBuilder(bool saveWithBackup)
        {
            ResetReportState();
            EnsureResourceFolders();

            if (!OpenGameSceneIfNeeded())
            {
                WritePolishReport();
                return;
            }

            if (saveWithBackup && !BackupGameScene())
            {
                AddError("Scene backup failed. Save was aborted.");
                WritePolishReport();
                return;
            }

            Canvas canvas = FindFirstSceneObject<Canvas>();
            if (canvas == null)
            {
                AddWarning("Canvas was missing. Created a placeholder Canvas for visual polish.");
                GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
                CountAdded("Debug Overlay");
            }

            ApplyDebugOverlay(canvas);
            ApplyViewBackgroundBindings();
            ApplyHotspotVisuals();
            ApplyNavigationVisuals();
            ApplyPanelPresets();

            if (saveWithBackup)
            {
                Scene scene = SceneManager.GetActiveScene();
                lastSaved = EditorSceneManager.SaveScene(scene);
                if (!lastSaved)
                {
                    AddError("EditorSceneManager.SaveScene returned false.");
                }
            }

            WritePolishReport();
            AssetDatabase.Refresh();
            Debug.Log("Placeholder visual polish completed. Saved: " + lastSaved + ", Errors: " + errors.Count + ", Warnings: " + warnings.Count);
        }

        private static void ResetReportState()
        {
            stats.Clear();
            warnings.Clear();
            errors.Clear();
            viewBindingRows.Clear();
            hotspotRows.Clear();
            panelRows.Clear();
            lastBackupPath = string.Empty;
            lastSaved = false;
        }

        private static void EnsureResourceFolders()
        {
            string[] folders =
            {
                "Assets/Resources",
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

            for (int i = 0; i < folders.Length; i++)
            {
                EnsureFolder(folders[i]);
            }
        }

        private static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                return;
            }

            string parent = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            string name = Path.GetFileName(assetPath);
            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, name);
        }

        private static bool OpenGameSceneIfNeeded()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid() && activeScene.name == "GameScene")
            {
                return true;
            }

            if (!File.Exists(GameScenePath))
            {
                AddError("GameScene file not found: " + GameScenePath);
                return false;
            }

            EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            return true;
        }

        private static bool BackupGameScene()
        {
            try
            {
                if (!File.Exists(GameScenePath))
                {
                    AddError("Cannot back up missing GameScene: " + GameScenePath);
                    return false;
                }

                EnsureFolder("Assets/Backups");
                EnsureFolder("Assets/Backups/Scenes");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                lastBackupPath = "Assets/Backups/Scenes/GameScene.backup_before_visual_polish_" + timestamp + ".unity";
                File.Copy(GameScenePath, lastBackupPath, false);
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception exception)
            {
                AddError("Could not back up GameScene: " + exception.Message);
                return false;
            }
        }

        private static void ApplyDebugOverlay(Canvas canvas)
        {
            CategoryStats category = GetStats("Debug Overlay");
            category.processed++;
            DebugHotspotOverlay overlay = canvas != null ? canvas.GetComponent<DebugHotspotOverlay>() : null;
            if (overlay == null && canvas != null)
            {
                overlay = canvas.gameObject.AddComponent<DebugHotspotOverlay>();
                category.added++;
            }
            else if (overlay != null)
            {
                category.reused++;
            }

            if (overlay != null)
            {
                SetSerializedBool(overlay, "showOnStart", true);
            }
        }

        private static void ApplyViewBackgroundBindings()
        {
            LocationView[] views = FindSceneObjects<LocationView>();
            for (int i = 0; i < views.Length; i++)
            {
                LocationView view = views[i];
                if (view == null || string.IsNullOrEmpty(view.ViewId))
                {
                    continue;
                }

                CategoryStats category = GetStats("View Background Bindings");
                category.processed++;
                GameObject viewObject = view.gameObject;
                RectTransform rect = GetOrCreateComponent<RectTransform>(viewObject);
                SetRectStretch(rect);

                Image image = viewObject.GetComponent<Image>();
                if (image == null)
                {
                    image = viewObject.AddComponent<Image>();
                    category.added++;
                }
                else
                {
                    category.reused++;
                }
                image.raycastTarget = false;

                ViewBackgroundBinding binding = viewObject.GetComponent<ViewBackgroundBinding>();
                bool added = false;
                if (binding == null)
                {
                    binding = viewObject.AddComponent<ViewBackgroundBinding>();
                    added = true;
                    category.added++;
                }
                else
                {
                    category.reused++;
                }

                string resourcePath = "Backgrounds/" + view.ViewId;
                binding.SetResourcesPath(resourcePath);
                SetSerializedObject(binding, "targetImage", image);
                SetSerializedBool(binding, "loadOnEnable", true);
                SetSerializedBool(binding, "hideImageWhenMissing", false);
                binding.ClearSprite();

                viewBindingRows.Add(new ViewBindingRow
                {
                    viewId = view.ViewId,
                    gameObjectPath = GetHierarchyPath(viewObject),
                    resourcesPath = resourcePath,
                    componentAdded = added
                });
            }
        }

        private static void ApplyHotspotVisuals()
        {
            ClickableButton[] buttons = FindSceneObjects<ClickableButton>();
            Dictionary<string, int> indexByView = new Dictionary<string, int>();
            for (int i = 0; i < buttons.Length; i++)
            {
                ClickableButton clickable = buttons[i];
                if (clickable == null)
                {
                    continue;
                }

                CategoryStats category = GetStats("Hotspot Buttons");
                category.processed++;
                Button button = GetOrCreateComponent<Button>(clickable.gameObject);
                Image image = GetOrCreateComponent<Image>(clickable.gameObject);
                image.raycastTarget = true;
                Text label = EnsureButtonLabel(clickable.gameObject);

                HotspotButtonVisual visual = clickable.GetComponent<HotspotButtonVisual>();
                bool added = false;
                if (visual == null)
                {
                    visual = clickable.gameObject.AddComponent<HotspotButtonVisual>();
                    added = true;
                    category.added++;
                }
                else
                {
                    category.reused++;
                }

                string labelText = GetHotspotLabel(clickable);
                visual.SetDisplayLabel(labelText);
                SetSerializedObject(visual, "buttonImage", image);
                SetSerializedObject(visual, "labelText", label);
                SetSerializedObject(visual, "labelRoot", label != null ? label.gameObject : null);
                SetSerializedBool(visual, "showDebugLabel", true);
                SetSerializedFloat(visual, "visibleAlpha", 0.25f);
                SetSerializedFloat(visual, "hiddenAlpha", 0.02f);
                SetSerializedBool(visual, "keepRaycastTarget", true);
                visual.MakeDebugVisible();

                RectTransform rect = GetOrCreateComponent<RectTransform>(clickable.gameObject);
                ApplyClickableRect(clickable, rect, indexByView);

                hotspotRows.Add(new HotspotRow
                {
                    type = clickable.ClickableType.ToString(),
                    targetId = labelText,
                    gameObjectPath = GetHierarchyPath(clickable.gameObject),
                    label = labelText,
                    debugVisible = true
                });

                if (button == null)
                {
                    AddWarning("Button component could not be ensured for " + GetHierarchyPath(clickable.gameObject));
                }
            }
        }

        private static void ApplyNavigationVisuals()
        {
            NavigationButton[] navigationButtons = FindSceneObjects<NavigationButton>();
            for (int i = 0; i < navigationButtons.Length; i++)
            {
                NavigationButton navigation = navigationButtons[i];
                if (navigation == null)
                {
                    continue;
                }

                CategoryStats category = GetStats("Navigation Buttons");
                category.processed++;
                RectTransform rect = GetOrCreateComponent<RectTransform>(navigation.gameObject);
                Button button = GetOrCreateComponent<Button>(navigation.gameObject);
                Image image = GetOrCreateComponent<Image>(navigation.gameObject);
                Text label = EnsureButtonLabel(navigation.gameObject);
                if (label != null)
                {
                    label.text = navigation.ActionType == NavigationActionType.RotateLeft ? "Left" : navigation.ActionType == NavigationActionType.RotateRight ? "Right" : navigation.ActionType.ToString();
                    label.enabled = true;
                }

                image.color = new Color(0.12f, 0.12f, 0.12f, 0.65f);
                image.raycastTarget = true;
                if (navigation.ActionType == NavigationActionType.RotateLeft)
                {
                    SetRectAnchor(rect, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(48f, 48f), new Vector2(228f, 128f));
                }
                else if (navigation.ActionType == NavigationActionType.RotateRight)
                {
                    SetRectAnchor(rect, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-228f, 48f), new Vector2(-48f, 128f));
                }

                if (button != null)
                {
                    category.reused++;
                }
            }
        }

        private static void ApplyPanelPresets()
        {
            ApplyPanelPreset("ClueImagePanel", FindFirstSceneObject<ClueImagePanelUI>());
            ApplyPanelPreset("GameOverPanel", FindFirstSceneObject<GameOverPanelUI>());
            ApplyPanelPreset("EndingPanel", FindFirstSceneObject<EndingPanelUI>());
            ApplyPanelPreset("GhostStatusPanel", FindFirstSceneObject<GhostStatusUI>());
            ApplyPanelPreset("InventoryBar", FindFirstSceneObject<InventoryBarUI>());
        }

        private static void ApplyPanelPreset(string panelName, Component component)
        {
            CategoryStats category = GetStats("Panels");
            category.processed++;
            if (component == null)
            {
                AddWarning(panelName + " component was not found for PanelVisualPreset.");
                category.warnings++;
                return;
            }

            PanelVisualPreset preset = component.GetComponent<PanelVisualPreset>();
            bool added = false;
            if (preset == null)
            {
                preset = component.gameObject.AddComponent<PanelVisualPreset>();
                added = true;
                category.added++;
            }
            else
            {
                category.reused++;
            }

            Image image = component.GetComponent<Image>();
            if (image != null)
            {
                SetSerializedObject(preset, "backgroundImage", image);
            }

            SetSerializedBool(preset, "applyOnAwake", true);
            preset.ApplyPreset();
            panelRows.Add(panelName + "|" + (added ? "Yes" : "No") + "|Preset applied to " + GetHierarchyPath(component.gameObject));
        }

        private static Text EnsureButtonLabel(GameObject buttonObject)
        {
            Text label = buttonObject.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                return label;
            }

            GameObject labelObject = new GameObject("LabelText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform rect = labelObject.GetComponent<RectTransform>();
            SetRectStretch(rect);
            label = labelObject.GetComponent<Text>();
            label.font = GetDefaultFont();
            label.fontSize = 18;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            return label;
        }

        private static string GetHotspotLabel(ClickableButton clickable)
        {
            if (clickable == null)
            {
                return string.Empty;
            }

            switch (clickable.ClickableType)
            {
                case ClickableType.Door:
                    return !string.IsNullOrEmpty(clickable.LinkedDoorId) ? clickable.LinkedDoorId : clickable.ClickableId;
                case ClickableType.Puzzle:
                    return !string.IsNullOrEmpty(clickable.LinkedPuzzleId) ? clickable.LinkedPuzzleId : clickable.ClickableId;
                case ClickableType.ExamineImage:
                    return !string.IsNullOrEmpty(clickable.LinkedClueImageId) ? clickable.LinkedClueImageId : clickable.TargetObjectId;
                case ClickableType.HidePoint:
                    return !string.IsNullOrEmpty(clickable.TargetObjectId) ? clickable.TargetObjectId : clickable.ClickableId;
                case ClickableType.FinalDoor:
                    return "FinalDoor";
                default:
                    return !string.IsNullOrEmpty(clickable.ClickableId) ? clickable.ClickableId : clickable.gameObject.name;
            }
        }

        private static void ApplyClickableRect(ClickableButton clickable, RectTransform rect, Dictionary<string, int> indexByView)
        {
            if (clickable == null || rect == null)
            {
                return;
            }

            string viewId = GetParentViewId(clickable.transform);
            if (string.IsNullOrEmpty(viewId))
            {
                viewId = "Global";
            }

            int index;
            indexByView.TryGetValue(viewId, out index);
            indexByView[viewId] = index + 1;

            Vector2 size;
            if (clickable.ClickableType == ClickableType.Door)
            {
                size = new Vector2(260f, 120f);
            }
            else if (clickable.ClickableType == ClickableType.Puzzle)
            {
                size = new Vector2(240f, 90f);
            }
            else
            {
                size = new Vector2(220f, 80f);
            }

            Vector2 anchored = GetSuggestedPosition(clickable, index);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchored;
        }

        private static Vector2 GetSuggestedPosition(ClickableButton clickable, int index)
        {
            float xOffset = ((index % 3) - 1) * 280f;
            float yOffset = -220f + (index / 3) * 110f;
            if (clickable.ClickableType == ClickableType.ExamineImage)
            {
                yOffset = 180f - (index % 4) * 95f;
                xOffset = index % 2 == 0 ? -420f : 420f;
            }
            else if (clickable.ClickableType == ClickableType.HidePoint)
            {
                xOffset = -360f + (index % 3) * 360f;
                yOffset = -300f;
            }
            else if (clickable.ClickableType == ClickableType.FinalDoor)
            {
                xOffset = 0f;
                yOffset = -180f;
            }

            return new Vector2(xOffset, yOffset);
        }

        private static string GetParentViewId(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            LocationView view = transform.GetComponentInParent<LocationView>(true);
            return view != null ? view.ViewId : string.Empty;
        }

        private static void WritePolishReport()
        {
            string absolutePath = Path.Combine(Application.dataPath, "Docs/GeneratedPlaceholderVisualPolishReport.md");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Placeholder Visual Polish Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Scene: " + SceneManager.GetActiveScene().path);
                builder.AppendLine("- Saved: " + lastSaved);
                builder.AppendLine("- Backup Path: " + lastBackupPath);
                builder.AppendLine();
                builder.AppendLine("## Summary");
                builder.AppendLine();
                builder.AppendLine("| Category | Processed | Added | Reused | Warnings | Errors |");
                builder.AppendLine("|---|---:|---:|---:|---:|---:|");
                foreach (KeyValuePair<string, CategoryStats> pair in stats)
                {
                    CategoryStats stat = pair.Value;
                    builder.AppendLine("| " + stat.name + " | " + stat.processed + " | " + stat.added + " | " + stat.reused + " | " + stat.warnings + " | " + stat.errors + " |");
                }
                builder.AppendLine();
                builder.AppendLine("## View Background Bindings");
                builder.AppendLine();
                builder.AppendLine("| View ID | GameObject | Resources Path | Component Added |");
                builder.AppendLine("|---|---|---|---|");
                for (int i = 0; i < viewBindingRows.Count; i++)
                {
                    ViewBindingRow row = viewBindingRows[i];
                    builder.AppendLine("| " + row.viewId + " | " + EscapeMarkdown(row.gameObjectPath) + " | " + row.resourcesPath + " | " + (row.componentAdded ? "Yes" : "No") + " |");
                }
                builder.AppendLine();
                builder.AppendLine("## Hotspot Buttons");
                builder.AppendLine();
                builder.AppendLine("| Type | Target ID | GameObject | Label | Debug Visible |");
                builder.AppendLine("|---|---|---|---|---|");
                for (int i = 0; i < hotspotRows.Count; i++)
                {
                    HotspotRow row = hotspotRows[i];
                    builder.AppendLine("| " + row.type + " | " + EscapeMarkdown(row.targetId) + " | " + EscapeMarkdown(row.gameObjectPath) + " | " + EscapeMarkdown(row.label) + " | " + row.debugVisible + " |");
                }
                builder.AppendLine();
                builder.AppendLine("## Panels");
                builder.AppendLine();
                builder.AppendLine("| Panel | Preset Applied | Notes |");
                builder.AppendLine("|---|---:|---|");
                for (int i = 0; i < panelRows.Count; i++)
                {
                    string[] parts = panelRows[i].Split('|');
                    if (parts.Length >= 3)
                    {
                        builder.AppendLine("| " + parts[0] + " | " + parts[1] + " | " + EscapeMarkdown(parts[2]) + " |");
                    }
                }
                builder.AppendLine();
                builder.AppendLine("## Manual Polish Required");
                builder.AppendLine();
                builder.AppendLine("- Door hotspot positions");
                builder.AppendLine("- Puzzle hotspot positions");
                builder.AppendLine("- Clue hotspot positions");
                builder.AppendLine("- Actual background images");
                builder.AppendLine("- Actual icon, symbol, and clue images");
                builder.AppendLine("- Inventory and panel design");
                builder.AppendLine();
                AppendList(builder, "Warnings", warnings);
                AppendList(builder, "Errors", errors);
                File.WriteAllText(absolutePath, builder.ToString());
                AssetDatabase.Refresh();
                Debug.Log("[PlaceholderVisualPolishBuilder] Report written: " + absolutePath);
            }
            catch (Exception exception)
            {
                Debug.LogError("[PlaceholderVisualPolishBuilder] Could not write report: " + exception.Message);
            }
        }

        private static void AppendList(StringBuilder builder, string title, List<string> values)
        {
            builder.AppendLine("## " + title);
            builder.AppendLine();
            if (values.Count == 0)
            {
                builder.AppendLine("- None");
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    builder.AppendLine("- " + values[i]);
                }
            }
            builder.AppendLine();
        }

        private static T GetOrCreateComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        private static T FindFirstSceneObject<T>() where T : Component
        {
            T[] objects = FindSceneObjects<T>();
            return objects.Length > 0 ? objects[0] : null;
        }

        private static T[] FindSceneObjects<T>() where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            AddSceneObjects(UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None), result);
            AddSceneObjects(Resources.FindObjectsOfTypeAll<T>(), result);
            return result.ToArray();
        }

        private static void AddSceneObjects<T>(T[] objects, List<T> result) where T : UnityEngine.Object
        {
            if (objects == null)
            {
                return;
            }
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]) && !result.Contains(objects[i]))
                {
                    result.Add(objects[i]);
                }
            }
        }

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

        private static void SetRectStretch(RectTransform rect)
        {
            if (rect == null)
            {
                return;
            }
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetRectAnchor(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            if (rect == null)
            {
                return;
            }
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static Font GetDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            if (font == null)
            {
                AddWarning("Built-in UI font was not found.");
            }
            return font;
        }

        private static void SetSerializedObject(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                AddWarning("Serialized field not found: " + target.GetType().Name + "." + fieldName);
                return;
            }
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedBool(UnityEngine.Object target, string fieldName, bool value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                AddWarning("Serialized field not found: " + target.GetType().Name + "." + fieldName);
                return;
            }
            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSerializedFloat(UnityEngine.Object target, string fieldName, float value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                AddWarning("Serialized field not found: " + target.GetType().Name + "." + fieldName);
                return;
            }
            property.floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static CategoryStats GetStats(string name)
        {
            CategoryStats category;
            if (!stats.TryGetValue(name, out category))
            {
                category = new CategoryStats { name = name };
                stats.Add(name, category);
            }
            return category;
        }

        private static void CountAdded(string categoryName)
        {
            CategoryStats category = GetStats(categoryName);
            category.added++;
        }

        private static void AddWarning(string message)
        {
            warnings.Add(message);
            Debug.LogWarning("[PlaceholderVisualPolishBuilder] " + message);
        }

        private static void AddError(string message)
        {
            errors.Add(message);
            Debug.LogError("[PlaceholderVisualPolishBuilder] " + message);
        }

        private static string GetHierarchyPath(GameObject obj)
        {
            if (obj == null)
            {
                return string.Empty;
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

        private static string EscapeMarkdown(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
