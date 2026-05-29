# Placeholder Visual Polish Guide

## Purpose

This stage polishes the placeholder GameScene created by the source route builder so the scene is easier to test before final art arrives.

## Menus

- `Escape From Nightmare / Visual Polish / Apply Placeholder Visual Polish`
- `Escape From Nightmare / Visual Polish / Apply Placeholder Visual Polish And Save With Backup`
- `Escape From Nightmare / Visual Polish / Generate Placeholder Visual Polish Report`

## Applied Components

- `ViewBackgroundBinding`
- `HotspotButtonVisual`
- `DebugHotspotOverlay`
- `PanelVisualPreset`
- Basic RectTransform layout cleanup for generated placeholder buttons

## Debug Labels

Hotspot labels can be toggled with `F3` through `DebugHotspotOverlay`.

During final playtest, labels should be hidden and hotspots should use low alpha while keeping raycast targets enabled.

## Manual Polish Still Required

- Place Door hotspots over actual door images.
- Place Puzzle hotspots over actual interactable objects.
- Place ExamineImage hotspots over actual clue objects.
- Adjust HidePoint hotspots.
- Refine Inventory, Clue, GameOver, Ending, and Puzzle UI panels.

## Notes

- The builder does not create final design.
- The builder does not create image assets.
- Existing user objects are reused and not deleted.
