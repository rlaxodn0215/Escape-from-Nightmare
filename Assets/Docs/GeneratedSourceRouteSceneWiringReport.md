# Source Route Scene Wiring Report

- Generated At: 2026-05-29 23:07:42
- Active Scene: GameScene
- Errors: 0
- Warnings: 0

## Managers

| Manager | Count | Result | Notes |
|---|---:|---|---|
| GameDataManager | 1 | Found |  |
| LocationManager | 1 | Found |  |
| InteractionManager | 1 | Found |  |
| InventoryManager | 1 | Found |  |
| PuzzleManager | 1 | Found |  |
| SaveManager | 1 | Found |  |
| GameManager | 1 | Found |  |
| EndingManager | 1 | Found |  |
| ClueImageManager | 1 | Found |  |
| NoiseManager | 1 | Found |  |
| GhostManager | 1 | Found |  |
| HideManager | 1 | Found |  |
| ChaseManager | 1 | Found |  |

## Location Controllers

| Location ID | Found | Views Found | Result |
|---|---:|---:|---|
| Bedroom | 1 | 4 | Found |
| ChildRoom | 1 | 4 | Found |
| Study | 1 | 4 | Found |
| SecondFloorHallway | 1 | 2 | Found |
| LivingRoom | 1 | 2 | Found |
| Entrance | 1 | 1 | Found |
| Kitchen | 1 | 1 | Found |
| BasementStorage | 1 | 4 | Found |
| LockedRoom | 1 | 4 | Found |

## Required Door Buttons

| Door ID | Found | Button Path | Result |
|---|---:|---|---|
| Door_Bedroom_SecondFloorHallway | 1 | Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front/ButtonLayer/Button_Door_Door_Bedroom_SecondFloorHallway | Found |
| Door_SecondFloorHallway_Bedroom | 1 | Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back/ButtonLayer/Button_Door_Door_SecondFloorHallway_Bedroom | Found |
| Door_SecondFloorHallway_ChildRoom | 1 | Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back/ButtonLayer/Button_Door_Door_SecondFloorHallway_ChildRoom | Found |
| Door_ChildRoom_SecondFloorHallway | 1 | Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front/ButtonLayer/Button_Door_Door_ChildRoom_SecondFloorHallway | Found |
| Door_SecondFloorHallway_Study | 1 | Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_Study | Found |
| Door_Study_SecondFloorHallway | 1 | Canvas/LocationRoot/Location_Study/View_Study_Front/ButtonLayer/Button_Door_Door_Study_SecondFloorHallway | Found |
| Door_SecondFloorHallway_LivingRoom | 1 | Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_LivingRoom | Found |
| Door_LivingRoom_SecondFloorHallway | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Door_Door_LivingRoom_SecondFloorHallway | Found |
| Door_LivingRoom_Kitchen | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Door_Door_LivingRoom_Kitchen | Found |
| Door_Kitchen_LivingRoom | 1 | Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Door_Door_Kitchen_LivingRoom | Found |
| Door_Kitchen_BasementStorage | 1 | Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Door_Door_Kitchen_BasementStorage | Found |
| Door_BasementStorage_Kitchen | 1 | Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front/ButtonLayer/Button_Door_Door_BasementStorage_Kitchen | Found |
| Door_BasementStorage_LockedRoom | 1 | Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Right/ButtonLayer/Button_Door_Door_BasementStorage_LockedRoom | Found |
| Door_LockedRoom_BasementStorage | 1 | Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Front/ButtonLayer/Button_Door_Door_LockedRoom_BasementStorage | Found |
| Door_LivingRoom_Entrance | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Door_Door_LivingRoom_Entrance | Found |
| Door_Entrance_LivingRoom | 1 | Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_Door_Door_Entrance_LivingRoom | Found |

## Required Puzzle Buttons

| Puzzle ID | Found | Button Path | Prefab Load | Result |
|---|---:|---|---|---|
| Puzzle_Bedroom_01 | 1 | Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front/ButtonLayer/Button_Puzzle_Puzzle_Bedroom_01 | Loaded | Found |
| Puzzle_LivingRoom_01 | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Puzzle_Puzzle_LivingRoom_01 | Loaded | Found |
| Puzzle_ChildRoom_01 | 1 | Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front/ButtonLayer/Button_Puzzle_Puzzle_ChildRoom_01 | Loaded | Found |
| Puzzle_Study_01 | 1 | Canvas/LocationRoot/Location_Study/View_Study_Front/ButtonLayer/Button_Puzzle_Puzzle_Study_01 | Loaded | Found |
| Puzzle_LivingRoom_02 | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Puzzle_Puzzle_LivingRoom_02 | Loaded | Found |
| Puzzle_Kitchen_01 | 1 | Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Puzzle_Puzzle_Kitchen_01 | Loaded | Found |
| Puzzle_BasementStorage_01 | 1 | Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front/ButtonLayer/Button_Puzzle_Puzzle_BasementStorage_01 | Loaded | Found |
| Puzzle_LockedRoom_01 | 1 | Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Front/ButtonLayer/Button_Puzzle_Puzzle_LockedRoom_01 | Loaded | Found |
| Puzzle_Entrance_01 | 1 | Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_Puzzle_Puzzle_Entrance_01 | Loaded | Found |

## Recommended ExamineImage Buttons

| Clue ID | Found | Button Path | Result |
|---|---:|---|---|
| BedroomPhotoCodeClue | 1 | Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Left/ButtonLayer/Button_Clue_BedroomPhotoCodeClue | Found |
| LivingRoomEntranceCodeClue | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Clue_LivingRoomEntranceCodeClue | Found |
| ChildRoomCardSymbolClueImage | 1 | Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Right/ButtonLayer/Button_Clue_ChildRoomCardSymbolClueImage | Found |
| StudyBookSymbolClueImage | 1 | Canvas/LocationRoot/Location_Study/View_Study_Right/ButtonLayer/Button_Clue_StudyBookSymbolClueImage | Found |
| KitchenCodeClueImage | 1 | Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Clue_KitchenCodeClueImage | Found |
| KitchenFridgeSurfaceSymbolClue | 1 | Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Clue_KitchenFridgeSurfaceSymbolClue | Found |
| BasementPowerPatternClue | 1 | Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Left/ButtonLayer/Button_Clue_BasementPowerPatternClue | Found |
| BasementClueImage | 1 | Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Back/ButtonLayer/Button_Clue_BasementClueImage | Found |

## Final Door / Entrance

| Requirement | Found | Result | Notes |
|---|---:|---|---|
| Puzzle_Entrance_01 or FinalDoor FrontDoorKey | 1 | OK | Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_FinalDoor_FrontDoorKey requiredItemId=FrontDoorKey linkedPuzzleId=Puzzle_Entrance_01 |

## Missing Wiring

### Errors

- None

### Warnings

- None


## Notes

- Runtime Full Route Test can pass even if Scene wiring is incomplete because it uses direct manager calls.
- This report validates manual GameScene wiring readiness.
- Do not auto-create Scene buttons from this report; wire them manually.
