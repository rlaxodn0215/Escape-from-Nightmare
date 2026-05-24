using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.Services;
using EscapeFromNightmares.UI;
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
        public void Stage1Data_RoomsUseExpectedDirectionalFaces()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var expectedDirections = new[]
            {
                RoomFaceDirection.North,
                RoomFaceDirection.East,
                RoomFaceDirection.South,
                RoomFaceDirection.West
            };

            foreach (var room in stage.rooms)
            {
                if (room.roomId == "second_floor_hallway")
                {
                    Assert.That(room.faces, Has.Length.EqualTo(2), room.roomId);
                    Assert.That(room.faces.Select(face => face.direction), Is.EquivalentTo(new[] { RoomFaceDirection.North, RoomFaceDirection.South }), room.roomId);
                    continue;
                }

                Assert.That(room.faces, Has.Length.EqualTo(4), room.roomId);
                Assert.That(room.faces.Select(face => face.direction), Is.EquivalentTo(expectedDirections), room.roomId);
                Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/" + room.roomId + "_" + face.direction.ToString().ToLowerInvariant()), Is.True, room.roomId);
            }
        }

        [Test]
        public void Stage1Data_SecondFloorHallwayUsesTwoPerspectiveFaces()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "second_floor_hallway");
            var north = room.faces.First(face => face.direction == RoomFaceDirection.North);
            var south = room.faces.First(face => face.direction == RoomFaceDirection.South);

            Assert.That(room.faces, Has.Length.EqualTo(2));
            Assert.That(north.backgroundResource, Is.EqualTo("EscapeFromNightmares/Rooms/second_floor_hallway_north"));
            Assert.That(south.backgroundResource, Is.EqualTo("EscapeFromNightmares/Rooms/second_floor_hallway_south"));
            Assert.That(north.interactables.Select(item => item.targetRoomId), Is.EquivalentTo(new[] { "child_room", "study", "second_floor_bathroom", "mirror_room" }));
            Assert.That(south.interactables.Select(item => item.targetRoomId), Is.EquivalentTo(new[] { "master_bedroom", "dressing_room", "stairwell_2f" }));
        }

        [Test]
        public void Stage1Data_SecondFloorHallwayDoorsAreTransparentNormalizedHitboxes()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "second_floor_hallway");

            foreach (var interactable in room.interactables)
            {
                Assert.That(interactable.type, Is.EqualTo(InteractableType.Door), interactable.interactableId);
                Assert.That(interactable.showWorldImage, Is.False, interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.xMin, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.yMin, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.xMax, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.yMax, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.width, Is.GreaterThan(0f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.height, Is.GreaterThan(0f), interactable.interactableId);
            }
        }

        [Test]
        public void Stage1Data_StudyPlacesSafeExitAndClueOnFourFaces()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "study");

            Assert.That(room.faces, Has.Length.EqualTo(4));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "study_safe_surrounding", "study_safe_obj" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "study_exit" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "study_safe_clue_note", "study_desk_surface", "study_clue_board" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "study_portrait", "study_window_view" }));
            Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/study_" + face.direction.ToString().ToLowerInvariant()), Is.True);
        }

        [Test]
        public void Stage1Data_StudyInteractablesUseTransparentHitboxesAndClueResource()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "study");
            var clue = room.interactables.First(item => item.interactableId == "study_safe_clue_note");

            Assert.That(clue.clueViewResource, Is.EqualTo("EscapeFromNightmares/CloseUps/study_safe_clue_note"));
            foreach (var interactable in room.interactables)
            {
                Assert.That(interactable.showWorldImage, Is.False, interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.xMin, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.yMin, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.xMax, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.yMax, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.width, Is.GreaterThan(0f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.height, Is.GreaterThan(0f), interactable.interactableId);
            }
        }

        [Test]
        public void Stage1Data_ChildRoomPlacesSampleInteractionsOnFourFaces()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "child_room");

            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Contains.Item("child_desk_drawer"));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Contains.Item("child_room_door"));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.interactableId), Contains.Item("child_bed_hide"));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables.Select(item => item.interactableId), Contains.Item("child_window_silhouette"));
        }

        [Test]
        public void Stage1Data_ChildRoomUsesExpectedImageResourcesAndHitboxes()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "child_room");
            var interactables = room.faces.SelectMany(face => face.interactables).ToArray();

            Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/child_room_" + face.direction.ToString().ToLowerInvariant()), Is.True);
            Assert.That(interactables.All(interactable => interactable.imageResource == "EscapeFromNightmares/Objects/" + interactable.interactableId), Is.True);

            foreach (var interactable in interactables)
            {
                Assert.That(interactable.normalizedHitbox.xMin, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.yMin, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.xMax, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.yMax, Is.InRange(0f, 1f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.width, Is.GreaterThan(0f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.height, Is.GreaterThan(0f), interactable.interactableId);
            }
        }

        [Test]
        public void Stage1Data_ChildRoomHidesObjectSpritesButKeepsHitboxes()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var room = stage.rooms.First(item => item.roomId == "child_room");
            var interactables = room.faces.SelectMany(face => face.interactables).ToArray();

            Assert.That(interactables, Is.Not.Empty);
            foreach (var interactable in interactables)
            {
                Assert.That(interactable.showWorldImage, Is.False, interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.width, Is.GreaterThan(0f), interactable.interactableId);
                Assert.That(interactable.normalizedHitbox.height, Is.GreaterThan(0f), interactable.interactableId);
            }
        }

        [Test]
        public void Stage1Data_TornDrawingFragmentUsesProjectResourceIcon()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var item = stage.items.First(candidate => candidate.itemId == "torn_drawing_fragment");

            Assert.That(item.iconResource, Is.EqualTo("EscapeFromNightmares/Items/item_torn_drawing_fragment"));
        }

        [Test]
        public void Stage1Data_ChildDeskDrawerUsesCloseUpResources()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var drawer = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.interactableId == "child_desk_drawer");

            Assert.That(drawer.closeUpClosedResource, Is.EqualTo("EscapeFromNightmares/CloseUps/child_desk_drawer_closed"));
            Assert.That(drawer.closeUpOpenWithItemResource, Is.EqualTo("EscapeFromNightmares/CloseUps/child_desk_drawer_open_with_item"));
            Assert.That(drawer.closeUpOpenEmptyResource, Is.EqualTo("EscapeFromNightmares/CloseUps/child_desk_drawer_open_empty"));
            Assert.That(drawer.closeUpItemId, Is.EqualTo("torn_drawing_fragment"));
            Assert.That(drawer.closeUpOpenSoundId, Is.EqualTo("sfx_drawer_open"));
            Assert.That(drawer.closeUpCloseSoundId, Is.EqualTo("sfx_drawer_close"));
            Assert.That(drawer.disableRoomHitboxWhenUsed, Is.True);
            Assert.That(drawer.oneShot, Is.False);
        }

        [Test]
        public void Stage1Data_StudySafeUsesDeferredRewardCloseUpResources()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");
            var safe = stage.rooms.First(room => room.roomId == "study").interactables.First(item => item.interactableId == "study_safe_obj");

            Assert.That(puzzle.successFlag, Is.EqualTo("study_safe_unlocked"));
            Assert.That(puzzle.deferSolvedUntilRewardPickup, Is.True);
            Assert.That(safe.closeUpClosedResource, Is.EqualTo("EscapeFromNightmares/CloseUps/study_safe_locked"));
            Assert.That(safe.closeUpOpenWithItemResource, Is.EqualTo("EscapeFromNightmares/CloseUps/study_safe_open_with_item"));
            Assert.That(safe.closeUpOpenEmptyResource, Is.EqualTo("EscapeFromNightmares/CloseUps/study_safe_open_empty"));
            Assert.That(safe.closeUpItemId, Is.EqualTo("fuse_holder"));
        }

        [Test]
        public void Stage1Data_CoreInspectionObjectsUseClueCloseUps()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var expected = new[]
            {
                "child_desk_surface",
                "child_drawing_board",
                "child_window_view",
                "study_safe_surrounding",
                "study_desk_surface",
                "study_clue_board",
                "study_portrait",
                "study_window_view"
            };
            var interactables = stage.rooms.SelectMany(room => room.interactables);

            foreach (var id in expected)
            {
                var interactable = interactables.First(item => item.interactableId == id);
                Assert.That(interactable.type, Is.EqualTo(InteractableType.ClueObject), id);
                Assert.That(interactable.clueViewResource, Is.EqualTo("EscapeFromNightmares/CloseUps/" + id), id);
                Assert.That(interactable.showWorldImage, Is.False, id);
            }
        }

        [Test]
        public void GameDirector_ConditionalRoomBackgroundsUseSessionState()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var flags = new FlagService(session);
            var childNorth = stage.rooms.First(room => room.roomId == "child_room").faces.First(face => face.direction == RoomFaceDirection.North);
            var studyNorth = stage.rooms.First(room => room.roomId == "study").faces.First(face => face.direction == RoomFaceDirection.North);

            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(childNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/child_room_north"));
            session.AddItem("torn_drawing_fragment");
            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(childNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/child_room_north_drawer_empty"));

            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(studyNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/study_north"));
            session.SetFlag("study_safe_opened");
            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(studyNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/study_north_safe_open_with_item"));
            session.AddItem("fuse_holder");
            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(studyNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/study_north_safe_open_empty"));
            session.SetFlag("puzzle_study_safe_clear");
            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(studyNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/study_north_safe_open_empty"));
        }

        [Test]
        public void GameDirector_ChildDeskDrawerHitboxStopsRenderingAfterItemAcquired()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var room = stage.rooms.First(item => item.roomId == "child_room");
            var drawer = room.interactables.First(item => item.interactableId == "child_desk_drawer");
            var door = room.interactables.First(item => item.interactableId == "child_room_door");

            Assert.That(GameDirector.ShouldRenderRoomHitbox(drawer, session), Is.True);
            Assert.That(GameDirector.ShouldRenderRoomHitbox(door, session), Is.True);

            session.MarkInteractableUsed(drawer.interactableId);

            Assert.That(GameDirector.ShouldRenderRoomHitbox(drawer, session), Is.False);
            Assert.That(GameDirector.ShouldRenderRoomHitbox(door, session), Is.True);
        }

        [Test]
        public void Stage1Data_ChildBedHideUsesHideViewResource()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var hideSpot = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.interactableId == "child_bed_hide");

            Assert.That(hideSpot.hideViewResource, Is.EqualTo("EscapeFromNightmares/HideViews/child_bed_under_view"));
            Assert.That(hideSpot.soundId, Is.EqualTo("sfx_hide"));
        }

        [Test]
        public void EscapeActionResolver_ChildDeskDrawerOpensCloseUpBeforeItemPickup()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var resolver = new EscapeActionResolver(session, new FlagService(session), stage.soundCatalog);
            var drawer = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.interactableId == "child_desk_drawer");

            var result = resolver.ResolveInteractable(drawer);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.OpenCloseUp && action.Value == "child_desk_drawer"), Is.True);
            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.AcquireItem), Is.False);
            Assert.That(session.HasItem("torn_drawing_fragment"), Is.False);
        }

        [Test]
        public void InteractionSystem_ChildDeskDrawerReturnsOpenCloseUp()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var drawer = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.interactableId == "child_desk_drawer");

            var result = service.Resolve(drawer);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.OpenCloseUp));
            Assert.That(result.Value, Is.EqualTo("child_desk_drawer"));
        }

        [Test]
        public void InteractionSystem_StudySafeClueReturnsOpenCloseUp()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var clue = stage.rooms.First(room => room.roomId == "study").interactables.First(item => item.interactableId == "study_safe_clue_note");

            var result = service.Resolve(clue);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.OpenCloseUp));
            Assert.That(result.Value, Is.EqualTo("study_safe_clue_note"));
        }

        [Test]
        public void InventoryWindow_BuildSlotModelsShowsAcquiredItemsAndSelection()
        {
            var texture = new Texture2D(4, 4);
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, 4f, 4f), Vector2.one * 0.5f);

            var models = InventoryWindow.BuildSlotModels(
                new[] { "torn_drawing_fragment" },
                "torn_drawing_fragment",
                3,
                _ => sprite);

            Assert.That(models, Has.Length.EqualTo(3));
            Assert.That(models[0].ItemId, Is.EqualTo("torn_drawing_fragment"));
            Assert.That(models[0].Icon, Is.EqualTo(sprite));
            Assert.That(models[0].Selected, Is.True);
            Assert.That(models[1].HasItem, Is.False);
            Assert.That(models[2].HasItem, Is.False);
        }

        [Test]
        public void RoomSpriteCatalog_TryFindReturnsDirectSpriteReference()
        {
            var texture = new Texture2D(4, 4);
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, 4f, 4f), Vector2.one * 0.5f);
            var catalog = ScriptableObject.CreateInstance<RoomSpriteCatalog>();
            catalog.SetSprites(new[] { new SpriteEntry { spriteId = "child_room_north", sprite = sprite } });

            Assert.That(catalog.TryFind("child_room_north", out var found), Is.True);
            Assert.That(found, Is.EqualTo(sprite));
        }

        [Test]
        public void Stage1Data_FaceInteractablesPreserveRoomInteractables()
        {
            var stage = RuntimeStageFactory.CreateStage1();

            foreach (var room in stage.rooms)
            {
                var roomInteractableIds = room.interactables.Select(interactable => interactable.interactableId);
                var faceInteractableIds = room.faces.SelectMany(face => face.interactables).Select(interactable => interactable.interactableId);

                Assert.That(faceInteractableIds, Is.EquivalentTo(roomInteractableIds), room.roomId);
            }
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
        public void FlagService_ConditionsMet_ChecksFlagsItemsAndSolvedPuzzles()
        {
            var session = new GameSession();
            session.Start(RuntimeStageFactory.CreateStage1());
            session.AddItem("fuse");
            session.SetFlag("power_ready");
            session.SetFlag("clue_a");
            session.MarkPuzzleSolved("study_safe");
            var service = new FlagService(session);
            var conditions = new ConditionDefinition
            {
                requiredItemIds = new[] { "fuse" },
                requiredFlagIds = new[] { "power_ready" },
                anyFlagIds = new[] { "clue_a", "clue_b" },
                forbiddenFlagIds = new[] { "blocked" },
                solvedPuzzleIds = new[] { "study_safe" }
            };

            Assert.That(service.ConditionsMet(conditions), Is.True);

            session.SetFlag("blocked");

            Assert.That(service.ConditionsMet(conditions), Is.False);
        }

        [Test]
        public void InteractionSystem_ConditionFailure_ReturnsNone()
        {
            var session = new GameSession();
            session.Start(RuntimeStageFactory.CreateStage1());
            var service = new InteractionSystem(session);
            var interactable = new InteractableDefinition
            {
                interactableId = "locked_note",
                type = InteractableType.ClueObject,
                eventId = "clue_locked",
                conditions = new ConditionDefinition { requiredFlagIds = new[] { "missing_flag" } }
            };

            var result = service.Resolve(interactable);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.None));
        }

        [Test]
        public void Puzzle_TrySolve_StudySafeUnlocksWithoutRewardOrSolvedState()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "3", "1", "4", "2" }));
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasFlag("study_safe_unlocked"), Is.True);
            Assert.That(session.HasItem("fuse_holder"), Is.False);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
            Assert.That(session.HasFlag("puzzle_study_safe_clear"), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_WrongTokensDoNotGrantRewardOrSolvedState()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");

            Assert.That(service.TrySolve(puzzle, new[] { "0", "0", "0", "0" }), Is.False);
            Assert.That(session.HasItem("fuse_holder"), Is.False);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
        }

        [Test]
        public void StudySafeDigit_NextValueCyclesZeroThroughNine()
        {
            var value = 0;

            for (var expected = 1; expected <= 9; expected++)
            {
                value = GameDirector.NextStudySafeDigitValue(value);
                Assert.That(value, Is.EqualTo(expected));
            }

            Assert.That(GameDirector.NextStudySafeDigitValue(value), Is.EqualTo(0));
        }

        [Test]
        public void StudySafeDigit_ResourceMapsToTenImageAssets()
        {
            for (var digit = 0; digit <= 9; digit++)
            {
                Assert.That(GameDirector.StudySafeDigitResource(digit), Is.EqualTo("EscapeFromNightmares/Puzzles/study_safe_digit_" + digit));
            }
        }

        [Test]
        public void StudySafeDigit_ResourcesLoadAsSprites()
        {
            for (var digit = 0; digit <= 9; digit++)
            {
                Assert.That(Resources.Load<Sprite>(GameDirector.StudySafeDigitResource(digit)), Is.Not.Null, digit.ToString());
            }
        }

        [Test]
        public void StudySafeDigit_LayoutPreservesSerializedOrSceneObjects()
        {
            Assert.That(GameDirector.ShouldPreserveStudySafeDigitLayout(true, false), Is.True);
            Assert.That(GameDirector.ShouldPreserveStudySafeDigitLayout(false, true), Is.True);
            Assert.That(GameDirector.ShouldPreserveStudySafeDigitLayout(true, true), Is.True);
            Assert.That(GameDirector.ShouldPreserveStudySafeDigitLayout(false, false), Is.False);
        }

        [Test]
        public void StudySafeDigit_CorrectInputStateUnlocksAndOpensWithoutReward()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);

            GameDirector.ApplyStudySafeUnlockAndOpenState(session);

            Assert.That(session.HasFlag("study_safe_unlocked"), Is.True);
            Assert.That(session.HasFlag("study_safe_opened"), Is.True);
            Assert.That(session.HasItem("fuse_holder"), Is.False);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
            Assert.That(session.HasFlag("puzzle_study_safe_clear"), Is.False);
        }

        [Test]
        public void StudySafeDigit_MatchesOnlyConfiguredAnswer()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");

            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(new[] { 3, 1, 4, 2 }, puzzle.answerTokens), Is.True);
            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(new[] { 3, 1, 4, 1 }, puzzle.answerTokens), Is.False);
            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(new[] { 3, 1, 4 }, puzzle.answerTokens), Is.False);
        }

        [Test]
        public void StudySafeDigit_WrongAutoStateDoesNotSolveOrReward()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");
            var wrongDigits = new[] { 3, 1, 4, 1 };

            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(wrongDigits, puzzle.answerTokens), Is.False);
            Assert.That(session.HasItem("fuse_holder"), Is.False);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_DeferredStudySafeDoesNotTrackSolvedPuzzle()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");
            puzzle.oneShot = true;

            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
        }

        [Test]
        public void GameSession_RotateFaceWrapsAndMoveToResetsNorth()
        {
            var session = new GameSession();
            session.Start(RuntimeStageFactory.CreateStage1());

            session.RotateFace(-1);

            Assert.That(session.CurrentFaceDirection, Is.EqualTo(RoomFaceDirection.West));

            session.MoveTo("study");

            Assert.That(session.CurrentRoomId, Is.EqualTo("study"));
            Assert.That(session.CurrentFaceDirection, Is.EqualTo(RoomFaceDirection.North));
        }

        [Test]
        public void GameDirector_NextFaceDirectionTogglesTwoFaceHallwayOnly()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var hallway = stage.rooms.First(item => item.roomId == "second_floor_hallway");
            var childRoom = stage.rooms.First(item => item.roomId == "child_room");

            Assert.That(GameDirector.NextFaceDirection(hallway, RoomFaceDirection.North, 1), Is.EqualTo(RoomFaceDirection.South));
            Assert.That(GameDirector.NextFaceDirection(hallway, RoomFaceDirection.South, 1), Is.EqualTo(RoomFaceDirection.North));
            Assert.That(GameDirector.NextFaceDirection(hallway, RoomFaceDirection.North, -1), Is.EqualTo(RoomFaceDirection.South));
            Assert.That(GameDirector.NextFaceDirection(childRoom, RoomFaceDirection.North, 1), Is.EqualTo(RoomFaceDirection.East));
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
        public void EscapeActionResolver_OpenPuzzleDoesNotSolveImmediately()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var resolver = new EscapeActionResolver(session, new FlagService(session), stage.soundCatalog);
            var puzzleObject = stage.rooms.First(room => room.roomId == "study").interactables.First(item => item.puzzleId == "study_safe");

            var result = resolver.ResolveInteractable(puzzleObject);

            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.OpenPuzzle && action.Value == "study_safe"), Is.True);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
            Assert.That(session.HasItem("fuse_holder"), Is.False);
        }

        [Test]
        public void Stage1Data_PuzzlesHaveCloseUpResourcePaths()
        {
            var stage = RuntimeStageFactory.CreateStage1();

            Assert.That(stage.puzzles.All(puzzle => puzzle.closeUpResource == "EscapeFromNightmares/Puzzles/" + puzzle.puzzleId), Is.True);
        }

        [Test]
        public void EscapeActionResolver_OneShotUsedInteractableFails()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var resolver = new EscapeActionResolver(session, new FlagService(session), stage.soundCatalog);
            var interactable = new InteractableDefinition
            {
                interactableId = "one_shot_item",
                type = InteractableType.ItemPickup,
                grantsItemId = "fuse",
                oneShot = true
            };

            Assert.That(resolver.ResolveInteractable(interactable).Succeeded, Is.True);

            session.MarkInteractableUsed(interactable.interactableId);

            Assert.That(resolver.ResolveInteractable(interactable).Succeeded, Is.False);
        }

        [Test]
        public void SoundCatalog_TryFind_ReturnsEntryWithCategory()
        {
            var catalog = RuntimeStageFactory.CreateStage1().soundCatalog;

            Assert.That(catalog.TryFind("ui_click", out var entry), Is.True);
            Assert.That(entry.category, Is.EqualTo(SoundCategory.Ui));
            Assert.That(entry.resourcePath, Is.EqualTo("EscapeFromNightmares/Audio/UI/ui_click"));
        }

        [Test]
        public void SoundCatalog_TryFind_ReturnsDrawerSoundEntries()
        {
            var catalog = RuntimeStageFactory.CreateStage1().soundCatalog;

            Assert.That(catalog.TryFind("sfx_drawer_open", out var openEntry), Is.True);
            Assert.That(openEntry.category, Is.EqualTo(SoundCategory.Sfx));
            Assert.That(openEntry.resourcePath, Is.EqualTo("EscapeFromNightmares/Audio/SFX/sfx_drawer_open"));
            Assert.That(catalog.TryFind("sfx_drawer_close", out var closeEntry), Is.True);
            Assert.That(closeEntry.category, Is.EqualTo(SoundCategory.Sfx));
            Assert.That(closeEntry.resourcePath, Is.EqualTo("EscapeFromNightmares/Audio/SFX/sfx_drawer_close"));
        }

        [Test]
        public void EscapeActionResolver_SoundIdAddsPlaySoundAction()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            var session = new GameSession();
            session.Start(stage);
            var resolver = new EscapeActionResolver(session, new FlagService(session), stage.soundCatalog);
            var interactable = new InteractableDefinition
            {
                interactableId = "sound_note",
                type = InteractableType.ClueObject,
                eventId = "clue_sound",
                soundId = "ui_click"
            };

            var result = resolver.ResolveInteractable(interactable);
            var soundAction = result.Actions.First(action => action.Type == EscapeActionType.PlaySound);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(soundAction.SoundEntry.category, Is.EqualTo(SoundCategory.Ui));
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
            Assert.That(catalog.titleLogoPath, Is.EqualTo("EscapeFromNightmares/Title/title_logo_escape_from_nightmare"));
            Assert.That(catalog.titleStartButtonPath, Is.EqualTo("EscapeFromNightmares/Title/UI/button_start"));
            Assert.That(catalog.titleSettingsButtonPath, Is.EqualTo("EscapeFromNightmares/Title/UI/button_settings"));
            Assert.That(catalog.titleQuitButtonPath, Is.EqualTo("EscapeFromNightmares/Title/UI/button_quit"));
            Assert.That(catalog.titleCloseButtonPath, Is.EqualTo("EscapeFromNightmares/Title/UI/button_close"));
            Assert.That(catalog.settingsPanelBackgroundPath, Is.EqualTo("EscapeFromNightmares/Title/UI/settings_panel_bg"));
            Assert.That(catalog.settingsHeaderPath, Is.EqualTo("EscapeFromNightmares/Title/UI/settings_header"));
            Assert.That(catalog.settingsMasterLabelPath, Is.EqualTo("EscapeFromNightmares/Title/UI/settings_label_master"));
            Assert.That(catalog.settingsBgmLabelPath, Is.EqualTo("EscapeFromNightmares/Title/UI/settings_label_bgm"));
            Assert.That(catalog.settingsSfxLabelPath, Is.EqualTo("EscapeFromNightmares/Title/UI/settings_label_sfx"));
            Assert.That(catalog.settingsUiLabelPath, Is.EqualTo("EscapeFromNightmares/Title/UI/settings_label_ui"));
            Assert.That(catalog.settingsSliderTrackPath, Is.EqualTo("EscapeFromNightmares/Title/UI/slider_track"));
            Assert.That(catalog.settingsSliderFillPath, Is.EqualTo("EscapeFromNightmares/Title/UI/slider_fill"));
            Assert.That(catalog.settingsSliderHandlePath, Is.EqualTo("EscapeFromNightmares/Title/UI/slider_handle"));
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

        [Test]
        public void SoundManager_PlayMissingClip_DoesNotThrow()
        {
            var gameObject = new GameObject("SoundManagerMissingClipTest");
            try
            {
                var manager = gameObject.AddComponent<SoundManager>();
                manager.Initialize(new ResourceManager(ResourcePathCatalog.CreateDefault()));

                Assert.DoesNotThrow(() => manager.Play(new SoundEntry
                {
                    soundId = "missing",
                    resourcePath = "missing/path/for/test",
                    category = SoundCategory.Sfx
                }));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }
    }
}
