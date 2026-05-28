# Source Alignment Patch

## Purpose

This patch aligns the temporary integration JSON from pass 8 with the source planning names and progression.

It does not create Scenes, Prefabs, Sprites, Audio assets, or UI objects. Unity Editor wiring still needs to be updated manually.

## Main Differences From Pass 8

- `Hallway` is now `SecondFloorHallway`.
- `Basement` is now `BasementStorage`.
- Basement, LockedRoom, and Entrance puzzle IDs are source-aligned.
- Symbols are now generic `Symbol_01` through `Symbol_06`.
- Kitchen no longer rewards `FrontDoorKey`.
- `FrontDoorKey` is now rewarded by the locked room final puzzle.
- LivingRoom now has two puzzle concepts:
  - `Puzzle_LivingRoom_01`: item-use puzzle that rewards `SmallClockworkDevice`.
  - `Puzzle_LivingRoom_02`: symbol cycle puzzle that unlocks `KitchenCodeClueImage`.

## Location ID Changes

- `Hallway` -> `SecondFloorHallway`
- `Basement` -> `BasementStorage`

## Puzzle ID Changes

- `Puzzle_BasementPower_01` -> `Puzzle_BasementStorage_01`
- `Puzzle_LockedRoomFinal_01` -> `Puzzle_LockedRoom_01`
- `Puzzle_EntranceDoor_01` -> `Puzzle_Entrance_01`

Prefab names stay the same. A Puzzle ID and Prefab name do not need to match.

Examples:

- `Puzzle_BasementStorage_01` uses `PuzzleUI/PuzzleUI_BasementPowerDevice`
- `Puzzle_LockedRoom_01` uses `PuzzleUI/PuzzleUI_LockedRoomFinal`
- `Puzzle_Entrance_01` uses `PuzzleUI/PuzzleUI_EntranceDoor`

## Item Reward Flow

- `Puzzle_Bedroom_01` rewards `OldDrawerKey`.
- `Puzzle_LivingRoom_01` rewards `SmallClockworkDevice`.
- `Puzzle_Kitchen_01` rewards `BasementFuse`.
- `Puzzle_BasementStorage_01` consumes `BasementFuse` and transforms `SmallClockworkDevice` into `ModifiedClockworkDevice`.
- `Puzzle_LockedRoom_01` consumes `ModifiedClockworkDevice` and rewards `FrontDoorKey`.
- `Puzzle_Entrance_01` uses `FrontDoorKey` and enters Ending.

## Why Kitchen No Longer Rewards FrontDoorKey

The source plan places `FrontDoorKey` behind the locked room final puzzle. Kitchen provides `BasementFuse`, which is needed for the basement power device. This keeps the late-game chain intact.

## BasementStorage And LockedRoom Flow

1. Get `BasementFuse` from Kitchen.
2. Get `SmallClockworkDevice` from LivingRoom item-use puzzle.
3. Use both on `Puzzle_BasementStorage_01`.
4. Unlock `Door_BasementStorage_LockedRoom`.
5. Gain `ModifiedClockworkDevice`.
6. Use it in `Puzzle_LockedRoom_01`.
7. Receive `FrontDoorKey` and start final chase state.

## Asset Status

No actual Prefabs, Sprites, Audio clips, Scenes, or UI objects were created by this patch. Missing Prefab/Sprite warnings from validators are expected until manual asset work is done.
