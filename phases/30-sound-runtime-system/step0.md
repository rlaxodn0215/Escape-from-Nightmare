# Step 0 - SoundRuntimeSystem

## Pre-Implementation Proposal

Implement a focused `SoundRuntimeSystem` that builds a `SoundDefinition` lookup from `StageDefinition`, plays room ambience on room changes, and routes event/puzzle audio through dedicated `AudioEmitter` channels.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `SoundRuntimeSystem`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Final mix balancing.
- Dynamic monster proximity audio logic.
- Timeline/cinematic audio.
- Audio occlusion or spatial simulation.
- New audio resource generation.

## Validation Results

- Passed: `reports/unity-validation/sound_runtime_system_validation.json`
- Unity MCP `refresh_unity` compiled the added runtime and editor scripts; after a forced all-assets refresh, no C# compiler errors remained.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Sound Runtime`.
- Unity MCP found one scene `SoundRuntimeSystem`.
- Unity MCP component inspection confirmed `SoundRuntimeSystem` references `GameBootstrap`, `RoomSystem`, and five `AudioEmitter` channels.
- Unity MCP component inspection confirmed `InteractionSystem`, `PuzzleSystem`, and `EventRuntimeSystem` reference `SoundRuntimeSystem`.
- Static review confirmed 62 sound definition assets, 13 room ambience references, and 15 event audio cue references.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/SoundRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1SoundRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Rooms`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Events`
- `reports/unity-validation/sound_runtime_system_validation.json`

## Current State

- `SoundRuntimeSystem` builds a lookup from `StageDefinition.Sounds`.
- `SoundRuntimeSystem` plays room ambience when `RoomSystem.RoomChanged` fires.
- `AudioRoot` now has `BgmEmitter`, `AmbienceEmitter`, `SfxEmitter`, `EventEmitter`, and `UiEmitter` channels.
- `EventRuntimeSystem` routes `audioCue` and `PlayAudio` effects through `SoundRuntimeSystem`.
- `PuzzleSystem` plays puzzle success and error SFX.
- `InteractionSystem` plays item pickup and door open SFX.
- Room assets now reference matching ambience definitions where a matching Stage 1 ambience asset exists.
- Event assets now reference matching event/SFX/BGM audio cues where a matching Stage 1 sound asset exists.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `HidingRuntimeSystem`.

## Next Action

Start `HidingRuntimeSystem` using `HideSpot` interactables and `HidingGaugeUI`.
