# ID Migration Guide - Source Aligned

| Legacy ID | Source-Aligned ID | Type | Notes |
|---|---|---|---|
| Hallway | SecondFloorHallway | Location | 2층 복도 |
| Basement | BasementStorage | Location | 지하 창고 |
| Puzzle_BasementPower_01 | Puzzle_BasementStorage_01 | Puzzle | 전원 장치 |
| Puzzle_LockedRoomFinal_01 | Puzzle_LockedRoom_01 | Puzzle | 잠긴 방 최종 퍼즐 |
| Puzzle_EntranceDoor_01 | Puzzle_Entrance_01 | Puzzle | 현관 퍼즐 |
| Symbol_Moon | Symbol_01 | Symbol | 테스트용 원문 정렬 |
| Symbol_Eye | Symbol_02 | Symbol | 테스트용 원문 정렬 |
| Symbol_Key | Symbol_03 | Symbol | 테스트용 원문 정렬 |

## Scene Wiring Updates

Existing Scene objects are not modified automatically.

Update these manually:

- `LocationController.locationId`
- `LocationView.viewId`
- `ClickableButton.linkedDoorId`
- `ClickableButton.linkedPuzzleId`
- `ClickableButton.linkedClueImageId`
- `ClickableButton.requiredItemId`
- Puzzle Prefab `PuzzleSequenceOptionButton.optionId`
- Puzzle Prefab `PuzzleSymbolCycleSlot` setup

## Door ID Migration

Replace legacy Hallway door IDs with SecondFloorHallway door IDs.

Examples:

- `Door_Bedroom_Hallway` -> `Door_Bedroom_SecondFloorHallway`
- `Door_Hallway_Bedroom` -> `Door_SecondFloorHallway_Bedroom`
- `Door_Hallway_LivingRoom` -> `Door_SecondFloorHallway_LivingRoom`

Replace Basement door IDs with BasementStorage door IDs.

Examples:

- `Door_Kitchen_Basement` -> `Door_Kitchen_BasementStorage`
- `Door_Basement_Kitchen` -> `Door_BasementStorage_Kitchen`
- `Door_Basement_LockedRoom` -> `Door_BasementStorage_LockedRoom`

## Validator Order

1. `Escape From Nightmare / Validate Game Data`
2. `Escape From Nightmare / Validate Puzzle Prefab Contracts`
3. `Escape From Nightmare / Validate Current Scene Wiring`
4. `Escape From Nightmare / Generate Scene Wiring Report`

The JSON validator warns about legacy IDs. The Scene wiring validator catches Scene objects that still point to old IDs.
