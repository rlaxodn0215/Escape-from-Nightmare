# GameScene Interaction Runtime Test Report

- Generated At: 2026-05-29 23:21:06
- Active Scene: GameScene
- Total: 39
- Passed: 39
- Failed: 0

## Summary

| Category | Passed | Failed |
|---|---:|---:|
| Navigation | 1 | 0 |
| Door Buttons | 16 | 0 |
| Puzzle Buttons | 9 | 0 |
| ExamineImage Buttons | 8 | 0 |
| HidePoint Buttons | 3 | 0 |
| FinalDoor | 1 | 0 |
| Full Scene Click Route | 1 | 0 |

## Results

| # | Category | Target ID | Result | Message | Duration |
|---:|---|---|---|---|---:|
| 1 | Navigation | RotateLeft/RotateRight | Pass | RotateRight moved to Bedroom_Right and RotateLeft returned to Bedroom_Front. | 1.93s |
| 2 | Door Buttons | Door_Bedroom_SecondFloorHallway | Pass | Door click moved to SecondFloorHallway/SecondFloorHallway_Back. | 0.11s |
| 3 | Door Buttons | Door_SecondFloorHallway_Bedroom | Pass | Door click moved to Bedroom/Bedroom_Front. | 0.10s |
| 4 | Door Buttons | Door_SecondFloorHallway_ChildRoom | Pass | Door click moved to ChildRoom/ChildRoom_Front. | 0.11s |
| 5 | Door Buttons | Door_ChildRoom_SecondFloorHallway | Pass | Door click moved to SecondFloorHallway/SecondFloorHallway_Back. | 0.10s |
| 6 | Door Buttons | Door_SecondFloorHallway_Study | Pass | Door click moved to Study/Study_Front. | 0.11s |
| 7 | Door Buttons | Door_Study_SecondFloorHallway | Pass | Door click moved to SecondFloorHallway/SecondFloorHallway_Front. | 0.10s |
| 8 | Door Buttons | Door_SecondFloorHallway_LivingRoom | Pass | Door click moved to LivingRoom/LivingRoom_Back. | 0.11s |
| 9 | Door Buttons | Door_LivingRoom_SecondFloorHallway | Pass | Door click moved to SecondFloorHallway/SecondFloorHallway_Front. | 0.11s |
| 10 | Door Buttons | Door_LivingRoom_Kitchen | Pass | Door click moved to Kitchen/Kitchen_Front. | 0.10s |
| 11 | Door Buttons | Door_Kitchen_LivingRoom | Pass | Door click moved to LivingRoom/LivingRoom_Back. | 0.10s |
| 12 | Door Buttons | Door_Kitchen_BasementStorage | Pass | Door click moved to BasementStorage/BasementStorage_Front. | 0.10s |
| 13 | Door Buttons | Door_BasementStorage_Kitchen | Pass | Door click moved to Kitchen/Kitchen_Front. | 0.10s |
| 14 | Door Buttons | Door_BasementStorage_LockedRoom | Pass | Door click moved to LockedRoom/LockedRoom_Front. | 0.10s |
| 15 | Door Buttons | Door_LockedRoom_BasementStorage | Pass | Door click moved to BasementStorage/BasementStorage_Right. | 0.11s |
| 16 | Door Buttons | Door_LivingRoom_Entrance | Pass | Door click moved to Entrance/Entrance_Front. | 0.10s |
| 17 | Door Buttons | Door_Entrance_LivingRoom | Pass | Door click moved to LivingRoom/LivingRoom_Front. | 0.10s |
| 18 | Puzzle Buttons | Puzzle_Bedroom_01 | Pass | Scene button opened puzzle UI: PuzzleUI_BedroomNumberCode. | 0.20s |
| 19 | Puzzle Buttons | Puzzle_LivingRoom_01 | Pass | Scene button opened puzzle UI: PuzzleUI_LivingRoomItemUse. | 0.20s |
| 20 | Puzzle Buttons | Puzzle_ChildRoom_01 | Pass | Scene button opened puzzle UI: PuzzleUI_ChildRoomCardOrder. | 0.21s |
| 21 | Puzzle Buttons | Puzzle_Study_01 | Pass | Scene button opened puzzle UI: PuzzleUI_StudyBookOrder. | 0.20s |
| 22 | Puzzle Buttons | Puzzle_LivingRoom_02 | Pass | Scene button opened puzzle UI: PuzzleUI_LivingRoomSymbolSequence. | 0.21s |
| 23 | Puzzle Buttons | Puzzle_Kitchen_01 | Pass | Scene button opened puzzle UI: PuzzleUI_KitchenNumberCode. | 0.20s |
| 24 | Puzzle Buttons | Puzzle_BasementStorage_01 | Pass | Scene button opened puzzle UI: PuzzleUI_BasementPowerDevice. | 0.21s |
| 25 | Puzzle Buttons | Puzzle_LockedRoom_01 | Pass | Scene button opened puzzle UI: PuzzleUI_LockedRoomFinal. | 0.21s |
| 26 | Puzzle Buttons | Puzzle_Entrance_01 | Pass | Scene button opened puzzle UI: PuzzleUI_EntranceDoor. | 0.21s |
| 27 | ExamineImage Buttons | BedroomPhotoCodeClue | Pass | Clue panel opened from scene button. | 0.11s |
| 28 | ExamineImage Buttons | LivingRoomEntranceCodeClue | Pass | Clue panel opened from scene button. | 0.10s |
| 29 | ExamineImage Buttons | ChildRoomCardSymbolClueImage | Pass | Clue panel opened from scene button. | 0.10s |
| 30 | ExamineImage Buttons | StudyBookSymbolClueImage | Pass | Clue panel opened from scene button. | 0.10s |
| 31 | ExamineImage Buttons | KitchenCodeClueImage | Pass | Clue panel opened from scene button. | 0.10s |
| 32 | ExamineImage Buttons | KitchenFridgeSurfaceSymbolClue | Pass | Clue panel opened from scene button. | 0.10s |
| 33 | ExamineImage Buttons | BasementPowerPatternClue | Pass | Clue panel opened from scene button. | 0.11s |
| 34 | ExamineImage Buttons | BasementClueImage | Pass | Clue panel opened from scene button. | 0.11s |
| 35 | HidePoint Buttons | HidePoint_Study_Desk | Pass | HidePoint entered and ForceExitHidePoint restored state. | 0.10s |
| 36 | HidePoint Buttons | HidePoint_SecondFloorHallway_Cabinet | Pass | HidePoint entered and ForceExitHidePoint restored state. | 0.10s |
| 37 | HidePoint Buttons | HidePoint_Bedroom_Closet | Pass | HidePoint entered and ForceExitHidePoint restored state. | 0.10s |
| 38 | FinalDoor | FinalDoor_FrontDoorKey | Pass | FinalDoor reached Ending through scene button flow. | 0.33s |
| 39 | Full Scene Click Route | BedroomToEnding | Pass | Scene buttons completed the full route and reached Ending. | 6.08s |

## Failed Interactions

- None

## Notes

- This test invokes actual Scene Button.onClick listeners.
- This test verifies ClickableButton / NavigationButton wiring.
- This test does not validate final visual design.
- This test does not require final Sprite or Audio assets.
