# Step 0: Room Runtime System

## Pre-Implementation Proposal

- `unit_type`: `system`
- `unit_id`: `RoomRuntimeSystem`
- `requires_user_design_approval`: `true`
- Goal: make the Stage 1 scene able to load `Stage1Definition`, resolve the active room, and display the current room background.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-19: This was recorded as `approved_for_implementation` for this scoped system unit.

## Out of Scope

- No clickable hotspot implementation.
- No inventory, puzzle, monster, hiding, map, or sound runtime.
- No UI prefab implementation.
- No final build.

## Decision Log

- 2026-05-19: Use `SpriteRenderer` under `RoomView` for the first room background slice.
- 2026-05-19: Bind room background sprites and item icons as part of this unit because the room runtime needs usable `RoomDefinition.BackgroundSprite` references.
- 2026-05-19: Added `RoomSystem`, `RoomViewPresenter`, and `Stage1SceneReferenceSeeder`.
- 2026-05-19: Unity BatchMode setup initially failed because Unity fake-null broke a null-coalescing component add path; fixed and reran successfully.

## Validation Results

- Unity BatchMode setup completed with exit code 0.
- BatchMode log contains: `Seeded Stage1 scene room runtime references.`
- `Stage1.unity` contains `RoomSystem` on `Systems`.
- `Stage1.unity` contains `RoomViewPresenter` and `SpriteRenderer` on `RoomView`.
- `child_room.asset` has a non-empty `backgroundSprite` reference.
- `front_door_key.asset` has a non-empty `icon` reference.

## Review Artifact

- `reports/unity-validation/room_runtime_system_validation.json`

## Current State

Room runtime system unit is complete. Entering Play Mode should initialize `RoomSystem`, resolve `child_room`, and assign the room background sprite to the `RoomView` SpriteRenderer.

## Remaining Gaps

- Movement and clickable hotspot definitions are not implemented yet.
- `RoomView` SpriteRenderer has no edit-time sprite by design; it is assigned at runtime by `RoomSystem`.
- No UI or inventory runtime is implemented yet.

## Resume Instructions

1. Treat `RoomRuntimeSystem` as completed.
2. Continue with `InteractionRuntimeSystem` or `InteractableHotspot.prefab`.
3. Use Unity BatchMode or reconnect Unity MCP for scene validation.

## Next Action

Open the next Harness unit for interaction runtime and hotspot prefab setup.
