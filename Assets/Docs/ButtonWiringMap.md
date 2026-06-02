# Button Wiring Map

## Door Buttons

| Scene Location | Scene View | Button Purpose | clickableType | linkedDoorId |
|---|---|---|---|---|
| Bedroom | Bedroom_Front | Go to Hallway | Door | Door_Bedroom_Hallway |
| Hallway | Hallway_Back | Go to Bedroom | Door | Door_Hallway_Bedroom |
| Hallway | Hallway_Left | Go to ChildRoom | Door | Door_Hallway_ChildRoom |
| ChildRoom | ChildRoom_Front | Return to Hallway | Door | Door_ChildRoom_Hallway |
| Hallway | Hallway_Right | Go to Study | Door | Door_Hallway_Study |
| Study | Study_Front | Return to Hallway | Door | Door_Study_Hallway |
| Hallway | Hallway_Front | Go to LivingRoom | Door | Door_Hallway_LivingRoom |
| LivingRoom | LivingRoom_Back | Return to Hallway | Door | Door_LivingRoom_Hallway |
| LivingRoom | LivingRoom_Front | Go to Kitchen | Door | Door_LivingRoom_Kitchen |
| Kitchen | Kitchen_Front | Return to LivingRoom | Door | Door_Kitchen_LivingRoom |
| LivingRoom | LivingRoom_Back | Go to Entrance | Door | Door_LivingRoom_Entrance |

## Puzzle Buttons

| Location | View | Puzzle | clickableType | linkedPuzzleId |
|---|---|---|---|---|
| Bedroom | Bedroom_Back | Bedroom code lock | Puzzle | Puzzle_Bedroom_01 |
| ChildRoom | ChildRoom_Front | Card order | Puzzle | Puzzle_ChildRoom_01 |
| Study | Study_Front | Book order | Puzzle | Puzzle_Study_01 |
| LivingRoom | LivingRoom_Front | Symbol sequence | Puzzle | Puzzle_LivingRoom_01 |
| Kitchen | Kitchen_Front | Kitchen code lock | Puzzle | Puzzle_Kitchen_01 |

## ExamineImage Buttons

| Location | View | Target | clickableType | linkedClueImageId |
|---|---|---|---|---|
| Bedroom | Bedroom_Back | Old Photo | ExamineImage | BedroomPhotoCodeClue |
| ChildRoom | ChildRoom_Back | Scattered Cards | ExamineImage | ChildRoomCardSymbolClueImage |
| LivingRoom | LivingRoom_Left | Symbol Painting | ExamineImage | Clue_LivingRoom_SymbolHint |
| Study | Study_Right | Basement Password Note | ExamineImage | Clue_BasementPassword |
| Kitchen | Kitchen_Left | Kitchen Number Hint | ExamineImage | Clue_Kitchen_NumberHint |

## UseItemTarget Buttons

| Location | View | Target | requiredItemId | linkedClueImageId | linkedPuzzleId | linkedDoorId |
|---|---|---|---|---|---|---|
| Bedroom | Bedroom_Back | Locked Drawer | OldDrawerKey | Clue_LockedDrawer_Note |  |  |

## HidePoint Buttons

| Location | View | HidePoint ID | clickableType | targetObjectId |
|---|---|---|---|---|
| Bedroom | Bedroom_Back | HidePoint_Bedroom_Closet | HidePoint | HidePoint_Bedroom_Closet |
| Bedroom | Bedroom_Back | HidePoint_Bedroom_CurtainCloset | HidePoint | HidePoint_Bedroom_CurtainCloset |

## FinalDoor Buttons

| Location | View | Purpose | requiredItemId | linkedPuzzleId | linkedDoorId |
|---|---|---|---|---|---|
| Entrance | Entrance_Front | Direct ending | FrontDoorKey |  |  |
| Entrance | Entrance_Front | Puzzle ending | FrontDoorKey | Puzzle_EntranceDoor_01 |  |

## Rules

- `linkedDoorId` must match `doors.json.doorId`.
- `linkedPuzzleId` must match `puzzles.json.puzzleId`.
- `linkedClueImageId` must match `clues.json.clueId`.
- `requiredItemId` must match `items.json.itemId`.
- Sequence `optionId` must match `puzzle_answers.json.answerSequence`.
- Symbol `optionId` must match `symbols.json.symbolId`.
