# Step 14: windows-package

## Read First

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/CODEX_HARNESS.md`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/design/08_REMAINING_TASKS.txt`
- `/phases/0-mvp/index.json`
- `/build/EscapeFromNightmares.love`

## Resource Gate

Before creating the Windows package, verify step 12 completed and `build/EscapeFromNightmares.love` includes non-empty `assets/` fallback resources. If the packaged game has no asset files, stop as `blocked` with objective choices instead of producing `build/windows/`.

## Task

Create the Windows executable package under `build/windows/`.

1. Confirm the local LÖVE runtime path from step 0 summary or local environment settings.
2. Use `build/EscapeFromNightmares.love` as the input package.
3. Place the Windows executable and required runtime dll/files under `build/windows/`.
4. Use `EscapeFromNightmares` as the package name.
5. If the LÖVE runtime path is missing or not executable, stop as `blocked` with 2-3 objective choices.

## Acceptance Criteria

```bash
dir build\windows
```

When the local environment permits it:

```bash
build\windows\EscapeFromNightmares.exe
```

## Verification

1. Confirm Windows runtime files exist under `build/windows/`.
2. Confirm the exe starts the LÖVE game when local execution is available.
3. Confirm `build/` is still only used for distributable outputs.
4. Confirm the packaged game includes placeholder resources from step 12.
5. Mark step 14 completed only after the Windows package is actually produced and verified.

## Prohibited

- Do not guess the LÖVE runtime path. Reason: local environments differ.
- Do not add the LÖVE runtime to the repo unless explicitly required by the packaging step and local policy.
- Do not mark Windows packaging as completed if the exe package is missing or unverified.
