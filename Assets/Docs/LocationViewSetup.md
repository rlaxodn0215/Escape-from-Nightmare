# Location / View Setup Guide

This guide describes how to manually set up Location, View, and Button navigation objects in the Unity Editor.

## 1. GameScene basic structure example

```text
GameScene
- Managers
  - GameManager
  - GameDataManager
  - LocationManager
  - InteractionManager
  - InventoryManager
  - PuzzleManager
  - SaveManager
- Canvas
  - LocationRoot
    - Location_Bedroom
      - View_Bedroom_Front
      - View_Bedroom_Right
      - View_Bedroom_Back
      - View_Bedroom_Left
  - NavigationButtons
    - Button_Left
    - Button_Right
```

## 2. LocationController setup

- Add `LocationController` to `Location_Bedroom`.
- Set `locationId` to `Bedroom`.
- Set `defaultViewId` to `Bedroom_Front`.
- Register child `LocationView` objects in the `views` list.
- You can also use `CacheViews` to collect child views automatically.

## 3. LocationView setup

- Add `LocationView` to `View_Bedroom_Front`.
- Set `viewId` to `Bedroom_Front`.
- Set `rootObject` to the same GameObject, or leave it empty so the script assigns itself.

## 4. LocationManager setup

- Assign `Canvas/LocationRoot` to `locationRoot`.
- Set `collectLocationsFromRoot` to `true`.
- Set `startingLocationId` to `Bedroom`.
- Set `startingViewId` to `Bedroom_Front`.

## 5. Left and right rotation buttons

- Add `NavigationButton` to `Button_Left`.
- Set `actionType` to `RotateLeft`.
- Add `NavigationButton` to `Button_Right`.
- Set `actionType` to `RotateRight`.

## 6. Door button setup

- Add `ClickableButton` to the UI Button that acts as a door.
- Set `clickableType` to `Door`.
- Set `linkedDoorId` to `Door_Bedroom_Hallway`.
- Add the matching `DoorRecord` to `doors.json`.

## 7. doors.json example

The JSON below is an example only. Do not replace the real `Assets/StreamingAssets/Data/doors.json` with this unless you intentionally want this data.

```json
{
  "doors": [
    {
      "doorId": "Door_Bedroom_Hallway",
      "fromLocationId": "Bedroom",
      "fromViewId": "Bedroom_Front",
      "toLocationId": "Hallway",
      "toViewId": "Hallway_Back",
      "requiredItemId": "",
      "requiredPuzzleId": "",
      "startsLocked": false,
      "staysUnlockedAfterOpen": true
    }
  ]
}
```

`toViewId` is optional. If it is empty, `LocationManager` uses the target location's default view or first available view.
