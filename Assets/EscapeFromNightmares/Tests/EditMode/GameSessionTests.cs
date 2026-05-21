using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.Services;
using NUnit.Framework;
using UnityEngine;

namespace EscapeFromNightmares.Tests.EditMode
{
    public sealed class GameSessionTests
    {
        [Test]
        public void Stage1Data_HasStartRoomAndRequiredPuzzles()
        {
            var stage = RuntimeStageFactory.CreateStage1();

            Assert.That(stage.startRoomId, Is.EqualTo("child_room"));
            Assert.That(stage.rooms.Any(room => room.roomId == "altar_room"), Is.True);
            Assert.That(stage.puzzles.Any(puzzle => puzzle.puzzleId == "front_door_escape"), Is.True);
        }

        [Test]
        public void Inventory_AcquireItem_AddsItOnce()
        {
            var session = new GameSession();
            session.Start(RuntimeStageFactory.CreateStage1());

            Assert.That(session.AddItem("fuse"), Is.True);
            Assert.That(session.AddItem("fuse"), Is.False);
            Assert.That(session.HasItem("fuse"), Is.True);
        }

        [Test]
        public void Puzzle_TrySolve_GrantsRewardAndFlag()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");

            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasItem("fuse_holder"), Is.True);
            Assert.That(session.HasFlag("puzzle_study_safe_clear"), Is.True);
        }

        [Test]
        public void InteractionSystem_DoorInteraction_ReturnsMoveRoom()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var door = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.targetRoomId == "second_floor_hallway");

            var result = service.Resolve(door);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.MoveRoom));
            Assert.That(result.Value, Is.EqualTo("second_floor_hallway"));
        }

        [Test]
        public void SettingsSaveService_SavesSettingsAndClearRecordsOnly()
        {
            var directory = Path.Combine(Path.GetTempPath(), "EscapeFromNightmaresTests");
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            var service = new SettingsSaveService(directory);
            service.SaveSettings(new SettingsSaveService.SettingsData { masterVolume = 0.9f, bgmVolume = 0.25f, sfxVolume = 0.5f, uiVolume = 0.75f });
            service.SaveClearRecords(new SettingsSaveService.ClearRecordsData { stage1Clear = true });

            Assert.That(service.LoadSettings().masterVolume, Is.EqualTo(0.9f).Within(0.001f));
            Assert.That(service.LoadSettings().bgmVolume, Is.EqualTo(0.25f).Within(0.001f));
            Assert.That(service.LoadSettings().sfxVolume, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(service.LoadSettings().uiVolume, Is.EqualTo(0.75f).Within(0.001f));
            Assert.That(service.LoadClearRecords().stage1Clear, Is.True);
            Assert.That(File.Exists(Path.Combine(directory, "settings.json")), Is.True);
            Assert.That(File.Exists(Path.Combine(directory, "clear_records.json")), Is.True);
        }

        [Test]
        public void ResourcePathCatalog_CreateDefault_UsesExpectedResourcesPaths()
        {
            var catalog = ResourcePathCatalog.CreateDefault();

            Assert.That(catalog.titleBackgroundPath, Is.EqualTo("EscapeFromNightmares/Title/title_background"));
            Assert.That(catalog.titleBgmPath, Is.EqualTo("EscapeFromNightmares/Audio/BGM/title_loop"));
            Assert.That(catalog.uiClickPath, Is.EqualTo("EscapeFromNightmares/Audio/UI/ui_click"));
            Assert.That(catalog.confirmSfxPath, Is.EqualTo("EscapeFromNightmares/Audio/SFX/sfx_confirm"));
        }

        [Test]
        public void ResourceManager_MissingSpritePath_ReturnsFallbackSprite()
        {
            var manager = new ResourceManager(ResourcePathCatalog.CreateDefault());

            Assert.That(manager.LoadSprite("missing/path/for/test"), Is.Not.Null);
            Assert.That(manager.LoadAudioClip("missing/path/for/test"), Is.Null);
        }

        [Test]
        public void SoundManager_VolumeToDecibels_MapsRange()
        {
            Assert.That(SoundManager.VolumeToDecibels(1f), Is.EqualTo(0f).Within(0.001f));
            Assert.That(SoundManager.VolumeToDecibels(0f), Is.EqualTo(-80f).Within(0.001f));
            Assert.That(SoundManager.VolumeToDecibels(0.5f), Is.EqualTo(-6.0206f).Within(0.001f));
        }

        [Test]
        public void SoundManager_ApplyVolumesWithoutMixer_UsesAudioSourceFallback()
        {
            var gameObject = new GameObject("SoundManagerTest");
            try
            {
                var manager = gameObject.AddComponent<SoundManager>();
                manager.Initialize(new ResourceManager(ResourcePathCatalog.CreateDefault()));

                Assert.DoesNotThrow(() => manager.ApplyVolumes(new SettingsSaveService.SettingsData
                {
                    masterVolume = 0.5f,
                    bgmVolume = 0.5f,
                    sfxVolume = 0.25f,
                    uiVolume = 0.75f
                }));
                Assert.That(gameObject.GetComponentsInChildren<AudioSource>().Length, Is.EqualTo(3));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }
    }
}
