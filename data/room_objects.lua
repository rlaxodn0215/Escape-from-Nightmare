local roomObjects = {
    child_room = {
        { id = "child_desk_drawer", type = "item_pickup", itemId = "torn_drawing_fragment", hitbox = { x = 792, y = 388, w = 144, h = 72 } },
        { id = "child_toy_clue", type = "clue_object", hitbox = { x = 388, y = 426, w = 172, h = 92 }, placeholder = true },
        { id = "child_room_door", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 548, y = 168, w = 184, h = 360 } },
        { id = "child_room_edge_out", type = "edge_navigation", targetRoom = "second_floor_hallway", hitbox = { x = 1216, y = 96, w = 64, h = 528 } }
    },
    second_floor_hallway = {
        { id = "hallway_to_child_room", type = "door", targetRoom = "child_room", hitbox = { x = 88, y = 176, w = 150, h = 340 } },
        { id = "hallway_to_master_bedroom", type = "door", targetRoom = "master_bedroom", hitbox = { x = 300, y = 168, w = 140, h = 348 } },
        { id = "hallway_to_guest_room", type = "door", targetRoom = "guest_room", hitbox = { x = 498, y = 166, w = 136, h = 342 } },
        { id = "hallway_to_bathroom", type = "door", targetRoom = "second_floor_bathroom", hitbox = { x = 696, y = 166, w = 136, h = 342 } },
        { id = "hallway_to_study", type = "door", targetRoom = "study", hitbox = { x = 894, y = 168, w = 140, h = 348 } },
        { id = "hallway_to_mirror_room", type = "door", targetRoom = "mirror_room", hitbox = { x = 1094, y = 176, w = 130, h = 330 } },
        { id = "hallway_edge_stairwell", type = "edge_navigation", targetRoom = "stairwell_2f", hitbox = { x = 1216, y = 96, w = 64, h = 528 } }
    },
    master_bedroom = {
        { id = "master_to_hallway", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "master_to_dressing", type = "door", targetRoom = "dressing_room", hitbox = { x = 1044, y = 176, w = 150, h = 340 } },
        { id = "master_bedroom_drawer", type = "puzzle_object", useTarget = "master_bedroom_drawer", hitbox = { x = 776, y = 426, w = 162, h = 76 } },
        { id = "master_edge_back", type = "edge_navigation", targetRoom = "second_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    dressing_room = {
        { id = "dressing_to_master", type = "door", targetRoom = "master_bedroom", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "dressing_color_clue", type = "clue_object", hitbox = { x = 520, y = 250, w = 240, h = 96 }, placeholder = true },
        { id = "dressing_edge_back", type = "edge_navigation", targetRoom = "master_bedroom", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    guest_room = {
        { id = "guest_to_hallway", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "guest_edge_back", type = "edge_navigation", targetRoom = "second_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    second_floor_bathroom = {
        { id = "bathroom_2f_to_hallway", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "second_floor_bathroom_mirror_clue", type = "clue_object", hitbox = { x = 574, y = 158, w = 146, h = 230 }, placeholder = true },
        { id = "bathroom_2f_edge_back", type = "edge_navigation", targetRoom = "second_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    study = {
        { id = "study_to_hallway", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "study_number_order_clue", type = "clue_object", hitbox = { x = 468, y = 298, w = 196, h = 110 }, placeholder = true },
        { id = "study_safe", type = "puzzle_object", useTarget = "study_safe", hitbox = { x = 894, y = 292, w = 132, h = 148 } },
        { id = "study_edge_back", type = "edge_navigation", targetRoom = "second_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    mirror_room = {
        { id = "mirror_to_hallway", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "mirror_symbol_panel", type = "puzzle_object", useTarget = "mirror_symbol_panel", hitbox = { x = 536, y = 170, w = 208, h = 306 } },
        { id = "mirror_edge_back", type = "edge_navigation", targetRoom = "second_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    stairwell_2f = {
        { id = "stairwell_2f_to_hallway", type = "door", targetRoom = "second_floor_hallway", hitbox = { x = 84, y = 176, w = 150, h = 340 } },
        { id = "stairwell_2f_down", type = "edge_navigation", targetRoom = "stairwell_1f", hitbox = { x = 0, y = 96, w = 64, h = 528 } },
        { id = "stairwell_2f_up_attic", type = "locked_door", targetRoom = "attic_main", locked = true, hitbox = { x = 1070, y = 150, w = 148, h = 300 } }
    },
    stairwell_1f = {
        { id = "stairwell_1f_to_hallway", type = "door", targetRoom = "first_floor_hallway", hitbox = { x = 84, y = 176, w = 150, h = 340 } },
        { id = "stairwell_1f_up", type = "edge_navigation", targetRoom = "stairwell_2f", hitbox = { x = 1216, y = 96, w = 64, h = 528 } }
    },
    first_floor_hallway = {
        { id = "hallway_1f_to_entrance", type = "door", targetRoom = "entrance", hitbox = { x = 72, y = 176, w = 138, h = 340 } },
        { id = "hallway_1f_to_living", type = "door", targetRoom = "living_room", hitbox = { x = 268, y = 168, w = 136, h = 342 } },
        { id = "hallway_1f_to_dining", type = "door", targetRoom = "dining_room", hitbox = { x = 462, y = 168, w = 136, h = 342 } },
        { id = "hallway_1f_to_bathroom", type = "door", targetRoom = "first_floor_bathroom", hitbox = { x = 656, y = 168, w = 136, h = 342 } },
        { id = "hallway_1f_to_storage", type = "door", targetRoom = "first_floor_storage", hitbox = { x = 850, y = 168, w = 136, h = 342 } },
        { id = "hallway_1f_edge_stairwell", type = "edge_navigation", targetRoom = "stairwell_1f", hitbox = { x = 1216, y = 96, w = 64, h = 528 } }
    },
    entrance = {
        { id = "entrance_to_hallway", type = "door", targetRoom = "first_floor_hallway", hitbox = { x = 88, y = 176, w = 150, h = 340 } },
        { id = "front_door_escape", type = "escape_door", targetRoom = "ending", locked = true, useTarget = "front_door_escape", hitbox = { x = 542, y = 128, w = 196, h = 420 } }
    },
    living_room = {
        { id = "living_to_hallway", type = "door", targetRoom = "first_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "living_to_photo_room", type = "door", targetRoom = "family_photo_room", hitbox = { x = 1044, y = 176, w = 150, h = 340 } },
        { id = "living_edge_back", type = "edge_navigation", targetRoom = "first_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    dining_room = {
        { id = "dining_to_hallway", type = "door", targetRoom = "first_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "dining_seat_clue", type = "clue_object", hitbox = { x = 472, y = 394, w = 336, h = 122 }, placeholder = true },
        { id = "dining_to_kitchen", type = "door", targetRoom = "kitchen", hitbox = { x = 1044, y = 176, w = 150, h = 340 } },
        { id = "dining_edge_kitchen", type = "edge_navigation", targetRoom = "kitchen", hitbox = { x = 1216, y = 96, w = 64, h = 528 } }
    },
    kitchen = {
        { id = "kitchen_to_dining", type = "door", targetRoom = "dining_room", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "kitchen_clock_clue", type = "clue_object", hitbox = { x = 596, y = 122, w = 88, h = 88 }, placeholder = true },
        { id = "kitchen_to_laundry", type = "door", targetRoom = "laundry_room", hitbox = { x = 548, y = 168, w = 184, h = 360 } },
        { id = "kitchen_to_garage", type = "locked_door", targetRoom = "garage", locked = true, hitbox = { x = 1044, y = 176, w = 150, h = 340 } }
    },
    laundry_room = {
        { id = "laundry_to_kitchen", type = "door", targetRoom = "kitchen", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "laundry_storage_box", type = "puzzle_object", useTarget = "laundry_storage_box", hitbox = { x = 842, y = 408, w = 164, h = 82 } },
        { id = "breaker_box", type = "item_pickup", itemId = "old_keychain", useTarget = "breaker_box", hitbox = { x = 1028, y = 180, w = 108, h = 160 } },
        { id = "laundry_edge_back", type = "edge_navigation", targetRoom = "kitchen", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    first_floor_bathroom = {
        { id = "bathroom_1f_to_hallway", type = "door", targetRoom = "first_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "bathroom_1f_edge_back", type = "edge_navigation", targetRoom = "first_floor_hallway", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    first_floor_storage = {
        { id = "storage_1f_to_hallway", type = "door", targetRoom = "first_floor_hallway", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "storage_1f_to_basement", type = "locked_door", targetRoom = "basement_entry", locked = true, hitbox = { x = 1044, y = 176, w = 150, h = 340 } }
    },
    garage = {
        { id = "garage_to_kitchen", type = "door", targetRoom = "kitchen", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "garage_to_basement", type = "locked_door", targetRoom = "basement_entry", locked = true, hitbox = { x = 1044, y = 176, w = 150, h = 340 } }
    },
    family_photo_room = {
        { id = "photo_room_to_living", type = "door", targetRoom = "living_room", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "family_photo_puzzle", type = "clue_object", hitbox = { x = 468, y = 154, w = 344, h = 210 }, placeholder = true },
        { id = "hidden_photo_drawer", type = "item_pickup", itemId = "study_safe_clue", hitbox = { x = 824, y = 410, w = 156, h = 76 } },
        { id = "photo_room_edge_back", type = "edge_navigation", targetRoom = "living_room", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    basement_entry = {
        { id = "basement_entry_to_garage", type = "door", targetRoom = "garage", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "basement_entry_to_main", type = "edge_navigation", targetRoom = "basement_main", hitbox = { x = 1216, y = 96, w = 64, h = 528 } }
    },
    basement_main = {
        { id = "basement_main_to_entry", type = "door", targetRoom = "basement_entry", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "basement_wall_symbols", type = "clue_object", hitbox = { x = 452, y = 168, w = 376, h = 156 }, placeholder = true },
        { id = "basement_main_to_storage", type = "door", targetRoom = "basement_storage", hitbox = { x = 548, y = 168, w = 184, h = 360 } },
        { id = "basement_main_to_altar", type = "locked_door", targetRoom = "altar_room", locked = true, hitbox = { x = 1044, y = 176, w = 150, h = 340 } }
    },
    basement_storage = {
        { id = "basement_storage_to_main", type = "door", targetRoom = "basement_main", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "basement_storage_edge_back", type = "edge_navigation", targetRoom = "basement_main", hitbox = { x = 0, y = 96, w = 64, h = 528 } }
    },
    altar_room = {
        { id = "altar_to_basement_main", type = "door", targetRoom = "basement_main", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "basement_altar", type = "puzzle_object", useTarget = "basement_altar", hitbox = { x = 492, y = 198, w = 296, h = 250 } },
        { id = "front_door_key_on_altar", type = "item_pickup", itemId = "front_door_key", visibleFlag = "front_door_key_on_altar_visible", hitbox = { x = 594, y = 370, w = 92, h = 54 } }
    },
    attic_main = {
        { id = "attic_main_to_stairwell", type = "door", targetRoom = "stairwell_2f", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "attic_main_to_toy_storage", type = "door", targetRoom = "attic_toy_storage", hitbox = { x = 548, y = 168, w = 184, h = 360 } },
        { id = "attic_main_to_album_storage", type = "door", targetRoom = "attic_album_storage", hitbox = { x = 1044, y = 176, w = 150, h = 340 } }
    },
    attic_toy_storage = {
        { id = "attic_toy_to_main", type = "door", targetRoom = "attic_main", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "attic_toy_sequence", type = "puzzle_object", useTarget = "attic_toy_sequence", hitbox = { x = 496, y = 252, w = 286, h = 166 } },
        { id = "attic_toy_box", type = "item_pickup", itemId = "small_doll", visibleFlag = "attic_toy_box_open", hitbox = { x = 820, y = 410, w = 176, h = 88 } },
        { id = "attic_toy_box_symbol", type = "item_pickup", itemId = "symbol_fragment", visibleFlag = "attic_toy_box_open", hitbox = { x = 1014, y = 410, w = 108, h = 88 } }
    },
    attic_album_storage = {
        { id = "attic_album_to_main", type = "door", targetRoom = "attic_main", hitbox = { x = 72, y = 176, w = 150, h = 340 } },
        { id = "attic_family_album_photo", type = "clue_object", hitbox = { x = 486, y = 280, w = 308, h = 142 }, placeholder = true }
    }
}

return roomObjects
