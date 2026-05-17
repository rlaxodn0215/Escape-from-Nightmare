local monsterNodes = {
    start_node = "second_floor_hallway",
    nodes = {
        child_room = { room = "child_room", neighbors = { "second_floor_hallway" }, danger = 8, hide_spots = 1 },
        second_floor_hallway = { room = "second_floor_hallway", neighbors = { "child_room", "master_bedroom", "guest_room", "second_floor_bathroom", "study", "mirror_room", "stairwell_2f" }, danger = 12, hide_spots = 1 },
        master_bedroom = { room = "master_bedroom", neighbors = { "second_floor_hallway", "dressing_room" }, danger = 10, hide_spots = 2 },
        dressing_room = { room = "dressing_room", neighbors = { "master_bedroom" }, danger = 8, hide_spots = 1 },
        guest_room = { room = "guest_room", neighbors = { "second_floor_hallway" }, danger = 8, hide_spots = 1 },
        second_floor_bathroom = { room = "second_floor_bathroom", neighbors = { "second_floor_hallway" }, danger = 9, hide_spots = 1 },
        study = { room = "study", neighbors = { "second_floor_hallway" }, danger = 10, hide_spots = 1 },
        mirror_room = { room = "mirror_room", neighbors = { "second_floor_hallway" }, danger = 14, hide_spots = 0 },
        stairwell_2f = { room = "stairwell_2f", neighbors = { "second_floor_hallway", "stairwell_1f", "attic_main" }, danger = 14, hide_spots = 0 },
        stairwell_1f = { room = "stairwell_1f", neighbors = { "stairwell_2f", "first_floor_hallway" }, danger = 14, hide_spots = 0 },
        entrance = { room = "entrance", neighbors = { "first_floor_hallway" }, danger = 18, hide_spots = 0 },
        first_floor_hallway = { room = "first_floor_hallway", neighbors = { "entrance", "living_room", "dining_room", "first_floor_bathroom", "first_floor_storage", "stairwell_1f" }, danger = 12, hide_spots = 1 },
        living_room = { room = "living_room", neighbors = { "first_floor_hallway", "family_photo_room" }, danger = 16, hide_spots = 1 },
        dining_room = { room = "dining_room", neighbors = { "first_floor_hallway", "kitchen" }, danger = 10, hide_spots = 0 },
        kitchen = { room = "kitchen", neighbors = { "dining_room", "laundry_room", "garage" }, danger = 16, hide_spots = 1 },
        laundry_room = { room = "laundry_room", neighbors = { "kitchen" }, danger = 12, hide_spots = 1 },
        first_floor_bathroom = { room = "first_floor_bathroom", neighbors = { "first_floor_hallway" }, danger = 9, hide_spots = 1 },
        first_floor_storage = { room = "first_floor_storage", neighbors = { "first_floor_hallway", "basement_entry" }, danger = 10, hide_spots = 2 },
        garage = { room = "garage", neighbors = { "kitchen", "basement_entry" }, danger = 10, hide_spots = 1 },
        family_photo_room = { room = "family_photo_room", neighbors = { "living_room" }, danger = 10, hide_spots = 1 },
        basement_entry = { room = "basement_entry", neighbors = { "garage", "first_floor_storage", "basement_main" }, danger = 14, hide_spots = 0 },
        basement_main = { room = "basement_main", neighbors = { "basement_entry", "basement_storage", "altar_room" }, danger = 18, hide_spots = 1 },
        basement_storage = { room = "basement_storage", neighbors = { "basement_main" }, danger = 12, hide_spots = 2 },
        altar_room = { room = "altar_room", neighbors = { "basement_main" }, danger = 24, hide_spots = 0 },
        attic_main = { room = "attic_main", neighbors = { "stairwell_2f", "attic_toy_storage", "attic_album_storage" }, danger = 14, hide_spots = 1 },
        attic_toy_storage = { room = "attic_toy_storage", neighbors = { "attic_main" }, danger = 12, hide_spots = 1 },
        attic_album_storage = { room = "attic_album_storage", neighbors = { "attic_main" }, danger = 12, hide_spots = 1 }
    },
    event_spawns = {
        event_window_silhouette = "child_room",
        event_kitchen_first_appearance = "kitchen",
        event_electric_noise_attracts_monster = "laundry_room",
        event_final_chase_trigger = "altar_room"
    },
    final_chase_path = { "altar_room", "basement_main", "basement_entry", "garage", "kitchen", "dining_room", "living_room", "first_floor_hallway", "entrance" }
}

return monsterNodes
