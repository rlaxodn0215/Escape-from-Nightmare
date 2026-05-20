# Step 0 - EventRuntimeSystem

## Pre-Implementation Proposal

Implement a focused `EventRuntimeSystem` that looks up Stage 1 `EventDefinition` assets by event ID, executes basic `EventEffect` types, records runtime flags, triggers stage clear/game over state, grants items, changes rooms, and plays event audio through `AudioEmitter` when available.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `EventRuntimeSystem`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Full monster AI behavior.
- Full sound mixing and ambience system.
- Bespoke event cinematics.
- Room art mutation and spawned visual object presentation.
- Progress saves or checkpoints.

## Validation Results

- Passed: `reports/unity-validation/event_runtime_system_validation.json`
- Unity MCP `refresh_unity` compiled the added runtime and editor scripts; after a forced all-assets refresh, no C# compiler errors remained.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Event Runtime`.
- Unity MCP found one scene `EventRuntimeSystem`.
- Unity MCP component inspection confirmed `EventRuntimeSystem` references `GameBootstrap`, `GameStateManager`, `SaveManager`, `RoomSystem`, `InventorySystem`, and `AudioEmitter`.
- Unity MCP component inspection confirmed `InteractionSystem` and `PuzzleSystem` reference `EventRuntimeSystem`.
- Static review confirmed 18 event assets and 27 seeded event effects.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/EventRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1EventRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scripts/Systems/InteractionSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Systems/PuzzleSystem.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Events`
- `reports/unity-validation/event_runtime_system_validation.json`

## Current State

- `EventRuntimeSystem` builds lookups from `StageDefinition.Events`, `StageDefinition.Sounds`, and `StageDefinition.Items`.
- `EventRuntimeSystem` supports runtime flags, item grants, audio playback, room changes, game over, and Stage 1 clear/save.
- `InteractionSystem` executes interactable event IDs after successful movement, item pickup, or default clue/object interactions.
- `InteractionSystem` can now satisfy `requiredFlag` checks through `EventRuntimeSystem`.
- `PuzzleSystem` executes puzzle success and failure event IDs.
- Stage 1 event assets now have minimal effects for puzzle rewards, key flags, final chase start, stage clear, and game over.
- Monster-facing effects are logged and queued for the later monster runtime system.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `SoundRuntimeSystem`.

## Next Action

Start `SoundRuntimeSystem` for ambience, UI SFX, puzzle feedback, and event audio routing.
