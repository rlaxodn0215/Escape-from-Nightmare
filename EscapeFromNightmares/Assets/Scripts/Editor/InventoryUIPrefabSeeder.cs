using EscapeFromNightmares.Systems;
using EscapeFromNightmares.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class InventoryUIPrefabSeeder
    {
        private const string StageScenePath = "Assets/Scenes/Stage1.unity";
        private const string PrefabPath = "Assets/Prefabs/UI/InventoryUI.prefab";
        private const string PanelSpritePath = "Assets/Sprites/UI/ui_inventory_panel.png";
        private const string SlotSpritePath = "Assets/Sprites/UI/ui_inventory_slot.png";
        private const string SelectedFrameSpritePath = "Assets/Sprites/UI/ui_selected_item_frame.png";

        [MenuItem("Escape From Nightmares/Seed Inventory UI Prefab")]
        public static void Seed()
        {
            GameObject prefabRoot = BuildInventoryUI();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Object.DestroyImmediate(prefabRoot);

            Scene scene = EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
            GameObject canvas = GameObject.Find("UICanvas");
            GameObject systems = GameObject.Find("Systems");
            if (canvas == null || systems == null)
            {
                Debug.LogError("Stage1 scene is missing UICanvas or Systems.");
                return;
            }

            InventorySystem inventorySystem = systems.GetComponent<InventorySystem>();
            GameObject existing = GameObject.Find("UICanvas/InventoryUI");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instance.name = "InventoryUI";
            ConfigureSceneInstance(instance, inventorySystem);
            instance.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Seeded InventoryUI prefab and Stage1 scene instance.");
        }

        private static GameObject BuildInventoryUI()
        {
            Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
            Sprite slotSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SlotSpritePath);
            Sprite selectedFrameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SelectedFrameSpritePath);

            GameObject root = CreateRectObject("InventoryUI", null);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            InventoryUI inventoryUI = root.AddComponent<InventoryUI>();
            Image panelImage = root.AddComponent<Image>();
            panelImage.sprite = panelSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(720f, 220f);
            rootRect.anchoredPosition = new Vector2(0f, -205f);

            Button[] slotButtons = new Button[10];
            Image[] slotIcons = new Image[10];
            Image[] selectionFrames = new Image[10];

            for (int index = 0; index < slotButtons.Length; index++)
            {
                GameObject slot = CreateRectObject($"Slot{index + 1:00}", root.transform);
                RectTransform slotRect = slot.GetComponent<RectTransform>();
                slotRect.anchorMin = new Vector2(0f, 0.5f);
                slotRect.anchorMax = new Vector2(0f, 0.5f);
                slotRect.pivot = new Vector2(0.5f, 0.5f);
                slotRect.sizeDelta = new Vector2(78f, 78f);
                slotRect.anchoredPosition = new Vector2(62f + index * 66f, -16f);

                Image slotImage = slot.AddComponent<Image>();
                slotImage.sprite = slotSprite;
                slotImage.type = Image.Type.Sliced;
                slotImage.color = Color.white;

                Button button = slot.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                slotButtons[index] = button;

                GameObject icon = CreateRectObject("Icon", slot.transform);
                RectTransform iconRect = icon.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.sizeDelta = new Vector2(48f, 48f);
                Image iconImage = icon.AddComponent<Image>();
                iconImage.color = new Color(1f, 1f, 1f, 0f);
                iconImage.raycastTarget = false;
                iconImage.enabled = false;
                slotIcons[index] = iconImage;

                GameObject frame = CreateRectObject("SelectedFrame", slot.transform);
                RectTransform frameRect = frame.GetComponent<RectTransform>();
                frameRect.anchorMin = Vector2.zero;
                frameRect.anchorMax = Vector2.one;
                frameRect.offsetMin = new Vector2(-4f, -4f);
                frameRect.offsetMax = new Vector2(4f, 4f);
                Image frameImage = frame.AddComponent<Image>();
                frameImage.sprite = selectedFrameSprite;
                frameImage.type = Image.Type.Sliced;
                frameImage.raycastTarget = false;
                frameImage.enabled = false;
                selectionFrames[index] = frameImage;
            }

            TMP_Text selectedLabel = CreateLabel("SelectedItemLabel", root.transform, new Vector2(0f, 70f), new Vector2(620f, 32f), 20f);
            Button closeButton = CreateCloseButton(root.transform);

            SerializedObject serialized = new SerializedObject(inventoryUI);
            serialized.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            serialized.FindProperty("closeButton").objectReferenceValue = closeButton;
            SetArray(serialized.FindProperty("slotButtons"), slotButtons);
            SetArray(serialized.FindProperty("slotIcons"), slotIcons);
            SetArray(serialized.FindProperty("selectionFrames"), selectionFrames);
            serialized.FindProperty("selectedItemLabel").objectReferenceValue = selectedLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        private static void ConfigureSceneInstance(GameObject instance, InventorySystem inventorySystem)
        {
            InventoryUI inventoryUI = instance.GetComponent<InventoryUI>();
            if (inventoryUI == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(inventoryUI);
            serialized.FindProperty("inventorySystem").objectReferenceValue = inventorySystem;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(inventoryUI);
        }

        private static GameObject CreateRectObject(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            if (parent != null)
            {
                gameObject.transform.SetParent(parent, false);
            }

            return gameObject;
        }

        private static TMP_Text CreateLabel(string name, Transform parent, Vector2 position, Vector2 size, float fontSize)
        {
            GameObject label = CreateRectObject(name, parent);
            RectTransform rect = label.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI text = label.AddComponent<TextMeshProUGUI>();
            text.text = "";
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.82f, 0.82f, 0.82f, 1f);
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateCloseButton(Transform parent)
        {
            GameObject close = CreateRectObject("CloseButton", parent);
            RectTransform rect = close.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(-30f, -30f);
            rect.sizeDelta = new Vector2(36f, 36f);

            Image image = close.AddComponent<Image>();
            image.color = new Color(0.04f, 0.04f, 0.04f, 0.72f);

            Button button = close.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            TMP_Text label = CreateLabel("Label", close.transform, Vector2.zero, new Vector2(30f, 30f), 22f);
            label.text = "X";
            label.color = new Color(0.78f, 0.78f, 0.78f, 1f);
            return button;
        }

        private static void SetArray<T>(SerializedProperty property, T[] values) where T : Object
        {
            property.arraySize = values.Length;
            for (int index = 0; index < values.Length; index++)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }
    }
}
