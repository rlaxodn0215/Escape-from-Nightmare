# Generated Scene Wiring Report

- Generated At: 2026-05-29 21:40:30
- Active Scene: GameScene

## Managers
- GameManager: 1
  - Managers/GameManager
- SaveManager: 1
  - Managers/SaveManager
- GameDataManager: 1
  - Managers/GameDataManager
- LocationManager: 1
  - Managers/LocationManager
- InteractionManager: 1
  - Managers/InteractionManager
- InventoryManager: 1
  - Managers/InventoryManager
- PuzzleManager: 1
  - PuzzleManager
- EndingManager: 1
  - Managers/EndingManager
- NoiseManager: 1
  - Managers/NoiseManager
- GhostManager: 1
  - Managers/GhostManager
- HideManager: 1
  - Managers/HideManager
- ChaseManager: 1
  - Managers/ChaseManager
- ClueImageManager: 1
  - Managers/ClueImageManager

## Locations
### LockedRoom
- Path: Canvas/LocationRoot/Location_LockedRoom
- Default View: LockedRoom_Front
- JSON Location Exists: True
- Views:
  - LockedRoom_Front (Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Front)
  - LockedRoom_Right (Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Right)
  - LockedRoom_Back (Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Back)
  - LockedRoom_Left (Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Left)

### BasementStorage
- Path: Canvas/LocationRoot/Location_BasementStorage
- Default View: BasementStorage_Front
- JSON Location Exists: True
- Views:
  - BasementStorage_Front (Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front)
  - BasementStorage_Right (Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Right)
  - BasementStorage_Back (Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Back)
  - BasementStorage_Left (Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Left)

### Kitchen
- Path: Canvas/LocationRoot/Location_Kitchen
- Default View: Kitchen_Front
- JSON Location Exists: True
- Views:
  - Kitchen_Front (Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front)

### Entrance
- Path: Canvas/LocationRoot/Location_Entrance
- Default View: Entrance_Front
- JSON Location Exists: True
- Views:
  - Entrance_Front (Canvas/LocationRoot/Location_Entrance/View_Entrance_Front)

### LivingRoom
- Path: Canvas/LocationRoot/Location_LivingRoom
- Default View: LivingRoom_Front
- JSON Location Exists: True
- Views:
  - LivingRoom_Front (Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front)
  - LivingRoom_Back (Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back)

### SecondFloorHallway
- Path: Canvas/LocationRoot/Location_SecondFloorHallway
- Default View: SecondFloorHallway_Front
- JSON Location Exists: True
- Views:
  - SecondFloorHallway_Front (Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front)
  - SecondFloorHallway_Back (Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back)

### Study
- Path: Canvas/LocationRoot/Location_Study
- Default View: Study_Front
- JSON Location Exists: True
- Views:
  - Study_Front (Canvas/LocationRoot/Location_Study/View_Study_Front)
  - Study_Right (Canvas/LocationRoot/Location_Study/View_Study_Right)
  - Study_Back (Canvas/LocationRoot/Location_Study/View_Study_Back)
  - Study_Left (Canvas/LocationRoot/Location_Study/View_Study_Left)

### ChildRoom
- Path: Canvas/LocationRoot/Location_ChildRoom
- Default View: ChildRoom_Front
- JSON Location Exists: True
- Views:
  - ChildRoom_Front (Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front)
  - ChildRoom_Right (Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Right)
  - ChildRoom_Back (Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Back)
  - ChildRoom_Left (Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Left)

### Bedroom
- Path: Canvas/LocationRoot/Location_Bedroom
- Default View: Bedroom_Front
- JSON Location Exists: True
- Views:
  - Bedroom_Front (Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front)
  - Bedroom_Right (Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Right)
  - Bedroom_Back (Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Back)
  - Bedroom_Left (Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Left)

## Clickable Buttons
| Path | Type | clickableId | Door | Puzzle | Clue | Item | Required Item | Target | Location | View |
|---|---|---|---|---|---|---|---|---|---|---|
| Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_FinalDoor_FrontDoorKey | FinalDoor | FinalDoor_FrontDoorKey |  | Puzzle_Entrance_01 |  |  | FrontDoorKey |  | Entrance | Entrance_Front |
| Canvas/LocationRoot/Location_Study/View_Study_Left/ButtonLayer/Button_HidePoint_HidePoint_Study_Desk | HidePoint | HidePoint_Study_Desk |  |  |  |  |  | HidePoint_Study_Desk | Study | Study_Left |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back/ButtonLayer/Button_HidePoint_HidePoint_SecondFloorHallway_Cabinet | HidePoint | HidePoint_SecondFloorHallway_Cabinet |  |  |  |  |  | HidePoint_SecondFloorHallway_Cabinet | SecondFloorHallway | SecondFloorHallway_Back |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Back/ButtonLayer/Button_HidePoint_HidePoint_Bedroom_Closet | HidePoint | HidePoint_Bedroom_Closet |  |  |  |  |  | HidePoint_Bedroom_Closet | Bedroom | Bedroom_Back |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Back/ButtonLayer/Button_Clue_BasementClueImage | ExamineImage | BasementClueImage |  |  | BasementClueImage |  |  |  | BasementStorage | BasementStorage_Back |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Left/ButtonLayer/Button_Clue_BasementPowerPatternClue | ExamineImage | BasementPowerPatternClue |  |  | BasementPowerPatternClue |  |  |  | BasementStorage | BasementStorage_Left |
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Clue_KitchenFridgeSurfaceSymbolClue | ExamineImage | KitchenFridgeSurfaceSymbolClue |  |  | KitchenFridgeSurfaceSymbolClue |  |  |  | Kitchen | Kitchen_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Clue_KitchenCodeClueImage | ExamineImage | KitchenCodeClueImage |  |  | KitchenCodeClueImage |  |  |  | LivingRoom | LivingRoom_Back |
| Canvas/LocationRoot/Location_Study/View_Study_Right/ButtonLayer/Button_Clue_StudyBookSymbolClueImage | ExamineImage | StudyBookSymbolClueImage |  |  | StudyBookSymbolClueImage |  |  |  | Study | Study_Right |
| Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Right/ButtonLayer/Button_Clue_ChildRoomCardSymbolClueImage | ExamineImage | ChildRoomCardSymbolClueImage |  |  | ChildRoomCardSymbolClueImage |  |  |  | ChildRoom | ChildRoom_Right |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Clue_LivingRoomEntranceCodeClue | ExamineImage | LivingRoomEntranceCodeClue |  |  | LivingRoomEntranceCodeClue |  |  |  | LivingRoom | LivingRoom_Front |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Left/ButtonLayer/Button_Clue_BedroomPhotoCodeClue | ExamineImage | BedroomPhotoCodeClue |  |  | BedroomPhotoCodeClue |  |  |  | Bedroom | Bedroom_Left |
| Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_Puzzle_Puzzle_Entrance_01 | Puzzle | Puzzle_Entrance_01 |  | Puzzle_Entrance_01 |  |  |  |  | Entrance | Entrance_Front |
| Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Front/ButtonLayer/Button_Puzzle_Puzzle_LockedRoom_01 | Puzzle | Puzzle_LockedRoom_01 |  | Puzzle_LockedRoom_01 |  |  |  |  | LockedRoom | LockedRoom_Front |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front/ButtonLayer/Button_Puzzle_Puzzle_BasementStorage_01 | Puzzle | Puzzle_BasementStorage_01 |  | Puzzle_BasementStorage_01 |  |  |  |  | BasementStorage | BasementStorage_Front |
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Puzzle_Puzzle_Kitchen_01 | Puzzle | Puzzle_Kitchen_01 |  | Puzzle_Kitchen_01 |  |  |  |  | Kitchen | Kitchen_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Puzzle_Puzzle_LivingRoom_02 | Puzzle | Puzzle_LivingRoom_02 |  | Puzzle_LivingRoom_02 |  |  |  |  | LivingRoom | LivingRoom_Back |
| Canvas/LocationRoot/Location_Study/View_Study_Front/ButtonLayer/Button_Puzzle_Puzzle_Study_01 | Puzzle | Puzzle_Study_01 |  | Puzzle_Study_01 |  |  |  |  | Study | Study_Front |
| Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front/ButtonLayer/Button_Puzzle_Puzzle_ChildRoom_01 | Puzzle | Puzzle_ChildRoom_01 |  | Puzzle_ChildRoom_01 |  |  |  |  | ChildRoom | ChildRoom_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Puzzle_Puzzle_LivingRoom_01 | Puzzle | Puzzle_LivingRoom_01 |  | Puzzle_LivingRoom_01 |  |  |  |  | LivingRoom | LivingRoom_Front |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front/ButtonLayer/Button_Puzzle_Puzzle_Bedroom_01 | Puzzle | Puzzle_Bedroom_01 |  | Puzzle_Bedroom_01 |  |  |  |  | Bedroom | Bedroom_Front |
| Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_Door_Door_Entrance_LivingRoom | Door | Door_Entrance_LivingRoom | Door_Entrance_LivingRoom |  |  |  |  |  | Entrance | Entrance_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Door_Door_LivingRoom_Entrance | Door | Door_LivingRoom_Entrance | Door_LivingRoom_Entrance |  |  |  |  |  | LivingRoom | LivingRoom_Front |
| Canvas/LocationRoot/Location_LockedRoom/View_LockedRoom_Front/ButtonLayer/Button_Door_Door_LockedRoom_BasementStorage | Door | Door_LockedRoom_BasementStorage | Door_LockedRoom_BasementStorage |  |  |  |  |  | LockedRoom | LockedRoom_Front |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Right/ButtonLayer/Button_Door_Door_BasementStorage_LockedRoom | Door | Door_BasementStorage_LockedRoom | Door_BasementStorage_LockedRoom |  |  |  |  |  | BasementStorage | BasementStorage_Right |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front/ButtonLayer/Button_Door_Door_BasementStorage_Kitchen | Door | Door_BasementStorage_Kitchen | Door_BasementStorage_Kitchen |  |  |  |  |  | BasementStorage | BasementStorage_Front |
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Door_Door_Kitchen_BasementStorage | Door | Door_Kitchen_BasementStorage | Door_Kitchen_BasementStorage |  |  |  |  |  | Kitchen | Kitchen_Front |
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Door_Door_Kitchen_LivingRoom | Door | Door_Kitchen_LivingRoom | Door_Kitchen_LivingRoom |  |  |  |  |  | Kitchen | Kitchen_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Door_Door_LivingRoom_Kitchen | Door | Door_LivingRoom_Kitchen | Door_LivingRoom_Kitchen |  |  |  |  |  | LivingRoom | LivingRoom_Back |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Door_Door_LivingRoom_SecondFloorHallway | Door | Door_LivingRoom_SecondFloorHallway | Door_LivingRoom_SecondFloorHallway |  |  |  |  |  | LivingRoom | LivingRoom_Back |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_LivingRoom | Door | Door_SecondFloorHallway_LivingRoom | Door_SecondFloorHallway_LivingRoom |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Front |
| Canvas/LocationRoot/Location_Study/View_Study_Front/ButtonLayer/Button_Door_Door_Study_SecondFloorHallway | Door | Door_Study_SecondFloorHallway | Door_Study_SecondFloorHallway |  |  |  |  |  | Study | Study_Front |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_Study | Door | Door_SecondFloorHallway_Study | Door_SecondFloorHallway_Study |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Front |
| Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front/ButtonLayer/Button_Door_Door_ChildRoom_SecondFloorHallway | Door | Door_ChildRoom_SecondFloorHallway | Door_ChildRoom_SecondFloorHallway |  |  |  |  |  | ChildRoom | ChildRoom_Front |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back/ButtonLayer/Button_Door_Door_SecondFloorHallway_ChildRoom | Door | Door_SecondFloorHallway_ChildRoom | Door_SecondFloorHallway_ChildRoom |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Back |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back/ButtonLayer/Button_Door_Door_SecondFloorHallway_Bedroom | Door | Door_SecondFloorHallway_Bedroom | Door_SecondFloorHallway_Bedroom |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Back |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front/ButtonLayer/Button_Door_Door_Bedroom_SecondFloorHallway | Door | Door_Bedroom_SecondFloorHallway | Door_Bedroom_SecondFloorHallway |  |  |  |  |  | Bedroom | Bedroom_Front |

## Navigation Buttons
| Path | actionType | targetLocationId | targetViewId |
|---|---|---|---|
| Canvas/NavigationButtons/Button_RotateRight | Rotate Right |  |  |
| Canvas/NavigationButtons/Button_RotateLeft | Rotate Left |  |  |

## Inventory UI
- InventoryBarUI Count: 1
- InventorySlotUI Count: 6
  - Canvas/InventoryBar/Slot_06
  - Canvas/InventoryBar/Slot_05
  - Canvas/InventoryBar/Slot_04
  - Canvas/InventoryBar/Slot_03
  - Canvas/InventoryBar/Slot_02
  - Canvas/InventoryBar/Slot_01

## Panels
- ClueImagePanelUI: 1
  - Canvas/ClueImagePanel
- GameOverPanelUI: 1
  - Canvas/GameOverPanel
- EndingPanelUI: 1
  - Canvas/EndingPanel
- GhostStatusUI: 1
  - Canvas/GhostStatusPanel
- HideExitButton: 1
  - Canvas/HideExitButton

## Puzzle Prefabs
| Puzzle ID | Type | Prefab Path | Load Result | PuzzleUIBase |
|---|---|---|---|---|
| Puzzle_Bedroom_01 | NumberCode | PuzzleUI/PuzzleUI_BedroomNumberCode | Loaded | Yes |
| Puzzle_ChildRoom_01 | Sequence | PuzzleUI/PuzzleUI_ChildRoomCardOrder | Loaded | Yes |
| Puzzle_Study_01 | Sequence | PuzzleUI/PuzzleUI_StudyBookOrder | Loaded | Yes |
| Puzzle_LivingRoom_01 | ItemUse | PuzzleUI/PuzzleUI_LivingRoomItemUse | Loaded | Yes |
| Puzzle_LivingRoom_02 | SymbolCycle | PuzzleUI/PuzzleUI_LivingRoomSymbolSequence | Loaded | Yes |
| Puzzle_Entrance_01 | ItemUse | PuzzleUI/PuzzleUI_EntranceDoor | Loaded | Yes |
| Puzzle_Kitchen_01 | NumberCode | PuzzleUI/PuzzleUI_KitchenNumberCode | Loaded | Yes |
| Puzzle_BasementStorage_01 | PowerDevice | PuzzleUI/PuzzleUI_BasementPowerDevice | Loaded | Yes |
| Puzzle_LockedRoom_01 | FinalSymbolItem | PuzzleUI/PuzzleUI_LockedRoomFinal | Loaded | Yes |

## Warnings
- This report is a snapshot for manual wiring review.
- Run `Escape From Nightmare / Validate Current Scene Wiring` for detailed Scene errors.
- Run `Escape From Nightmare / Validate Puzzle Prefab Contracts` for detailed Prefab errors.
