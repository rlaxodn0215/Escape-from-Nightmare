local puzzleChains = {
    child_room_intro = {
        id = "child_room_intro",
        rooms = { "child_room", "second_floor_hallway" },
        required_items = {},
        required_flags = {},
        main_steps = {
            { action = "start_room", room = "child_room" },
            { action = "pickup_item", objectId = "child_desk_drawer", itemId = "torn_drawing_fragment" },
            { action = "event", eventId = "event_window_silhouette" },
            { action = "move_to", room = "second_floor_hallway" }
        },
        completion_condition = { flag = "has_torn_drawing_fragment" },
        next_chains = { "family_photo_study_safe", "attic_toy_chain" }
    },
    family_photo_study_safe = {
        id = "family_photo_study_safe",
        rooms = { "child_room", "family_photo_room", "study" },
        required_items = { "torn_drawing_fragment" },
        required_flags = {},
        main_steps = {
            { action = "inspect", objectId = "child_toy_clue" },
            { action = "inspect", objectId = "family_photo_puzzle" },
            { action = "pickup_item", objectId = "hidden_photo_drawer", itemId = "study_safe_clue" },
            { action = "solve_puzzle", puzzleId = "study_safe" },
            { action = "pickup_item", objectId = "study_safe", itemId = "fuse_holder" }
        },
        completion_condition = { flag = "has_fuse_holder" },
        next_chains = { "laundry_electric_restore" }
    },
    kitchen_dining_laundry_number = {
        id = "kitchen_dining_laundry_number",
        rooms = { "dining_room", "kitchen", "laundry_room" },
        required_items = {},
        required_flags = {},
        main_steps = {
            { action = "inspect", objectId = "dining_seat_clue" },
            { action = "inspect", objectId = "kitchen_clock_clue" },
            { action = "solve_puzzle", puzzleId = "laundry_storage_box" },
            { action = "pickup_item", objectId = "laundry_storage_box", itemId = "fuse" },
            { action = "event", eventId = "event_kitchen_first_appearance" }
        },
        completion_condition = { flag = "has_fuse" },
        next_chains = { "laundry_electric_restore" }
    },
    laundry_electric_restore = {
        id = "laundry_electric_restore",
        rooms = { "laundry_room", "garage", "first_floor_storage", "basement_entry" },
        required_items = { "fuse_holder", "fuse" },
        required_flags = {},
        main_steps = {
            { action = "use_items", objectId = "breaker_box", itemIds = { "fuse_holder", "fuse" } },
            { action = "event", eventId = "event_restore_electricity" },
            { action = "event", eventId = "event_basement_door_unlocked" },
            { action = "pickup_item", objectId = "breaker_box", itemId = "old_keychain" },
            { action = "event", eventId = "event_electric_noise_attracts_monster" }
        },
        completion_condition = { flag = "basement_electronic_lock_released" },
        next_chains = { "basement_altar_chain" }
    },
    mirror_symbol_chain = {
        id = "mirror_symbol_chain",
        rooms = { "second_floor_bathroom", "dressing_room", "study", "mirror_room" },
        required_items = {},
        required_flags = {},
        main_steps = {
            { action = "inspect", objectId = "second_floor_bathroom_mirror_clue" },
            { action = "inspect", objectId = "dressing_color_clue" },
            { action = "inspect", objectId = "study_number_order_clue" },
            { action = "solve_puzzle", puzzleId = "mirror_symbol_panel" },
            { action = "pickup_item", objectId = "mirror_symbol_panel", itemId = "broken_hand_mirror" }
        },
        completion_condition = { flag = "has_broken_hand_mirror" },
        next_chains = { "basement_altar_chain" }
    },
    master_bedroom_necklace_chain = {
        id = "master_bedroom_necklace_chain",
        rooms = { "dressing_room", "master_bedroom" },
        required_items = {},
        required_flags = {},
        main_steps = {
            { action = "inspect", objectId = "dressing_color_clue" },
            { action = "solve_puzzle", puzzleId = "master_bedroom_drawer" },
            { action = "pickup_item", objectId = "master_bedroom_drawer", itemId = "old_necklace" }
        },
        completion_condition = { flag = "has_old_necklace" },
        next_chains = { "basement_altar_chain" }
    },
    attic_toy_chain = {
        id = "attic_toy_chain",
        rooms = { "attic_main", "attic_album_storage", "attic_toy_storage" },
        required_items = { "torn_drawing_fragment" },
        required_flags = {},
        main_steps = {
            { action = "inspect", objectId = "attic_family_album_photo" },
            { action = "solve_puzzle", puzzleId = "attic_toy_sequence" },
            { action = "pickup_item", objectId = "attic_toy_box", itemId = "small_doll" },
            { action = "pickup_item", objectId = "attic_toy_box_symbol", itemId = "symbol_fragment" }
        },
        completion_condition = { flags = { "has_small_doll", "has_symbol_fragment" } },
        next_chains = { "basement_altar_chain" }
    },
    basement_altar_chain = {
        id = "basement_altar_chain",
        rooms = { "basement_main", "altar_room" },
        required_items = { "broken_hand_mirror", "small_doll", "old_keychain", "old_necklace", "symbol_fragment" },
        required_flags = { "basement_electronic_lock_released" },
        main_steps = {
            { action = "inspect", objectId = "basement_wall_symbols" },
            { action = "solve_puzzle", puzzleId = "basement_altar" },
            { action = "event", eventId = "event_front_door_key_appears" },
            { action = "pickup_item", objectId = "front_door_key_on_altar", itemId = "front_door_key" },
            { action = "event", eventId = "event_final_chase_trigger" }
        },
        completion_condition = { flag = "final_chase_ready" },
        next_chains = { "final_chase_escape" }
    },
    final_chase_escape = {
        id = "final_chase_escape",
        rooms = { "altar_room", "basement_main", "stairwell_1f", "living_room", "entrance" },
        required_items = { "front_door_key" },
        required_flags = { "final_chase_ready" },
        main_steps = {
            { action = "move_to", room = "basement_main" },
            { action = "move_to", room = "stairwell_1f" },
            { action = "move_to", room = "living_room" },
            { action = "move_to", room = "entrance" },
            { action = "solve_puzzle", puzzleId = "front_door_escape" }
        },
        completion_condition = { clear_flag = "stage1_clear" },
        next_chains = {}
    }
}

return puzzleChains
