# Step 0: HUD Inventory Button

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `HUDInventoryButton`
- `requires_user_design_approval`: `true`
- Goal: create an in-game HUD inventory button that opens the existing `InventoryUI` scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: User asked to proceed with the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No map or settings HUD buttons.
- No title, pause, settings, map, puzzle, hiding, or game-over prefabs.
- No room-specific item pickup hotspot placement.
- No item combination UI.
- No progress save, checkpoint, or autosave.

## Decision Log

- 2026-05-20: Use the existing `ui_button_inventory.png` sprite.
- 2026-05-20: Keep the button as an always-visible HUD control under `UICanvas`.
- 2026-05-20: Use a tiny bridge component that calls `InventoryUI.Show()` so the inactive inventory panel can be opened.

## Validation Results

- Unity MCP was unavailable at the start of this unit, so Unity BatchMode fallback was used.
- Unity BatchMode ran `EscapeFromNightmares.Editor.HUDInventoryButtonSeeder.Seed`.
- First seeding attempt created `HUDInventoryButton.prefab` but could not find inactive `UICanvas/InventoryUI`; seeder was corrected to search inactive canvas children.
- Final BatchMode log contains `Seeded HUD inventory button prefab and Stage1 scene instance.`
- Final BatchMode log exited with return code 0.
- Static prefab validation confirmed:
  - `HUDInventoryButton.prefab` exists.
  - `Button`, `Image`, and `HUDInventoryButton` components are present.
  - `HUDInventoryButton.button` is serialized.
  - no missing script references were found.
- Static scene validation confirmed:
  - `UICanvas/HUDInventoryButton` is present as a prefab instance.
  - the scene instance is active at start.
  - the scene instance overrides `inventoryUI` with `UICanvas/InventoryUI`.

## Review Artifact

- `reports/unity-validation/hud_inventory_button_validation.json`

## Current State

HUD inventory button is complete for this unit. It uses the existing inventory button sprite and opens the existing `InventoryUI` scene instance.

## Remaining Gaps

- Map and settings HUD buttons are not implemented yet.
- MapUI and SettingsUI prefabs are not created yet.
- Room-specific item pickup hotspots are not placed yet.
- Puzzle, monster, hiding, sound, and map gameplay systems are still deferred.

## Resume Instructions

1. Treat `HUDInventoryButton` as completed.
2. Continue with `MapUI.prefab` or another single approved UI surface.

## Next Action

Open the next Harness unit.
