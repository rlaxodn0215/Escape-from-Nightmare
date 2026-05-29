# GameScene Generated Wiring Map

## Managers

- GameManager
- SaveManager
- GameDataManager
- LocationManager
- InteractionManager
- InventoryManager
- PuzzleManager
- EndingManager
- ClueImageManager
- NoiseManager
- GhostManager
- HideManager
- ChaseManager

## UI Roots

- LocationRoot
- NavigationButtons
- InventoryBar
- PuzzleUIRoot
- ClueImagePanel
- GameOverPanel
- EndingPanel
- HideExitButton
- GhostStatusPanel

## Door Buttons

| Door ID | Parent View | Button Name |
|---|---|---|
| Door_Bedroom_SecondFloorHallway | Bedroom_Front | Button_Door_Door_Bedroom_SecondFloorHallway |
| Door_SecondFloorHallway_Bedroom | SecondFloorHallway_Back | Button_Door_Door_SecondFloorHallway_Bedroom |
| Door_SecondFloorHallway_ChildRoom | SecondFloorHallway_Back | Button_Door_Door_SecondFloorHallway_ChildRoom |
| Door_ChildRoom_SecondFloorHallway | ChildRoom_Front | Button_Door_Door_ChildRoom_SecondFloorHallway |
| Door_SecondFloorHallway_Study | SecondFloorHallway_Front | Button_Door_Door_SecondFloorHallway_Study |
| Door_Study_SecondFloorHallway | Study_Front | Button_Door_Door_Study_SecondFloorHallway |
| Door_SecondFloorHallway_LivingRoom | SecondFloorHallway_Front | Button_Door_Door_SecondFloorHallway_LivingRoom |
| Door_LivingRoom_SecondFloorHallway | LivingRoom_Back | Button_Door_Door_LivingRoom_SecondFloorHallway |
| Door_LivingRoom_Kitchen | LivingRoom_Back | Button_Door_Door_LivingRoom_Kitchen |
| Door_Kitchen_LivingRoom | Kitchen_Front | Button_Door_Door_Kitchen_LivingRoom |
| Door_Kitchen_BasementStorage | Kitchen_Front | Button_Door_Door_Kitchen_BasementStorage |
| Door_BasementStorage_Kitchen | BasementStorage_Front | Button_Door_Door_BasementStorage_Kitchen |
| Door_BasementStorage_LockedRoom | BasementStorage_Right | Button_Door_Door_BasementStorage_LockedRoom |
| Door_LockedRoom_BasementStorage | LockedRoom_Front | Button_Door_Door_LockedRoom_BasementStorage |
| Door_LivingRoom_Entrance | LivingRoom_Front | Button_Door_Door_LivingRoom_Entrance |
| Door_Entrance_LivingRoom | Entrance_Front | Button_Door_Door_Entrance_LivingRoom |

## Puzzle Buttons

| Puzzle ID | Parent View | Button Name |
|---|---|---|
| Puzzle_Bedroom_01 | Bedroom_Front | Button_Puzzle_Puzzle_Bedroom_01 |
| Puzzle_LivingRoom_01 | LivingRoom_Front | Button_Puzzle_Puzzle_LivingRoom_01 |
| Puzzle_ChildRoom_01 | ChildRoom_Front | Button_Puzzle_Puzzle_ChildRoom_01 |
| Puzzle_Study_01 | Study_Front | Button_Puzzle_Puzzle_Study_01 |
| Puzzle_LivingRoom_02 | LivingRoom_Back | Button_Puzzle_Puzzle_LivingRoom_02 |
| Puzzle_Kitchen_01 | Kitchen_Front | Button_Puzzle_Puzzle_Kitchen_01 |
| Puzzle_BasementStorage_01 | BasementStorage_Front | Button_Puzzle_Puzzle_BasementStorage_01 |
| Puzzle_LockedRoom_01 | LockedRoom_Front | Button_Puzzle_Puzzle_LockedRoom_01 |
| Puzzle_Entrance_01 | Entrance_Front | Button_Puzzle_Puzzle_Entrance_01 |

## Clue Buttons

| Clue ID | Parent View | Button Name |
|---|---|---|
| BedroomPhotoCodeClue | Bedroom_Left | Button_Clue_BedroomPhotoCodeClue |
| LivingRoomEntranceCodeClue | LivingRoom_Front | Button_Clue_LivingRoomEntranceCodeClue |
| ChildRoomCardSymbolClueImage | ChildRoom_Right | Button_Clue_ChildRoomCardSymbolClueImage |
| StudyBookSymbolClueImage | Study_Right | Button_Clue_StudyBookSymbolClueImage |
| KitchenCodeClueImage | LivingRoom_Back | Button_Clue_KitchenCodeClueImage |
| KitchenFridgeSurfaceSymbolClue | Kitchen_Front | Button_Clue_KitchenFridgeSurfaceSymbolClue |
| BasementPowerPatternClue | BasementStorage_Left | Button_Clue_BasementPowerPatternClue |
| BasementClueImage | BasementStorage_Back | Button_Clue_BasementClueImage |

## HidePoints

| HidePoint ID | Parent View | Button Name |
|---|---|---|
| HidePoint_Bedroom_Closet | Bedroom_Back | Button_HidePoint_HidePoint_Bedroom_Closet |
| HidePoint_SecondFloorHallway_Cabinet | SecondFloorHallway_Back | Button_HidePoint_HidePoint_SecondFloorHallway_Cabinet |
| HidePoint_Study_Desk | Study_Left | Button_HidePoint_HidePoint_Study_Desk |

## FinalDoor

| Parent View | Button Name | Required Item | Linked Puzzle |
|---|---|---|---|
| Entrance_Front | Button_FinalDoor_FrontDoorKey | FrontDoorKey | Puzzle_Entrance_01 |
