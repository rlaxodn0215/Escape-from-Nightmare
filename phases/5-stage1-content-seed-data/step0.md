# Step 0: Stage1 Content Seed Data

## Pre-Implementation Proposal

- `unit_type`: `system`
- `unit_id`: `Stage1ContentSeedData`
- `requires_user_design_approval`: `true`
- Goal: create initial Stage 1 ScriptableObject content assets from the design/resource manifests.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-19: This was recorded as `approved_for_implementation` for this scoped system unit.
- 2026-05-19: Approved exception to create multiple coupled data assets in one system unit.

## Out of Scope

- No runtime system implementation.
- No prefab creation.
- No scene wiring beyond existing Stage1 bootstrap.
- No final build.

## Decision Log

- 2026-05-19: Use `Assets/ScriptableObjects/Stage1` folders created by the bootstrap unit.
- 2026-05-19: Seed data must use existing final resource filenames from `resource_manifest.json`.
- 2026-05-19: Created coupled seed data in one unit because the assets are one graph around `Stage1Definition`.
- 2026-05-19: Unity MCP became stale during sound seeding; stopped Unity processes and used Unity BatchMode to run `Stage1SoundDefinitionSeeder.Seed`.

## Validation Results

- Static asset count validation:
  - Rooms: 27
  - Items: 10
  - Puzzles: 7
  - Events: 18
  - Monster nodes: 9
  - Sounds: 62
  - Stage definition: present
- `Stage1Definition.asset` contains serialized references for rooms, items, puzzles, events, monster nodes, and sounds.
- Unity BatchMode sound seeding completed with exit code 0.
- BatchMode log contains: `Seeded 62 Stage 1 sound definitions.`

## Review Artifact

- `reports/unity-validation/stage1_content_seed_data_validation.json`

## Current State

Stage 1 seed data is complete enough for runtime systems to consume.

## Created Assets

- `Assets/ScriptableObjects/Stage1/Stage1Definition.asset`
- `Assets/ScriptableObjects/Stage1/Rooms/*.asset`
- `Assets/ScriptableObjects/Stage1/Items/*.asset`
- `Assets/ScriptableObjects/Stage1/Puzzles/*.asset`
- `Assets/ScriptableObjects/Stage1/Events/*.asset`
- `Assets/ScriptableObjects/Stage1/MonsterNodes/*.asset`
- `Assets/ScriptableObjects/Stage1/Sounds/**/*.asset`

## Remaining Gaps

- InteractableDefinition assets are not seeded yet because precise hit areas are still a separate room/interaction unit.
- Background sprite references are not yet connected to RoomDefinition assets.
- Item icon references are not yet connected to ItemDefinition assets.
- Runtime systems do not yet consume `Stage1Definition`.

## Resume Instructions

1. Treat `Stage1ContentSeedData` as completed.
2. Continue with `RoomRuntimeSystem`.
3. Reopen Unity/MCP if Unity-native scene validation is needed; Unity was stopped to recover from a stale MCP bridge.

## Next Action

Open the next Harness unit for the room runtime system.
