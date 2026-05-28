# Manual Scene Wiring Checklist

## TitleScene

- [ ] GameManager placed.
- [ ] SaveManager placed.
- [ ] TitleMenuUI placed.
- [ ] NewGameButton connected.
- [ ] ContinueButton connected.
- [ ] DeleteSaveButton connected.
- [ ] QuitButton connected.
- [ ] StatusText connected.

## GameScene Managers

- [ ] GameDataManager
- [ ] LocationManager
- [ ] InteractionManager
- [ ] InventoryManager
- [ ] PuzzleManager
- [ ] SaveManager or DontDestroyOnLoad SaveManager from TitleScene
- [ ] EndingManager
- [ ] NoiseManager
- [ ] GhostManager
- [ ] HideManager
- [ ] ChaseManager
- [ ] ClueImageManager

## LocationRoot

For each Location:

- [ ] Add LocationController.
- [ ] Set `locationId`.
- [ ] Set `defaultViewId`.
- [ ] Add child View objects.
- [ ] Add LocationView to each View.
- [ ] Set each `viewId`.

IDs must match `locations.json`.

## Navigation Buttons

- [ ] Button_Left
  - [ ] NavigationButton.actionType = RotateLeft
- [ ] Button_Right
  - [ ] NavigationButton.actionType = RotateRight

## Door Buttons

Example: Bedroom to Hallway

- [ ] Button component.
- [ ] ClickableButton.clickableType = Door.
- [ ] linkedDoorId = Door_Bedroom_Hallway.

Door button `linkedDoorId` must match `doors.json`.

## Puzzle Buttons

Example: Bedroom puzzle

- [ ] Button component.
- [ ] ClickableButton.clickableType = Puzzle.
- [ ] linkedPuzzleId = Puzzle_Bedroom_01.

Puzzle button `linkedPuzzleId` must match `puzzles.json`.

## ExamineImage Buttons

Example: Bedroom photo

- [ ] Button component.
- [ ] ClickableButton.clickableType = ExamineImage.
- [ ] linkedClueImageId = Clue_Bedroom_Photo.

Clue ID must match `clues.json`.

## UseItemTarget Buttons

Example: Locked drawer

- [ ] Button component.
- [ ] ClickableButton.clickableType = UseItemTarget.
- [ ] requiredItemId = OldDrawerKey.
- [ ] linkedClueImageId = Clue_LockedDrawer_Note.

Item and clue IDs must match `items.json` and `clues.json`.

## HidePoint Buttons

Example: Bedroom closet

- [ ] Button component.
- [ ] ClickableButton.clickableType = HidePoint.
- [ ] targetObjectId = HidePoint_Bedroom_Closet.
- [ ] HidePointController added.
- [ ] HidePointController.hidePointId = HidePoint_Bedroom_Closet.

## FinalDoor Buttons

Direct ending:

- [ ] ClickableButton.clickableType = FinalDoor.
- [ ] requiredItemId = FrontDoorKey.
- [ ] linkedPuzzleId empty.
- [ ] linkedDoorId empty.

Puzzle ending:

- [ ] ClickableButton.clickableType = FinalDoor.
- [ ] requiredItemId = FrontDoorKey.
- [ ] linkedPuzzleId = Puzzle_EntranceDoor_01.

## Inventory UI

- [ ] InventoryBarUI placed.
- [ ] InventorySlotUI slots placed manually.
- [ ] iconImage connected.
- [ ] labelText connected.
- [ ] selectedIndicator connected.
- [ ] emptyRoot and filledRoot connected if used.

## ClueImagePanel

- [ ] ClueImagePanelUI placed.
- [ ] clueImage connected.
- [ ] titleText connected.
- [ ] descriptionText connected.
- [ ] messageText connected.
- [ ] closeButton connected.
- [ ] ClueImageManager.clueImagePanel connected.

## PuzzleUIRoot

- [ ] PuzzleManager.puzzleUiRoot connected.
- [ ] Puzzle UI prefabs placed manually under `Assets/Resources/PuzzleUI`.
- [ ] Puzzle prefab names match `puzzles.json` prefabPath values.

## GameOverPanel

- [ ] GameOverPanelUI placed.
- [ ] restartButton connected.
- [ ] returnTitleButton connected.

## EndingPanel

- [ ] EndingPanelUI placed.
- [ ] EndingManager.endingPanel connected.
- [ ] skipButton connected if used.

## Ghost UI

- [ ] GhostStatusUI placed.
- [ ] stateText connected.
- [ ] dangerText connected.
- [ ] chaseText connected.
- [ ] hideText connected.

## Build Settings

- [ ] TitleScene registered.
- [ ] GameScene registered.
- [ ] Scene names exactly match `TitleScene` and `GameScene`, unless overridden in `game_settings.json`.

## Validator Criteria

Pass criteria:

- Error count is 0.
- Warnings are only for missing Sprite or Prefab Resources during this stage.

Fix immediately:

- JSON parse failure.
- Missing JSON file.
- Duplicate ID.
- Missing Location reference.
- Missing View reference.
- Missing Door reference.
- Missing Puzzle reference.
- Missing Item reference.
- Missing Clue reference.
- Missing Symbol reference.
