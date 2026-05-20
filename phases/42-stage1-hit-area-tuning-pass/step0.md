# Step 0 - Stage1HitAreaTuningPass

## Pre-Implementation Proposal

Tune interactable hit areas room by room using current Stage1 data. Keep hotspots invisible. Do not tune map markers or monster balance in this same unit.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1HitAreaTuningPass`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Map marker tuning.
- Monster/hiding balance tuning.
- New gameplay systems.
- Visible hotspot markers or hover highlights.
- Full manual click-by-click playthrough.

## Validation Results

- `Stage1HitAreaTuningPassValidation`: passed.
  - Parsed 53 Stage 1 interactable hit areas.
  - 0 out-of-bounds rects.
  - 0 undersized rects.
  - 0 oversized rects.
  - Same-room overlaps reduced from 5 to 1.
- Unity MCP seeding: passed.
  - `Escape From Nightmares/Seed Stage1 Interactable Definitions` reported 53 definitions across 23 rooms.
  - `Escape From Nightmares/Seed Stage1 Hotspot Scene Instances` reported 53 hotspot instances across 23 room groups.
- Unity console: passed with note.
  - No C# compile errors were reported.
  - Console contained only MCP lifecycle entries and a non-blocking UDP port warning.
- Unity MCP EditMode tests: passed, 6/6.
- Unity MCP PlayMode tests: passed, 7/7.
- Unity MCP `execute_code`: blocked non-critical by a Windows path-length error from `mono.exe`; static PowerShell validation and Unity MCP menu/test validation were used instead.

## Review Artifact

- `reports/unity-validation/stage1_hit_area_tuning_pass_validation.json`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1InteractableDefinitionSeeder.cs`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Interactables`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`

## Current State

- Completed. Tuned high-overlap Stage 1 hit areas and reseeded the corresponding ScriptableObject assets and Stage1 scene hotspot instances.
- Adjusted these hit areas:
  - `first_floor_hallway_to_entrance`
  - `first_floor_hallway_to_garage`
  - `living_room_to_first_floor_hallway`
  - `living_room_curtain_hide`
  - `basement_wall_symbols`
  - `front_door_key_on_altar`
- Residual overlap:
  - `breaker_box` and `laundry_room_cabinet_hide` overlap by 296 pixels, 1.2 percent of the smaller rect, and is accepted as negligible until manual visual tuning.

## Resume Instructions

This phase is complete. Start a new Harness unit only if `phases/index.json` has no `in_progress` or `blocked` phase. Recommended next unit: `Stage1MapMarkerTuningPass`.

## Next Action

Start `Stage1MapMarkerTuningPass` as the next single Harness unit.
