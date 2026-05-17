local rooms = {
    startRoom = "child_room",
    list = {
        entrance = {
            floor = "1f",
            background = "assets/images/rooms/entrance.png",
            connections = { first_floor_hallway = "first_floor_hallway" }
        },
        first_floor_hallway = {
            floor = "1f",
            background = "assets/images/rooms/first_floor_hallway.png",
            connections = {
                entrance = "entrance",
                living_room = "living_room",
                dining_room = "dining_room",
                first_floor_bathroom = "first_floor_bathroom",
                first_floor_storage = "first_floor_storage",
                stairwell_1f = "stairwell_1f"
            }
        },
        living_room = {
            floor = "1f",
            background = "assets/images/rooms/living_room.png",
            connections = { first_floor_hallway = "first_floor_hallway", family_photo_room = "family_photo_room" }
        },
        dining_room = {
            floor = "1f",
            background = "assets/images/rooms/dining_room.png",
            connections = { first_floor_hallway = "first_floor_hallway", kitchen = "kitchen" }
        },
        kitchen = {
            floor = "1f",
            background = "assets/images/rooms/kitchen.png",
            connections = { dining_room = "dining_room", laundry_room = "laundry_room", garage = "garage" }
        },
        laundry_room = {
            floor = "1f",
            background = "assets/images/rooms/laundry_room.png",
            connections = { kitchen = "kitchen" }
        },
        first_floor_bathroom = {
            floor = "1f",
            background = "assets/images/rooms/first_floor_bathroom.png",
            connections = { first_floor_hallway = "first_floor_hallway" }
        },
        first_floor_storage = {
            floor = "1f",
            background = "assets/images/rooms/first_floor_storage.png",
            connections = { first_floor_hallway = "first_floor_hallway", basement_entry = "basement_entry" }
        },
        garage = {
            floor = "1f",
            background = "assets/images/rooms/garage.png",
            connections = { kitchen = "kitchen", basement_entry = "basement_entry" }
        },
        family_photo_room = {
            floor = "1f",
            background = "assets/images/rooms/family_photo_room.png",
            connections = { living_room = "living_room" }
        },
        stairwell_1f = {
            floor = "1f",
            background = "assets/images/rooms/stairwell_1f.png",
            connections = { first_floor_hallway = "first_floor_hallway", stairwell_2f = "stairwell_2f" }
        },
        child_room = {
            floor = "2f",
            background = "assets/images/rooms/child_room.png",
            connections = { second_floor_hallway = "second_floor_hallway" }
        },
        second_floor_hallway = {
            floor = "2f",
            background = "assets/images/rooms/second_floor_hallway.png",
            connections = {
                child_room = "child_room",
                master_bedroom = "master_bedroom",
                guest_room = "guest_room",
                second_floor_bathroom = "second_floor_bathroom",
                study = "study",
                mirror_room = "mirror_room",
                stairwell_2f = "stairwell_2f"
            }
        },
        master_bedroom = {
            floor = "2f",
            background = "assets/images/rooms/master_bedroom.png",
            connections = { second_floor_hallway = "second_floor_hallway", dressing_room = "dressing_room" }
        },
        dressing_room = {
            floor = "2f",
            background = "assets/images/rooms/dressing_room.png",
            connections = { master_bedroom = "master_bedroom" }
        },
        guest_room = {
            floor = "2f",
            background = "assets/images/rooms/guest_room.png",
            connections = { second_floor_hallway = "second_floor_hallway" }
        },
        second_floor_bathroom = {
            floor = "2f",
            background = "assets/images/rooms/second_floor_bathroom.png",
            connections = { second_floor_hallway = "second_floor_hallway" }
        },
        study = {
            floor = "2f",
            background = "assets/images/rooms/study.png",
            connections = { second_floor_hallway = "second_floor_hallway" }
        },
        mirror_room = {
            floor = "2f",
            background = "assets/images/rooms/mirror_room.png",
            connections = { second_floor_hallway = "second_floor_hallway" }
        },
        stairwell_2f = {
            floor = "2f",
            background = "assets/images/rooms/stairwell_2f.png",
            connections = { second_floor_hallway = "second_floor_hallway", stairwell_1f = "stairwell_1f", attic_main = "attic_main" }
        },
        basement_entry = {
            floor = "basement",
            background = "assets/images/rooms/basement_entry.png",
            connections = { garage = "garage", first_floor_storage = "first_floor_storage", basement_main = "basement_main" }
        },
        basement_main = {
            floor = "basement",
            background = "assets/images/rooms/basement_main.png",
            connections = { basement_entry = "basement_entry", basement_storage = "basement_storage", altar_room = "altar_room" }
        },
        basement_storage = {
            floor = "basement",
            background = "assets/images/rooms/basement_storage.png",
            connections = { basement_main = "basement_main" }
        },
        altar_room = {
            floor = "basement",
            background = "assets/images/rooms/altar_room.png",
            connections = { basement_main = "basement_main" }
        },
        attic_main = {
            floor = "attic",
            background = "assets/images/rooms/attic_main.png",
            connections = { stairwell_2f = "stairwell_2f", attic_toy_storage = "attic_toy_storage", attic_album_storage = "attic_album_storage" }
        },
        attic_toy_storage = {
            floor = "attic",
            background = "assets/images/rooms/attic_toy_storage.png",
            connections = { attic_main = "attic_main" }
        },
        attic_album_storage = {
            floor = "attic",
            background = "assets/images/rooms/attic_album_storage.png",
            connections = { attic_main = "attic_main" }
        }
    }
}

return rooms
