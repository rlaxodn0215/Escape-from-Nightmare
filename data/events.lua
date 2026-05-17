local events = {
    event_puzzle_error_soft = {
        id = "event_puzzle_error_soft",
        type = "puzzle_failure",
        sound = "sfx_puzzle_error",
        danger_delta = 4
    },
    event_window_silhouette = {
        id = "event_window_silhouette",
        type = "visual_stinger",
        room = "child_room",
        image = "assets/images/monster/monster_silhouette_window.png",
        sound = "event_window_silhouette",
        flags_set = { "window_silhouette_seen" }
    },
    event_open_hidden_photo_drawer = {
        id = "event_open_hidden_photo_drawer",
        type = "container_open",
        objectId = "hidden_photo_drawer",
        sound = "sfx_drawer_open",
        flags_set = { "hidden_photo_drawer_open" }
    },
    event_open_study_safe = {
        id = "event_open_study_safe",
        type = "container_open",
        objectId = "study_safe",
        sound = "sfx_safe_open",
        flags_set = { "study_safe_open" }
    },
    event_open_laundry_storage_box = {
        id = "event_open_laundry_storage_box",
        type = "container_open",
        objectId = "laundry_storage_box",
        sound = "sfx_box_open",
        flags_set = { "laundry_storage_box_open" }
    },
    event_restore_electricity = {
        id = "event_restore_electricity",
        type = "world_state",
        objectId = "breaker_box",
        sound = "sfx_electricity_restore",
        flags_set = { "electricity_restored" }
    },
    event_basement_door_unlocked = {
        id = "event_basement_door_unlocked",
        type = "unlock",
        objectIds = { "garage_to_basement", "storage_1f_to_basement", "basement_main_to_altar" },
        sound = "sfx_electronic_lock_release",
        flags_set = { "basement_electronic_lock_released" }
    },
    event_break_mirror = {
        id = "event_break_mirror",
        type = "object_change",
        objectId = "mirror_symbol_panel",
        sound = "sfx_mirror_crack",
        flags_set = { "mirror_broken" }
    },
    event_open_master_bedroom_drawer = {
        id = "event_open_master_bedroom_drawer",
        type = "container_open",
        objectId = "master_bedroom_drawer",
        sound = "sfx_drawer_open",
        flags_set = { "master_bedroom_drawer_open" }
    },
    event_toy_wrong_sound = {
        id = "event_toy_wrong_sound",
        type = "puzzle_failure",
        objectId = "attic_toy_sequence",
        sound = "sfx_toy_wrong_sound",
        danger_delta = 10
    },
    event_open_attic_toy_box = {
        id = "event_open_attic_toy_box",
        type = "container_open",
        objectId = "attic_toy_box",
        sound = "sfx_toy_box_open",
        flags_set = { "attic_toy_box_open" }
    },
    event_front_door_key_appears = {
        id = "event_front_door_key_appears",
        type = "spawn_object",
        objectId = "front_door_key_on_altar",
        sound = "sfx_key_appear",
        flags_set = { "front_door_key_visible" }
    },
    event_kitchen_first_appearance = {
        id = "event_kitchen_first_appearance",
        type = "monster_appearance",
        room = "kitchen",
        image = "assets/images/monster/monster_doorway_kitchen.png",
        sound = "event_kitchen_first_appearance",
        flags_set = { "kitchen_first_appearance_seen" }
    },
    event_electric_noise_attracts_monster = {
        id = "event_electric_noise_attracts_monster",
        type = "monster_pressure",
        sound = "monster_searching",
        danger_delta = 15,
        flags_set = { "monster_attracted_by_electricity" }
    },
    event_final_chase_trigger = {
        id = "event_final_chase_trigger",
        type = "final_chase",
        sound = "event_final_chase_trigger",
        bgm = "bgm_final_chase",
        flags_set = { "final_chase_active" }
    },
    event_player_captured = {
        id = "event_player_captured",
        type = "game_over",
        image = "assets/images/monster/monster_gameover_shadow.png",
        sound = "monster_capture",
        flags_set = { "player_captured" }
    },
    event_front_door_locked = {
        id = "event_front_door_locked",
        type = "locked_feedback",
        objectId = "front_door_escape",
        sound = "sfx_door_locked"
    },
    event_stage1_clear = {
        id = "event_stage1_clear",
        type = "clear",
        objectId = "front_door_escape",
        sound = "sfx_front_door_unlock",
        clear_record = "stage1_clear"
    }
}

return events
