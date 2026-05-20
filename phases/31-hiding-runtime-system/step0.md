# Step 0 - HidingRuntimeSystem

## Pre-Implementation Proposal

Implement a focused `HidingRuntimeSystem` that starts hiding from `HideSpot` interactables, displays `HidingGaugeUI`, tracks a capture gauge, exposes capture APIs for the future monster system, and fires `event_player_captured` when capture fills.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `HidingRuntimeSystem`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Full monster AI pathfinding and search behavior.
- Dynamic hide-spot discovery tuning.
- Bespoke hiding animations.
- Final capture balance values.
- Keyboard or held-key input.

## Validation Results

- Passed: `reports/unity-validation/hiding_runtime_system_validation.json`
- Unity MCP `refresh_unity` compiled the added runtime and editor scripts; after a forced all-assets refresh, no C# compiler errors remained.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Hiding Runtime`.
- Unity MCP found one scene `HidingRuntimeSystem`.
- Unity MCP component inspection confirmed `HidingRuntimeSystem` references `HidingGaugeUI`, `EventRuntimeSystem`, and `SoundRuntimeSystem`.
- Unity MCP component inspection confirmed `InteractionSystem` references `HidingRuntimeSystem`.
- Static review confirmed 5 hide spot interactable definitions and 5 scene hide spot instances.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/HidingRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Systems/InteractionSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/UI/HidingGaugeUI.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1HidingRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `reports/unity-validation/hiding_runtime_system_validation.json`

## Current State

- `InteractionSystem` routes `HideSpot` interactables into `HidingRuntimeSystem`.
- `HidingRuntimeSystem` tracks `IsHiding`, active hide spot ID, and capture gauge.
- `HidingRuntimeSystem` shows `HidingGaugeUI` while hiding or under capture pressure.
- `HidingRuntimeSystem` exposes `AddCapturePressure` and `SetCaptureGauge01` for the future monster runtime system.
- Capture gauge reaching 1 executes `event_player_captured`.
- Current hide/capture values are implementation placeholders pending monster playtest tuning.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `MonsterRuntimeSystem`.

## Next Action

Start `MonsterRuntimeSystem` to drive capture pressure, monster states, and `MonsterOverlay` presentation.
