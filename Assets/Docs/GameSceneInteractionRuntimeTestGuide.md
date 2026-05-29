# GameScene Interaction Runtime Test Guide

## Purpose

This test verifies the actual placeholder GameScene interaction surface. Unlike the puzzle runtime tests that call manager APIs directly, this runner finds real Scene `Button` objects and invokes their configured `onClick` listeners.

## Difference From Earlier Runtime Tests

- Earlier tests: call `PuzzleManager.OpenPuzzle`, puzzle input APIs, and reward checks directly.
- This test: clicks `NavigationButton` and `ClickableButton` scene objects through `Button.onClick.Invoke()`.

## Test Targets

- Navigation buttons
- Door buttons
- Puzzle buttons
- ExamineImage buttons
- HidePoint buttons
- FinalDoor button
- Full scene click route from Bedroom to Ending

## Menus

- `Escape From Nightmare / Tests / Prepare GameScene Interaction Runtime Test Runner`
- `Escape From Nightmare / Tests / Run GameScene Interaction Runtime Tests`

## Result File

- `Assets/Docs/GeneratedGameSceneInteractionRuntimeTestReport.md`

## Failure Checks

- Missing `Button` component
- Missing `ClickableButton`
- Wrong `linkedDoorId`
- Wrong `linkedPuzzleId`
- Wrong `linkedClueImageId`
- Location/View parent activation issue
- Missing `PuzzleManager.puzzleUiRoot`
- Missing `ClueImageManager.clueImagePanel`
- Missing `InventoryManager`
- Missing `SaveManager`
