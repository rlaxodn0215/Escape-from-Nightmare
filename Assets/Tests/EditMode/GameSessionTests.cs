using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.Services;
using EscapeFromNightmares.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmares.Tests.EditMode
{
    public sealed class GameSessionTests
    {
        [Test]
        public void Stage1Data_HasStartRoomAndRequiredPuzzles()
        {
            var stage = StageTestData.LoadStage1();

            Assert.That(stage.startRoomId, Is.EqualTo("child_room"));
            Assert.That(stage.rooms.Any(room => room.roomId == "altar_room"), Is.True);
            Assert.That(stage.puzzles.Any(puzzle => puzzle.puzzleId == "front_door_escape"), Is.True);
        }

        [Test]
        public void Stage1Data_RoomsUseExpectedDirectionalFaces()
        {
            var stage = StageTestData.LoadStage1();
            var expectedDirections = new[]
            {
                RoomFaceDirection.North,
                RoomFaceDirection.East,
                RoomFaceDirection.South,
                RoomFaceDirection.West
            };

            foreach (var room in stage.rooms)
            {
                if (room.roomId == "second_floor_hallway" || room.roomId == "first_floor_hallway")
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
        public void Stage1Data_SingleConnectionFourFaceRoomsExposeExactlyOneExitDoor()
        {
            var stage = StageTestData.LoadStage1();

            foreach (var room in stage.rooms.Where(item => item.faces.Length == 4 && item.connectedRoomIds.Length == 1))
            {
                var exitDoors = room.faces
                    .SelectMany(face => face.interactables ?? System.Array.Empty<InteractableDefinition>())
                    .Where(IsExitDoor)
                    .ToArray();
                var exitDoorFaces = room.faces
                    .Where(face => (face.interactables ?? System.Array.Empty<InteractableDefinition>()).Any(IsExitDoor))
                    .ToArray();

                Assert.That(exitDoors, Has.Length.EqualTo(1), room.roomId);
                Assert.That(exitDoors[0].targetRoomId, Is.EqualTo(room.connectedRoomIds[0]), room.roomId);
                Assert.That(exitDoorFaces, Has.Length.EqualTo(1), room.roomId);
            }
        }

        [Test]
        public void Stage1Data_SecondFloorHallwayUsesTwoPerspectiveFaces()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
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
        public void Stage1Data_FirstFloorLaundryPowerChainUsesPlannedFaces()
        {
            var stage = StageTestData.LoadStage1();
            var stairwell = stage.rooms.First(item => item.roomId == "stairwell_1f");
            var hallway = stage.rooms.First(item => item.roomId == "first_floor_hallway");
            var dining = stage.rooms.First(item => item.roomId == "dining_room");
            var kitchen = stage.rooms.First(item => item.roomId == "kitchen");
            var laundry = stage.rooms.First(item => item.roomId == "laundry_room");

            Assert.That(stairwell.hideSpotCount, Is.EqualTo(0));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "stairwell_2f" }));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "first_floor_hallway" }));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);

            Assert.That(hallway.hideSpotCount, Is.EqualTo(0));
            Assert.That(hallway.faces, Has.Length.EqualTo(2));
            Assert.That(hallway.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.targetRoomId), Is.EquivalentTo(new[] { "stairwell_1f", "dining_room", "kitchen", "laundry_room" }));
            Assert.That(hallway.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.targetRoomId), Is.EquivalentTo(new[] { "entrance", "living_room", "family_photo_room" }));

            Assert.That(dining.hideSpotCount, Is.EqualTo(0));
            Assert.That(dining.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "dining_seat_order_clue" }));
            Assert.That(dining.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "first_floor_hallway" }));

            Assert.That(kitchen.hideSpotCount, Is.EqualTo(1));
            Assert.That(kitchen.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "kitchen_clock_clue" }));
            Assert.That(kitchen.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "first_floor_hallway" }));
            Assert.That(kitchen.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "kitchen_hide" }));

            Assert.That(laundry.hideSpotCount, Is.EqualTo(1));
            Assert.That(laundry.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "laundry_box" }));
            Assert.That(laundry.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "breaker_box_obj", "basement_entry_door" }));
            Assert.That(laundry.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "laundry_hide" }));
            Assert.That(laundry.faces.First(face => face.direction == RoomFaceDirection.West).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "first_floor_hallway" }));
        }

        [Test]
        public void Stage1Data_BasementAltarChainUsesPlannedFaces()
        {
            var stage = StageTestData.LoadStage1();
            var laundry = stage.rooms.First(item => item.roomId == "laundry_room");
            var entry = stage.rooms.First(item => item.roomId == "basement_entry");
            var main = stage.rooms.First(item => item.roomId == "basement_main");
            var altarRoom = stage.rooms.First(item => item.roomId == "altar_room");
            var basementDoor = laundry.faces.First(face => face.direction == RoomFaceDirection.East).interactables.First(item => item.interactableId == "basement_entry_door");

            Assert.That(laundry.connectedRoomIds, Is.EquivalentTo(new[] { "first_floor_hallway", "basement_entry" }));
            Assert.That(basementDoor.targetRoomId, Is.EqualTo("basement_entry"));
            Assert.That(basementDoor.showWorldImage, Is.False);
            Assert.That(basementDoor.conditions.requiredFlagIds, Is.EqualTo(new[] { "electricity_restored" }));

            Assert.That(entry.hideSpotCount, Is.EqualTo(0));
            Assert.That(entry.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "laundry_room" }));
            Assert.That(entry.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "basement_main" }));
            Assert.That(entry.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(entry.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);

            Assert.That(main.hideSpotCount, Is.EqualTo(1));
            Assert.That(main.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "basement_wall_symbols" }));
            Assert.That(main.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "altar_room" }));
            Assert.That(main.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "basement_entry" }));
            Assert.That(main.faces.First(face => face.direction == RoomFaceDirection.West).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "basement_main_hide" }));

            var altar = altarRoom.interactables.First(item => item.interactableId == "altar");
            var key = altarRoom.interactables.First(item => item.interactableId == "front_door_key_on_altar");
            Assert.That(altarRoom.hideSpotCount, Is.EqualTo(0));
            Assert.That(altarRoom.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "altar", "front_door_key_on_altar" }));
            Assert.That(altarRoom.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "basement_main" }));
            Assert.That(altar.puzzleId, Is.EqualTo("basement_altar"));
            Assert.That(altar.showWorldImage, Is.False);
            Assert.That(key.grantsItemId, Is.EqualTo("front_door_key"));
            Assert.That(key.solvesPuzzleId, Is.EqualTo("basement_altar"));
            Assert.That(key.conditions.requiredFlagIds, Is.EqualTo(new[] { "front_door_key_spawned" }));
            Assert.That(key.disableRoomHitboxWhenUsed, Is.True);
        }

        [Test]
        public void Stage1Data_BasementAltarChainResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/basement_entry_north",
                "EscapeFromNightmares/Rooms/basement_entry_east",
                "EscapeFromNightmares/Rooms/basement_entry_south",
                "EscapeFromNightmares/Rooms/basement_entry_west",
                "EscapeFromNightmares/Rooms/basement_main_north",
                "EscapeFromNightmares/Rooms/basement_main_east",
                "EscapeFromNightmares/Rooms/basement_main_south",
                "EscapeFromNightmares/Rooms/basement_main_west",
                "EscapeFromNightmares/Rooms/altar_room_north",
                "EscapeFromNightmares/Rooms/altar_room_east",
                "EscapeFromNightmares/Rooms/altar_room_south",
                "EscapeFromNightmares/Rooms/altar_room_west",
                "EscapeFromNightmares/Rooms/altar_room_north_key_spawned",
                "EscapeFromNightmares/Rooms/altar_room_north_key_taken",
                "EscapeFromNightmares/CloseUps/basement_wall_symbols",
                "EscapeFromNightmares/HideViews/basement_main_hide_view",
                "EscapeFromNightmares/Puzzles/basement_altar",
                "EscapeFromNightmares/Items/item_front_door_key"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }

            foreach (var path in paths.Where(path => !path.StartsWith("EscapeFromNightmares/Items/")))
            {
                AssertPngResourceSize(path, 1280, 720);
            }

            AssertPngResourceSize("EscapeFromNightmares/Items/item_front_door_key", 512, 512);
        }

        [Test]
        public void Stage1Data_EntranceUsesFinalDoorHarness()
        {
            var stage = StageTestData.LoadStage1();
            var entrance = stage.rooms.First(item => item.roomId == "entrance");
            var north = entrance.faces.First(face => face.direction == RoomFaceDirection.North);
            var east = entrance.faces.First(face => face.direction == RoomFaceDirection.East);
            var south = entrance.faces.First(face => face.direction == RoomFaceDirection.South);
            var west = entrance.faces.First(face => face.direction == RoomFaceDirection.West);
            var frontDoor = entrance.interactables.First(item => item.interactableId == "front_door");

            Assert.That(entrance.connectedRoomIds, Is.EquivalentTo(new[] { "first_floor_hallway", "living_room" }));
            Assert.That(entrance.hideSpotCount, Is.EqualTo(0));
            Assert.That(entrance.faces, Has.Length.EqualTo(4));
            Assert.That(north.interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "front_door" }));
            Assert.That(east.interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "first_floor_hallway" }));
            Assert.That(south.interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "living_room" }));
            Assert.That(west.interactables, Is.Empty);
            Assert.That(frontDoor.type, Is.EqualTo(InteractableType.PuzzleObject));
            Assert.That(frontDoor.puzzleId, Is.EqualTo("front_door_escape"));
            Assert.That(frontDoor.showWorldImage, Is.False);
            Assert.That(north.conditionalBackgrounds.Select(item => item.backgroundResource), Is.EqualTo(new[] { "EscapeFromNightmares/Rooms/entrance_north_chase" }));
            Assert.That(north.conditionalBackgrounds[0].conditions.requiredFlagIds, Is.EqualTo(new[] { "final_chase_started" }));
        }

        [Test]
        public void Stage1Data_FinalChaseFrontDoorResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/entrance_north",
                "EscapeFromNightmares/Rooms/entrance_east",
                "EscapeFromNightmares/Rooms/entrance_south",
                "EscapeFromNightmares/Rooms/entrance_west",
                "EscapeFromNightmares/Rooms/entrance_north_chase",
                "EscapeFromNightmares/Rooms/first_floor_hallway_south_chase",
                "EscapeFromNightmares/Puzzles/front_door_escape"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
                AssertPngResourceSize(path, 1280, 720);
            }
        }

        [Test]
        public void Stage1Data_FirstFloorLaundryPowerChainResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/stairwell_1f_north",
                "EscapeFromNightmares/Rooms/stairwell_1f_east",
                "EscapeFromNightmares/Rooms/stairwell_1f_south",
                "EscapeFromNightmares/Rooms/stairwell_1f_west",
                "EscapeFromNightmares/Rooms/first_floor_hallway_north",
                "EscapeFromNightmares/Rooms/first_floor_hallway_south",
                "EscapeFromNightmares/Rooms/dining_room_north",
                "EscapeFromNightmares/Rooms/dining_room_east",
                "EscapeFromNightmares/Rooms/dining_room_south",
                "EscapeFromNightmares/Rooms/dining_room_west",
                "EscapeFromNightmares/Rooms/kitchen_north",
                "EscapeFromNightmares/Rooms/kitchen_east",
                "EscapeFromNightmares/Rooms/kitchen_south",
                "EscapeFromNightmares/Rooms/kitchen_west",
                "EscapeFromNightmares/Rooms/laundry_room_north",
                "EscapeFromNightmares/Rooms/laundry_room_east",
                "EscapeFromNightmares/Rooms/laundry_room_south",
                "EscapeFromNightmares/Rooms/laundry_room_west",
                "EscapeFromNightmares/CloseUps/dining_seat_order_clue",
                "EscapeFromNightmares/CloseUps/kitchen_clock_clue",
                "EscapeFromNightmares/HideViews/kitchen_sink_hide_view",
                "EscapeFromNightmares/HideViews/laundry_machine_hide_view",
                "EscapeFromNightmares/Puzzles/laundry_storage_box",
                "EscapeFromNightmares/Puzzles/breaker_box",
                "EscapeFromNightmares/Items/item_fuse",
                "EscapeFromNightmares/Items/item_old_keychain"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }

            foreach (var path in paths.Where(path => !path.StartsWith("EscapeFromNightmares/Items/")))
            {
                AssertPngResourceSize(path, 1280, 720);
            }

            AssertPngResourceSize("EscapeFromNightmares/Items/item_fuse", 512, 512);
            AssertPngResourceSize("EscapeFromNightmares/Items/item_old_keychain", 512, 512);
        }

        [Test]
        public void Stage1Data_StudyPlacesSafeExitAndClueOnFourFaces()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
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
        public void Stage1Data_MirrorRoomUsesFourFaceHarness()
        {
            var stage = StageTestData.LoadStage1();
            var room = stage.rooms.First(item => item.roomId == "mirror_room");
            var panel = room.interactables.First(item => item.interactableId == "mirror_panel_obj");
            var exit = room.interactables.First(item => item.interactableId == "mirror_exit");

            Assert.That(room.connectedRoomIds, Is.EqualTo(new[] { "second_floor_hallway" }));
            Assert.That(room.hideSpotCount, Is.EqualTo(0));
            Assert.That(room.faces, Has.Length.EqualTo(4));
            Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/mirror_room_" + face.direction.ToString().ToLowerInvariant()), Is.True);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "mirror_panel_obj" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "mirror_exit" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);
            Assert.That(panel.type, Is.EqualTo(InteractableType.PuzzleObject));
            Assert.That(panel.puzzleId, Is.EqualTo("mirror_symbol_panel"));
            Assert.That(panel.showWorldImage, Is.False);
            Assert.That(panel.normalizedHitbox, Is.EqualTo(new Rect(0.32f, 0.20f, 0.36f, 0.22f)));
            Assert.That(exit.type, Is.EqualTo(InteractableType.Door));
            Assert.That(exit.targetRoomId, Is.EqualTo("second_floor_hallway"));
            Assert.That(exit.showWorldImage, Is.False);
            Assert.That(exit.normalizedHitbox, Is.EqualTo(new Rect(0.55f, 0.12f, 0.24f, 0.72f)));
        }

        [Test]
        public void Stage1Data_MirrorRoomResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/mirror_room_north",
                "EscapeFromNightmares/Rooms/mirror_room_east",
                "EscapeFromNightmares/Rooms/mirror_room_south",
                "EscapeFromNightmares/Rooms/mirror_room_west",
                "EscapeFromNightmares/Puzzles/mirror_symbol_panel"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }

            foreach (var path in paths.Where(item => !item.StartsWith("EscapeFromNightmares/Items/")))
            {
                AssertPngResourceSize(path, 1280, 720);
            }

            AssertPngResourceSize("EscapeFromNightmares/Items/item_small_doll", 512, 512);
            AssertPngResourceSize("EscapeFromNightmares/Items/item_symbol_fragment", 512, 512);
        }

        [Test]
        public void Stage1Data_SecondFloorBathroomUsesFourFaceHarness()
        {
            var stage = StageTestData.LoadStage1();
            var room = stage.rooms.First(item => item.roomId == "second_floor_bathroom");
            var clue = room.interactables.First(item => item.interactableId == "bathroom_mirror_rule_clue");
            var exit = room.interactables.First(item => item.interactableId == "bathroom_exit");

            Assert.That(room.connectedRoomIds, Is.EqualTo(new[] { "second_floor_hallway" }));
            Assert.That(room.hideSpotCount, Is.EqualTo(0));
            Assert.That(room.faces, Has.Length.EqualTo(4));
            Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/second_floor_bathroom_" + face.direction.ToString().ToLowerInvariant()), Is.True);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "bathroom_mirror_rule_clue" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "bathroom_exit" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);
            Assert.That(clue.type, Is.EqualTo(InteractableType.ClueObject));
            Assert.That(clue.clueViewResource, Is.EqualTo("EscapeFromNightmares/CloseUps/bathroom_mirror_rule_clue"));
            Assert.That(clue.showWorldImage, Is.False);
            Assert.That(exit.type, Is.EqualTo(InteractableType.Door));
            Assert.That(exit.targetRoomId, Is.EqualTo("second_floor_hallway"));
            Assert.That(exit.showWorldImage, Is.False);
        }

        [Test]
        public void Stage1Data_SecondFloorBathroomResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/second_floor_bathroom_north",
                "EscapeFromNightmares/Rooms/second_floor_bathroom_east",
                "EscapeFromNightmares/Rooms/second_floor_bathroom_south",
                "EscapeFromNightmares/Rooms/second_floor_bathroom_west",
                "EscapeFromNightmares/CloseUps/bathroom_mirror_rule_clue"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }
        }

        [Test]
        public void Stage1Data_DressingRoomUsesFourFaceHarness()
        {
            var stage = StageTestData.LoadStage1();
            var room = stage.rooms.First(item => item.roomId == "dressing_room");
            var clue = room.interactables.First(item => item.interactableId == "dressing_color_sequence_clue");
            var exit = room.interactables.First(item => item.interactableId == "dressing_exit");

            Assert.That(room.connectedRoomIds, Is.EqualTo(new[] { "second_floor_hallway" }));
            Assert.That(room.hideSpotCount, Is.EqualTo(0));
            Assert.That(room.faces, Has.Length.EqualTo(4));
            Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/dressing_room_" + face.direction.ToString().ToLowerInvariant()), Is.True);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "dressing_color_sequence_clue" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "dressing_exit" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);
            Assert.That(clue.type, Is.EqualTo(InteractableType.ClueObject));
            Assert.That(clue.clueViewResource, Is.EqualTo("EscapeFromNightmares/CloseUps/dressing_color_sequence_clue"));
            Assert.That(clue.showWorldImage, Is.False);
            Assert.That(exit.type, Is.EqualTo(InteractableType.Door));
            Assert.That(exit.targetRoomId, Is.EqualTo("second_floor_hallway"));
            Assert.That(exit.showWorldImage, Is.False);
        }

        [Test]
        public void Stage1Data_DressingRoomResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/dressing_room_north",
                "EscapeFromNightmares/Rooms/dressing_room_east",
                "EscapeFromNightmares/Rooms/dressing_room_south",
                "EscapeFromNightmares/Rooms/dressing_room_west",
                "EscapeFromNightmares/CloseUps/dressing_color_sequence_clue"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }
        }

        [Test]
        public void Stage1Data_MasterBedroomUsesFourFaceHarness()
        {
            var stage = StageTestData.LoadStage1();
            var room = stage.rooms.First(item => item.roomId == "master_bedroom");
            var drawer = room.interactables.First(item => item.interactableId == "master_drawer_obj");
            var exit = room.interactables.First(item => item.interactableId == "master_exit");

            Assert.That(room.connectedRoomIds, Is.EqualTo(new[] { "second_floor_hallway" }));
            Assert.That(room.hideSpotCount, Is.EqualTo(0));
            Assert.That(room.faces, Has.Length.EqualTo(4));
            Assert.That(room.faces.All(face => face.backgroundResource == "EscapeFromNightmares/Rooms/master_bedroom_" + face.direction.ToString().ToLowerInvariant()), Is.True);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "master_drawer_obj" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Is.EquivalentTo(new[] { "master_exit" }));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);
            Assert.That(drawer.type, Is.EqualTo(InteractableType.PuzzleObject));
            Assert.That(drawer.puzzleId, Is.EqualTo("master_bedroom_drawer"));
            Assert.That(drawer.showWorldImage, Is.False);
            Assert.That(drawer.normalizedHitbox, Is.EqualTo(new Rect(0.44f, 0.20f, 0.32f, 0.48f)));
            Assert.That(exit.type, Is.EqualTo(InteractableType.Door));
            Assert.That(exit.targetRoomId, Is.EqualTo("second_floor_hallway"));
            Assert.That(exit.showWorldImage, Is.False);
            Assert.That(exit.normalizedHitbox, Is.EqualTo(new Rect(0.66f, 0.12f, 0.26f, 0.74f)));
        }

        [Test]
        public void Stage1Data_MasterBedroomResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/master_bedroom_north",
                "EscapeFromNightmares/Rooms/master_bedroom_east",
                "EscapeFromNightmares/Rooms/master_bedroom_south",
                "EscapeFromNightmares/Rooms/master_bedroom_west",
                "EscapeFromNightmares/Puzzles/master_bedroom_drawer"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }
        }

        [Test]
        public void Stage1Data_AtticChainUsesFourFaceHarness()
        {
            var stage = StageTestData.LoadStage1();
            var stairwell = stage.rooms.First(item => item.roomId == "stairwell_2f");
            var attic = stage.rooms.First(item => item.roomId == "attic_main");
            var toyStorage = stage.rooms.First(item => item.roomId == "attic_toy_storage");

            Assert.That(stairwell.connectedRoomIds, Is.EquivalentTo(new[] { "second_floor_hallway", "stairwell_1f", "attic_main" }));
            Assert.That(stairwell.hideSpotCount, Is.EqualTo(0));
            Assert.That(stairwell.faces, Has.Length.EqualTo(4));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "second_floor_hallway" }));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "attic_main" }));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "stairwell_1f" }));
            Assert.That(stairwell.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);

            var album = attic.interactables.First(item => item.interactableId == "attic_family_album_photo");
            Assert.That(attic.connectedRoomIds, Is.EquivalentTo(new[] { "stairwell_2f", "attic_toy_storage" }));
            Assert.That(attic.hideSpotCount, Is.EqualTo(0));
            Assert.That(attic.faces, Has.Length.EqualTo(4));
            Assert.That(attic.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "attic_family_album_photo" }));
            Assert.That(attic.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "attic_toy_storage" }));
            Assert.That(attic.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "stairwell_2f" }));
            Assert.That(attic.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);
            Assert.That(album.type, Is.EqualTo(InteractableType.ClueObject));
            Assert.That(album.clueViewResource, Is.EqualTo("EscapeFromNightmares/CloseUps/attic_family_album_photo"));
            Assert.That(album.showWorldImage, Is.False);

            var toyBox = toyStorage.interactables.First(item => item.interactableId == "attic_toy_box");
            Assert.That(toyStorage.connectedRoomIds, Is.EqualTo(new[] { "attic_main" }));
            Assert.That(toyStorage.hideSpotCount, Is.EqualTo(0));
            Assert.That(toyStorage.faces, Has.Length.EqualTo(4));
            Assert.That(toyStorage.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Is.EqualTo(new[] { "attic_toy_box" }));
            Assert.That(toyStorage.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.targetRoomId), Is.EqualTo(new[] { "attic_main" }));
            Assert.That(toyStorage.faces.First(face => face.direction == RoomFaceDirection.South).interactables, Is.Empty);
            Assert.That(toyStorage.faces.First(face => face.direction == RoomFaceDirection.West).interactables, Is.Empty);
            Assert.That(toyBox.type, Is.EqualTo(InteractableType.PuzzleObject));
            Assert.That(toyBox.puzzleId, Is.EqualTo("attic_toy_sequence"));
            Assert.That(toyBox.showWorldImage, Is.False);
        }

        [Test]
        public void Stage1Data_AtticChainResourcesLoadAsSprites()
        {
            var paths = new[]
            {
                "EscapeFromNightmares/Rooms/stairwell_2f_north",
                "EscapeFromNightmares/Rooms/stairwell_2f_east",
                "EscapeFromNightmares/Rooms/stairwell_2f_south",
                "EscapeFromNightmares/Rooms/stairwell_2f_west",
                "EscapeFromNightmares/Rooms/attic_main_north",
                "EscapeFromNightmares/Rooms/attic_main_east",
                "EscapeFromNightmares/Rooms/attic_main_south",
                "EscapeFromNightmares/Rooms/attic_main_west",
                "EscapeFromNightmares/Rooms/attic_toy_storage_north",
                "EscapeFromNightmares/Rooms/attic_toy_storage_east",
                "EscapeFromNightmares/Rooms/attic_toy_storage_south",
                "EscapeFromNightmares/Rooms/attic_toy_storage_west",
                "EscapeFromNightmares/CloseUps/attic_family_album_photo",
                "EscapeFromNightmares/CloseUps/basement_wall_symbols",
                "EscapeFromNightmares/Puzzles/attic_toy_sequence",
                "EscapeFromNightmares/Items/item_small_doll",
                "EscapeFromNightmares/Items/item_symbol_fragment"
            };

            foreach (var path in paths)
            {
                Assert.That(Resources.Load<Sprite>(path), Is.Not.Null, path);
            }
        }

        [Test]
        public void Stage1Data_ChildRoomPlacesSampleInteractionsOnFourFaces()
        {
            var stage = StageTestData.LoadStage1();
            var room = stage.rooms.First(item => item.roomId == "child_room");

            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.North).interactables.Select(item => item.interactableId), Contains.Item("child_desk_drawer"));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.East).interactables.Select(item => item.interactableId), Contains.Item("child_room_door"));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.South).interactables.Select(item => item.interactableId), Contains.Item("child_bed_hide"));
            Assert.That(room.faces.First(face => face.direction == RoomFaceDirection.West).interactables.Select(item => item.interactableId), Contains.Item("child_window_silhouette"));
        }

        [Test]
        public void Stage1Data_ChildRoomUsesExpectedImageResourcesAndHitboxes()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
            var item = stage.items.First(candidate => candidate.itemId == "torn_drawing_fragment");

            Assert.That(item.iconResource, Is.EqualTo("EscapeFromNightmares/Items/item_torn_drawing_fragment"));
        }

        [Test]
        public void Stage1Data_ChildDeskDrawerUsesCloseUpResources()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
            var expected = new[]
            {
                "child_desk_surface",
                "child_drawing_board",
                "child_window_view",
                "study_safe_surrounding",
                "study_desk_surface",
                "study_clue_board",
                "study_portrait",
                "study_window_view",
                "bathroom_mirror_rule_clue",
                "dressing_color_sequence_clue"
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
            var stage = StageTestData.LoadStage1();
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
        public void GameDirector_FinalChaseBackgroundsUseSessionState()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var flags = new FlagService(session);
            var entranceNorth = stage.rooms.First(room => room.roomId == "entrance").faces.First(face => face.direction == RoomFaceDirection.North);
            var hallwaySouth = stage.rooms.First(room => room.roomId == "first_floor_hallway").faces.First(face => face.direction == RoomFaceDirection.South);

            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(entranceNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/entrance_north"));
            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(hallwaySouth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/first_floor_hallway_south"));

            session.SetFlag("final_chase_started");

            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(entranceNorth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/entrance_north_chase"));
            Assert.That(GameDirector.ResolveRoomFaceBackgroundResource(hallwaySouth, flags), Is.EqualTo("EscapeFromNightmares/Rooms/first_floor_hallway_south_chase"));
        }

        [Test]
        public void GameDirector_ChildDeskDrawerHitboxStopsRenderingAfterItemAcquired()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
            var hideSpot = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.interactableId == "child_bed_hide");

            Assert.That(hideSpot.hideViewResource, Is.EqualTo("EscapeFromNightmares/HideViews/child_bed_under_view"));
            Assert.That(hideSpot.soundId, Is.EqualTo("sfx_hide"));
        }

        [Test]
        public void EscapeActionResolver_ChildDeskDrawerOpensCloseUpBeforeItemPickup()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var clue = stage.rooms.First(room => room.roomId == "study").interactables.First(item => item.interactableId == "study_safe_clue_note");

            var result = service.Resolve(clue);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.OpenCloseUp));
            Assert.That(result.Value, Is.EqualTo("study_safe_clue_note"));
        }

        [Test]
        public void InteractionSystem_DressingColorClueReturnsOpenCloseUp()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var clue = stage.rooms.First(room => room.roomId == "dressing_room").interactables.First(item => item.interactableId == "dressing_color_sequence_clue");

            var result = service.Resolve(clue);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.OpenCloseUp));
            Assert.That(result.Value, Is.EqualTo("dressing_color_sequence_clue"));
        }

        [Test]
        public void InteractionSystem_MirrorPanelReturnsOpenPuzzle()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var panel = stage.rooms.First(room => room.roomId == "mirror_room").interactables.First(item => item.interactableId == "mirror_panel_obj");

            var result = service.Resolve(panel);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.OpenPuzzle));
            Assert.That(result.Value, Is.EqualTo("mirror_symbol_panel"));
        }

        [Test]
        public void InteractionSystem_MasterDrawerReturnsOpenPuzzle()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var drawer = stage.rooms.First(room => room.roomId == "master_bedroom").interactables.First(item => item.interactableId == "master_drawer_obj");

            var result = service.Resolve(drawer);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.OpenPuzzle));
            Assert.That(result.Value, Is.EqualTo("master_bedroom_drawer"));
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
        public void MonsterPlacementCatalog_DefaultEntriesExcludeChildRoomAndUseDisabledEmptyRects()
        {
            var stage = StageTestData.LoadStage1();
            var catalog = MonsterPlacementCatalog.CreateDefault(stage);
            var expectedKeys = stage.rooms
                .Where(room => room.roomId != "child_room")
                .SelectMany(room => room.faces.Select(face => room.roomId + "|" + face.direction))
                .ToArray();
            var actualKeys = catalog.Placements
                .Select(entry => entry.roomId + "|" + entry.faceDirection)
                .ToArray();

            Assert.That(actualKeys, Is.EquivalentTo(expectedKeys));
            Assert.That(catalog.Placements.Any(entry => entry.roomId == "child_room"), Is.False);
            Assert.That(catalog.Placements.All(entry => !entry.enabled), Is.True);
            Assert.That(catalog.Placements.All(entry => entry.normalizedRect.width == 0f && entry.normalizedRect.height == 0f), Is.True);
        }

        [Test]
        public void MonsterPlacementCatalog_AssetLoadsAndMatchesRuntimeRoomFaces()
        {
            var stage = StageTestData.LoadStage1();
            var catalog = AssetDatabase.LoadAssetAtPath<MonsterPlacementCatalog>("Assets/EscapeFromNightmares/ScriptableObjects/MonsterPlacementCatalog.asset");
            var expectedKeys = stage.rooms
                .Where(room => room.roomId != "child_room")
                .SelectMany(room => room.faces.Select(face => room.roomId + "|" + face.direction))
                .ToArray();

            Assert.That(catalog, Is.Not.Null);

            var actualKeys = catalog.Placements
                .Select(entry => entry.roomId + "|" + entry.faceDirection)
                .ToArray();

            Assert.That(actualKeys, Is.EquivalentTo(expectedKeys));
            Assert.That(catalog.Placements.Any(entry => entry.roomId == "child_room"), Is.False);
            Assert.That(catalog.Placements.Where(entry => !entry.enabled).All(entry => entry.normalizedRect.width == 0f && entry.normalizedRect.height == 0f), Is.True);
            Assert.That(catalog.Placements.Where(entry => entry.enabled).All(entry => entry.normalizedRect.x >= 0f
                && entry.normalizedRect.y >= 0f
                && entry.normalizedRect.xMax <= 1f
                && entry.normalizedRect.yMax <= 1f
                && entry.normalizedRect.width > 0f
                && entry.normalizedRect.height > 0f), Is.True);
        }

        [Test]
        public void MonsterPlacementCatalog_CreateMergedDefaultEntriesPreservesExistingEnabledRects()
        {
            var stage = StageTestData.LoadStage1();
            var preservedRect = new Rect(0.2f, 0.25f, 0.3f, 0.45f);
            var entries = MonsterPlacementCatalog.CreateMergedDefaultEntries(stage, new[]
            {
                new MonsterPlacementEntry
                {
                    roomId = "kitchen",
                    faceDirection = RoomFaceDirection.North,
                    enabled = true,
                    normalizedRect = preservedRect
                },
                new MonsterPlacementEntry
                {
                    roomId = "child_room",
                    faceDirection = RoomFaceDirection.North,
                    enabled = true,
                    normalizedRect = new Rect(0.1f, 0.1f, 0.1f, 0.1f)
                }
            }).ToArray();
            var kitchenNorth = entries.First(entry => entry.roomId == "kitchen" && entry.faceDirection == RoomFaceDirection.North);

            Assert.That(kitchenNorth.enabled, Is.True);
            Assert.That(kitchenNorth.normalizedRect, Is.EqualTo(preservedRect));
            Assert.That(entries.Any(entry => entry.roomId == "child_room"), Is.False);
            Assert.That(entries.Where(entry => entry.roomId != "kitchen" || entry.faceDirection != RoomFaceDirection.North).All(entry => !entry.enabled), Is.True);
        }

        [Test]
        public void GameDirector_TryResolveMonsterPlacementRequiresVisibleStateEnabledEntryAndPositiveRect()
        {
            var catalog = ScriptableObject.CreateInstance<MonsterPlacementCatalog>();
            try
            {
                var rect = new Rect(0.25f, 0.3f, 0.2f, 0.5f);
                catalog.SetPlacements(new[]
                {
                    new MonsterPlacementEntry
                    {
                        roomId = "kitchen",
                        faceDirection = RoomFaceDirection.North,
                        enabled = true,
                        normalizedRect = rect
                    },
                    new MonsterPlacementEntry
                    {
                        roomId = "kitchen",
                        faceDirection = RoomFaceDirection.East,
                        enabled = false,
                        normalizedRect = rect
                    }
                });

                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Disabled, out _), Is.False);
                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Normal, out _), Is.False);
                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.East, MonsterState.Approaching, out _), Is.False);
                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Approaching, out var approachingRect), Is.True);
                Assert.That(approachingRect, Is.EqualTo(rect));
                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Searching, out _), Is.True);
                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.North, MonsterState.NearDetection, out _), Is.True);
                Assert.That(GameDirector.TryResolveMonsterPlacement(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Chase, out _), Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }

        [Test]
        public void GameDirector_CreateMonsterQaSnapshotReportsHiddenAndMissingPlacementReasons()
        {
            var catalog = ScriptableObject.CreateInstance<MonsterPlacementCatalog>();
            try
            {
                var hidden = GameDirector.CreateMonsterQaSnapshot(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Normal, false);
                var missing = GameDirector.CreateMonsterQaSnapshot(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Approaching, false);
                var noCatalog = GameDirector.CreateMonsterQaSnapshot(null, "kitchen", RoomFaceDirection.North, MonsterState.Searching, false);

                Assert.That(hidden.Status, Is.EqualTo(GameDirector.MonsterQaStatus.StateHidden));
                Assert.That(hidden.StatusText, Is.EqualTo("state hidden"));
                Assert.That(missing.Status, Is.EqualTo(GameDirector.MonsterQaStatus.PlacementMissing));
                Assert.That(missing.StatusText, Is.EqualTo("placement missing"));
                Assert.That(noCatalog.Status, Is.EqualTo(GameDirector.MonsterQaStatus.CatalogMissing));
                Assert.That(noCatalog.StatusText, Is.EqualTo("catalog missing"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }

        [Test]
        public void GameDirector_CreateMonsterQaSnapshotReportsDisabledEmptyAndReadyPlacements()
        {
            var catalog = ScriptableObject.CreateInstance<MonsterPlacementCatalog>();
            try
            {
                catalog.SetPlacements(new[]
                {
                    new MonsterPlacementEntry
                    {
                        roomId = "kitchen",
                        faceDirection = RoomFaceDirection.North,
                        enabled = false,
                        normalizedRect = new Rect(0.2f, 0.2f, 0.3f, 0.4f)
                    },
                    new MonsterPlacementEntry
                    {
                        roomId = "kitchen",
                        faceDirection = RoomFaceDirection.East,
                        enabled = true,
                        normalizedRect = Rect.zero
                    },
                    new MonsterPlacementEntry
                    {
                        roomId = "kitchen",
                        faceDirection = RoomFaceDirection.South,
                        enabled = true,
                        normalizedRect = new Rect(0.2f, 0.2f, 0.3f, 0.4f)
                    }
                });

                var disabled = GameDirector.CreateMonsterQaSnapshot(catalog, "kitchen", RoomFaceDirection.North, MonsterState.Approaching, false);
                var empty = GameDirector.CreateMonsterQaSnapshot(catalog, "kitchen", RoomFaceDirection.East, MonsterState.Searching, false);
                var ready = GameDirector.CreateMonsterQaSnapshot(catalog, "kitchen", RoomFaceDirection.South, MonsterState.Chase, true);

                Assert.That(disabled.Status, Is.EqualTo(GameDirector.MonsterQaStatus.PlacementDisabled));
                Assert.That(disabled.StatusText, Is.EqualTo("placement disabled"));
                Assert.That(empty.Status, Is.EqualTo(GameDirector.MonsterQaStatus.PlacementEmpty));
                Assert.That(empty.StatusText, Is.EqualTo("placement empty"));
                Assert.That(ready.Status, Is.EqualTo(GameDirector.MonsterQaStatus.PlacementReady));
                Assert.That(ready.ShouldShowMonster, Is.True);
                Assert.That(ready.MonsterImageActive, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }

        [Test]
        public void MonsterRuntimeQaPanel_CreateBuildsInactiveNonBlockingPanelWithText()
        {
            var rootObject = new GameObject("MonsterQaPanelRoot", typeof(RectTransform));
            try
            {
                var panel = MonsterRuntimeQaPanel.Create(rootObject.GetComponent<RectTransform>());

                Assert.That(panel.name, Is.EqualTo("MonsterRuntimeQaPanel"));
                Assert.That(panel.gameObject.activeSelf, Is.False);
                Assert.That(panel.CanvasGroup, Is.Not.Null);
                Assert.That(panel.CanvasGroup.alpha, Is.EqualTo(0f));
                Assert.That(panel.CanvasGroup.blocksRaycasts, Is.False);
                Assert.That(panel.InfoText, Is.Not.Null);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(rootObject);
            }
        }

        [Test]
        public void MonsterAIController_DebugStateOverridePersistsUntilCleared()
        {
            var monsterAI = new MonsterAIController();
            var danger = new DangerSystem();
            var hiding = new HidingSystem();

            monsterAI.Enable();
            monsterAI.ForceDebugState(MonsterState.NearDetection);
            monsterAI.Tick(danger, hiding);

            Assert.That(monsterAI.State, Is.EqualTo(MonsterState.NearDetection));

            monsterAI.ClearDebugState();
            monsterAI.Tick(danger, hiding);

            Assert.That(monsterAI.State, Is.EqualTo(MonsterState.Normal));
        }

        [Test]
        public void GameDirector_ApplyMonsterPlacementMapsNormalizedRectToRectTransformAnchors()
        {
            var gameObject = new GameObject("MonsterImageTest", typeof(RectTransform));
            try
            {
                var transform = gameObject.GetComponent<RectTransform>();

                GameDirector.ApplyMonsterPlacement(transform, new Rect(0.2f, 0.3f, 0.4f, 0.5f));

                Assert.That(transform.anchorMin.x, Is.EqualTo(0.2f).Within(0.001f));
                Assert.That(transform.anchorMin.y, Is.EqualTo(0.3f).Within(0.001f));
                Assert.That(transform.anchorMax.x, Is.EqualTo(0.6f).Within(0.001f));
                Assert.That(transform.anchorMax.y, Is.EqualTo(0.8f).Within(0.001f));
                Assert.That(transform.offsetMin, Is.EqualTo(Vector2.zero));
                Assert.That(transform.offsetMax, Is.EqualTo(Vector2.zero));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void Stage1Data_FaceInteractablesPreserveRoomInteractables()
        {
            var stage = StageTestData.LoadStage1();

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
            session.Start(StageTestData.LoadStage1());

            Assert.That(session.AddItem("fuse"), Is.True);
            Assert.That(session.AddItem("fuse"), Is.False);
            Assert.That(session.HasItem("fuse"), Is.True);
        }

        [Test]
        public void FlagService_ConditionsMet_ChecksFlagsItemsAndSolvedPuzzles()
        {
            var session = new GameSession();
            session.Start(StageTestData.LoadStage1());
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
            session.Start(StageTestData.LoadStage1());
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
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");

            Assert.That(service.TrySolve(puzzle, new[] { "0", "0", "0", "0" }), Is.False);
            Assert.That(session.HasItem("fuse_holder"), Is.False);
            Assert.That(session.HasSolvedPuzzle("study_safe"), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_MirrorSymbolPanelGrantsMirrorAndSetsSolvedState()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "mirror_symbol_panel");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "heart", "child_hand", "cracked_circle", "keyhole" }));
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasItem("broken_hand_mirror"), Is.True);
            Assert.That(session.HasFlag("mirror_destroyed"), Is.True);
            Assert.That(session.HasSolvedPuzzle("mirror_symbol_panel"), Is.True);
        }

        [Test]
        public void Puzzle_TrySolve_MasterBedroomDrawerGrantsNecklaceAndSetsSolvedState()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "master_bedroom_drawer");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "black", "white", "red", "gray" }));
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasItem("old_necklace"), Is.True);
            Assert.That(session.HasFlag("master_drawer_opened"), Is.True);
            Assert.That(session.HasSolvedPuzzle("master_bedroom_drawer"), Is.True);
        }

        [Test]
        public void Puzzle_TrySolve_LaundryStorageBoxGrantsFuseOnce()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "laundry_storage_box");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "0", "9", "1", "5" }));
            Assert.That(puzzle.oneShot, Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasItem("fuse"), Is.True);
            Assert.That(session.HasFlag("puzzle_laundry_box_clear"), Is.True);
            Assert.That(session.HasSolvedPuzzle("laundry_storage_box"), Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_BreakerBoxRequiresFusePartsAndGrantsKeychainOnce()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "breaker_box");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "fuse_holder", "fuse" }));
            Assert.That(puzzle.requiredItemIds, Is.EqualTo(new[] { "fuse_holder", "fuse" }));
            Assert.That(puzzle.consumeRequiredItems, Is.False);
            Assert.That(puzzle.oneShot, Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);

            session.AddItem("fuse_holder");
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);

            session.AddItem("fuse");
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasItem("old_keychain"), Is.True);
            Assert.That(session.HasItem("fuse_holder"), Is.True);
            Assert.That(session.HasItem("fuse"), Is.True);
            Assert.That(session.HasFlag("electricity_restored"), Is.True);
            Assert.That(session.HasSolvedPuzzle("breaker_box"), Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_AtticToySequenceRequiresDrawingFragment()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "attic_toy_sequence");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "doll", "train", "block", "bell" }));
            Assert.That(puzzle.requiredItemIds, Is.EqualTo(new[] { "torn_drawing_fragment" }));
            Assert.That(puzzle.oneShot, Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);
            Assert.That(session.HasItem("small_doll"), Is.False);
            Assert.That(session.HasItem("symbol_fragment"), Is.False);
            Assert.That(session.HasSolvedPuzzle("attic_toy_sequence"), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_AtticToySequenceGrantsRewardsOnce()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            session.AddItem("torn_drawing_fragment");
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "attic_toy_sequence");

            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasItem("small_doll"), Is.True);
            Assert.That(session.HasItem("symbol_fragment"), Is.True);
            Assert.That(session.HasFlag("attic_toy_box_opened"), Is.True);
            Assert.That(session.HasSolvedPuzzle("attic_toy_sequence"), Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_BasementAltarSpawnsKeyWithoutImmediateReward()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "basement_altar");

            Assert.That(puzzle.answerTokens, Is.EqualTo(new[] { "broken_hand_mirror", "small_doll", "old_keychain", "old_necklace" }));
            Assert.That(puzzle.requiredItemIds, Is.EqualTo(new[] { "broken_hand_mirror", "small_doll", "old_keychain", "old_necklace" }));
            Assert.That(puzzle.rewardItemIds, Is.Empty);
            Assert.That(puzzle.oneShot, Is.True);
            Assert.That(puzzle.deferSolvedUntilRewardPickup, Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);

            session.AddItem("broken_hand_mirror");
            session.AddItem("small_doll");
            session.AddItem("old_keychain");
            session.AddItem("old_necklace");

            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasFlag("front_door_key_spawned"), Is.True);
            Assert.That(session.HasFlag("final_chase_started"), Is.False);
            Assert.That(session.HasItem("front_door_key"), Is.False);
            Assert.That(session.HasSolvedPuzzle("basement_altar"), Is.False);
            Assert.That(session.HasItem("broken_hand_mirror"), Is.True);
            Assert.That(session.HasItem("small_doll"), Is.True);
            Assert.That(session.HasItem("old_keychain"), Is.True);
            Assert.That(session.HasItem("old_necklace"), Is.True);
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);
        }

        [Test]
        public void Puzzle_TrySolve_FrontDoorEscapeRequiresKeyAndSetsStageClear()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new PuzzleService(session);
            var puzzle = stage.puzzles.First(item => item.puzzleId == "front_door_escape");

            Assert.That(puzzle.requiredItemIds, Is.EqualTo(new[] { "front_door_key" }));
            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.False);
            Assert.That(session.HasFlag("stage1_clear"), Is.False);

            session.AddItem("front_door_key");

            Assert.That(service.TrySolve(puzzle, puzzle.answerTokens), Is.True);
            Assert.That(session.HasFlag("stage1_clear"), Is.True);
            Assert.That(session.HasSolvedPuzzle("front_door_escape"), Is.True);
        }

        [Test]
        public void GameDirector_ApplyStageClearStateSavesRecordAndResetsMonster()
        {
            var directory = Path.Combine(Path.GetTempPath(), "EscapeFromNightmaresStageClearTests");
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var saveService = new SettingsSaveService(directory);
            var monsterAI = new MonsterAIController();
            monsterAI.ForceChase();

            GameDirector.ApplyStageClearState(session, stage, saveService, monsterAI);

            Assert.That(session.HasFlag("stage1_clear"), Is.True);
            Assert.That(saveService.LoadClearRecords().stage1Clear, Is.True);
            Assert.That(File.Exists(Path.Combine(directory, "clear_records.json")), Is.True);
            Assert.That(monsterAI.State, Is.EqualTo(MonsterState.Disabled));
        }

        [Test]
        public void StageClearPanel_FallbackCreatesBlockingPanelWithTitleButton()
        {
            var rootObject = new GameObject("StageClearTestRoot", typeof(RectTransform));
            try
            {
                var root = rootObject.GetComponent<RectTransform>();
                var sprite = Resources.Load<Sprite>(GameDirector.StageClearBackgroundResource);

                var ui = GameDirector.CreateStageClearPanel(root, sprite);
                var group = ui.panel.GetComponent<CanvasGroup>();

                Assert.That(ui.panel.name, Is.EqualTo("StageClearPanel"));
                Assert.That(ui.backgroundImage.sprite, Is.EqualTo(sprite));
                Assert.That(ui.titleButton, Is.Not.Null);
                Assert.That(group, Is.Not.Null);
                Assert.That(group.alpha, Is.EqualTo(0f));
                Assert.That(group.blocksRaycasts, Is.False);
                Assert.That(ui.panel.activeSelf, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(rootObject);
            }
        }

        [Test]
        public void GameDirector_ApplyFinalChaseStartStateForcesMonsterChaseAndFlag()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            session.AddItem("front_door_key");
            var monsterAI = new MonsterAIController();
            var flags = new FlagService(session);

            GameDirector.ApplyFinalChaseStartState(session, monsterAI, flags);
            monsterAI.Tick(new DangerSystem(), new HidingSystem());

            Assert.That(session.HasItem("front_door_key"), Is.True);
            Assert.That(session.HasFlag("final_chase_started"), Is.True);
            Assert.That(monsterAI.State, Is.EqualTo(MonsterState.Chase));
        }

        [Test]
        public void GameDirector_ApplyLaundryStorageBoxMonsterStartStateEnablesMonsterAndSetsFirstAppearanceOnce()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var monsterAI = new MonsterAIController();
            var flags = new FlagService(session);
            var danger = new DangerSystem();

            GameDirector.ApplyLaundryStorageBoxMonsterStartState(session, monsterAI, flags, danger);
            Assert.That(session.HasFlag(GameDirector.KitchenFirstAppearanceEventFlag), Is.True);
            Assert.That(monsterAI.State, Is.EqualTo(MonsterState.Normal));

            session.SetFlag("sentinel_flag");
            GameDirector.ApplyLaundryStorageBoxMonsterStartState(session, monsterAI, flags, danger);

            Assert.That(session.HasFlag(GameDirector.KitchenFirstAppearanceEventFlag), Is.True);
            Assert.That(session.HasFlag("sentinel_flag"), Is.True);
            Assert.That(monsterAI.State, Is.EqualTo(MonsterState.Normal));
        }

        [Test]
        public void EscapeActionResolver_FrontDoorKeyPickupMarksBasementAltarSolved()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var resolver = new EscapeActionResolver(session, new FlagService(session), stage.soundCatalog);
            var pickup = stage.rooms.First(room => room.roomId == "altar_room").interactables.First(item => item.interactableId == "front_door_key_on_altar");

            Assert.That(resolver.ResolveInteractable(pickup).Succeeded, Is.False);

            session.SetFlag("front_door_key_spawned");
            var result = resolver.ResolveInteractable(pickup);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.AcquireItem && action.Value == "front_door_key"), Is.True);
            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.MarkPuzzleSolved && action.Value == "basement_altar"), Is.True);
            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.SetFlag && action.Value == "event_front_door_key_appears"), Is.True);
        }

        [Test]
        public void GameDirector_ShouldRenderRoomHitboxHonorsConditionsAndUsedState()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var pickup = stage.rooms.First(room => room.roomId == "altar_room").interactables.First(item => item.interactableId == "front_door_key_on_altar");

            Assert.That(GameDirector.ShouldRenderRoomHitbox(pickup, session), Is.False);

            session.SetFlag("front_door_key_spawned");
            Assert.That(GameDirector.ShouldRenderRoomHitbox(pickup, session), Is.True);

            session.MarkInteractableUsed("front_door_key_on_altar");
            Assert.That(GameDirector.ShouldRenderRoomHitbox(pickup, session), Is.False);
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
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
            var puzzle = stage.puzzles.First(item => item.puzzleId == "study_safe");

            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(new[] { 3, 1, 4, 2 }, puzzle.answerTokens), Is.True);
            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(new[] { 3, 1, 4, 1 }, puzzle.answerTokens), Is.False);
            Assert.That(GameDirector.StudySafeDigitsMatchAnswer(new[] { 3, 1, 4 }, puzzle.answerTokens), Is.False);
        }

        [Test]
        public void StudySafeDigit_WrongAutoStateDoesNotSolveOrReward()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();
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
            session.Start(StageTestData.LoadStage1());

            session.RotateFace(-1);

            Assert.That(session.CurrentFaceDirection, Is.EqualTo(RoomFaceDirection.West));

            session.MoveTo("study");

            Assert.That(session.CurrentRoomId, Is.EqualTo("study"));
            Assert.That(session.CurrentFaceDirection, Is.EqualTo(RoomFaceDirection.North));
        }

        [Test]
        public void GameDirector_NextFaceDirectionTogglesTwoFaceHallways()
        {
            var stage = StageTestData.LoadStage1();
            var hallway = stage.rooms.First(item => item.roomId == "second_floor_hallway");
            var firstFloorHallway = stage.rooms.First(item => item.roomId == "first_floor_hallway");
            var childRoom = stage.rooms.First(item => item.roomId == "child_room");

            Assert.That(GameDirector.NextFaceDirection(hallway, RoomFaceDirection.North, 1), Is.EqualTo(RoomFaceDirection.South));
            Assert.That(GameDirector.NextFaceDirection(hallway, RoomFaceDirection.South, 1), Is.EqualTo(RoomFaceDirection.North));
            Assert.That(GameDirector.NextFaceDirection(hallway, RoomFaceDirection.North, -1), Is.EqualTo(RoomFaceDirection.South));
            Assert.That(GameDirector.NextFaceDirection(firstFloorHallway, RoomFaceDirection.North, 1), Is.EqualTo(RoomFaceDirection.South));
            Assert.That(GameDirector.NextFaceDirection(firstFloorHallway, RoomFaceDirection.South, -1), Is.EqualTo(RoomFaceDirection.North));
            Assert.That(GameDirector.NextFaceDirection(childRoom, RoomFaceDirection.North, 1), Is.EqualTo(RoomFaceDirection.East));
        }

        [Test]
        public void InteractionSystem_DoorInteraction_ReturnsMoveRoom()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var service = new InteractionSystem(session);
            var door = stage.rooms.First(room => room.roomId == "child_room").interactables.First(item => item.targetRoomId == "second_floor_hallway");

            var result = service.Resolve(door);

            Assert.That(result.ResultType, Is.EqualTo(InteractionResultType.MoveRoom));
            Assert.That(result.Value, Is.EqualTo("second_floor_hallway"));
        }

        [Test]
        public void EscapeActionResolver_MirrorExitReturnsMoveRoomAction()
        {
            var stage = StageTestData.LoadStage1();
            var session = new GameSession();
            session.Start(stage);
            var resolver = new EscapeActionResolver(session, new FlagService(session), stage.soundCatalog);
            var exit = stage.rooms.First(room => room.roomId == "mirror_room").interactables.First(item => item.interactableId == "mirror_exit");

            var result = resolver.ResolveInteractable(exit);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Actions.Any(action => action.Type == EscapeActionType.MoveRoom && action.Value == "second_floor_hallway"), Is.True);
        }

        [Test]
        public void EscapeActionResolver_OpenPuzzleDoesNotSolveImmediately()
        {
            var stage = StageTestData.LoadStage1();
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
            var stage = StageTestData.LoadStage1();

            Assert.That(stage.puzzles.All(puzzle => puzzle.closeUpResource == "EscapeFromNightmares/Puzzles/" + puzzle.puzzleId), Is.True);
        }

        [Test]
        public void Stage1Data_FaceCloseUpAndPuzzleResourcesLoadAsSprites()
        {
            var stage = StageTestData.LoadStage1();

            foreach (var room in stage.rooms)
            {
                foreach (var face in room.faces)
                {
                    foreach (var interactable in face.interactables)
                    {
                        AssertSpriteLoads(interactable.clueViewResource, interactable.interactableId);
                        AssertSpriteLoads(interactable.closeUpClosedResource, interactable.interactableId);
                        AssertSpriteLoads(interactable.closeUpOpenWithItemResource, interactable.interactableId);
                        AssertSpriteLoads(interactable.closeUpOpenEmptyResource, interactable.interactableId);

                        if (!string.IsNullOrEmpty(interactable.puzzleId))
                        {
                            var puzzle = stage.puzzles.First(item => item.puzzleId == interactable.puzzleId);
                            AssertSpriteLoads(puzzle.closeUpResource, interactable.interactableId);
                        }
                    }
                }
            }
        }

        [Test]
        public void Stage1Data_CloseUpInteractablesArePlacedOnOneFaceOnly()
        {
            var stage = StageTestData.LoadStage1();

            foreach (var room in stage.rooms)
            {
                var closeUpInteractables = room.faces
                    .SelectMany(face => face.interactables.Select(interactable => new { face.direction, interactable }))
                    .Where(entry => HasCloseUpOrPuzzleResource(entry.interactable))
                    .GroupBy(entry => entry.interactable.interactableId);

                foreach (var group in closeUpInteractables)
                {
                    Assert.That(group.Select(entry => entry.direction).Distinct().Count(), Is.EqualTo(1), room.roomId + "/" + group.Key);
                }
            }
        }

        [Test]
        public void Stage1Data_AiGeneratedCloseUpAndPuzzleAssetsLoadAndUseExpectedDimensions()
        {
            foreach (var resourcePath in AiGeneratedCloseUpAndPuzzleResources())
            {
                AssertSpriteLoads(resourcePath, resourcePath);
                AssertPngResourceSize(resourcePath, 1280, 720);
            }

            for (var digit = 0; digit <= 9; digit++)
            {
                var resourcePath = GameDirector.StudySafeDigitResource(digit);
                AssertSpriteLoads(resourcePath, resourcePath);
                AssertPngResourceSize(resourcePath, 180, 220);
            }
        }

        [Test]
        public void Stage1Data_StageClearEndingAssetLoadsAndIsRegistered()
        {
            Assert.That(GameDirector.StageClearBackgroundResource, Is.EqualTo("EscapeFromNightmares/Endings/stage1_clear_background"));
            AssertSpriteLoads(GameDirector.StageClearBackgroundResource, "stage clear background");
            AssertPngResourceSize(GameDirector.StageClearBackgroundResource, 1280, 720);

            var catalog = AssetDatabase.LoadAssetAtPath<RoomSpriteCatalog>("Assets/EscapeFromNightmares/ScriptableObjects/RoomSpriteCatalog.asset");

            Assert.That(catalog, Is.Not.Null);
            Assert.That(catalog.Sprites.Any(entry => entry.spriteId == "stage1_clear_background" && entry.sprite != null), Is.True);
        }

        [Test]
        public void Stage1Data_MonsterShadowAssetLoadsAndIsRegistered()
        {
            Assert.That(GameDirector.MonsterShadowResource, Is.EqualTo("EscapeFromNightmares/Monster/monster_shadow"));
            AssertSpriteLoads(GameDirector.MonsterShadowResource, "monster shadow object");

            var catalog = AssetDatabase.LoadAssetAtPath<RoomSpriteCatalog>("Assets/EscapeFromNightmares/ScriptableObjects/RoomSpriteCatalog.asset");

            Assert.That(catalog, Is.Not.Null);
            Assert.That(catalog.Sprites.Any(entry => entry.spriteId == "monster_shadow" && entry.sprite != null), Is.True);
        }

        [Test]
        public void Stage1Data_StrictIdentityCloseUpCasesAreDocumentedAsAiGeneratedExceptions()
        {
            const string harnessPath = "docs/ROOM_IMAGE_HARNESS.md";
            if (!File.Exists(harnessPath))
            {
                Assert.Ignore(harnessPath + " is not present in this worktree.");
            }

            var harness = File.ReadAllText(harnessPath);

            foreach (var entry in StrictIdentityCloseUpCases())
            {
                Assert.That(Resources.Load<Sprite>(entry.sourceResourcePath), Is.Not.Null, entry.sourceResourcePath);
                Assert.That(Resources.Load<Sprite>(entry.targetResourcePath), Is.Not.Null, entry.targetResourcePath);
                Assert.That(harness, Does.Contain(entry.targetResourcePath), entry.targetResourcePath);
                Assert.That(harness, Does.Contain("Exception"), entry.targetResourcePath);
                Assert.That(harness, Does.Contain("AI-generated replacement"), entry.targetResourcePath);
            }
        }

        [Test]
        public void Documentation_StrictIdentityCloseUpCasesRecordCropRects()
        {
            const string harnessPath = "docs/ROOM_IMAGE_HARNESS.md";
            if (!File.Exists(harnessPath))
            {
                Assert.Ignore(harnessPath + " is not present in this worktree.");
            }

            var harness = File.ReadAllText(harnessPath);

            foreach (var entry in StrictIdentityCloseUpCases())
            {
                Assert.That(harness, Does.Contain(entry.targetResourcePath), entry.targetResourcePath);
                Assert.That(harness, Does.Contain(entry.cropDescription), entry.targetResourcePath);
            }
        }

        [Test]
        public void EscapeActionResolver_OneShotUsedInteractableFails()
        {
            var stage = StageTestData.LoadStage1();
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
            var catalog = StageTestData.LoadStage1().soundCatalog;

            Assert.That(catalog.TryFind("ui_click", out var entry), Is.True);
            Assert.That(entry.category, Is.EqualTo(SoundCategory.Ui));
            Assert.That(entry.resourcePath, Is.EqualTo("EscapeFromNightmares/Audio/UI/ui_click"));
        }

        [Test]
        public void SoundCatalog_TryFind_ReturnsDrawerSoundEntries()
        {
            var catalog = StageTestData.LoadStage1().soundCatalog;

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
            var stage = StageTestData.LoadStage1();
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

        private static bool IsExitDoor(InteractableDefinition interactable)
        {
            return interactable != null
                && (interactable.type == InteractableType.Door
                    || interactable.type == InteractableType.LockedDoor
                    || interactable.type == InteractableType.EscapeDoor);
        }

        private static bool HasCloseUpOrPuzzleResource(InteractableDefinition interactable)
        {
            return interactable != null
                && (!string.IsNullOrEmpty(interactable.clueViewResource)
                    || !string.IsNullOrEmpty(interactable.closeUpClosedResource)
                    || !string.IsNullOrEmpty(interactable.closeUpOpenWithItemResource)
                    || !string.IsNullOrEmpty(interactable.closeUpOpenEmptyResource)
                    || !string.IsNullOrEmpty(interactable.puzzleId));
        }

        private static void AssertSpriteLoads(string resourcePath, string context)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }

            Assert.That(Resources.Load<Sprite>(resourcePath), Is.Not.Null, context + " -> " + resourcePath);
        }

        private static void AssertPngResourceSize(string resourcePath, int width, int height)
        {
            var texture = LoadPngResource(resourcePath);
            try
            {
                Assert.That(texture.width, Is.EqualTo(width), resourcePath);
                Assert.That(texture.height, Is.EqualTo(height), resourcePath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static string[] AiGeneratedCloseUpAndPuzzleResources()
        {
            return new[]
            {
                "EscapeFromNightmares/CloseUps/bathroom_mirror_rule_clue",
                "EscapeFromNightmares/CloseUps/child_desk_drawer_closed",
                "EscapeFromNightmares/CloseUps/child_desk_drawer_open_empty",
                "EscapeFromNightmares/CloseUps/child_desk_drawer_open_with_item",
                "EscapeFromNightmares/CloseUps/child_desk_surface",
                "EscapeFromNightmares/CloseUps/child_drawing_board",
                "EscapeFromNightmares/CloseUps/child_window_view",
                "EscapeFromNightmares/CloseUps/dressing_color_sequence_clue",
                "EscapeFromNightmares/CloseUps/dining_seat_order_clue",
                "EscapeFromNightmares/CloseUps/kitchen_clock_clue",
                "EscapeFromNightmares/CloseUps/study_clue_board",
                "EscapeFromNightmares/CloseUps/study_desk_surface",
                "EscapeFromNightmares/CloseUps/study_portrait",
                "EscapeFromNightmares/CloseUps/study_safe_clue_note",
                "EscapeFromNightmares/CloseUps/study_safe_locked",
                "EscapeFromNightmares/CloseUps/study_safe_open_empty",
                "EscapeFromNightmares/CloseUps/study_safe_open_with_item",
                "EscapeFromNightmares/CloseUps/study_safe_surrounding",
                "EscapeFromNightmares/CloseUps/study_window_view",
                "EscapeFromNightmares/CloseUps/attic_family_album_photo",
                "EscapeFromNightmares/Puzzles/attic_toy_sequence",
                "EscapeFromNightmares/Puzzles/basement_altar",
                "EscapeFromNightmares/Puzzles/breaker_box",
                "EscapeFromNightmares/Puzzles/front_door_escape",
                "EscapeFromNightmares/Puzzles/laundry_storage_box",
                "EscapeFromNightmares/Puzzles/master_bedroom_drawer",
                "EscapeFromNightmares/Puzzles/mirror_symbol_panel",
                "EscapeFromNightmares/Puzzles/study_safe"
            };
        }

        private static StrictIdentityCloseUpCase[] StrictIdentityCloseUpCases()
        {
            return new[]
            {
                new StrictIdentityCloseUpCase(
                    "EscapeFromNightmares/Rooms/second_floor_bathroom_north",
                    "EscapeFromNightmares/CloseUps/bathroom_mirror_rule_clue",
                    new Rect(0.26f, 0.24f, 0.48f, 0.48f),
                    "Rect(0.26, 0.24, 0.48, 0.48)"),
                new StrictIdentityCloseUpCase(
                    "EscapeFromNightmares/Rooms/dressing_room_north",
                    "EscapeFromNightmares/CloseUps/dressing_color_sequence_clue",
                    new Rect(0.22f, 0.18f, 0.56f, 0.56f),
                    "Rect(0.22, 0.18, 0.56, 0.56)"),
                new StrictIdentityCloseUpCase(
                    "EscapeFromNightmares/Rooms/mirror_room_north",
                    "EscapeFromNightmares/Puzzles/mirror_symbol_panel",
                    new Rect(0.32f, 0.13f, 0.36f, 0.36f),
                    "Rect(0.32, 0.13, 0.36, 0.36)"),
                new StrictIdentityCloseUpCase(
                    "EscapeFromNightmares/Rooms/master_bedroom_north",
                    "EscapeFromNightmares/Puzzles/master_bedroom_drawer",
                    new Rect(0.36f, 0.20f, 0.48f, 0.48f),
                    "Rect(0.36, 0.20, 0.48, 0.48)")
            };
        }

        private static Texture2D LoadPngResource(string resourcePath)
        {
            var path = Path.Combine(Application.dataPath, "EscapeFromNightmares/Resources/" + resourcePath + ".png");
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Assert.That(texture.LoadImage(File.ReadAllBytes(path)), Is.True, resourcePath);
            return texture;
        }

        private static float MaxCropDelta(Texture2D source, Texture2D target, Rect cropRect)
        {
            var maxDelta = 0f;
            for (var y = 0; y < target.height; y += 45)
            {
                for (var x = 0; x < target.width; x += 64)
                {
                    var expected = SampleCropPixel(source, cropRect, x, y, target.width, target.height);
                    var actual = target.GetPixel(x, y);
                    maxDelta = Mathf.Max(maxDelta, Mathf.Abs(expected.r - actual.r));
                    maxDelta = Mathf.Max(maxDelta, Mathf.Abs(expected.g - actual.g));
                    maxDelta = Mathf.Max(maxDelta, Mathf.Abs(expected.b - actual.b));
                    maxDelta = Mathf.Max(maxDelta, Mathf.Abs(expected.a - actual.a));
                }
            }

            return maxDelta;
        }

        private static Color SampleCropPixel(Texture2D source, Rect cropRect, int targetX, int targetY, int targetWidth, int targetHeight)
        {
            var sourceX = cropRect.x * source.width;
            var sourceY = cropRect.y * source.height;
            var sourceWidth = cropRect.width * source.width;
            var sourceHeight = cropRect.height * source.height;
            var sampleX = sourceX + (targetX + 0.5f) / targetWidth * sourceWidth - 0.5f;
            var sampleY = sourceY + (targetY + 0.5f) / targetHeight * sourceHeight - 0.5f;
            return SampleBilinear(source, sampleX, sampleY);
        }

        private static Color SampleBilinear(Texture2D texture, float x, float y)
        {
            x = Mathf.Clamp(x, 0f, texture.width - 1f);
            y = Mathf.Clamp(y, 0f, texture.height - 1f);
            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var x1 = Mathf.Min(x0 + 1, texture.width - 1);
            var y1 = Mathf.Min(y0 + 1, texture.height - 1);
            var tx = x - x0;
            var ty = y - y0;
            var bottom = Color.Lerp(texture.GetPixel(x0, y0), texture.GetPixel(x1, y0), tx);
            var top = Color.Lerp(texture.GetPixel(x0, y1), texture.GetPixel(x1, y1), tx);
            return Color.Lerp(bottom, top, ty);
        }

        private sealed class StrictIdentityCloseUpCase
        {
            public readonly string sourceResourcePath;
            public readonly string targetResourcePath;
            public readonly Rect cropRect;
            public readonly string cropDescription;

            public StrictIdentityCloseUpCase(string sourceResourcePath, string targetResourcePath, Rect cropRect, string cropDescription)
            {
                this.sourceResourcePath = sourceResourcePath;
                this.targetResourcePath = targetResourcePath;
                this.cropRect = cropRect;
                this.cropDescription = cropDescription;
            }
        }
    }
}
