# Game Data ID Map

This document is the first-pass ID map for wiring Unity Editor objects to the JSON data.

## Locations

- Bedroom
- Hallway
- ChildRoom
- Study
- LivingRoom
- Kitchen
- Basement
- LockedRoom
- Entrance

## Views

- Bedroom: Bedroom_Front, Bedroom_Right, Bedroom_Back, Bedroom_Left
- Hallway: Hallway_Front, Hallway_Right, Hallway_Back, Hallway_Left
- ChildRoom: ChildRoom_Front, ChildRoom_Right, ChildRoom_Back, ChildRoom_Left
- Study: Study_Front, Study_Right, Study_Back, Study_Left
- LivingRoom: LivingRoom_Front, LivingRoom_Right, LivingRoom_Back, LivingRoom_Left
- Kitchen: Kitchen_Front, Kitchen_Right, Kitchen_Back, Kitchen_Left
- Basement: Basement_Front, Basement_Right, Basement_Back, Basement_Left
- LockedRoom: LockedRoom_Front, LockedRoom_Right, LockedRoom_Back, LockedRoom_Left
- Entrance: Entrance_Front, Entrance_Right, Entrance_Back, Entrance_Left

## Doors

| Door ID | From | To | Requirement | Notes |
|---|---|---|---|---|
| Door_Bedroom_Hallway | Bedroom_Front | Hallway_Back | None | First exit |
| Door_Hallway_Bedroom | Hallway_Back | Bedroom_Front | None | Return to bedroom |
| Door_Hallway_ChildRoom | Hallway_Left | ChildRoom_Front | None | Child room access |
| Door_ChildRoom_Hallway | ChildRoom_Front | Hallway_Left | None | Return to hallway |
| Door_Hallway_Study | Hallway_Right | Study_Front | Puzzle_ChildRoom_01 | Unlocked by child room puzzle |
| Door_Study_Hallway | Study_Front | Hallway_Right | None | Return to hallway |
| Door_Hallway_LivingRoom | Hallway_Front | LivingRoom_Back | None | Living room access |
| Door_LivingRoom_Hallway | LivingRoom_Back | Hallway_Front | None | Return to hallway |
| Door_LivingRoom_Kitchen | LivingRoom_Right | Kitchen_Front | Puzzle_LivingRoom_01 | Unlocked by symbol puzzle |
| Door_Kitchen_LivingRoom | Kitchen_Front | LivingRoom_Right | None | Return to living room |
| Door_Kitchen_Basement | Kitchen_Back | Basement_Front | Puzzle_Kitchen_01 | Future route after kitchen code |
| Door_Basement_Kitchen | Basement_Front | Kitchen_Back | None | Return to kitchen |
| Door_Basement_LockedRoom | Basement_Right | LockedRoom_Front | Puzzle_BasementPower_01 | Future locked room access |
| Door_LockedRoom_Basement | LockedRoom_Front | Basement_Right | None | Return to basement |
| Door_LivingRoom_Entrance | LivingRoom_Front | Entrance_Front | FrontDoorKey | Exit route |
| Door_Entrance_LivingRoom | Entrance_Back | LivingRoom_Front | None | Return to living room |

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

Bedroom -> Hallway -> ChildRoom -> Hallway -> Study -> Hallway -> LivingRoom -> Kitchen -> Entrance -> Ending

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
