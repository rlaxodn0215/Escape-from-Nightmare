# Harness Development Status

## Current Build Focus
- Target scene flow: `TitleScene` -> `MainScene` -> `child_room` -> `second_floor_hallway` -> `study` -> `study_safe`.
- Harness purpose: validate escape-room runtime flow before final room, puzzle, monster, and art production.
- Player-facing status/log UI is intentionally not shown. Runtime state changes should use Unity `Debug.Log`.

## Implemented Systems
- Escape runtime base:
  - `GameSession` tracks current room, current face, inventory, selected item, flags, solved puzzles, and used interactables.
  - `FlagService`, `PuzzleService`, `InteractionSystem`, and `EscapeActionResolver` handle conditions, puzzle solving, rewards, flags, and standard escape actions.
  - 4-face room navigation is active through left/right rotation.
  - `second_floor_hallway` is the current exception: it uses a 2-screen perspective hub and left/right swaps between the two hallway views.
- Sound:
  - `SoundManager` and `SoundCatalog` route BGM, SFX, UI, ambience, and monster categories.
  - Stage ambient BGM, UI click, item pickup, hide, puzzle, door, drawer open, and drawer close fallback sounds are registered.
- Title scene:
  - Image-based title background, logo, buttons, settings panel, and slider UI are set up.
  - Start targets `MainScene`.
- Main scene:
  - Direct scene view exists with camera, canvas, event system, audio listener, `GameDirector`, `SoundManager`, room view, close-up modal, hide view modal, and inventory window.
  - Existing scene references are also runtime-normalized by `GameDirector` so older scene layouts still behave correctly.

## Current `child_room` Harness
- Room structure:
  - `child_room` has four faces: North, East, South, West.
  - Current face starts on North when entering the room.
  - Moving rooms resets face direction to North.
- Interaction visibility:
  - World object sprites are hidden in the 4-face room view.
  - Transparent hitbox buttons remain for interaction.
- Face interactions:
  - North: `child_desk_drawer`
  - East: `child_room_door`
  - South: `child_bed_hide`
  - West: `child_window_silhouette`
- Desk drawer:
  - Opens a full-screen close-up instead of immediately granting the item.
  - Close-up states: closed, open with torn drawing fragment, open empty.
  - Drawer open, drawer close, and item pickup sounds are connected.
  - After `torn_drawing_fragment` is acquired, the room-level `child_desk_drawer` hitbox is disabled.
- Hide view:
  - `child_bed_hide` opens a full-screen under-bed view modal.
  - The latest image uses a low under-bed viewpoint with the top portion heavily covered by bed cloth/frame.
  - Back arrow exits the hide view and returns to the room face.
- Window:
  - West room image has the person silhouette removed.

## Current `second_floor_hallway` Harness
- Room structure:
  - `second_floor_hallway` has two perspective screens instead of the standard four faces.
  - North screen is the child-room-side view looking into the hallway.
  - South screen is the stairwell/inner-side view looking back toward the child room.
  - Left/right arrows toggle between North and South.
- Door layout:
  - North screen: `child_room`, `study`, `second_floor_bathroom`, `mirror_room`.
  - South screen: `master_bedroom`, `dressing_room`, `stairwell_2f`.
- Interaction visibility:
  - Hallway doors are painted into the background images.
  - Door objects use transparent hitboxes with `showWorldImage=false`.
- Visual continuity:
  - The child-room exit image and hallway child-room doorway were adjusted to share the same open-door direction and mood.

## Current `study` Harness
- Room structure:
  - `study` follows the standard four-face room rule.
  - North: safe/bookcase wall.
  - East: exit door back to `second_floor_hallway`.
  - South: desk and clue note.
  - West: moonlit window and portrait wall with no interaction in this milestone.
- Interaction visibility:
  - Study interactables are included in the background art.
  - `study_safe_obj`, `study_exit`, and `study_safe_clue_note` use transparent hitboxes with `showWorldImage=false`.
- Safe clue:
  - `study_safe_clue_note` opens a read-only close-up image through the shared close-up modal.
  - The clue image communicates the order `3-1-4-2` as separate aged marks/cards, not as one plain written code line.
- Safe puzzle:
  - `study_safe` remains `PuzzleType.NumberLock`.
  - Answer tokens are `3`, `1`, `4`, `2`.
  - Solving grants `fuse_holder` and sets `puzzle_study_safe_clear`.
  - Wrong answers do not grant the item or solved flag.

## UI / Modal Behavior
- Close-up and hide screens are full-screen.
- Close/exit controls use a shared back-arrow image rather than an `X`.
- Modal open/close uses fade-in/fade-out through `CanvasGroup`.
- Room direction changes and room movement use the full-screen scene transition overlay fade.
- Puzzle close-ups, clue close-ups, and hide views use modal `CanvasGroup` fade and block raycasts while transitioning.
- Inventory is opened by a dedicated inventory button and shows item icons in slots.

## Assets And Builder
- Runtime resources live under `Assets/EscapeFromNightmares/Resources/EscapeFromNightmares`.
- `RoomSpriteCatalog` directly references room backgrounds, close-up images, hide view image, inventory UI, rotation buttons, back arrow, puzzle images, and item icons.
- `Escape From Nightmares/Rebuild Main Sample Assets` is responsible for regenerating scenes, prefabs, fallback images, fallback sounds, and catalog references.
- Known limitation: Unity batchmode builder/test execution can fail with exit code `1` when the project is already locked by an open editor. Close the editor before running batch validation.
- Recently added study resources:
  - `Rooms/study_north.png`
  - `Rooms/study_east.png`
  - `Rooms/study_south.png`
  - `Rooms/study_west.png`
  - `Puzzles/study_safe.png`
  - `CloseUps/study_safe_clue_note.png`
  - `Items/item_fuse_holder.png`

## Validation Status
- Verified with C# builds:
  - `dotnet build EscapeFromNightmares.csproj /p:UseSharedCompilation=false`
  - `dotnet build EscapeFromNightmares.Editor.csproj /p:UseSharedCompilation=false`
  - `dotnet build EscapeFromNightmares.EditModeTests.csproj /p:UseSharedCompilation=false`
- Unity Test Runner:
  - Previously succeeded before the latest lock issue.
  - Latest local check could not find `Unity.exe` in PATH or `C:\Program Files\Unity\Hub\Editor`, so batch EditMode tests were not run in this environment.
- Manual validation still needed in Unity Play Mode:
  - Title -> MainScene transition.
  - Four-face rotation.
  - Hidden world-object hitboxes remain clickable.
  - Drawer close-up fade, open, item pickup, empty state, and disabled room hitbox.
  - Hide view fade and back-arrow return.
  - Inventory window item icon display.
  - Child room -> second floor hallway transition.
  - Hallway two-screen left/right transition.
  - Hallway -> study entry.
  - Study four-face rotation.
  - Study clue close-up fade open/close.
  - Study safe puzzle fade open/close.
  - `3-1-4-2` safe success grants `fuse_holder`.
  - Study East exit returns to the hallway.

## Next Recommended Work
- Run `Rebuild Main Sample Assets` from the Unity editor after scripts recompile.
- Run EditMode tests from Unity Test Runner with the project not open in another Unity process.
- Manually play through `TitleScene` -> `MainScene` and record any UI scale or hitbox adjustment needs.
- Manually tune hallway and study hitboxes against the final generated images.
- Continue with the next connected room after the `child_room` -> `second_floor_hallway` -> `study` -> `study_safe` loop is stable.
