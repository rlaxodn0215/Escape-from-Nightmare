# Step 0: Settings UI Prefab

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `SettingsUI.prefab`
- `requires_user_design_approval`: `true`
- Goal: create a reusable settings UI prefab with BGM and SFX sliders connected to `SaveManager`, placed under `UICanvas` as an inactive scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No HUD settings button.
- No title, pause, map, puzzle, hiding, or game-over prefabs.
- No audio mixer integration.
- No progress save, checkpoint, or autosave.

## Decision Log

- 2026-05-20: Use UGUI sliders for BGM and SFX volume as required by the UI guide.
- 2026-05-20: Connect the scene instance to `Systems.SaveManager`.
- 2026-05-20: Save only BGM/SFX settings through `SaveManager`; do not persist gameplay progress.

## Validation Results

- Unity MCP was unavailable at the start of this heartbeat unit, so Unity BatchMode fallback was used.
- Unity BatchMode ran `EscapeFromNightmares.Editor.SettingsUIPrefabSeeder.Seed`.
- BatchMode log contains `Seeded SettingsUI prefab and Stage1 scene instance.`
- BatchMode log exited with return code 0.
- Static prefab validation confirmed:
  - `SettingsUI.prefab` exists.
  - `SettingsUI`, `CanvasGroup`, close button, BGM slider, SFX slider, and value labels are serialized.
  - no missing script references were found.
- Static scene validation confirmed:
  - `UICanvas/SettingsUI` is present as a prefab instance.
  - the scene instance is inactive at start.
  - the scene instance overrides `saveManager` with `Systems.SaveManager`.
- Save scope validation:
  - BGM/SFX settings save through `SaveManager`.
  - no progress save, checkpoint, or autosave was added.

## Review Artifact

- `reports/unity-validation/settings_ui_prefab_validation.json`

## Current State

Settings UI prefab is complete for this unit. It contains BGM and SFX sliders, value labels, a close button, and a `SaveManager` connection in `Stage1.unity`.

## Remaining Gaps

- HUD settings button is not implemented yet.
- Audio mixer integration is not implemented yet.
- Title, pause, puzzle, hiding, and game-over UI prefabs are still not created.

## Resume Instructions

1. Treat `SettingsUI.prefab` as completed.
2. Continue with `HUDSettingsButton` or another single approved UI surface.

## Next Action

Open the next Harness unit.
