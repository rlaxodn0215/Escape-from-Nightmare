# Step 0: Interactable Hotspot Prefab

## Pre-Implementation Proposal

- `unit_type`: `prefab`
- `unit_id`: `InteractableHotspot.prefab`
- `requires_user_design_approval`: `true`
- Goal: validate and complete a reusable invisible rectangular hotspot prefab that receives pointer clicks and forwards its `InteractableDefinition` to `InteractionSystem`.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped prefab unit.

## Out of Scope

- No room-specific hotspot placement.
- No `InteractableDefinition` asset seeding.
- No screen-edge hotspot prefab changes.
- No puzzle, item, event, or door behavior expansion.
- No visible hover highlight or clickable marker.

## Decision Log

- 2026-05-20: Keep the hotspot invisible by using a transparent `Image` with `raycastTarget` enabled.
- 2026-05-20: Keep the runtime click path through `IPointerClickHandler` and `InteractionSystem.HandleInteractableClicked`.
- 2026-05-20: Add hit-area application helpers so future room placement can map `InteractableDefinition.HitArea` to the prefab RectTransform.

## Validation Results

- Unity MCP was used. Ran `refresh_unity` with forced all-assets refresh and script compilation.
- Executed `Escape From Nightmares/Seed Interactable Hotspot Prefab`; Unity console confirmed `Seeded InteractableHotspot prefab.`
- Unity MCP prefab hierarchy inspection confirmed the prefab root has `RectTransform`, `CanvasRenderer`, `Image`, and `InteractableHotspot` components.
- Static prefab checks confirmed the `Image` color alpha is `0` and `m_RaycastTarget` is `1`.
- Static prefab checks confirmed `raycastImage` is assigned; reusable prefab `definition` and `interactionSystem` are intentionally unassigned.
- Static script check confirmed `Configure(InteractableDefinition, InteractionSystem)` and `ApplyHitArea(Rect)` are available for future room-specific placement.
- Static missing-script scan found no missing script markers in `InteractableHotspot.prefab` or `Stage1.unity`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/InteractableHotspot.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/InteractableHotspotPrefabSeeder.cs`
- `EscapeFromNightmares/Assets/Prefabs/Interactables/InteractableHotspot.prefab`
- `reports/unity-validation/interactable_hotspot_prefab_validation.json`

## Current State

Interactable hotspot prefab is complete for this unit. It is a transparent UI raycast target that forwards clicks to `InteractionSystem` and can later be sized from `InteractableDefinition.HitArea`.

## Resume Instructions

1. Treat this phase as completed.
2. Read `phases/index.json` before starting any further work.
3. If there is no `in_progress` or `blocked` phase, read `NEXT_ACTION.md` and start exactly one next Harness unit.

## Next Action

Start the next Harness unit from `NEXT_ACTION.md` only after confirming `phases/index.json` has no active phase.
