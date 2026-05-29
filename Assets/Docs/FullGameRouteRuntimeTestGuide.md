# Full Game Route Runtime Test Guide

## Purpose

This test validates the full source-aligned route across all nine puzzles, from the first bedroom code through the entrance ending.

It checks progression logic, reward state, clue unlocks, door unlocks, final chase state, and ending state. It does not validate final art, audio, animations, or manual button positions.

## Prerequisites

- Validate Game Data: Error 0
- Validate Puzzle Prefab Contracts: Error 0
- First Five Runtime Tests: Passed 5, Failed 0
- Remaining Runtime Tests: Passed 4, Failed 0
- All nine Puzzle UI prefabs load from `Resources/PuzzleUI`

## Menus

- `Escape From Nightmare / Tests / Prepare Full Game Route Runtime Test Runner`
- `Escape From Nightmare / Tests / Run Full Game Route Runtime Test`

The menu creates a temporary `FullGameRouteRuntimeTestRunner` object in the active scene and does not save the scene.

## Route

1. `Puzzle_Bedroom_01`
2. `Puzzle_LivingRoom_01`
3. `Puzzle_ChildRoom_01`
4. `Puzzle_Study_01`
5. `Puzzle_LivingRoom_02`
6. `Puzzle_Kitchen_01`
7. `Puzzle_BasementStorage_01`
8. `Puzzle_LockedRoom_01`
9. `Puzzle_Entrance_01`

## Negative Gate Checks

- Basement power device must not complete without `BasementFuse` and `SmallClockworkDevice`.
- Locked room final must not complete without `ModifiedClockworkDevice`.
- Entrance door must not enter ending without `FrontDoorKey`.

## Result File

- `Assets/Docs/GeneratedFullGameRouteRuntimeTestReport.md`

## Failure Checklist

- `rewardType` / `rewardId`
- `requiredItemId`
- `puzzle_answers.json`
- `PuzzleManager.CurrentPuzzleInstance`
- `InventoryManager` state
- `SaveManager` state
- `ClueImageManager` state
- `EndingManager` auto return setting
