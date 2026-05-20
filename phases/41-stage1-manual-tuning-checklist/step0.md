# Step 0 - Stage1ManualTuningChecklist

## Pre-Implementation Proposal

Create a final manual tuning checklist for hit areas, map markers, monster/hiding balance, and generated-player smoke hook decisions before deciding whether implementation work is complete.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1ManualTuningChecklist`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Changing gameplay implementation.
- Editing hit-area values.
- Editing map marker values.
- Changing monster/hiding balance values.
- Deleting the heartbeat automation.

## Validation Results

- Passed: `reports/unity-validation/stage1_manual_tuning_checklist_validation.json`
- Created `reports/unity-validation/stage1_manual_tuning_checklist.md`.
- Checklist includes pass criteria and edit targets for hit areas, map markers, monster/hiding balance, puzzle/finale walkthrough, game-over restart, and generated-player deterministic smoke.
- Checklist records that Stage 1 should not be marked fully complete yet and heartbeat automation should not be deleted yet.
- Checklist recommends `Stage1HitAreaTuningPass` as the next Harness unit.

## Review Artifact

- `reports/unity-validation/stage1_manual_tuning_checklist.md`
- `reports/unity-validation/stage1_manual_tuning_checklist_validation.json`

## Current State

- Manual tuning checklist exists.
- Remaining work is focused tuning and deterministic validation.
- Heartbeat automation remains active because all required work is not intentionally complete.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `Stage1HitAreaTuningPass`.

## Next Action

Start `Stage1HitAreaTuningPass` to tune interactable hit areas room by room.
