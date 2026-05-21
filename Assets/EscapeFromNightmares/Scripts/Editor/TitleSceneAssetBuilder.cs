using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using EscapeFromNightmares.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    public static class TitleSceneAssetBuilder
    {
        private const string Root = "Assets/EscapeFromNightmares";
        private const string CatalogPath = Root + "/ScriptableObjects/ResourcePathCatalog.asset";
        private const string MixerPath = Root + "/Audio/EscapeAudioMixer.mixer";
        private const string PrefabPath = Root + "/Prefabs/UI/TitleCanvas.prefab";
        private const string ScenePath = Root + "/Scenes/TitleScene.unity";
        private const string TitleBackgroundAssetPath = Root + "/Resources/EscapeFromNightmares/Title/title_background.png";
        private const string TitleBgmAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/BGM/title_loop.wav";
        private const string UiClickAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/UI/ui_click.wav";
        private const string ConfirmSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_confirm.wav";

        [InitializeOnLoadMethod]
        private static void EnsureAssetsAfterLoad()
        {
            EditorApplication.delayCall -= EnsureMissingAssets;
            EditorApplication.delayCall += EnsureMissingAssets;
        }

        [MenuItem("Escape From Nightmares/Rebuild Title Scene Assets")]
        public static void RebuildAssets()
        {
            EnsureAssets(true);
        }

        private static void EnsureMissingAssets()
        {
            EnsureAssets(false);
        }

        private static void EnsureAssets(bool rebuild)
        {
            EnsureFolders();
            var catalog = EnsureCatalog();
            EnsureDummyResources();
            AssetDatabase.ImportAsset(TitleBackgroundAssetPath);
            AssetDatabase.ImportAsset(TitleBgmAssetPath);
            AssetDatabase.ImportAsset(UiClickAssetPath);
            AssetDatabase.ImportAsset(ConfirmSfxAssetPath);
            var mixer = EnsureAudioMixer(rebuild);
            var prefab = EnsureTitlePrefab(catalog, mixer, rebuild);
            EnsureTitleScene(prefab, rebuild);
            EnsureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            CreateFolder(Root + "/ScriptableObjects");
            CreateFolder(Root + "/Audio");
            CreateFolder(Root + "/Prefabs");
            CreateFolder(Root + "/Prefabs/UI");
            CreateFolder(Root + "/Scenes");
            CreateFolder(Root + "/Resources");
            CreateFolder(Root + "/Resources/EscapeFromNightmares");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Title");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio/BGM");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio/UI");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio/SFX");
        }

        private static ResourcePathCatalog EnsureCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ResourcePathCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ResourcePathCatalog.CreateDefault();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }

            catalog.titleBackgroundPath = "EscapeFromNightmares/Title/title_background";
            catalog.titleBgmPath = "EscapeFromNightmares/Audio/BGM/title_loop";
            catalog.uiClickPath = "EscapeFromNightmares/Audio/UI/ui_click";
            catalog.confirmSfxPath = "EscapeFromNightmares/Audio/SFX/sfx_confirm";
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static void EnsureDummyResources()
        {
            if (!File.Exists(TitleBackgroundAssetPath))
            {
                File.WriteAllBytes(TitleBackgroundAssetPath, CreateTitleBackgroundPng());
            }

            if (!File.Exists(TitleBgmAssetPath))
            {
                File.WriteAllBytes(TitleBgmAssetPath, CreateWav(55f, 1.5f, 0.08f));
            }

            if (!File.Exists(UiClickAssetPath))
            {
                File.WriteAllBytes(UiClickAssetPath, CreateWav(900f, 0.08f, 0.18f));
            }

            if (!File.Exists(ConfirmSfxAssetPath))
            {
                File.WriteAllBytes(ConfirmSfxAssetPath, CreateWav(440f, 0.16f, 0.2f));
            }
        }

        private static AudioMixer EnsureAudioMixer(bool rebuild)
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (mixer != null && !rebuild)
            {
                TryRepairMixer(mixer);
                return mixer;
            }

            if (mixer != null && rebuild)
            {
                AssetDatabase.DeleteAsset(MixerPath);
            }

            var controllerType = Type.GetType("UnityEditor.Audio.AudioMixerController, UnityEditor");
            var createMethod = controllerType?.GetMethod("CreateMixerControllerAtPath", BindingFlags.Public | BindingFlags.Static);
            if (createMethod == null)
            {
                Debug.LogWarning("Could not create AudioMixer automatically. Create one at " + MixerPath + " and expose MasterVolume, BgmVolume, SfxVolume, UiVolume.");
                return null;
            }

            createMethod.Invoke(null, new object[] { MixerPath });
            mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (mixer != null)
            {
                TryCreateMixerGroups(mixer);
                TryRepairMixer(mixer);
            }

            return mixer;
        }

        private static GameObject EnsureTitlePrefab(ResourcePathCatalog catalog, AudioMixer mixer, bool rebuild)
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (existing != null && !rebuild)
            {
                return existing;
            }

            var root = new GameObject("TitleCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TitleSceneController));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            var background = CreateImage("Background", root.transform, new Color(0.015f, 0.015f, 0.018f, 1f));
            Stretch(background.rectTransform, Vector2.zero, Vector2.one);

            var title = CreateText("TitleText", root.transform, "Escape From Nightmares", 58, TextAnchor.MiddleCenter);
            title.color = new Color(0.86f, 0.82f, 0.74f, 1f);
            Stretch(title.rectTransform, new Vector2(0.08f, 0.68f), new Vector2(0.92f, 0.86f));

            var subtitle = CreateText("SubtitleText", root.transform, "A quiet house. A wrong shadow.", 18, TextAnchor.MiddleCenter);
            subtitle.color = new Color(0.55f, 0.52f, 0.48f, 1f);
            Stretch(subtitle.rectTransform, new Vector2(0.15f, 0.61f), new Vector2(0.85f, 0.68f));

            var startButton = CreateButton("StartButton", root.transform, "Start");
            Stretch(startButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.45f), new Vector2(0.58f, 0.52f));
            var settingsButton = CreateButton("SettingsButton", root.transform, "Settings");
            Stretch(settingsButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.35f), new Vector2(0.58f, 0.42f));
            var quitButton = CreateButton("QuitButton", root.transform, "Quit");
            Stretch(quitButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.25f), new Vector2(0.58f, 0.32f));

            var settingsPanelObject = CreateSettingsPanel(root.transform, out var settingsPanel);
            settingsPanelObject.SetActive(false);

            var soundManager = new GameObject("SoundManager", typeof(SoundManager));
            soundManager.transform.SetParent(root.transform, false);

            var controller = root.GetComponent<TitleSceneController>();
            controller.SetReferences(catalog, mixer, background, startButton, settingsButton, quitButton, settingsPanel, "SampleScene");

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static void EnsureTitleScene(GameObject prefab, bool rebuild)
        {
            if (File.Exists(ScenePath) && !rebuild)
            {
                EnsureExistingTitleSceneCamera();
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateMainCamera();
            PrefabUtility.InstantiatePrefab(prefab);
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.SetAsLastSibling();
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static void EnsureExistingTitleSceneCamera()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.path != ScenePath)
            {
                return;
            }

            if (activeScene.GetRootGameObjects().Any(root => root.GetComponentInChildren<Camera>() != null))
            {
                return;
            }

            CreateMainCamera();
            EditorSceneManager.SaveScene(activeScene);
        }

        private static void CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.orthographic = true;
            camera.orthographicSize = 5f;
        }

        private static void EnsureBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            AddBuildScene(scenes, ScenePath, 0);
            AddBuildScene(scenes, "Assets/Scenes/SampleScene.unity", 1);
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static GameObject CreateSettingsPanel(Transform parent, out SettingsAudioPanel settingsPanel)
        {
            var panel = new GameObject("SettingsPanel", typeof(RectTransform), typeof(Image), typeof(SettingsAudioPanel));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.color = new Color(0.02f, 0.018f, 0.018f, 0.96f);
            Stretch(panel.GetComponent<RectTransform>(), new Vector2(0.28f, 0.18f), new Vector2(0.72f, 0.62f));

            var header = CreateText("Header", panel.transform, "Settings", 28, TextAnchor.MiddleCenter);
            Stretch(header.rectTransform, new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.96f));

            var master = CreateSliderRow(panel.transform, "Master", 0.66f);
            var bgm = CreateSliderRow(panel.transform, "BGM", 0.5f);
            var sfx = CreateSliderRow(panel.transform, "SFX", 0.34f);
            var ui = CreateSliderRow(panel.transform, "UI", 0.18f);

            var closeButton = CreateButton("CloseButton", panel.transform, "Close");
            Stretch(closeButton.GetComponent<RectTransform>(), new Vector2(0.36f, 0.04f), new Vector2(0.64f, 0.14f));

            settingsPanel = panel.GetComponent<SettingsAudioPanel>();
            settingsPanel.SetControls(master, bgm, sfx, ui, closeButton);
            return panel;
        }

        private static Slider CreateSliderRow(Transform parent, string label, float y)
        {
            var text = CreateText(label + "Label", parent, label, 16, TextAnchor.MiddleLeft);
            Stretch(text.rectTransform, new Vector2(0.1f, y), new Vector2(0.28f, y + 0.1f));

            var sliderObject = new GameObject(label + "Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(parent, false);
            Stretch(sliderObject.GetComponent<RectTransform>(), new Vector2(0.32f, y + 0.02f), new Vector2(0.9f, y + 0.08f));

            var background = CreateImage("Background", sliderObject.transform, new Color(0.1f, 0.1f, 0.1f, 1f));
            Stretch(background.rectTransform, Vector2.zero, Vector2.one);
            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            Stretch(fillArea.GetComponent<RectTransform>(), new Vector2(0.03f, 0.25f), new Vector2(0.97f, 0.75f));
            var fill = CreateImage("Fill", fillArea.transform, new Color(0.55f, 0.08f, 0.08f, 1f));
            Stretch(fill.rectTransform, Vector2.zero, Vector2.one);
            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderObject.transform, false);
            Stretch(handleArea.GetComponent<RectTransform>(), new Vector2(0.03f, 0f), new Vector2(0.97f, 1f));
            var handle = CreateImage("Handle", handleArea.transform, new Color(0.86f, 0.82f, 0.74f, 1f));
            handle.rectTransform.sizeDelta = new Vector2(16f, 24f);

            var slider = sliderObject.GetComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.value = 0.8f;
            return slider;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            var image = imageObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, string text, int fontSize, TextAnchor anchor)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var component = textObject.GetComponent<Text>();
            component.text = text;
            component.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            component.fontSize = fontSize;
            component.color = new Color(0.82f, 0.78f, 0.7f, 1f);
            component.alignment = anchor;
            component.horizontalOverflow = HorizontalWrapMode.Wrap;
            component.verticalOverflow = VerticalWrapMode.Truncate;
            return component;
        }

        private static Button CreateButton(string name, Transform parent, string label)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = new Color(0.11f, 0.015f, 0.02f, 0.95f);
            var text = CreateText("Text", buttonObject.transform, label, 18, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, Vector2.zero, Vector2.one);
            return buttonObject.GetComponent<Button>();
        }

        private static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void CreateFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            var folder = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent))
            {
                CreateFolder(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }

        private static void AddBuildScene(System.Collections.Generic.List<EditorBuildSettingsScene> scenes, string path, int index)
        {
            scenes.RemoveAll(scene => scene.path == path);
            scenes.Insert(Mathf.Clamp(index, 0, scenes.Count), new EditorBuildSettingsScene(path, true));
        }

        private static byte[] CreateTitleBackgroundPng()
        {
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var vertical = y / (float)texture.height;
                    var vignette = Mathf.Abs((x / (float)texture.width) - 0.5f) * 0.35f;
                    var redPulse = x > 920 && y < 260 ? 0.045f : 0f;
                    texture.SetPixel(x, y, new Color(0.012f + vignette, 0.012f, 0.015f + vertical * 0.025f, 1f) + new Color(redPulse, 0f, 0f, 0f));
                }
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateWav(float frequency, float duration, float amplitude)
        {
            const int sampleRate = 44100;
            var sampleCount = Mathf.CeilToInt(sampleRate * duration);
            var dataSize = sampleCount * 2;
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataSize);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(sampleRate);
                writer.Write(sampleRate * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(dataSize);

                for (var sample = 0; sample < sampleCount; sample++)
                {
                    var envelope = 1f - sample / (float)sampleCount;
                    var value = Mathf.Sin(sample * frequency * Mathf.PI * 2f / sampleRate) * amplitude * envelope;
                    writer.Write((short)(value * short.MaxValue));
                }

                return stream.ToArray();
            }
        }

        private static void TryRepairMixer(AudioMixer mixer)
        {
            TryCreateMixerGroups(mixer);
            EditorUtility.SetDirty(mixer);
        }

        private static void TryCreateMixerGroups(AudioMixer mixer)
        {
            var controllerType = Type.GetType("UnityEditor.Audio.AudioMixerController, UnityEditor");
            if (controllerType == null || !controllerType.IsInstanceOfType(mixer))
            {
                return;
            }

            var masterGroup = controllerType.GetProperty("masterGroup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(mixer);
            if (masterGroup == null)
            {
                return;
            }

            EnsureMixerGroup(mixer, controllerType, masterGroup, "BGM");
            EnsureMixerGroup(mixer, controllerType, masterGroup, "SFX");
            EnsureMixerGroup(mixer, controllerType, masterGroup, "UI");
        }

        private static void EnsureMixerGroup(AudioMixer mixer, Type controllerType, object masterGroup, string groupName)
        {
            if (mixer.FindMatchingGroups(groupName).Length > 0)
            {
                return;
            }

            var addGroupMethod = controllerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(method => method.Name == "AddGroup" && method.GetParameters().Length == 2);
            if (addGroupMethod == null)
            {
                return;
            }

            try
            {
                var parameters = addGroupMethod.GetParameters();
                var arguments = parameters[0].ParameterType == typeof(string)
                    ? new[] { groupName, masterGroup }
                    : new[] { masterGroup, groupName };
                addGroupMethod.Invoke(mixer, arguments);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not add AudioMixer group '" + groupName + "': " + exception.Message);
            }
        }

    }
}
