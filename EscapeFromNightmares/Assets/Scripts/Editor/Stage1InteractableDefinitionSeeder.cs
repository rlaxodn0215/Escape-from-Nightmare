using System.Collections.Generic;
using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmares.Editor
{
    public static class Stage1InteractableDefinitionSeeder
    {
        private const string MenuPath = "Escape From Nightmares/Seed Stage1 Interactable Definitions";
        private const string OutputRoot = "Assets/ScriptableObjects/Stage1/Interactables";
        private const string RoomRoot = "Assets/ScriptableObjects/Stage1/Rooms";
        private const string ItemRoot = "Assets/ScriptableObjects/Stage1/Items";
        private const string PuzzleRoot = "Assets/ScriptableObjects/Stage1/Puzzles";

        private static readonly InteractableSeed[] Seeds =
        {
            Door("child_room_door_to_second_floor_hallway", "child_room", "second_floor_hallway", new Rect(1088f, 220f, 150f, 310f)),
            Door("second_floor_hallway_to_child_room", "second_floor_hallway", "child_room", new Rect(72f, 250f, 140f, 260f)),
            Door("second_floor_hallway_to_master_bedroom", "second_floor_hallway", "master_bedroom", new Rect(264f, 230f, 130f, 270f)),
            Door("second_floor_hallway_to_study", "second_floor_hallway", "study", new Rect(456f, 226f, 130f, 276f)),
            Door("second_floor_hallway_to_mirror_room", "second_floor_hallway", "mirror_room", new Rect(690f, 224f, 140f, 278f)),
            Door("second_floor_hallway_to_stairwell_2f", "second_floor_hallway", "stairwell_2f", new Rect(1038f, 180f, 168f, 330f)),
            Door("stairwell_2f_to_second_floor_hallway", "stairwell_2f", "second_floor_hallway", new Rect(70f, 240f, 170f, 320f)),
            Door("stairwell_2f_to_stairwell_1f", "stairwell_2f", "stairwell_1f", new Rect(880f, 210f, 240f, 360f)),
            Door("stairwell_1f_to_stairwell_2f", "stairwell_1f", "stairwell_2f", new Rect(122f, 204f, 210f, 360f)),
            Door("stairwell_1f_to_first_floor_hallway", "stairwell_1f", "first_floor_hallway", new Rect(902f, 230f, 176f, 300f)),
            Door("first_floor_hallway_to_stairwell_1f", "first_floor_hallway", "stairwell_1f", new Rect(54f, 232f, 144f, 310f)),
            Door("first_floor_hallway_to_living_room", "first_floor_hallway", "living_room", new Rect(246f, 240f, 142f, 280f)),
            Door("first_floor_hallway_to_kitchen", "first_floor_hallway", "kitchen", new Rect(430f, 230f, 146f, 292f)),
            Door("first_floor_hallway_to_dining_room", "first_floor_hallway", "dining_room", new Rect(622f, 232f, 148f, 288f)),
            Door("first_floor_hallway_to_laundry_room", "first_floor_hallway", "laundry_room", new Rect(814f, 236f, 148f, 286f)),
            Door("first_floor_hallway_to_entrance", "first_floor_hallway", "entrance", new Rect(1004f, 220f, 116f, 312f)),
            Door("entrance_to_first_floor_hallway", "entrance", "first_floor_hallway", new Rect(66f, 230f, 152f, 310f)),
            Door("living_room_to_first_floor_hallway", "living_room", "first_floor_hallway", new Rect(1088f, 238f, 112f, 292f)),
            Door("kitchen_to_first_floor_hallway", "kitchen", "first_floor_hallway", new Rect(82f, 232f, 146f, 300f)),
            Door("dining_room_to_first_floor_hallway", "dining_room", "first_floor_hallway", new Rect(78f, 236f, 150f, 296f)),
            Door("laundry_room_to_first_floor_hallway", "laundry_room", "first_floor_hallway", new Rect(1040f, 236f, 150f, 298f)),
            Door("first_floor_hallway_to_garage", "first_floor_hallway", "garage", new Rect(1160f, 254f, 90f, 246f)),
            Door("garage_to_first_floor_hallway", "garage", "first_floor_hallway", new Rect(72f, 244f, 150f, 288f)),
            Door("garage_to_basement_entry", "garage", "basement_entry", new Rect(1004f, 216f, 178f, 326f)),
            Door("basement_entry_to_garage", "basement_entry", "garage", new Rect(86f, 228f, 160f, 300f)),
            Door("basement_entry_to_basement_main", "basement_entry", "basement_main", new Rect(940f, 218f, 178f, 330f)),
            Door("basement_main_to_basement_entry", "basement_main", "basement_entry", new Rect(80f, 240f, 160f, 292f)),
            Door("basement_main_to_basement_storage", "basement_main", "basement_storage", new Rect(472f, 238f, 150f, 288f)),
            Door("basement_main_to_altar_room", "basement_main", "altar_room", new Rect(1012f, 200f, 180f, 348f)),
            Door("altar_room_to_basement_main", "altar_room", "basement_main", new Rect(58f, 238f, 154f, 296f)),

            Pickup("child_desk_drawer", "child_room", "torn_drawing_fragment", "event_window_silhouette", new Rect(164f, 410f, 170f, 96f)),
            Clue("child_toy_arrangement_clue", "child_room", new Rect(496f, 470f, 210f, 92f)),
            Clue("study_number_clue", "study", new Rect(358f, 286f, 172f, 116f)),
            Puzzle("study_safe", "study", "study_safe", "event_open_study_safe", new Rect(880f, 350f, 168f, 142f)),
            Pickup("hidden_photo_drawer", "family_photo_room", "study_safe_clue", "event_open_hidden_photo_drawer", new Rect(730f, 410f, 178f, 108f)),
            Clue("dining_table_clue", "dining_room", new Rect(446f, 430f, 260f, 112f)),
            Puzzle("laundry_storage_box", "laundry_room", "laundry_storage_box", "event_open_laundry_storage_box", new Rect(766f, 408f, 190f, 128f)),
            PuzzleWithRequirements("breaker_box", "laundry_room", "event_restore_electricity", new Rect(262f, 226f, 148f, 166f), "fuse_holder", "fuse"),
            Clue("kitchen_clock_clue", "kitchen", new Rect(546f, 142f, 118f, 110f)),
            Clue("second_floor_bathroom_mirror_clue", "second_floor_bathroom", new Rect(520f, 198f, 184f, 166f)),
            Clue("dressing_room_color_clue", "dressing_room", new Rect(780f, 292f, 174f, 150f)),
            Puzzle("mirror_symbol_panel", "mirror_room", "mirror_symbol_panel", "event_break_mirror", new Rect(514f, 250f, 236f, 210f)),
            Puzzle("master_bedroom_drawer", "master_bedroom", "master_bedroom_drawer", "event_open_master_bedroom_drawer", new Rect(222f, 424f, 182f, 110f)),
            Puzzle("attic_toy_box", "attic_toy_storage", "attic_toy_sequence", "event_open_attic_toy_box", new Rect(594f, 424f, 230f, 132f)),
            Clue("basement_wall_symbols", "basement_main", new Rect(454f, 152f, 250f, 78f)),
            Puzzle("basement_altar", "altar_room", "basement_altar", "event_front_door_key_appears", new Rect(512f, 290f, 260f, 188f)),
            PickupWithFlagRequirement("front_door_key_on_altar", "altar_room", "front_door_key", "event_final_chase_trigger", new Rect(608f, 494f, 72f, 54f), "front_door_key_appeared"),
            EscapeDoor("front_door", "entrance", "front_door_escape", "event_stage1_clear", new Rect(542f, 158f, 204f, 402f), "front_door_key"),

            HideSpot("living_room_curtain_hide", "living_room", new Rect(924f, 190f, 128f, 390f)),
            HideSpot("kitchen_sink_hide", "kitchen", new Rect(794f, 424f, 198f, 120f)),
            HideSpot("laundry_room_cabinet_hide", "laundry_room", new Rect(92f, 318f, 174f, 228f)),
            HideSpot("guest_room_bed_hide", "guest_room", new Rect(434f, 434f, 348f, 116f)),
            HideSpot("first_floor_storage_shelf_hide", "first_floor_storage", new Rect(782f, 218f, 190f, 330f))
        };

        [MenuItem(MenuPath)]
        public static void Seed()
        {
            Directory.CreateDirectory(OutputRoot);

            Dictionary<string, RoomDefinition> rooms = LoadAssetsByName<RoomDefinition>(RoomRoot);
            Dictionary<string, ItemDefinition> items = LoadAssetsByName<ItemDefinition>(ItemRoot);
            Dictionary<string, PuzzleDefinition> puzzles = LoadAssetsByName<PuzzleDefinition>(PuzzleRoot);
            Dictionary<string, List<InteractableDefinition>> interactablesByRoom = new Dictionary<string, List<InteractableDefinition>>();
            int createdOrUpdated = 0;

            foreach (InteractableSeed seed in Seeds)
            {
                if (!rooms.ContainsKey(seed.RoomId))
                {
                    Debug.LogWarning($"Skipping interactable {seed.InteractableId}; missing room {seed.RoomId}.");
                    continue;
                }

                string assetPath = $"{OutputRoot}/{seed.InteractableId}.asset";
                InteractableDefinition definition = AssetDatabase.LoadAssetAtPath<InteractableDefinition>(assetPath);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<InteractableDefinition>();
                    AssetDatabase.CreateAsset(definition, assetPath);
                }

                SerializedObject serialized = new SerializedObject(definition);
                serialized.FindProperty("roomId").stringValue = seed.RoomId;
                serialized.FindProperty("interactableId").stringValue = seed.InteractableId;
                serialized.FindProperty("interactableType").enumValueIndex = (int)seed.Type;
                serialized.FindProperty("hitArea").rectValue = seed.HitArea;
                serialized.FindProperty("eventId").stringValue = seed.EventId;
                serialized.FindProperty("targetRoomId").stringValue = seed.TargetRoomId;
                serialized.FindProperty("itemReward").objectReferenceValue = LoadOptional(items, seed.ItemRewardId, "item", seed.InteractableId);
                serialized.FindProperty("puzzleDefinition").objectReferenceValue = LoadOptional(puzzles, seed.PuzzleId, "puzzle", seed.InteractableId);
                SetRequirements(serialized.FindProperty("requirements"), seed.RequiredItemIds, seed.RequiredFlagIds);
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(definition);

                if (!interactablesByRoom.TryGetValue(seed.RoomId, out List<InteractableDefinition> roomInteractables))
                {
                    roomInteractables = new List<InteractableDefinition>();
                    interactablesByRoom.Add(seed.RoomId, roomInteractables);
                }

                roomInteractables.Add(definition);
                createdOrUpdated++;
            }

            foreach (KeyValuePair<string, List<InteractableDefinition>> entry in interactablesByRoom)
            {
                RoomDefinition room = rooms[entry.Key];
                SerializedObject serializedRoom = new SerializedObject(room);
                SerializedProperty interactableDefinitions = serializedRoom.FindProperty("interactableDefinitions");
                interactableDefinitions.arraySize = entry.Value.Count;

                for (int i = 0; i < entry.Value.Count; i++)
                {
                    interactableDefinitions.GetArrayElementAtIndex(i).objectReferenceValue = entry.Value[i];
                }

                serializedRoom.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(room);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Seeded {createdOrUpdated} Stage 1 interactable definitions across {interactablesByRoom.Count} rooms.");
        }

        private static Dictionary<string, T> LoadAssetsByName<T>(string folder) where T : Object
        {
            Dictionary<string, T> results = new Dictionary<string, T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    results[Path.GetFileNameWithoutExtension(path)] = asset;
                }
            }

            return results;
        }

        private static Object LoadOptional<T>(Dictionary<string, T> assets, string id, string assetType, string interactableId) where T : Object
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            if (assets.TryGetValue(id, out T asset))
            {
                return asset;
            }

            Debug.LogWarning($"Interactable {interactableId} references missing {assetType} '{id}'.");
            return null;
        }

        private static void SetRequirements(SerializedProperty property, string[] requiredItemIds, string[] requiredFlagIds)
        {
            property.arraySize = requiredItemIds.Length + requiredFlagIds.Length;

            for (int i = 0; i < requiredItemIds.Length; i++)
            {
                SerializedProperty requirement = property.GetArrayElementAtIndex(i);
                requirement.FindPropertyRelative("requiredItemId").stringValue = requiredItemIds[i];
                requirement.FindPropertyRelative("requiredFlag").stringValue = string.Empty;
                requirement.FindPropertyRelative("consumeItem").boolValue = false;
            }

            for (int i = 0; i < requiredFlagIds.Length; i++)
            {
                SerializedProperty requirement = property.GetArrayElementAtIndex(requiredItemIds.Length + i);
                requirement.FindPropertyRelative("requiredItemId").stringValue = string.Empty;
                requirement.FindPropertyRelative("requiredFlag").stringValue = requiredFlagIds[i];
                requirement.FindPropertyRelative("consumeItem").boolValue = false;
            }
        }

        private static InteractableSeed Door(string id, string roomId, string targetRoomId, Rect hitArea)
        {
            return new InteractableSeed(id, roomId, InteractableType.Door, hitArea, string.Empty, targetRoomId, string.Empty, string.Empty, new string[0], new string[0]);
        }

        private static InteractableSeed Pickup(string id, string roomId, string itemRewardId, string eventId, Rect hitArea)
        {
            return new InteractableSeed(id, roomId, InteractableType.ItemPickup, hitArea, eventId, string.Empty, itemRewardId, string.Empty, new string[0], new string[0]);
        }

        private static InteractableSeed PickupWithFlagRequirement(string id, string roomId, string itemRewardId, string eventId, Rect hitArea, params string[] requiredFlagIds)
        {
            return new InteractableSeed(id, roomId, InteractableType.ItemPickup, hitArea, eventId, string.Empty, itemRewardId, string.Empty, new string[0], requiredFlagIds);
        }

        private static InteractableSeed Clue(string id, string roomId, Rect hitArea)
        {
            return new InteractableSeed(id, roomId, InteractableType.ClueObject, hitArea, string.Empty, string.Empty, string.Empty, string.Empty, new string[0], new string[0]);
        }

        private static InteractableSeed Puzzle(string id, string roomId, string puzzleId, string eventId, Rect hitArea)
        {
            return new InteractableSeed(id, roomId, InteractableType.PuzzleObject, hitArea, eventId, string.Empty, string.Empty, puzzleId, new string[0], new string[0]);
        }

        private static InteractableSeed PuzzleWithRequirements(string id, string roomId, string eventId, Rect hitArea, params string[] requiredItemIds)
        {
            return new InteractableSeed(id, roomId, InteractableType.PuzzleObject, hitArea, eventId, string.Empty, string.Empty, string.Empty, requiredItemIds, new string[0]);
        }

        private static InteractableSeed EscapeDoor(string id, string roomId, string puzzleId, string eventId, Rect hitArea, params string[] requiredItemIds)
        {
            return new InteractableSeed(id, roomId, InteractableType.EscapeDoor, hitArea, eventId, string.Empty, string.Empty, puzzleId, requiredItemIds, new string[0]);
        }

        private static InteractableSeed HideSpot(string id, string roomId, Rect hitArea)
        {
            return new InteractableSeed(id, roomId, InteractableType.HideSpot, hitArea, string.Empty, string.Empty, string.Empty, string.Empty, new string[0], new string[0]);
        }

        private readonly struct InteractableSeed
        {
            public InteractableSeed(
                string interactableId,
                string roomId,
                InteractableType type,
                Rect hitArea,
                string eventId,
                string targetRoomId,
                string itemRewardId,
                string puzzleId,
                string[] requiredItemIds,
                string[] requiredFlagIds)
            {
                InteractableId = interactableId;
                RoomId = roomId;
                Type = type;
                HitArea = hitArea;
                EventId = eventId;
                TargetRoomId = targetRoomId;
                ItemRewardId = itemRewardId;
                PuzzleId = puzzleId;
                RequiredItemIds = requiredItemIds;
                RequiredFlagIds = requiredFlagIds;
            }

            public string InteractableId { get; }
            public string RoomId { get; }
            public InteractableType Type { get; }
            public Rect HitArea { get; }
            public string EventId { get; }
            public string TargetRoomId { get; }
            public string ItemRewardId { get; }
            public string PuzzleId { get; }
            public string[] RequiredItemIds { get; }
            public string[] RequiredFlagIds { get; }
        }
    }
}
