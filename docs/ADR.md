# Architecture Decision Records

## Philosophy
Ship a playable Stage 1 MVP first. Prefer simple, data-driven Lua systems that can run with dummy assets and later accept final hand-drawn resources without changing game logic.

## ADR-001: Use LÖVE 2D and Lua
**Decision**: Build the game with LÖVE 2D and Lua for Windows PC.  
**Reason**: The design explicitly targets a lightweight 2D horror escape game and final `.love` / Windows exe packaging.  
**Tradeoff**: Do not use web, Unity, Ren'Py, HTML5, JavaScript, or browser game tooling.

## ADR-002: Mouse Click Only Input
**Decision**: All player interaction uses mouse clicks.  
**Reason**: The game is a first-person point-and-click escape horror experience.  
**Tradeoff**: Keyboard movement and action shortcuts are not part of the MVP.

## ADR-003: Data-Driven Stage Content
**Decision**: Define rooms, objects, items, puzzles, events, monster nodes, sounds, and Stage 1 rules in `data/*.lua`.  
**Reason**: Stage content needs to be readable, replaceable, and expandable without tangling it with runtime systems.  
**Tradeoff**: Initial data files may be verbose, but they keep systems reusable.

## ADR-004: No Progress Saves
**Decision**: Save only settings and clear records. Never save run progress.  
**Reason**: Death must restart Stage 1 from the beginning and preserve horror pressure.  
**Tradeoff**: Players cannot resume midway through a run.

## ADR-005: Dummy Asset Fallbacks
**Decision**: Missing final images and sounds may be replaced by dummy images or silent fallbacks.  
**Reason**: The project must remain runnable before final resources exist.  
**Tradeoff**: Placeholder presentation is acceptable during implementation, but asset paths must remain final-resource-compatible.
