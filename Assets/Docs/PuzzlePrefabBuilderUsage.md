# Puzzle Prefab Builder Usage

## Purpose

The Puzzle Prefab Builder creates the first five Puzzle UI Prefabs for fast manual testing.

This is not runtime auto-generation. The Builder only runs when a developer selects the Unity Editor menu item.

## Generated Prefabs

- `PuzzleUI_BedroomNumberCode`
- `PuzzleUI_KitchenNumberCode`
- `PuzzleUI_ChildRoomCardOrder`
- `PuzzleUI_StudyBookOrder`
- `PuzzleUI_LivingRoomSymbolSequence`

## Menus

- `Escape From Nightmare / Puzzle Prefabs / Create Missing First Five Puzzle Prefabs`
- `Escape From Nightmare / Puzzle Prefabs / Rebuild First Five Puzzle Prefabs With Backup`

## Recommended Workflow

1. Run `Escape From Nightmare / Validate Game Data`.
2. Run `Escape From Nightmare / Puzzle Prefabs / Create Missing First Five Puzzle Prefabs`.
3. Run `Escape From Nightmare / Validate Puzzle Prefab Contracts`.
4. In GameScene, connect `PuzzleManager.puzzleUiRoot`.
5. Add Puzzle buttons and set `linkedPuzzleId`.
6. Enter Play Mode and open each puzzle.

## Rebuild Warning

The rebuild menu overwrites existing first-five Prefabs.

Before overwriting, it creates backups in:

- `Assets/Backups/PuzzleUI`

Use rebuild carefully if you have manually customized a Prefab.

## Current Limits

- Generated UI is temporary development UI.
- No real Sprites are created.
- No Audio is created.
- No animations are created.
- No mobile or gamepad optimization is included.
- Text uses `UnityEngine.UI.Text`, not TextMeshPro.
