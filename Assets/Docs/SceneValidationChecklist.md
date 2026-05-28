# Scene Validation Checklist

## JSON

- [ ] Run `Validate Game Data`.
- [ ] Error count is 0.
- [ ] No duplicate IDs.
- [ ] No missing ID references.

## Build Settings

- [ ] `TitleScene` is registered.
- [ ] `GameScene` is registered.
- [ ] Scene names match exactly.

## TitleScene

- [ ] `GameManager` exists.
- [ ] `SaveManager` exists.
- [ ] `TitleMenuUI` exists.
- [ ] `NewGameButton` connected.
- [ ] `ContinueButton` connected.
- [ ] `DeleteSaveButton` connected.
- [ ] `QuitButton` connected.
- [ ] `StatusText` connected.

## GameScene Managers

- [ ] `GameDataManager`
- [ ] `LocationManager`
- [ ] `InteractionManager`
- [ ] `InventoryManager`
- [ ] `PuzzleManager`
- [ ] `EndingManager`
- [ ] `NoiseManager`
- [ ] `GhostManager`
- [ ] `HideManager`
- [ ] `ChaseManager`
- [ ] `ClueImageManager`

## Location / View

- [ ] `LocationRoot` connected.
- [ ] `Bedroom` placed.
- [ ] `Hallway` placed.
- [ ] `ChildRoom` placed.
- [ ] `Study` placed.
- [ ] `LivingRoom` placed.
- [ ] `Kitchen` placed.
- [ ] `Entrance` placed.
- [ ] Four directional Views placed for each main Location.
- [ ] `LocationController.locationId` values checked.
- [ ] `LocationView.viewId` values checked.

## Navigation

- [ ] `RotateLeft` button connected.
- [ ] `RotateRight` button connected.

## Door Buttons

- [ ] `Door_Bedroom_Hallway`
- [ ] `Door_Hallway_ChildRoom`
- [ ] `Door_Hallway_Study`
- [ ] `Door_Hallway_LivingRoom`
- [ ] `Door_LivingRoom_Kitchen`
- [ ] `Door_LivingRoom_Entrance`

## Puzzle Buttons

- [ ] `Puzzle_Bedroom_01`
- [ ] `Puzzle_ChildRoom_01`
- [ ] `Puzzle_Study_01`
- [ ] `Puzzle_LivingRoom_01`
- [ ] `Puzzle_Kitchen_01`

## Puzzle Prefabs

- [ ] `PuzzleUI_BedroomNumberCode`
- [ ] `PuzzleUI_ChildRoomCardOrder`
- [ ] `PuzzleUI_StudyBookOrder`
- [ ] `PuzzleUI_LivingRoomSymbolSequence`
- [ ] `PuzzleUI_KitchenNumberCode`

## Inventory UI

- [ ] `InventoryBarUI` connected.
- [ ] At least 5 slots.
- [ ] `selectedIndicator` connected.

## Clue UI

- [ ] `ClueImagePanelUI` connected.
- [ ] `ClueImageManager.clueImagePanel` connected.

## GameOver / Ending

- [ ] `GameOverPanelUI` connected.
- [ ] `EndingPanelUI` connected.
- [ ] `EndingManager.endingPanel` connected.

## Ghost / Hide / Chase

- [ ] `HideExitButton` connected.
- [ ] At least one HidePoint button.
- [ ] `GhostStatusUI` connected.
- [ ] `NoiseManager` exists.
- [ ] `GhostManager` exists.
- [ ] `HideManager` exists.
- [ ] `ChaseManager` exists.

## Validation Menus

- [ ] Run `Validate Current Scene Wiring`.
- [ ] Run `Validate Puzzle Prefab Contracts`.
- [ ] Run `Generate Scene Wiring Report`.

## Pass Criteria

- [ ] Error count is 0.
- [ ] Warnings are reviewed and accepted or fixed.
