# Step 0 - MapRuntimeSystem

## Pre-Implementation Proposal

Implement a focused `MapRuntimeSystem` that observes `RoomSystem.RoomChanged`, derives the active floor and marker position from Stage 1 room data, and updates `MapUI` so the map opens on the current room/floor.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `MapRuntimeSystem`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- New map art or UI layout redesign.
- Clickable map fast travel.
- Final marker tuning.
- Additional map tutorial text.
- Unrelated gameplay systems.

## Validation Results

- Passed: `reports/unity-validation/map_runtime_system_validation.json`
- Unity MCP validated `MapRuntimeSystem.cs`, `MapUI.cs`, and `Stage1MapRuntimeSeeder.cs` with zero diagnostics.
- Unity MCP `refresh_unity` compiled the added runtime and editor scripts; remaining console errors were MCP client disposal messages, not C# compiler errors.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Map Runtime` and logged `Seeded Stage1 map runtime.`
- Unity MCP found one scene `MapRuntimeSystem`.
- Unity MCP component inspection confirmed `MapRuntimeSystem` references `GameBootstrap`, `RoomSystem`, inactive `MapUI`, and 27 room markers.
- Static scene inspection confirmed `Stage1.unity` serializes `MapRuntimeSystem`, its `mapUI` reference, and `markers`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/MapRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/UI/MapUI.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1MapRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `reports/unity-validation/map_runtime_system_validation.json`

## Current State

- `MapRuntimeSystem` subscribes to `RoomSystem.RoomChanged`.
- `MapRuntimeSystem` initializes the map from the start/current room.
- `MapUI` can now receive current room state through `SetCurrentRoom`.
- Stage1 `Systems` has one `MapRuntimeSystem` connected to `RoomSystem` and `MapUI`.
- Stage1 has 27 serialized map marker positions, one for each known room.
- Marker coordinates are practical placeholders pending final playtest/map-art tuning.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `Stage1PlayableFlowValidation`.

## Next Action

Start `Stage1PlayableFlowValidation` to validate the playable Stage 1 flow across title, HUD, room movement, interaction, puzzle, event, map, hiding, monster, and game-over systems.
