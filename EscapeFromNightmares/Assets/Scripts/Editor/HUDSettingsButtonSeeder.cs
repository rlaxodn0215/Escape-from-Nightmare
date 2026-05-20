using EscapeFromNightmares.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class HUDSettingsButtonSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/HUDSettingsButton.prefab";
        private const string ButtonSpritePath = "Assets/Sprites/UI/ui_button_settings_small.png";

        [MenuItem("Escape From Nightmares/Seed HUD Settings Button")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildButton();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            GameObject settingsObject = FindChild(canvas, "SettingsUI");
            if (canvas == null || settingsObject == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas or UICanvas/SettingsUI.");
                return;
            }

            GameObject existing = FindChild(canvas, "HUDSettingsButton");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "HUDSettingsButton";
            ConfigureSceneInstance(instance, settingsObject.GetComponent<SettingsUI>());

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded HUD settings button prefab and Stage1 scene instance.");
        }

        private static GameObject BuildButton()
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonSpritePath);
            GameObject root = new GameObject("HUDSettingsButton", typeof(RectTransform), typeof(CanvasRenderer));

            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(226f, -58f);
            rect.sizeDelta = new Vector2(72f, 72f);

            Image image = root.AddComponent<Image>();
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;

            Button button = root.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            HUDSettingsButton hudButton = root.AddComponent<HUDSettingsButton>();
            SerializedObject serialized = new SerializedObject(hudButton);
            serialized.FindProperty("button").objectReferenceValue = button;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static void ConfigureSceneInstance(GameObject instance, SettingsUI settingsUI)
        {
            HUDSettingsButton hudButton = instance.GetComponent<HUDSettingsButton>();
            if (hudButton == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(hudButton);
            serialized.FindProperty("settingsUI").objectReferenceValue = settingsUI;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(hudButton);
        }

        private static GameObject FindChild(GameObject parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            Transform[] children = parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child != parent.transform && child.name == childName)
                {
                    return child.gameObject;
                }
            }

            return null;
        }
    }
}
