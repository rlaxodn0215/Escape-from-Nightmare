# Step 0: Runtime Folder And Bootstrap

## Pre-Implementation Proposal

- `unit_type`: `system`
- `unit_id`: `RuntimeFolderAndBootstrap`
- `requires_user_design_approval`: `true`
- Goal: establish the Unity source folder layout and minimal runtime bootstrap components needed before gameplay systems.

## Planned Files And Assets

- Folders:
  - `Assets/Scripts/Core`
  - `Assets/Scripts/Systems`
  - `Assets/Scripts/UI`
  - `Assets/Scripts/AI`
  - `Assets/Scripts/Data`
  - `Assets/ScriptableObjects/Stage1/*`
  - `Assets/Prefabs/*`
  - `Assets/UI/Fonts`
  - `Assets/UI/Materials`
  - `Assets/Tests/EditMode`
  - `Assets/Tests/PlayMode`
- Scripts:
  - `Assets/Scripts/Core/GameBootstrap.cs`
  - `Assets/Scripts/Core/GameStateManager.cs`
  - `Assets/Scripts/Core/InputRouter.cs`
  - `Assets/Scripts/Core/SaveManager.cs`
  - `Assets/Scripts/Core/SceneFlowController.cs`

## User Decisions

- 2026-05-19: User said to proceed to the next task.
- 2026-05-19: This was recorded as `approved_for_implementation` for `RuntimeFolderAndBootstrap`.

## Out of Scope

- No room movement implementation.
- No inventory, puzzle, map, hiding, monster AI, or sound systems.
- No prefab creation beyond folders.
- No ScriptableObject data creation.
- No final build.

## Decision Log

- 2026-05-19: Keep this unit limited to Core bootstrap, input click routing, settings/clear-record persistence, and scene-flow placeholders.
- 2026-05-19: Runtime progress will stay in memory; only settings and `stage1_clear` are persisted.
- 2026-05-19: Created the recommended source/data/prefab/test folder layout.
- 2026-05-19: Added five Core scripts and attached them to the Stage1 scene roots after Unity import.

## Scene/Prefab Changes

- `StageRoot`: added `GameBootstrap`.
- `Systems`: added `GameStateManager`, `SceneFlowController`, `InputRouter`, and `SaveManager`.
- No prefab assets were created.

## Validation Results

- Unity MCP imported `Assets/Scripts`.
- Unity editor state: not compiling, no domain reload pending, ready for tools.
- Unity MCP script diagnostics:
  - `GameBootstrap.cs`: 0 errors, 0 warnings
  - `GameStateManager.cs`: 0 errors, 0 warnings
  - `InputRouter.cs`: 0 errors, 0 warnings
  - `SaveManager.cs`: 0 errors, 0 warnings
  - `SceneFlowController.cs`: 0 errors, 0 warnings
- Unity MCP scene hierarchy:
  - `StageRoot`: `Transform`, `GameBootstrap`
  - `Systems`: `Transform`, `GameStateManager`, `SceneFlowController`, `InputRouter`, `SaveManager`
- Unity MCP `manage_scene validate`: clean; `totalIssues: 0`, `missingScripts: 0`, `brokenPrefabs: 0`.
- Console note: recent console entries are MCP client lifecycle logs, not project script compile errors.

## Review Artifact

- `reports/unity-validation/runtime_folder_and_bootstrap_validation.json`

## Current State

Runtime folder/bootstrap unit is complete. The Stage1 scene has a minimal Core entry point and manager objects, but gameplay systems are not implemented yet.

## Remaining Gaps

- Data model ScriptableObject classes are not created.
- Required UI/gameplay prefabs are not created.
- Room, inventory, puzzle, monster, hiding, sound, and map systems are not implemented.
- EditMode/PlayMode test suites are not created.

## Resume Instructions

1. Treat `RuntimeFolderAndBootstrap` as completed.
2. Continue with the next Harness unit, recommended `DataModelDefinitions`.
3. Keep the next unit scoped to data classes only, or choose one prefab/system slice if priorities change.

## Next Action

Open the next implementation unit for data model definitions.
