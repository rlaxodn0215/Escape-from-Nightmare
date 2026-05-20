# Step 0: Title UI Prefab

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `TitleUI.prefab`
- `requires_user_design_approval`: `true`
- Goal: create a reusable title UI prefab with Start, Settings, and Quit buttons, placed under `UICanvas` and connected to existing scene systems.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No separate Boot or Title scene.
- No pause, puzzle, hiding, or game-over prefabs.
- No full title flow validation or build.
- No new story text.

## Decision Log

- 2026-05-20: Use existing `ui_button_start`, `ui_button_settings`, and `ui_button_quit` sprites.
- 2026-05-20: Start calls `GameStateManager.StartStage1Run` and hides the title overlay; full scene-flow wiring remains a later focused unit.
- 2026-05-20: Settings opens the existing `SettingsUI` scene instance.
- 2026-05-20: Quit calls `SceneFlowController.QuitGame`.

## Validation Results

- Initial Unity BatchMode attempt exited with return code 1 because an editor instance held the project lock before the seeder ran.
- Unity MCP was available afterward.
- Unity MCP `validate_script` passed with 0 errors and 0 warnings for:
  - `TitleUI.cs`
  - `TitleUIPrefabSeeder.cs`
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Title UI Prefab`.
- Unity console log contains `Seeded TitleUI prefab and Stage1 scene instance.`
- Static prefab validation confirmed:
  - `TitleUI.prefab` exists.
  - `TitleUI`, `CanvasGroup`, Start, Settings, Quit buttons, and title label are serialized.
  - no missing script references were found.
- Static scene validation confirmed:
  - `UICanvas/TitleUI` is present as a prefab instance.
  - the scene instance is active at start.
  - the scene instance overrides `gameStateManager`, `sceneFlowController`, and `settingsUI`.

## Review Artifact

- `reports/unity-validation/title_ui_prefab_validation.json`

## Current State

Title UI prefab is complete for this unit. It contains Start, Settings, and Quit buttons and opens the existing SettingsUI through the Settings button. Start moves the in-memory game state to Stage 1 playing and hides the title overlay.

## Remaining Gaps

- Full title scene or boot/title scene flow is not implemented yet.
- Pause, puzzle, hiding, and game-over UI prefabs are still not created.
- Room-specific item pickup hotspots are not placed yet.
- Puzzle, monster, hiding, and sound gameplay systems are still deferred.

## Resume Instructions

1. Treat `TitleUI.prefab` as completed.
2. Continue with `PauseUI.prefab` or another single approved UI surface.

## Next Action

Open the next Harness unit.
