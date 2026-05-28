# Integration Test Scenario

## A. Preparation

1. Add scenes to Build Settings:
   - TitleScene
   - GameScene
2. Place minimum TitleScene objects:
   - GameManager
   - SaveManager
   - TitleMenuUI
3. Place minimum GameScene managers:
   - GameDataManager
   - LocationManager
   - InteractionManager
   - InventoryManager
   - PuzzleManager
   - SaveManager or the DontDestroyOnLoad SaveManager from TitleScene
   - EndingManager
   - NoiseManager
   - GhostManager
   - HideManager
   - ChaseManager
   - ClueImageManager
4. To test real puzzle opening, add these prefabs manually under `Assets/Resources/PuzzleUI`:
   - PuzzleUI_BedroomNumberCode
   - PuzzleUI_ChildRoomCardOrder
   - PuzzleUI_StudyBookOrder
   - PuzzleUI_LivingRoomSymbolSequence
   - PuzzleUI_KitchenNumberCode

This pass does not create prefabs. Missing puzzle prefabs should produce warnings.

## B. Data Validation

1. Run the Unity menu:
   - Escape From Nightmare / Validate Game Data
2. Expected result:
   - ID reference errors should be 0.
   - Missing Resources sprite/prefab warnings are allowed in this stage.
   - Reduce warnings after real prefabs and sprites are connected.

## C. New Game

1. Start from TitleScene.
2. Click New Game.
3. Confirm GameScene loads.
4. Confirm Bedroom starts.
5. Confirm `save_data.json` is created.

## D. Bedroom

1. On Bedroom_Front, place a photo button:
   - ClickableButton.clickableType = ExamineImage
   - linkedClueImageId = Clue_Bedroom_Photo
2. Click the photo.
3. Confirm ClueImagePanel shows the 7319 hint.
4. Place a bedroom puzzle button:
   - ClickableButton.clickableType = Puzzle
   - linkedPuzzleId = Puzzle_Bedroom_01
5. Open the puzzle.
6. Enter 7319.
7. Confirm Puzzle_Bedroom_01 completes.
8. Confirm OldDrawerKey is acquired.
9. Confirm InventoryBar shows OldDrawerKey.

## E. OldDrawerKey Use

1. Place a locked drawer button:
   - ClickableButton.clickableType = UseItemTarget
   - requiredItemId = OldDrawerKey
   - linkedClueImageId = Clue_LockedDrawer_Note
2. Select OldDrawerKey in inventory.
3. Click the drawer.
4. Confirm Clue_LockedDrawer_Note appears.
5. Confirm OldDrawerKey is removed because it is consumable.

## F. Movement

1. On Bedroom_Front, place a door button:
   - ClickableButton.clickableType = Door
   - linkedDoorId = Door_Bedroom_Hallway
2. Confirm it moves to Hallway_Back.
3. On Hallway_Left, place a ChildRoom door button:
   - linkedDoorId = Door_Hallway_ChildRoom
4. Confirm ChildRoom_Front loads.

## G. ChildRoom Puzzle

1. Place a ChildRoom puzzle button:
   - ClickableButton.clickableType = Puzzle
   - linkedPuzzleId = Puzzle_ChildRoom_01
2. Open the puzzle.
3. Choose Card_Red -> Card_Blue -> Card_Green.
4. Confirm Puzzle_ChildRoom_01 completes.
5. Confirm Door_Hallway_Study is saved as opened.

## H. Study

1. On Hallway_Right, place the Study door:
   - linkedDoorId = Door_Hallway_Study
2. Confirm Study opens.
3. Place Study puzzle button:
   - linkedPuzzleId = Puzzle_Study_01
4. Choose Book_Red -> Book_Blue -> Book_Green -> Book_Black.
5. Confirm Clue_BasementPassword unlocks.
6. Place a clue button and confirm the 4826 kitchen code clue.

## I. LivingRoom

1. On Hallway_Front, place LivingRoom door:
   - linkedDoorId = Door_Hallway_LivingRoom
2. Place LivingRoom symbol puzzle:
   - linkedPuzzleId = Puzzle_LivingRoom_01
3. Choose Symbol_Moon -> Symbol_Eye -> Symbol_Key.
4. Confirm Door_LivingRoom_Kitchen opens.

## J. Kitchen

1. On LivingRoom_Right, place Kitchen door:
   - linkedDoorId = Door_LivingRoom_Kitchen
2. Place Kitchen number puzzle:
   - linkedPuzzleId = Puzzle_Kitchen_01
3. Enter 4826.
4. Confirm FrontDoorKey is acquired.
5. Confirm InventoryBar shows FrontDoorKey.

## K. Ending

Direct FinalDoor:

1. On Entrance or LivingRoom_Front, place a FinalDoor button:
   - ClickableButton.clickableType = FinalDoor
   - requiredItemId = FrontDoorKey
   - linkedPuzzleId empty
   - linkedDoorId empty
2. Select FrontDoorKey.
3. Click FinalDoor.
4. Confirm GameManager.EnterEnding is called.
5. Confirm EndingPanel appears and returns to TitleScene.

Puzzle FinalDoor:

1. Set linkedPuzzleId = Puzzle_EntranceDoor_01.
2. Select FrontDoorKey and click FinalDoor.
3. Complete Puzzle_EntranceDoor_01.
4. Confirm rewardType Ending enters the ending.

Puzzle_EntranceDoor_01 UI may still be TODO, so test direct FinalDoor first.

## L. Ghost / Noise / Hide

1. Fail a puzzle enough times to reach `failCountToNoise`.
2. Confirm NoiseManager.MakeNoise is called.
3. Confirm GhostManager enters RespondingToNoise.
4. Wait for SearchingLocation.
5. Stay in the same Location and confirm dangerLevel increases.
6. Reach dangerThreshold and confirm Chase starts.
7. Click a HidePoint:
   - ClickableButton.clickableType = HidePoint
   - targetObjectId = HidePoint_Bedroom_Closet
8. Confirm HideManager.IsHiding.
9. Wait until CanExitSafely.
10. Click HideExitButton.
11. Confirm Playing state returns.

## M. Save / Continue

1. Make progress and confirm SaveManager.SaveGame is called.
2. Stop Play.
3. Start from TitleScene again.
4. Confirm Continue is enabled.
5. Click Continue.
6. Confirm position, owned items, completed puzzles, opened doors, and unlocked clues restore.

## N. GameOver

1. Enter Chase.
2. Move repeatedly without hiding.
3. Reach chaseMoveLimit.
4. Confirm GameManager.GameOver.
5. Confirm GameOverPanel appears.
6. Restart from checkpoint.
7. Return to TitleScene.

## O. Allowed Warnings

Allowed for this stage:

- Resources.Load<Sprite> failures because sprites do not exist yet.
- Resources.Load<GameObject> failures because puzzle UI prefabs do not exist yet.

Must fix immediately:

- Missing JSON files.
- JSON parse failures.
- Duplicate core IDs.
- Missing Location, Door, Puzzle, Item, Clue, or Symbol references.
