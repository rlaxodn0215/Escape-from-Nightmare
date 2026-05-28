# Source-Aligned Puzzle Prefab Build Guide

## Common Rules

- Place Prefabs under `Assets/Resources/PuzzleUI`.
- `prefabPath` is a `Resources.Load` path without extension.
- The Prefab root or a child must have a `PuzzleUIBase` derived component.
- Use `UnityEngine.UI.Button`, `Text`, and `Image`.
- Do not use TextMeshPro.
- Do not rely on auto-generated UI objects.

## NumberCode Prefabs

Prefabs:

- `PuzzleUI_BedroomNumberCode`
- `PuzzleUI_KitchenNumberCode`

Required:

- `PuzzleUI_BedroomNumberCode` or `PuzzleUI_KitchenNumberCode` on root.
- Digit buttons 0 through 9.
- `PuzzleNumberButton.digit = 0` through `9`.
- `displayText`
- `messageText`
- `timerText`
- `submitButton`
- `clearButton`
- `backspaceButton`
- `closeButton`

Timing:

- Bedroom uses `PuzzleRecord.timeLimitSeconds = 60`.
- Kitchen uses `PuzzleRecord.timeLimitSeconds = 45`.
- Timeout makes noise, closes the UI, and temporarily locks retry until the ghost leaves.

## Sequence Prefabs

Prefabs:

- `PuzzleUI_ChildRoomCardOrder`
- `PuzzleUI_StudyBookOrder`

Required:

- `PuzzleSequenceUIBase` derived script.
- `PuzzleSequenceOptionButton` on each option.
- `sequenceText`
- `messageText`
- `submitButton`
- `resetButton`
- `closeButton`

Option IDs:

- ChildRoom uses five symbols: `Symbol_01`, `Symbol_03`, `Symbol_04`, `Symbol_05`, `Symbol_06`.
- Study uses six symbols: `Symbol_01` through `Symbol_06`.

## SymbolCycle Prefab

Prefab:

- `PuzzleUI_LivingRoomSymbolSequence`

Required:

- `PuzzleUI_LivingRoomSymbolSequence` on root.
- Five `PuzzleSymbolCycleSlot` components.
- Each slot has a `Button`.
- `submitButton`
- `resetButton`
- `closeButton`

Behavior:

- Clicking a slot cycles through `Symbol_01` through `Symbol_06`.
- Submit compares the five selected symbols with `LivingRoomSymbolSequence`.

Source derivation:

- From all 6 symbols, remove the symbol not present in the five child room cards.
- Remove that symbol from `StudyBookSymbolOrder`.
- The remaining five-symbol order is `LivingRoomSymbolSequence`.
- Current implementation reads the final answer directly from `puzzle_answers.json`.

## PowerDevice Prefab

Prefab:

- `PuzzleUI_BasementPowerDevice`

Required:

- `PuzzlePowerDeviceUIBase` derived script.
- `PuzzlePowerSwitchButton` for:
  - `Switch_Left`
  - `Switch_Right`
  - `Switch_Center`
- `powerButton`
- `resetButton`
- `closeButton`
- `inputText`
- `messageText`

Rules:

- Input length is 5.
- Requires `BasementFuse`.
- Also requires `SmallClockworkDevice`.
- Success unlocks `Door_BasementStorage_LockedRoom`.
- Success unlocks `BasementClueImage`.
- Success transforms `SmallClockworkDevice` into `ModifiedClockworkDevice`.

## LockedRoomFinal Prefab

Prefab:

- `PuzzleUI_LockedRoomFinal`

Required:

- `PuzzleUI_LockedRoomFinal` on root.
- Five `PuzzleSymbolCycleSlot` components.
- `submitButton`
- `resetButton`
- `closeButton`
- A UI Button wired manually to `PuzzleUI_LockedRoomFinal.UseRequiredDevice`.

Rules:

- First solve the five-symbol sequence.
- Then select/use `ModifiedClockworkDevice`.
- Success rewards `FrontDoorKey`.
- Success sets `finalChaseStarted = true`.

## EntranceDoor Prefab

Prefab:

- `PuzzleUI_EntranceDoor`

Required:

- `PuzzleUI_EntranceDoor`
- `useSelectedItemButton`
- `closeButton`

Rules:

- Requires `FrontDoorKey`.
- Success completes `Puzzle_Entrance_01`.
- Reward type `Ending` enters the ending flow.

## Validation

Run:

1. `Escape From Nightmare / Validate Game Data`
2. `Escape From Nightmare / Validate Puzzle Prefab Contracts`
3. `Escape From Nightmare / Validate Current Scene Wiring`

## Pass 11 Builder Support

The following five Prefabs can be generated from the Unity Editor menu:

- `Escape From Nightmare / Puzzle Prefabs / Create Missing First Five Puzzle Prefabs`
- `Escape From Nightmare / Puzzle Prefabs / Rebuild First Five Puzzle Prefabs With Backup`

The rebuild menu backs up existing Prefabs to:

- `Assets/Backups/PuzzleUI`

Builder-generated Prefabs are initial test Prefabs, not final art. After manually editing a generated Prefab, run `Validate Puzzle Prefab Contracts` again.

## First Five Prefab Mapping

| Puzzle ID | Prefab Asset | prefabPath |
|---|---|---|
| Puzzle_Bedroom_01 | PuzzleUI_BedroomNumberCode.prefab | PuzzleUI/PuzzleUI_BedroomNumberCode |
| Puzzle_Kitchen_01 | PuzzleUI_KitchenNumberCode.prefab | PuzzleUI/PuzzleUI_KitchenNumberCode |
| Puzzle_ChildRoom_01 | PuzzleUI_ChildRoomCardOrder.prefab | PuzzleUI/PuzzleUI_ChildRoomCardOrder |
| Puzzle_Study_01 | PuzzleUI_StudyBookOrder.prefab | PuzzleUI/PuzzleUI_StudyBookOrder |
| Puzzle_LivingRoom_02 | PuzzleUI_LivingRoomSymbolSequence.prefab | PuzzleUI/PuzzleUI_LivingRoomSymbolSequence |
