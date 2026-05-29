# Remaining Puzzle Prefab Builder Usage

## Targets

This builder creates the remaining four source-aligned puzzle UI prefabs:

- `Assets/Resources/PuzzleUI/PuzzleUI_LivingRoomItemUse.prefab`
- `Assets/Resources/PuzzleUI/PuzzleUI_BasementPowerDevice.prefab`
- `Assets/Resources/PuzzleUI/PuzzleUI_LockedRoomFinal.prefab`
- `Assets/Resources/PuzzleUI/PuzzleUI_EntranceDoor.prefab`

## Menus

- `Escape From Nightmare / Puzzle Prefabs / Create Missing Remaining Puzzle Prefabs`
- `Escape From Nightmare / Puzzle Prefabs / Rebuild Remaining Puzzle Prefabs With Backup`

## Create Missing

Creates only prefabs that do not already exist. Existing prefabs are skipped and are not overwritten.

## Rebuild

Backs up existing prefabs, then rebuilds them.

Backup location:

`Assets/Backups/PuzzleUI`

Backups are outside `Resources`, so they are not loaded by `Resources.Load`.

## Recommended Order

1. Run `Escape From Nightmare / Validate Game Data`.
2. Run `Escape From Nightmare / Puzzle Prefabs / Create Missing Remaining Puzzle Prefabs`.
3. Run `Escape From Nightmare / Validate Puzzle Prefab Contracts`.
4. Run `Escape From Nightmare / Tests / Run Remaining Puzzle Runtime Tests`.
5. Check `Assets/Docs/GeneratedRemainingPuzzleRuntimeTestReport.md`.

## Notes

- These prefabs are initial test UI, not final art.
- They use `UnityEngine.UI.Text`, `Button`, and `Image`.
- They do not create sprites, audio, scenes, or permanent scene objects.
