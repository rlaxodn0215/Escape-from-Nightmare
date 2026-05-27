using System.Collections.Generic;
using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.Services;
using EscapeFromNightmares.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmares.Editor
{
    /// <summary>
    /// Rebuilds title/main scene infrastructure using direct serialized Sprite and AudioClip references.
    /// </summary>
    public static class TitleSceneAssetBuilder
    {
        private const string TitleScenePath = "Assets/Scenes/Title.unity";
        private const string MainScenePath = "Assets/Scenes/Main.unity";
        private const string ResourceCatalogPath = "Assets/Resources/EscapeFromNightmares/ResourcePathCatalog.asset";

        [MenuItem("Escape From Nightmares/Rebuild Title Scene Assets")]
        public static void RebuildAssets()
        {
            EnsureFolders();
            var resourceCatalog = EnsureResourceCatalog();
            EnsureTitleScene();
            EnsureMainSceneBindings(resourceCatalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Escape From Nightmares/Rebuild Main Sample Assets")]
        public static void RebuildMainSampleAssets()
        {
            RebuildAssets();
        }

        private static void EnsureTitleScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            var title = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TitleSceneController));
            title.transform.SetParent(canvasObject.transform, false);
            Stretch(title.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

            var background = CreateImage("Background", title.transform, LoadSprite("Assets/Art/UI/title_background.png"));
            Stretch(background.rectTransform, Vector2.zero, Vector2.one);

            var titleLogo = CreateImage("TitleLogoImage", title.transform, LoadSprite("Assets/Art/UI/title_logo_escape_from_nightmare.png"));
            titleLogo.preserveAspect = true;
            Stretch(titleLogo.rectTransform, new Vector2(0.16f, 0.66f), new Vector2(0.84f, 0.88f));

            var startButton = CreateImageButton("StartButton", title.transform, LoadSprite("Assets/Art/UI/button_start.png"));
            Stretch(startButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.45f), new Vector2(0.58f, 0.52f));

            var quitButton = CreateImageButton("QuitButton", title.transform, LoadSprite("Assets/Art/UI/button_quit.png"));
            Stretch(quitButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.35f), new Vector2(0.58f, 0.42f));

            var soundManager = new GameObject("SoundManager", typeof(SoundManager));
            soundManager.transform.SetParent(title.transform, false);

            title.GetComponent<TitleSceneController>().SetReferences(
                background,
                titleLogo,
                startButton,
                quitButton,
                LoadAudio("Assets/Audio/BGM/title_loop.wav"),
                LoadAudio("Assets/Audio/UI/ui_click.wav"),
                LoadAudio("Assets/Audio/SFX/sfx_confirm.wav"),
                "Main");

            CreateMainCamera();
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            EditorSceneManager.SaveScene(scene, TitleScenePath);
        }

        private static void EnsureMainSceneBindings(ResourcePathCatalog resourceCatalog)
        {
            if (!File.Exists(MainScenePath))
            {
                return;
            }

            var scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            var director = Object.FindFirstObjectByType<GameDirector>();
            if (director == null)
            {
                var gameObject = new GameObject("Escape From Nightmares Runtime");
                director = gameObject.AddComponent<GameDirector>();
            }

            var serialized = new SerializedObject(director);
            serialized.FindProperty("resourceCatalog").objectReferenceValue = resourceCatalog;
            SetSpriteBindings(serialized.FindProperty("spriteBindings"));
            SetSoundBindings(serialized.FindProperty("soundBindings"));
            SetMonsterPlacements(serialized.FindProperty("monsterPlacementCatalog"));
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void SetSpriteBindings(SerializedProperty property)
        {
            if (property == null)
            {
                return;
            }

            var entries = BuildSpriteBindings();
            property.arraySize = entries.Count;
            for (var index = 0; index < entries.Count; index++)
            {
                var element = property.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("spriteId").stringValue = entries[index].spriteId;
                element.FindPropertyRelative("sprite").objectReferenceValue = entries[index].sprite;
            }
        }

        private static void SetSoundBindings(SerializedProperty property)
        {
            if (property == null)
            {
                return;
            }

            var entries = BuildSoundBindings();

            property.arraySize = entries.Length;
            for (var index = 0; index < entries.Length; index++)
            {
                var element = property.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("soundId").stringValue = entries[index].soundId;
                element.FindPropertyRelative("clip").objectReferenceValue = entries[index].clip;
                element.FindPropertyRelative("category").enumValueIndex = (int)entries[index].category;
                element.FindPropertyRelative("loop").boolValue = entries[index].loop;
            }
        }

        private static void SetMonsterPlacements(SerializedProperty property)
        {
            if (property == null)
            {
                return;
            }

            var stage = RuntimeStageFactory.CreateStage1();
            var placements = MonsterPlacementCatalog.CreateDefaultEntries(stage);
            var placementsProperty = property.FindPropertyRelative("placements");
            var entries = new List<MonsterPlacementEntry>(placements);
            placementsProperty.arraySize = entries.Count;
            for (var index = 0; index < entries.Count; index++)
            {
                var element = placementsProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("roomId").stringValue = entries[index].roomId;
                element.FindPropertyRelative("faceDirection").enumValueIndex = (int)entries[index].faceDirection;
                element.FindPropertyRelative("enabled").boolValue = entries[index].enabled;
                element.FindPropertyRelative("normalizedRect").rectValue = entries[index].normalizedRect;
            }
        }

        private static ResourcePathCatalog EnsureResourceCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ResourcePathCatalog>(ResourceCatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<ResourcePathCatalog>();
                AssetDatabase.CreateAsset(catalog, ResourceCatalogPath);
            }

            catalog.SetSpriteBindings(BuildSpriteBindings());
            catalog.SetSoundBindings(BuildSoundBindings());
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static List<SpriteBinding> BuildSpriteBindings()
        {
            var entries = new List<SpriteBinding>();
            AddSprites(entries, "Assets/Art");
            return entries
                .Where(entry => entry != null && entry.sprite != null && !string.IsNullOrWhiteSpace(entry.spriteId))
                .GroupBy(entry => entry.spriteId)
                .Select(group => group.OrderBy(entry => AssetDatabase.GetAssetPath(entry.sprite), System.StringComparer.Ordinal).First())
                .OrderBy(entry => entry.spriteId, System.StringComparer.Ordinal)
                .ToList();
        }

        private static SoundEntry[] BuildSoundBindings()
        {
            return new[]
            {
                Sound("bgm_stage1_ambient", "Assets/Audio/BGM/bgm_stage1_ambient.wav", SoundCategory.Bgm, true),
                Sound("bgm_final_chase", "Assets/Audio/BGM/bgm_final_chase.wav", SoundCategory.Bgm, true),
                Sound("ui_click", "Assets/Audio/UI/ui_click.wav", SoundCategory.Ui, false),
                Sound("sfx_confirm", "Assets/Audio/SFX/sfx_confirm.wav", SoundCategory.Sfx, false),
                Sound("sfx_door", "Assets/Audio/SFX/sfx_door.wav", SoundCategory.Sfx, false),
                Sound("sfx_item_pickup", "Assets/Audio/SFX/sfx_item_pickup.wav", SoundCategory.Sfx, false),
                Sound("sfx_drawer_open", "Assets/Audio/SFX/sfx_drawer_open.wav", SoundCategory.Sfx, false),
                Sound("sfx_drawer_close", "Assets/Audio/SFX/sfx_drawer_close.wav", SoundCategory.Sfx, false),
                Sound("sfx_hide", "Assets/Audio/SFX/sfx_hide.wav", SoundCategory.Sfx, false),
                Sound("sfx_puzzle_success", "Assets/Audio/SFX/sfx_puzzle_success.wav", SoundCategory.Sfx, false),
                Sound("sfx_puzzle_failure", "Assets/Audio/SFX/sfx_puzzle_failure.wav", SoundCategory.Sfx, false)
            }.OrderBy(entry => entry.soundId, System.StringComparer.Ordinal).ToArray();
        }

        private static SoundEntry Sound(string soundId, string path, SoundCategory category, bool loop)
        {
            return new SoundEntry
            {
                soundId = soundId,
                clip = LoadAudio(path),
                category = category,
                loop = loop
            };
        }

        private static void AddSprites(List<SpriteBinding> entries, string root)
        {
            if (!AssetDatabase.IsValidFolder(root))
            {
                return;
            }

            foreach (var guid in AssetDatabase.FindAssets("t:Sprite", new[] { root }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    entries.Add(new SpriteBinding
                    {
                        spriteId = Path.GetFileNameWithoutExtension(path),
                        sprite = sprite
                    });
                }
            }
        }

        private static Image CreateImage(string name, Transform parent, Sprite sprite)
        {
            var image = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false);
            image.sprite = sprite;
            image.color = Color.white;
            return image;
        }

        private static Button CreateImageButton(string name, Transform parent, Sprite sprite)
        {
            var button = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<Button>();
            button.transform.SetParent(parent, false);
            button.GetComponent<Image>().sprite = sprite;
            button.GetComponent<Image>().color = Color.white;
            return button;
        }

        private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Sprite LoadSprite(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static AudioClip LoadAudio(string path)
        {
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/Scenes");
            EnsureFolder("Assets/Data");
            EnsureFolder("Assets/Resources/EscapeFromNightmares");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            var folder = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, folder);
        }

        private static void CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.backgroundColor = Color.black;
        }
    }
}
