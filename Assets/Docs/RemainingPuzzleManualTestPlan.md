# Remaining Puzzle Manual Test Plan

## LivingRoom ItemUse

1. Obtain `OldDrawerKey`.
2. Open `Puzzle_LivingRoom_01`.
3. Select and use `OldDrawerKey`.
4. Confirm `SmallClockworkDevice` is acquired.

## BasementPowerDevice

1. Ensure `BasementFuse` is owned.
2. Ensure `SmallClockworkDevice` is owned.
3. Open `Puzzle_BasementStorage_01`.
4. Enter `Switch_Left -> Switch_Right -> Switch_Center -> Switch_Left -> Switch_Right`.
5. Press `PowerButton`.
6. Confirm `ModifiedClockworkDevice` is acquired.
7. Confirm `Door_BasementStorage_LockedRoom` is opened.
8. Confirm `BasementClueImage` is unlocked.

## LockedRoomFinal

1. Ensure `ModifiedClockworkDevice` is owned.
2. Open `Puzzle_LockedRoom_01`.
3. Set the five symbol slots to `Symbol_01`, `Symbol_03`, `Symbol_04`, `Symbol_05`, `Symbol_06`.
4. Submit.
5. Use `ModifiedClockworkDevice`.
6. Confirm `FrontDoorKey` is acquired.
7. Confirm `finalChaseStarted` is true.

## EntranceDoor

1. Ensure `FrontDoorKey` is owned.
2. Open `Puzzle_Entrance_01`.
3. Use `FrontDoorKey`.
4. Confirm Ending state is entered.
