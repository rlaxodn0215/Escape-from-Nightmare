# Step 4: item-study-safe-clue-resource

## Game Unit

- unit_type: resource
- unit_id: item_study_safe_clue.png
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

- Prior ready-for-review resource units were approved by the user instruction to proceed according to the Harness.
- Stage 1 audio targets are recorded as final WAV resources.
- Item icon status is now 1 final and 9 placeholder.
- The next single item icon resource is `item_study_safe_clue.png`.
- Existing target path: `EscapeFromNightmares/Assets/Sprites/Items/item_study_safe_clue.png`.
- Current manifest status: `placeholder`.
- This step is in `design_review`; no image generation or asset replacement has started.

## Pre-Implementation Proposal

Create the final inventory icon for `item_study_safe_clue.png` as a Unity-importable PNG. The icon should depict the clue used to solve the `study_safe` number lock after the family photo drawer sequence.

Resource requirements:

- Output path: `EscapeFromNightmares/Assets/Sprites/Items/item_study_safe_clue.png`
- Target display size: square item icon, normalized to `256x256`
- Style: dark hand-drawn 2D, dusty paper/photo clue, thin uneven lines, muted grayscale with weak red accent only if needed
- Subject: a small damaged photo-backed note or clue scrap that implies family-photo order and bookshelf numbers without readable text
- Constraints: no readable text, no explicit answer digits, no UI frame, no glow highlight, no cursor, no click marker, no watermark
- Manifest update after generation: set only this entry from `placeholder` to `final`

## User Decisions

- 2026-05-19: User approved all approval-gated work needed to proceed according to the Harness; this was treated as `approved_for_implementation` for `item_study_safe_clue.png`.
- No naming changes are proposed for this unit.
- The existing `fuse_holder` / `item_electric_part.png` naming mismatch remains out of scope.

## Decision Log

- 2026-05-19: Created this design-review step after the user instructed Codex to proceed according to the Harness.
- 2026-05-19: Scoped the step to one resource file only: `item_study_safe_clue.png`.
- 2026-05-19: Started implementation for the approved single resource unit.
- 2026-05-19: Used the built-in image generation path through the `imagegen` skill; no CLI fallback or API key was used.
- 2026-05-19: Replaced only `item_study_safe_clue.png` and preserved the existing Unity `.meta` file.
- 2026-05-19: User approved all approval-gated work needed to proceed according to the Harness; this ready-for-review step was treated as approved and completed.

## Out of Scope

- Other item icons.
- Puzzle implementation for `study_safe`.
- Inventory UI prefab or scene wiring.
- ScriptableObject item data.
- Resolving the separate `fuse_holder` / `item_electric_part.png` naming mismatch.
- Unity validation tooling.

## Resource Inventory

Current item icon queue after this unit:

- `item_electric_part.png`
- `item_fuse.png`
- `item_broken_hand_mirror.png`
- `item_small_doll.png`
- `item_old_keychain.png`
- `item_old_necklace.png`
- `item_symbol_fragment.png`
- `item_front_door_key.png`

## Acceptance Criteria

- `item_study_safe_clue.png` exists at the Unity target path.
- PNG dimensions are `256x256`.
- File is non-zero byte and loadable as an image.
- `resource_manifest.json` marks only `item_study_safe_clue.png` as `final`.
- Existing `item_torn_drawing_fragment.png` and audio `final` statuses remain unchanged.

## Validation Results

- PASS: `EscapeFromNightmares/Assets/Sprites/Items/item_study_safe_clue.png` exists.
- PASS: Image loads via `System.Drawing`.
- PASS: Dimensions are `256x256`.
- PASS: File is non-zero byte: `127377` bytes.
- PASS: `resource_manifest.json` changed `item_study_safe_clue.png` from `placeholder` to `final`.
- NOT RUN: Unity import validation was not run because Unity MCP tools are not callable in this session and no approved Unity validation tooling exists yet.

## Review Artifact

- `phases/0-resource-inventory/step4.md`
- `phases/0-resource-inventory/index.json`
- `resource_manifest.json`
- `EscapeFromNightmares/Assets/Sprites/Items/item_study_safe_clue.png`
- `EscapeFromNightmares/Assets/Sprites/Items/item_study_safe_clue.png.meta`

## Current State

- Status: `completed`.
- The generated final icon has replaced the placeholder PNG at the Unity target path.
- The existing `.meta` file remains unchanged.
- This step did not create gameplay scripts, prefabs, ScriptableObjects, scenes, or UI wiring.
- The next listed item icon is `item_electric_part.png`, but that resource is tied to the known `fuse_holder` / `item_electric_part.png` design-resource naming mismatch.

## Resume Instructions

Read this file, `phases/0-resource-inventory/index.json`, and `resource_manifest.json`. This step is complete; continue with the next active Harness step.

## Next Action

Continue with the next active Harness step.

## Next Step Blocker

Do not start the next item icon until this `ready_for_review` step is approved. The broader resource inventory still has Unity-native validation blockers and the `fuse_holder` / `item_electric_part.png` naming decision.
