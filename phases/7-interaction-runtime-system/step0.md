# Step 0: Interaction Runtime System

## Pre-Implementation Proposal

- `unit_type`: `system`
- `unit_id`: `InteractionRuntimeSystem`
- `requires_user_design_approval`: `true`
- Goal: create the invisible click hotspot runtime foundation for mouse-only point-and-click interaction.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-19: This was recorded as `approved_for_implementation` for this scoped system unit.

## Out of Scope

- No full room interactable content placement.
- No puzzle UI, inventory UI, hiding UI, or monster behavior.
- No hover highlight or visible clickable marker.

## Decision Log

- 2026-05-19: Use UGUI `RectTransform` hit areas and `IPointerClickHandler`.
- 2026-05-19: Keep hotspot visuals fully transparent.
- 2026-05-19: Added `InteractionSystem`, `InteractableHotspot`, and `Stage1InteractionReferenceSeeder`.
- 2026-05-19: Created `InteractableHotspot.prefab` and `ScreenEdgeHotspot.prefab` as transparent raycast targets.

## Validation Results

- Unity BatchMode setup completed with exit code 0.
- BatchMode log contains: `Seeded interaction runtime and hotspot prefabs.`
- `Stage1.unity` contains `InteractionSystem` on `Systems`.
- `InteractableHotspot.prefab` exists and contains:
  - `RectTransform`
  - `CanvasRenderer`
  - transparent `Image` with `raycastTarget` enabled
  - `InteractableHotspot`
- `ScreenEdgeHotspot.prefab` exists.

## Review Artifact

- `reports/unity-validation/interaction_runtime_system_validation.json`

## Current State

Interaction runtime foundation is complete. It does not place room-specific hotspots yet.

## Remaining Gaps

- InteractableDefinition assets and room-specific hit areas still need a focused placement unit.
- Puzzle, item pickup, hide spot, and event behavior are only logged or deferred.

## Resume Instructions

1. Treat `InteractionRuntimeSystem` as completed.
2. Continue with UI/inventory foundations or room interactable placement.

## Next Action

Open the next Harness unit.
