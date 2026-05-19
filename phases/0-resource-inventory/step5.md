# Step 5: item-electric-part-resource

## Game Unit

- unit_type: resource
- unit_id: item_electric_part.png
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

- Prior ready-for-review resource unit `item_study_safe_clue.png` was approved by the user instruction to proceed according to the Harness.
- Item icon status before this unit is 2 final and 8 placeholder.
- The next item icon resource is `item_electric_part.png`.
- Existing target path: `EscapeFromNightmares/Assets/Sprites/Items/item_electric_part.png`.
- Current manifest status before implementation: `placeholder`.

## Pre-Implementation Proposal

Create the final inventory icon for `item_electric_part.png` as a Unity-importable PNG. This resource visually represents the design item `fuse_holder`, acquired from the `study_safe` and used at the laundry room breaker box.

Resource requirements:

- Output path: `EscapeFromNightmares/Assets/Sprites/Items/item_electric_part.png`
- Target display size: square item icon, normalized to `256x256`
- Style: dark hand-drawn 2D, dusty metal and ceramic electrical component, thin uneasy lines, muted grayscale with weak red rust/copper accent if needed
- Subject: an old fuse holder or small electrical part with cracked ceramic, tarnished metal contacts, and a slightly unsafe antique-house feel
- Constraints: no readable text, no labels, no UI frame, no glow highlight, no cursor, no click marker, no watermark
- Manifest update after generation: set only this entry from `placeholder` to `final`

## User Decisions

- 2026-05-19: User approved all approval-gated work needed to proceed according to the Harness.
- 2026-05-19: For this unit, preserve the design item ID `fuse_holder` and keep `item_electric_part.png` as the resource filename/icon. No design ID rename is performed.

## Decision Log

- 2026-05-19: Created this implementation step after the user approved proceeding according to the Harness.
- 2026-05-19: Scoped the step to one resource file only: `item_electric_part.png`.
- 2026-05-19: Started implementation for the approved single resource unit.
- 2026-05-19: Used the built-in image generation path through the `imagegen` skill; no CLI fallback or API key was used.
- 2026-05-19: Replaced only `item_electric_part.png` and preserved the existing Unity `.meta` file.
- 2026-05-19: User approved all approval-gated work needed to proceed according to the Harness; this ready-for-review step was treated as approved and completed.

## Out of Scope

- Other item icons.
- Puzzle implementation for `study_safe`.
- Laundry breaker-box implementation.
- Inventory UI prefab or scene wiring.
- ScriptableObject item data.
- Renaming design item IDs.
- Unity validation tooling.

## Resource Inventory

Current item icon queue after this unit:

- `item_fuse.png`
- `item_broken_hand_mirror.png`
- `item_small_doll.png`
- `item_old_keychain.png`
- `item_old_necklace.png`
- `item_symbol_fragment.png`
- `item_front_door_key.png`

## Acceptance Criteria

- `item_electric_part.png` exists at the Unity target path.
- PNG dimensions are `256x256`.
- File is non-zero byte and loadable as an image.
- `resource_manifest.json` marks only `item_electric_part.png` as `final`.
- Existing final item and audio statuses remain unchanged.

## Validation Results

- PASS: `EscapeFromNightmares/Assets/Sprites/Items/item_electric_part.png` exists.
- PASS: Image loads via `System.Drawing`.
- PASS: Dimensions are `256x256`.
- PASS: File is non-zero byte: `73571` bytes.
- PASS: `resource_manifest.json` changed `item_electric_part.png` from `placeholder` to `final`.
- NOT RUN: Unity import validation was not run because Unity MCP tools are not callable in this session and no approved Unity validation tooling exists yet.

## Review Artifact

- `phases/0-resource-inventory/step5.md`
- `phases/0-resource-inventory/index.json`
- `resource_manifest.json`
- `EscapeFromNightmares/Assets/Sprites/Items/item_electric_part.png`
- `EscapeFromNightmares/Assets/Sprites/Items/item_electric_part.png.meta`

## Current State

- Status: `completed`.
- The generated final icon has replaced the placeholder PNG at the Unity target path.
- The existing `.meta` file remains unchanged.
- This step did not create gameplay scripts, prefabs, ScriptableObjects, scenes, or UI wiring.

## Resume Instructions

Read this file, `phases/0-resource-inventory/index.json`, and `resource_manifest.json`. This step is complete; continue with the next active Harness step.

## Next Action

Continue with the next active Harness step.

## Next Step Blocker

Do not start the next item icon until this `ready_for_review` step is approved. Do not begin C# systems, scenes, prefabs, ScriptableObjects, or other item icons during this unit.
