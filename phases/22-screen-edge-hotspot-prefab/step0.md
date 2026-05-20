# Step 0: Screen Edge Hotspot Prefab

## Pre-Implementation Proposal

- `unit_type`: `prefab`
- `unit_id`: `ScreenEdgeHotspot.prefab`
- `requires_user_design_approval`: `true`
- Goal: validate and complete a reusable invisible screen-edge hotspot prefab that can receive pointer clicks and forward movement interactables to `InteractionSystem`.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped prefab unit.

## Out of Scope

- No room-specific edge placement.
- No `InteractableDefinition` asset seeding.
- No room movement content expansion.
- No visible edge arrows, highlights, or markers.
- No additional gameplay prefabs beyond `ScreenEdgeHotspot.prefab`.

## Decision Log

- 2026-05-20: Keep screen-edge hotspots invisible by using a transparent `Image` with `raycastTarget` enabled.
- 2026-05-20: Reuse `InteractableHotspot` click forwarding so screen-edge movement still flows through `InteractionSystem`.
- 2026-05-20: Add a marker component and default edge-size RectTransform so this prefab is distinguishable from object hotspots.

## Validation Results

- Unity MCP was used. Ran `refresh_unity` with forced all-assets refresh and script compilation.
- Executed `Escape From Nightmares/Seed Screen Edge Hotspot Prefab`; Unity console confirmed `Seeded ScreenEdgeHotspot prefab.`
- Unity MCP prefab hierarchy inspection confirmed the prefab root has `RectTransform`, `CanvasRenderer`, `Image`, `InteractableHotspot`, and `ScreenEdgeHotspot` components.
- Static prefab checks confirmed the `Image` color alpha is `0` and `m_RaycastTarget` is `1`.
- Static prefab checks confirmed the default RectTransform is right-edge anchored with size `96x720`.
- Static prefab checks confirmed the `ScreenEdgeHotspot` marker component is present with default `edge` value `Right`.
- Static missing-script scan found no missing script markers in `ScreenEdgeHotspot.prefab` or `Stage1.unity`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/ScreenEdgeHotspot.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/ScreenEdgeHotspotPrefabSeeder.cs`
- `EscapeFromNightmares/Assets/Prefabs/Interactables/ScreenEdgeHotspot.prefab`
- `reports/unity-validation/screen_edge_hotspot_prefab_validation.json`

## Current State

Screen-edge hotspot prefab is complete for this unit. It is a transparent right-edge UI raycast target that reuses `InteractableHotspot` click forwarding and carries a `ScreenEdgeHotspot` marker component.

## Resume Instructions

1. Treat this phase as completed.
2. Read `phases/index.json` before starting any further work.
3. If there is no `in_progress` or `blocked` phase, read `NEXT_ACTION.md` and start exactly one next Harness unit.

## Next Action

Start the next Harness unit from `NEXT_ACTION.md` only after confirming `phases/index.json` has no active phase.
