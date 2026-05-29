# Source Route Scene Wiring Validation Guide

## Purpose

This validator checks whether the active `GameScene` is manually wired for the source-aligned full route.

The runtime route test can pass through direct manager calls even when scene buttons are missing. This validator is for actual manual play readiness.

## Menu

- `Escape From Nightmare / Validate Source Route Scene Wiring`

## Checked Areas

- Managers
- `PuzzleManager.puzzleUiRoot`
- `LocationManager.locationRoot`
- `InventoryBarUI`
- `ClueImagePanelUI`
- `GameOverPanelUI`
- `EndingPanelUI`
- `HideExitButton`
- `GhostStatusUI`
- `LocationController`
- `LocationView`
- Door buttons
- Puzzle buttons
- ExamineImage buttons
- UseItemTarget buttons
- FinalDoor / Entrance wiring

## Result File

- `Assets/Docs/GeneratedSourceRouteSceneWiringReport.md`

## Error And Warning Rules

- Required manager missing: Error, except `GameManager` and `SaveManager` may be Warning because they can come from `TitleScene`.
- Required `LocationController` missing: Error.
- Required `LocationView` missing: Error.
- Required Door button missing: Error.
- Required Puzzle button missing: Error.
- Entrance passes if either `Puzzle_Entrance_01` button exists or a `FinalDoor` button requires `FrontDoorKey`.
- Recommended ExamineImage clue button missing: Warning.
- `GhostStatusUI` missing: Warning.

## Usage

Run the validator, then use the Missing Wiring section of the generated report to manually wire the scene. Do not auto-create scene buttons from the report.
