# Step 2: stage1-audio-selection-manifest

## Game Unit

- unit_type: resource
- unit_id: stage1_audio_selection_manifest
- requires_user_design_approval: true

## Read Before Work

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/docs/CODEX_HARNESS.md`
- `/design/06_RESOURCES_LIST.txt`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/design/08_REMAINING_TASKS.txt`
- `/resource_manifest.json`

## Current State

- `resource_manifest.json` contains 62 required audio entries.
- All 62 required target audio files currently remain placeholder `.ogg` markers.
- The available production audio pack is under `EscapeFromNightmares/Assets/Audio/Gamemaster Audio - Pro Sound Collection`.
- The pack contains WAV source files and Unity `.meta` files; no OGG source files were found in the pack.
- Local `ffmpeg` and `ffprobe` commands were not available in this session.

## Pre-Implementation Proposal

Create a reviewable Stage 1 audio selection manifest that maps each required audio filename from `design/06_RESOURCES_LIST.txt` to an appropriate source WAV file from `Gamemaster Audio - Pro Sound Collection`.

This unit organizes and classifies audio choices only. It does not copy, delete, move, convert, or replace audio assets.

## User Decisions

- 2026-05-19: User stated that audio assets exist in `Audio/Gamemaster Audio - Pro Sound Collection` and requested classification/organization under Harness.
- 2026-05-19: User approved this unit with `approved_for_implementation: stage1_audio_selection_manifest`.

## Decision Log

- 2026-05-19: Treated `stage1_audio_selection_manifest` as a single resource unit.
- 2026-05-19: Kept the design target filenames unchanged as `.ogg`.
- 2026-05-19: Selected WAV sources from the pack for all required BGM, ambience, SFX, monster, UI, and event audio slots.
- 2026-05-19: Did not modify existing placeholder audio files because conversion/replacement is a separate implementation unit.
- 2026-05-19: User instructed Codex to proceed according to the Harness; this ready-for-review step was treated as approved and completed.

## Out of Scope

- C# sound system implementation.
- Scene, prefab, ScriptableObject, or manager wiring.
- Copying selected WAV files into target audio folders.
- Converting WAV files to OGG target files.
- Deleting or moving the original audio pack.
- Replacing the existing 62 placeholder `.ogg` marker files.

## Resource Inventory

- Manifest artifact: `audio_selection_manifest.json`.
- Required audio entries mapped: 62 / 62.
- Selected source format: WAV.
- Design target format: OGG.
- Source categories used:
  - `Backgrounds`
  - `Cinematic Sounds`
  - `Collectibles_Items_Powerup`
  - `Doors`
  - `Electricity_Hums`
  - `Footsteps`
  - `Impacts_Smashable`
  - `Switches_Buttons_Gears_Levers`
  - `User_Interface_Menu`
  - `Whooshes`
  - `Zombie`

## Acceptance Criteria

- `audio_selection_manifest.json` exists.
- Every required Stage 1 audio entry in `resource_manifest.json` has a selected source WAV mapping.
- Selected source paths are under the approved audio pack path.
- The step does not alter gameplay implementation, scenes, prefabs, ScriptableObjects, or placeholder target audio.
- Any blocked conversion/import checks are recorded.

## Validation Results

- PASS: All 62 required audio entries are represented in `audio_selection_manifest.json`.
- PASS: Each manifest entry includes `required_filename`, `unity_target_path`, `category`, `selected_source_wav`, `selection_status`, and `reason`.
- PASS: The source collection was found at `EscapeFromNightmares/Assets/Audio/Gamemaster Audio - Pro Sound Collection`.
- PASS: Existing target placeholder audio files were not changed by this unit.
- BLOCKED: No OGG conversion was performed because conversion/replacement is out of scope for this approved unit.
- BLOCKED: Unity import validation was not run because Unity MCP tools are not callable in this session and no approved Unity validation tooling exists yet.

## Review Artifact

- `audio_selection_manifest.json`
- `reports/unity-validation/stage1_audio_selection_validation.json`
- `phases/0-resource-inventory/step2.md`
- `phases/0-resource-inventory/index.json`

## Current State

- Status: `completed`.
- Stage 1 audio has been classified and mapped to source WAV candidates.
- The broader `resource-inventory` phase remains blocked until the user approves an audio replacement strategy:
  - convert selected WAV files to the existing `.ogg` target filenames, or
  - update design/resource targets to use `.wav`.

## Resume Instructions

Read this file, `audio_selection_manifest.json`, and `resource_manifest.json`. This step is complete; continue with the next active Harness step.

## Next Action

Continue with the next active Harness step.

## Next Step Blocker

Do not begin audio replacement, C# sound systems, scene wiring, prefabs, or ScriptableObject audio references until this mapping is reviewed and the next audio implementation unit is approved.
