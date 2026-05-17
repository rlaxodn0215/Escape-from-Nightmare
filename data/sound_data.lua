local soundData = {
    bgm = {
        bgm_title = { path = "assets/sounds/bgm/bgm_title.ogg", loop = true },
        bgm_stage1_ambient = { path = "assets/sounds/bgm/bgm_stage1_ambient.ogg", loop = true },
        bgm_chase = { path = "assets/sounds/bgm/bgm_chase.ogg", loop = true },
        bgm_final_chase = { path = "assets/sounds/bgm/bgm_final_chase.ogg", loop = true },
        bgm_ending_hint = { path = "assets/sounds/bgm/bgm_ending_hint.ogg", loop = false }
    },
    ambience = {
        amb_house_low_hum = { path = "assets/sounds/ambience/amb_house_low_hum.ogg", loop = true },
        amb_1f_hallway = { path = "assets/sounds/ambience/amb_1f_hallway.ogg", loop = true },
        amb_2f_hallway = { path = "assets/sounds/ambience/amb_2f_hallway.ogg", loop = true },
        amb_child_room = { path = "assets/sounds/ambience/amb_child_room.ogg", loop = true },
        amb_kitchen = { path = "assets/sounds/ambience/amb_kitchen.ogg", loop = true },
        amb_laundry_room = { path = "assets/sounds/ambience/amb_laundry_room.ogg", loop = true },
        amb_basement = { path = "assets/sounds/ambience/amb_basement.ogg", loop = true },
        amb_attic = { path = "assets/sounds/ambience/amb_attic.ogg", loop = true },
        amb_mirror_room = { path = "assets/sounds/ambience/amb_mirror_room.ogg", loop = true },
        amb_altar_room = { path = "assets/sounds/ambience/amb_altar_room.ogg", loop = true }
    },
    sfx = {
        sfx_click = { path = "assets/sounds/sfx/sfx_click.ogg" },
        sfx_item_pickup = { path = "assets/sounds/sfx/sfx_item_pickup.ogg" },
        sfx_inventory_open = { path = "assets/sounds/sfx/sfx_inventory_open.ogg" },
        sfx_inventory_close = { path = "assets/sounds/sfx/sfx_inventory_close.ogg" },
        sfx_item_use = { path = "assets/sounds/sfx/sfx_item_use.ogg" },
        sfx_item_combine = { path = "assets/sounds/sfx/sfx_item_combine.ogg" },
        sfx_door_open = { path = "assets/sounds/sfx/sfx_door_open.ogg" },
        sfx_door_locked = { path = "assets/sounds/sfx/sfx_door_locked.ogg" },
        sfx_drawer_open = { path = "assets/sounds/sfx/sfx_drawer_open.ogg" },
        sfx_box_open = { path = "assets/sounds/sfx/sfx_box_open.ogg" },
        sfx_puzzle_success = { path = "assets/sounds/sfx/sfx_puzzle_success.ogg" },
        sfx_puzzle_error = { path = "assets/sounds/sfx/sfx_puzzle_error.ogg" },
        sfx_object_shake = { path = "assets/sounds/sfx/sfx_object_shake.ogg" },
        sfx_number_lock_turn = { path = "assets/sounds/sfx/sfx_number_lock_turn.ogg" },
        sfx_safe_open = { path = "assets/sounds/sfx/sfx_safe_open.ogg" },
        sfx_symbol_click = { path = "assets/sounds/sfx/sfx_symbol_click.ogg" },
        sfx_fuse_insert = { path = "assets/sounds/sfx/sfx_fuse_insert.ogg" },
        sfx_breaker_switch = { path = "assets/sounds/sfx/sfx_breaker_switch.ogg" },
        sfx_electricity_restore = { path = "assets/sounds/sfx/sfx_electricity_restore.ogg" },
        sfx_electronic_lock_release = { path = "assets/sounds/sfx/sfx_electronic_lock_release.ogg" },
        sfx_mirror_crack = { path = "assets/sounds/sfx/sfx_mirror_crack.ogg" },
        sfx_altar_activate = { path = "assets/sounds/sfx/sfx_altar_activate.ogg" },
        sfx_key_appear = { path = "assets/sounds/sfx/sfx_key_appear.ogg" },
        sfx_front_door_unlock = { path = "assets/sounds/sfx/sfx_front_door_unlock.ogg" },
        sfx_toy_wrong_sound = { path = "assets/sounds/sfx/sfx_toy_wrong_sound.ogg" },
        sfx_toy_box_open = { path = "assets/sounds/sfx/sfx_toy_box_open.ogg" }
    },
    monster = {
        monster_footstep_far = { path = "assets/sounds/monster/monster_footstep_far.ogg" },
        monster_footstep_mid = { path = "assets/sounds/monster/monster_footstep_mid.ogg" },
        monster_footstep_near = { path = "assets/sounds/monster/monster_footstep_near.ogg" },
        monster_breath_far = { path = "assets/sounds/monster/monster_breath_far.ogg" },
        monster_breath_near = { path = "assets/sounds/monster/monster_breath_near.ogg" },
        monster_low_growl = { path = "assets/sounds/monster/monster_low_growl.ogg" },
        monster_searching = { path = "assets/sounds/monster/monster_searching.ogg" },
        monster_detected = { path = "assets/sounds/monster/monster_detected.ogg" },
        monster_chase_start = { path = "assets/sounds/monster/monster_chase_start.ogg" },
        monster_capture = { path = "assets/sounds/monster/monster_capture.ogg" }
    },
    ui = {
        ui_button_hover = { path = "assets/sounds/ui/ui_button_hover.ogg" },
        ui_button_click = { path = "assets/sounds/ui/ui_button_click.ogg" },
        ui_menu_open = { path = "assets/sounds/ui/ui_menu_open.ogg" },
        ui_menu_close = { path = "assets/sounds/ui/ui_menu_close.ogg" },
        ui_map_open = { path = "assets/sounds/ui/ui_map_open.ogg" },
        ui_map_close = { path = "assets/sounds/ui/ui_map_close.ogg" }
    },
    events = {
        event_gameover_bgm_cut = { path = "assets/sounds/events/event_gameover_bgm_cut.ogg" },
        event_screen_rewind = { path = "assets/sounds/events/event_screen_rewind.ogg" },
        event_window_silhouette = { path = "assets/sounds/events/event_window_silhouette.ogg" },
        event_kitchen_first_appearance = { path = "assets/sounds/events/event_kitchen_first_appearance.ogg" },
        event_final_chase_trigger = { path = "assets/sounds/events/event_final_chase_trigger.ogg" }
    },
    missing_fallback = { silent = true }
}

return soundData
