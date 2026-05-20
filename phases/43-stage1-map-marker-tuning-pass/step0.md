# Step 0 - Stage1MapMarkerTuningPass

## Pre-Implementation Proposal

Tune Stage 1 map marker positions and floor display using current Stage1 room/map data. Keep the map practical and readable, with the current room marker shown only on the active current-room floor.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1MapMarkerTuningPass`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Monster/hiding balance tuning.
- New map UI features.
- New rooms or floor art.
- Puzzle or finale walkthrough tuning.
- Visible gameplay hotspot markers.

## Validation Results

- `Stage1MapMarkerTuningPassValidation`: passed.
  - Parsed 27 map marker positions.
  - All markers are inside conservative map-image bounds of x +/-300 and y +/-180.
  - Stage1 map runtime was reseeded through Unity MCP.
- Unity console: passed with note.
  - No C# compile errors were reported after the final compile.
  - Console contained MCP lifecycle entries and normal test-result output.
- Unity MCP EditMode tests: passed, 6/6.
- Unity MCP PlayMode tests: passed, 8/8.

## Review Artifact

- `reports/unity-validation/stage1_map_marker_tuning_pass_validation.json`
- `EscapeFromNightmares/Assets/Scripts/UI/MapUI.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1MapRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Tests/PlayMode/Stage1PlayModeSmokeTests.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`

## Current State

- Completed. Tuned all 27 Stage 1 room marker positions for a more readable per-floor layout.
- `MapUI` now tracks the current room floor separately from the viewed floor tab.
- The current-room marker is hidden when the player views a different floor and reappears when returning to the current room's floor.
- Added PlayMode coverage for marker visibility while switching floor tabs.

## Resume Instructions

This phase is complete. Start a new Harness unit only if `phases/index.json` has no `in_progress` or `blocked` phase. Recommended next unit: `Stage1MonsterHidingBalanceTuningPass`.

## Next Action

Start `Stage1MonsterHidingBalanceTuningPass` as the next single Harness unit.
