# First Five Puzzle Runtime Test Guide

## Purpose

This runner verifies the first five source-aligned puzzle flows without manually clicking UI buttons.

It checks:
- Puzzle open
- UI instantiate
- Initialize
- Answer submit
- CompletePuzzle
- Reward item or clue state
- Save state

## Test Targets

- `Puzzle_Bedroom_01`
- `Puzzle_Kitchen_01`
- `Puzzle_ChildRoom_01`
- `Puzzle_Study_01`
- `Puzzle_LivingRoom_02`

## Preconditions

- `Escape From Nightmare / Validate Game Data` has 0 errors.
- `Escape From Nightmare / Validate Puzzle Prefab Contracts` has 0 errors.
- The first five Puzzle UI Prefabs exist under `Assets/Resources/PuzzleUI`.
- `PuzzleManager` can load prefabs through `Resources.Load`.
- `PuzzleManager.puzzleUiRoot` is connected for normal scene play.

The runtime runner can create temporary Play Mode-only manager objects when the scene has not been fully wired yet. It does not save those objects into the scene.

## Editor Menus

- `Escape From Nightmare / Tests / Prepare First Five Puzzle Runtime Test Runner`
- `Escape From Nightmare / Tests / Run First Five Puzzle Runtime Tests`

The Prepare menu creates or selects a `FirstFivePuzzleRuntimeTestRunner` object in the active scene and leaves `runOnStart` disabled for normal manual Play Mode. The scene is not saved automatically.

The Run menu prepares the runner, requests a one-time runtime test launch for the current editor session, then enters Play Mode.

## Result File

Runtime results are written to:

`Assets/Docs/GeneratedFirstFivePuzzleRuntimeTestReport.md`

## What This Test Covers

- Opening each puzzle by `puzzleId`
- Instantiating the target Puzzle UI Prefab
- Calling the puzzle UI initialization path
- Entering the correct answer through public puzzle methods
- Completing the puzzle
- Verifying expected reward state
- Backing up and restoring `save_data.json`

## What This Test Does Not Cover

- Final visual design
- Manual button layout
- Actual player click feel
- Sprite rendering quality
- Sound
- Animation
- Full room navigation

## Failure Checklist

If a test fails, check:

- `PuzzleManager.puzzleUiRoot`
- `PuzzleRecord.prefabPath`
- `PuzzleRecord.answerVariableName`
- `puzzle_answers.json`
- `rewardType` / `rewardId`
- `InventoryManager`
- `SaveManager`
- `ClueImageManager`
- The generated Console errors from `FirstFivePuzzleRuntimeTestRunner`
