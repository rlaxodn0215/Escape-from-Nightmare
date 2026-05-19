# Step 11: item-symbol-fragment-resource

## Game Unit

- unit_type: resource
- unit_id: item_symbol_fragment.png
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

- Prior ready-for-review resource unit `item_old_necklace.png` was approved by the user instruction to proceed according to the Harness.
- Item icon status before this unit is 8 final and 2 placeholder.
- The next item icon resource is `item_symbol_fragment.png`.
- Existing target path: `EscapeFromNightmares/Assets/Sprites/Items/item_symbol_fragment.png`.
- Current manifest status before implementation: `placeholder`.

## Pre-Implementation Proposal

Create the final inventory icon for `item_symbol_fragment.png` as a Unity-importable PNG. This resource represents the symbol fragment used as a basement altar matching clue.

Resource requirements:

- Output path: `EscapeFromNightmares/Assets/Sprites/Items/item_symbol_fragment.png`
- Target display size: square item icon, normalized to `256x256`
- Style: dark hand-drawn 2D, chipped stone or brittle paper/wood fragment, thin uneasy lines, muted grayscale with weak red accent only if needed
- Subject: an irregular broken fragment bearing a simple occult-like abstract symbol mark, not a readable letter or number, with dusty cracked texture
- Constraints: no readable text, no labels, no letters, no numbers, no UI frame, no glow highlight, no cursor, no click marker, no watermark
- Manifest update after generation: set only this entry from `placeholder` to `final`

## User Decisions

- 2026-05-19: User approved all approval-gated work needed to proceed according to the Harness.

## Decision Log

- 2026-05-19: Created this implementation step after the user approved proceeding according to the Harness.
- 2026-05-19: Scoped the step to one resource file only: `item_symbol_fragment.png`.
- 2026-05-19: Started implementation for the approved single resource unit.
- 2026-05-19: Used the built-in image generation path through the `imagegen` skill; no CLI fallback or API key was used.
- 2026-05-19: Replaced only `item_symbol_fragment.png` and preserved the existing Unity `.meta` file.

## Out of Scope

- Other item icons.
- Basement altar implementation.
- Inventory UI prefab or scene wiring.
- ScriptableObject item data.
- Unity validation tooling.

## Resource Inventory

Current item icon queue after this unit:

- `item_front_door_key.png`

## Acceptance Criteria

- `item_symbol_fragment.png` exists at the Unity target path.
- PNG dimensions are `256x256`.
- File is non-zero byte and loadable as an image.
- `resource_manifest.json` marks only `item_symbol_fragment.png` as `final`.
- Existing final item and audio statuses remain unchanged.

## Validation Results

- PASS: `EscapeFromNightmares/Assets/Sprites/Items/item_symbol_fragment.png` exists.
- PASS: Image loads via `System.Drawing`.
- PASS: Dimensions are `256x256`.
- PASS: File is non-zero byte: `88654` bytes.
- PASS: `resource_manifest.json` changed `item_symbol_fragment.png` from `placeholder` to `final`.
- NOT RUN: Unity import validation was not run because Unity MCP tools are not callable in this session and no approved Unity validation tooling exists yet.

## Review Artifact

- `phases/0-resource-inventory/step11.md`
- `phases/0-resource-inventory/index.json`
- `resource_manifest.json`
- `EscapeFromNightmares/Assets/Sprites/Items/item_symbol_fragment.png`
- `EscapeFromNightmares/Assets/Sprites/Items/item_symbol_fragment.png.meta`

## Current State

- Status: `ready_for_review`.
- The generated final icon has replaced the placeholder PNG at the Unity target path.
- The existing `.meta` file remains unchanged.
- This step did not create gameplay scripts, prefabs, ScriptableObjects, scenes, or UI wiring.

## Resume Instructions

Read this file, `phases/0-resource-inventory/index.json`, and `resource_manifest.json`. If the user accepts the icon, mark this step approved/completed and begin the next single resource unit, `item_front_door_key.png`.

## Next Action

User review is required for `item_symbol_fragment.png`. Do not start the next resource unit until this step is approved or revision instructions are provided.

## Next Step Blocker

Do not start the next item icon until this `ready_for_review` step is approved. Do not begin C# systems, scenes, prefabs, ScriptableObjects, or other item icons during this unit.
