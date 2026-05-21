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
            return new List<PuzzleDefinition>
            {
                Puzzle("study_safe", "서재 금고", PuzzleType.NumberLock, new[] {"3", "1", "4", "2"}, new[] {"fuse_holder"}, "puzzle_study_safe_clear", null),
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
                Room("child_room", "아이 방", "2F", new[] {"second_floor_hallway"}, 1, I("child_desk_drawer", "책상 서랍", InteractableType.ItemPickup, grantsItemId: "torn_drawing_fragment"), Door("child_room_door", "복도로 나가기", "second_floor_hallway"), Hide("child_bed_hide", "침대 밑")),
                Room("second_floor_hallway", "2층 복도", "2F", new[] {"child_room", "study", "mirror_room", "master_bedroom", "dressing_room", "second_floor_bathroom", "stairwell_2f"}, 0, Doors("child_room", "study", "mirror_room", "master_bedroom", "dressing_room", "second_floor_bathroom", "stairwell_2f")),
                Room("study", "서재", "2F", new[] {"second_floor_hallway"}, 1, PuzzleObject("study_safe_obj", "서재 금고", "study_safe"), Door("study_exit", "복도로 나가기", "second_floor_hallway")),
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
            catalog.entries.Add(new SoundEntry { soundId = "bgm_stage1_ambient", resourcePath = "Audio/BGM/bgm_stage1_ambient", loop = true });
            catalog.entries.Add(new SoundEntry { soundId = "bgm_final_chase", resourcePath = "Audio/BGM/bgm_final_chase", loop = true });
            return catalog;
        }

        private static ItemDefinition Item(string id, string name, ItemType type, string altarSymbol)
        {
            var item = ScriptableObject.CreateInstance<ItemDefinition>();
            item.itemId = id;
            item.displayName = name;
            item.itemType = type;
            item.altarSymbol = altarSymbol;
            item.iconResource = "Sprites/Items/item_" + id;
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
            return puzzle;
        }

        private static RoomDefinition Room(string id, string name, string floor, string[] connected, int hideSpotCount, params InteractableDefinition[] interactables)
        {
            var room = ScriptableObject.CreateInstance<RoomDefinition>();
            room.roomId = id;
            room.displayName = name;
            room.floorName = floor;
            room.backgroundResource = "Sprites/Rooms/room_" + id;
            room.connectedRoomIds = connected;
            room.hideSpotCount = hideSpotCount;
            room.interactables = interactables;
            room.allowMonster = id != "child_room";
            return room;
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

        private static InteractableDefinition I(string id, string name, InteractableType type, string grantsItemId = "", string targetRoomId = "", string puzzleId = "", string eventId = "")
        {
            return new InteractableDefinition
            {
                interactableId = id,
                displayName = name,
                type = type,
                grantsItemId = grantsItemId,
                targetRoomId = targetRoomId,
                puzzleId = puzzleId,
                eventId = eventId
            };
        }
    }
}
