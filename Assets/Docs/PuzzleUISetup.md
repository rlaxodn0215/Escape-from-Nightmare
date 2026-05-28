# Puzzle UI Setup Guide

## 1. System purpose

This system provides reusable input logic for:

- Number code puzzles
- Sequence order puzzles
- Symbol sequence puzzles
- Answer lookup from `Assets/StreamingAssets/Data/puzzle_answers.json`

Puzzle Prefabs and UI objects are still created manually in the Unity Editor.

## 2. Number code puzzle setup

Example Prefab hierarchy:

```text
PuzzleUI_BedroomNumberCode
- Root
  - DisplayText
  - MessageText
  - NumberButtons
    - Button_0
    - Button_1
    - Button_2
    - Button_3
    - Button_4
    - Button_5
    - Button_6
    - Button_7
    - Button_8
    - Button_9
  - SubmitButton
  - ClearButton
  - BackspaceButton
  - CloseButton
```

Required components:

- Add `PuzzleUI_BedroomNumberCode` to the root.
- Add `PuzzleNumberButton` to each number button.
- Set each `PuzzleNumberButton.digit` from `0` to `9`.
- Connect `displayText`, `messageText`, `numberButtonRoot`, `submitButton`, `clearButton`, `backspaceButton`, and `closeButton`.

`PuzzleUI_KitchenNumberCode` uses the same setup.

## 3. Sequence puzzle setup

Example Prefab hierarchy:

```text
PuzzleUI_StudyBookOrder
- Root
  - SequenceText
  - MessageText
  - Options
    - Button_Book_Red
    - Button_Book_Blue
    - Button_Book_Green
  - SubmitButton
  - ResetButton
  - CloseButton
```

Required components:

- Add `PuzzleUI_StudyBookOrder` to the root.
- Add `PuzzleSequenceOptionButton` to each option button.
- Set `optionId` to values such as `Book_Red`, `Book_Blue`, and `Book_Green`.
- Connect `sequenceText`, `messageText`, `optionButtonRoot`, `submitButton`, `resetButton`, and `closeButton`.

`PuzzleUI_ChildRoomCardOrder` uses the same setup.

## 4. Symbol sequence puzzle setup

Example Prefab hierarchy:

```text
PuzzleUI_LivingRoomSymbolSequence
- Root
  - SequenceText
  - MessageText
  - SymbolOptions
    - Button_Symbol_Moon
    - Button_Symbol_Eye
    - Button_Symbol_Key
  - ResetButton
  - CloseButton
```

Required components:

- Add `PuzzleUI_LivingRoomSymbolSequence` to the root.
- Add `PuzzleSequenceOptionButton` to each symbol button.
- Set `optionId` to values such as `Symbol_Moon`.
- You can enable `refreshOptionsFromSymbolRecords`.
- If `symbols.json` contains `spritePath`, the option button can load a symbol sprite from `Resources`.

## 5. puzzle_answers.json format

`answerVariableName` should match `PuzzleRecord.answerVariableName`.

Number code example:

```json
{
  "answers": [
    {
      "puzzleId": "Puzzle_Bedroom_01",
      "answerVariableName": "BedroomCode",
      "answerText": "7319",
      "answerSequence": [],
      "caseSensitive": false,
      "ignoreWhitespace": true
    }
  ]
}
```

Sequence example:

```json
{
  "answers": [
    {
      "puzzleId": "Puzzle_Study_01",
      "answerVariableName": "StudyBookOrder",
      "answerText": "",
      "answerSequence": [
        "Book_Red",
        "Book_Blue",
        "Book_Green"
      ],
      "caseSensitive": false,
      "ignoreWhitespace": true
    }
  ]
}
```

Symbol sequence example:

```json
{
  "answers": [
    {
      "puzzleId": "Puzzle_LivingRoom_01",
      "answerVariableName": "LivingRoomSymbolSequence",
      "answerText": "",
      "answerSequence": [
        "Symbol_Moon",
        "Symbol_Eye",
        "Symbol_Key"
      ],
      "caseSensitive": false,
      "ignoreWhitespace": true
    }
  ]
}
```

## 6. puzzles.json connection example

```json
{
  "puzzleId": "Puzzle_Bedroom_01",
  "locationId": "Bedroom",
  "type": "NumberCode",
  "prefabPath": "PuzzleUI/PuzzleUI_BedroomNumberCode",
  "codeLength": 4,
  "answerVariableName": "BedroomCode",
  "timeLimitSeconds": 0,
  "failCountToNoise": 3,
  "rewardType": "Item",
  "rewardId": "FrontDoorKey",
  "requiredItemId": "",
  "startsSolved": false
}
```

## 7. Failure and noise

- `PuzzleRecord.failCountToNoise = 3` means the third wrong answer calls `NoiseManager.MakeNoise`.
- `failCountToNoise = 0` disables puzzle failure noise.
- Noise uses the current `LocationManager.CurrentLocationId` when available.

## 8. Notes

- This step does not create actual Prefabs.
- This step does not fill real answer data.
- UI Button references must be connected manually in the Unity Editor.
- Use `UnityEngine.UI.Text`, not TextMeshPro.
