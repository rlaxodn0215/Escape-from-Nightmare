# Step 13: remaining-visual-resources-batch

## Game Unit

- unit_type: resource
- unit_id: remaining_monster_and_ui_images
- requires_user_design_approval: true

## Read Before Work

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/docs/CODEX_HARNESS.md`
- `/design/04_MONSTER_HIDING_AI.txt`
- `/design/06_RESOURCES_LIST.txt`
- `/resource_manifest.json`

## Current State

- User approved all remaining approval-gated resource work and requested no separate approvals.
- This is an approved exception to the normal one-resource-unit cadence so the remaining visual resources can be completed together.
- Item icons are 10 / 10 final.
- Remaining placeholders before this unit:
  - monster images: 7
  - UI images: 23
- Unity MCP is callable with instance `EscapeFromNightmares@c6f9dc4f`.

## Pre-Implementation Proposal

Generate the remaining visual resources needed to complete resource inventory:

- 7 monster image resources under `EscapeFromNightmares/Assets/Sprites/Monster`
- 23 UI image resources under `EscapeFromNightmares/Assets/Sprites/UI`

Monster images should use dark hand-drawn 2D horror styling and avoid readable text, UI markers, and explicit gore. UI images should be minimal flat hand-drawn grayscale interface sprites with weak red accents only where appropriate.

## User Decisions

- 2026-05-19: User approved all approval-gated work needed to proceed according to the Harness.
- 2026-05-19: User requested that Codex not ask for separate approvals and instead create all remaining resources.
- 2026-05-19: User reported Unity MCP is active; Unity MCP checks should be used where possible.

## Decision Log

- 2026-05-19: Approved exception to combine remaining monster and UI resource files into one batch resource unit.
- 2026-05-19: Started implementation for the approved remaining visual resource batch.
- 2026-05-19: Generated 7 monster images with the built-in image generation path.
- 2026-05-19: Generated 23 UI images locally as hand-drawn grayscale UI sprites.
- 2026-05-19: Unity MCP imported `Assets/Sprites/Monster` and `Assets/Sprites/UI` and found the expected generated Texture2D assets.

## Out of Scope

- C# runtime systems.
- Unity scenes and Build Settings changes.
- Prefabs and prefab references.
- ScriptableObject data authoring.
- Gameplay wiring.

## Resource Inventory

Monster resources:

- `monster_silhouette_window.png`
- `monster_doorway_kitchen.png`
- `monster_shadow_hallway.png`
- `monster_dark_corner.png`
- `monster_near_detection.png`
- `monster_chase_overlay.png`
- `monster_gameover_shadow.png`

UI resources:

- `ui_button_start.png`
- `ui_button_settings.png`
- `ui_button_quit.png`
- `ui_button_continue.png`
- `ui_button_return_title.png`
- `ui_button_inventory.png`
- `ui_button_map.png`
- `ui_button_settings_small.png`
- `ui_inventory_panel.png`
- `ui_inventory_slot.png`
- `ui_selected_item_frame.png`
- `ui_map_panel.png`
- `ui_map_floor_1f.png`
- `ui_map_floor_2f.png`
- `ui_map_floor_basement.png`
- `ui_map_floor_attic.png`
- `ui_map_current_room_marker.png`
- `ui_puzzle_panel.png`
- `ui_number_lock_digit.png`
- `ui_symbol_button.png`
- `ui_hiding_danger_gauge_bg.png`
- `ui_hiding_danger_gauge_fill.png`
- `ui_gameover_text.png`

## Acceptance Criteria

- All listed monster and UI PNGs exist at their Unity target paths.
- Each PNG is non-zero byte and loadable as an image.
- `resource_manifest.json` marks all listed entries as `final`.
- Unity MCP asset checks can find the generated assets or record exact blocked findings.
- Existing final room, audio, and item statuses remain unchanged.

## Validation Results

- PASS: Local PNG validation passed for 7 monster images and 23 UI images.
- PASS: All checked PNG files are non-zero byte and loadable as images.
- PASS: `resource_manifest.json` marks every required resource as `final`.
- PASS: Unity MCP reimported `Assets/Sprites/Monster`.
- PASS: Unity MCP reimported `Assets/Sprites/UI`.
- PASS: Unity MCP found 7 Texture2D assets under `Assets/Sprites/Monster`.
- PASS: Unity MCP found 23 Texture2D assets under `Assets/Sprites/UI`.
- PASS: Unity MCP found 67 Texture2D assets under `Assets/Sprites`, matching 27 room, 10 item, 7 monster, and 23 UI resources.

## Review Artifact

- `phases/0-resource-inventory/step13.md`
- `phases/0-resource-inventory/index.json`
- `resource_manifest.json`
- `EscapeFromNightmares/Assets/Sprites/Monster/*.png`
- `EscapeFromNightmares/Assets/Sprites/UI/*.png`
- `reports/unity-validation/remaining_visual_resources_validation.json`

## Current State

- Status: `completed`.
- All resources in `resource_manifest.json` are now `final`.

## Resume Instructions

Read this file, `phases/0-resource-inventory/index.json`, and `resource_manifest.json`. Resource inventory is complete. Continue with Unity validation/tooling or the next approved Harness unit.

## Next Action

Resource inventory is complete. Next Harness work should move to Unity validation/tooling or the next approved game unit.

## Next Step Blocker

Do not begin C# systems, scenes, prefabs, or ScriptableObjects unless the next Harness unit is opened and its validation/tooling requirements are satisfied.
