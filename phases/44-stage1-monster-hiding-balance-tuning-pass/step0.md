# Step 0 - Stage1MonsterHidingBalanceTuningPass

## Pre-Implementation Proposal

Tune Stage 1 monster pressure and hiding balance using current runtime data and tests. Preserve the existing Stage 1 content and focus on making chase and hiding survivable when the player reacts quickly.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1MonsterHidingBalanceTuningPass`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- New monster events.
- New rooms.
- New UI surfaces.
- Puzzle content changes.
- Final generated-player smoke mode.

## Validation Results

- `Stage1MonsterHidingBalanceTuningPassValidation`: passed.
  - `defaultHideSeconds`: 5.
  - `captureDecayPerSecond`: 0.34.
  - `nearCapturePressurePerSecond`: 0.06.
  - `chaseCapturePressurePerSecond`: 0.18.
  - `chaseRoomMovesToEscape`: 3.
- Unity MCP seeding: passed.
  - `Escape From Nightmares/Seed Stage1 Hiding Runtime` completed.
  - `Escape From Nightmares/Seed Stage1 Monster Runtime` completed.
- Unity console: passed with note.
  - No C# compile errors were reported after the final compile.
  - Console contained MCP lifecycle entries and normal test-result output.
- Unity MCP EditMode tests: passed, 6/6.
- Unity MCP PlayMode tests: passed, 10/10.

## Review Artifact

- `reports/unity-validation/stage1_monster_hiding_balance_tuning_pass_validation.json`
- `EscapeFromNightmares/Assets/Scripts/Systems/HidingRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Systems/MonsterRuntimeSystem.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1HidingRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1MonsterRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Tests/PlayMode/Stage1DeepFlowPlayModeTests.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`

## Current State

- Completed. Monster pressure and hiding balance are tuned for a clearer survival window.
- Chase can now be escaped after three successful room moves.
- Successful hiding now calms an active `Searching`, `NearDetection`, or `Chase` monster threat.
- Added PlayMode coverage for both chase escape and hiding success.

## Resume Instructions

This phase is complete. Start a new Harness unit only if `phases/index.json` has no `in_progress` or `blocked` phase. Recommended next unit: `Stage1PuzzleFinaleWalkthroughValidation`.

## Next Action

Start `Stage1PuzzleFinaleWalkthroughValidation` as the next single Harness unit.
