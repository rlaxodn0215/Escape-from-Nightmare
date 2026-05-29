# Art Resource Intake Guide

## Purpose

This guide describes how to receive real art files without changing JSON, Prefabs, or Scene wiring by hand.

## Recommended Flow

1. Place art files in `Assets/ArtIntake`.
2. Run `Escape From Nightmare / Art Resources / Generate Art Resource Intake Report`.
3. Run `Escape From Nightmare / Art Resources / Copy Matching ArtIntake Files To Resources With Backup`.
4. Run `Escape From Nightmare / Art Resources / Apply Sprite Import Settings To Visual Resources`.
5. Run `Escape From Nightmare / Visual Polish / Validate Visual Resources`.
6. Run `Escape From Nightmare / Art Resources / Validate Art Resource Bindings`.
7. Run the post-polish regression test sequence.

## Folder Mapping

| ArtIntake Folder | Resources Folder |
|---|---|
| `Assets/ArtIntake/Backgrounds` | `Assets/Resources/Backgrounds` |
| `Assets/ArtIntake/ExamineImages` | `Assets/Resources/ExamineImages` |
| `Assets/ArtIntake/ClueImages` | `Assets/Resources/ClueImages` |
| `Assets/ArtIntake/Items` | `Assets/Resources/Items` |
| `Assets/ArtIntake/Symbols` | `Assets/Resources/Symbols` |
| `Assets/ArtIntake/Ghost` | `Assets/Resources/Ghost` |
| `Assets/ArtIntake/UI` | `Assets/Resources/UI` |

## Backup Rule

When matching ArtIntake files are copied to Resources, existing Resources files are backed up under `Assets/Backups/ArtResources/yyyyMMdd_HHmmss`.

## Notes

- File names must match the final Resources path segment.
- Supported extensions: `.png`, `.jpg`, `.jpeg`, `.psd`, `.tga`.
- Final delivery should prefer `.png`, especially for transparent icons and symbols.
