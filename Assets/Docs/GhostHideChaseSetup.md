# Ghost / Hide / Chase Setup Guide

## Purpose

This pass connects the first skeleton of the danger loop:

- Noise is made by puzzle failure or another scripted event.
- GhostManager reacts to the noise location after a delay.
- The ghost enters the location and increases danger while the player is there.
- When danger reaches the threshold, ChaseManager starts a chase.
- HideManager lets the player hide and wait until it is safe to exit.
- ChaseManager sends the game to GameOver when the move limit is reached.

## Manager Setup

Place these manager objects manually in GameScene:

- GameManager
- GameDataManager
- LocationManager
- InteractionManager
- InventoryManager
- PuzzleManager
- SaveManager
- NoiseManager
- GhostManager
- HideManager
- ChaseManager

LocationManager must already be connected to the manually placed LocationController and LocationView objects.

## ghost_rules.json

`ghost_rules.json` keeps the existing wrapper structure:

```json
{
  "ghostRules": []
}
```

Fields:

- `ruleId`: Rule ID.
- `locationId`: Location where this rule applies.
- `minArrivalTime`: Minimum seconds before the ghost arrives after noise.
- `maxArrivalTime`: Maximum seconds before the ghost arrives after noise.
- `minLeaveTime`: Minimum seconds before the ghost leaves after arriving.
- `maxLeaveTime`: Maximum seconds before the ghost leaves after arriving.
- `dangerIncreasePerSecond`: Danger gain per second while the ghost and player are in the same Location.

Example: bedroom noise response

```json
{
  "ghostRules": [
    {
      "ruleId": "GhostRule_Bedroom",
      "locationId": "Bedroom",
      "minArrivalTime": 5,
      "maxArrivalTime": 10,
      "minLeaveTime": 5,
      "maxLeaveTime": 20,
      "dangerIncreasePerSecond": 0.25
    }
  ]
}
```

Example: a more dangerous basement

```json
{
  "ghostRules": [
    {
      "ruleId": "GhostRule_Basement",
      "locationId": "Basement",
      "minArrivalTime": 2,
      "maxArrivalTime": 6,
      "minLeaveTime": 8,
      "maxLeaveTime": 20,
      "dangerIncreasePerSecond": 0.5
    }
  ]
}
```

These examples are documentation only. Do not replace the real data file unless you are intentionally authoring game data.

## game_settings.json

The following optional settings can override Inspector defaults when their values are greater than 0:

- `hideMinSeconds`
- `hideMaxSeconds`
- `ghostDefaultMinArrivalTime`
- `ghostDefaultMaxArrivalTime`
- `ghostDefaultMinLeaveTime`
- `ghostDefaultMaxLeaveTime`
- `ghostDangerThreshold`
- `chaseMoveLimit`

Existing `game_settings.json` can stay minimal. Missing fields are treated as default values.

## HidePoint Setup

Example: bedroom closet hide point

```text
HidePoint_Bedroom_Closet_Button
- Button
- ClickableButton
  - clickableType = HidePoint
  - targetObjectId = HidePoint_Bedroom_Closet
- HidePointController
  - hidePointId = HidePoint_Bedroom_Closet
  - usable = true
```

Click flow:

1. ClickableButton sends the click to InteractionManager.
2. InteractionManager resolves the hide point ID.
3. HideManager.EnterHidePoint starts hiding.
4. GameState changes to Hiding.
5. HideManager waits for the ghost-leave timer.
6. CanExitSafely becomes true.
7. If chase is active, ChaseManager.EndChaseByHideSuccess is called.

## HideExitButton Setup

Create a manual UI Button for leaving the hide state and add `HideExitButton`.

Recommended fields:

- `rootObject`: the button root.
- `showOnlyWhileHiding`: true.

The button calls `HideManager.ExitHidePoint()`.

## GhostStatusUI Setup

Create a development status panel manually:

```text
Canvas
- GhostStatusPanel
  - StateText
  - DangerText
  - ChaseText
  - HideText
```

Add `GhostStatusUI` to `GhostStatusPanel` and connect:

- `stateText`
- `dangerText`
- `chaseText`
- `hideText`

This UI is for debugging and may be replaced by final horror presentation later.

## Puzzle Failure Noise

Puzzle noise uses `PuzzleRecord.failCountToNoise`.

If `failCountToNoise` is 3:

1. The player submits a wrong answer 3 times.
2. PuzzleUIBase calls `NoiseManager.MakeNoise(currentLocationId, puzzleId)`.
3. NoiseManager calls `GhostManager.ReactToNoise(locationId)`.
4. GhostManager starts its delayed arrival routine.

If `failCountToNoise` is 0 or less, puzzle failures do not make noise.

## Chase Movement

During chase, LocationManager registers movement when these actions succeed:

- `SetLocation`
- `SetView`
- `RotateLeft`
- `RotateRight`

When `MoveCountDuringChase` reaches `maxMovesBeforeCatch`, ChaseManager calls `GameManager.GameOver()`.

Moving while hidden does not increase the chase move count.

## Test Scenario

1. Place all managers in GameScene.
2. Set up at least one LocationController and LocationView, for example `Bedroom` and `Bedroom_Front`.
3. Add a HidePoint button with ClickableButton and HidePointController.
4. Add a HideExitButton.
5. Add GhostStatusUI and connect Text fields.
6. Trigger `NoiseManager.MakeNoiseAtCurrentLocation("TestNoise")` from a temporary manual test button or Inspector call.
7. Watch GhostStatusUI change from Patrolling to RespondingToNoise to SearchingLocation.
8. Stay in the same Location until danger reaches the threshold.
9. Confirm ChaseManager starts chasing.
10. Hide, wait until safe, then press HideExitButton.
11. Repeat without hiding and move until GameOver.

## Notes

- This pass does not create ghost sprites, ghost prefabs, animation, pathfinding, or sound playback.
- HidePoint and UI objects are placed manually in the Unity Editor.
- The real `ghost_rules.json` and `game_settings.json` files are not overwritten by this setup.
