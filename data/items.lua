local items = {
    torn_drawing_fragment = {
        id = "torn_drawing_fragment",
        name = "Torn Drawing Fragment",
        icon = "assets/images/items/torn_drawing_fragment.png",
        type = "clue_item",
        acquired_from = "child_desk_drawer",
        used_in = { "attic_toy_sequence" },
        flags_on_acquire = { "has_torn_drawing_fragment" }
    },
    study_safe_clue = {
        id = "study_safe_clue",
        name = "Study Safe Clue",
        icon = "assets/images/items/study_safe_clue.png",
        type = "clue_item",
        acquired_from = "hidden_photo_drawer",
        used_in = { "study_safe" },
        flags_on_acquire = { "has_study_safe_clue" }
    },
    fuse_holder = {
        id = "fuse_holder",
        name = "Fuse Holder",
        icon = "assets/images/items/fuse_holder.png",
        type = "mechanical_part",
        acquired_from = "study_safe",
        used_in = { "breaker_box" },
        flags_on_acquire = { "has_fuse_holder" }
    },
    fuse = {
        id = "fuse",
        name = "Fuse",
        icon = "assets/images/items/fuse.png",
        type = "mechanical_part",
        acquired_from = "laundry_storage_box",
        used_in = { "breaker_box" },
        flags_on_acquire = { "has_fuse" }
    },
    broken_hand_mirror = {
        id = "broken_hand_mirror",
        name = "Broken Hand Mirror",
        icon = "assets/images/items/broken_hand_mirror.png",
        type = "altar_object",
        acquired_from = "mirror_symbol_panel",
        used_in = { "basement_altar" },
        altar_symbol = "cracked_circle",
        flags_on_acquire = { "has_broken_hand_mirror" }
    },
    small_doll = {
        id = "small_doll",
        name = "Small Doll",
        icon = "assets/images/items/small_doll.png",
        type = "altar_object",
        acquired_from = "attic_toy_box",
        used_in = { "basement_altar" },
        altar_symbol = "child_hand",
        flags_on_acquire = { "has_small_doll" }
    },
    old_keychain = {
        id = "old_keychain",
        name = "Old Keychain",
        icon = "assets/images/items/old_keychain.png",
        type = "altar_object",
        acquired_from = "breaker_box",
        used_in = { "basement_altar" },
        altar_symbol = "keyhole",
        flags_on_acquire = { "has_old_keychain" }
    },
    old_necklace = {
        id = "old_necklace",
        name = "Old Necklace",
        icon = "assets/images/items/old_necklace.png",
        type = "altar_object",
        acquired_from = "master_bedroom_drawer",
        used_in = { "basement_altar" },
        altar_symbol = "heart",
        flags_on_acquire = { "has_old_necklace" }
    },
    symbol_fragment = {
        id = "symbol_fragment",
        name = "Symbol Fragment",
        icon = "assets/images/items/symbol_fragment.png",
        type = "clue_item",
        acquired_from = "attic_toy_box",
        used_in = { "basement_altar" },
        flags_on_acquire = { "has_symbol_fragment" }
    },
    front_door_key = {
        id = "front_door_key",
        name = "Front Door Key",
        icon = "assets/images/items/front_door_key.png",
        type = "key_item",
        acquired_from = "front_door_key_on_altar",
        used_in = { "front_door_escape" },
        flags_on_acquire = { "has_front_door_key", "final_chase_ready" }
    }
}

return items
