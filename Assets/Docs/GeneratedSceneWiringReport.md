# Generated Scene Wiring Report

- Generated At: 2026-06-02 21:46:35
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
### SmallLivingRoom
- Path: Canvas/LocationRoot/Location_SmallLivingRoom
- Default View: SmallLivingRoom_Front
- JSON Location Exists: True
- Views:
  - SmallLivingRoom_Front (Canvas/LocationRoot/Location_SmallLivingRoom/View_SmallLivingRoom_Front)
  - SmallLivingRoom_Back (Canvas/LocationRoot/Location_SmallLivingRoom/View_SmallLivingRoom_Back)

### BasementStorage
- Path: Canvas/LocationRoot/Location_BasementStorage
- Default View: BasementStorage_Front
- JSON Location Exists: True
- Views:
  - BasementStorage_Front (Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front)
  - BasementStorage_Back (Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Back)

### Entrance
- Path: Canvas/LocationRoot/Location_Entrance
- Default View: Entrance_Front
- JSON Location Exists: True
- Views:
  - Entrance_Front (Canvas/LocationRoot/Location_Entrance/View_Entrance_Front)
  - Entrance_Back (Canvas/LocationRoot/Location_Entrance/View_Entrance_Back)

### LivingRoom
- Path: Canvas/LocationRoot/Location_LivingRoom
- Default View: LivingRoom_Front
- JSON Location Exists: True
- Views:
  - LivingRoom_Front (Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front)
  - LivingRoom_Back (Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back)

### ChildRoom
- Path: Canvas/LocationRoot/Location_ChildRoom
- Default View: ChildRoom_Front
- JSON Location Exists: True
- Views:
  - ChildRoom_Front (Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front)
  - ChildRoom_Back (Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Back)

### Kitchen
- Path: Canvas/LocationRoot/Location_Kitchen
- Default View: Kitchen_Front
- JSON Location Exists: True
- Views:
  - Kitchen_Front (Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front)
  - Kitchen_Back (Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Back)

### Bedroom
- Path: Canvas/LocationRoot/Location_Bedroom
- Default View: Bedroom_Back
- JSON Location Exists: True
- Views:
  - Bedroom_Front (Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front)
  - Bedroom_Back (Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Back)

### BasementStairs
- Path: Canvas/LocationRoot/Location_BasementStairs
- Default View: BasementStairs_Front
- JSON Location Exists: True
- Views:
  - BasementStairs_Front (Canvas/LocationRoot/Location_BasementStairs/View_BasementStairs_Front)
  - BasementStairs_Back (Canvas/LocationRoot/Location_BasementStairs/View_BasementStairs_Back)

### SecondFloorHallway
- Path: Canvas/LocationRoot/Location_SecondFloorHallway
- Default View: SecondFloorHallway_Front
- JSON Location Exists: True
- Views:
  - SecondFloorHallway_Front (Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front)
  - SecondFloorHallway_Back (Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back)

### SecondFloorStairs
- Path: Canvas/LocationRoot/Location_SecondFloorStairs
- Default View: SecondFloorStairs_Front
- JSON Location Exists: True
- Views:
  - SecondFloorStairs_Front (Canvas/LocationRoot/Location_SecondFloorStairs/View_SecondFloorStairs_Front)
  - SecondFloorStairs_Back (Canvas/LocationRoot/Location_SecondFloorStairs/View_SecondFloorStairs_Back)

### FirstFloorHall
- Path: Canvas/LocationRoot/Location_FirstFloorHall
- Default View: FirstFloorHall_Front
- JSON Location Exists: True
- Views:
  - FirstFloorHall_Front (Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Front)
  - FirstFloorHall_Back (Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Back)

### Study
- Path: Canvas/LocationRoot/Location_Study
- Default View: Study_Front
- JSON Location Exists: True
- Views:
  - Study_Front (Canvas/LocationRoot/Location_Study/View_Study_Front)
  - Study_Back (Canvas/LocationRoot/Location_Study/View_Study_Back)

## Clickable Buttons
| Path | Type | clickableId | Door | Puzzle | Clue | Item | Required Item | Target | Location | View |
|---|---|---|---|---|---|---|---|---|---|---|
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Back/ButtonLayer/Button_Door_Door_Kitchen_BasementStairs | Door | Door_Kitchen_BasementStairs | Door_Kitchen_BasementStairs |  |  |  |  |  | Kitchen | Kitchen_Back |
| Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Front/ButtonLayer/Button_Door_Door_ChildRoom_SecondFloorHallway | Door | Door_ChildRoom_SecondFloorHallway | Door_ChildRoom_SecondFloorHallway |  |  |  |  |  | ChildRoom | ChildRoom_Front |
| Canvas/LocationRoot/Location_ChildRoom/View_ChildRoom_Back/ButtonLayer/Button_Clue_ChildRoomCardSymbolClueImage | ExamineImage | ChildRoomCardSymbolClueImage |  |  | ChildRoomCardSymbolClueImage |  |  |  | ChildRoom | ChildRoom_Back |
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Clue_KitchenFridgeSurfaceSymbolClue | ExamineImage | KitchenFridgeSurfaceSymbolClue |  |  | KitchenFridgeSurfaceSymbolClue |  |  |  | Kitchen | Kitchen_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Back/ButtonLayer/Button_Clue_KitchenCodeClueImage | ExamineImage | KitchenCodeClueImage |  |  | KitchenCodeClueImage |  |  |  | LivingRoom | LivingRoom_Back |
| Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Back/ButtonLayer/Button_Door_Door_FirstFloorHall_SecondFloorHallway | Door | Door_FirstFloorHall_SecondFloorHallway | Door_FirstFloorHall_SecondFloorHallway |  |  |  |  |  | FirstFloorHall | FirstFloorHall_Back |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front/ButtonLayer/Button_Clue_BasementPowerPatternClue | ExamineImage | BasementPowerPatternClue |  |  | BasementPowerPatternClue |  |  |  | BasementStorage | BasementStorage_Front |
| Canvas/LocationRoot/Location_Entrance/View_Entrance_Front/ButtonLayer/Button_Door_Door_Entrance_FirstFloorHall | Door | Door_Entrance_FirstFloorHall | Door_Entrance_FirstFloorHall |  |  |  |  |  | Entrance | Entrance_Front |
| Canvas/LocationRoot/Location_Kitchen/View_Kitchen_Front/ButtonLayer/Button_Door_Door_Kitchen_FirstFloorHall | Door | Door_Kitchen_FirstFloorHall | Door_Kitchen_FirstFloorHall |  |  |  |  |  | Kitchen | Kitchen_Front |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Front/ButtonLayer/Button_Door_Door_Bedroom_SecondFloorHallway | Door | Door_Bedroom_SecondFloorHallway | Door_Bedroom_SecondFloorHallway |  |  |  |  |  | Bedroom | Bedroom_Front |
| Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Front/ButtonLayer/Button_Door_Door_FirstFloorHall_SmallLivingRoom | Door | Door_FirstFloorHall_SmallLivingRoom | Door_FirstFloorHall_SmallLivingRoom |  |  |  |  |  | FirstFloorHall | FirstFloorHall_Front |
| Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Front/ButtonLayer/Button_Door_Door_FirstFloorHall_Entrance | Door | Door_FirstFloorHall_Entrance | Door_FirstFloorHall_Entrance |  |  |  |  |  | FirstFloorHall | FirstFloorHall_Front |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Back/ButtonLayer/Button_Clue_BasementClueImage | ExamineImage | BasementClueImage |  |  | BasementClueImage |  |  |  | BasementStorage | BasementStorage_Back |
| Canvas/LocationRoot/Location_Study/View_Study_Back/ButtonLayer/Button_Door_Door_Study_SecondFloorHallway | Door | Door_Study_SecondFloorHallway | Door_Study_SecondFloorHallway |  |  |  |  |  | Study | Study_Back |
| Canvas/LocationRoot/Location_BasementStairs/View_BasementStairs_Front/ButtonLayer/Button_Door_Door_BasementStairs_Kitchen | Door | Door_BasementStairs_Kitchen | Door_BasementStairs_Kitchen |  |  |  |  |  | BasementStairs | BasementStairs_Front |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Door_Door_LivingRoom_FirstFloorHall | Door | Door_LivingRoom_FirstFloorHall | Door_LivingRoom_FirstFloorHall |  |  |  |  |  | LivingRoom | LivingRoom_Front |
| Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Back/ButtonLayer/Button_Door_Door_FirstFloorHall_Kitchen | Door | Door_FirstFloorHall_Kitchen | Door_FirstFloorHall_Kitchen |  |  |  |  |  | FirstFloorHall | FirstFloorHall_Back |
| Canvas/LocationRoot/Location_LivingRoom/View_LivingRoom_Front/ButtonLayer/Button_Clue_LivingRoomEntranceCodeClue | ExamineImage | LivingRoomEntranceCodeClue |  |  | LivingRoomEntranceCodeClue |  |  |  | LivingRoom | LivingRoom_Front |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Back/ButtonLayer/Button_HidePoint_HidePoint_Bedroom_CurtainCloset | HidePoint | HidePoint_Bedroom_CurtainCloset |  |  |  |  |  | HidePoint_Bedroom_CurtainCloset | Bedroom | Bedroom_Back |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Back/ButtonLayer/Button_Door_Door_SecondFloorHallway_Study | Door | Door_SecondFloorHallway_Study | Door_SecondFloorHallway_Study |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Back |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_FirstFloorHall | Door | Door_SecondFloorHallway_FirstFloorHall | Door_SecondFloorHallway_FirstFloorHall |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Front |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_ChildRoom | Door | Door_SecondFloorHallway_ChildRoom | Door_SecondFloorHallway_ChildRoom |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Front |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Back/ButtonLayer/Button_HidePoint_HidePoint_Bedroom_Closet | HidePoint | HidePoint_Bedroom_Closet |  |  |  |  |  | HidePoint_Bedroom_Closet | Bedroom | Bedroom_Back |
| Canvas/LocationRoot/Location_SmallLivingRoom/View_SmallLivingRoom_Front/ButtonLayer/Button_Door_Door_SmallLivingRoom_FirstFloorHall | Door | Door_SmallLivingRoom_FirstFloorHall | Door_SmallLivingRoom_FirstFloorHall |  |  |  |  |  | SmallLivingRoom | SmallLivingRoom_Front |
| Canvas/LocationRoot/Location_BasementStorage/View_BasementStorage_Front/ButtonLayer/Button_Door_Door_BasementStorage_BasementStairs | Door | Door_BasementStorage_BasementStairs | Door_BasementStorage_BasementStairs |  |  |  |  |  | BasementStorage | BasementStorage_Front |
| Canvas/LocationRoot/Location_BasementStairs/View_BasementStairs_Back/ButtonLayer/Button_Door_Door_BasementStairs_BasementStorage | Door | Door_BasementStairs_BasementStorage | Door_BasementStairs_BasementStorage |  |  |  |  |  | BasementStairs | BasementStairs_Back |
| Canvas/LocationRoot/Location_SecondFloorHallway/View_SecondFloorHallway_Front/ButtonLayer/Button_Door_Door_SecondFloorHallway_Bedroom | Door | Door_SecondFloorHallway_Bedroom | Door_SecondFloorHallway_Bedroom |  |  |  |  |  | SecondFloorHallway | SecondFloorHallway_Front |
| Canvas/LocationRoot/Location_Bedroom/View_Bedroom_Back/ButtonLayer/Button_Clue_BedroomPhotoCodeClue | ExamineImage | BedroomPhotoCodeClue |  |  | BedroomPhotoCodeClue |  |  |  | Bedroom | Bedroom_Back |
| Canvas/LocationRoot/Location_Study/View_Study_Front/ButtonLayer/Button_Clue_StudyBookSymbolClueImage | ExamineImage | StudyBookSymbolClueImage |  |  | StudyBookSymbolClueImage |  |  |  | Study | Study_Front |
| Canvas/LocationRoot/Location_FirstFloorHall/View_FirstFloorHall_Front/ButtonLayer/Button_Door_Door_FirstFloorHall_LivingRoom | Door | Door_FirstFloorHall_LivingRoom | Door_FirstFloorHall_LivingRoom |  |  |  |  |  | FirstFloorHall | FirstFloorHall_Front |

## Navigation Buttons
| Path | actionType | targetLocationId | targetViewId |
|---|---|---|---|
| Canvas/NavigationButtons/Button_RotateLeft | Rotate Left |  |  |
| Canvas/NavigationButtons/Button_RotateRight | Rotate Right |  |  |

## Inventory UI
- InventoryBarUI Count: 1
- InventorySlotUI Count: 6
  - Canvas/InventoryBar/Slot_04
  - Canvas/InventoryBar/Slot_02
  - Canvas/InventoryBar/Slot_03
  - Canvas/InventoryBar/Slot_01
  - Canvas/InventoryBar/Slot_05
  - Canvas/InventoryBar/Slot_06

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
  - Canvas/HideViewRoot/HideExitButton

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
