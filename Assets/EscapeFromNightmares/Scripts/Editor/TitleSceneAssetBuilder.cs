using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
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
        private const string InventoryWindowPrefabPath = Root + "/Prefabs/UI/InventoryWindow.prefab";
        private const string ScenePath = Root + "/Scenes/TitleScene.unity";
        private const string MainScenePath = Root + "/Scenes/MainScene.unity";
        private const string RoomSpriteCatalogPath = Root + "/ScriptableObjects/RoomSpriteCatalog.asset";
        private const string TitleBackgroundAssetPath = Root + "/Resources/EscapeFromNightmares/Title/title_background.png";
        private const string TitleLogoAssetPath = Root + "/Resources/EscapeFromNightmares/Title/title_logo_escape_from_nightmare.png";
        private const string StartButtonAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/button_start.png";
        private const string SettingsButtonAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/button_settings.png";
        private const string QuitButtonAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/button_quit.png";
        private const string CloseButtonAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/button_close.png";
        private const string SettingsPanelBackgroundAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/settings_panel_bg.png";
        private const string SettingsHeaderAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/settings_header.png";
        private const string SettingsMasterLabelAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/settings_label_master.png";
        private const string SettingsBgmLabelAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/settings_label_bgm.png";
        private const string SettingsSfxLabelAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/settings_label_sfx.png";
        private const string SettingsUiLabelAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/settings_label_ui.png";
        private const string SliderTrackAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/slider_track.png";
        private const string SliderFillAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/slider_fill.png";
        private const string SliderHandleAssetPath = Root + "/Resources/EscapeFromNightmares/Title/UI/slider_handle.png";
        private const string TitleBgmAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/BGM/title_loop.wav";
        private const string StageAmbientBgmAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/BGM/bgm_stage1_ambient.wav";
        private const string FinalChaseBgmAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/BGM/bgm_final_chase.wav";
        private const string UiClickAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/UI/ui_click.wav";
        private const string ConfirmSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_confirm.wav";
        private const string DoorSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_door.wav";
        private const string ItemPickupSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_item_pickup.wav";
        private const string HideSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_hide.wav";
        private const string PuzzleSuccessSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_puzzle_success.wav";
        private const string PuzzleFailureSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_puzzle_failure.wav";
        private const string DrawerOpenSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_drawer_open.wav";
        private const string DrawerCloseSfxAssetPath = Root + "/Resources/EscapeFromNightmares/Audio/SFX/sfx_drawer_close.wav";
        private const string DrawerClosedAssetPath = Root + "/Resources/EscapeFromNightmares/CloseUps/child_desk_drawer_closed.png";
        private const string DrawerOpenWithItemAssetPath = Root + "/Resources/EscapeFromNightmares/CloseUps/child_desk_drawer_open_with_item.png";
        private const string DrawerOpenEmptyAssetPath = Root + "/Resources/EscapeFromNightmares/CloseUps/child_desk_drawer_open_empty.png";
        private const string StudySafeClueNoteAssetPath = Root + "/Resources/EscapeFromNightmares/CloseUps/study_safe_clue_note.png";
        private const string ChildBedUnderViewAssetPath = Root + "/Resources/EscapeFromNightmares/HideViews/child_bed_under_view.png";
        private const string FuseHolderIconAssetPath = Root + "/Resources/EscapeFromNightmares/Items/item_fuse_holder.png";
        private const string InventoryPanelAssetPath = Root + "/Resources/EscapeFromNightmares/UI/inventory_panel_bg.png";
        private const string InventoryButtonAssetPath = Root + "/Resources/EscapeFromNightmares/UI/inventory_button.png";
        private const string InventoryCloseButtonAssetPath = Root + "/Resources/EscapeFromNightmares/UI/inventory_close_button.png";
        private const string InventorySlotEmptyAssetPath = Root + "/Resources/EscapeFromNightmares/UI/inventory_slot_empty.png";
        private const string InventorySlotSelectedAssetPath = Root + "/Resources/EscapeFromNightmares/UI/inventory_slot_selected.png";
        private const string RotateLeftButtonAssetPath = Root + "/Resources/EscapeFromNightmares/UI/ui_rotate_left.png";
        private const string RotateRightButtonAssetPath = Root + "/Resources/EscapeFromNightmares/UI/ui_rotate_right.png";
        private const string HideExitButtonAssetPath = Root + "/Resources/EscapeFromNightmares/UI/ui_hide_exit.png";
        private const string BackArrowButtonAssetPath = Root + "/Resources/EscapeFromNightmares/UI/ui_back_arrow.png";

        private static readonly string[] ChildRoomFaceAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Rooms/child_room_north.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/child_room_east.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/child_room_south.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/child_room_west.png"
        };

        private static readonly string[] SecondFloorHallwayFaceAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Rooms/second_floor_hallway_north.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/second_floor_hallway_south.png"
        };

        private static readonly string[] StudyFaceAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Rooms/study_north.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/study_east.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/study_south.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/study_west.png"
        };

        private static readonly string[] StateRoomAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Rooms/child_room_north_drawer_empty.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/study_north_safe_open.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/study_north_safe_open_with_item.png",
            Root + "/Resources/EscapeFromNightmares/Rooms/study_north_safe_open_empty.png"
        };

        private static readonly string[] ChildRoomObjectAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Objects/child_desk_drawer.png",
            Root + "/Resources/EscapeFromNightmares/Objects/child_room_door.png",
            Root + "/Resources/EscapeFromNightmares/Objects/child_bed_hide.png",
            Root + "/Resources/EscapeFromNightmares/Objects/child_window_silhouette.png",
            Root + "/Resources/EscapeFromNightmares/Items/item_torn_drawing_fragment.png"
        };

        private static readonly string[] PuzzleSampleAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/laundry_storage_box.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/breaker_box.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/mirror_symbol_panel.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/master_bedroom_drawer.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/attic_toy_sequence.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/basement_altar.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/front_door_escape.png"
        };

        private static readonly string[] StudySafeDigitAssetPaths =
        {
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_0.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_1.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_2.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_3.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_4.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_5.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_6.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_7.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_8.png",
            Root + "/Resources/EscapeFromNightmares/Puzzles/study_safe_digit_9.png"
        };

        private static readonly string[] CloseUpAssetPaths =
        {
            DrawerClosedAssetPath,
            DrawerOpenWithItemAssetPath,
            DrawerOpenEmptyAssetPath,
            StudySafeClueNoteAssetPath,
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_safe_locked.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_safe_open_with_item.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_safe_open_empty.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/child_desk_surface.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/child_drawing_board.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/child_window_view.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_safe_surrounding.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_desk_surface.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_clue_board.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_portrait.png",
            Root + "/Resources/EscapeFromNightmares/CloseUps/study_window_view.png"
        };

        private static readonly string[] HideViewAssetPaths =
        {
            ChildBedUnderViewAssetPath
        };

        private static readonly string[] MainUiAssetPaths =
        {
            InventoryPanelAssetPath,
            InventoryButtonAssetPath,
            InventoryCloseButtonAssetPath,
            InventorySlotEmptyAssetPath,
            InventorySlotSelectedAssetPath,
            RotateLeftButtonAssetPath,
            RotateRightButtonAssetPath,
            HideExitButtonAssetPath,
            BackArrowButtonAssetPath
        };

        private static readonly string[] TitleImageAssetPaths =
        {
            TitleBackgroundAssetPath,
            TitleLogoAssetPath,
            StartButtonAssetPath,
            SettingsButtonAssetPath,
            QuitButtonAssetPath,
            CloseButtonAssetPath,
            SettingsPanelBackgroundAssetPath,
            SettingsHeaderAssetPath,
            SettingsMasterLabelAssetPath,
            SettingsBgmLabelAssetPath,
            SettingsSfxLabelAssetPath,
            SettingsUiLabelAssetPath,
            SliderTrackAssetPath,
            SliderFillAssetPath,
            SliderHandleAssetPath
        };

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

        [MenuItem("Escape From Nightmares/Rebuild Main Sample Assets")]
        public static void RebuildMainSampleAssets()
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
            EnsureDummyResources(rebuild);
            ImportAll(TitleImageAssetPaths);
            AssetDatabase.ImportAsset(TitleBgmAssetPath);
            AssetDatabase.ImportAsset(UiClickAssetPath);
            AssetDatabase.ImportAsset(ConfirmSfxAssetPath);
            ImportAll(ChildRoomFaceAssetPaths);
            ImportAll(SecondFloorHallwayFaceAssetPaths);
            ImportAll(StudyFaceAssetPaths);
            ImportAll(StateRoomAssetPaths);
            ImportAll(ChildRoomObjectAssetPaths);
            AssetDatabase.ImportAsset(FuseHolderIconAssetPath);
            ImportAll(PuzzleSampleAssetPaths);
            ImportAll(StudySafeDigitAssetPaths);
            ImportAll(CloseUpAssetPaths);
            ImportAll(HideViewAssetPaths);
            ImportAll(MainUiAssetPaths);
            var roomSpriteCatalog = EnsureRoomSpriteCatalog();
            var inventoryWindowPrefab = EnsureInventoryWindowPrefab(roomSpriteCatalog, rebuild);
            var mixer = EnsureAudioMixer(rebuild);
            var prefab = EnsureTitlePrefab(catalog, mixer, rebuild);
            EnsureTitleScene(prefab, rebuild);
            EnsureMainScene(roomSpriteCatalog, inventoryWindowPrefab, rebuild);
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
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Title/UI");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio/BGM");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio/UI");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Audio/SFX");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Rooms");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Objects");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Items");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/Puzzles");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/CloseUps");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/HideViews");
            CreateFolder(Root + "/Resources/EscapeFromNightmares/UI");
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
            catalog.titleLogoPath = "EscapeFromNightmares/Title/title_logo_escape_from_nightmare";
            catalog.titleStartButtonPath = "EscapeFromNightmares/Title/UI/button_start";
            catalog.titleSettingsButtonPath = "EscapeFromNightmares/Title/UI/button_settings";
            catalog.titleQuitButtonPath = "EscapeFromNightmares/Title/UI/button_quit";
            catalog.titleCloseButtonPath = "EscapeFromNightmares/Title/UI/button_close";
            catalog.settingsPanelBackgroundPath = "EscapeFromNightmares/Title/UI/settings_panel_bg";
            catalog.settingsHeaderPath = "EscapeFromNightmares/Title/UI/settings_header";
            catalog.settingsMasterLabelPath = "EscapeFromNightmares/Title/UI/settings_label_master";
            catalog.settingsBgmLabelPath = "EscapeFromNightmares/Title/UI/settings_label_bgm";
            catalog.settingsSfxLabelPath = "EscapeFromNightmares/Title/UI/settings_label_sfx";
            catalog.settingsUiLabelPath = "EscapeFromNightmares/Title/UI/settings_label_ui";
            catalog.settingsSliderTrackPath = "EscapeFromNightmares/Title/UI/slider_track";
            catalog.settingsSliderFillPath = "EscapeFromNightmares/Title/UI/slider_fill";
            catalog.settingsSliderHandlePath = "EscapeFromNightmares/Title/UI/slider_handle";
            catalog.titleBgmPath = "EscapeFromNightmares/Audio/BGM/title_loop";
            catalog.uiClickPath = "EscapeFromNightmares/Audio/UI/ui_click";
            catalog.confirmSfxPath = "EscapeFromNightmares/Audio/SFX/sfx_confirm";
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static void EnsureDummyResources(bool rebuild)
        {
            EnsurePng(TitleBackgroundAssetPath, CreateTitleBackgroundPng(), rebuild);
            EnsurePng(TitleLogoAssetPath, CreateTitleLogoPng(), rebuild);
            EnsurePng(StartButtonAssetPath, CreateButtonPng("START", 360, 96), rebuild);
            EnsurePng(SettingsButtonAssetPath, CreateButtonPng("SETTINGS", 360, 96), rebuild);
            EnsurePng(QuitButtonAssetPath, CreateButtonPng("QUIT", 360, 96), rebuild);
            EnsurePng(CloseButtonAssetPath, CreateButtonPng("CLOSE", 300, 76), rebuild);
            EnsurePng(SettingsPanelBackgroundAssetPath, CreateSettingsPanelBackgroundPng(), rebuild);
            EnsurePng(SettingsHeaderAssetPath, CreateHeaderPng("SETTINGS", 420, 90), rebuild);
            EnsurePng(SettingsMasterLabelAssetPath, CreateLabelPng("MASTER", 220, 64), rebuild);
            EnsurePng(SettingsBgmLabelAssetPath, CreateLabelPng("BGM", 220, 64), rebuild);
            EnsurePng(SettingsSfxLabelAssetPath, CreateLabelPng("SFX", 220, 64), rebuild);
            EnsurePng(SettingsUiLabelAssetPath, CreateLabelPng("UI", 220, 64), rebuild);
            EnsurePng(SliderTrackAssetPath, CreateSliderTrackPng(), rebuild);
            EnsurePng(SliderFillAssetPath, CreateSliderFillPng(), rebuild);
            EnsurePng(SliderHandleAssetPath, CreateSliderHandlePng(), rebuild);

            if (!File.Exists(TitleBgmAssetPath))
            {
                File.WriteAllBytes(TitleBgmAssetPath, CreateWav(55f, 1.5f, 0.08f));
            }

            if (!File.Exists(StageAmbientBgmAssetPath))
            {
                File.WriteAllBytes(StageAmbientBgmAssetPath, CreateWav(72f, 2.2f, 0.06f));
            }

            if (!File.Exists(FinalChaseBgmAssetPath))
            {
                File.WriteAllBytes(FinalChaseBgmAssetPath, CreateWav(96f, 1.2f, 0.08f));
            }

            if (!File.Exists(UiClickAssetPath))
            {
                File.WriteAllBytes(UiClickAssetPath, CreateWav(900f, 0.08f, 0.18f));
            }

            if (!File.Exists(ConfirmSfxAssetPath))
            {
                File.WriteAllBytes(ConfirmSfxAssetPath, CreateWav(440f, 0.16f, 0.2f));
            }

            EnsureWav(DoorSfxAssetPath, 180f, 0.16f, 0.18f);
            EnsureWav(ItemPickupSfxAssetPath, 660f, 0.12f, 0.18f);
            EnsureWav(DrawerOpenSfxAssetPath, 120f, 0.22f, 0.16f);
            EnsureWav(DrawerCloseSfxAssetPath, 90f, 0.18f, 0.18f);
            EnsureWav(HideSfxAssetPath, 130f, 0.2f, 0.12f);
            EnsureWav(PuzzleSuccessSfxAssetPath, 520f, 0.22f, 0.18f);
            EnsureWav(PuzzleFailureSfxAssetPath, 95f, 0.18f, 0.16f);
            EnsureChildRoomImages();
            EnsureSecondFloorHallwayImages();
            EnsureStudyImages();
            EnsureStateRoomImages();
            EnsurePuzzleImages();
            EnsureCloseUpImages();
            EnsureHideViewImages();
            EnsureMainUiImages(rebuild);
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
            if (existing != null && !rebuild && !NeedsTitlePrefabUpgrade(existing))
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
            background.sprite = LoadSprite(TitleBackgroundAssetPath);
            Stretch(background.rectTransform, Vector2.zero, Vector2.one);

            var titleLogo = CreateImage("TitleLogoImage", root.transform, Color.white);
            titleLogo.sprite = LoadSprite(TitleLogoAssetPath);
            titleLogo.preserveAspect = true;
            Stretch(titleLogo.rectTransform, new Vector2(0.16f, 0.66f), new Vector2(0.84f, 0.88f));

            var startButton = CreateImageButton("StartButton", root.transform);
            startButton.GetComponent<Image>().sprite = LoadSprite(StartButtonAssetPath);
            Stretch(startButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.45f), new Vector2(0.58f, 0.52f));
            var settingsButton = CreateImageButton("SettingsButton", root.transform);
            settingsButton.GetComponent<Image>().sprite = LoadSprite(SettingsButtonAssetPath);
            Stretch(settingsButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.35f), new Vector2(0.58f, 0.42f));
            var quitButton = CreateImageButton("QuitButton", root.transform);
            quitButton.GetComponent<Image>().sprite = LoadSprite(QuitButtonAssetPath);
            Stretch(quitButton.GetComponent<RectTransform>(), new Vector2(0.42f, 0.25f), new Vector2(0.58f, 0.32f));

            var settingsPanelObject = CreateSettingsPanel(root.transform, out var settingsPanel);
            settingsPanelObject.SetActive(false);

            var soundManager = new GameObject("SoundManager", typeof(SoundManager));
            soundManager.transform.SetParent(root.transform, false);

            var controller = root.GetComponent<TitleSceneController>();
            controller.SetReferences(catalog, mixer, background, titleLogo, startButton, settingsButton, quitButton, settingsPanel, "MainScene");

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static bool NeedsTitlePrefabUpgrade(GameObject prefab)
        {
            if (prefab == null)
            {
                return true;
            }

            if (prefab.transform.Find("TitleText") != null || prefab.transform.Find("SubtitleText") != null)
            {
                return true;
            }

            var logo = prefab.transform.Find("TitleLogoImage")?.GetComponent<Image>();
            var background = prefab.transform.Find("Background")?.GetComponent<Image>();
            return logo == null || logo.sprite == null || background == null || background.sprite == null;
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

        private static void EnsureMainScene(RoomSpriteCatalog roomSpriteCatalog, GameObject inventoryWindowPrefab, bool rebuild)
        {
            if (File.Exists(MainScenePath) && !rebuild && MainSceneLooksCurrent())
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateMainCamera();
            var canvasObject = new GameObject("MainCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            var root = canvasObject.GetComponent<RectTransform>();

            var roomFace = CreateImage("RoomFaceImage", root, Color.white);
            roomFace.sprite = LoadSprite(ChildRoomFaceAssetPaths[0]);
            roomFace.preserveAspect = true;
            Stretch(roomFace.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f));

            var roomObjectLayer = new GameObject("RoomObjectLayer", typeof(RectTransform));
            roomObjectLayer.transform.SetParent(root, false);
            Stretch(roomObjectLayer.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 1f));

            var rotateLeft = CreateImageButton("RotateLeftButton", root);
            rotateLeft.GetComponent<Image>().sprite = LoadSprite(RotateLeftButtonAssetPath);
            Stretch(rotateLeft.GetComponent<RectTransform>(), new Vector2(0.02f, 0.41f), new Vector2(0.1f, 0.59f));

            var rotateRight = CreateImageButton("RotateRightButton", root);
            rotateRight.GetComponent<Image>().sprite = LoadSprite(RotateRightButtonAssetPath);
            Stretch(rotateRight.GetComponent<RectTransform>(), new Vector2(0.9f, 0.41f), new Vector2(0.98f, 0.59f));

            var inventoryButton = CreateImageButton("InventoryButton", root);
            inventoryButton.GetComponent<Image>().sprite = LoadSprite(InventoryButtonAssetPath);
            Stretch(inventoryButton.GetComponent<RectTransform>(), new Vector2(0.87f, 0.86f), new Vector2(0.97f, 0.96f));

            var closeUp = CreateCloseUpPanel(root);
            var hideView = CreateHideViewPanel(root);
            var puzzle = CreatePuzzlePanel(root);
            var transitionOverlay = CreateSceneTransitionOverlay(root);

            var inventoryWindow = inventoryWindowPrefab != null
                ? (GameObject)PrefabUtility.InstantiatePrefab(inventoryWindowPrefab, canvasObject.transform)
                : CreateInventoryWindowPrefab(roomSpriteCatalog);
            inventoryWindow.name = "InventoryWindow";
            inventoryWindow.SetActive(false);

            var runtime = new GameObject("Escape From Nightmares Runtime", typeof(GameDirector));
            var director = runtime.GetComponent<GameDirector>();
            director.SetSceneReferences(
                roomSpriteCatalog,
                roomFace,
                roomObjectLayer.GetComponent<RectTransform>(),
                rotateLeft,
                rotateRight,
                inventoryButton,
                inventoryWindow.GetComponent<InventoryWindow>(),
                closeUp.panel,
                closeUp.image,
                closeUp.openButton,
                closeUp.itemButton,
                closeUp.closeButton,
                closeUp.itemHitbox,
                hideView.panel,
                hideView.image,
                hideView.exitButton,
                puzzle.panel,
                puzzle.title,
                puzzle.input,
                puzzle.log,
                puzzle.image,
                puzzle.tokens,
                puzzle.backButton,
                puzzle.safeDigitImages,
                puzzle.safeDigitButtons,
                transitionOverlay);

            new GameObject("Sound Manager", typeof(SoundManager)).transform.SetAsLastSibling();
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.SetAsLastSibling();
            EditorSceneManager.SaveScene(scene, MainScenePath);
        }

        private static bool MainSceneLooksCurrent()
        {
            if (!File.Exists(MainScenePath))
            {
                return false;
            }

            var sceneText = File.ReadAllText(MainScenePath);
            return sceneText.Contains("MainCanvas")
                && sceneText.Contains("InventoryWindow")
                && sceneText.Contains("DrawerCloseUpPanel")
                && sceneText.Contains("HideViewPanel")
                && sceneText.Contains("PuzzleBackButton")
                && sceneText.Contains("StudySafeDigitButton0")
                && sceneText.Contains("SceneTransitionOverlay");
        }

        private readonly struct CloseUpUi
        {
            public readonly GameObject panel;
            public readonly Image image;
            public readonly Button openButton;
            public readonly Button itemButton;
            public readonly Button closeButton;
            public readonly RectTransform itemHitbox;

            public CloseUpUi(GameObject panel, Image image, Button openButton, Button itemButton, Button closeButton, RectTransform itemHitbox)
            {
                this.panel = panel;
                this.image = image;
                this.openButton = openButton;
                this.itemButton = itemButton;
                this.closeButton = closeButton;
                this.itemHitbox = itemHitbox;
            }
        }

        private readonly struct PuzzleUi
        {
            public readonly GameObject panel;
            public readonly Text title;
            public readonly Text input;
            public readonly Text log;
            public readonly Image image;
            public readonly RectTransform tokens;
            public readonly Button backButton;
            public readonly Image[] safeDigitImages;
            public readonly Button[] safeDigitButtons;

            public PuzzleUi(GameObject panel, Text title, Text input, Text log, Image image, RectTransform tokens, Button backButton, Image[] safeDigitImages, Button[] safeDigitButtons)
            {
                this.panel = panel;
                this.title = title;
                this.input = input;
                this.log = log;
                this.image = image;
                this.tokens = tokens;
                this.backButton = backButton;
                this.safeDigitImages = safeDigitImages;
                this.safeDigitButtons = safeDigitButtons;
            }
        }

        private readonly struct HideViewUi
        {
            public readonly GameObject panel;
            public readonly Image image;
            public readonly Button exitButton;

            public HideViewUi(GameObject panel, Image image, Button exitButton)
            {
                this.panel = panel;
                this.image = image;
                this.exitButton = exitButton;
            }
        }

        private static CloseUpUi CreateCloseUpPanel(RectTransform root)
        {
            var panel = new GameObject("DrawerCloseUpPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root, false);
            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.92f);
            Stretch(panel.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 1f));

            var image = CreateImage("DrawerCloseUpImage", panel.transform, Color.white);
            image.sprite = LoadSprite(DrawerClosedAssetPath);
            image.preserveAspect = true;
            Stretch(image.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f));

            var openButton = CreateTransparentButton("DrawerOpenHotspot", panel.transform);
            Stretch(openButton.GetComponent<RectTransform>(), new Vector2(0.28f, 0.28f), new Vector2(0.7f, 0.62f));

            var itemButton = CreateTransparentButton("DrawerItemHotspot", panel.transform);
            var itemHitbox = itemButton.GetComponent<RectTransform>();
            Stretch(itemHitbox, new Vector2(0.44f, 0.3f), new Vector2(0.66f, 0.54f));

            var closeButton = CreateImageButton("DrawerCloseButton", panel.transform);
            closeButton.GetComponent<Image>().sprite = LoadSprite(BackArrowButtonAssetPath);
            Stretch(closeButton.GetComponent<RectTransform>(), new Vector2(0.02f, 0.86f), new Vector2(0.12f, 0.97f));

            panel.SetActive(false);
            return new CloseUpUi(panel, image, openButton, itemButton, closeButton, itemHitbox);
        }

        private static HideViewUi CreateHideViewPanel(RectTransform root)
        {
            var panel = new GameObject("HideViewPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root, false);
            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.96f);
            Stretch(panel.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 1f));

            var image = CreateImage("HideViewImage", panel.transform, Color.white);
            image.sprite = LoadSprite(ChildBedUnderViewAssetPath);
            image.preserveAspect = true;
            Stretch(image.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f));

            var exitButton = CreateImageButton("HideExitButton", panel.transform);
            exitButton.GetComponent<Image>().sprite = LoadSprite(BackArrowButtonAssetPath);
            Stretch(exitButton.GetComponent<RectTransform>(), new Vector2(0.02f, 0.86f), new Vector2(0.12f, 0.97f));

            panel.SetActive(false);
            return new HideViewUi(panel, image, exitButton);
        }

        private static PuzzleUi CreatePuzzlePanel(RectTransform root)
        {
            var panel = new GameObject("PuzzleCloseUpPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root, false);
            panel.GetComponent<Image>().color = new Color(0.012f, 0.01f, 0.012f, 0.96f);
            Stretch(panel.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 1f));

            var title = CreateText("PuzzleTitle", panel.transform, 26, TextAnchor.MiddleCenter);
            Stretch(title.rectTransform, new Vector2(0.04f, 0.88f), new Vector2(0.96f, 0.98f));
            var image = CreateImage("PuzzleImage", panel.transform, Color.white);
            image.sprite = LoadSprite(PuzzleSampleAssetPaths[0]);
            image.preserveAspect = true;
            Stretch(image.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f));
            var input = CreateText("PuzzleInput", panel.transform, 18, TextAnchor.MiddleLeft);
            Stretch(input.rectTransform, new Vector2(0.66f, 0.74f), new Vector2(0.94f, 0.84f));
            var log = CreateText("PuzzleLog", panel.transform, 16, TextAnchor.UpperLeft);
            Stretch(log.rectTransform, new Vector2(0.66f, 0.62f), new Vector2(0.94f, 0.72f));

            var tokenPanel = new GameObject("PuzzleTokenPanel", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            tokenPanel.transform.SetParent(panel.transform, false);
            tokenPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            Stretch(tokenPanel.GetComponent<RectTransform>(), new Vector2(0.66f, 0.12f), new Vector2(0.94f, 0.6f));
            var layout = tokenPanel.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 5f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            var digitImages = new Image[4];
            var digitButtons = new Button[4];
            var digitRects = new[]
            {
                new Rect(0.374f, 0.282f, 0.056f, 0.104f),
                new Rect(0.445f, 0.282f, 0.047f, 0.104f),
                new Rect(0.503f, 0.282f, 0.048f, 0.104f),
                new Rect(0.566f, 0.282f, 0.047f, 0.104f)
            };

            for (var index = 0; index < digitImages.Length; index++)
            {
                digitImages[index] = CreateImage("StudySafeDigit" + index, panel.transform, Color.white);
                digitImages[index].sprite = LoadSprite(StudySafeDigitAssetPaths[0]);
                digitImages[index].preserveAspect = true;
                digitImages[index].raycastTarget = false;
                Stretch(digitImages[index].rectTransform, digitRects[index].min, digitRects[index].max);

                digitButtons[index] = CreateTransparentButton("StudySafeDigitButton" + index, panel.transform);
                Stretch(digitButtons[index].GetComponent<RectTransform>(), digitRects[index].min, digitRects[index].max);
            }

            var backButton = CreateImageButton("PuzzleBackButton", panel.transform);
            backButton.GetComponent<Image>().sprite = LoadSprite(BackArrowButtonAssetPath);
            Stretch(backButton.GetComponent<RectTransform>(), new Vector2(0.02f, 0.86f), new Vector2(0.12f, 0.97f));

            panel.SetActive(false);
            return new PuzzleUi(panel, title, input, log, image, tokenPanel.GetComponent<RectTransform>(), backButton, digitImages, digitButtons);
        }

        private static CanvasGroup CreateSceneTransitionOverlay(RectTransform root)
        {
            var overlay = new GameObject("SceneTransitionOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            overlay.transform.SetParent(root, false);
            overlay.GetComponent<Image>().color = Color.black;
            Stretch(overlay.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 1f));
            var group = overlay.GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            overlay.SetActive(false);
            return group;
        }

        private static GameObject EnsureInventoryWindowPrefab(RoomSpriteCatalog catalog, bool rebuild)
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(InventoryWindowPrefabPath);
            if (existing != null && !rebuild)
            {
                return existing;
            }

            var root = CreateInventoryWindowPrefab(catalog);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, InventoryWindowPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject CreateInventoryWindowPrefab(RoomSpriteCatalog catalog)
        {
            var root = new GameObject("InventoryWindow", typeof(RectTransform), typeof(Image), typeof(InventoryWindow));
            var rootImage = root.GetComponent<Image>();
            rootImage.sprite = LoadSprite(InventoryPanelAssetPath);
            rootImage.color = Color.white;
            Stretch(root.GetComponent<RectTransform>(), new Vector2(0.68f, 0.12f), new Vector2(0.96f, 0.84f));

            var gridObject = new GameObject("SlotGrid", typeof(RectTransform), typeof(GridLayoutGroup));
            gridObject.transform.SetParent(root.transform, false);
            Stretch(gridObject.GetComponent<RectTransform>(), new Vector2(0.1f, 0.16f), new Vector2(0.9f, 0.86f));
            var grid = gridObject.GetComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.cellSize = new Vector2(92f, 92f);
            grid.spacing = new Vector2(12f, 12f);

            var slots = new InventorySlotView[8];
            for (var index = 0; index < slots.Length; index++)
            {
                var slotObject = new GameObject("InventorySlot" + (index + 1), typeof(RectTransform), typeof(Image), typeof(Button), typeof(InventorySlotView));
                slotObject.transform.SetParent(gridObject.transform, false);
                var frame = slotObject.GetComponent<Image>();
                frame.sprite = LoadSprite(InventorySlotEmptyAssetPath);
                frame.color = Color.white;
                var icon = CreateImage("Icon", slotObject.transform, Color.white);
                icon.raycastTarget = false;
                Stretch(icon.rectTransform, new Vector2(0.14f, 0.14f), new Vector2(0.86f, 0.86f));
                var view = slotObject.GetComponent<InventorySlotView>();
                view.SetReferences(frame, icon, slotObject.GetComponent<Button>());
                slots[index] = view;
            }

            var closeButton = CreateImageButton("CloseButton", root.transform);
            closeButton.GetComponent<Image>().sprite = LoadSprite(InventoryCloseButtonAssetPath);
            Stretch(closeButton.GetComponent<RectTransform>(), new Vector2(0.72f, 0.88f), new Vector2(0.94f, 0.98f));

            root.GetComponent<InventoryWindow>().SetReferences(
                root,
                closeButton,
                slots,
                LoadSprite(InventorySlotEmptyAssetPath),
                LoadSprite(InventorySlotSelectedAssetPath));
            return root;
        }

        private static RoomSpriteCatalog EnsureRoomSpriteCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<RoomSpriteCatalog>(RoomSpriteCatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<RoomSpriteCatalog>();
                AssetDatabase.CreateAsset(catalog, RoomSpriteCatalogPath);
            }

            var entries = new List<SpriteEntry>
            {
                Entry("child_room_north", ChildRoomFaceAssetPaths[0]),
                Entry("child_room_east", ChildRoomFaceAssetPaths[1]),
                Entry("child_room_south", ChildRoomFaceAssetPaths[2]),
                Entry("child_room_west", ChildRoomFaceAssetPaths[3]),
                Entry("second_floor_hallway_north", SecondFloorHallwayFaceAssetPaths[0]),
                Entry("second_floor_hallway_south", SecondFloorHallwayFaceAssetPaths[1]),
                Entry("study_north", StudyFaceAssetPaths[0]),
                Entry("study_east", StudyFaceAssetPaths[1]),
                Entry("study_south", StudyFaceAssetPaths[2]),
                Entry("study_west", StudyFaceAssetPaths[3]),
                Entry("child_room_north_drawer_empty", StateRoomAssetPaths[0]),
                Entry("study_north_safe_open", StateRoomAssetPaths[1]),
                Entry("study_north_safe_open_with_item", StateRoomAssetPaths[2]),
                Entry("study_north_safe_open_empty", StateRoomAssetPaths[3]),
                Entry("study_safe", PuzzleSampleAssetPaths[0]),
                Entry("study_safe_digit_0", StudySafeDigitAssetPaths[0]),
                Entry("study_safe_digit_1", StudySafeDigitAssetPaths[1]),
                Entry("study_safe_digit_2", StudySafeDigitAssetPaths[2]),
                Entry("study_safe_digit_3", StudySafeDigitAssetPaths[3]),
                Entry("study_safe_digit_4", StudySafeDigitAssetPaths[4]),
                Entry("study_safe_digit_5", StudySafeDigitAssetPaths[5]),
                Entry("study_safe_digit_6", StudySafeDigitAssetPaths[6]),
                Entry("study_safe_digit_7", StudySafeDigitAssetPaths[7]),
                Entry("study_safe_digit_8", StudySafeDigitAssetPaths[8]),
                Entry("study_safe_digit_9", StudySafeDigitAssetPaths[9]),
                Entry("child_desk_drawer", ChildRoomObjectAssetPaths[0]),
                Entry("child_room_door", ChildRoomObjectAssetPaths[1]),
                Entry("child_bed_hide", ChildRoomObjectAssetPaths[2]),
                Entry("child_window_silhouette", ChildRoomObjectAssetPaths[3]),
                Entry("item_torn_drawing_fragment", ChildRoomObjectAssetPaths[4]),
                Entry("item_fuse_holder", FuseHolderIconAssetPath),
                Entry("child_desk_drawer_closed", DrawerClosedAssetPath),
                Entry("child_desk_drawer_open_with_item", DrawerOpenWithItemAssetPath),
                Entry("child_desk_drawer_open_empty", DrawerOpenEmptyAssetPath),
                Entry("study_safe_clue_note", StudySafeClueNoteAssetPath),
                Entry("study_safe_locked", CloseUpAssetPaths[4]),
                Entry("study_safe_open_with_item", CloseUpAssetPaths[5]),
                Entry("study_safe_open_empty", CloseUpAssetPaths[6]),
                Entry("child_desk_surface", CloseUpAssetPaths[7]),
                Entry("child_drawing_board", CloseUpAssetPaths[8]),
                Entry("child_window_view", CloseUpAssetPaths[9]),
                Entry("study_safe_surrounding", CloseUpAssetPaths[10]),
                Entry("study_desk_surface", CloseUpAssetPaths[11]),
                Entry("study_clue_board", CloseUpAssetPaths[12]),
                Entry("study_portrait", CloseUpAssetPaths[13]),
                Entry("study_window_view", CloseUpAssetPaths[14]),
                Entry("child_bed_under_view", ChildBedUnderViewAssetPath),
                Entry("inventory_panel_bg", InventoryPanelAssetPath),
                Entry("inventory_button", InventoryButtonAssetPath),
                Entry("inventory_close_button", InventoryCloseButtonAssetPath),
                Entry("inventory_slot_empty", InventorySlotEmptyAssetPath),
                Entry("inventory_slot_selected", InventorySlotSelectedAssetPath),
                Entry("ui_rotate_left", RotateLeftButtonAssetPath),
                Entry("ui_rotate_right", RotateRightButtonAssetPath),
                Entry("ui_hide_exit", HideExitButtonAssetPath),
                Entry("ui_back_arrow", BackArrowButtonAssetPath)
            };

            catalog.SetSprites(entries);
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static SpriteEntry Entry(string id, string assetPath)
        {
            return new SpriteEntry
            {
                spriteId = id,
                sprite = LoadSprite(assetPath)
            };
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
            AddBuildScene(scenes, MainScenePath, 1);
            AddBuildScene(scenes, "Assets/Scenes/SampleScene.unity", 2);
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static GameObject CreateSettingsPanel(Transform parent, out SettingsAudioPanel settingsPanel)
        {
            var panel = new GameObject("SettingsPanel", typeof(RectTransform), typeof(Image), typeof(SettingsAudioPanel));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.sprite = LoadSprite(SettingsPanelBackgroundAssetPath);
            image.color = Color.white;
            Stretch(panel.GetComponent<RectTransform>(), new Vector2(0.28f, 0.18f), new Vector2(0.72f, 0.62f));

            var header = CreateImage("HeaderImage", panel.transform, Color.white);
            header.sprite = LoadSprite(SettingsHeaderAssetPath);
            header.preserveAspect = true;
            Stretch(header.rectTransform, new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.96f));

            var master = CreateSliderRow(panel.transform, "Master", 0.66f, out var masterLabel);
            var bgm = CreateSliderRow(panel.transform, "BGM", 0.5f, out var bgmLabel);
            var sfx = CreateSliderRow(panel.transform, "SFX", 0.34f, out var sfxLabel);
            var ui = CreateSliderRow(panel.transform, "UI", 0.18f, out var uiLabel);

            var closeButton = CreateImageButton("CloseButton", panel.transform);
            closeButton.GetComponent<Image>().sprite = LoadSprite(CloseButtonAssetPath);
            Stretch(closeButton.GetComponent<RectTransform>(), new Vector2(0.36f, 0.04f), new Vector2(0.64f, 0.14f));

            settingsPanel = panel.GetComponent<SettingsAudioPanel>();
            settingsPanel.SetControls(master, bgm, sfx, ui, closeButton);
            settingsPanel.SetVisuals(image, header, masterLabel, bgmLabel, sfxLabel, uiLabel);
            return panel;
        }

        private static Slider CreateSliderRow(Transform parent, string label, float y, out Image labelImage)
        {
            labelImage = CreateImage(label + "LabelImage", parent, Color.white);
            labelImage.sprite = LoadSprite(SettingsLabelPath(label));
            labelImage.preserveAspect = true;
            Stretch(labelImage.rectTransform, new Vector2(0.1f, y), new Vector2(0.28f, y + 0.1f));

            var sliderObject = new GameObject(label + "Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(parent, false);
            Stretch(sliderObject.GetComponent<RectTransform>(), new Vector2(0.32f, y + 0.02f), new Vector2(0.9f, y + 0.08f));

            var background = CreateImage("Background", sliderObject.transform, new Color(0.1f, 0.1f, 0.1f, 1f));
            background.sprite = LoadSprite(SliderTrackAssetPath);
            Stretch(background.rectTransform, Vector2.zero, Vector2.one);
            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            Stretch(fillArea.GetComponent<RectTransform>(), new Vector2(0.03f, 0.25f), new Vector2(0.97f, 0.75f));
            var fill = CreateImage("Fill", fillArea.transform, new Color(0.55f, 0.08f, 0.08f, 1f));
            fill.sprite = LoadSprite(SliderFillAssetPath);
            Stretch(fill.rectTransform, Vector2.zero, Vector2.one);
            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderObject.transform, false);
            Stretch(handleArea.GetComponent<RectTransform>(), new Vector2(0.03f, 0f), new Vector2(0.97f, 1f));
            var handle = CreateImage("Handle", handleArea.transform, new Color(0.86f, 0.82f, 0.74f, 1f));
            handle.sprite = LoadSprite(SliderHandleAssetPath);
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

        private static Text CreateText(string name, Transform parent, int size, TextAnchor anchor)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.color = new Color(0.88f, 0.86f, 0.82f, 1f);
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button CreateImageButton(string name, Transform parent)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = new Color(0.11f, 0.015f, 0.02f, 0.95f);
            return buttonObject.GetComponent<Button>();
        }

        private static Button CreateTransparentButton(string name, Transform parent)
        {
            var button = CreateImageButton(name, parent);
            button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.001f);
            return button;
        }

        private static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Sprite LoadSprite(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite CreateArrowSprite(string id)
        {
            return LoadSprite(InventorySlotSelectedAssetPath);
        }

        private static string SettingsLabelPath(string label)
        {
            switch (label)
            {
                case "Master":
                    return SettingsMasterLabelAssetPath;
                case "BGM":
                    return SettingsBgmLabelAssetPath;
                case "SFX":
                    return SettingsSfxLabelAssetPath;
                default:
                    return SettingsUiLabelAssetPath;
            }
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

        private static void ImportAll(string[] paths)
        {
            foreach (var path in paths)
            {
                AssetDatabase.ImportAsset(path);
                ConfigureSpriteImporter(path);
            }
        }

        private static void ConfigureSpriteImporter(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        private static void EnsurePng(string path, byte[] content, bool rebuild)
        {
            if (!rebuild && File.Exists(path))
            {
                return;
            }

            File.WriteAllBytes(path, content);
        }

        private static void EnsureWav(string path, float frequency, float duration, float amplitude)
        {
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, CreateWav(frequency, duration, amplitude));
            }
        }

        private static void EnsureChildRoomImages()
        {
            var faceNames = new[] { "NORTH WALL", "EAST WALL", "SOUTH WALL", "WEST WALL" };
            var faceColors = new[]
            {
                new Color(0.07f, 0.08f, 0.12f, 1f),
                new Color(0.065f, 0.055f, 0.075f, 1f),
                new Color(0.055f, 0.06f, 0.07f, 1f),
                new Color(0.05f, 0.045f, 0.06f, 1f)
            };

            for (var index = 0; index < ChildRoomFaceAssetPaths.Length; index++)
            {
                if (!File.Exists(ChildRoomFaceAssetPaths[index]))
                {
                    File.WriteAllBytes(ChildRoomFaceAssetPaths[index], CreateRoomFacePng(faceNames[index], faceColors[index], index));
                }
            }

            EnsureObjectPng(ChildRoomObjectAssetPaths[0], new Color(0.34f, 0.2f, 0.13f, 1f), "DESK");
            EnsureObjectPng(ChildRoomObjectAssetPaths[1], new Color(0.22f, 0.12f, 0.08f, 1f), "DOOR");
            EnsureObjectPng(ChildRoomObjectAssetPaths[2], new Color(0.09f, 0.08f, 0.12f, 1f), "BED");
            EnsureObjectPng(ChildRoomObjectAssetPaths[3], new Color(0.02f, 0.02f, 0.025f, 1f), "WINDOW");
            EnsureObjectPng(ChildRoomObjectAssetPaths[4], new Color(0.72f, 0.62f, 0.42f, 1f), "PAPER");
        }

        private static void EnsureSecondFloorHallwayImages()
        {
            if (!File.Exists(SecondFloorHallwayFaceAssetPaths[0]))
            {
                File.WriteAllBytes(SecondFloorHallwayFaceAssetPaths[0], CreateSecondFloorHallwayPng(true));
            }

            if (!File.Exists(SecondFloorHallwayFaceAssetPaths[1]))
            {
                File.WriteAllBytes(SecondFloorHallwayFaceAssetPaths[1], CreateSecondFloorHallwayPng(false));
            }
        }

        private static void EnsureStudyImages()
        {
            var faceNames = new[] { "STUDY SAFE", "STUDY DOOR", "STUDY CLUE", "STUDY WINDOW" };
            var faceColors = new[]
            {
                new Color(0.045f, 0.05f, 0.065f, 1f),
                new Color(0.04f, 0.045f, 0.06f, 1f),
                new Color(0.05f, 0.046f, 0.058f, 1f),
                new Color(0.035f, 0.042f, 0.06f, 1f)
            };

            for (var index = 0; index < StudyFaceAssetPaths.Length; index++)
            {
                if (!File.Exists(StudyFaceAssetPaths[index]))
                {
                    File.WriteAllBytes(StudyFaceAssetPaths[index], CreateRoomFacePng(faceNames[index], faceColors[index], index + 6));
                }
            }

            if (!File.Exists(StudySafeClueNoteAssetPath))
            {
                File.WriteAllBytes(StudySafeClueNoteAssetPath, CreateStudyCluePng());
            }

            EnsureObjectPng(FuseHolderIconAssetPath, new Color(0.35f, 0.28f, 0.18f, 1f), "FUSE");
        }

        private static void EnsureStateRoomImages()
        {
            if (!File.Exists(StateRoomAssetPaths[0]) && File.Exists(ChildRoomFaceAssetPaths[0]))
            {
                File.WriteAllBytes(StateRoomAssetPaths[0], CreateStateRoomPng(ChildRoomFaceAssetPaths[0], new Rect(0.17f, 0.28f, 0.22f, 0.24f)));
            }

            if (!File.Exists(StateRoomAssetPaths[1]) && File.Exists(StudyFaceAssetPaths[0]))
            {
                File.WriteAllBytes(StateRoomAssetPaths[1], CreateStateRoomPng(StudyFaceAssetPaths[0], new Rect(0.49f, 0.20f, 0.33f, 0.52f), true, true));
            }

            if (!File.Exists(StateRoomAssetPaths[2]) && File.Exists(StudyFaceAssetPaths[0]))
            {
                File.WriteAllBytes(StateRoomAssetPaths[2], CreateStateRoomPng(StudyFaceAssetPaths[0], new Rect(0.49f, 0.20f, 0.33f, 0.52f), true, true));
            }

            if (!File.Exists(StateRoomAssetPaths[3]) && File.Exists(StudyFaceAssetPaths[0]))
            {
                File.WriteAllBytes(StateRoomAssetPaths[3], CreateStateRoomPng(StudyFaceAssetPaths[0], new Rect(0.49f, 0.20f, 0.33f, 0.52f), true, false));
            }
        }

        private static void EnsurePuzzleImages()
        {
            for (var index = 0; index < PuzzleSampleAssetPaths.Length; index++)
            {
                if (!File.Exists(PuzzleSampleAssetPaths[index]))
                {
                    var name = Path.GetFileNameWithoutExtension(PuzzleSampleAssetPaths[index]).Replace('_', ' ').ToUpperInvariant();
                    File.WriteAllBytes(PuzzleSampleAssetPaths[index], CreatePuzzlePng(name, index));
                }
            }

            for (var digit = 0; digit < StudySafeDigitAssetPaths.Length; digit++)
            {
                if (!File.Exists(StudySafeDigitAssetPaths[digit]))
                {
                    File.WriteAllBytes(StudySafeDigitAssetPaths[digit], CreateStudySafeDigitPng(digit));
                }
            }
        }

        private static void EnsureCloseUpImages()
        {
            if (!File.Exists(DrawerClosedAssetPath))
            {
                File.WriteAllBytes(DrawerClosedAssetPath, CreateDrawerCloseUpPng(false, false));
            }

            if (!File.Exists(DrawerOpenWithItemAssetPath))
            {
                File.WriteAllBytes(DrawerOpenWithItemAssetPath, CreateDrawerCloseUpPng(true, true));
            }

            if (!File.Exists(DrawerOpenEmptyAssetPath))
            {
                File.WriteAllBytes(DrawerOpenEmptyAssetPath, CreateDrawerCloseUpPng(true, false));
            }

            EnsureDerivedCloseUp(CloseUpAssetPaths[4], PuzzleSampleAssetPaths[0], new Rect(0f, 0f, 1f, 1f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[5], PuzzleSampleAssetPaths[0], new Rect(0f, 0f, 1f, 1f), true, true);
            EnsureDerivedCloseUp(CloseUpAssetPaths[6], PuzzleSampleAssetPaths[0], new Rect(0f, 0f, 1f, 1f), true, false);
            EnsureDerivedCloseUp(CloseUpAssetPaths[7], ChildRoomFaceAssetPaths[0], new Rect(0.08f, 0.25f, 0.36f, 0.36f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[8], ChildRoomFaceAssetPaths[0], new Rect(0.38f, 0.34f, 0.34f, 0.36f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[9], ChildRoomFaceAssetPaths[3], new Rect(0.0f, 0.18f, 0.42f, 0.64f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[10], StudyFaceAssetPaths[0], new Rect(0.38f, 0.14f, 0.46f, 0.74f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[11], StudyFaceAssetPaths[2], new Rect(0.16f, 0.14f, 0.62f, 0.45f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[12], StudyFaceAssetPaths[2], new Rect(0.28f, 0.44f, 0.42f, 0.38f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[13], StudyFaceAssetPaths[3], new Rect(0.22f, 0.36f, 0.32f, 0.42f));
            EnsureDerivedCloseUp(CloseUpAssetPaths[14], StudyFaceAssetPaths[3], new Rect(0f, 0.16f, 0.34f, 0.7f));
        }

        private static void EnsureHideViewImages()
        {
            if (!File.Exists(ChildBedUnderViewAssetPath))
            {
                File.WriteAllBytes(ChildBedUnderViewAssetPath, CreateBedUnderViewPng());
            }
        }

        private static void EnsureMainUiImages(bool rebuild)
        {
            EnsurePng(InventoryPanelAssetPath, CreateInventoryPanelPng(), rebuild);
            EnsurePng(InventoryButtonAssetPath, CreateInventoryButtonPng(), rebuild);
            EnsurePng(InventoryCloseButtonAssetPath, CreateCloseIconPng(), rebuild);
            EnsurePng(InventorySlotEmptyAssetPath, CreateInventorySlotPng(false), rebuild);
            EnsurePng(InventorySlotSelectedAssetPath, CreateInventorySlotPng(true), rebuild);
            EnsurePng(RotateLeftButtonAssetPath, CreateArrowButtonPng(true), rebuild);
            EnsurePng(RotateRightButtonAssetPath, CreateArrowButtonPng(false), rebuild);
            EnsurePng(HideExitButtonAssetPath, CreateExitButtonPng(), rebuild);
            EnsurePng(BackArrowButtonAssetPath, CreateBackArrowButtonPng(), rebuild);
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

        private static byte[] CreateTitleLogoPng()
        {
            var texture = CreateTransparentTexture(1280, 260);
            DrawSoftPanel(texture, 20, 30, 1240, 190, new Color(0.055f, 0.012f, 0.018f, 0.92f), new Color(0.7f, 0.08f, 0.08f, 0.78f));
            DrawPixelTextCentered(texture, "ESCAPE FROM NIGHTMARE", 52, 100, texture.width - 104, 7, new Color(0.9f, 0.82f, 0.68f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateButtonPng(string label, int width, int height)
        {
            var texture = CreateTransparentTexture(width, height);
            DrawSoftPanel(texture, 8, 8, width - 16, height - 16, new Color(0.09f, 0.016f, 0.02f, 0.96f), new Color(0.55f, 0.06f, 0.06f, 0.9f));
            DrawPixelTextCentered(texture, label, 20, height / 2 - 12, width - 40, 4, new Color(0.9f, 0.82f, 0.68f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateSettingsPanelBackgroundPng()
        {
            var texture = CreateTransparentTexture(720, 480);
            DrawSoftPanel(texture, 10, 10, 700, 460, new Color(0.018f, 0.016f, 0.018f, 0.98f), new Color(0.45f, 0.045f, 0.045f, 0.82f));
            for (var y = 28; y < texture.height - 28; y += 24)
            {
                DrawRect(texture, 30, y, texture.width - 60, 1, new Color(0.16f, 0.08f, 0.07f, 0.22f));
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateHeaderPng(string label, int width, int height)
        {
            var texture = CreateTransparentTexture(width, height);
            DrawPixelTextCentered(texture, label, 12, 28, width - 24, 5, new Color(0.88f, 0.78f, 0.62f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateLabelPng(string label, int width, int height)
        {
            var texture = CreateTransparentTexture(width, height);
            DrawPixelText(texture, 8, 18, label, new Color(0.78f, 0.68f, 0.52f, 1f), 4);
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateSliderTrackPng()
        {
            var texture = CreateTransparentTexture(512, 32);
            DrawRect(texture, 0, 11, texture.width, 10, new Color(0.07f, 0.055f, 0.055f, 1f));
            DrawRect(texture, 0, 10, texture.width, 1, new Color(0.42f, 0.28f, 0.2f, 1f));
            DrawRect(texture, 0, 21, texture.width, 1, new Color(0.01f, 0.008f, 0.008f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateSliderFillPng()
        {
            var texture = CreateTransparentTexture(512, 32);
            DrawRect(texture, 0, 11, texture.width, 10, new Color(0.48f, 0.035f, 0.035f, 1f));
            DrawRect(texture, 0, 10, texture.width, 1, new Color(0.82f, 0.24f, 0.18f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateSliderHandlePng()
        {
            var texture = CreateTransparentTexture(48, 64);
            DrawSoftPanel(texture, 12, 6, 24, 52, new Color(0.72f, 0.62f, 0.48f, 1f), new Color(0.95f, 0.82f, 0.62f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateRoomFacePng(string label, Color baseColor, int variant)
        {
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var vertical = y / (float)texture.height;
                    var vignette = Mathf.Abs(x / (float)texture.width - 0.5f) * 0.22f;
                    var noise = ((x * 17 + y * 31 + variant * 53) % 97) / 97f * 0.025f;
                    var color = baseColor + new Color(vignette * 0.25f + noise, vertical * 0.025f, vertical * 0.04f, 0f);
                    texture.SetPixel(x, y, color);
                }
            }

            DrawRect(texture, 0, 0, texture.width, 92, new Color(0.025f, 0.022f, 0.024f, 1f));
            DrawRect(texture, 0, 628, texture.width, 92, new Color(0.018f, 0.016f, 0.018f, 1f));
            DrawRect(texture, 70, 120, 1140, 40, new Color(0.11f, 0.08f, 0.06f, 1f));

            if (variant == 0)
            {
                DrawRect(texture, 165, 360, 360, 200, new Color(0.14f, 0.08f, 0.055f, 1f));
                DrawRect(texture, 230, 280, 210, 80, new Color(0.06f, 0.055f, 0.065f, 1f));
                DrawRect(texture, 745, 210, 260, 175, new Color(0.095f, 0.065f, 0.05f, 1f));
                DrawRect(texture, 890, 470, 235, 98, new Color(0.11f, 0.075f, 0.045f, 1f));
                DrawRect(texture, 1030, 180, 185, 350, new Color(0.045f, 0.047f, 0.047f, 1f));
            }
            else if (variant == 1)
            {
                DrawRect(texture, 715, 110, 330, 500, new Color(0.018f, 0.014f, 0.015f, 1f));
                DrawRect(texture, 720, 118, 24, 486, new Color(0.52f, 0.045f, 0.035f, 1f));
                DrawRect(texture, 1025, 105, 170, 512, new Color(0.13f, 0.09f, 0.065f, 1f));
                DrawRect(texture, 1128, 330, 18, 28, new Color(0.66f, 0.54f, 0.36f, 1f));
                DrawRect(texture, 760, 550, 250, 62, new Color(0.36f, 0.055f, 0.035f, 1f));
            }
            else
            {
                DrawRect(texture, 120 + variant * 60, 205, 240, 220, new Color(0.08f, 0.07f, 0.08f, 1f));
                DrawRect(texture, 900 - variant * 40, 180, 190, 300, new Color(0.04f, 0.035f, 0.045f, 1f));
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateSecondFloorHallwayPng(bool childRoomEnd)
        {
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var vertical = y / (float)texture.height;
                    var centerShade = Mathf.Abs(x / (float)texture.width - 0.5f) * 0.16f;
                    var noise = ((x * 19 + y * 23 + (childRoomEnd ? 17 : 43)) % 89) / 89f * 0.02f;
                    var color = new Color(0.035f + noise, 0.045f + vertical * 0.025f, 0.06f + centerShade, 1f);
                    texture.SetPixel(x, y, color);
                }
            }

            DrawRect(texture, 0, 0, texture.width, 150, new Color(0.026f, 0.022f, 0.021f, 1f));
            DrawRect(texture, 0, 560, texture.width, 160, new Color(0.018f, 0.02f, 0.024f, 1f));
            DrawRect(texture, 0, 250, texture.width, 45, new Color(0.075f, 0.052f, 0.038f, 1f));
            DrawRect(texture, 0, 286, texture.width, 16, new Color(0.035f, 0.029f, 0.027f, 1f));
            DrawRect(texture, 500, 160, 240, 360, new Color(0.05f, 0.03f, 0.028f, 1f));
            DrawRect(texture, 520, 176, 200, 328, new Color(0.11f, 0.025f, 0.022f, 1f));

            if (childRoomEnd)
            {
                DrawDoor(texture, 30, 105, 270, 510, true);
                DrawDoor(texture, 990, 108, 225, 510, false);
                DrawDoor(texture, 845, 190, 90, 300, false);
                DrawDoor(texture, 650, 215, 72, 245, false);
                DrawRect(texture, 64, 114, 190, 420, new Color(0.032f, 0.047f, 0.07f, 1f));
                DrawRect(texture, 85, 170, 115, 145, new Color(0.07f, 0.055f, 0.045f, 1f));
                DrawRect(texture, 420, 90, 220, 185, new Color(0.032f, 0.024f, 0.021f, 1f));
                DrawRect(texture, 450, 120, 18, 145, new Color(0.11f, 0.08f, 0.055f, 1f));
                DrawRect(texture, 600, 120, 18, 145, new Color(0.11f, 0.08f, 0.055f, 1f));
            }
            else
            {
                DrawDoor(texture, 240, 120, 230, 500, false);
                DrawDoor(texture, 680, 210, 105, 300, false);
                DrawRect(texture, 830, 80, 310, 260, new Color(0.01f, 0.01f, 0.012f, 1f));
                DrawRect(texture, 825, 330, 330, 34, new Color(0.08f, 0.06f, 0.045f, 1f));
                DrawRect(texture, 925, 365, 12, 180, new Color(0.11f, 0.08f, 0.055f, 1f));
                DrawRect(texture, 1045, 365, 12, 180, new Color(0.11f, 0.08f, 0.055f, 1f));
                DrawRect(texture, 935, 385, 185, 120, new Color(0.025f, 0.036f, 0.055f, 1f));
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static void DrawDoor(Texture2D texture, int x, int y, int width, int height, bool open)
        {
            DrawRect(texture, x, y, width, height, new Color(0.038f, 0.028f, 0.024f, 1f));
            DrawRect(texture, x + 10, y + 10, width - 20, height - 20, open ? new Color(0.018f, 0.021f, 0.026f, 1f) : new Color(0.105f, 0.075f, 0.052f, 1f));
            DrawRect(texture, x + width - 42, y + height / 2, 16, 18, new Color(0.55f, 0.44f, 0.28f, 1f));
            if (!open)
            {
                DrawRect(texture, x + 35, y + height / 2 + 60, width - 70, 24, new Color(0.065f, 0.045f, 0.034f, 1f));
                DrawRect(texture, x + 35, y + height / 2 - 95, width - 70, 24, new Color(0.065f, 0.045f, 0.034f, 1f));
            }
        }

        private static void EnsureObjectPng(string path, Color color, string label)
        {
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, CreateObjectPng(color, label));
            }
        }

        private static byte[] CreateObjectPng(Color color, string label)
        {
            var texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            var transparent = new Color(0f, 0f, 0f, 0f);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            DrawRect(texture, 96, 132, 320, 210, color);
            DrawRect(texture, 120, 156, 272, 34, color * 0.65f + new Color(0f, 0f, 0f, 0.35f));
            DrawRect(texture, 120, 218, 272, 34, color * 0.75f + new Color(0f, 0f, 0f, 0.25f));
            DrawRect(texture, 120, 280, 272, 34, color * 0.85f + new Color(0f, 0f, 0f, 0.15f));
            DrawText(texture, 112, 370, label, new Color(0.9f, 0.82f, 0.68f, 1f), 3);
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreatePuzzlePng(string label, int variant)
        {
            var texture = new Texture2D(900, 620, TextureFormat.RGBA32, false);
            var baseColor = new Color(0.035f + variant * 0.004f, 0.03f, 0.034f, 1f);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, baseColor + new Color(y / (float)texture.height * 0.03f, 0f, 0.02f, 0f));
                }
            }

            DrawRect(texture, 70, 80, 760, 420, new Color(0.11f, 0.09f, 0.08f, 1f));
            DrawRect(texture, 110, 130, 680, 90, new Color(0.025f, 0.023f, 0.026f, 1f));
            for (var index = 0; index < 4; index++)
            {
                DrawRect(texture, 145 + index * 150, 285, 96, 96, new Color(0.22f, 0.18f, 0.15f, 1f));
            }

            DrawText(texture, 104, 520, label, new Color(0.75f, 0.66f, 0.5f, 1f), 3);
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateStudySafeDigitPng(int digit)
        {
            var texture = CreateTransparentTexture(180, 220);
            var rng = new System.Random(97 + digit);
            DrawMetalDigitLayer(texture, digit.ToString(), 12, 36, 20, new Color(0.02f, 0.015f, 0.01f, 0.72f));
            DrawMetalDigitLayer(texture, digit.ToString(), 10, 34, 20, new Color(0.055f, 0.038f, 0.018f, 1f));
            DrawMetalDigitLayer(texture, digit.ToString(), 8, 30, 20, new Color(0.36f, 0.25f, 0.11f, 1f));
            DrawMetalDigitLayer(texture, digit.ToString(), 6, 26, 20, new Color(0.58f, 0.43f, 0.2f, 1f));
            AddMetalDigitPatina(texture, rng);
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static void DrawMetalDigitLayer(Texture2D texture, string digit, int x, int y, int scale, Color color)
        {
            DrawPixelTextCentered(texture, digit, x, y, texture.width - x * 2, scale, color);
        }

        private static void AddMetalDigitPatina(Texture2D texture, System.Random rng)
        {
            for (var index = 0; index < 210; index++)
            {
                var x = rng.Next(0, texture.width);
                var y = rng.Next(0, texture.height);
                var color = texture.GetPixel(x, y);
                if (color.a <= 0.01f)
                {
                    continue;
                }

                var darken = rng.Next(8, 32) / 100f;
                texture.SetPixel(x, y, new Color(
                    Mathf.Max(0f, color.r - darken),
                    Mathf.Max(0f, color.g - darken),
                    Mathf.Max(0f, color.b - darken),
                    color.a));
            }

            for (var index = 0; index < 34; index++)
            {
                var x = rng.Next(8, texture.width - 24);
                var y = rng.Next(12, texture.height - 12);
                var length = rng.Next(8, 32);
                for (var offset = 0; offset < length; offset++)
                {
                    var px = Mathf.Clamp(x + offset, 0, texture.width - 1);
                    var py = Mathf.Clamp(y + offset / 5, 0, texture.height - 1);
                    var color = texture.GetPixel(px, py);
                    if (color.a > 0.01f)
                    {
                        texture.SetPixel(px, py, new Color(0.08f, 0.055f, 0.025f, color.a));
                    }
                }
            }
        }

        private static void EnsureDerivedCloseUp(string destinationPath, string sourcePath, Rect sourceRect, bool safeOpen = false, bool withItem = false)
        {
            if (File.Exists(destinationPath) || !File.Exists(sourcePath))
            {
                return;
            }

            File.WriteAllBytes(destinationPath, CreateCropPng(sourcePath, sourceRect, 1280, 720, safeOpen, withItem));
        }

        private static byte[] CreateStateRoomPng(string sourcePath, Rect changedArea, bool safeOpen = false, bool withItem = false)
        {
            var texture = LoadTextureFromPng(sourcePath);
            if (texture == null)
            {
                return CreateRoomFacePng("STATE", new Color(0.035f, 0.035f, 0.04f, 1f), 12);
            }

            var x = Mathf.RoundToInt(changedArea.x * texture.width);
            var y = Mathf.RoundToInt(changedArea.y * texture.height);
            var width = Mathf.RoundToInt(changedArea.width * texture.width);
            var height = Mathf.RoundToInt(changedArea.height * texture.height);
            if (safeOpen)
            {
                DrawOpenSafe(texture, x, y, width, height, withItem);
            }
            else
            {
                DrawRect(texture, x, y, width, height, new Color(0.015f, 0.012f, 0.012f, 0.86f));
                DrawRect(texture, x + 8, y + 8, Mathf.Max(1, width - 16), Mathf.Max(1, height - 16), new Color(0.05f, 0.039f, 0.032f, 0.82f));
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateCropPng(string sourcePath, Rect normalizedRect, int targetWidth, int targetHeight, bool safeOpen, bool withItem)
        {
            var source = LoadTextureFromPng(sourcePath);
            if (source == null)
            {
                return CreateObjectPng(new Color(0.08f, 0.07f, 0.065f, 1f), string.Empty);
            }

            var texture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
            var sourceX = Mathf.RoundToInt(normalizedRect.x * source.width);
            var sourceY = Mathf.RoundToInt(normalizedRect.y * source.height);
            var sourceWidth = Mathf.Max(1, Mathf.RoundToInt(normalizedRect.width * source.width));
            var sourceHeight = Mathf.Max(1, Mathf.RoundToInt(normalizedRect.height * source.height));

            for (var y = 0; y < targetHeight; y++)
            {
                for (var x = 0; x < targetWidth; x++)
                {
                    var u = x / (float)Mathf.Max(1, targetWidth - 1);
                    var v = y / (float)Mathf.Max(1, targetHeight - 1);
                    var sx = Mathf.Clamp(sourceX + Mathf.RoundToInt(u * sourceWidth), 0, source.width - 1);
                    var sy = Mathf.Clamp(sourceY + Mathf.RoundToInt(v * sourceHeight), 0, source.height - 1);
                    texture.SetPixel(x, y, source.GetPixel(sx, sy));
                }
            }

            if (safeOpen)
            {
                DrawOpenSafe(texture, 350, 160, 650, 430, withItem);
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(source);
            return png;
        }

        private static void DrawOpenSafe(Texture2D texture, int x, int y, int width, int height, bool withItem)
        {
            DrawAgedMetalRect(texture, x, y, width, height, new Color(0.026f, 0.023f, 0.022f, 1f), 31);
            DrawRect(texture, x + 10, y + 10, Mathf.Max(1, width - 20), 2, new Color(0.28f, 0.19f, 0.1f, 0.72f));
            DrawRect(texture, x + 10, y + height - 12, Mathf.Max(1, width - 20), 2, new Color(0.006f, 0.005f, 0.005f, 0.9f));
            DrawRect(texture, x + 10, y + 10, 2, Mathf.Max(1, height - 20), new Color(0.22f, 0.15f, 0.08f, 0.62f));

            var cavityX = x + width / 11;
            var cavityY = y + height / 7;
            var cavityWidth = width * 50 / 100;
            var cavityHeight = height * 2 / 3;
            DrawAgedMetalRect(texture, cavityX, cavityY, cavityWidth, cavityHeight, new Color(0.02f, 0.018f, 0.017f, 1f), 47);
            DrawRect(texture, cavityX + 12, cavityY + 12, Mathf.Max(1, cavityWidth - 24), Mathf.Max(1, cavityHeight - 24), new Color(0.004f, 0.004f, 0.004f, 0.96f));
            DrawAgedMetalRect(texture, cavityX + 22, cavityY + 24, Mathf.Max(1, cavityWidth - 44), Mathf.Max(1, cavityHeight - 48), new Color(0.011f, 0.01f, 0.009f, 1f), 59);
            DrawRect(texture, cavityX + 22, cavityY + cavityHeight / 2, Mathf.Max(1, cavityWidth - 44), 2, new Color(0.15f, 0.11f, 0.065f, 0.58f));

            var doorX = x + width * 58 / 100;
            var doorY = y + height / 20;
            var doorWidth = width * 37 / 100;
            var doorHeight = height * 9 / 10;
            DrawRect(texture, doorX - 8, doorY + 12, 10, Mathf.Max(1, doorHeight - 24), new Color(0.005f, 0.004f, 0.004f, 0.86f));
            DrawAgedMetalRect(texture, doorX, doorY, doorWidth, doorHeight, new Color(0.036f, 0.034f, 0.033f, 1f), 73);
            DrawRect(texture, doorX + 14, doorY + 18, Mathf.Max(1, doorWidth - 28), 2, new Color(0.32f, 0.21f, 0.11f, 0.62f));
            DrawRect(texture, doorX + 14, doorY + doorHeight - 20, Mathf.Max(1, doorWidth - 28), 2, new Color(0.004f, 0.004f, 0.004f, 0.82f));
            DrawRect(texture, doorX + doorWidth - 24, doorY + 24, 8, Mathf.Max(1, doorHeight - 48), new Color(0.12f, 0.09f, 0.055f, 0.9f));
            DrawRect(texture, doorX + 34, doorY + doorHeight / 2 - 15, 28, 30, new Color(0.25f, 0.18f, 0.095f, 1f));

            if (withItem)
            {
                var itemX = cavityX + cavityWidth / 4;
                var itemY = cavityY + cavityHeight * 62 / 100;
                var itemWidth = cavityWidth / 4;
                var itemHeight = Mathf.Max(10, cavityHeight / 9);
                DrawAgedMetalRect(texture, itemX, itemY, itemWidth, itemHeight, new Color(0.34f, 0.24f, 0.12f, 1f), 89);
                DrawRect(texture, itemX + 4, itemY + itemHeight / 3, Mathf.Max(1, itemWidth - 8), itemHeight / 3, new Color(0.075f, 0.052f, 0.03f, 0.88f));
                DrawRect(texture, itemX + itemWidth / 5, itemY - 4, Mathf.Max(1, itemWidth * 3 / 5), 4, new Color(0.55f, 0.39f, 0.18f, 0.8f));
            }
        }

        private static void DrawAgedMetalRect(Texture2D texture, int x, int y, int width, int height, Color baseColor, int seed)
        {
            for (var py = Mathf.Max(0, y); py < Mathf.Min(texture.height, y + height); py++)
            {
                for (var px = Mathf.Max(0, x); px < Mathf.Min(texture.width, x + width); px++)
                {
                    var u = (px - x) / (float)Mathf.Max(1, width);
                    var v = (py - y) / (float)Mathf.Max(1, height);
                    var noise = ((px * 17 + py * 31 + seed * 43) % 101) / 101f;
                    var edge = Mathf.Max(Mathf.Abs(u - 0.5f), Mathf.Abs(v - 0.5f)) * 0.08f;
                    var warmWear = noise > 0.86f ? 0.075f : 0f;
                    texture.SetPixel(px, py, new Color(
                        Mathf.Clamp01(baseColor.r + noise * 0.028f + warmWear - edge),
                        Mathf.Clamp01(baseColor.g + noise * 0.022f + warmWear * 0.58f - edge),
                        Mathf.Clamp01(baseColor.b + noise * 0.018f + warmWear * 0.25f - edge),
                        baseColor.a));
                }
            }

            for (var index = 0; index < Mathf.Max(8, (width + height) / 28); index++)
            {
                var sx = x + Mathf.Abs((seed * 23 + index * 47) % Mathf.Max(1, width));
                var sy = y + Mathf.Abs((seed * 31 + index * 29) % Mathf.Max(1, height));
                var length = 8 + Mathf.Abs((seed + index * 13) % 28);
                for (var offset = 0; offset < length; offset++)
                {
                    var px = Mathf.Clamp(sx + offset, 0, texture.width - 1);
                    var py = Mathf.Clamp(sy + offset / 5, 0, texture.height - 1);
                    var color = texture.GetPixel(px, py);
                    if (color.a > 0.01f)
                    {
                        texture.SetPixel(px, py, new Color(0.055f, 0.04f, 0.022f, color.a));
                    }
                }
            }
        }

        private static Texture2D LoadTextureFromPng(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            return texture.LoadImage(File.ReadAllBytes(path)) ? texture : null;
        }

        private static byte[] CreateStudyCluePng()
        {
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var noise = ((x * 13 + y * 29) % 83) / 83f * 0.025f;
                    texture.SetPixel(x, y, new Color(0.095f + noise, 0.075f + noise, 0.055f, 1f));
                }
            }

            DrawRect(texture, 80, 70, 1120, 580, new Color(0.12f, 0.085f, 0.06f, 1f));
            var digits = new[] { "3", "1", "4", "2" };
            for (var index = 0; index < digits.Length; index++)
            {
                var x = 170 + index * 250;
                DrawRect(texture, x, 150, 190, 380, new Color(0.58f, 0.52f, 0.42f, 1f));
                DrawRect(texture, x + 14, 164, 162, 352, new Color(0.47f, 0.42f, 0.34f, 1f));
                DrawText(texture, x + 72, 300, digits[index], new Color(0.12f, 0.09f, 0.07f, 1f), 8);
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateDrawerCloseUpPng(bool open, bool withItem)
        {
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var vertical = y / (float)texture.height;
                    texture.SetPixel(x, y, new Color(0.035f + vertical * 0.04f, 0.032f, 0.038f + vertical * 0.02f, 1f));
                }
            }

            DrawRect(texture, 0, 0, texture.width, 110, new Color(0.055f, 0.035f, 0.03f, 1f));
            DrawRect(texture, 120, 145, 1040, 430, new Color(0.17f, 0.11f, 0.075f, 1f));
            DrawRect(texture, 150, 170, 980, 55, new Color(0.12f, 0.08f, 0.06f, 1f));
            if (open)
            {
                DrawRect(texture, 310, 260, 650, 250, new Color(0.09f, 0.06f, 0.045f, 1f));
                DrawRect(texture, 270, 470, 730, 88, new Color(0.2f, 0.13f, 0.09f, 1f));
                DrawRect(texture, 570, 502, 140, 18, new Color(0.55f, 0.36f, 0.17f, 1f));
                if (withItem)
                {
                    DrawRect(texture, 545, 330, 190, 110, new Color(0.72f, 0.62f, 0.42f, 1f));
                    DrawRect(texture, 585, 362, 70, 4, new Color(0.42f, 0.1f, 0.08f, 1f));
                    DrawRect(texture, 585, 382, 90, 4, new Color(0.1f, 0.2f, 0.12f, 1f));
                }
            }
            else
            {
                DrawRect(texture, 365, 285, 550, 180, new Color(0.19f, 0.12f, 0.085f, 1f));
                DrawRect(texture, 405, 325, 470, 100, new Color(0.14f, 0.09f, 0.065f, 1f));
                DrawRect(texture, 585, 378, 110, 18, new Color(0.58f, 0.38f, 0.18f, 1f));
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateBedUnderViewPng()
        {
            var texture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var floorGlow = 1f - y / (float)texture.height;
                    var vignette = Mathf.Abs(x / (float)texture.width - 0.5f) * 0.16f;
                    texture.SetPixel(x, y, new Color(0.018f + floorGlow * 0.035f + vignette, 0.018f + floorGlow * 0.025f, 0.024f + floorGlow * 0.055f, 1f));
                }
            }

            DrawRect(texture, 0, 555, texture.width, 165, new Color(0.01f, 0.008f, 0.008f, 1f));
            DrawRect(texture, 0, 0, texture.width, 95, new Color(0.045f, 0.032f, 0.028f, 1f));
            DrawRect(texture, 90, 88, 1090, 16, new Color(0.12f, 0.07f, 0.05f, 1f));
            DrawRect(texture, 165, 200, 180, 250, new Color(0.055f, 0.05f, 0.06f, 1f));
            DrawRect(texture, 850, 170, 260, 320, new Color(0.04f, 0.035f, 0.04f, 1f));
            DrawRect(texture, 140, 112, 1000, 12, new Color(0.16f, 0.1f, 0.07f, 1f));
            DrawRect(texture, 0, 600, texture.width, 120, new Color(0f, 0f, 0f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateInventoryPanelPng()
        {
            var texture = CreateTransparentTexture(420, 620);
            DrawSoftPanel(texture, 10, 10, 400, 600, new Color(0.018f, 0.016f, 0.018f, 0.96f), new Color(0.55f, 0.08f, 0.07f, 0.88f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateInventoryButtonPng()
        {
            var texture = CreateTransparentTexture(180, 120);
            DrawSoftPanel(texture, 8, 8, 164, 104, new Color(0.018f, 0.016f, 0.015f, 0.98f), new Color(0.92f, 0.78f, 0.44f, 1f));
            DrawRect(texture, 58, 35, 64, 48, new Color(0.62f, 0.47f, 0.27f, 1f));
            DrawRect(texture, 65, 24, 50, 20, new Color(0.34f, 0.24f, 0.16f, 1f));
            DrawRect(texture, 66, 43, 48, 3, new Color(0.92f, 0.78f, 0.44f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateArrowButtonPng(bool left)
        {
            var texture = CreateTransparentTexture(128, 160);
            DrawSoftPanel(texture, 8, 8, 112, 144, new Color(0.012f, 0.011f, 0.012f, 0.96f), new Color(0.9f, 0.78f, 0.48f, 1f));
            var color = new Color(0.95f, 0.86f, 0.58f, 1f);
            for (var row = 0; row < 58; row++)
            {
                var width = row < 29 ? row + 1 : 58 - row;
                var y = 51 + row;
                var x = left ? 37 + row / 2 : 91 - row / 2 - width;
                DrawRect(texture, x, y, width, 2, color);
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateExitButtonPng()
        {
            var texture = CreateTransparentTexture(220, 110);
            DrawSoftPanel(texture, 8, 8, 204, 94, new Color(0.012f, 0.011f, 0.012f, 0.98f), new Color(0.9f, 0.78f, 0.48f, 1f));
            DrawPixelTextCentered(texture, "EXIT", 18, 40, 184, 5, new Color(0.95f, 0.86f, 0.58f, 1f));
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateCloseIconPng()
        {
            var texture = CreateTransparentTexture(96, 96);
            DrawSoftPanel(texture, 8, 8, 80, 80, new Color(0.08f, 0.02f, 0.024f, 0.95f), new Color(0.62f, 0.08f, 0.08f, 0.92f));
            for (var index = 0; index < 8; index++)
            {
                DrawRect(texture, 28 + index * 5, 28 + index * 5, 6, 6, new Color(0.9f, 0.82f, 0.68f, 1f));
                DrawRect(texture, 62 - index * 5, 28 + index * 5, 6, 6, new Color(0.9f, 0.82f, 0.68f, 1f));
            }

            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateInventorySlotPng(bool selected)
        {
            var texture = CreateTransparentTexture(128, 128);
            var border = selected ? new Color(0.86f, 0.68f, 0.28f, 0.95f) : new Color(0.48f, 0.39f, 0.28f, 0.86f);
            DrawSoftPanel(texture, 8, 8, 112, 112, new Color(0.045f, 0.038f, 0.034f, 0.92f), border);
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static byte[] CreateBackArrowButtonPng()
        {
            var texture = CreateTransparentTexture(140, 120);
            DrawSoftPanel(texture, 8, 8, 124, 104, new Color(0.012f, 0.011f, 0.012f, 0.96f), new Color(0.9f, 0.78f, 0.48f, 1f));
            var color = new Color(0.95f, 0.86f, 0.58f, 1f);
            for (var row = 0; row < 44; row++)
            {
                var width = row < 22 ? row + 1 : 44 - row;
                DrawRect(texture, 34 + row / 2, 38 + row, width, 2, color);
            }

            DrawRect(texture, 58, 58, 46, 6, color);
            texture.Apply();
            var png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (var py = Mathf.Max(0, y); py < Mathf.Min(texture.height, y + height); py++)
            {
                for (var px = Mathf.Max(0, x); px < Mathf.Min(texture.width, x + width); px++)
                {
                    texture.SetPixel(px, py, color);
                }
            }
        }

        private static Texture2D CreateTransparentTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color[width * height];
            for (var index = 0; index < pixels.Length; index++)
            {
                pixels[index] = Color.clear;
            }

            texture.SetPixels(pixels);
            return texture;
        }

        private static void DrawSoftPanel(Texture2D texture, int x, int y, int width, int height, Color fill, Color border)
        {
            DrawRect(texture, x, y, width, height, fill);
            DrawRect(texture, x, y, width, 3, border);
            DrawRect(texture, x, y + height - 3, width, 3, border);
            DrawRect(texture, x, y, 3, height, border);
            DrawRect(texture, x + width - 3, y, 3, height, border);
            DrawRect(texture, x + 10, y + 10, width - 20, 1, border * 0.7f);
            DrawRect(texture, x + 10, y + height - 12, width - 20, 1, new Color(0f, 0f, 0f, 0.45f));
        }

        private static void DrawText(Texture2D texture, int x, int y, string text, Color color, int scale)
        {
            DrawPixelText(texture, x, y, text, color, scale);
        }

        private static void DrawPixelTextCentered(Texture2D texture, string text, int x, int y, int width, int scale, Color color)
        {
            var textWidth = GetPixelTextWidth(text, scale);
            DrawPixelText(texture, x + Mathf.Max(0, (width - textWidth) / 2), y, text, color, scale);
        }

        private static int GetPixelTextWidth(string text, int scale)
        {
            return Mathf.Max(0, text.Length * 6 * scale - scale);
        }

        private static void DrawPixelText(Texture2D texture, int x, int y, string text, Color color, int scale)
        {
            for (var index = 0; index < text.Length; index++)
            {
                DrawGlyph(texture, x + index * 6 * scale, y, char.ToUpperInvariant(text[index]), color, scale);
            }
        }

        private static void DrawGlyph(Texture2D texture, int x, int y, char glyph, Color color, int scale)
        {
            if (glyph == ' ')
            {
                return;
            }

            var pattern = GetGlyphPattern(glyph);
            for (var gy = 0; gy < 7; gy++)
            {
                for (var gx = 0; gx < 5; gx++)
                {
                    if (pattern[gy][gx] != '1')
                    {
                        continue;
                    }

                    DrawRect(texture, x + gx * scale, y + gy * scale, scale, scale, color);
                }
            }
        }

        private static string[] GetGlyphPattern(char glyph)
        {
            return Glyphs.TryGetValue(glyph, out var pattern) ? pattern : Glyphs['?'];
        }

        private static readonly Dictionary<char, string[]> Glyphs = new Dictionary<char, string[]>
        {
            { '?', new[] { "11110", "00001", "00001", "00110", "00100", "00000", "00100" } },
            { '0', new[] { "01110", "10001", "10011", "10101", "11001", "10001", "01110" } },
            { '1', new[] { "00100", "01100", "00100", "00100", "00100", "00100", "01110" } },
            { '2', new[] { "01110", "10001", "00001", "00010", "00100", "01000", "11111" } },
            { '3', new[] { "11110", "00001", "00001", "01110", "00001", "00001", "11110" } },
            { '4', new[] { "10010", "10010", "10010", "11111", "00010", "00010", "00010" } },
            { '5', new[] { "11111", "10000", "10000", "11110", "00001", "00001", "11110" } },
            { '6', new[] { "01111", "10000", "10000", "11110", "10001", "10001", "01110" } },
            { '7', new[] { "11111", "00001", "00010", "00100", "01000", "01000", "01000" } },
            { '8', new[] { "01110", "10001", "10001", "01110", "10001", "10001", "01110" } },
            { '9', new[] { "01110", "10001", "10001", "01111", "00001", "00001", "11110" } },
            { 'A', new[] { "01110", "10001", "10001", "11111", "10001", "10001", "10001" } },
            { 'B', new[] { "11110", "10001", "10001", "11110", "10001", "10001", "11110" } },
            { 'C', new[] { "01111", "10000", "10000", "10000", "10000", "10000", "01111" } },
            { 'D', new[] { "11110", "10001", "10001", "10001", "10001", "10001", "11110" } },
            { 'E', new[] { "11111", "10000", "10000", "11110", "10000", "10000", "11111" } },
            { 'F', new[] { "11111", "10000", "10000", "11110", "10000", "10000", "10000" } },
            { 'G', new[] { "01111", "10000", "10000", "10011", "10001", "10001", "01111" } },
            { 'H', new[] { "10001", "10001", "10001", "11111", "10001", "10001", "10001" } },
            { 'I', new[] { "11111", "00100", "00100", "00100", "00100", "00100", "11111" } },
            { 'J', new[] { "00111", "00010", "00010", "00010", "00010", "10010", "01100" } },
            { 'K', new[] { "10001", "10010", "10100", "11000", "10100", "10010", "10001" } },
            { 'L', new[] { "10000", "10000", "10000", "10000", "10000", "10000", "11111" } },
            { 'M', new[] { "10001", "11011", "10101", "10101", "10001", "10001", "10001" } },
            { 'N', new[] { "10001", "11001", "10101", "10011", "10001", "10001", "10001" } },
            { 'O', new[] { "01110", "10001", "10001", "10001", "10001", "10001", "01110" } },
            { 'P', new[] { "11110", "10001", "10001", "11110", "10000", "10000", "10000" } },
            { 'Q', new[] { "01110", "10001", "10001", "10001", "10101", "10010", "01101" } },
            { 'R', new[] { "11110", "10001", "10001", "11110", "10100", "10010", "10001" } },
            { 'S', new[] { "01111", "10000", "10000", "01110", "00001", "00001", "11110" } },
            { 'T', new[] { "11111", "00100", "00100", "00100", "00100", "00100", "00100" } },
            { 'U', new[] { "10001", "10001", "10001", "10001", "10001", "10001", "01110" } },
            { 'V', new[] { "10001", "10001", "10001", "10001", "10001", "01010", "00100" } },
            { 'W', new[] { "10001", "10001", "10001", "10101", "10101", "10101", "01010" } },
            { 'X', new[] { "10001", "10001", "01010", "00100", "01010", "10001", "10001" } },
            { 'Y', new[] { "10001", "10001", "01010", "00100", "00100", "00100", "00100" } },
            { 'Z', new[] { "11111", "00001", "00010", "00100", "01000", "10000", "11111" } }
        };

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
