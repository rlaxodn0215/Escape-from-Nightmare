// -----------------------------------------------------------------------------
// Codex comment pass: Title Scene Builder
// Role: Automates Unity Editor tasks such as scene building, prefab generation, resource validation, and report writing.
// Scope: This script belongs to Editor\TitleSceneBuilder.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using EscapeFromNightmare;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmare.EditorTools
{
    // Editor utility for the Title Scene Builder workflow, exposed through menu items or called by other validation tools.
    public static class TitleSceneBuilder
    {
        // Stores the Title Scene Path value used by this script's runtime or editor workflow.
        private const string TitleScenePath = "Assets/Scenes/TitleScene.unity";
        // Stores the Game Scene Path value used by this script's runtime or editor workflow.
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        // Stores the Logo Path value used by this script's runtime or editor workflow.
        private const string LogoPath = "Assets/Resources/UI/TitleLogo.png";
        // Stores the Background Path value used by this script's runtime or editor workflow.
        private const string BackgroundPath = "Assets/Resources/Backgrounds/TitleScene_Background.png";
        // Stores the Button Normal Path value used by this script's runtime or editor workflow.
        private const string ButtonNormalPath = "Assets/Resources/UI/Buttons/TitleButton_Normal.png";
        // Stores the Button Hover Path value used by this script's runtime or editor workflow.
        private const string ButtonHoverPath = "Assets/Resources/UI/Buttons/TitleButton_Hover.png";
        // Stores the Button Pressed Path value used by this script's runtime or editor workflow.
        private const string ButtonPressedPath = "Assets/Resources/UI/Buttons/TitleButton_Pressed.png";

        [MenuItem("Escape From Nightmare/Scenes/Build Title Scene")]
        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        public static void BuildTitleScene()
        {
            ConfigureSpriteImportSettings(LogoPath);
            ConfigureSpriteImportSettings(BackgroundPath);
            ConfigureSpriteImportSettings(ButtonNormalPath, 1024);
            ConfigureSpriteImportSettings(ButtonHoverPath, 1024);
            ConfigureSpriteImportSettings(ButtonPressedPath, 1024);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TitleScene";

            CreateCamera();
            CreateMainLight();
            CreatePersistentManagers();
            CreateTitleCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, TitleScenePath);
            EnsureBuildSettings();
            AssetDatabase.Refresh();

            Debug.Log("[TitleSceneBuilder] Built TitleScene with image logo and start/quit buttons.");
        }

        // Performs the Configure Sprite Import Settings operation while keeping its implementation details inside this script.
        private static void ConfigureSpriteImportSettings(string assetPath, int maxTextureSize = 2048)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning("[TitleSceneBuilder] Texture not found: " + assetPath);
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = maxTextureSize;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static void CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.008f, 0.008f, 0.008f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            cameraObject.AddComponent<AudioListener>();
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static void CreateMainLight()
        {
            GameObject lightObject = new GameObject("Main Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            light.intensity = 0.45f;
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static void CreatePersistentManagers()
        {
            GameObject root = new GameObject("PersistentManagers");

            GameObject gameManagerObject = new GameObject("GameManager");
            gameManagerObject.transform.SetParent(root.transform, false);
            gameManagerObject.AddComponent<GameManager>();

            GameObject saveManagerObject = new GameObject("SaveManager");
            saveManagerObject.transform.SetParent(root.transform, false);
            saveManagerObject.AddComponent<SaveManager>();
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static void CreateTitleCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
            Stretch(canvasRect);

            Image background = CreateImage("Background", canvasObject.transform, new Color(0.01f, 0.01f, 0.01f, 1f));
            Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
            if (backgroundSprite != null)
            {
                background.sprite = backgroundSprite;
                background.color = Color.white;
            }

            Stretch(background.rectTransform);

            Image vignette = CreateImage("ShadowVignette", canvasObject.transform, new Color(0f, 0f, 0f, 0.28f));
            Stretch(vignette.rectTransform);
            TitleLightFlicker flicker = vignette.gameObject.AddComponent<TitleLightFlicker>();
            SetSerializedObjectReference(flicker, "targetOverlay", vignette);

            Image shadowPanel = CreateImage("TitleShadowPanel", canvasObject.transform, new Color(0f, 0f, 0f, 0.42f));
            SetAnchoredRect(shadowPanel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(1220f, 720f));

            GameObject titleMenuObject = new GameObject("TitleMenu", typeof(RectTransform));
            titleMenuObject.layer = LayerMask.NameToLayer("UI");
            titleMenuObject.transform.SetParent(canvasObject.transform, false);
            RectTransform titleMenuRect = titleMenuObject.GetComponent<RectTransform>();
            Stretch(titleMenuRect);

            TitleMenuUI titleMenu = titleMenuObject.AddComponent<TitleMenuUI>();

            Image logoImage = CreateImage("TitleLogoImage", titleMenuObject.transform, Color.white);
            Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(LogoPath);
            logoImage.sprite = logoSprite;
            logoImage.preserveAspect = true;
            logoImage.raycastTarget = false;
            SetAnchoredRect(logoImage.rectTransform, new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), Vector2.zero, new Vector2(1080f, 460f));

            GameObject buttonRoot = new GameObject("ButtonPanel", typeof(RectTransform), typeof(VerticalLayoutGroup));
            buttonRoot.layer = LayerMask.NameToLayer("UI");
            buttonRoot.transform.SetParent(titleMenuObject.transform, false);
            SetAnchoredRect(buttonRoot.GetComponent<RectTransform>(), new Vector2(0.5f, 0.24f), new Vector2(0.5f, 0.24f), Vector2.zero, new Vector2(420f, 230f));

            VerticalLayoutGroup layout = buttonRoot.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Button startButton = CreateButton("StartButton", buttonRoot.transform, "시작");
            Button quitButton = CreateButton("QuitButton", buttonRoot.transform, "종료");

            SetTitleMenuReference(titleMenu, "newGameButton", startButton);
            SetTitleMenuReference(titleMenu, "quitButton", quitButton);
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static void CreateEventSystem()
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
            InputSystemUIInputModule inputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static Image CreateImage(string name, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.layer = LayerMask.NameToLayer("UI");
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static Button CreateButton(string name, Transform parent, string label)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonObject.layer = LayerMask.NameToLayer("UI");
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(420f, 105f);

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonNormalPath);
            image.type = Image.Type.Simple;
            image.color = Color.white;

            Button button = buttonObject.GetComponent<Button>();
            SpriteState spriteState = button.spriteState;
            spriteState.highlightedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonHoverPath);
            spriteState.selectedSprite = spriteState.highlightedSprite;
            spriteState.pressedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonPressedPath);
            button.spriteState = spriteState;

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.08f, 0.08f, 0.08f, 0.45f);
            colors.colorMultiplier = 1f;
            button.colors = colors;

            LayoutElement layout = buttonObject.GetComponent<LayoutElement>();
            layout.preferredHeight = 105f;
            layout.minHeight = 105f;

            Text text = CreateText("Text", buttonObject.transform, label, 32);
            text.color = new Color(0.84f, 0.84f, 0.84f, 1f);
            text.fontStyle = FontStyle.Bold;
            Stretch(text.rectTransform);

            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.72f);
            shadow.effectDistance = new Vector2(2f, -2f);

            return button;
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private static Text CreateText(string name, Transform parent, string text, int fontSize)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.layer = LayerMask.NameToLayer("UI");
            textObject.transform.SetParent(parent, false);

            Text textComponent = textObject.GetComponent<Text>();
            textComponent.text = text;
            textComponent.font = GetDefaultFont();
            textComponent.fontSize = fontSize;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.raycastTarget = false;
            return textComponent;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private static Font GetDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private static void SetTitleMenuReference(TitleMenuUI titleMenu, string propertyName, Object value)
        {
            SetSerializedObjectReference(titleMenu, propertyName, value);
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private static void SetSerializedObjectReference(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // Performs the Stretch operation while keeping its implementation details inside this script.
        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private static void SetAnchoredRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private static void EnsureBuildSettings()
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(TitleScenePath, true),
                new EditorBuildSettingsScene(GameScenePath, true)
            };

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
