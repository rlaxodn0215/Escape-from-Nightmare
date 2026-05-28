# Puzzle Prefab Contracts

## Common Rules

- Puzzle Prefabs should live under `Assets/Resources/PuzzleUI`.
- `PuzzleRecord.prefabPath` uses the `Resources.Load` path.
- Example file: `Assets/Resources/PuzzleUI/PuzzleUI_BedroomNumberCode.prefab`
- Example JSON path: `PuzzleUI/PuzzleUI_BedroomNumberCode`
- The Prefab root or a child must have a `PuzzleUIBase` derived component.
- A Close button should be connected whenever possible.

## NumberCode Prefab Contract

Required components:

- `PuzzleUI_BedroomNumberCode` or `PuzzleUI_KitchenNumberCode`
- `PuzzleNumberButton` for digits 0 through 9

Required fields:

- `displayText`
- `messageText`
- `submitButton`
- `clearButton`
- `backspaceButton`
- `closeButton`
- `numberButtonRoot`

Each number button needs:

- `Button`
- `PuzzleNumberButton`
- `digit = 0` through `digit = 9`

Answer data:

- Uses `puzzle_answers.json.answerText`.
- `PuzzleRecord.answerVariableName` must match `PuzzleAnswerRecord.answerVariableName`.
- `PuzzleRecord.codeLength` should match the answer text length.

## Sequence Prefab Contract

Required components:

- `PuzzleUI_ChildRoomCardOrder` or `PuzzleUI_StudyBookOrder`
- `PuzzleSequenceOptionButton`

Required fields:

- `sequenceText`
- `messageText`
- `optionButtonRoot`
- `submitButton`
- `resetButton`
- `closeButton`

Each option button needs:

- `Button`
- `PuzzleSequenceOptionButton`
- `optionId` matching one value in `puzzle_answers.json.answerSequence`

## SymbolSequence Prefab Contract

Required components:

- `PuzzleUI_LivingRoomSymbolSequence`
- `PuzzleSequenceOptionButton`

Each symbol button needs:

- `optionId = Symbol_Moon`, `Symbol_Eye`, `Symbol_Key`, or another `symbols.json.symbolId`
- Optional icon display through `symbols.json.spritePath`

Recommended setting:

- `refreshOptionsFromSymbolRecords = true`

## TODO Puzzle Types

Current TODO or future puzzle types:

- `PowerDevice`
- `FinalSequence`
- `EntranceDoor`

For now, a Prefab with a `PuzzleUIBase` derived script is enough for the first contract pass. Full logic can be implemented later.

## Validation

Run:

- `Escape From Nightmare / Validate Puzzle Prefab Contracts`

Missing real Prefabs are currently warnings because this phase checks manual wiring readiness, not asset creation.
