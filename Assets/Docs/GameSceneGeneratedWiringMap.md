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
- ScreenFadeManager

## UI Roots

- LocationRoot
- NavigationButtons
- InventoryBar
- PuzzleUIRoot
- ClueImagePanel
- GameOverPanel
- EndingPanel
- HideViewRoot
- HideViewRoot/Background
- HideViewRoot/HideExitButton
- GhostStatusPanel
- ScreenFadeOverlay

## Door Buttons

| Door ID | Parent View | Button Name |
|---|---|---|
| Door_Bedroom_SecondFloorHallway | Bedroom_Front | Button_Door_Door_Bedroom_SecondFloorHallway |
| Door_SecondFloorHallway_Bedroom | SecondFloorHallway_Back | Button_Door_Door_SecondFloorHallway_Bedroom |
| Door_SecondFloorHallway_ChildRoom | SecondFloorHallway_Front | Button_Door_Door_SecondFloorHallway_ChildRoom |
| Door_ChildRoom_SecondFloorHallway | ChildRoom_Back | Button_Door_Door_ChildRoom_SecondFloorHallway |
| Door_SecondFloorHallway_Study | SecondFloorHallway_Back | Button_Door_Door_SecondFloorHallway_Study |
| Door_Study_SecondFloorHallway | Study_Front | Button_Door_Door_Study_SecondFloorHallway |
| Door_SecondFloorHallway_FirstFloorHall | SecondFloorHallway_Front | Button_Door_Door_SecondFloorHallway_FirstFloorHall |
| Door_FirstFloorHall_SecondFloorHallway | FirstFloorHall_Back | Button_Door_Door_FirstFloorHall_SecondFloorHallway |
| Door_Entrance_FirstFloorHall | Entrance_Front | Button_Door_Door_Entrance_FirstFloorHall |
| Door_FirstFloorHall_Entrance | FirstFloorHall_Front | Button_Door_Door_FirstFloorHall_Entrance |
| Door_SmallLivingRoom_FirstFloorHall | SmallLivingRoom_Front | Button_Door_Door_SmallLivingRoom_FirstFloorHall |
| Door_FirstFloorHall_SmallLivingRoom | FirstFloorHall_Front | Button_Door_Door_FirstFloorHall_SmallLivingRoom |
| Door_LivingRoom_FirstFloorHall | LivingRoom_Front | Button_Door_Door_LivingRoom_FirstFloorHall |
| Door_FirstFloorHall_LivingRoom | FirstFloorHall_Front | Button_Door_Door_FirstFloorHall_LivingRoom |
| Door_Kitchen_FirstFloorHall | Kitchen_Front | Button_Door_Door_Kitchen_FirstFloorHall |
| Door_FirstFloorHall_Kitchen | FirstFloorHall_Back | Button_Door_Door_FirstFloorHall_Kitchen |
| Door_Kitchen_BasementStairs | Kitchen_Back | Button_Door_Door_Kitchen_BasementStairs |
| Door_BasementStairs_Kitchen | BasementStairs_Front | Button_Door_Door_BasementStairs_Kitchen |
| Door_BasementStairs_BasementStorage | BasementStairs_Back | Button_Door_Door_BasementStairs_BasementStorage |
| Door_BasementStorage_BasementStairs | BasementStorage_Front | Button_Door_Door_BasementStorage_BasementStairs |

## Puzzle Buttons

| Puzzle ID | Parent View | Button Name |
|---|---|---|

## Clue Buttons

| Clue ID | Parent View | Button Name |
|---|---|---|
| BedroomPhotoCodeClue | Bedroom_Back | Button_Clue_BedroomPhotoCodeClue |
| LivingRoomEntranceCodeClue | LivingRoom_Front | Button_Clue_LivingRoomEntranceCodeClue |
| ChildRoomCardSymbolClueImage | ChildRoom_Back | Button_Clue_ChildRoomCardSymbolClueImage |
| StudyBookSymbolClueImage | Study_Back | Button_Clue_StudyBookSymbolClueImage |
| KitchenCodeClueImage | SmallLivingRoom_Back | Button_Clue_KitchenCodeClueImage |
| KitchenFridgeSurfaceSymbolClue | Kitchen_Front | Button_Clue_KitchenFridgeSurfaceSymbolClue |
| BasementPowerPatternClue | BasementStorage_Front | Button_Clue_BasementPowerPatternClue |
| BasementClueImage | BasementStorage_Back | Button_Clue_BasementClueImage |

## HidePoints

| HidePoint ID | Parent View | Button Name |
|---|---|---|
| HidePoint_Bedroom_Closet | Bedroom_Back | Button_HidePoint_HidePoint_Bedroom_Closet |
| HidePoint_Bedroom_CurtainCloset | Bedroom_Back | Button_HidePoint_HidePoint_Bedroom_CurtainCloset |
| HidePoint_ChildRoom_Wardrobe | ChildRoom_Front | Button_HidePoint_HidePoint_ChildRoom_Wardrobe |

## FinalDoor

| Parent View | Button Name | Required Item | Linked Puzzle |
|---|---|---|---|
| Entrance_Front | Button_FinalDoor_FrontDoorKey | FrontDoorKey | Puzzle_Entrance_01 |
