using System.Collections.Generic;
using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Stage 1 ScriptableObject assets를 갱신하기 위한 editor-only seed data를 조립합니다.
    /// </summary>
    [System.Obsolete("Use Stage1DataAssetBuilder to author StageDefinition assets and StageRepository to load them at runtime.")]
    public static class RuntimeStageFactory
    {
        /// <summary>
        /// 현재 구현된 Stage 1의 방, 아이템, 퍼즐, 몬스터 그래프, 사운드 카탈로그를 생성합니다.
        /// </summary>
        public static StageDefinition CreateStage1()
        {
            var stage = ScriptableObject.CreateInstance<StageDefinition>();
            stage.stageId = "stage1";
            stage.displayName = "Stage 1 - Nightmare House";
            stage.startRoomId = "child_room";
            stage.clearFlag = "stage1_clear";

            // 데이터 생성 순서는 참조 방향에 맞춘다: 아이템/퍼즐/방을 만든 뒤 그래프와 사운드를 연결한다.
            stage.items = CreateItems();
            stage.puzzles = CreatePuzzles();
            stage.rooms = CreateRooms();
            stage.monsterNodeGraph = CreateMonsterGraph(stage.rooms);
            stage.soundCatalog = CreateSoundCatalog();
            return stage;
        }

        private static List<ItemDefinition> CreateItems()
        {
            // 아이템 ID는 Resources 경로와 퍼즐 required/reward ID에서 함께 사용되므로 문자열을 고정한다.
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
            // 퍼즐 성공 플래그는 조건부 배경, 후속 상호작용, 최종 추격 시작 조건에서 공유된다.
            var studySafe = Puzzle("study_safe", "Study Safe", PuzzleType.NumberLock, new[] {"3", "1", "4", "2"}, new[] {"fuse_holder"}, "study_safe_unlocked", null);
            studySafe.deferSolvedUntilRewardPickup = true;
            var laundryStorageBox = Puzzle("laundry_storage_box", "세탁실 보관함", PuzzleType.NumberLock, new[] {"0", "9", "1", "5"}, new[] {"fuse"}, "puzzle_laundry_box_clear", null, 10f);
            laundryStorageBox.oneShot = true;
            var breakerBox = Puzzle("breaker_box", "차단기함", PuzzleType.ItemUse, new[] {"fuse_holder", "fuse"}, new[] {"old_keychain"}, "electricity_restored", new[] {"fuse_holder", "fuse"});
            breakerBox.oneShot = true;
            var atticToySequence = Puzzle("attic_toy_sequence", "다락방 장난감 순서", PuzzleType.SilentSequence, new[] {"doll", "train", "block", "bell"}, new[] {"small_doll", "symbol_fragment"}, "attic_toy_box_opened", new[] {"torn_drawing_fragment"}, 15f);
            atticToySequence.oneShot = true;
            var basementAltar = Puzzle("basement_altar", "지하실 제단", PuzzleType.SymbolItemMatching, new[] {"broken_hand_mirror", "small_doll", "old_keychain", "old_necklace"}, System.Array.Empty<string>(), "front_door_key_spawned", new[] {"broken_hand_mirror", "small_doll", "old_keychain", "old_necklace"});
            basementAltar.oneShot = true;
            basementAltar.deferSolvedUntilRewardPickup = true;
            basementAltar.conditions.forbiddenFlagIds = new[] { "front_door_key_spawned" };
            return new List<PuzzleDefinition>
            {
                studySafe,
                laundryStorageBox,
                breakerBox,
                Puzzle("mirror_symbol_panel", "거울 방 문양 패널", PuzzleType.SymbolSequence, new[] {"heart", "child_hand", "cracked_circle", "keyhole"}, new[] {"broken_hand_mirror"}, "mirror_destroyed", null),
                Puzzle("master_bedroom_drawer", "안방 색상 서랍", PuzzleType.ColorSequence, new[] {"black", "white", "red", "gray"}, new[] {"old_necklace"}, "master_drawer_opened", null),
                atticToySequence,
                basementAltar,
                Puzzle("front_door_escape", "현관문 탈출", PuzzleType.ItemUse, new[] {"front_door_key"}, System.Array.Empty<string>(), "stage1_clear", new[] {"front_door_key"})
            };
        }

        private static List<RoomDefinition> CreateRooms()
        {
            // 방 목록은 Stage 1의 실제 이동 그래프를 정의하므로 connectedRoomIds와 문 타겟이 서로 맞아야 한다.
            return new List<RoomDefinition>
            {
                ChildRoom(),
                SecondFloorHallway(),
                Study(),
                MirrorRoom(),
                MasterBedroom(),
                DressingRoom(),
                SecondFloorBathroom(),
                Stairwell2f(),
                AtticMain(),
                AtticToyStorage(),
                Stairwell1f(),
                FirstFloorHallway(),
                Room("family_photo_room", "가족사진 방", "1F", new[] {"first_floor_hallway"}, 1, I("hidden_photo_drawer", "숨겨진 사진 서랍", InteractableType.ItemPickup, grantsItemId: "study_safe_clue"), Door("family_photo_exit", "복도로 나가기", "first_floor_hallway")),
                DiningRoom(),
                Kitchen(),
                LaundryRoom(),
                Room("living_room", "거실", "1F", new[] {"first_floor_hallway", "entrance"}, 1, Combine(Hide("living_curtain_hide", "커튼 뒤"), Doors("first_floor_hallway", "entrance"))),
                Entrance(),
                BasementEntry(),
                BasementMain(),
                AltarRoom()
            };
        }

        private static MonsterNodeGraph CreateMonsterGraph(List<RoomDefinition> rooms)
        {
            var graph = ScriptableObject.CreateInstance<MonsterNodeGraph>();
            // 현재는 방 연결 정보를 그대로 몬스터 노드 그래프로 변환해 방 이동 구조와 동기화한다.
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
            // soundId는 InteractableDefinition과 PuzzleDefinition에서 액션으로 참조하는 안정적인 키다.
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

        private static RoomDefinition Stairwell2f()
        {
            var hallwayExit = TransparentDoor("stairwell_2f_to_hallway", "2F Hallway", "second_floor_hallway", new Rect(0.32f, 0.12f, 0.34f, 0.72f));
            var atticExit = TransparentDoor("stairwell_2f_to_attic", "Attic", "attic_main", new Rect(0.42f, 0.08f, 0.28f, 0.74f));
            var firstFloorExit = TransparentDoor("stairwell_2f_to_1f", "1F Stairwell", "stairwell_1f", new Rect(0.30f, 0.18f, 0.38f, 0.66f));

            var room = Room(
                "stairwell_2f",
                "2F Stairwell",
                "2F",
                new[] { "second_floor_hallway", "stairwell_1f", "attic_main" },
                0,
                hallwayExit,
                atticExit,
                firstFloorExit);

            room.faces = new[]
            {
                Face("stairwell_2f", RoomFaceDirection.North, "Hallway Landing", new[] { hallwayExit }),
                Face("stairwell_2f", RoomFaceDirection.East, "Attic Stairs", new[] { atticExit }),
                Face("stairwell_2f", RoomFaceDirection.South, "First Floor Stairs", new[] { firstFloorExit }),
                Face("stairwell_2f", RoomFaceDirection.West, "Landing Wall", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition AtticMain()
        {
            var albumClue = Clue("attic_family_album_photo", "Family Album Photo", "EscapeFromNightmares/CloseUps/attic_family_album_photo", new Rect(0.30f, 0.18f, 0.42f, 0.42f));
            var toyStorageExit = TransparentDoor("attic_main_to_toy_storage", "Toy Storage", "attic_toy_storage", new Rect(0.58f, 0.13f, 0.28f, 0.70f));
            var stairwellExit = TransparentDoor("attic_main_to_stairwell", "2F Stairwell", "stairwell_2f", new Rect(0.36f, 0.12f, 0.32f, 0.72f));

            var room = Room(
                "attic_main",
                "Attic",
                "Attic",
                new[] { "stairwell_2f", "attic_toy_storage" },
                0,
                albumClue,
                toyStorageExit,
                stairwellExit);

            room.faces = new[]
            {
                Face("attic_main", RoomFaceDirection.North, "Family Album", new[] { albumClue }),
                Face("attic_main", RoomFaceDirection.East, "Toy Storage Door", new[] { toyStorageExit }),
                Face("attic_main", RoomFaceDirection.South, "Stairwell Door", new[] { stairwellExit }),
                Face("attic_main", RoomFaceDirection.West, "Attic Storage", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition AtticToyStorage()
        {
            var toyBox = PuzzleObject("attic_toy_box", "Toy Box", "attic_toy_sequence");
            toyBox.normalizedHitbox = new Rect(0.34f, 0.28f, 0.34f, 0.38f);
            toyBox.showWorldImage = false;

            var atticExit = TransparentDoor("attic_toy_storage_exit", "Back to Attic", "attic_main", new Rect(0.58f, 0.12f, 0.28f, 0.74f));

            var room = Room(
                "attic_toy_storage",
                "Attic Toy Storage",
                "Attic",
                new[] { "attic_main" },
                0,
                toyBox,
                atticExit);

            room.faces = new[]
            {
                Face("attic_toy_storage", RoomFaceDirection.North, "Toy Box", new[] { toyBox }),
                Face("attic_toy_storage", RoomFaceDirection.East, "Attic Door", new[] { atticExit }),
                Face("attic_toy_storage", RoomFaceDirection.South, "Toy Shelves", System.Array.Empty<InteractableDefinition>()),
                Face("attic_toy_storage", RoomFaceDirection.West, "Storage Corner", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition Stairwell1f()
        {
            var secondFloorExit = TransparentDoor("stairwell_1f_to_2f", "2F Stairwell", "stairwell_2f", new Rect(0.34f, 0.14f, 0.30f, 0.70f));
            var hallwayExit = TransparentDoor("stairwell_1f_to_hallway", "1F Hallway", "first_floor_hallway", new Rect(0.52f, 0.13f, 0.24f, 0.72f));

            var room = Room(
                "stairwell_1f",
                "1F Stairwell",
                "1F",
                new[] { "stairwell_2f", "first_floor_hallway" },
                0,
                secondFloorExit,
                hallwayExit);

            room.faces = new[]
            {
                Face("stairwell_1f", RoomFaceDirection.North, "Second Floor Stairs", new[] { secondFloorExit }),
                Face("stairwell_1f", RoomFaceDirection.East, "First Floor Hallway", new[] { hallwayExit }),
                Face("stairwell_1f", RoomFaceDirection.South, "Lower Stair Wall", System.Array.Empty<InteractableDefinition>()),
                Face("stairwell_1f", RoomFaceDirection.West, "Stairwell Wall", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition FirstFloorHallway()
        {
            var stairwellExit = TransparentDoor("first_floor_hallway_to_stairwell", "1F Stairwell", "stairwell_1f", new Rect(0.06f, 0.12f, 0.20f, 0.74f));
            var diningExit = TransparentDoor("first_floor_hallway_to_dining", "Dining Room", "dining_room", new Rect(0.28f, 0.13f, 0.18f, 0.72f));
            var kitchenExit = TransparentDoor("first_floor_hallway_to_kitchen", "Kitchen", "kitchen", new Rect(0.51f, 0.13f, 0.18f, 0.72f));
            var laundryExit = TransparentDoor("first_floor_hallway_to_laundry", "Laundry Room", "laundry_room", new Rect(0.74f, 0.14f, 0.18f, 0.70f));
            var entranceExit = TransparentDoor("first_floor_hallway_to_entrance", "Entrance", "entrance", new Rect(0.08f, 0.12f, 0.24f, 0.74f));
            var livingExit = TransparentDoor("first_floor_hallway_to_living", "Living Room", "living_room", new Rect(0.39f, 0.13f, 0.22f, 0.72f));
            var familyPhotoExit = TransparentDoor("first_floor_hallway_to_family_photo", "Family Photo Room", "family_photo_room", new Rect(0.70f, 0.13f, 0.20f, 0.72f));

            var room = Room(
                "first_floor_hallway",
                "1F Hallway",
                "1F",
                new[] { "stairwell_1f", "entrance", "living_room", "dining_room", "kitchen", "laundry_room", "family_photo_room" },
                0,
                stairwellExit,
                diningExit,
                kitchenExit,
                laundryExit,
                entranceExit,
                livingExit,
                familyPhotoExit);

            room.faces = new[]
            {
                Face("first_floor_hallway", RoomFaceDirection.North, "Utility Hall", new[] { stairwellExit, diningExit, kitchenExit, laundryExit }),
                WithBackground(
                    Face("first_floor_hallway", RoomFaceDirection.South, "Front Hall", new[] { entranceExit, livingExit, familyPhotoExit }),
                    new[]
                    {
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/first_floor_hallway_south_chase",
                            conditions = new ConditionDefinition { requiredFlagIds = new[] { "final_chase_started" } }
                        }
                    })
            };

            return room;
        }

        private static RoomDefinition Entrance()
        {
            var frontDoor = PuzzleObject("front_door", "Front Door", "front_door_escape");
            frontDoor.normalizedHitbox = new Rect(0.34f, 0.08f, 0.32f, 0.78f);
            frontDoor.showWorldImage = false;

            var hallwayExit = TransparentDoor("entrance_to_hallway", "1F Hallway", "first_floor_hallway", new Rect(0.58f, 0.12f, 0.28f, 0.74f));
            var livingExit = TransparentDoor("entrance_to_living", "Living Room", "living_room", new Rect(0.18f, 0.14f, 0.30f, 0.72f));

            var room = Room(
                "entrance",
                "Entrance",
                "1F",
                new[] { "first_floor_hallway", "living_room" },
                0,
                frontDoor,
                hallwayExit,
                livingExit);

            room.faces = new[]
            {
                WithBackground(
                    Face("entrance", RoomFaceDirection.North, "Front Door", new[] { frontDoor }),
                    new[]
                    {
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/entrance_north_chase",
                            conditions = new ConditionDefinition { requiredFlagIds = new[] { "final_chase_started" } }
                        }
                    }),
                Face("entrance", RoomFaceDirection.East, "Hallway Door", new[] { hallwayExit }),
                Face("entrance", RoomFaceDirection.South, "Living Room Door", new[] { livingExit }),
                Face("entrance", RoomFaceDirection.West, "Entry Wall", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition DiningRoom()
        {
            var seatOrderClue = Clue("dining_seat_order_clue", "Dining Seat Order", "EscapeFromNightmares/CloseUps/dining_seat_order_clue", new Rect(0.24f, 0.20f, 0.52f, 0.52f));
            var exit = TransparentDoor("dining_exit", "1F Hallway", "first_floor_hallway", new Rect(0.62f, 0.11f, 0.24f, 0.74f));

            var room = Room(
                "dining_room",
                "Dining Room",
                "1F",
                new[] { "first_floor_hallway" },
                0,
                seatOrderClue,
                exit);

            room.faces = new[]
            {
                Face("dining_room", RoomFaceDirection.North, "Seat Order", new[] { seatOrderClue }),
                Face("dining_room", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("dining_room", RoomFaceDirection.South, "Dining Cabinets", System.Array.Empty<InteractableDefinition>()),
                Face("dining_room", RoomFaceDirection.West, "Dining Window", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition Kitchen()
        {
            var clockClue = Clue("kitchen_clock_clue", "Kitchen Clock", "EscapeFromNightmares/CloseUps/kitchen_clock_clue", new Rect(0.38f, 0.16f, 0.24f, 0.30f));
            var exit = TransparentDoor("kitchen_exit", "1F Hallway", "first_floor_hallway", new Rect(0.62f, 0.12f, 0.24f, 0.74f));
            var hide = Hide("kitchen_hide", "Kitchen Sink");
            hide.normalizedHitbox = new Rect(0.30f, 0.42f, 0.40f, 0.30f);
            hide.hideViewResource = "EscapeFromNightmares/HideViews/kitchen_sink_hide_view";
            hide.showWorldImage = false;

            var room = Room(
                "kitchen",
                "Kitchen",
                "1F",
                new[] { "first_floor_hallway" },
                1,
                clockClue,
                exit,
                hide);

            room.faces = new[]
            {
                Face("kitchen", RoomFaceDirection.North, "Clock Clue", new[] { clockClue }),
                Face("kitchen", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("kitchen", RoomFaceDirection.South, "Sink Hide Spot", new[] { hide }),
                Face("kitchen", RoomFaceDirection.West, "Kitchen Stove", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition LaundryRoom()
        {
            var storageBox = PuzzleObject("laundry_box", "Laundry Storage Box", "laundry_storage_box");
            storageBox.normalizedHitbox = new Rect(0.33f, 0.26f, 0.34f, 0.36f);
            storageBox.showWorldImage = false;

            var breakerBox = PuzzleObject("breaker_box_obj", "Breaker Box", "breaker_box");
            breakerBox.normalizedHitbox = new Rect(0.14f, 0.20f, 0.26f, 0.44f);
            breakerBox.showWorldImage = false;

            var basementEntry = TransparentDoor("basement_entry_door", "Basement Entry", "basement_entry", new Rect(0.55f, 0.12f, 0.30f, 0.74f));
            basementEntry.conditions.requiredFlagIds = new[] { "electricity_restored" };

            var hide = Hide("laundry_hide", "Laundry Machine");
            hide.normalizedHitbox = new Rect(0.28f, 0.30f, 0.46f, 0.42f);
            hide.hideViewResource = "EscapeFromNightmares/HideViews/laundry_machine_hide_view";
            hide.showWorldImage = false;

            var exit = TransparentDoor("laundry_exit", "1F Hallway", "first_floor_hallway", new Rect(0.60f, 0.12f, 0.26f, 0.74f));

            var room = Room(
                "laundry_room",
                "Laundry Room",
                "1F",
                new[] { "first_floor_hallway", "basement_entry" },
                1,
                storageBox,
                breakerBox,
                basementEntry,
                hide,
                exit);

            room.faces = new[]
            {
                Face("laundry_room", RoomFaceDirection.North, "Storage Box", new[] { storageBox }),
                Face("laundry_room", RoomFaceDirection.East, "Breaker Box And Basement Entry", new[] { breakerBox, basementEntry }),
                Face("laundry_room", RoomFaceDirection.South, "Laundry Hide Spot", new[] { hide }),
                Face("laundry_room", RoomFaceDirection.West, "Hallway Door", new[] { exit })
            };

            return room;
        }

        private static RoomDefinition BasementEntry()
        {
            var laundryExit = TransparentDoor("basement_entry_to_laundry", "Laundry Room", "laundry_room", new Rect(0.28f, 0.10f, 0.36f, 0.76f));
            var basementMainExit = TransparentDoor("basement_entry_to_main", "Basement Main", "basement_main", new Rect(0.55f, 0.12f, 0.30f, 0.74f));

            var room = Room(
                "basement_entry",
                "Basement Entry",
                "Basement",
                new[] { "laundry_room", "basement_main" },
                0,
                laundryExit,
                basementMainExit);

            room.faces = new[]
            {
                Face("basement_entry", RoomFaceDirection.North, "Laundry Stairs", new[] { laundryExit }),
                Face("basement_entry", RoomFaceDirection.East, "Main Basement Door", new[] { basementMainExit }),
                Face("basement_entry", RoomFaceDirection.South, "Damp Wall", System.Array.Empty<InteractableDefinition>()),
                Face("basement_entry", RoomFaceDirection.West, "Pipe Wall", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition BasementMain()
        {
            var wallSymbols = Clue("basement_wall_symbols", "Basement Wall Symbols", "EscapeFromNightmares/CloseUps/basement_wall_symbols", new Rect(0.26f, 0.16f, 0.48f, 0.42f));
            var altarExit = TransparentDoor("basement_main_to_altar", "Altar Room", "altar_room", new Rect(0.56f, 0.10f, 0.30f, 0.76f));
            var entryExit = TransparentDoor("basement_main_to_entry", "Basement Entry", "basement_entry", new Rect(0.30f, 0.12f, 0.34f, 0.74f));
            var hide = Hide("basement_main_hide", "Basement Cabinet");
            hide.normalizedHitbox = new Rect(0.26f, 0.24f, 0.42f, 0.52f);
            hide.hideViewResource = "EscapeFromNightmares/HideViews/basement_main_hide_view";
            hide.showWorldImage = false;

            var room = Room(
                "basement_main",
                "Basement",
                "Basement",
                new[] { "basement_entry", "altar_room" },
                1,
                wallSymbols,
                altarExit,
                entryExit,
                hide);

            room.faces = new[]
            {
                Face("basement_main", RoomFaceDirection.North, "Wall Symbols", new[] { wallSymbols }),
                Face("basement_main", RoomFaceDirection.East, "Altar Door", new[] { altarExit }),
                Face("basement_main", RoomFaceDirection.South, "Entry Stairs", new[] { entryExit }),
                Face("basement_main", RoomFaceDirection.West, "Cabinet Hide Spot", new[] { hide })
            };

            return room;
        }

        private static RoomDefinition AltarRoom()
        {
            var altar = PuzzleObject("altar", "Altar", "basement_altar");
            altar.normalizedHitbox = new Rect(0.30f, 0.22f, 0.40f, 0.44f);
            altar.showWorldImage = false;

            var keyPickup = I("front_door_key_on_altar", "Front Door Key", InteractableType.ItemPickup, grantsItemId: "front_door_key");
            keyPickup.conditions.requiredFlagIds = new[] { "front_door_key_spawned" };
            keyPickup.normalizedHitbox = new Rect(0.46f, 0.36f, 0.12f, 0.16f);
            keyPickup.showWorldImage = false;
            keyPickup.disableRoomHitboxWhenUsed = true;
            keyPickup.solvesPuzzleId = "basement_altar";
            keyPickup.setFlagIds = new[] { "event_front_door_key_appears" };

            var exit = TransparentDoor("altar_room_to_basement_main", "Basement Main", "basement_main", new Rect(0.30f, 0.12f, 0.34f, 0.74f));

            var room = Room(
                "altar_room",
                "Altar Room",
                "Basement",
                new[] { "basement_main" },
                0,
                altar,
                keyPickup,
                exit);

            room.faces = new[]
            {
                WithBackground(
                    Face("altar_room", RoomFaceDirection.North, "Altar", new[] { altar, keyPickup }),
                    new[]
                    {
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/altar_room_north_key_taken",
                            conditions = new ConditionDefinition { requiredItemIds = new[] { "front_door_key" } }
                        },
                        new ConditionalBackgroundDefinition
                        {
                            backgroundResource = "EscapeFromNightmares/Rooms/altar_room_north_key_spawned",
                            conditions = new ConditionDefinition { requiredFlagIds = new[] { "front_door_key_spawned" } }
                        }
                    }),
                Face("altar_room", RoomFaceDirection.East, "Ritual Wall", System.Array.Empty<InteractableDefinition>()),
                Face("altar_room", RoomFaceDirection.South, "Basement Door", new[] { exit }),
                Face("altar_room", RoomFaceDirection.West, "Candle Wall", System.Array.Empty<InteractableDefinition>())
            };

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

        private static RoomDefinition MirrorRoom()
        {
            var panel = PuzzleObject("mirror_panel_obj", "Mirror Symbol Panel", "mirror_symbol_panel");
            panel.normalizedHitbox = new Rect(0.32f, 0.20f, 0.36f, 0.22f);
            panel.showWorldImage = false;

            var exit = Door("mirror_exit", "Back to Hallway", "second_floor_hallway");
            exit.normalizedHitbox = new Rect(0.55f, 0.12f, 0.24f, 0.72f);
            exit.showWorldImage = false;

            var room = Room(
                "mirror_room",
                "Mirror Room",
                "2F",
                new[] { "second_floor_hallway" },
                0,
                panel,
                exit);

            room.faces = new[]
            {
                Face("mirror_room", RoomFaceDirection.North, "Symbol Mirror", new[] { panel }),
                Face("mirror_room", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("mirror_room", RoomFaceDirection.South, "Distorted Reflection", System.Array.Empty<InteractableDefinition>()),
                Face("mirror_room", RoomFaceDirection.West, "Broken Mirrors", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition SecondFloorBathroom()
        {
            var mirrorRuleClue = Clue("bathroom_mirror_rule_clue", "Mirror Reversal Rule", "EscapeFromNightmares/CloseUps/bathroom_mirror_rule_clue", new Rect(0.30f, 0.24f, 0.40f, 0.48f));
            var exit = Door("bathroom_exit", "Back to Hallway", "second_floor_hallway");
            exit.normalizedHitbox = new Rect(0.66f, 0.12f, 0.26f, 0.74f);
            exit.showWorldImage = false;

            var room = Room(
                "second_floor_bathroom",
                "2F Bathroom",
                "2F",
                new[] { "second_floor_hallway" },
                0,
                mirrorRuleClue,
                exit);

            room.faces = new[]
            {
                Face("second_floor_bathroom", RoomFaceDirection.North, "Mirror Rule", new[] { mirrorRuleClue }),
                Face("second_floor_bathroom", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("second_floor_bathroom", RoomFaceDirection.South, "Bath Wall", System.Array.Empty<InteractableDefinition>()),
                Face("second_floor_bathroom", RoomFaceDirection.West, "Storage Wall", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition MasterBedroom()
        {
            var drawer = PuzzleObject("master_drawer_obj", "Master Bedroom Drawer", "master_bedroom_drawer");
            drawer.normalizedHitbox = new Rect(0.44f, 0.20f, 0.32f, 0.48f);
            drawer.showWorldImage = false;

            var exit = Door("master_exit", "Back to Hallway", "second_floor_hallway");
            exit.normalizedHitbox = new Rect(0.66f, 0.12f, 0.26f, 0.74f);
            exit.showWorldImage = false;

            var room = Room(
                "master_bedroom",
                "Master Bedroom",
                "2F",
                new[] { "second_floor_hallway" },
                0,
                drawer,
                exit);

            room.faces = new[]
            {
                Face("master_bedroom", RoomFaceDirection.North, "Color Drawer", new[] { drawer }),
                Face("master_bedroom", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("master_bedroom", RoomFaceDirection.South, "Bed And Wardrobe", System.Array.Empty<InteractableDefinition>()),
                Face("master_bedroom", RoomFaceDirection.West, "Moonlit Keepsakes", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomDefinition DressingRoom()
        {
            var colorSequenceClue = Clue("dressing_color_sequence_clue", "Color Sequence Clue", "EscapeFromNightmares/CloseUps/dressing_color_sequence_clue", new Rect(0.24f, 0.18f, 0.52f, 0.56f));
            var exit = Door("dressing_exit", "Back to Hallway", "second_floor_hallway");
            exit.normalizedHitbox = new Rect(0.62f, 0.12f, 0.28f, 0.74f);
            exit.showWorldImage = false;

            var room = Room(
                "dressing_room",
                "Dressing Room",
                "2F",
                new[] { "second_floor_hallway" },
                0,
                colorSequenceClue,
                exit);

            room.faces = new[]
            {
                Face("dressing_room", RoomFaceDirection.North, "Color Sequence", new[] { colorSequenceClue }),
                Face("dressing_room", RoomFaceDirection.East, "Hallway Door", new[] { exit }),
                Face("dressing_room", RoomFaceDirection.South, "Mirror Wall", System.Array.Empty<InteractableDefinition>()),
                Face("dressing_room", RoomFaceDirection.West, "Storage Wall", System.Array.Empty<InteractableDefinition>())
            };

            return room;
        }

        private static RoomFaceDefinition[] CreateFaces(string roomId, InteractableDefinition[] interactables)
        {
            // 별도 face 구성이 없는 방은 상호작용 타입을 기준으로 4방향에 기본 배치한다.
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
            // 첫 방은 튜토리얼 역할을 하므로 각 핵심 상호작용을 고정된 방향에 배치한다.
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

        private static InteractableDefinition TransparentDoor(string id, string name, string targetRoomId, Rect hitbox)
        {
            var door = Door(id, name, targetRoomId);
            door.normalizedHitbox = hitbox;
            door.showWorldImage = false;
            return door;
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
