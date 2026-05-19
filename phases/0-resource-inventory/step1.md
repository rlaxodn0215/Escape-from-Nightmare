# Step 1: item-torn-drawing-fragment-resource

## Game Unit

- unit_type: resource
- unit_id: item_torn_drawing_fragment.png
- requires_user_design_approval: true

## Read Before Work

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/docs/CODEX_HARNESS.md`
- `/design/03_PUZZLES_ITEMS_EVENTS.txt`
- `/design/06_RESOURCES_LIST.txt`
- `/resource_manifest.json`

## Current State

- All room background resources are complete: 27 / 27 `room_background` entries are `final`.
- Item icon resources are not started: 0 / 10 `item_icon` entries are `final`.
- The next current work item is `item_torn_drawing_fragment.png`.
- Existing target path: `EscapeFromNightmares/Assets/Sprites/Items/item_torn_drawing_fragment.png`.
- Current manifest status: `placeholder`.

## Pre-Implementation Proposal

Create the final inventory icon for `item_torn_drawing_fragment.png` as a Unity-importable PNG. The icon should depict a torn fragment of a child's drawing, readable at inventory size, with the same dark hand-drawn horror tone as the room backgrounds.

Resource requirements:

- Output path: `EscapeFromNightmares/Assets/Sprites/Items/item_torn_drawing_fragment.png`
- Target display size: square item icon, normalized to `256x256`
- Style: dark hand-drawn 2D, dusty paper, torn edges, faint childish drawing marks
- Constraints: no readable text, no UI frame, no glow highlight, no cursor, no click marker, no watermark
- Manifest update after generation: set only this entry from `placeholder` to `final`

## User Decisions

- 2026-05-19: User instructed Codex to proceed with the next Harness-recorded task; this was treated as `approved_for_implementation` for `item_torn_drawing_fragment.png`.
- No naming changes are proposed for this unit.

## Decision Log

- 2026-05-19: Implemented only the approved single resource unit, `item_torn_drawing_fragment.png`.
- 2026-05-19: Used the built-in image generation path through the `imagegen` skill; no CLI fallback or API key was used.
- 2026-05-19: Kept the existing Unity `.meta` file in place to preserve the asset GUID.
- 2026-05-19: User instructed Codex to proceed according to the Harness; this ready-for-review step was treated as approved and completed.

## Out of Scope

- Other item icons.
- Puzzle implementation.
- Inventory UI prefab or scene wiring.
- ScriptableObject item data.
- Audio placeholder replacement.
- Resolving the separate `fuse_holder` / `item_electric_part.png` naming mismatch.

## Resource Inventory

Current item icon queue after this unit:

- `item_study_safe_clue.png`
- `item_electric_part.png`
- `item_fuse.png`
- `item_broken_hand_mirror.png`
- `item_small_doll.png`
- `item_old_keychain.png`
- `item_old_necklace.png`
- `item_symbol_fragment.png`
- `item_front_door_key.png`

## Acceptance Criteria

- `item_torn_drawing_fragment.png` exists at the Unity target path.
- PNG dimensions are `256x256`.
- File is non-zero byte and loadable as an image.
- `resource_manifest.json` marks only `item_torn_drawing_fragment.png` as `final`.
- Existing room background `final` statuses remain unchanged.

## Validation Results

- PASS: `EscapeFromNightmares/Assets/Sprites/Items/item_torn_drawing_fragment.png` exists.
- PASS: Image loads via `System.Drawing`.
- PASS: Dimensions are `256x256`.
- PASS: File is non-zero byte: `105820` bytes.
- PASS: `resource_manifest.json` changed `item_torn_drawing_fragment.png` from `placeholder` to `final`.
- NOT RUN: Unity import validation was not run because Unity MCP tools are not callable in this session and no approved Unity validation tooling exists yet.

## Review Artifact

- `EscapeFromNightmares/Assets/Sprites/Items/item_torn_drawing_fragment.png`
- `EscapeFromNightmares/Assets/Sprites/Items/item_torn_drawing_fragment.png.meta`
- `resource_manifest.json`
- `phases/0-resource-inventory/index.json`
- `phases/0-resource-inventory/step1.md`

## Current State

- Status: `completed`.
- The generated final icon has replaced the placeholder PNG at the Unity target path.
- The existing `.meta` file remains unchanged.
- This step did not create gameplay scripts, prefabs, ScriptableObjects, scenes, or UI wiring.
- The broader `resource-inventory` phase still has known blockers for non-encoded OGG placeholder markers and the `fuse_holder` / `item_electric_part.png` naming decision.

## Resume Instructions

Read this file, `phases/0-resource-inventory/index.json`, and `resource_manifest.json`. This step is complete; continue with the next active Harness step.

## Next Action

Continue with the next active Harness step.

## Next Step Blocker

The broader `resource-inventory` phase still has known blockers for non-item resources, Unity-native validation, and the `fuse_holder` / `item_electric_part.png` naming decision.
