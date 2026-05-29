# Remaining Puzzle Runtime Test Report

- Generated At: 2026-05-29 23:12:33
- Active Scene: GameScene
- Total: 4
- Passed: 4
- Failed: 0

## Results

| Test | Puzzle ID | Result | Message | Duration |
|---|---|---|---|---|
| LivingRoom Item Use | Puzzle_LivingRoom_01 | Pass | Completed and rewarded SmallClockworkDevice. | 1.95s |
| Basement Power Device | Puzzle_BasementStorage_01 | Pass | Completed, opened locked room, unlocked BasementClueImage, and transformed items. | 0.21s |
| Locked Room Final | Puzzle_LockedRoom_01 | Pass | Completed, rewarded FrontDoorKey, and marked finalChaseStarted. | 0.31s |
| Entrance Door | Puzzle_Entrance_01 | Pass | Entrance puzzle completed and ending flow was triggered. Completed=True, EndingState=True | 0.20s |

## Expected Rewards / State

| Puzzle ID | Expected Result |
|---|---|
| Puzzle_LivingRoom_01 | SmallClockworkDevice |
| Puzzle_BasementStorage_01 | Door unlock, BasementClueImage, ModifiedClockworkDevice |
| Puzzle_LockedRoom_01 | FrontDoorKey, finalChaseStarted |
| Puzzle_Entrance_01 | GameState.Ending |

## Save Protection

- save_data.json is backed up before test.
- save_data.json is restored after test.
- If no save existed before test, generated save_data.json is deleted.

## Notes

- This test does not validate final UI design.
- This test does not validate final animation or audio.
- This test validates open, initialize, answer/use, completion, reward/state.
