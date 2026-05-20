# Step 0: HUD Map Button

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `HUDMapButton`
- `requires_user_design_approval`: `true`
- Goal: create an in-game HUD map button that opens the existing `MapUI` scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No HUD settings button.
- No MapSystem runtime or marker positioning logic.
- No title, pause, settings, puzzle, hiding, or game-over prefabs.
- No room-specific hotspot placement.

## Decision Log

- 2026-05-20: Use the existing `ui_button_map.png` sprite.
- 2026-05-20: Keep the button as an always-visible HUD control under `UICanvas`.
- 2026-05-20: Use a small bridge component that calls `MapUI.Show()` so the inactive map panel can be opened.

## Validation Results

- Unity MCP was unavailable at the start of this heartbeat unit, so Unity BatchMode fallback was used.
- Unity BatchMode ran `EscapeFromNightmares.Editor.HUDMapButtonSeeder.Seed`.
- BatchMode log contains `Seeded HUD map button prefab and Stage1 scene instance.`
- BatchMode log exited with return code 0.
- Static prefab validation confirmed:
  - `HUDMapButton.prefab` exists.
  - `Button`, `Image`, and `HUDMapButton` components are present.
  - `HUDMapButton.button` is serialized.
  - no missing script references were found.
- Static scene validation confirmed:
  - `UICanvas/HUDMapButton` is present as a prefab instance.
  - the scene instance is active at start.
  - the scene instance overrides `mapUI` with `UICanvas/MapUI`.

## Review Artifact

- `reports/unity-validation/hud_map_button_validation.json`

## Current State

HUD map button is complete for this unit. It uses the existing map button sprite and opens the existing `MapUI` scene instance.

## Remaining Gaps

- SettingsUI prefab and HUD settings button are not implemented yet.
- MapSystem runtime is not implemented yet.
- Room-specific item pickup hotspots are not placed yet.
- Puzzle, monster, hiding, and sound gameplay systems are still deferred.

## Resume Instructions

1. Treat `HUDMapButton` as completed.
2. Continue with `SettingsUI.prefab` or another single approved UI surface.

## Next Action

Open the next Harness unit.
