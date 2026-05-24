using System.Collections.Generic;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    public static class RuntimeStageFactory
    {
        public static StageDefinition CreateStage1()
        {
            var stage = ScriptableObject.CreateInstance<StageDefinition>();
            stage.stageId = "stage1";
            stage.displayName = "Stage 1 - Nightmare House";
            stage.startRoomId = "child_room";
            stage.clearFlag = "stage1_clear";

            stage.items = CreateItems();
            stage.puzzles = CreatePuzzles();
            stage.rooms = CreateRooms();
            stage.monsterNodeGraph = CreateMonsterGraph(stage.rooms);
            stage.soundCatalog = CreateSoundCatalog();
            return stage;
        }

        private static List<ItemDefinition> CreateItems()
        {
            return new List<ItemDefinition>
            {
                Item("torn_drawing_fragment", "찢어진 그림 조각", ItemType.ClueItem, ""),
                Item("study_safe_clue", "서재 금고 단서", ItemType.ClueItem, ""),
                Item("fuse_holder", "퓨즈 홀더", ItemType.MechanicalPart, ""),
                Item("fuse", "퓨즈", ItemType.MechanicalPart, ""),
                Item("broken_hand_mirror", "깨진 손거울", ItemType.AltarObject, "cracked_circle"),
                Item("small_doll", "작은 인형", ItemType.AltarObject, "child_hand"),
                Item("old_keychain", "낡은 열쇠고리", ItemType.AltarObject, "keyhole"),
                Item("old_necklace", "낡은 목걸이", ItemType.AltarObject, "heart"),
                Item("symbol_fragment", "문양 조각", ItemType.ClueItem, ""),
                Item("front_door_key", "현관 열쇠", ItemType.KeyItem, "")
            };
        }

        private static List<PuzzleDefinition> CreatePuzzles()
        {
            var studySafe = Puzzle("study_safe", "Study Safe", PuzzleType.NumberLock, new[] {"3", "1", "4", "2"}, new[] {"fuse_holder"}, "study_safe_unlocked", null);
            studySafe.deferSolvedUntilRewardPickup = true;
            return new List<PuzzleDefinition>
            {
                studySafe,
                Puzzle("laundry_storage_box", "세탁실 보관함", PuzzleType.NumberLock, new[] {"0", "9", "1", "5"}, new[] {"fuse"}, "puzzle_laundry_box_clear", null, 10f),
                Puzzle("breaker_box", "차단기함", PuzzleType.ItemUse, new[] {"fuse_holder", "fuse"}, new[] {"old_keychain"}, "electricity_restored", new[] {"fuse_holder", "fuse"}),
                Puzzle("mirror_symbol_panel", "거울 방 문양 패널", PuzzleType.SymbolSequence, new[] {"heart", "child_hand", "cracked_circle", "keyhole"}, new[] {"broken_hand_mirror"}, "mirror_destroyed", null),
                Puzzle("master_bedroom_drawer", "안방 색상 서랍", PuzzleType.ColorSequence, new[] {"black", "white", "red", "gray"}, new[] {"old_necklace"}, "master_drawer_opened", null),
                Puzzle("attic_toy_sequence", "다락방 장난감 순서", PuzzleType.SilentSequence, new[] {"doll", "train", "block", "bell"}, new[] {"small_doll", "symbol_fragment"}, "attic_toy_box_opened", new[] {"torn_drawing_fragment"}, 15f),
                Puzzle("basement_altar", "지하실 제단", PuzzleType.SymbolItemMatching, new[] {"broken_hand_mirror", "small_doll", "old_keychain", "old_necklace"}, new[] {"front_door_key"}, "front_door_key_spawned", new[] {"broken_hand_mirror", "small_doll", "old_keychain", "old_necklace"}),
                Puzzle("front_door_escape", "현관문 탈출", PuzzleType.ItemUse, new[] {"front_door_key"}, System.Array.Empty<string>(), "stage1_clear", new[] {"front_door_key"})
            };
        }

        private static List<RoomDefinition> CreateRooms()
        {
            return new List<RoomDefinition>
            {
                ChildRoom(),
                SecondFloorHallway(),
                Study(),
                Room("mirror_room", "거울 방", "2F", new[] {"second_floor_hallway"}, 0, PuzzleObject("mirror_panel_obj", "문양 패널", "mirror_symbol_panel"), Door("mirror_exit", "복도로 나가기", "second_floor_hallway")),
                Room("master_bedroom", "안방", "2F", new[] {"second_floor_hallway"}, 1, PuzzleObject("master_drawer_obj", "색상 서랍", "master_bedroom_drawer"), Hide("master_closet_hide", "옷장"), Door("master_exit", "복도로 나가기", "second_floor_hallway")),
                Room("dressing_room", "드레스룸", "2F", new[] {"second_floor_hallway"}, 1, I("color_clue", "옷 색상 순서 단서", InteractableType.ClueObject, eventId: "clue_color_sequence"), Door("dressing_exit", "복도로 나가기", "second_floor_hallway")),
                Room("second_floor_bathroom", "2층 욕실", "2F", new[] {"second_floor_hallway"}, 1, I("mirror_rule_clue", "반전 규칙 단서", InteractableType.ClueObject, eventId: "clue_mirror_rule"), Door("bathroom_exit", "복도로 나가기", "second_floor_hallway")),
                Room("stairwell_2f", "2층 계단", "2F", new[] {"second_floor_hallway", "stairwell_1f", "attic_main"}, 0, Doors("second_floor_hallway", "stairwell_1f", "attic_main")),
                Room("attic_main", "다락방", "Attic", new[] {"stairwell_2f", "attic_toy_storage"}, 1, Door("attic_toy_door", "장난감 보관실", "attic_toy_storage"), Door("attic_exit", "계단으로", "stairwell_2f")),
                Room("attic_toy_storage", "다락 장난감 보관실", "Attic", new[] {"attic_main"}, 1, PuzzleObject("attic_toy_box", "장난감 상자", "attic_toy_sequence"), Door("attic_toy_exit", "다락방으로", "attic_main")),
                Room("stairwell_1f", "1층 계단", "1F", new[] {"stairwell_2f", "first_floor_hallway"}, 0, Doors("stairwell_2f", "first_floor_hallway")),
                Room("first_floor_hallway", "1층 복도", "1F", new[] {"stairwell_1f", "entrance", "living_room", "dining_room", "kitchen", "laundry_room", "family_photo_room"}, 1, Doors("stairwell_1f", "entrance", "living_room", "dining_room", "kitchen", "laundry_room", "family_photo_room")),
                Room("family_photo_room", "가족사진 방", "1F", new[] {"first_floor_hallway"}, 1, I("hidden_photo_drawer", "숨겨진 사진 서랍", InteractableType.ItemPickup, grantsItemId: "study_safe_clue"), Door("family_photo_exit", "복도로 나가기", "first_floor_hallway")),
                Room("dining_room", "식당", "1F", new[] {"first_floor_hallway"}, 0, I("dining_clue", "식탁 좌석 순서", InteractableType.ClueObject, eventId: "clue_dining_order"), Door("dining_exit", "복도로 나가기", "first_floor_hallway")),
                Room("kitchen", "부엌", "1F", new[] {"first_floor_hallway"}, 1, I("kitchen_clock", "부엌 시계", InteractableType.EventTrigger, eventId: "event_kitchen_first_appearance"), Hide("kitchen_hide", "싱크대 아래"), Door("kitchen_exit", "복도로 나가기", "first_floor_hallway")),
                Room("laundry_room", "세탁실", "1F", new[] {"first_floor_hallway", "basement_entry"}, 1, PuzzleObject("laundry_box", "세탁실 보관함", "laundry_storage_box"), PuzzleObject("breaker_box_obj", "차단기함", "breaker_box"), Hide("laundry_hide", "세탁기 옆"), Door("laundry_exit", "복도로 나가기", "first_floor_hallway"), Door("basement_entry_door", "지하실 입구", "basement_entry")),
                Room("living_room", "거실", "1F", new[] {"first_floor_hallway", "entrance"}, 1, Combine(Hide("living_curtain_hide", "커튼 뒤"), Doors("first_floor_hallway", "entrance"))),
                Room("entrance", "현관", "1F", new[] {"first_floor_hallway", "living_room"}, 0, Combine(PuzzleObject("front_door", "현관문", "front_door_escape"), Doors("first_floor_hallway", "living_room"))),
                Room("basement_entry", "지하실 입구", "Basement", new[] {"laundry_room", "basement_main"}, 0, Doors("laundry_room", "basement_main")),
                Room("basement_main", "지하실", "Basement", new[] {"basement_entry", "altar_room"}, 1, Door("altar_room_door", "제단 방", "altar_room"), Door("basement_exit", "지하실 입구", "basement_entry")),
                Room("altar_room", "제단 방", "Basement", new[] {"basement_main"}, 0, PuzzleObject("altar", "제단", "basement_altar"), Door("altar_exit", "지하실로", "basement_main"))
            };
        }

        private static MonsterNodeGraph CreateMonsterGraph(List<RoomDefinition> rooms)
        {
            var graph = ScriptableObject.CreateInstance<MonsterNodeGraph>();
            foreach (var room in rooms)
            {
                graph.nodes.Add(new MonsterNode
                {
                    nodeId = room.roomId + "_node",
                    roomId = room.roomId,
                    connectedNodeIds = System.Array.ConvertAll(room.connectedRoomIds, connected => connected + "_node")
                });
            }

            return graph;
        }

        private static SoundCatalog CreateSoundCatalog()
        {
            var catalog = ScriptableObject.CreateInstance<SoundCatalog>();
            catalog.entries.Add(new SoundEntry { soundId = "bgm_stage1_ambient", resourcePath = "EscapeFromNightmares/Audio/BGM/bgm_stage1_ambient", category = SoundCategory.Bgm, loop = true });
            catalog.entries.Add(new SoundEntry { soundId = "bgm_final_chase", resourcePath = "EscapeFromNightmares/Audio/BGM/bgm_final_chase", category = SoundCategory.Bgm, loop = true });
            catalog.entries.Add(new SoundEntry { soundId = "ui_click", resourcePath = "EscapeFromNightmares/Audio/UI/ui_click", category = SoundCategory.Ui });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_confirm", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_confirm", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_door", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_door", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_item_pickup", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_item_pickup", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_drawer_open", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_drawer_open", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_drawer_close", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_drawer_close", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_hide", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_hide", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_puzzle_success", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_puzzle_success", category = SoundCategory.Sfx });
            catalog.entries.Add(new SoundEntry { soundId = "sfx_puzzle_failure", resourcePath = "EscapeFromNightmares/Audio/SFX/sfx_puzzle_failure", category = SoundCategory.Sfx });
            return catalog;
        }

        private static ItemDefinition Item(string id, string name, ItemType type, string altarSymbol)
        {
            var item = ScriptableObject.CreateInstance<ItemDefinition>();
            item.itemId = id;
            item.displayName = name;
            item.itemType = type;
            item.altarSymbol = altarSymbol;
            item.iconResource = "EscapeFromNightmares/Items/item_" + id;
            return item;
        }

        private static PuzzleDefinition Puzzle(string id, string name, PuzzleType type, string[] answer, string[] rewards, string flag, string[] requiredItems, float failureDanger = 8f)
        {
            var puzzle = ScriptableObject.CreateInstance<PuzzleDefinition>();
            puzzle.puzzleId = id;
            puzzle.displayName = name;
            puzzle.puzzleType = type;
            puzzle.answerTokens = answer;
            puzzle.rewardItemIds = rewards;
            puzzle.successFlag = flag;
            puzzle.successEventId = "event_" + id + "_success";
            puzzle.requiredItemIds = requiredItems ?? System.Array.Empty<string>();
            puzzle.failureDanger = failureDanger;
            puzzle.successSoundId = "sfx_puzzle_success";
            puzzle.failureSoundId = "sfx_puzzle_failure";
            puzzle.closeUpResource = "EscapeFromNightmares/Puzzles/" + id;
            return puzzle;
        }

        private static RoomDefinition Room(string id, string name, string floor, string[] connected, int hideSpotCount, params InteractableDefinition[] interactables)
        {
            var room = ScriptableObject.CreateInstance<RoomDefinition>();
            room.roomId = id;
            room.displayName = name;
            room.floorName = floor;
            room.backgroundResource = RoomFaceResource(id, RoomFaceDirection.North);
            room.connectedRoomIds = connected;
            room.hideSpotCount = hideSpotCount;
            room.interactables = interactables;
            room.faces = CreateFaces(id, interactables);
            room.allowMonster = id != "child_room";
            return room;
        }

        private static RoomDefinition SecondFloorHallway()
        {
            var childRoomDoor = HallwayDoor("door_to_child_room", "Child Room", "child_room", new Rect(0.03f, 0.09f, 0.34f, 0.76f));
            var studyDoor = HallwayDoor("door_to_study", "Study", "study", new Rect(0.78f, 0.12f, 0.2f, 0.72f));
            var bathroomDoor = HallwayDoor("door_to_second_floor_bathroom", "Bathroom", "second_floor_bathroom", new Rect(0.68f, 0.2f, 0.08f, 0.42f));
            var mirrorRoomDoor = HallwayDoor("door_to_mirror_room", "Mirror Room", "mirror_room", new Rect(0.53f, 0.24f, 0.07f, 0.36f));
            var masterBedroomDoor = HallwayDoor("door_to_master_bedroom", "Master Bedroom", "master_bedroom", new Rect(0.27f, 0.1f, 0.18f, 0.72f));
            var dressingRoomDoor = HallwayDoor("door_to_dressing_room", "Dressing Room", "dressing_room", new Rect(0.53f, 0.2f, 0.1f, 0.48f));
            var stairwellDoor = HallwayDoor("door_to_stairwell_2f", "Stairwell", "stairwell_2f", new Rect(0.57f, 0.42f, 0.26f, 0.45f));

            var room = Room(
                "second_floor_hallway",
                "2F Hallway",
                "2F",
                new[] { "child_room", "study", "mirror_room", "master_bedroom", "dressing_room", "second_floor_bathroom", "stairwell_2f" },
                0,
                childRoomDoor,
                studyDoor,
                mirrorRoomDoor,
                masterBedroomDoor,
                dressingRoomDoor,
                bathroomDoor,
                stairwellDoor);

            room.faces = new[]
            {
                Face("second_floor_hallway", RoomFaceDirection.North, "Child Room End", new[] { childRoomDoor, studyDoor, bathroomDoor, mirrorRoomDoor }),
                Face("second_floor_hallway", RoomFaceDirection.South, "Stairwell End", new[] { masterBedroomDoor, dressingRoomDoor, stairwellDoor })
            };

            return room;
        }

        private static InteractableDefinition HallwayDoor(string id, string name, string targetRoomId, Rect hitbox)
        {
            var door = Door(id, name, targetRoomId);
            door.normalizedHitbox = hitbox;
            door.showWorldImage = false;
            return door;
        }

        private static RoomDefinition ChildRoom()
        {
            var drawer = I("child_desk_drawer", "Child Desk Drawer", InteractableType.ItemPickup, grantsItemId: "torn_drawing_fragment");
            var deskSurface = Clue("child_desk_surface", "Child Desk Surface", "EscapeFromNightmares/CloseUps/child_desk_surface", new Rect(0.14f, 0.22f, 0.26f, 0.22f));
            var drawingBoard = Clue("child_drawing_board", "Child Drawing Board", "EscapeFromNightmares/CloseUps/child_drawing_board", new Rect(0.45f, 0.35f, 0.2f, 0.28f));
            var windowView = Clue("child_window_view", "Child Window View", "EscapeFromNightmares/CloseUps/child_window_view", new Rect(0.08f, 0.2f, 0.19f, 0.62f));
            var door = Door("child_room_door", "Hallway", "second_floor_hallway");
            var hide = Hide("child_bed_hide", "Child Bed");
            var silhouette = I("child_window_silhouette", "Window Silhouette", InteractableType.EventTrigger, eventId: "event_window_silhouette");

            var room = Room(
                "child_room",
                "Child Room",
                "2F",
                new[] {"second_floor_hallway"},
                1,
                drawer,
                deskSurface,
                drawingBoard,
                door,
                hide,
                silhouette,
                windowView);

            room.faces = new[]
            {
                WithBackground(Face("child_room", RoomFaceDirection.North, "Desk Wall", new[] { drawer, deskSurface, drawingBoard }), "EscapeFromNightmares/Rooms/child_room_north_drawer_empty", new ConditionDefinition { requiredItemIds = new[] { "torn_drawing_fragment" } }),
                Face("child_room", RoomFaceDirection.East, "Hallway Door", new[] { door }),
                Face("child_room", RoomFaceDirection.South, "Bed", new[] { hide }),
                Face("child_room", RoomFaceDirection.West, "Window", new[] { silhouette, windowView })
            };

            return room;
        }

        private static RoomDefinition Study()
        {
            var safe = PuzzleObject("study_safe_obj", "Study Safe", "study_safe");
            safe.normalizedHitbox = new Rect(0.49f, 0.14f, 0.28f, 0.72f);
            safe.showWorldImage = false;
            safe.closeUpClosedResource = "EscapeFromNightmares/CloseUps/study_safe_locked";
            safe.closeUpOpenWithItemResource = "EscapeFromNightmares/CloseUps/study_safe_open_with_item";
            safe.closeUpOpenEmptyResource = "EscapeFromNightmares/CloseUps/study_safe_open_empty";
            safe.closeUpItemId = "fuse_holder";
            safe.closeUpItemHitbox = new Rect(0.48f, 0.39f, 0.16f, 0.18f);
            safe.closeUpOpenSoundId = "sfx_drawer_open";
            safe.closeUpCloseSoundId = "sfx_drawer_close";

            var clue = I("study_safe_clue_note", "Safe Clue Note", InteractableType.ClueObject, eventId: "clue_study_safe_note");
            clue.clueViewResource = "EscapeFromNightmares/CloseUps/study_safe_clue_note";
            clue.normalizedHitbox = new Rect(0.33f, 0.45f, 0.36f, 0.34f);
            clue.showWorldImage = false;

            var safeSurrounding = Clue("study_safe_surrounding", "Study Safe Surrounding", "EscapeFromNightmares/CloseUps/study_safe_surrounding", new Rect(0.43f, 0.16f, 0.36f, 0.68f));
            var deskSurface = Clue("study_desk_surface", "Study Desk Surface", "EscapeFromNightmares/CloseUps/study_desk_surface", new Rect(0.2f, 0.2f, 0.56f, 0.38f));
            var clueBoard = Clue("study_clue_board", "Study Clue Board", "EscapeFromNightmares/CloseUps/study_clue_board", new Rect(0.34f, 0.47f, 0.3f, 0.34f));
            var portrait = Clue("study_portrait", "Study Portrait", "EscapeFromNightmares/CloseUps/study_portrait", new Rect(0.32f, 0.48f, 0.16f, 0.28f));
            var windowView = Clue("study_window_view", "Study Window View", "EscapeFromNightmares/CloseUps/study_window_view", new Rect(0.03f, 0.21f, 0.22f, 0.58f));

            var exit = Door("study_exit", "Back to Hallway", "second_floor_hallway");
            exit.normalizedHitbox = new Rect(0.68f, 0.14f, 0.22f, 0.72f);
            exit.showWorldImage = false;

            var room = Room(
                "study",
                "Study",
                "2F",
                new[] { "second_floor_hallway" },
                1,
                safe,
                safeSurrounding,
                clue,
                deskSurface,
                clueBoard,
                portrait,
                windowView,
                exit);

            room.faces = new[]
            {
                WithBackground(
                    Face("study", RoomFaceDirection.North, "Safe Wall", new[] { safeSurrounding, safe }),
                    new[]
                    {
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/study_north_safe_open_empty",
                            conditions = new ConditionDefinition { requiredFlagIds = new[] { "study_safe_opened" }, requiredItemIds = new[] { "fuse_holder" } }
                        },
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/study_north_safe_open_with_item",
                            conditions = new ConditionDefinition { requiredFlagIds = new[] { "study_safe_opened" }, forbiddenFlagIds = new[] { "puzzle_study_safe_clear" } }
                        },
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/study_north_safe_open",
                            conditions = new ConditionDefinition { requiredFlagIds = new[] { "study_safe_opened" } }
                        }
                    }),
                Face("study", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("study", RoomFaceDirection.South, "Desk Clue", new[] { clue, deskSurface, clueBoard }),
                Face("study", RoomFaceDirection.West, "Moonlit Window", new[] { portrait, windowView })
            };

            return room;
        }

        private static RoomFaceDefinition[] CreateFaces(string roomId, InteractableDefinition[] interactables)
        {
            var buckets = new[]
            {
                new List<InteractableDefinition>(),
                new List<InteractableDefinition>(),
                new List<InteractableDefinition>(),
                new List<InteractableDefinition>()
            };

            for (var index = 0; index < interactables.Length; index++)
            {
                buckets[FaceIndexFor(interactables[index], index)].Add(interactables[index]);
            }

            return new[]
            {
                Face(roomId, RoomFaceDirection.North, "북쪽 면", buckets[0].ToArray()),
                Face(roomId, RoomFaceDirection.East, "동쪽 면", buckets[1].ToArray()),
                Face(roomId, RoomFaceDirection.South, "남쪽 면", buckets[2].ToArray()),
                Face(roomId, RoomFaceDirection.West, "서쪽 면", buckets[3].ToArray())
            };
        }

        private static int FaceIndexFor(InteractableDefinition interactable, int index)
        {
            switch (interactable.interactableId)
            {
                case "child_desk_drawer":
                    return 0;
                case "child_room_door":
                    return 1;
                case "child_bed_hide":
                    return 2;
                case "child_window_silhouette":
                    return 3;
            }

            switch (interactable.type)
            {
                case InteractableType.ItemPickup:
                case InteractableType.PuzzleObject:
                case InteractableType.ClueObject:
                case InteractableType.EventTrigger:
                    return 0;
                case InteractableType.HideSpot:
                    return 2;
                case InteractableType.Door:
                case InteractableType.LockedDoor:
                case InteractableType.EscapeDoor:
                    return index % 3 + 1;
                default:
                    return 0;
            }
        }

        private static RoomFaceDefinition Face(string roomId, RoomFaceDirection direction, string name, InteractableDefinition[] interactables)
        {
            return new RoomFaceDefinition
            {
                direction = direction,
                displayName = name,
                backgroundResource = RoomFaceResource(roomId, direction),
                interactables = interactables
            };
        }

        private static string RoomFaceResource(string roomId, RoomFaceDirection direction)
        {
            return "EscapeFromNightmares/Rooms/" + roomId + "_" + direction.ToString().ToLowerInvariant();
        }

        private static InteractableDefinition[] Doors(params string[] roomIds)
        {
            var doors = new InteractableDefinition[roomIds.Length];
            for (var index = 0; index < roomIds.Length; index++)
            {
                doors[index] = Door("door_to_" + roomIds[index], roomIds[index] + "로 이동", roomIds[index]);
            }

            return doors;
        }

        private static InteractableDefinition[] Combine(params object[] entries)
        {
            var combined = new List<InteractableDefinition>();
            foreach (var entry in entries)
            {
                if (entry is InteractableDefinition interactable)
                {
                    combined.Add(interactable);
                }
                else if (entry is InteractableDefinition[] interactables)
                {
                    combined.AddRange(interactables);
                }
            }

            return combined.ToArray();
        }

        private static InteractableDefinition Door(string id, string name, string targetRoomId)
        {
            return I(id, name, InteractableType.Door, targetRoomId: targetRoomId);
        }

        private static InteractableDefinition Hide(string id, string name)
        {
            return I(id, name, InteractableType.HideSpot);
        }

        private static InteractableDefinition PuzzleObject(string id, string name, string puzzleId)
        {
            return I(id, name, InteractableType.PuzzleObject, puzzleId: puzzleId);
        }

        private static InteractableDefinition Clue(string id, string name, string clueViewResource, Rect hitbox)
        {
            var clue = I(id, name, InteractableType.ClueObject);
            clue.clueViewResource = clueViewResource;
            clue.normalizedHitbox = hitbox;
            clue.showWorldImage = false;
            return clue;
        }

        private static RoomFaceDefinition WithBackground(RoomFaceDefinition face, string backgroundResource, ConditionDefinition conditions)
        {
            return WithBackground(face, new[]
            {
                new ConditionalBackgroundDefinition
                {
                    backgroundResource = backgroundResource,
                    conditions = conditions
                }
            });
        }

        private static RoomFaceDefinition WithBackground(RoomFaceDefinition face, ConditionalBackgroundDefinition[] conditionalBackgrounds)
        {
            face.conditionalBackgrounds = conditionalBackgrounds ?? System.Array.Empty<ConditionalBackgroundDefinition>();
            return face;
        }

        private static InteractableDefinition I(string id, string name, InteractableType type, string grantsItemId = "", string targetRoomId = "", string puzzleId = "", string eventId = "")
        {
            var interactable = new InteractableDefinition
            {
                interactableId = id,
                displayName = name,
                type = type,
                grantsItemId = grantsItemId,
                targetRoomId = targetRoomId,
                puzzleId = puzzleId,
                eventId = eventId,
                imageResource = "EscapeFromNightmares/Objects/" + id,
                normalizedHitbox = HitboxFor(id),
                soundId = SoundFor(type),
                oneShot = type == InteractableType.ItemPickup,
                showWorldImage = !IsChildRoomInteractable(id)
            };

            if (id == "child_desk_drawer")
            {
                interactable.closeUpClosedResource = "EscapeFromNightmares/CloseUps/child_desk_drawer_closed";
                interactable.closeUpOpenWithItemResource = "EscapeFromNightmares/CloseUps/child_desk_drawer_open_with_item";
                interactable.closeUpOpenEmptyResource = "EscapeFromNightmares/CloseUps/child_desk_drawer_open_empty";
                interactable.closeUpItemId = "torn_drawing_fragment";
                interactable.closeUpItemHitbox = new Rect(0.44f, 0.30f, 0.22f, 0.24f);
                interactable.closeUpOpenSoundId = "sfx_drawer_open";
                interactable.closeUpCloseSoundId = "sfx_drawer_close";
                interactable.soundId = "ui_click";
                interactable.oneShot = false;
                interactable.disableRoomHitboxWhenUsed = true;
            }
            else if (id == "child_bed_hide")
            {
                interactable.hideViewResource = "EscapeFromNightmares/HideViews/child_bed_under_view";
            }

            return interactable;
        }

        private static bool IsChildRoomInteractable(string id)
        {
            return id == "child_desk_drawer"
                || id == "child_room_door"
                || id == "child_bed_hide"
                || id == "child_window_silhouette";
        }

        private static Rect HitboxFor(string interactableId)
        {
            switch (interactableId)
            {
                case "child_desk_drawer":
                    return new Rect(0.12f, 0.2f, 0.28f, 0.38f);
                case "child_room_door":
                    return new Rect(0.63f, 0.12f, 0.31f, 0.76f);
                case "child_bed_hide":
                    return new Rect(0.34f, 0.18f, 0.46f, 0.36f);
                case "child_window_silhouette":
                    return new Rect(0.18f, 0.3f, 0.32f, 0.54f);
                default:
                    return new Rect(0.4f, 0.4f, 0.2f, 0.2f);
            }
        }

        private static string SoundFor(InteractableType type)
        {
            switch (type)
            {
                case InteractableType.Door:
                case InteractableType.LockedDoor:
                case InteractableType.EscapeDoor:
                    return "sfx_door";
                case InteractableType.ItemPickup:
                    return "sfx_item_pickup";
                case InteractableType.HideSpot:
                    return "sfx_hide";
                case InteractableType.PuzzleObject:
                case InteractableType.ClueObject:
                case InteractableType.EventTrigger:
                    return "ui_click";
                default:
                    return string.Empty;
            }
        }
    }
}
