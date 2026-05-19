# Step 3: stage1-audio-wav-target-update

## Game Unit

- unit_type: resource
- unit_id: stage1_audio_wav_target_update
- requires_user_design_approval: true

## Read Before Work

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/docs/CODEX_HARNESS.md`
- `/design/06_RESOURCES_LIST.txt`
- `/resource_manifest.json`
- `/audio_selection_manifest.json`

## Current State

- Stage 1 audio target format is now WAV.
- The selected WAV files are copied into Unity target audio category folders.
- Previous OGG placeholder files and their OGG meta files have been removed from the target folders.
- The source audio pack remains in `EscapeFromNightmares/Assets/Audio/Gamemaster Audio - Pro Sound Collection`.

## Pre-Implementation Proposal

Convert Stage 1 audio resource targets from placeholder OGG markers to real WAV target assets by copying the selected source WAV files from `audio_selection_manifest.json` into the planned Unity audio folders.

The unit updates manifests and the resource list to use WAV filenames, removes only the obsolete target OGG placeholder files and OGG meta files, and records validation results.

## User Decisions

- 2026-05-19: User selected the design/manifest target update approach: change targets to WAV instead of converting WAV sources to OGG.
- 2026-05-19: User selected deletion of existing placeholder `.ogg` and `.ogg.meta` files.
- 2026-05-19: User requested implementation of the approved plan.

## Decision Log

- 2026-05-19: Updated the 62 audio entries in `resource_manifest.json` from `.ogg` to `.wav` and set their status to `final`.
- 2026-05-19: Updated the 62 entries in `audio_selection_manifest.json` to `.wav` targets and `copied_to_target` status.
- 2026-05-19: Updated the 62 audio filenames in `design/06_RESOURCES_LIST.txt` to `.wav`.
- 2026-05-19: Copied selected WAV source files into the Unity target audio folders.
- 2026-05-19: Removed obsolete target OGG placeholder files and their OGG meta files.
- 2026-05-19: User instructed Codex to proceed according to the Harness; this ready-for-review step was treated as approved and completed.

## Out of Scope

- C# sound system implementation.
- Scene, prefab, ScriptableObject, or manager wiring.
- Audio balancing, loop point editing, normalization, or mastering.
- Deleting or modifying source files inside `Gamemaster Audio - Pro Sound Collection`.
- Unity-generated `.wav.meta` authoring.

## Resource Inventory

- Required Stage 1 audio entries: 62.
- Target format: WAV.
- Target folders:
  - `EscapeFromNightmares/Assets/Audio/BGM`
  - `EscapeFromNightmares/Assets/Audio/Ambience`
  - `EscapeFromNightmares/Assets/Audio/SFX`
  - `EscapeFromNightmares/Assets/Audio/Monster`
  - `EscapeFromNightmares/Assets/Audio/UI`
  - `EscapeFromNightmares/Assets/Audio/Events`

## Acceptance Criteria

- All Stage 1 audio entries in `resource_manifest.json` use `.wav`.
- All Stage 1 audio entries in `audio_selection_manifest.json` use `.wav` targets.
- All 62 selected target WAV files exist and are non-zero byte.
- No target OGG placeholder files or OGG meta files remain in the Stage 1 target audio folders.
- All selected source WAV files still exist in the original source pack.
- No gameplay scripts, scenes, prefabs, or ScriptableObjects are changed by this unit.

## Validation Results

- PASS: `resource_manifest.json` parses as JSON.
- PASS: `audio_selection_manifest.json` parses as JSON.
- PASS: `phases/0-resource-inventory/index.json` parses as JSON.
- PASS: `reports/unity-validation/stage1_audio_wav_target_update_validation.json` parses as JSON.
- PASS: Stage 1 audio entry count is 62.
- PASS: All 62 Stage 1 audio entries use `.wav` targets.
- PASS: All 62 target WAV files exist.
- PASS: All 62 target WAV files are non-zero byte.
- PASS: No target OGG placeholder files remain in the target audio folders.
- PASS: No target OGG meta files remain in the target audio folders.
- PASS: All 62 selected source WAV files still exist in the source audio pack.
- BLOCKED: Unity import validation was not run because Unity MCP tools are not callable in this session and no approved Unity validation tooling exists yet.

## Review Artifact

- `resource_manifest.json`
- `audio_selection_manifest.json`
- `design/06_RESOURCES_LIST.txt`
- `reports/unity-validation/stage1_audio_wav_target_update_validation.json`
- `EscapeFromNightmares/Assets/Audio/BGM/*.wav`
- `EscapeFromNightmares/Assets/Audio/Ambience/*.wav`
- `EscapeFromNightmares/Assets/Audio/SFX/*.wav`
- `EscapeFromNightmares/Assets/Audio/Monster/*.wav`
- `EscapeFromNightmares/Assets/Audio/UI/*.wav`
- `EscapeFromNightmares/Assets/Audio/Events/*.wav`

## Current State

- Status: `completed`.
- Stage 1 audio resources now have real WAV target files.
- The broader resource inventory still has non-audio resource blockers and Unity-native validation blockers.

## Resume Instructions

Read this file, `resource_manifest.json`, and `audio_selection_manifest.json`. This step is complete; continue with the next active Harness step.

## Next Action

Continue with the next active Harness step. Do not begin C# sound systems, scene wiring, prefabs, or ScriptableObject audio references until the resource inventory phase allows it.

## Next Step Blocker

Unity-native import validation remains blocked until Unity MCP tools are callable or `UnityValidationTools` is approved as a separate system unit.
