# PRD: Escape From Nightmares

## Goal

Build a playable Stage 1 MVP of a first-person point-and-click horror escape game in Unity 6.3.9f1 for Windows PC.

## Player Experience

- The player explores a nightmare-like family house using mouse clicks only.
- The core loop is exploration, item discovery, puzzle solving, monster pressure, hiding or evasion, final chase, and escape.
- Horror should come from footsteps, breathing, lighting changes, silhouettes, distorted objects, and pressure rather than frequent jump scares.

## MVP Scope

- Implement Stage 1 only.
- Start from `child_room`.
- Allow room movement through door objects and screen-edge navigation.
- Support inventory, map, settings, puzzle screens, hiding, monster pressure, game over, restart, and ending.
- Clear condition: solve the basement altar puzzle, obtain `front_door_key`, survive the final chase, and escape through the front door.
- Save only settings and `stage1_clear`.

## Technical Scope

- Use Unity 6.3.9f1, C#, URP 2D, Input System, UGUI Canvas, EventSystem, and TextMeshPro.
- Keep Stage 1 content data-driven with ScriptableObject assets or Serializable C# data.
- Build for Windows PC.
- Use replaceable placeholder assets when final hand-drawn resources are absent.

## Out of Scope

- Additional stages beyond Stage 1.
- Progress saves, checkpoints, or autosaves.
- Web, Ren'Py, HTML5, or JavaScript implementations.
- Long story text, visible protagonist rendering, or invented rooms and characters.
- Final hand-drawn production assets when they are unavailable; use replaceable dummy assets instead.

## Source Details

- Project overview: `design/00_PROJECT_OVERVIEW.txt`
- UI, input, and save rules: `design/01_GAME_SYSTEMS_UI_RULES.txt`
- Completion criteria: `design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
