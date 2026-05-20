# Step 0: Map UI Prefab

## Pre-Implementation Proposal

- `unit_type`: `ui`
- `unit_id`: `MapUI.prefab`
- `requires_user_design_approval`: `true`
- Goal: create a reusable map UI prefab with floor views and a current-room marker placeholder, placed under `UICanvas` as an inactive scene instance.

## User Decisions

- 2026-05-19: User said to continue until the final result and that approvals should always be assumed.
- 2026-05-20: Automation requested the next Harness step.
- 2026-05-20: Standing approval was recorded as `approved_for_implementation` for this scoped UI unit.

## Out of Scope

- No HUD map button.
- No MapSystem runtime.
- No room-specific current-location calculation.
- No title, pause, settings, puzzle, hiding, or game-over prefabs.
- No room-specific hotspot placement.

## Decision Log

- 2026-05-20: Use existing `ui_map_panel`, floor sprites, and current-room marker sprite.
- 2026-05-20: Keep the prefab inactive in the scene until a later HUD map button unit opens it.
- 2026-05-20: Provide simple floor switching inside the prefab so the map surface is usable before MapSystem exists.

## Validation Results

- Unity MCP was unavailable at the start of this heartbeat unit, so Unity BatchMode fallback was used.
- Unity BatchMode ran `EscapeFromNightmares.Editor.MapUIPrefabSeeder.Seed`.
- BatchMode log contains `Seeded MapUI prefab and Stage1 scene instance.`
- BatchMode log exited with return code 0.
- Static prefab validation confirmed:
  - `MapUI.prefab` exists.
  - `MapUI`, `CanvasGroup`, close button, 4 floor buttons, 4 floor images, current-room marker, and floor label are serialized.
  - no missing script references were found.
- Static scene validation confirmed:
  - `UICanvas/MapUI` is present as a prefab instance.
  - the scene instance is inactive at start.

## Review Artifact

- `reports/unity-validation/map_ui_prefab_validation.json`

## Current State

Map UI prefab is complete for this unit. It contains a map panel, four floor views, floor-switch buttons, a current-room marker placeholder, and a close button. The Stage1 scene contains an inactive `UICanvas/MapUI` instance.

## Remaining Gaps

- HUD map button is not implemented yet.
- MapSystem runtime is not implemented yet.
- Current-room marker position is placeholder until room mapping exists.
- SettingsUI and other required UI prefabs are still not created.

## Resume Instructions

1. Treat `MapUI.prefab` as completed.
2. Continue with `HUDMapButton` or another single approved UI surface.

## Next Action

Open the next Harness unit.
