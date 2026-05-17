local puzzleInputs = {
    study_safe = {
        id = "study_safe",
        room = "study",
        objectId = "study_safe",
        type = "number_lock",
        digits = 4,
        answer = { 3, 1, 4, 2 },
        clue_items = { "study_safe_clue" },
        success_events = { "event_open_study_safe" },
        failure_events = { "event_puzzle_error_soft" },
        rewards = { items = { "fuse_holder" } },
        danger_delta_on_failure = 4
    },
    laundry_storage_box = {
        id = "laundry_storage_box",
        room = "laundry_room",
        objectId = "laundry_storage_box",
        type = "number_lock",
        digits = 4,
        answer = { 0, 9, 1, 5 },
        clue_objects = { "dining_seat_clue", "kitchen_clock_clue" },
        success_events = { "event_open_laundry_storage_box" },
        failure_events = { "event_puzzle_error_soft" },
        rewards = { items = { "fuse" } },
        danger_delta_on_failure = 6
    },
    mirror_symbol_panel = {
        id = "mirror_symbol_panel",
        room = "mirror_room",
        objectId = "mirror_symbol_panel",
        type = "symbol_sequence",
        symbols = { "heart", "child_hand", "cracked_circle", "keyhole" },
        answer = { "heart", "child_hand", "cracked_circle", "keyhole" },
        clue_objects = { "second_floor_bathroom_mirror_clue", "dressing_color_clue", "study_number_order_clue" },
        success_events = { "event_break_mirror" },
        failure_events = { "event_puzzle_error_soft" },
        rewards = { items = { "broken_hand_mirror" } },
        danger_delta_on_failure = 4
    },
    attic_toy_sequence = {
        id = "attic_toy_sequence",
        room = "attic_toy_storage",
        objectId = "attic_toy_sequence",
        type = "silent_sequence",
        choices = { "doll", "train", "block", "bell" },
        answer = { "doll", "train", "block", "bell" },
        clue_items = { "torn_drawing_fragment" },
        clue_objects = { "attic_family_album_photo" },
        success_events = { "event_open_attic_toy_box" },
        failure_events = { "event_toy_wrong_sound" },
        rewards = { items = { "small_doll", "symbol_fragment" } },
        danger_delta_on_failure = 10
    },
    master_bedroom_drawer = {
        id = "master_bedroom_drawer",
        room = "master_bedroom",
        objectId = "master_bedroom_drawer",
        type = "color_sequence",
        choices = { "black", "white", "red", "gray" },
        answer = { "black", "white", "red", "gray" },
        clue_objects = { "dressing_color_clue" },
        success_events = { "event_open_master_bedroom_drawer" },
        failure_events = { "event_puzzle_error_soft" },
        rewards = { items = { "old_necklace" } },
        danger_delta_on_failure = 4
    },
    basement_altar = {
        id = "basement_altar",
        room = "altar_room",
        objectId = "basement_altar",
        type = "symbol_item_matching",
        required_items = { "broken_hand_mirror", "small_doll", "old_keychain", "old_necklace" },
        answer = {
            cracked_circle = "broken_hand_mirror",
            child_hand = "small_doll",
            keyhole = "old_keychain",
            heart = "old_necklace"
        },
        clue_items = { "symbol_fragment" },
        clue_objects = { "basement_wall_symbols" },
        success_events = { "event_front_door_key_appears" },
        failure_events = { "event_puzzle_error_soft" },
        rewards = { objects = { "front_door_key_on_altar" } },
        danger_delta_on_failure = 8
    },
    front_door_escape = {
        id = "front_door_escape",
        room = "entrance",
        objectId = "front_door_escape",
        type = "item_use",
        required_items = { "front_door_key" },
        success_events = { "event_stage1_clear" },
        failure_events = { "event_front_door_locked" },
        clear_flag = "stage1_clear"
    }
}

return puzzleInputs
