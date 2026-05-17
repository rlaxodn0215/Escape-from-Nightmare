# Architecture Decision Records

## Philosophy

Ship a playable Stage 1 MVP first. Prefer simple, data-driven Unity systems that can run with placeholder assets and later accept final hand-drawn resources without changing game logic.

## ADR-001: Use Unity 6, URP 2D, and C#

**Decision**: Build the game with Unity 6.3.9f1, C#, and URP 2D for Windows PC.
**Reason**: The repository already contains a Unity project configured with URP 2D and the Input System.
**Tradeoff**: Do not use web, Ren'Py, HTML5, JavaScript, or unrelated game tooling for the MVP.

## ADR-002: Mouse Click Only Input

**Decision**: All player interaction uses mouse clicks through Unity's Input System and UI/EventSystem pipeline.
**Reason**: The game is a first-person point-and-click escape horror experience.
**Tradeoff**: Keyboard movement and action shortcuts are not part of the MVP.

## ADR-003: Data-Driven Stage Content

**Decision**: Define rooms, objects, items, puzzles, events, monster nodes, sounds, and Stage 1 rules as ScriptableObject assets or Serializable C# data.
**Reason**: Stage content needs to be readable, replaceable, testable, and expandable without tangling it with runtime systems.
**Tradeoff**: Initial data assets may be verbose, but they keep systems reusable.

## ADR-004: No Progress Saves

**Decision**: Save only settings and clear records. Never save run progress.
**Reason**: Death must restart Stage 1 from the beginning and preserve horror pressure.
**Tradeoff**: Players cannot resume midway through a run.

## ADR-005: Dummy Asset Fallbacks

**Decision**: Missing final images and sounds may be replaced by dummy sprites, generated placeholders, silent clips, or graceful no-audio fallbacks.
**Reason**: The project must remain runnable before final resources exist.
**Tradeoff**: Placeholder presentation is acceptable during implementation, but asset names and references must remain final-resource-compatible.

## ADR-006: Unity BatchMode for Verification

**Decision**: Use Unity BatchMode for EditMode tests, PlayMode tests, validation methods, and Windows builds when local Unity is available.
**Reason**: Unity project correctness depends on scenes, serialized assets, prefabs, and package state, not only C# compilation.
**Tradeoff**: Automation can be blocked on machines without a configured Unity editor path.

## ADR-007: Build Outputs Stay Generated

**Decision**: Write Windows player builds to a local ignored build output folder such as `Build/Windows/`.
**Reason**: Final outputs must be easy to find and separate from source files.
**Tradeoff**: Release distribution needs a separate artifact upload or local handoff step.

## ADR-008: Do Not Track Generated Unity Outputs

**Decision**: Do not track generated Unity folders or player builds. Publish source, docs, scripts, ScriptableObject assets, prefabs, placeholder assets, and phase metadata.
**Reason**: Generated artifacts are environment-specific and can be recreated locally.
**Tradeoff**: Reproducible builds require Unity version and package versions to stay pinned.
