# Step 0: Hide Spot Prefab

## Pre-Implementation Proposal

- `unit_type`: `prefab`
- `unit_id`: `HideSpot.prefab`
- `requires_user_design_approval`: `true`
- Goal: create a reusable invisible hide spot interaction prefab that can receive pointer clicks and later enter hiding state through `InteractableType.HideSpot` definitions.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped prefab unit.

## Out of Scope

- No `HidingSystem` implementation.
- No mouse movement danger calculation.
- No monster hide-spot search behavior.
- No room-specific hide spot placement.
- No visible hide marker or hover highlight.

## Decision Log

- 2026-05-20: Keep hide spots invisible by using a transparent `Image` with `raycastTarget` enabled.
- 2026-05-20: Reuse `InteractableHotspot` click forwarding so future `InteractableType.HideSpot` definitions flow through `InteractionSystem`.
- 2026-05-20: Add a `HideSpot` marker component with a default recommended hold duration for future hiding system wiring.

## Validation Results

- Unity MCP was used. Ran `refresh_unity` with forced all-assets refresh and script compilation.
- Executed `Escape From Nightmares/Seed Hide Spot Prefab`; Unity console confirmed `Seeded HideSpot prefab.`
- Unity MCP prefab hierarchy inspection confirmed the prefab root has `RectTransform`, `CanvasRenderer`, `Image`, `InteractableHotspot`, and `HideSpot` components.
- Static prefab checks confirmed the `Image` color alpha is `0` and `m_RaycastTarget` is `1`.
- Static prefab checks confirmed the `HideSpot` marker component is present with `hideSpotId` set to `hide_spot` and `recommendedHoldSeconds` set to `6`.
- Static missing-script scan found no missing script markers in `HideSpot.prefab` or `Stage1.unity`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/HideSpot.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/HideSpotPrefabSeeder.cs`
- `EscapeFromNightmares/Assets/Prefabs/Interactables/HideSpot.prefab`
- `reports/unity-validation/hide_spot_prefab_validation.json`

## Current State

Hide spot prefab is complete for this unit. It is a transparent UI raycast target that reuses `InteractableHotspot` click forwarding and carries a `HideSpot` marker component.

## Resume Instructions

1. Treat this phase as completed.
2. Read `phases/index.json` before starting any further work.
3. If there is no `in_progress` or `blocked` phase, read `NEXT_ACTION.md` and start exactly one next Harness unit.

## Next Action

Start the next Harness unit from `NEXT_ACTION.md` only after confirming `phases/index.json` has no active phase.
