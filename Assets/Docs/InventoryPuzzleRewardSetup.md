# Inventory / Puzzle Reward Setup Guide

This guide describes the manual Unity Editor setup for item pickup, inventory display, item use, and puzzle rewards.

## 1. System purpose

- Pick up items through `ClickableButton`.
- Show owned items in pre-placed inventory slots.
- Select an item by clicking an inventory slot.
- Use the selected item on a `UseItemTarget`.
- Apply puzzle rewards after `PuzzleManager.CompletePuzzle`.

## 2. InventoryBar setup

Example hierarchy:

```text
Canvas
- InventoryBar
  - Slot_01
  - Slot_02
  - Slot_03
  - Slot_04
  - Slot_05
```

InventoryBar:

- Add `InventoryBarUI`.
- Assign `slotRoot` to the `InventoryBar` transform.
- Set `autoCollectSlots` to `true`, or manually fill the `slots` list.

Each slot:

- Add a `Button` component.
- Add `InventorySlotUI`.
- Assign `iconImage` to an `Image`.
- Assign `labelText` to a `UnityEngine.UI.Text`.
- Assign `selectedIndicator`.
- Assign `emptyRoot` and `filledRoot` if the slot has separate empty/filled visuals.

Slots are not created automatically. Place them manually in the Unity Editor.

## 3. PickupItem setup

Example object:

```text
Pickup_Key_Button
- Button
- ClickableButton
- PickupItemController
```

`ClickableButton` values:

- `clickableType = PickupItem`
- `linkedItemId = OldDrawerKey`

Runtime behavior:

- Clicking the button calls `InventoryManager.TryAddItem`.
- The item is added to `SaveManager.ownedItemIds`.
- The inventory bar refreshes through `InventoryManager.InventoryChanged`.
- `PickupItemController` hides the object after pickup.

## 4. UseItemTarget setup

Example: using `OldDrawerKey` on a locked drawer.

```text
LockedDrawer_Button
- Button
- ClickableButton
```

`ClickableButton` values:

- `clickableType = UseItemTarget`
- `requiredItemId = OldDrawerKey`
- `linkedPuzzleId = Puzzle_Bedroom_01`

Optional follow-up links:

- `linkedPuzzleId`: opens a puzzle after successful item use.
- `linkedClueImageId`: unlocks and shows a clue image.
- `linkedDoorId`: marks a door as opened/unlocked in save data.
- `linkedItemId`: grants another item if it is different from `requiredItemId`.

Runtime behavior:

- If no item is selected, use fails.
- If the selected item does not match `requiredItemId`, use fails.
- If the selected item matches, `SaveManager.usedItemIds` is updated.
- If the item is consumable in `items.json`, it is removed from inventory.

## 5. Puzzle rewardType values

`PuzzleRecord.rewardType` remains a string. Use these values:

- `None`: no reward.
- `Item`: grants `rewardId` as an item.
- `Clue`: unlocks `rewardId` as a clue.
- `DoorUnlock`: marks `rewardId` as an opened door.
- `FinalChase`: sets `finalChaseStarted` in save data.
- `Ending`: calls `GameManager.EnterEnding`.

## 6. items.json example

This is documentation only. Do not overwrite the real JSON file unless you intentionally want this data.

```json
{
  "items": [
    {
      "itemId": "OldDrawerKey",
      "displayName": "Old Drawer Key",
      "description": "A small rusty key.",
      "iconPath": "Items/OldDrawerKey",
      "consumable": true
    },
    {
      "itemId": "FrontDoorKey",
      "displayName": "Front Door Key",
      "description": "The key to the front door.",
      "iconPath": "Items/FrontDoorKey",
      "consumable": false
    }
  ]
}
```

`iconPath` is a `Resources.Load<Sprite>()` path. For example, `Items/OldDrawerKey` can later point to `Assets/Resources/Items/OldDrawerKey`.

## 7. puzzles.json reward examples

Item reward:

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

Clue reward:

```json
{
  "puzzleId": "Puzzle_Study_01",
  "locationId": "Study",
  "type": "BookOrder",
  "prefabPath": "PuzzleUI/PuzzleUI_StudyBookOrder",
  "codeLength": 0,
  "answerVariableName": "StudyBookOrder",
  "timeLimitSeconds": 0,
  "failCountToNoise": 2,
  "rewardType": "Clue",
  "rewardId": "Clue_BasementPassword",
  "requiredItemId": "",
  "startsSolved": false
}
```

Door unlock reward:

```json
{
  "puzzleId": "Puzzle_Kitchen_01",
  "locationId": "Kitchen",
  "type": "NumberCode",
  "prefabPath": "PuzzleUI/PuzzleUI_KitchenNumberCode",
  "codeLength": 4,
  "answerVariableName": "KitchenCode",
  "timeLimitSeconds": 0,
  "failCountToNoise": 3,
  "rewardType": "DoorUnlock",
  "rewardId": "Door_Kitchen_Basement",
  "requiredItemId": "",
  "startsSolved": false
}
```

## 8. Notes

- Do not replace the real `StreamingAssets/Data` JSON files during this setup step.
- Real icon assets are not required yet.
- Puzzle UI answer logic is still TODO.
- Prefabs and scene objects are placed manually in the Unity Editor.
