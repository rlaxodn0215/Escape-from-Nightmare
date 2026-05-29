# Visual Resource Connection Guide

## Purpose

Use this guide to place final visual assets under `Assets/Resources` so runtime UI components and validators can load them with `Resources.Load<Sprite>()`.

## Background Images

Location views use `ViewBackgroundBinding`.

- Path rule: `Backgrounds/{ViewId}`
- Example file: `Assets/Resources/Backgrounds/Bedroom_Front.png`
- Component value: `Backgrounds/Bedroom_Front`

## Clue Images

Clue images use `clues.json` `imagePath`.

- Example file: `Assets/Resources/ExamineImages/BedroomPhotoCodeClue.png`
- JSON value: `ExamineImages/BedroomPhotoCodeClue`

## Item Icons

Item icons use `items.json` `iconPath`.

- Example file: `Assets/Resources/Items/OldDrawerKey.png`
- JSON value: `Items/OldDrawerKey`

## Symbol Sprites

Symbol sprites use `symbols.json` `spritePath`.

- Example file: `Assets/Resources/Symbols/Symbol_01.png`
- JSON value: `Symbols/Symbol_01`

## Validation Menu

Run:

`Escape From Nightmare / Visual Polish / Validate Visual Resources`

## Notes

- Do not include file extensions in Resources paths.
- Imported images must use Sprite import type.
- Missing sprites are Warnings during placeholder development, not project Errors.
