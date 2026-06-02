# Game Data ID Map

This document is the first-pass ID map for wiring Unity Editor objects to the JSON data.

## Locations

- Bedroom
- ChildRoom
- Study
- SecondFloorHallway
- LivingRoom
- Kitchen
- BasementStorage
- LockedRoom
- Entrance

## Floor Plan

- 2F: Bedroom, ChildRoom, Study, SecondFloorHallway
- 1F: LivingRoom, Kitchen, Entrance
- B1: BasementStorage, LockedRoom

`SecondFloorHallway_Front` includes the direct stairs route down to `FirstFloorHall`. The main 2F to 1F route does not stop at a separate staircase location.

## Views

- Bedroom: Bedroom_Front, Bedroom_Back
- ChildRoom: ChildRoom_Front, ChildRoom_Right, ChildRoom_Back, ChildRoom_Left
- Study: Study_Front, Study_Right, Study_Back
- SecondFloorHallway: SecondFloorHallway_Front, SecondFloorHallway_Back
- LivingRoom: LivingRoom_Front, LivingRoom_Back
- Kitchen: Kitchen_Front
- BasementStorage: BasementStorage_Front, BasementStorage_Right, BasementStorage_Back, BasementStorage_Left
- LockedRoom: LockedRoom_Front, LockedRoom_Right, LockedRoom_Back, LockedRoom_Left
- Entrance: Entrance_Front

## Doors

| Door ID | From | To | Requirement | Notes |
|---|---|---|---|---|
| Door_Bedroom_SecondFloorHallway | Bedroom_Front | SecondFloorHallway_Back | None | Bedroom exit to 2F hallway |
| Door_SecondFloorHallway_Bedroom | SecondFloorHallway_Back | Bedroom_Front | None | Return to bedroom |
| Door_SecondFloorHallway_ChildRoom | SecondFloorHallway_Front | ChildRoom_Front | None | Child room access |
| Door_ChildRoom_SecondFloorHallway | ChildRoom_Front | SecondFloorHallway_Front | None | Return to hallway Front |
| Door_SecondFloorHallway_Study | SecondFloorHallway_Back | Study_Front | None | Study access from 2F hallway Back |
| Door_Study_SecondFloorHallway | Study_Front | SecondFloorHallway_Front | None | Return to hallway Front |
| Door_SecondFloorHallway_FirstFloorHall | SecondFloorHallway_Front | FirstFloorHall_Front | None | Click stairs to go down to 1F hall |
| Door_FirstFloorHall_SecondFloorHallway | FirstFloorHall_Back | SecondFloorHallway_Front | None | Return upstairs to 2F hallway Front |
| Door_LivingRoom_Kitchen | LivingRoom_Front | Kitchen_Front | None | Kitchen access |
| Door_Kitchen_LivingRoom | Kitchen_Front | LivingRoom_Front | None | Return to living room |
| Door_Kitchen_BasementStorage | Kitchen_Front | BasementStorage_Front | None | Basement storage access |
| Door_BasementStorage_Kitchen | BasementStorage_Front | Kitchen_Front | None | Return to kitchen |
| Door_BasementStorage_LockedRoom | BasementStorage_Right | LockedRoom_Front | Puzzle_BasementStorage_01 | Locked room access |
| Door_LockedRoom_BasementStorage | LockedRoom_Front | BasementStorage_Right | None | Return to basement storage |
| Door_LivingRoom_Entrance | LivingRoom_Back | Entrance_Front | FrontDoorKey | Exit route |
| Door_Entrance_LivingRoom | Entrance_Front | LivingRoom_Back | None | Return to living room |

## Items

| Item ID | Display Name | Consumable | Used For |
|---|---|---|---|
| OldDrawerKey | Old Drawer Key | true | Opens locked drawer clue |
| FrontDoorKey | Front Door Key | false | Entrance door or FinalDoor ending |
| BasementFuse | Basement Fuse | true | Future basement power device |
| RustyHandle | Rusty Handle | true | Future mechanism |

## Puzzles

| Puzzle ID | Location | Type | Answer Variable | Reward Type | Reward ID | Test Status |
|---|---|---|---|---|---|---|
| Puzzle_Bedroom_01 | Bedroom | NumberCode | BedroomCode | Item | OldDrawerKey | Required |
| Puzzle_ChildRoom_01 | ChildRoom | Sequence | ChildRoomCardOrder | DoorUnlock | Door_Hallway_Study | Required |
| Puzzle_Study_01 | Study | Sequence | StudyBookOrder | Clue | Clue_BasementPassword | Required |
| Puzzle_LivingRoom_01 | LivingRoom | SymbolSequence | LivingRoomSymbolSequence | DoorUnlock | Door_LivingRoom_Kitchen | Required |
| Puzzle_Kitchen_01 | Kitchen | NumberCode | KitchenCode | Item | FrontDoorKey | Required |
| Puzzle_BasementPower_01 | Basement | PowerDevice | BasementPowerRoute | DoorUnlock | Door_Basement_LockedRoom | Future |
| Puzzle_LockedRoomFinal_01 | LockedRoom | FinalSequence | LockedRoomFinalSequence | Item | FrontDoorKey | Future |
| Puzzle_EntranceDoor_01 | Entrance | EntranceDoor | EntranceDoorFinal | Ending |  | Optional/Future |

## Clues

| Clue ID | Location | Requirement | Image Path | Purpose |
|---|---|---|---|---|
| Clue_Bedroom_Photo | Bedroom | startsUnlocked | ExamineImages/BedroomPhoto | Bedroom code hint |
| Clue_LockedDrawer_Note | Bedroom | OldDrawerKey | ClueImages/LockedDrawerNote | Child card order clue |
| Clue_ChildRoom_CardHint | ChildRoom | startsUnlocked | ClueImages/ChildRoomCardHint | Card order reinforcement |
| Clue_Study_BookHint | Study | Puzzle_ChildRoom_01 | ClueImages/StudyBookHint | Study book order clue |
| Clue_BasementPassword | Study | Puzzle_Study_01 | ClueImages/BasementPassword | Kitchen code clue |
| Clue_LivingRoom_SymbolHint | LivingRoom | startsUnlocked | ClueImages/LivingRoomSymbolHint | Symbol sequence clue |
| Clue_Kitchen_NumberHint | Kitchen | Puzzle_Study_01 | ClueImages/KitchenNumberHint | Kitchen code hint |
| Clue_EntranceDoor | Entrance | FrontDoorKey | ClueImages/EntranceDoor | Ending flavor clue |

## Symbols

| Symbol ID | Display Name | Sprite Path | Value |
|---|---|---|---|
| Symbol_Moon | Moon | Symbols/Symbol_Moon | 1 |
| Symbol_Eye | Eye | Symbols/Symbol_Eye | 2 |
| Symbol_Key | Key | Symbols/Symbol_Key | 3 |
| Symbol_Candle | Candle | Symbols/Symbol_Candle | 4 |
| Symbol_Raven | Raven | Symbols/Symbol_Raven | 5 |

## Ghost Rules

| Rule ID | Location | Arrival | Leave | Danger |
|---|---|---|---|---|
| GhostRule_Bedroom | Bedroom | 6-12s | 5-15s | 0.25 |
| GhostRule_Hallway | Hallway | 4-10s | 5-16s | 0.30 |
| GhostRule_ChildRoom | ChildRoom | 5-11s | 5-18s | 0.28 |
| GhostRule_Study | Study | 5-10s | 6-18s | 0.30 |
| GhostRule_LivingRoom | LivingRoom | 4-9s | 6-18s | 0.32 |
| GhostRule_Kitchen | Kitchen | 3-8s | 6-20s | 0.35 |
| GhostRule_Basement | Basement | 2-6s | 8-20s | 0.50 |
| GhostRule_LockedRoom | LockedRoom | 2-5s | 8-20s | 0.55 |
| GhostRule_Entrance | Entrance | 2-5s | 8-20s | 0.45 |

## Main Test Route

Bedroom -> SecondFloorHallway_Front -> ChildRoom -> SecondFloorHallway_Front -> SecondFloorHallway_Back -> Study -> SecondFloorHallway_Front stairs -> FirstFloorHall -> LivingRoom/SmallLivingRoom -> Kitchen -> Entrance -> Ending

## Wiring Rules

- JSON `locationId` must match `LocationController.locationId`.
- JSON `viewId` must match `LocationView.viewId`.
- `DoorRecord.doorId` must match Door button `linkedDoorId`.
- `PuzzleRecord.puzzleId` must match Puzzle button `linkedPuzzleId`.
- `ClueRecord.clueId` must match ExamineImage button `linkedClueImageId`.
- `ItemRecord.itemId` must match PickupItem, UseItemTarget, and inventory IDs.
- `PuzzleAnswerRecord.answerVariableName` must match `PuzzleRecord.answerVariableName`.
- Sequence button `optionId` must match `puzzle_answers.json` answerSequence values.
- Symbol button `optionId` must match `symbols.json` symbolId values.
