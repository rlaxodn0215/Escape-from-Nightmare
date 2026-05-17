# Architecture

## Project Shape
```text
main.lua
conf.lua
src/
  core/       # game loop helpers, state manager, input, camera, save manager
  scenes/     # title, game, pause, gameover, ending
  systems/    # room, interaction, inventory, puzzle, event, hiding, sound, map
  ai/         # monster FSM, danger values, monster movement
  ui/         # inventory, puzzle, map, danger gauge, settings
data/         # rooms, objects, items, puzzles, events, monster nodes, sounds, stage
assets/       # images, sounds, fonts
saves/        # settings and clear records only
build/        # .love and Windows package outputs
```

## Core Pattern
- Use a data-driven LÖVE 2D architecture.
- Keep stage content in `data/*.lua`; keep runtime behavior in `src/*`.
- Rooms, clickable objects, items, puzzle inputs, puzzle chains, events, monster nodes, sound metadata, and Stage 1 rules must be declarative data where practical.
- Clickable objects use rectangular hitboxes: `{x, y, w, h}`.

## Runtime Flow
```text
LÖVE callbacks
-> state manager
-> active scene
-> input system
-> room / interaction / puzzle / inventory / monster systems
-> event system
-> UI and rendering
```

## State and Saves
- Runtime progress lives in memory and resets on game over.
- `saves/settings.json` may store BGM/SFX settings.
- `saves/clear_records.json` may store `stage1_clear`.
- Do not persist inventory, puzzle state, unlocked doors, monster state, current room, or checkpoints.

## Source Details
- Full folder design: `design/05_IMPLEMENTATION_STRUCTURE.txt`
- Stage 1 room structure: `design/02_STAGE1_SPACE_ROOMS.txt`
- Puzzle, item, and event data: `design/03_PUZZLES_ITEMS_EVENTS.txt`
- Monster and hiding behavior: `design/04_MONSTER_HIDING_AI.txt`
