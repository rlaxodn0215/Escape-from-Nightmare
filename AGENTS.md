# Project: Escape From Nightmares

## Stack
- LÖVE 2D
- Lua
- Windows PC
- Base resolution: 1280 x 720
- Input: mouse click only

## CRITICAL Rules
- CRITICAL: Do not build this as a web, Unity, Ren'Py, HTML5, or JavaScript game.
- CRITICAL: Do not implement progress saves or checkpoints.
- CRITICAL: Only settings and clear records may be saved.
- CRITICAL: Do not highlight clickable objects by default.
- CRITICAL: Do not invent rooms, story, characters, or stages outside Stage 1.
- CRITICAL: If final assets are missing, use dummy images or silent fallbacks while preserving replaceable asset paths.

## Context Map
- `docs/PRD.md`: product goal, MVP scope, clear condition.
- `docs/ARCHITECTURE.md`: folder structure, system responsibilities, data flow.
- `docs/ADR.md`: technical decisions and rejected alternatives.
- `docs/UI_GUIDE.md`: art, UI, and interaction rules.
- `docs/CODEX_HARNESS.md`: phase/step planning and execution rules.
- `design/*.txt`: source-of-truth details. Read only the specific design file needed for the current step.

## Design Source Files
- Project and scope: `design/00_PROJECT_OVERVIEW.txt`
- UI, input, save rules: `design/01_GAME_SYSTEMS_UI_RULES.txt`
- Stage 1 rooms: `design/02_STAGE1_SPACE_ROOMS.txt`
- Puzzles, items, events: `design/03_PUZZLES_ITEMS_EVENTS.txt`
- Monster and hiding AI: `design/04_MONSTER_HIDING_AI.txt`
- Implementation structure: `design/05_IMPLEMENTATION_STRUCTURE.txt`
- Assets and sounds: `design/06_RESOURCES_LIST.txt`
- Final development instruction: `design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- Remaining unknowns: `design/08_REMAINING_TASKS.txt`

## Process
- Keep each implementation step small and tied to one layer or subsystem.
- Follow TDD where practical: write focused tests or verification scripts before risky behavior changes.
- Use conventional commits: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`.
- Do not guess unknown decisions. If user input is required, stop as `blocked` and provide a question with 2-3 choices. Mark the recommended choice with `(Recommended)`.

## Commands
- Development run command depends on the local LÖVE installation and must be confirmed per environment.
- LÖVE runtime is provided by a local path, not committed into this repo. If the path is unknown or not executable, block with choices instead of guessing.
- Packaging `.love` and Windows exe outputs must be handled in dedicated build steps.
- Final `.love` output: `build/EscapeFromNightmares.love`.
- Final Windows exe package output: `build/windows/`.
- `build/` is for distributable outputs only. Do not move runtime source, original `assets/`, or `data/` into `build/`.
- `build/` is ignored by Git. Final build outputs stay local and must not be pushed to `main`.
- Verification commands should be added as the Lua project is implemented, such as Lua syntax checks, data load checks, and LÖVE smoke runs.
