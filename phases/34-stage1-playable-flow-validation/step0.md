# Step 0 - Stage1PlayableFlowValidation

## Pre-Implementation Proposal

Run a focused playable-flow validation for Stage 1 before build work. Check required scene roots, runtime systems, UI surfaces, required prefabs, data references, Build Settings, Unity compiler diagnostics, and available Unity tests.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1PlayableFlowValidation`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- New gameplay systems.
- New prefabs or scenes.
- Final balance tuning.
- Windows player build generation.
- Asset art/audio redesign.

## Validation Results

- Passed with followups: `reports/unity-validation/stage1_playable_flow_validation.json`
- Unity MCP confirmed Stage1 is the active scene and enabled Build Settings scene.
- Unity MCP confirmed required playable scene roots are present.
- Unity MCP confirmed the `Systems` root has all current runtime systems, including `MapRuntimeSystem`.
- Unity MCP confirmed one scene instance exists for all required core UI surfaces and HUD buttons.
- Unity MCP confirmed `RoomViewPresenter`, `MonsterOverlay`, and six `AudioEmitter` instances exist.
- Static checks confirmed required UI and gameplay prefab files exist.
- Static checks confirmed Stage1 data assets exist: 27 rooms, 53 interactables, 10 items, 7 puzzles, 23 events, 9 monster nodes, and sound definitions.
- Static YAML scan found no missing script markers in `Stage1.unity` or prefabs.
- Unity EditMode test job `565efa318ce249b79d39b68297a92d28` succeeded with 0 concrete tests.
- Unity PlayMode test job `b7ad6426ba5b4899afd935a3707ec2e1` succeeded with 0 concrete tests.
- Followup: no `BuildScript.BuildWindows` entry point exists yet.

## Review Artifact

- `reports/unity-validation/stage1_playable_flow_validation.json`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `EscapeFromNightmares/ProjectSettings/EditorBuildSettings.asset`

## Current State

- Stage1 has the required playable roots and required runtime systems.
- Stage1 has required UI surfaces, HUD buttons, room view, monster overlay, audio emitters, prefabs, and data assets.
- Build Settings uses `Assets/Scenes/Stage1.unity`; `SampleScene` is not the only enabled build scene.
- Unity test runner is available, but no concrete gameplay smoke tests are authored yet.
- Windows build work needs a `BuildScript.BuildWindows` entry point or equivalent build profile next.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `WindowsBuildScript`.

## Next Action

Start `WindowsBuildScript` to add a batchmode-friendly Windows player build entry point before the final build step.
