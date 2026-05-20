# Step 0 - MonsterRuntimeSystem

## Pre-Implementation Proposal

Implement a focused `MonsterRuntimeSystem` that receives event-driven monster state requests, shows the correct `MonsterOverlay`, plays monster sound cues, and applies capture pressure through `HidingRuntimeSystem` when the monster is in near-detection or chase states.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `MonsterRuntimeSystem`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Full pathfinding or authored patrol routes.
- Final balance tuning.
- Bespoke monster cinematics.
- New monster art or audio assets.
- Map gameplay.

## Validation Results

- Passed: `reports/unity-validation/monster_runtime_system_validation.json`
- Unity MCP `refresh_unity` compiled the added runtime and editor scripts; after a forced all-assets refresh, no C# compiler errors remained.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Monster Runtime`.
- Unity MCP found one scene `MonsterRuntimeSystem`.
- Unity MCP component inspection confirmed `MonsterRuntimeSystem` references `GameBootstrap`, `RoomSystem`, `HidingRuntimeSystem`, `SoundRuntimeSystem`, and `MonsterOverlay`.
- Unity MCP component inspection confirmed `EventRuntimeSystem` references `MonsterRuntimeSystem`.
- Static review confirmed 9 Stage 1 monster node assets.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/MonsterRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Systems/EventRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1MonsterRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `reports/unity-validation/monster_runtime_system_validation.json`

## Current State

- `EventRuntimeSystem` now routes `StartMonster` and `ChangeMonsterState` effects into `MonsterRuntimeSystem`.
- `MonsterRuntimeSystem` tracks current monster state and current monster room.
- `MonsterRuntimeSystem` displays silhouette, near-detection, or chase overlay sprites through `MonsterOverlay`.
- `MonsterRuntimeSystem` plays basic monster sound cues through `SoundRuntimeSystem`.
- `MonsterRuntimeSystem` applies capture pressure through `HidingRuntimeSystem` during near-detection and chase.
- Capture timing values are placeholders pending playtest tuning.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `MapRuntimeSystem`.

## Next Action

Start `MapRuntimeSystem` to update `MapUI` current room and floor state from `RoomSystem`.
