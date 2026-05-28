# Examine / Clue Image Setup Guide

## 1. System purpose

This system supports:

- Clicking an examine button.
- Checking clue requirements.
- Showing examine or clue images.
- Saving unlocked clue IDs.
- Closing the clue image panel.

## 2. ClueImagePanel setup

Create the panel manually in the Unity Editor:

```text
Canvas
- ClueImagePanel
  - Background
  - ClueImage
  - TitleText
  - DescriptionText
  - MessageText
  - CloseButton
```

On `ClueImagePanel`:

- Add `ClueImagePanelUI`.
- Assign `rootObject` to `ClueImagePanel`.
- Assign `clueImage`.
- Assign `titleText`.
- Assign `descriptionText`.
- Assign `messageText`.
- Assign `closeButton`.
- Keep `hideOnAwake = true`.

On `ClueImageManager`:

- Assign the `clueImagePanel` field to the `ClueImagePanelUI` component.

## 3. ExamineImage button setup

Example:

```text
BedroomPhoto_Button
- Button
- ClickableButton
```

`ClickableButton` values:

- `clickableType = ExamineImage`
- `linkedClueImageId = Clue_Bedroom_Photo`

Runtime flow:

1. The button calls `InteractionManager.HandleExamineImage`.
2. `ClueImageManager.ShowClueImage("Clue_Bedroom_Photo")` runs.
3. Requirements are checked.
4. If allowed, `ClueImagePanelUI.ShowClue` displays the title, description, and sprite.
5. `GameState.Examine` is set.
6. Close button calls `HideCurrentImage`.
7. `GameState.Playing` is restored.

## 4. UseItemTarget opens a clue

Example:

```text
LockedDrawer_Button
- Button
- ClickableButton
```

`ClickableButton` values:

- `clickableType = UseItemTarget`
- `requiredItemId = OldDrawerKey`
- `linkedClueImageId = Clue_LockedDrawer_Note`

Runtime flow:

1. Select `OldDrawerKey` in the inventory.
2. Click `LockedDrawer_Button`.
3. `InventoryManager.TryUseSelectedItem("OldDrawerKey")` runs.
4. If successful, `ClueImageManager.UnlockClue("Clue_LockedDrawer_Note")` runs.
5. `ClueImageManager.ShowClueImage("Clue_LockedDrawer_Note")` displays the clue.
6. `SaveManager.unlockedClueIds` is updated.

If the item is consumable, `InventoryManager` removes it according to `ItemRecord.consumable`.

## 5. clues.json fields

Each clue can use these fields:

- `clueId`
- `locationId`
- `imagePath`
- `requiredPuzzleId`
- `requiredItemId`
- `startsUnlocked`
- `displayName`
- `description`
- `lockedMessage`

## 6. Resources image paths

`ClueRecord.imagePath` uses a `Resources.Load<Sprite>()` path.

Examples:

```text
Assets/Resources/ExamineImages/BedroomPhoto.png
imagePath = "ExamineImages/BedroomPhoto"

Assets/Resources/ClueImages/BasementPassword.png
imagePath = "ClueImages/BasementPassword"
```

Do not include the file extension in JSON.

## 7. startsUnlocked usage

- Use `startsUnlocked = true` for simple examine images.
- Use `startsUnlocked = false` for clues gated by puzzles or items.
- Puzzle reward clues should use `rewardType = "Clue"` and `rewardId = clueId`.

## 8. Puzzle reward clue unlock

Puzzle reward example:

```json
{
  "puzzleId": "Puzzle_Study_01",
  "rewardType": "Clue",
  "rewardId": "Clue_BasementPassword"
}
```

This unlocks the clue and saves it, but does not automatically show the image.

## 9. Example JSON

Initially visible examine image:

```json
{
  "clueId": "Clue_Bedroom_Photo",
  "locationId": "Bedroom",
  "imagePath": "ExamineImages/BedroomPhoto",
  "requiredPuzzleId": "",
  "requiredItemId": "",
  "startsUnlocked": true,
  "displayName": "Old Photo",
  "description": "A faded family photo. Something is written on the back.",
  "lockedMessage": ""
}
```

Clue visible after puzzle completion:

```json
{
  "clueId": "Clue_BasementPassword",
  "locationId": "Study",
  "imagePath": "ClueImages/BasementPassword",
  "requiredPuzzleId": "Puzzle_Study_01",
  "requiredItemId": "",
  "startsUnlocked": false,
  "displayName": "Basement Password",
  "description": "A note with a strange four-digit number.",
  "lockedMessage": "You do not understand this clue yet."
}
```

Clue requiring an item:

```json
{
  "clueId": "Clue_LockedDrawer_Note",
  "locationId": "Bedroom",
  "imagePath": "ClueImages/LockedDrawerNote",
  "requiredPuzzleId": "",
  "requiredItemId": "OldDrawerKey",
  "startsUnlocked": false,
  "displayName": "Hidden Note",
  "description": "A note hidden inside the old drawer.",
  "lockedMessage": "The drawer is locked."
}
```

## 10. Notes

- Do not overwrite the real JSON files during this setup step.
- Do not create real image assets in this step.
- UI panels and buttons are manually placed in the Unity Editor.
- Use `UnityEngine.UI.Text`, not TextMeshPro.
