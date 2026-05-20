# Step 0: HUD Settings Button

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `HUDSettingsButton`
- `requires_user_design_approval`: `true`
- Goal: create an in-game HUD settings button that opens the existing `SettingsUI` scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No pause, title, puzzle, hiding, or game-over prefabs.
- No audio mixer integration.
- No room-specific hotspot placement.
- No gameplay progress save.

## Decision Log

- 2026-05-20: Use the existing `ui_button_settings_small.png` sprite for the HUD button.
- 2026-05-20: Keep the button as an always-visible HUD control under `UICanvas`.
- 2026-05-20: Use a small bridge component that calls `SettingsUI.Show()` so the inactive settings panel can be opened.

## Validation Results

- Unity MCP was unavailable at the start of this heartbeat unit, so Unity BatchMode fallback was used.
- Unity BatchMode ran `EscapeFromNightmares.Editor.HUDSettingsButtonSeeder.Seed`.
- BatchMode log contains `Seeded HUD settings button prefab and Stage1 scene instance.`
- BatchMode log exited with return code 0.
- Static prefab validation confirmed:
  - `HUDSettingsButton.prefab` exists.
  - `Button`, `Image`, and `HUDSettingsButton` components are present.
  - `HUDSettingsButton.button` is serialized.
  - no missing script references were found.
- Static scene validation confirmed:
  - `UICanvas/HUDSettingsButton` is present as a prefab instance.
  - the scene instance is active at start.
  - the scene instance overrides `settingsUI` with `UICanvas/SettingsUI`.

## Review Artifact

- `reports/unity-validation/hud_settings_button_validation.json`

## Current State

HUD settings button is complete for this unit. It uses the existing small settings button sprite and opens the existing `SettingsUI` scene instance.

## Remaining Gaps

- Title, pause, puzzle, hiding, and game-over UI prefabs are still not created.
- Audio mixer integration is not implemented yet.
- Room-specific item pickup hotspots are not placed yet.
- Puzzle, monster, hiding, and sound gameplay systems are still deferred.

## Resume Instructions

1. Treat `HUDSettingsButton` as completed.
2. Continue with `TitleUI.prefab` or another single approved UI surface.

## Next Action

Open the next Harness unit.
