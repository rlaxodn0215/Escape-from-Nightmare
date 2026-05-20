# Step 0: Hiding Gauge UI Prefab

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `HidingGaugeUI.prefab`
- `requires_user_design_approval`: `true`
- Goal: create a small reusable hiding danger gauge prefab with background and fill visuals, placed under `UICanvas` as an inactive scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No `HidingSystem` implementation.
- No mouse movement danger calculation.
- No monster search or hide spot interaction wiring.
- No capture/game-over trigger wiring.
- No additional UI prefabs beyond `HidingGaugeUI.prefab`.

## Decision Log

- 2026-05-20: Use existing `ui_hiding_danger_gauge_bg` and `ui_hiding_danger_gauge_fill` sprites.
- 2026-05-20: `HidingGaugeUI` only owns visibility and a normalized danger fill value.
- 2026-05-20: Place `HidingGaugeUI` inactive under `UICanvas`; future hiding system can show it and drive the fill amount.

## Validation Results

- Unity MCP was used. Ran `refresh_unity` with forced all-assets refresh and script compilation.
- Executed `Escape From Nightmares/Seed Hiding Gauge UI Prefab`; Unity console confirmed `Seeded HidingGaugeUI prefab and Stage1 scene instance.`
- Static prefab checks confirmed `HidingGaugeUI.prefab` exists with `GaugeBackground`, `GaugeFill`, and `Label`.
- Static prefab checks confirmed `HidingGaugeUI` references `CanvasGroup`, `GaugeFill`, `Label`, and a zero initial `danger01`.
- Static prefab checks confirmed `GaugeFill` is configured as a filled horizontal Image with `fillAmount` initialized to `0`.
- Static scene checks confirmed `Stage1.unity` contains `UICanvas/HidingGaugeUI` as an inactive prefab instance.
- Static missing-script scan found no missing script markers in `HidingGaugeUI.prefab` or `Stage1.unity`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/UI/HidingGaugeUI.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/HidingGaugeUIPrefabSeeder.cs`
- `EscapeFromNightmares/Assets/Prefabs/UI/HidingGaugeUI.prefab`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `reports/unity-validation/hiding_gauge_ui_prefab_validation.json`

## Current State

Hiding gauge UI prefab is complete for this unit. It is placed under `UICanvas` as an inactive scene instance and exposes a normalized danger fill API for future `HidingSystem` wiring.

## Resume Instructions

1. Treat this phase as completed.
2. Read `phases/index.json` before starting any further work.
3. If there is no `in_progress` or `blocked` phase, read `NEXT_ACTION.md` and start exactly one next Harness unit.

## Next Action

Start the next Harness unit from `NEXT_ACTION.md` only after confirming `phases/index.json` has no active phase.
