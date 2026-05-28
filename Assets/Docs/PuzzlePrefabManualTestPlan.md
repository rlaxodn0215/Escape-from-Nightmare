# Puzzle Prefab Manual Test Plan

## A. Builder Creation Test

1. Run Unity Editor menu:
   - `Escape From Nightmare / Puzzle Prefabs / Create Missing First Five Puzzle Prefabs`
2. Confirm five Prefabs exist under `Assets/Resources/PuzzleUI`.
3. Confirm Console has no Builder errors.

## B. Contract Validator Test

1. Run:
   - `Escape From Nightmare / Validate Puzzle Prefab Contracts`
2. Confirm Error count is 0.
3. Review any Warnings.

## C. BedroomNumberCode Play Mode Test

1. In GameScene, connect `PuzzleManager.puzzleUiRoot`.
2. Configure Bedroom puzzle button:
   - `clickableType = Puzzle`
   - `linkedPuzzleId = Puzzle_Bedroom_01`
3. Enter Play Mode.
4. Click the puzzle button.
5. Confirm `PuzzleUI_BedroomNumberCode` loads.
6. Enter `7319`.
7. Press Submit.
8. Confirm `OldDrawerKey` is added.

## D. KitchenNumberCode Play Mode Test

1. Configure Kitchen puzzle button:
   - `linkedPuzzleId = Puzzle_Kitchen_01`
2. Enter `4826`.
3. Press Submit.
4. Confirm `BasementFuse` is added.

## E. ChildRoom Sequence Test

1. Configure button:
   - `linkedPuzzleId = Puzzle_ChildRoom_01`
2. Input:
   - `Symbol_01`
   - `Symbol_03`
   - `Symbol_04`
   - `Symbol_05`
   - `Symbol_06`
3. Confirm completion.
4. Confirm `ChildRoomCardSymbolClueImage` is unlocked.

## F. Study Sequence Test

1. Configure button:
   - `linkedPuzzleId = Puzzle_Study_01`
2. Input:
   - `Symbol_01`
   - `Symbol_02`
   - `Symbol_03`
   - `Symbol_04`
   - `Symbol_05`
   - `Symbol_06`
3. Confirm completion.
4. Confirm `StudyBookSymbolClueImage` is unlocked.

## G. LivingRoom SymbolCycle Test

1. Configure button:
   - `linkedPuzzleId = Puzzle_LivingRoom_02`
2. Cycle five slots to the answer sequence.
3. Press Submit.
4. Confirm `KitchenCodeClueImage` is unlocked.

## H. Wrong Answer Test

- Enter wrong input in each puzzle.
- Confirm `failCountToNoise` eventually calls `NoiseManager.MakeNoise`.

## I. Number Puzzle Timeout Test

- Bedroom timeout: 60 seconds.
- Kitchen timeout: 45 seconds.
- Confirm timeout closes the puzzle and locks retry.
- Confirm retry is unlocked after the ghost leaves.

## 1. Puzzle_Bedroom_01

- Open `PuzzleUI_BedroomNumberCode`.
- Enter `7319`.
- Confirm `OldDrawerKey` is added.
- Enter wrong input 3 times.
- Confirm noise is made.
- Let the 60-second timer expire.
- Confirm noise, UI close, and retry lock until ghost leaves.

## 2. Puzzle_ChildRoom_01

- Open `PuzzleUI_ChildRoomCardOrder`.
- Click `Symbol_01`, `Symbol_03`, `Symbol_04`, `Symbol_05`, `Symbol_06`.
- Confirm completion.
- Confirm `ChildRoomCardSymbolClueImage` unlocks.
- Test wrong order and confirm noise after configured failures.

## 3. Puzzle_Study_01

- Open `PuzzleUI_StudyBookOrder`.
- Click `Symbol_01`, `Symbol_02`, `Symbol_03`, `Symbol_04`, `Symbol_05`, `Symbol_06`.
- Confirm completion.
- Confirm `StudyBookSymbolClueImage` unlocks.

## 4. Puzzle_LivingRoom_01

- Select `OldDrawerKey`.
- Use it on the configured living room target.
- Confirm `SmallClockworkDevice` is acquired.
- Confirm `OldDrawerKey` is consumed.

## 5. Puzzle_LivingRoom_02

- Open `PuzzleUI_LivingRoomSymbolSequence`.
- Cycle five slots to:
  - `Symbol_01`
  - `Symbol_03`
  - `Symbol_04`
  - `Symbol_05`
  - `Symbol_06`
- Submit.
- Confirm `KitchenCodeClueImage` unlocks.

## 6. Puzzle_Kitchen_01

- Open `PuzzleUI_KitchenNumberCode`.
- Enter `4826`.
- Confirm `BasementFuse` is acquired.
- Let the 45-second timer expire.
- Confirm noise, UI close, and retry lock.

## 7. Puzzle_BasementStorage_01

- Confirm player owns `BasementFuse`.
- Confirm player owns `SmallClockworkDevice`.
- Input:
  - `Switch_Left`
  - `Switch_Right`
  - `Switch_Center`
  - `Switch_Left`
  - `Switch_Right`
- Press Power.
- Confirm `Door_BasementStorage_LockedRoom` unlocks.
- Confirm `BasementClueImage` unlocks.
- Confirm `BasementFuse` is consumed.
- Confirm `SmallClockworkDevice` becomes `ModifiedClockworkDevice`.

## 8. Puzzle_LockedRoom_01

- Cycle five slots to:
  - `Symbol_01`
  - `Symbol_03`
  - `Symbol_04`
  - `Symbol_05`
  - `Symbol_06`
- Submit.
- Select `ModifiedClockworkDevice`.
- Press the button wired to `UseRequiredDevice`.
- Confirm `FrontDoorKey` is acquired.
- Confirm `finalChaseStarted` is true.

## 9. Puzzle_Entrance_01

- Select `FrontDoorKey`.
- Press the use button.
- Confirm puzzle completes.
- Confirm Ending flow starts.

## Acceptable Current Warnings

- Missing Prefab warning until Prefabs are manually created.
- Missing Sprite warning until Sprite assets are manually added.

## Must Fix

- Missing ID references.
- Wrong answerVariableName.
- Missing required buttons on a created Prefab.
- Symbol option IDs that do not match `Symbol_01` through `Symbol_06`.
