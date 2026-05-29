# Source Route GameScene Builder Usage

## Purpose

Creates placeholder GameScene wiring for the source-aligned route. This is an Editor-only builder and does not create runtime auto-layout.

## Menus

- `Escape From Nightmare / Scene Setup / Build Missing Source Route GameScene Wiring`
- `Escape From Nightmare / Scene Setup / Build Missing Source Route GameScene Wiring And Save With Backup`
- `Escape From Nightmare / Scene Setup / Generate Source Route GameScene Builder Report`

## Backup

The save menu backs up `Assets/Scenes/GameScene.unity` into `Assets/Backups/Scenes` before saving.

## Recommended Validation Order

1. Validate Game Data
2. Validate Puzzle Prefab Contracts
3. Build Missing Source Route GameScene Wiring And Save With Backup
4. Validate Source Route Scene Wiring
5. Generate Scene Wiring Report
6. Run First Five Runtime Tests
7. Run Remaining Runtime Tests
8. Run Full Game Route Runtime Test
