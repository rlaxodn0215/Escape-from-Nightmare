# Step 0: Stage1 Scene Skeleton

## Pre-Implementation Proposal

- `unit_type`: `scene`
- `unit_id`: `Stage1SceneSkeleton`
- `requires_user_design_approval`: `true`
- Goal: create a proper Stage 1 Unity scene shell so the project is no longer SampleScene-only.
- Files/assets expected:
  - `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
  - `reports/unity-validation/stage1_scene_skeleton_validation.json`
- Unity-native changes:
  - Create/load `Assets/Scenes/Stage1.unity`.
  - Add required scene root objects:
    - `StageRoot`
    - `Systems`
    - `RoomView`
    - `InteractableLayer`
    - `MonsterLayer`
    - `AudioRoot`
    - `UICanvas`
    - `EventSystem`
    - `DebugRoot`
  - Keep a camera and 2D light in the scene for render readiness.
  - Register `Stage1.unity` in Build Settings.

## User Decisions

- 2026-05-19: User said to proceed to the next task.
- 2026-05-19: This was recorded as `approved_for_implementation` for `Stage1SceneSkeleton`.

## Out of Scope

- No gameplay C# systems.
- No room data, puzzle data, item data, or monster AI.
- No UI prefab implementation beyond the root `UICanvas` object if available.
- No prefab creation.
- No final build.

## Decision Log

- 2026-05-19: Stage1 scene skeleton selected as the next Harness unit after resource inventory and project readiness validation.
- 2026-05-19: Initial scene creation with the Lit2D scene template timed out and did not create a file, so the scene was created without the template.
- 2026-05-19: Added the required playable root objects, then added `Main Camera` and `Global Light 2D` to satisfy scene setup policy.
- 2026-05-19: Registered only `Assets/Scenes/Stage1.unity` in Build Settings.

## Validation Results

- Unity MCP active scene: `Assets/Scenes/Stage1.unity`
- Build Settings: one enabled scene, `Assets/Scenes/Stage1.unity`
- Required roots present:
  - `StageRoot`
  - `Systems`
  - `RoomView`
  - `InteractableLayer`
  - `MonsterLayer`
  - `AudioRoot`
  - `UICanvas`
  - `EventSystem`
  - `DebugRoot`
- Render/input support roots present:
  - `Main Camera`
  - `Global Light 2D`
- `UICanvas` components: `Canvas`, `CanvasScaler`, `GraphicRaycaster`
- `EventSystem` components: `EventSystem`, `InputSystemUIInputModule`
- `DebugRoot` is present and inactive.
- Unity MCP `manage_scene validate`: clean; `totalIssues: 0`, `missingScripts: 0`, `brokenPrefabs: 0`.

## Review Artifact

- `reports/unity-validation/stage1_scene_skeleton_validation.json`

## Current State

Stage1 scene skeleton is complete and saved. The project is no longer SampleScene-only in Build Settings.

## Remaining Gaps

- Root objects do not yet have runtime manager scripts.
- No gameplay prefabs or UI prefabs have been created.
- No ScriptableObject data assets have been created.
- No EditMode or PlayMode tests were run in this step.

## Resume Instructions

1. Treat `Stage1SceneSkeleton` as completed.
2. Continue with the next Harness unit: recommended `RuntimeFolderAndBootstrap`.
3. Use Unity MCP for script compilation, hierarchy checks, and serialized reference checks after implementation.

## Next Action

Open the next implementation unit for runtime folder/bootstrap setup.
