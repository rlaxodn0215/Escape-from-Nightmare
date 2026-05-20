# Step 0: Game Over UI Prefab

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `GameOverUI.prefab`
- `requires_user_design_approval`: `true`
- Goal: create a reusable game-over UI prefab with a game-over display, restart button, and return-to-title button, placed under `UICanvas` as an inactive scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No monster capture trigger implementation.
- No rewind animation timeline or audio event wiring.
- No full title scene or boot/title scene flow.
- No puzzle or hiding gauge prefabs.
- No gameplay progress save.

## Decision Log

- 2026-05-20: Use existing `ui_gameover_text`, `ui_button_continue`, and `ui_button_return_title` sprites.
- 2026-05-20: Restart uses `GameStateManager.StartStage1Run()` to reset runtime room state to `child_room` without saving progress.
- 2026-05-20: Return to Title shows the existing `TitleUI` overlay and keeps the flow in the current Stage1 scene for now.

## Validation Results

- Unity MCP was used. Initial menu execution failed because Unity had not imported the newly added scripts yet.
- Ran Unity MCP `refresh_unity` with forced all-assets refresh and script compilation, then executed `Escape From Nightmares/Seed Game Over UI Prefab`.
- Unity console confirmed `Seeded GameOverUI prefab and Stage1 scene instance.`
- Static prefab checks confirmed `GameOverUI.prefab` exists with `RestartButton`, `ReturnTitleButton`, `GameOverText`, and `HintLabel` assigned.
- Static scene checks confirmed `Stage1.unity` contains `UICanvas/GameOverUI` as an inactive prefab instance.
- Static scene reference checks confirmed the scene instance references `GameStateManager` and `TitleUI`.
- Static missing-script scan found no missing script markers in `GameOverUI.prefab` or `Stage1.unity`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/UI/GameOverUI.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/GameOverUIPrefabSeeder.cs`
- `EscapeFromNightmares/Assets/Prefabs/UI/GameOverUI.prefab`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `reports/unity-validation/game_over_ui_prefab_validation.json`

## Current State

Game-over UI prefab is complete for this unit. It is placed under `UICanvas` as an inactive scene instance and exposes restart and return-to-title behavior through existing runtime UI references.

## Resume Instructions

1. Treat this phase as completed.
2. Read `phases/index.json` before starting any further work.
3. If there is no `in_progress` or `blocked` phase, read `NEXT_ACTION.md` and start exactly one next Harness unit.

## Next Action

Start the next Harness unit from `NEXT_ACTION.md` only after confirming `phases/index.json` has no active phase.
