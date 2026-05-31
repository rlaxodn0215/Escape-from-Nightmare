# Visual Resource Requirements

All paths below are `Resources.Load<Sprite>()` paths. Do not include file extensions in JSON or component fields.

## Recommended Import Settings

| Type | Recommended Resolution | Transparency | Max Size | Resources Folder | ArtIntake Folder |
|---|---|---|---:|---|---|
| Backgrounds | 1920x1080 or 2048+ 16:9 | No | 4096 | `Assets/Resources/Backgrounds` | `Assets/ArtIntake/Backgrounds` |
| Clue / Examine Images | 1920x1080 or panel-ready high resolution | Optional | 2048 | `Assets/Resources/ClueImages`, `Assets/Resources/ExamineImages` | `Assets/ArtIntake/ClueImages`, `Assets/ArtIntake/ExamineImages` |
| Item Icons | 512x512 | Yes, PNG recommended | 512 | `Assets/Resources/Items` | `Assets/ArtIntake/Items` |
| Symbols | 512x512 | Yes, PNG recommended | 512 | `Assets/Resources/Symbols` | `Assets/ArtIntake/Symbols` |
| Ghost | 1024-2048 | Yes, PNG recommended | 2048 | `Assets/Resources/Ghost` | `Assets/ArtIntake/Ghost` |
| UI Buttons / Panels | 512-1024 | Optional | 1024 | `Assets/Resources/UI` | `Assets/ArtIntake/UI` |

## Background Sprite Requirements

| View ID | Resources Path | ArtIntake File | Resources File |
|---|---|---|---|
| Bedroom_Front | Backgrounds/Bedroom_Front | Assets/ArtIntake/Backgrounds/Bedroom_Front.png | Assets/Resources/Backgrounds/Bedroom_Front.png |
| Bedroom_Back | Backgrounds/Bedroom_Back | Assets/ArtIntake/Backgrounds/Bedroom_Back.png | Assets/Resources/Backgrounds/Bedroom_Back.png |
| ChildRoom_Front | Backgrounds/ChildRoom_Front | Assets/ArtIntake/Backgrounds/ChildRoom_Front.png | Assets/Resources/Backgrounds/ChildRoom_Front.png |
| ChildRoom_Back | Backgrounds/ChildRoom_Back | Assets/ArtIntake/Backgrounds/ChildRoom_Back.png | Assets/Resources/Backgrounds/ChildRoom_Back.png |
| Study_Front | Backgrounds/Study_Front | Assets/ArtIntake/Backgrounds/Study_Front.png | Assets/Resources/Backgrounds/Study_Front.png |
| Study_Right | Backgrounds/Study_Right | Assets/ArtIntake/Backgrounds/Study_Right.png | Assets/Resources/Backgrounds/Study_Right.png |
| Study_Back | Backgrounds/Study_Back | Assets/ArtIntake/Backgrounds/Study_Back.png | Assets/Resources/Backgrounds/Study_Back.png |
| Study_Left | Backgrounds/Study_Left | Assets/ArtIntake/Backgrounds/Study_Left.png | Assets/Resources/Backgrounds/Study_Left.png |
| SecondFloorHallway_Front | Backgrounds/SecondFloorHallway_Front | Assets/ArtIntake/Backgrounds/SecondFloorHallway_Front.png | Assets/Resources/Backgrounds/SecondFloorHallway_Front.png |
| SecondFloorHallway_Back | Backgrounds/SecondFloorHallway_Back | Assets/ArtIntake/Backgrounds/SecondFloorHallway_Back.png | Assets/Resources/Backgrounds/SecondFloorHallway_Back.png |
| LivingRoom_Front | Backgrounds/LivingRoom_Front | Assets/ArtIntake/Backgrounds/LivingRoom_Front.png | Assets/Resources/Backgrounds/LivingRoom_Front.png |
| LivingRoom_Back | Backgrounds/LivingRoom_Back | Assets/ArtIntake/Backgrounds/LivingRoom_Back.png | Assets/Resources/Backgrounds/LivingRoom_Back.png |
| Entrance_Front | Backgrounds/Entrance_Front | Assets/ArtIntake/Backgrounds/Entrance_Front.png | Assets/Resources/Backgrounds/Entrance_Front.png |
| Kitchen_Front | Backgrounds/Kitchen_Front | Assets/ArtIntake/Backgrounds/Kitchen_Front.png | Assets/Resources/Backgrounds/Kitchen_Front.png |
| BasementStorage_Front | Backgrounds/BasementStorage_Front | Assets/ArtIntake/Backgrounds/BasementStorage_Front.png | Assets/Resources/Backgrounds/BasementStorage_Front.png |
| BasementStorage_Right | Backgrounds/BasementStorage_Right | Assets/ArtIntake/Backgrounds/BasementStorage_Right.png | Assets/Resources/Backgrounds/BasementStorage_Right.png |
| BasementStorage_Back | Backgrounds/BasementStorage_Back | Assets/ArtIntake/Backgrounds/BasementStorage_Back.png | Assets/Resources/Backgrounds/BasementStorage_Back.png |
| BasementStorage_Left | Backgrounds/BasementStorage_Left | Assets/ArtIntake/Backgrounds/BasementStorage_Left.png | Assets/Resources/Backgrounds/BasementStorage_Left.png |
| LockedRoom_Front | Backgrounds/LockedRoom_Front | Assets/ArtIntake/Backgrounds/LockedRoom_Front.png | Assets/Resources/Backgrounds/LockedRoom_Front.png |
| LockedRoom_Right | Backgrounds/LockedRoom_Right | Assets/ArtIntake/Backgrounds/LockedRoom_Right.png | Assets/Resources/Backgrounds/LockedRoom_Right.png |
| LockedRoom_Back | Backgrounds/LockedRoom_Back | Assets/ArtIntake/Backgrounds/LockedRoom_Back.png | Assets/Resources/Backgrounds/LockedRoom_Back.png |
| LockedRoom_Left | Backgrounds/LockedRoom_Left | Assets/ArtIntake/Backgrounds/LockedRoom_Left.png | Assets/Resources/Backgrounds/LockedRoom_Left.png |

## Clue / Examine Image Requirements

| Clue ID | Resources Path | ArtIntake File |
|---|---|---|
| BedroomPhotoCodeClue | ExamineImages/BedroomPhotoCodeClue | Assets/ArtIntake/ExamineImages/BedroomPhotoCodeClue.png |
| LivingRoomEntranceCodeClue | ClueImages/LivingRoomEntranceCodeClue | Assets/ArtIntake/ClueImages/LivingRoomEntranceCodeClue.png |
| ChildRoomCardSymbolClueImage | ClueImages/ChildRoomCardSymbolClueImage | Assets/ArtIntake/ClueImages/ChildRoomCardSymbolClueImage.png |
| StudyBookSymbolClueImage | ClueImages/StudyBookSymbolClueImage | Assets/ArtIntake/ClueImages/StudyBookSymbolClueImage.png |
| KitchenCodeClueImage | ClueImages/KitchenCodeClueImage | Assets/ArtIntake/ClueImages/KitchenCodeClueImage.png |
| KitchenFridgeSurfaceSymbolClue | ClueImages/KitchenFridgeSurfaceSymbolClue | Assets/ArtIntake/ClueImages/KitchenFridgeSurfaceSymbolClue.png |
| BasementPowerPatternClue | ClueImages/BasementPowerPatternClue | Assets/ArtIntake/ClueImages/BasementPowerPatternClue.png |
| BasementClueImage | ClueImages/BasementClueImage | Assets/ArtIntake/ClueImages/BasementClueImage.png |

## Item Icon Requirements

| Item ID | Resources Path | ArtIntake File |
|---|---|---|
| OldDrawerKey | Items/OldDrawerKey | Assets/ArtIntake/Items/OldDrawerKey.png |
| SmallClockworkDevice | Items/SmallClockworkDevice | Assets/ArtIntake/Items/SmallClockworkDevice.png |
| ModifiedClockworkDevice | Items/ModifiedClockworkDevice | Assets/ArtIntake/Items/ModifiedClockworkDevice.png |
| BasementFuse | Items/BasementFuse | Assets/ArtIntake/Items/BasementFuse.png |
| FrontDoorKey | Items/FrontDoorKey | Assets/ArtIntake/Items/FrontDoorKey.png |

## Symbol Sprite Requirements

| Symbol ID | Resources Path | ArtIntake File |
|---|---|---|
| Symbol_01 | Symbols/Symbol_01 | Assets/ArtIntake/Symbols/Symbol_01.png |
| Symbol_02 | Symbols/Symbol_02 | Assets/ArtIntake/Symbols/Symbol_02.png |
| Symbol_03 | Symbols/Symbol_03 | Assets/ArtIntake/Symbols/Symbol_03.png |
| Symbol_04 | Symbols/Symbol_04 | Assets/ArtIntake/Symbols/Symbol_04.png |
| Symbol_05 | Symbols/Symbol_05 | Assets/ArtIntake/Symbols/Symbol_05.png |
| Symbol_06 | Symbols/Symbol_06 | Assets/ArtIntake/Symbols/Symbol_06.png |

## Optional UI / Ghost

| Asset | Resources Path | ArtIntake File |
|---|---|---|
| Default Button | UI/Buttons/DefaultButton | Assets/ArtIntake/UI/Buttons/DefaultButton.png |
| Default Panel | UI/Panels/DefaultPanel | Assets/ArtIntake/UI/Panels/DefaultPanel.png |
| Default Ghost | Ghost/Ghost_Default | Assets/ArtIntake/Ghost/Ghost_Default.png |

## Notes

- Actual image files are intentionally not generated by the tooling.
- Run `Apply Sprite Import Settings To Visual Resources` after copying art into `Assets/Resources`.
- Missing required art remains a Warning until real art files are delivered.
