# Step 12: resource-fallback-assets

## Read First

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/docs/CODEX_HARNESS.md`
- `/design/06_RESOURCES_LIST.txt`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/design/08_REMAINING_TASKS.txt`
- `/data/rooms.lua`
- `/data/items.lua`
- `/data/events.lua`
- `/data/sound_data.lua`

## Task

Create replaceable fallback resources before packaging so the build is never shipped with an empty `assets/` tree.

1. Scan `data/*.lua` for every referenced path under `assets/images/` and `assets/sounds/`.
2. Create every missing image as a simple dummy PNG at the exact referenced path.
   - Room backgrounds must be `1280x720`.
   - Item icons and UI/object/monster images may be smaller, but must be valid PNG files.
   - Keep the visual style dark, mostly monochrome, and unobtrusive.
3. Create every missing sound as a valid silent `.ogg` file at the exact referenced path.
   - Prefer an existing local `ffmpeg`, `oggenc`, or equivalent encoder.
   - If no encoder is available and the user explicitly approves dependency installation, the bundled Python may install `imageio-ffmpeg` to obtain a local ffmpeg binary for placeholder generation.
   - If no encoder is available and dependency installation is not approved, stop as `blocked` with 2-3 objective choices.
4. Add or update a verification script that fails when:
   - any referenced `assets/...` file is missing,
   - `assets/` contains zero files,
   - an image file is invalid or unreadable,
   - a sound file is invalid or unreadable when the local verification tool can check it.
5. Do not add final art claims. These are placeholder resources only, with stable paths for later replacement.

## Acceptance Criteria

```bash
Get-ChildItem -Recurse -File assets | Measure-Object
```

The count must be greater than `0`.

Run the project verification command or script added by this step. It must pass before this step can be marked completed.

## Verification

1. Confirm every resource path referenced by `data/rooms.lua`, `data/items.lua`, `data/events.lua`, and `data/sound_data.lua` exists under `assets/`.
2. Confirm the placeholder files are committed by the successful step commit.
3. Confirm no source, data, docs, design, saves, or build output files were moved into `assets/`.
4. Mark step 12 completed only after real files exist in `assets/`.

## Prohibited

- Do not leave `assets/` empty and proceed to packaging. Reason: packaged builds must include visible/audio fallback resources.
- Do not invent final production art direction beyond `docs/UI_GUIDE.md`. Reason: this step creates replaceable placeholders.
- Do not skip a referenced resource path because the runtime has silent fallback. Reason: packaging must include complete placeholder files.
