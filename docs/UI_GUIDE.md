# UI Guide

## Visual Direction

- Dark hand-drawn 2D horror.
- Mostly black, white, and gray.
- Use weak red accents only for danger, monster pressure, altar clues, key events, and game over.
- Lines should feel thin, uneven, and uneasy.
- UI must stay minimal and avoid covering the room view more than necessary.

## Unity UI Stack

- Use UGUI Canvas, EventSystem, Button, Image, Slider, and TextMeshPro.
- Use the Input System pointer path for mouse click interaction.
- Keep the base composition at 1280 x 720 and scale through Canvas Scaler.
- Prefer reusable prefabs for title, pause, settings, inventory, map, puzzle, hiding, and game over surfaces.

## Interaction Rules

- Mouse click only.
- Do not show hover highlights or obvious clickable markers on room objects.
- Use sound, object changes, puzzle feedback, and visual reactions instead of explanatory text where possible.
- Investigation text should be rare and short.

## Core UI Surfaces

- Title: Start, Settings, Quit.
- Pause: Continue, Settings, Return to Title.
- Settings: BGM volume and SFX volume.
- In-game HUD: inventory button, map button, settings button.
- Inventory: item selection, item use target flow, minimal combination support.
- Map: simple floor plan by floor with current location marker.
- Puzzle screen: focused enlarged puzzle view.
- Hiding: small danger gauge.
- Game over: short “Game Over” display, then rewind-like restart.

## Layout Rules

- Base resolution is 1280 x 720.
- UI panels should be simple, flat, and hand-drawn rather than glossy or decorative.
- The playfield is the main surface; menus and panels should feel temporary.
- Map UI should be practical and readable, not decorative.
- RectTransform hit areas may exist for interaction but must not be visually highlighted in normal play.

## Asset Rules

- Use the filenames and categories from `design/06_RESOURCES_LIST.txt`.
- Place final or placeholder assets under Unity-friendly folders such as `Assets/Sprites`, `Assets/Audio`, and `Assets/UI`.
- If final assets are absent, create or use dark placeholders that preserve final names and references.
- Room placeholders may include small room labels for development, but clickable objects must not be highlighted.

## Source Details

- Visual and horror direction: `design/00_PROJECT_OVERVIEW.txt`
- UI, input, and save rules: `design/01_GAME_SYSTEMS_UI_RULES.txt`
- Resource list: `design/06_RESOURCES_LIST.txt`
