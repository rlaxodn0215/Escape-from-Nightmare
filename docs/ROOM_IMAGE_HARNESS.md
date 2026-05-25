# Room Image Generation Harness

Use this harness before generating or applying four-face room images. The room data is the source of truth: generated art must match `connectedRoomIds`, face interactables, planned hitboxes, and the current scope notes.

## Deliverable Class

- Generated room images are `Script + Scene/Builder + ResourceCatalog update + Generated bitmap assets` when they become runtime assets.
- Visual-only inspection notes are documentation, but any visible or clickable runtime change still needs the matching builder/catalog/test coverage.
- If final lore, final object identity, or final puzzle rules are unclear, keep the asset as first-pass art and record the uncertainty instead of locking it into gameplay.

## Room Inputs

Record these before prompting image generation:

- Room id and display name.
- Connected room ids and the face that owns each exit.
- Required interactables per face, including puzzle objects, clue objects, doors, item pickups, and hide spots.
- Forbidden objects per face, especially extra doors, duplicated puzzle objects, or objects that imply unplanned gameplay.
- Normalized hitboxes for click targets.
- Current visual style and palette.

## Generation Rules

- Door count must match navigation data. If a room has one connected room, only one visible exit door should appear, and it should appear on the face with the door interactable.
- Non-door faces must not show door-like exit shapes with handles and frames. Wardrobes, cabinets, and panels are allowed only when they read clearly as furniture or puzzle surfaces.
- Major interactables must not be duplicated across faces unless the room design explicitly calls for repeated objects.
- Puzzle objects should appear only on the face that opens that puzzle.
- Hide locations should not be visually finalized unless the runtime room defines that hide spot.
- Keep perspective stable: no warped furniture, stretched walls, melted handles, impossible object scale, or repeated/duplicated object fragments.
- Do not include readable text, numbers, UI labels, or characters unless the feature specifically requires them.
- Place clickable objects inside the planned hitbox area with enough visual margin for later tuning.

## Prompt Template

```text
Create a 1280x720 dark gothic 2D escape-room background for [room id], [face direction].
Room topology: connected exits are [list], and this face [does/does not] contain an exit door.
Required objects on this face: [objects].
Forbidden objects on this face: [objects].
Composition: [camera angle, main wall, furniture placement].
Hitbox target area: [normalized rect and visual description].
Style: moonlit blue-black palette, aged wood, worn wallpaper, no text, no numbers, no UI labels, no characters.
Quality constraints: no image distortion, no duplicate major objects, no extra door-like shapes, clean straight perspective.
```

## Inspection Checklist

- The face count and direction match the runtime room definition.
- The number of visible exit doors matches `connectedRoomIds`.
- Exit doors appear only on the face that has the door interactable.
- Required puzzle/clue/item objects appear once and in the expected face.
- Ambient faces do not imply new gameplay rules.
- Object placement matches planned hitboxes.
- No image distortion, duplicate major objects, accidental characters, readable text, or UI-like labels.
- New or replaced images load through `Resources` and `RoomSpriteCatalog`.

## Strict Close-Up Identity Harness

Use this stricter harness when a room face has an interactable that opens a close-up, puzzle, or object-state image.

- Pass only when the opened image is the same source image as the room face target area, cropped and enlarged without repainting or regeneration.
- Similar composition, matching style, matching object type, or matching symbol/color order is not enough.
- If the close-up adds details that are not visible in the face image, changes object shape, changes lighting, changes framing geometry, or uses a separately generated variant, mark it Fail.
- If there is no recorded source crop rect or no preserved crop-derived artifact trail, mark it Fail until it is regenerated from the room face crop.
- State-specific close-ups can differ only by the intended state delta, such as drawer open/closed or item present/empty; the unchanged surrounding pixels still need to come from the same face/source crop.

Required QA record for each close-up pair:

- Source room face resource.
- Interactable id and target close-up/puzzle resource.
- Source crop rect in normalized face coordinates.
- Result: Pass, Fail, or N/A.
- One-line reason, especially whether the image is crop-derived or independently generated.

Current strict identity crop cases after AI-generated replacement:

- `EscapeFromNightmares/Rooms/second_floor_bathroom_north` -> `EscapeFromNightmares/CloseUps/bathroom_mirror_rule_clue`, `Rect(0.26, 0.24, 0.48, 0.48)`, Exception. AI-generated replacement is intentional and is not crop-derived.
- `EscapeFromNightmares/Rooms/dressing_room_north` -> `EscapeFromNightmares/CloseUps/dressing_color_sequence_clue`, `Rect(0.22, 0.18, 0.56, 0.56)`, Exception. AI-generated replacement is intentional and is not crop-derived.
- `EscapeFromNightmares/Rooms/mirror_room_north` -> `EscapeFromNightmares/Puzzles/mirror_symbol_panel`, `Rect(0.32, 0.13, 0.36, 0.36)`, Exception. AI-generated replacement is intentional and is not crop-derived.
- `EscapeFromNightmares/Rooms/master_bedroom_north` -> `EscapeFromNightmares/Puzzles/master_bedroom_drawer`, `Rect(0.36, 0.20, 0.48, 0.48)`, Exception. AI-generated replacement is intentional and is not crop-derived.

## AI-Generated Close-Up/Puzzle Harness

Use this harness when the accepted objective is high-quality generated close-up or puzzle art instead of strict room-face pixel identity.

- General close-up and puzzle PNGs must be `1280x720`.
- `study_safe_digit_0.png` through `study_safe_digit_9.png` must stay `180x220`.
- All assets must keep their existing `Resources` paths and `.meta` GUIDs.
- General generated images must avoid readable text, numbers, UI labels, watermarks, and characters unless the asset role explicitly requires them.
- Safe digit assets are the only number exception: each must show exactly one Arabic numeral matching its filename, with no other text or numbers.
- Replaced assets must continue to load as `Sprite` resources through the existing catalog/runtime paths.
- Strict identity pairs that receive AI-generated replacements must be recorded as `Exception`, not `Pass`.

## Current Inspection: master_bedroom

- North `master_bedroom_north.png`: Pass after regeneration. The color drawer/puzzle area is present and there is no extra exit-door shape on this non-exit face.
- East `master_bedroom_east.png`: Pass. The image presents one clear hallway exit door, matching `master_exit`.
- South `master_bedroom_south.png`: Pass. The image is ambient bed/wardrobe composition and does not present a clear exit door.
- West `master_bedroom_west.png`: Pass with note. The image is ambient moonlit keepsake/storage composition; portrait figures read as paintings, not characters.

## Current Inspection: attic_toy_chain

- `stairwell_2f_north/east/south.png`: First-pass AI-generated runtime art. Each face owns one planned exit and should not imply extra exits.
- `stairwell_2f_west.png`: First-pass ambient wall art with no planned interaction.
- `attic_main_north.png`: First-pass album clue face. The only planned interaction is `attic_family_album_photo`.
- `attic_main_east/south.png`: First-pass exit faces for toy storage and 2F stairwell.
- `attic_main_west.png`: First-pass ambient attic storage art with no planned interaction.
- `attic_toy_storage_north.png`: First-pass toy box puzzle face. The only planned interaction is `attic_toy_box`.
- `attic_toy_storage_east.png`: First-pass exit face back to `attic_main`.
- `attic_toy_storage_south/west.png`: First-pass ambient toy storage faces with no planned interaction.
- `attic_family_album_photo.png`, `attic_toy_sequence.png`, `item_small_doll.png`, and `item_symbol_fragment.png`: AI-generated runtime assets. Strict room-face crop identity is not claimed for these assets.

## Current Inspection: first_floor_laundry_power_chain
- `stairwell_1f_north/east.png`: First-pass exit faces for the 2F stairwell and 1F hallway.
- `stairwell_1f_south/west.png`: First-pass ambient stairwell faces with no planned interaction.
- `first_floor_hallway_north.png`: Two-screen hub face containing `stairwell_1f`, `dining_room`, `kitchen`, and `laundry_room` exits.
- `first_floor_hallway_south.png`: Two-screen hub face containing `entrance`, `living_room`, and `family_photo_room` exits.
- `dining_room_north.png`: First-pass dining clue face. The only planned interaction is `dining_seat_order_clue`.
- `dining_room_east.png`: First-pass exit face back to `first_floor_hallway`.
- `dining_room_south/west.png`: First-pass ambient dining faces with no planned interaction.
- `kitchen_north.png`: First-pass clock clue face. The only planned interaction is `kitchen_clock_clue`.
- `kitchen_east.png`: First-pass exit face back to `first_floor_hallway`.
- `kitchen_south.png`: First-pass hide spot face. The only planned interaction is `kitchen_hide`.
- `kitchen_west.png`: First-pass ambient kitchen face with no planned interaction.
- `laundry_room_north.png`: First-pass storage box puzzle face. The only planned interaction is `laundry_box`.
- `laundry_room_east.png`: First-pass breaker box puzzle face. The only planned interaction is `breaker_box_obj`.
- `laundry_room_south.png`: First-pass hide spot face. The only planned interaction is `laundry_hide`.
- `laundry_room_west.png`: First-pass exit face back to `first_floor_hallway`.
- `dining_seat_order_clue.png`, `kitchen_clock_clue.png`, `kitchen_sink_hide_view.png`, `laundry_machine_hide_view.png`, `item_fuse.png`, and `item_old_keychain.png`: AI-generated runtime assets. Strict room-face crop identity is not claimed for these assets.

## Current Inspection: basement_altar_chain
- `laundry_room_east.png`: Regenerated in place. The face contains the existing breaker box and the basement entry door/hatch; the existing `.meta` GUID is preserved.
- `basement_entry_north/east.png`: First-pass generated exit faces for `laundry_room` and `basement_main`.
- `basement_entry_south/west.png`: First-pass generated ambient basement entry faces with no planned interaction.
- `basement_main_north.png`: First-pass generated clue face containing `basement_wall_symbols`.
- `basement_main_east/south.png`: First-pass generated exit faces for `altar_room` and `basement_entry`.
- `basement_main_west.png`: First-pass generated hide-spot face containing `basement_main_hide`.
- `altar_room_north.png`: First-pass generated altar puzzle face with no visible front door key.
- `altar_room_north_key_spawned.png`: Conditional generated state after `front_door_key_spawned`, with the front door key visible on the altar.
- `altar_room_north_key_taken.png`: Conditional generated state after `front_door_key` is acquired, with the altar visible and key gone.
- `altar_room_south.png`: First-pass generated exit face back to `basement_main`.
- `altar_room_east/west.png`: First-pass generated ambient altar-room faces with no planned interaction.
- `basement_wall_symbols.png`, `basement_altar.png`, `basement_main_hide_view.png`, and `item_front_door_key.png`: AI-generated runtime assets registered in `RoomSpriteCatalog.asset` and `TitleSceneAssetBuilder`.

## Current Inspection: stage1_clear_ending_screen
- `Endings/stage1_clear_background.png`: First-pass generated ending background for the clear screen. It shows the front door open to dawn, contains no in-image text, UI, characters, or watermark, and is registered in `RoomSpriteCatalog.asset` and `TitleSceneAssetBuilder`.
