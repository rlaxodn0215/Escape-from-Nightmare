# Step 0 - Stage1AutomatedSmokeTests

## Pre-Implementation Proposal

Add concrete Unity EditMode and PlayMode smoke tests for Stage1 scene structure, runtime wiring, UI surfaces, build settings, and startup room flow. The goal is to replace the previous 0-test runner pass with real automated coverage.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1AutomatedSmokeTests`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Full puzzle walkthrough automation.
- Final balance tuning.
- Generated-player automation hooks.
- New gameplay systems.
- New prefabs or scenes.

## Validation Results

- Passed: `reports/unity-validation/stage1_automated_smoke_tests_validation.json`
- Unity MCP test discovery now finds 9 concrete tests: 6 EditMode and 3 PlayMode.
- Unity MCP script validation reported zero diagnostics for both smoke test source files.
- EditMode test job `d062f3576d07454faa9e90f3f10f298a` succeeded: 6 of 6 completed with no failures.
- PlayMode test job `f7888791742346739ee790d2eaf9e151` succeeded: 3 passed, 0 failed, 0 skipped.
- All added test files and asmdef files have generated `.meta` files.

## Review Artifact

- `EscapeFromNightmares/Assets/Tests/EditMode/EscapeFromNightmares.EditMode.Tests.asmdef`
- `EscapeFromNightmares/Assets/Tests/EditMode/Stage1EditModeSmokeTests.cs`
- `EscapeFromNightmares/Assets/Tests/PlayMode/EscapeFromNightmares.PlayMode.Tests.asmdef`
- `EscapeFromNightmares/Assets/Tests/PlayMode/Stage1PlayModeSmokeTests.cs`
- `reports/unity-validation/stage1_automated_smoke_tests_validation.json`

## Current State

- EditMode smoke tests cover Stage1 required roots, Systems runtime components, UI surfaces/HUD buttons, required prefabs, Build Settings, and StageDefinition asset presence.
- PlayMode smoke tests cover Stage1 load/start room, runtime systems/UI surfaces after load, and MapRuntimeSystem marker coverage.
- The previous 0-test runner gap is resolved.
- Deeper puzzle-chain, game-over restart, stage-clear, and balance/tuning tests remain followups.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `Stage1FinalPlaytestTuningReview`.

## Next Action

Start `Stage1FinalPlaytestTuningReview` to review remaining playtest tuning gaps for hit areas, map markers, and monster/hiding balance.
