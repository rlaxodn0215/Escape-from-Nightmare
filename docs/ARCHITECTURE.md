# Architecture

## Project Shape

```text
EscapeFromNightmares/
  Assets/
    Scenes/              # Boot, Title, Stage1, Ending or equivalent scene assets
    Scripts/
      Core/              # app bootstrap, scene flow, save manager, input gateway
      Systems/           # room, interaction, inventory, puzzle, event, hiding, sound, map
      AI/                # monster FSM, danger values, monster node movement
      UI/                # title, pause, inventory, puzzle, map, danger gauge, settings
      Data/              # ScriptableObject definitions and serializable runtime models
    ScriptableObjects/   # Stage 1 rooms, interactables, items, puzzles, events, monster nodes
    Prefabs/             # reusable UI, interactables, room view, monster overlays
    Sprites/             # room, object, item, UI, monster, effect sprites
    Audio/               # BGM, ambience, SFX, monster, UI, event audio clips
    UI/                  # fonts, UI textures, canvas prefabs if separated
    Tests/               # EditMode and PlayMode tests
  Packages/
  ProjectSettings/
```

## Core Pattern

- Use a data-driven Unity architecture.
- Keep Stage 1 content in ScriptableObject assets or Serializable C# data; keep runtime behavior in `Assets/Scripts/*`.
- Rooms, clickable objects, items, puzzle inputs, puzzle chains, events, monster nodes, sound metadata, and Stage 1 rules should be declarative where practical.
- Clickable objects use rectangular screen or RectTransform hit areas mapped to the 1280 x 720 base composition.
- Treat scripts, ScriptableObjects, prefabs, and scene hierarchy as equal parts of implementation. A feature that requires presentation is not complete until its prefab or scene object is connected.

## Scene Hierarchy

Playable scenes must use this root hierarchy:

```text
StageRoot
Systems
RoomView
InteractableLayer
MonsterLayer
AudioRoot
UICanvas
EventSystem
DebugRoot
```

- `StageRoot`: active stage controller and `StageDefinition` reference.
- `Systems`: game state, room, interaction, inventory, puzzle, event, monster, hiding, sound, and map systems.
- `RoomView`: current room background and room-level visual layers.
- `InteractableLayer`: hotspot prefab instances for objects, doors, edges, items, and hide spots.
- `MonsterLayer`: monster silhouette, chase, near-detection, and game-over presentation.
- `AudioRoot`: BGM, ambience, SFX, monster, UI, and event audio emitters.
- `UICanvas`: HUD and all modal UI surfaces.
- `EventSystem`: Unity EventSystem and Input System UI module.
- `DebugRoot`: optional development-only diagnostics.

`SampleScene` defaults alone are not a playable scene. Before playable build work, Build Settings must include `Boot` or `Stage1`, not only `Assets/Scenes/SampleScene.unity`.

## Prefab Standards

Required UI prefabs:

- title
- pause
- settings
- inventory
- map
- puzzle
- hiding gauge
- game over

Required gameplay prefabs:

- room view
- interactable hotspot
- screen edge hotspot
- hide spot
- monster overlay
- audio emitter

Each prefab must be placed in a scene or referenced by a ScriptableObject or manager. Missing scripts, missing components, or missing serialized references block the owning step.

## Runtime Flow

```text
Unity scene load
-> bootstrap / game state manager
-> active scene controller
-> Input System pointer click
-> room / interaction / puzzle / inventory / monster systems
-> event system
-> UGUI canvas and audio feedback
```

## Standard Data Types

- `RoomDefinition`: room ID, background sprite, movement links, danger modifier, monster entry flag.
- `InteractableDefinition`: room ID, interactable ID, type, click area, required items or flags, event ID.
- `ItemDefinition`: item ID, display name, icon, acquisition/use/combine metadata.
- `PuzzleDefinition`: puzzle ID, input type, answer, success event, failure event.
- `MonsterNodeDefinition`: node ID, room ID, connected nodes, search/chase weights.
- `StageDefinition`: start room, initial flags, clear condition, fail condition.

## State and Saves

- Runtime progress lives in memory and resets on game over.
- Settings may be saved with PlayerPrefs or a small JSON file in `Application.persistentDataPath`.
- Clear records may store `stage1_clear`.
- Do not persist inventory, puzzle state, unlocked doors, monster state, current room, or checkpoints.

## Build Outputs

- Windows Build output should be generated outside tracked source folders, preferably under a local ignored `Build/Windows/` path.
- Do not relocate source code, original assets, or ScriptableObject ownership into generated build output folders.
- Publish source, data assets, placeholder assets, docs, scripts, and phase metadata to version control; keep generated player builds local unless a release process explicitly uploads them.
- Windows packaging requires a valid local Unity editor installation. If Unity is unavailable, build steps must stop as `blocked`.
- Before a Windows Build, run `SceneHierarchyValidation`, `PrefabReferenceValidation`, `ScriptableObjectReferenceValidation`, and `BuildSettingsValidation`.

## Source Details

- Full folder design: `design/05_IMPLEMENTATION_STRUCTURE.txt`
- Stage 1 room structure: `design/02_STAGE1_SPACE_ROOMS.txt`
- Puzzle, item, and event data: `design/03_PUZZLES_ITEMS_EVENTS.txt`
- Monster and hiding behavior: `design/04_MONSTER_HIDING_AI.txt`
