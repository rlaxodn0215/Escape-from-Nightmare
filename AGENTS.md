# Escape From Nightmares Agent Guide

## Project Baseline

- Engine: Unity 6.3.9f1 (`6000.3.9f1`)
- Render stack: Universal Render Pipeline 2D
- Language: C#
- Input: Unity Input System, mouse click only for player gameplay
- UI: UGUI Canvas, EventSystem, TextMeshPro
- Platform: Windows PC
- Goal: playable Stage 1 MVP of a first-person point-and-click horror escape game

## Reading Order

Use `docs/*.md` as compressed guardrails and `design/*.txt` as the detailed source.

1. Read `docs/PRD.md`, `docs/ARCHITECTURE.md`, `docs/ADR.md`, and `docs/UI_GUIDE.md` before implementation.
2. Read only the relevant `design/*.txt` files for the current task.
3. Use `design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt` for implementation order and completion criteria.
4. Use `design/08_REMAINING_TASKS.txt` for unresolved production details.

## Context Map

- Project overview and horror direction: `design/00_PROJECT_OVERVIEW.txt`
- UI, input, save, restart rules: `design/01_GAME_SYSTEMS_UI_RULES.txt`
- Stage 1 room structure: `design/02_STAGE1_SPACE_ROOMS.txt`
- Puzzles, items, events: `design/03_PUZZLES_ITEMS_EVENTS.txt`
- Monster and hiding behavior: `design/04_MONSTER_HIDING_AI.txt`
- Unity implementation structure and data assets: `design/05_IMPLEMENTATION_STRUCTURE.txt`
- Resource list and placeholder policy: `design/06_RESOURCES_LIST.txt`
- Development instructions: `design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- Remaining tasks: `design/08_REMAINING_TASKS.txt`

## Unity Folder Policy

Runtime implementation must live under `EscapeFromNightmares/Assets`.

Recommended folders:

- `Assets/Scenes`
- `Assets/Scripts`
- `Assets/Scripts/Core`
- `Assets/Scripts/Systems`
- `Assets/Scripts/UI`
- `Assets/Scripts/AI`
- `Assets/Scripts/Data`
- `Assets/ScriptableObjects`
- `Assets/Prefabs`
- `Assets/Sprites`
- `Assets/Audio`
- `Assets/UI`
- `Assets/Tests`

Do not manually edit or commit generated Unity folders such as `Library/`, `Logs/`, `Temp/`, `Obj/`, `Build/`, or `UserSettings/`.

## Unity MCP Policy

The Unity project includes `com.coplaydev.unity-mcp` in `EscapeFromNightmares/Packages/manifest.json`. When Unity MCP tools are callable, use them for Unity-native checks before relying on text-only inspection.

Use Unity MCP for:

- scene hierarchy inspection
- prefab existence and prefab instance checks
- missing component checks
- serialized reference and asset reference checks
- Build Settings scene checks

If Unity MCP tools are not callable in the current session, record that limitation and use Unity BatchMode plus static YAML/project-file validation as the fallback. Do not claim a Unity MCP validation passed unless an MCP tool actually performed it.

## Scene Hierarchy Standard

Playable Unity scenes must include these root objects before a step can be considered complete:

- `StageRoot`: owns the active stage controller and `StageDefinition` reference
- `Systems`: holds runtime managers such as game state, room, interaction, inventory, puzzle, event, monster, hiding, sound, and map systems
- `RoomView`: displays the current room background and room-level visual layers
- `InteractableLayer`: contains clickable hotspot instances, screen-edge hotspots, doors, items, and hide spots
- `MonsterLayer`: contains monster overlays, chase visuals, silhouette events, and monster presentation objects
- `AudioRoot`: contains BGM, ambience, SFX, monster, UI, and event audio sources or audio emitters
- `UICanvas`: contains HUD, title/pause/settings/inventory/map/puzzle/hiding/game-over UI
- `EventSystem`: Unity UI EventSystem and Input System UI module
- `DebugRoot`: optional development-only diagnostics, disabled or stripped for release builds

If a scene only contains `SampleScene` defaults such as `Main Camera` and `Global Light 2D`, it is not a playable project scene.

## Prefab Completion Standard

Scripts alone are not enough for Unity feature completion. A feature that needs scene or UI presentation is complete only when its prefab or scene object is created and connected.

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

Each prefab must either be placed in a scene or referenced by a ScriptableObject or manager. Unreferenced prefab files do not count as completed gameplay work.

## Collaborative Unit Rule

Build the game with the user one game unit at a time. A game unit can be one prefab, one scene, one room or room connection, one puzzle, one UI surface, one monster event, one resource set, or one focused system slice.

Before implementing any game unit:

- enter `design_review`
- present a `Pre-Implementation Proposal`
- record `User Decisions`
- define `Out of Scope`
- wait for `approved_for_implementation`

Do not create files, prefabs, scenes, resources, or C# implementation before `approved_for_implementation`.

Default unit metadata:

- `unit_type`: `resource`, `prefab`, `scene`, `room`, `puzzle`, `ui`, `monster_event`, or `system`
- `unit_id`: concrete target such as `TitleUI.prefab`, `child_room`, or `study_safe`
- `requires_user_design_approval`: `true`

Do not combine multiple prefabs, scenes, rooms, or puzzles into one step unless the user explicitly approves the exception during `design_review`.

## Data Model Standard

Stage data should be data-driven through ScriptableObject assets or Serializable C# data types.

- `RoomDefinition`: room ID, background sprite, movement links, danger modifier, monster entry flag
- `InteractableDefinition`: room ID, interactable ID, type, click area, required items or flags, event ID
- `ItemDefinition`: item ID, display name, icon, acquisition/use/combine metadata
- `PuzzleDefinition`: puzzle ID, input type, answer, success event, failure event
- `MonsterNodeDefinition`: node ID, room ID, connected nodes, search/chase weights
- `StageDefinition`: start room, initial flags, clear condition, fail condition

Keep runtime systems reusable. Keep Stage 1 content in data assets where practical.

## Hard Rules

- Keep the first-person point-and-click format.
- Player gameplay uses mouse clicks only.
- Do not show hover highlights or obvious clickable markers on room objects.
- Do not implement progress saves, checkpoints, or autosaves.
- Save only settings and the `stage1_clear` record.
- Do not invent additional stages, rooms, protagonists, or long story text unless the design files are updated first.
- Use placeholder art/audio only when final resources are missing, and keep paths replaceable.
- Run `resource-inventory` before C# systems, scene, prefab, or build work.
- Do not begin gameplay implementation while required resource status is unknown.
- Do not begin a game unit implementation before `approved_for_implementation`.
- Keep each implementation step scoped to one approved game unit.

## Resource-First Rule

The first Harness step must be `resource-inventory`. It inventories every required filename from `design/06_RESOURCES_LIST.txt`, maps each one to a Unity target path, and records whether it is `missing`, `placeholder`, or `final`.

The resource inventory must produce `resource_manifest.json` or an equivalent reviewable artifact with:

- `required_filename`
- `unity_target_path`
- `category`
- `status`
- `referenced_by`

Block development if required images, audio, or prefabs are missing and no approved placeholder plan exists. Also block on zero-byte files, wrong extensions, Unity import failures, or design/resource naming mismatches such as an item ID not matching its expected icon filename.

## Verification

Prefer these checks when Unity is available:

```powershell
Unity.exe -batchmode -quit -projectPath EscapeFromNightmares -runTests -testPlatform EditMode
Unity.exe -batchmode -quit -projectPath EscapeFromNightmares -runTests -testPlatform PlayMode
Unity.exe -batchmode -quit -projectPath EscapeFromNightmares -executeMethod BuildScript.BuildWindows
```

If the local Unity executable or build script is unavailable, record the check as blocked with the exact missing requirement.

Before any playable build step, also verify:

- `ResourceManifestValidation`
- `RequiredAssetPresenceValidation`
- `ZeroByteAssetValidation`
- `UnityImportableAssetValidation`
- `DesignResourceConsistencyValidation`
- `SceneHierarchyValidation`
- `PrefabReferenceValidation`
- `ScriptableObjectReferenceValidation`
- `BuildSettingsValidation`
- `SampleScene` is not the only enabled Build Settings scene
