# Full Game Route Manual Playtest Checklist

## New Game

- [ ] Start New Game from `TitleScene`
- [ ] Confirm start location is `Bedroom`

## Bedroom

- [ ] Check Bedroom code clue
- [ ] Solve `Puzzle_Bedroom_01`
- [ ] Confirm `OldDrawerKey`

## LivingRoom ItemUse

- [ ] Use `OldDrawerKey`
- [ ] Confirm `SmallClockworkDevice`

## ChildRoom

- [ ] Solve `Puzzle_ChildRoom_01`
- [ ] Confirm `ChildRoomCardSymbolClueImage`

## Study

- [ ] Solve `Puzzle_Study_01`
- [ ] Confirm `StudyBookSymbolClueImage`

## LivingRoom Symbol

- [ ] Solve `Puzzle_LivingRoom_02`
- [ ] Confirm `KitchenCodeClueImage`

## Kitchen

- [ ] Solve `Puzzle_Kitchen_01`
- [ ] Confirm `BasementFuse`
- [ ] Confirm `FrontDoorKey` is not granted here

## BasementStorage

- [ ] Confirm `BasementFuse` and `SmallClockworkDevice`
- [ ] Solve `Puzzle_BasementStorage_01`
- [ ] Confirm `Door_BasementStorage_LockedRoom` opened
- [ ] Confirm `BasementClueImage`
- [ ] Confirm `ModifiedClockworkDevice`

## LockedRoom

- [ ] Solve `Puzzle_LockedRoom_01`
- [ ] Use `ModifiedClockworkDevice`
- [ ] Confirm `FrontDoorKey`
- [ ] Confirm `finalChaseStarted`

## Entrance

- [ ] Use `FrontDoorKey`
- [ ] Confirm Ending state

## Save / Continue

- [ ] Save midway
- [ ] Continue restores location, items, puzzles, doors, clues, and final chase state

## GameOver / Chase

- [ ] Confirm chase state
- [ ] Confirm `GameOverPanelUI`
- [ ] Confirm Restart
