# Editor Wiring Step By Step

## Goal

Connect the JSON data set to manually placed Unity Editor objects.

This phase does not auto-create scenes, prefabs, UI objects, Location/View objects, or buttons. It assumes you place them by hand, then use the validation menus to find wiring mistakes quickly.

## Recommended Order

### Step 1. Validate JSON

- Run `Escape From Nightmare / Validate Game Data`.
- Confirm `Errors: 0`.
- Missing Sprite or Prefab warnings are acceptable until real assets are added.

### Step 2. Wire TitleScene

- Add `GameManager`.
- Add `SaveManager`.
- Add a `Canvas`.
- Add `TitleMenuUI`.
- Connect `NewGameButton`.
- Connect `ContinueButton`.
- Connect `DeleteSaveButton`.
- Connect `QuitButton`.
- Connect `StatusText`.

### Step 3. Wire GameScene Managers

- Add `GameDataManager`.
- Add `LocationManager`.
- Add `InteractionManager`.
- Add `InventoryManager`.
- Add `PuzzleManager`.
- Add `EndingManager`.
- Add `NoiseManager`.
- Add `GhostManager`.
- Add `HideManager`.
- Add `ChaseManager`.
- Add `ClueImageManager`.

`GameManager` and `SaveManager` may come from TitleScene through `DontDestroyOnLoad`, but placing duplicates should be avoided unless the singleton setup is verified.

### Step 4. Create LocationRoot

- Create `LocationRoot` under the GameScene Canvas or the scene UI root.
- Connect it to `LocationManager.locationRoot`.
- Create Location objects such as `Location_Bedroom`.
- Add `LocationController`.
- Set `locationId`.
- Set `defaultViewId`.

### Step 5. Create Views

For each Location, create four View objects:

- `View_Bedroom_Front`
- `View_Bedroom_Right`
- `View_Bedroom_Back`
- `View_Bedroom_Left`

Add `LocationView` to each View and set `viewId` exactly as written in `locations.json`.

### Step 6. Wire Navigation Buttons

- Add a left rotation button with `NavigationButton.actionType = RotateLeft`.
- Add a right rotation button with `NavigationButton.actionType = RotateRight`.
- Optional direct navigation buttons may use `SetLocation` or `SetView`.

### Step 7. Wire Door Buttons

- Add `Button`.
- Add `ClickableButton`.
- Set `clickableType = Door`.
- Set `linkedDoorId` from `doors.json`.

### Step 8. Wire Puzzle Buttons

- Add `Button`.
- Add `ClickableButton`.
- Set `clickableType = Puzzle`.
- Set `linkedPuzzleId` from `puzzles.json`.

### Step 9. Wire ExamineImage Buttons

- Add `Button`.
- Add `ClickableButton`.
- Set `clickableType = ExamineImage`.
- Set `linkedClueImageId` from `clues.json`.

### Step 10. Wire Inventory UI

- Add `InventoryBarUI`.
- Connect `slotRoot`.
- Add `InventorySlotUI` to each slot.
- Connect `iconImage`.
- Connect `labelText`.
- Connect `selectedIndicator`.
- Connect `emptyRoot` and `filledRoot` if used.

### Step 11. Wire ClueImagePanel

- Add `ClueImagePanelUI`.
- Connect `clueImage`.
- Connect `titleText`.
- Connect `descriptionText`.
- Connect `messageText`.
- Connect `closeButton`.
- Connect `ClueImageManager.clueImagePanel`.

### Step 12. Wire PuzzleUIRoot

- Create the puzzle UI parent object.
- Connect it to `PuzzleManager.puzzleUiRoot`.

### Step 13. Wire GameOverPanel and EndingPanel

- Add `GameOverPanelUI`.
- Connect `restartButton`.
- Connect `returnTitleButton`.
- Add `EndingPanelUI`.
- Connect `EndingManager.endingPanel`.

### Step 14. Wire Ghost / Hide UI

- Add `HideExitButton`.
- Add at least one `HidePointController`.
- Add `GhostStatusUI`.
- Connect `stateText`, `dangerText`, `chaseText`, and `hideText`.

### Step 15. Validate Scene Wiring

- Run `Escape From Nightmare / Validate Current Scene Wiring`.

### Step 16. Validate Puzzle Prefab Contracts

- Run `Escape From Nightmare / Validate Puzzle Prefab Contracts`.

### Step 17. Generate Scene Wiring Report

- Run `Escape From Nightmare / Generate Scene Wiring Report`.
- Review `Assets/Docs/GeneratedSceneWiringReport.md`.

## Pass Criteria

- `Validate Game Data`: Error 0.
- `Validate Current Scene Wiring`: Error 0.
- `Validate Puzzle Prefab Contracts`: required puzzle Error 0.
- Warnings are reviewed and either fixed or accepted intentionally.

## Common Mistakes

- Door `linkedDoorId` typo.
- Puzzle `linkedPuzzleId` typo.
- `LocationView.viewId` typo.
- Sequence puzzle `optionId` does not match `answerSequence`.
- `PuzzleManager.puzzleUiRoot` not connected.
- `ClueImageManager.clueImagePanel` not connected.
- TitleScene or GameScene missing from Build Settings.
