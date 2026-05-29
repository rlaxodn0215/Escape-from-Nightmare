# Remaining Puzzle Runtime Test Guide

## Purpose

This runner verifies the remaining four puzzle flows after the first five puzzle runtime tests pass.

## Test Targets

- `Puzzle_LivingRoom_01`
- `Puzzle_BasementStorage_01`
- `Puzzle_LockedRoom_01`
- `Puzzle_Entrance_01`

## Automated Inputs

- `Puzzle_LivingRoom_01`: add/select `OldDrawerKey`, then call `UseSelectedItem`.
- `Puzzle_BasementStorage_01`: add `BasementFuse` and `SmallClockworkDevice`, then input `Switch_Left`, `Switch_Right`, `Switch_Center`, `Switch_Left`, `Switch_Right`.
- `Puzzle_LockedRoom_01`: add `ModifiedClockworkDevice`, set five symbol slots to `Symbol_01`, `Symbol_03`, `Symbol_04`, `Symbol_05`, `Symbol_06`, submit, select/use the device.
- `Puzzle_Entrance_01`: add/select `FrontDoorKey`, then call `UseSelectedItem`.

## Expected Rewards / State

- `Puzzle_LivingRoom_01`: `SmallClockworkDevice`
- `Puzzle_BasementStorage_01`: `Door_BasementStorage_LockedRoom`, `BasementClueImage`, `ModifiedClockworkDevice`
- `Puzzle_LockedRoom_01`: `FrontDoorKey`, `finalChaseStarted`
- `Puzzle_Entrance_01`: `GameState.Ending`

## Save Protection

The runner backs up `save_data.json` before testing and restores it after testing. If no save existed before the test, the generated test save is deleted.

## Failure Checklist

Check:

- `prefabPath`
- `requiredItemId`
- `rewardType` / `rewardId`
- `InventoryManager`
- `SaveManager`
- `ClueImageManager`
- `Door_BasementStorage_LockedRoom`
- `BasementClueImage`
- `ModifiedClockworkDevice`
- `FrontDoorKey`
- `GameManager.EnterEnding`
