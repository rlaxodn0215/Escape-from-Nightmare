# GameScene Interaction Runtime Test Report

- Generated At: 2026-06-02 20:16:56
- Active Scene: GameScene
- Total: 34
- Passed: 7
- Failed: 27

## Summary

| Category | Passed | Failed |
|---|---:|---:|
| Navigation | 0 | 1 |
| Door Buttons | 0 | 20 |
| Puzzle Buttons | 0 | 0 |
| ExamineImage Buttons | 7 | 1 |
| HidePoint Buttons | 0 | 3 |
| FinalDoor | 0 | 1 |
| Full Scene Click Route | 0 | 1 |

## Results

| # | Category | Target ID | Result | Message | Duration |
|---:|---|---|---|---|---:|
| 1 | Navigation | RotateLeft/RotateRight | Fail | Unexpected views. After right: Bedroom_Back, after left: Bedroom_Back | 14.98s |
| 2 | Door Buttons | Door_Bedroom_SecondFloorHallway | Fail | Expected SecondFloorHallway/SecondFloorHallway_Front but got Bedroom/Bedroom_Front. | 0.11s |
| 3 | Door Buttons | Door_SecondFloorHallway_Bedroom | Fail | Expected Bedroom/Bedroom_Front but got SecondFloorHallway/SecondFloorHallway_Front. | 0.11s |
| 4 | Door Buttons | Door_SecondFloorHallway_ChildRoom | Fail | Expected ChildRoom/ChildRoom_Front but got SecondFloorHallway/SecondFloorHallway_Front. | 0.11s |
| 5 | Door Buttons | Door_ChildRoom_SecondFloorHallway | Fail | Expected SecondFloorHallway/SecondFloorHallway_Front but got ChildRoom/ChildRoom_Front. | 0.12s |
| 6 | Door Buttons | Door_SecondFloorHallway_Study | Fail | Expected Study/Study_Front but got SecondFloorHallway/SecondFloorHallway_Front. | 0.12s |
| 7 | Door Buttons | Door_Study_SecondFloorHallway | Fail | Expected SecondFloorHallway/SecondFloorHallway_Front but got Study/Study_Front. | 0.10s |
| 8 | Door Buttons | Door_SecondFloorHallway_FirstFloorHall | Fail | Expected FirstFloorHall/FirstFloorHall_Front but got SecondFloorHallway/SecondFloorHallway_Front. | 0.11s |
| 9 | Door Buttons | Door_FirstFloorHall_SecondFloorHallway | Fail | Expected SecondFloorHallway/SecondFloorHallway_Front but got FirstFloorHall/FirstFloorHall_Back. | 0.10s |
| 10 | Door Buttons | Door_Entrance_FirstFloorHall | Fail | Expected FirstFloorHall/FirstFloorHall_Front but got Entrance/Entrance_Front. | 0.11s |
| 11 | Door Buttons | Door_FirstFloorHall_Entrance | Fail | Expected Entrance/Entrance_Front but got FirstFloorHall/FirstFloorHall_Front. | 0.10s |
| 12 | Door Buttons | Door_SmallLivingRoom_FirstFloorHall | Fail | Expected FirstFloorHall/FirstFloorHall_Front but got SmallLivingRoom/SmallLivingRoom_Front. | 0.13s |
| 13 | Door Buttons | Door_FirstFloorHall_SmallLivingRoom | Fail | Expected SmallLivingRoom/SmallLivingRoom_Front but got FirstFloorHall/FirstFloorHall_Front. | 0.10s |
| 14 | Door Buttons | Door_LivingRoom_FirstFloorHall | Fail | Expected FirstFloorHall/FirstFloorHall_Front but got LivingRoom/LivingRoom_Front. | 0.10s |
| 15 | Door Buttons | Door_FirstFloorHall_LivingRoom | Fail | Expected LivingRoom/LivingRoom_Front but got FirstFloorHall/FirstFloorHall_Front. | 0.10s |
| 16 | Door Buttons | Door_Kitchen_FirstFloorHall | Fail | Expected FirstFloorHall/FirstFloorHall_Back but got Kitchen/Kitchen_Front. | 0.10s |
| 17 | Door Buttons | Door_FirstFloorHall_Kitchen | Fail | Expected Kitchen/Kitchen_Front but got FirstFloorHall/FirstFloorHall_Back. | 0.10s |
| 18 | Door Buttons | Door_Kitchen_BasementStairs | Fail | Expected BasementStairs/BasementStairs_Front but got Kitchen/Kitchen_Back. | 0.11s |
| 19 | Door Buttons | Door_BasementStairs_Kitchen | Fail | Expected Kitchen/Kitchen_Back but got BasementStairs/BasementStairs_Front. | 0.10s |
| 20 | Door Buttons | Door_BasementStairs_BasementStorage | Fail | Expected BasementStorage/BasementStorage_Front but got BasementStairs/BasementStairs_Back. | 0.10s |
| 21 | Door Buttons | Door_BasementStorage_BasementStairs | Fail | Expected BasementStairs/BasementStairs_Back but got BasementStorage/BasementStorage_Front. | 0.10s |
| 22 | ExamineImage Buttons | BedroomPhotoCodeClue | Fail | Clue panel did not become visible. | 0.11s |
| 23 | ExamineImage Buttons | LivingRoomEntranceCodeClue | Pass | Clue panel opened from scene button. | 0.11s |
| 24 | ExamineImage Buttons | ChildRoomCardSymbolClueImage | Pass | Clue panel opened from scene button. | 0.10s |
| 25 | ExamineImage Buttons | StudyBookSymbolClueImage | Pass | Clue panel opened from scene button. | 0.11s |
| 26 | ExamineImage Buttons | KitchenCodeClueImage | Pass | Clue panel opened from scene button. | 0.11s |
| 27 | ExamineImage Buttons | KitchenFridgeSurfaceSymbolClue | Pass | Clue panel opened from scene button. | 0.11s |
| 28 | ExamineImage Buttons | BasementPowerPatternClue | Pass | Clue panel opened from scene button. | 0.12s |
| 29 | ExamineImage Buttons | BasementClueImage | Pass | Clue panel opened from scene button. | 0.10s |
| 30 | HidePoint Buttons | HidePoint_Bedroom_CurtainCloset | Fail | HidePoint did not enter/exit correctly. Entered=False, Exited=True | 0.11s |
| 31 | HidePoint Buttons | HidePoint_Bedroom_Closet | Fail | HidePoint did not enter/exit correctly. Entered=False, Exited=True | 0.10s |
| 32 | HidePoint Buttons | HidePointSummary | Fail | No HidePoint interaction passed. | 0.00s |
| 33 | FinalDoor | FinalDoor_FrontDoorKey | Fail | FinalDoor ClickableButton was not found. | 0.00s |
| 34 | Full Scene Click Route | BedroomToBasement | Fail | Layout route did not satisfy every expected room transition. | 3.54s |

## Failed Interactions

- Navigation / RotateLeft/RotateRight: Unexpected views. After right: Bedroom_Back, after left: Bedroom_Back
- Door Buttons / Door_Bedroom_SecondFloorHallway: Expected SecondFloorHallway/SecondFloorHallway_Front but got Bedroom/Bedroom_Front.
- Door Buttons / Door_SecondFloorHallway_Bedroom: Expected Bedroom/Bedroom_Front but got SecondFloorHallway/SecondFloorHallway_Front.
- Door Buttons / Door_SecondFloorHallway_ChildRoom: Expected ChildRoom/ChildRoom_Front but got SecondFloorHallway/SecondFloorHallway_Front.
- Door Buttons / Door_ChildRoom_SecondFloorHallway: Expected SecondFloorHallway/SecondFloorHallway_Front but got ChildRoom/ChildRoom_Front.
- Door Buttons / Door_SecondFloorHallway_Study: Expected Study/Study_Front but got SecondFloorHallway/SecondFloorHallway_Front.
- Door Buttons / Door_Study_SecondFloorHallway: Expected SecondFloorHallway/SecondFloorHallway_Front but got Study/Study_Front.
- Door Buttons / Door_SecondFloorHallway_FirstFloorHall: Expected FirstFloorHall/FirstFloorHall_Front but got SecondFloorHallway/SecondFloorHallway_Front.
- Door Buttons / Door_FirstFloorHall_SecondFloorHallway: Expected SecondFloorHallway/SecondFloorHallway_Front but got FirstFloorHall/FirstFloorHall_Back.
- Door Buttons / Door_Entrance_FirstFloorHall: Expected FirstFloorHall/FirstFloorHall_Front but got Entrance/Entrance_Front.
- Door Buttons / Door_FirstFloorHall_Entrance: Expected Entrance/Entrance_Front but got FirstFloorHall/FirstFloorHall_Front.
- Door Buttons / Door_SmallLivingRoom_FirstFloorHall: Expected FirstFloorHall/FirstFloorHall_Front but got SmallLivingRoom/SmallLivingRoom_Front.
- Door Buttons / Door_FirstFloorHall_SmallLivingRoom: Expected SmallLivingRoom/SmallLivingRoom_Front but got FirstFloorHall/FirstFloorHall_Front.
- Door Buttons / Door_LivingRoom_FirstFloorHall: Expected FirstFloorHall/FirstFloorHall_Front but got LivingRoom/LivingRoom_Front.
- Door Buttons / Door_FirstFloorHall_LivingRoom: Expected LivingRoom/LivingRoom_Front but got FirstFloorHall/FirstFloorHall_Front.
- Door Buttons / Door_Kitchen_FirstFloorHall: Expected FirstFloorHall/FirstFloorHall_Back but got Kitchen/Kitchen_Front.
- Door Buttons / Door_FirstFloorHall_Kitchen: Expected Kitchen/Kitchen_Front but got FirstFloorHall/FirstFloorHall_Back.
- Door Buttons / Door_Kitchen_BasementStairs: Expected BasementStairs/BasementStairs_Front but got Kitchen/Kitchen_Back.
- Door Buttons / Door_BasementStairs_Kitchen: Expected Kitchen/Kitchen_Back but got BasementStairs/BasementStairs_Front.
- Door Buttons / Door_BasementStairs_BasementStorage: Expected BasementStorage/BasementStorage_Front but got BasementStairs/BasementStairs_Back.
- Door Buttons / Door_BasementStorage_BasementStairs: Expected BasementStairs/BasementStairs_Back but got BasementStorage/BasementStorage_Front.
- ExamineImage Buttons / BedroomPhotoCodeClue: Clue panel did not become visible.
- HidePoint Buttons / HidePoint_Bedroom_CurtainCloset: HidePoint did not enter/exit correctly. Entered=False, Exited=True
- HidePoint Buttons / HidePoint_Bedroom_Closet: HidePoint did not enter/exit correctly. Entered=False, Exited=True
- HidePoint Buttons / HidePointSummary: No HidePoint interaction passed.
- FinalDoor / FinalDoor_FrontDoorKey: FinalDoor ClickableButton was not found.
- Full Scene Click Route / BedroomToBasement: Layout route did not satisfy every expected room transition.

## Notes

- This test invokes actual Scene Button.onClick listeners.
- This test verifies ClickableButton / NavigationButton wiring.
- This test does not validate final visual design.
- This test does not require final Sprite or Audio assets.
