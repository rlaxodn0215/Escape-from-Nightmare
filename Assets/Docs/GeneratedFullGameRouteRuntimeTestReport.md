# Full Game Route Runtime Test Report

- Generated At: 2026-05-29 23:13:21
- Active Scene: GameScene
- Total Steps: 13
- Passed: 13
- Failed: 0

## Route Summary

Bedroom -> LivingRoom ItemUse -> ChildRoom -> Study -> LivingRoom Symbol -> Kitchen -> BasementStorage -> LockedRoom -> Entrance -> Ending

## Step Results

| Step | Target ID | Result | Message | Duration |
|---:|---|---|---|---:|
| 1. Gate: Basement Power Requires Items | Puzzle_BasementStorage_01 | Pass | Basement power did not complete without required items. | 1.96s |
| 2. Gate: Locked Room Requires ModifiedClockworkDevice | Puzzle_LockedRoom_01 | Pass | LockedRoomFinal did not complete without ModifiedClockworkDevice. | 0.21s |
| 3. Gate: Entrance Requires FrontDoorKey | Puzzle_Entrance_01 | Pass | Entrance did not enter Ending without FrontDoorKey. | 0.21s |
| 4. Initial State | NewGame | Pass | New game state is clean. | 0.00s |
| 5. Bedroom Code | Puzzle_Bedroom_01 | Pass | OldDrawerKey acquired. | 0.22s |
| 6. LivingRoom ItemUse | Puzzle_LivingRoom_01 | Pass | SmallClockworkDevice acquired. | 0.21s |
| 7. ChildRoom Card Order | Puzzle_ChildRoom_01 | Pass | ChildRoomCardSymbolClueImage unlocked. | 0.22s |
| 8. Study Book Order | Puzzle_Study_01 | Pass | StudyBookSymbolClueImage unlocked. | 0.21s |
| 9. LivingRoom Symbol Sequence | Puzzle_LivingRoom_02 | Pass | KitchenCodeClueImage unlocked. | 0.21s |
| 10. Kitchen Code | Puzzle_Kitchen_01 | Pass | BasementFuse acquired and FrontDoorKey not granted. | 0.20s |
| 11. Basement Power Device | Puzzle_BasementStorage_01 | Pass | Basement door opened, clue unlocked, ModifiedClockworkDevice acquired. | 0.22s |
| 12. LockedRoom Final | Puzzle_LockedRoom_01 | Pass | FrontDoorKey acquired and finalChaseStarted is true. | 0.32s |
| 13. Entrance Ending | Puzzle_Entrance_01 | Pass | Ending state reached. | 0.23s |

## Expected Progression State

| Stage | Expected State |
|---|---|
| Bedroom | OldDrawerKey |
| LivingRoom ItemUse | SmallClockworkDevice |
| ChildRoom | ChildRoomCardSymbolClueImage |
| Study | StudyBookSymbolClueImage |
| LivingRoom Symbol | KitchenCodeClueImage |
| Kitchen | BasementFuse |
| BasementStorage | Door_BasementStorage_LockedRoom, BasementClueImage, ModifiedClockworkDevice |
| LockedRoom | FrontDoorKey, finalChaseStarted |
| Entrance | GameState.Ending |

## Negative Gate Checks

| Gate | Result | Message |
|---|---|---|
| Gate: Basement Power Requires Items | Pass | Basement power did not complete without required items. |
| Gate: Locked Room Requires ModifiedClockworkDevice | Pass | LockedRoomFinal did not complete without ModifiedClockworkDevice. |
| Gate: Entrance Requires FrontDoorKey | Pass | Entrance did not enter Ending without FrontDoorKey. |

## Save Protection

- save_data.json is backed up before test.
- save_data.json is restored after test.
- If no save existed before test, generated save_data.json is deleted.

## Notes

- This test does not validate final art.
- This test does not validate manual Scene button click positions.
- This test validates progression logic, rewards, unlocks, and ending state.
