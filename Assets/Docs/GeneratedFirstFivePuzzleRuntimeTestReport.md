# First Five Puzzle Runtime Test Report

- Generated At: 2026-05-29 23:10:49
- Active Scene: GameScene
- Total: 5
- Passed: 5
- Failed: 0

## Results

| Test | Puzzle ID | Result | Message | Duration |
|---|---|---|---|---|
| Bedroom Number Code | Puzzle_Bedroom_01 | Pass | Completed and rewarded OldDrawerKey. | 1.94s |
| Kitchen Number Code | Puzzle_Kitchen_01 | Pass | Completed and rewarded BasementFuse without granting FrontDoorKey. | 0.21s |
| ChildRoom Sequence | Puzzle_ChildRoom_01 | Pass | Completed and unlocked ChildRoomCardSymbolClueImage. | 0.21s |
| Study Sequence | Puzzle_Study_01 | Pass | Completed and unlocked StudyBookSymbolClueImage. | 0.21s |
| LivingRoom Symbol Cycle | Puzzle_LivingRoom_02 | Pass | Completed and unlocked KitchenCodeClueImage. | 0.21s |

## Expected Rewards

| Puzzle ID | Expected Result |
|---|---|
| Puzzle_Bedroom_01 | OldDrawerKey |
| Puzzle_Kitchen_01 | BasementFuse |
| Puzzle_ChildRoom_01 | ChildRoomCardSymbolClueImage |
| Puzzle_Study_01 | StudyBookSymbolClueImage |
| Puzzle_LivingRoom_02 | KitchenCodeClueImage |

## Notes

- This test backs up and restores save_data.json.
- This test does not validate final visual design.
- This test does not test manual button layout.
- This test validates open, initialize, answer submit, completion, and reward state.
