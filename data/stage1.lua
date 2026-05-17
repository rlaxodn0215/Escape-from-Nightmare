local stage1 = {
    id = "stage1",
    start_room = "child_room",
    clear_record = "stage1_clear",
    first_chain = "child_room_intro",
    clear_condition = {
        puzzle = "front_door_escape",
        required_item = "front_door_key",
        required_flags = { "final_chase_active" },
        clear_event = "event_stage1_clear"
    },
    failure_conditions = {
        captured_event = "event_player_captured"
    },
    runtime_only_state = {
        inventory = true,
        puzzle_state = true,
        unlocked_doors = true,
        monster_state = true,
        current_room = true
    },
    persistent_state = {
        settings = "saves/settings.json",
        clear_records = "saves/clear_records.json"
    },
    initial_flags = {
        electricity_restored = false,
        basement_electronic_lock_released = false,
        final_chase_ready = false,
        final_chase_active = false,
        stage1_clear = false
    },
    data_files = {
        rooms = "data/rooms.lua",
        room_objects = "data/room_objects.lua",
        items = "data/items.lua",
        puzzle_chains = "data/puzzle_chains.lua",
        puzzle_inputs = "data/puzzle_inputs.lua",
        events = "data/events.lua",
        monster_nodes = "data/monster_nodes.lua",
        sound_data = "data/sound_data.lua"
    }
}

return stage1
