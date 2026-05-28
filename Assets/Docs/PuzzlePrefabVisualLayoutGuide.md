# Puzzle Prefab Visual Layout Guide

## Common Layout

- The Prefab root is instantiated under `PuzzleManager.puzzleUiRoot`.
- The root fills the available screen area.
- The root acts as a full-screen Panel.
- The puzzle interaction area should stay centered.
- Put `CloseButton` in a predictable position, usually bottom controls or upper right.

## NumberCode Layout

- Top: title and timer.
- Center: current code display.
- Lower center: digit buttons 0 through 9.
- Bottom: Submit, Clear, Backspace, Close.

The generated Builder layout is functional development UI. It can be restyled later without changing component field names.

## Sequence Layout

- Top: title.
- Center: current selected sequence.
- Lower center: option buttons.
- Bottom: Submit, Reset, Close.

Generated option buttons show `Symbol_01` style labels until real symbol art is connected.

## SymbolCycle Layout

- Top: title.
- Center: five symbol slots.
- Each slot cycles through `Symbol_01` through `Symbol_06` when clicked.
- Bottom: Submit, Reset, Close.

If Sprite assets are missing, the slot label displays the symbol ID or display name.

## Temporary Visual Rules

- Development UI is allowed to be plain.
- Missing Sprite assets should not block interaction.
- Keep serialized field connections stable when replacing visuals.
- Later art pass can replace Image colors, Sprite assignments, and layout spacing.
