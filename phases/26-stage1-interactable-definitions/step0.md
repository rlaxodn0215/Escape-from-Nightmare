# Step 0 - Stage1InteractableDefinitions

## Pre-Implementation Proposal

Seed Stage 1 `InteractableDefinition` assets under `Assets/ScriptableObjects/Stage1/Interactables` and assign each asset to its owning `RoomDefinition.interactableDefinitions` array. The unit prepares room-owned interactable data for later scene hotspot placement.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1InteractableDefinitions`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Room-specific hotspot prefab instances in `Stage1.unity`.
- Puzzle-solving runtime behavior.
- Event execution runtime behavior.
- Monster and hiding runtime systems.
- Final hit-area tuning against finished room art.
- Visible hover highlights or obvious clickable markers.

## Validation Results

- Passed: `reports/unity-validation/stage1_interactable_definitions_validation.json`
- Unity MCP `refresh_unity` compiled the added editor seeder with no C# compiler errors observed.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Interactable Definitions`.
- Static review confirmed 53 `.asset` files under `Assets/ScriptableObjects/Stage1/Interactables`.
- Static review confirmed 27 room assets checked and 23 rooms now have interactable references; four rooms are intentionally empty for this unit: `attic_main`, `attic_album_storage`, `basement_storage`, and `first_floor_bathroom`.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1InteractableDefinitionSeeder.cs`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Interactables`
- `reports/unity-validation/stage1_interactable_definitions_validation.json`

## Current State

- `Stage1InteractableDefinitionSeeder` creates and updates 53 Stage 1 interactable definition assets.
- Seeded data includes 30 doors, 3 item pickups, 7 puzzle objects, 7 clue objects, 5 hide spots, and 1 escape door.
- Each seeded interactable has a room id, interactable id, type, and 1280x720 hit area.
- Item pickups reference existing item assets.
- Puzzle and escape interactables reference existing puzzle assets where applicable.
- Each seeded room owns its matching `InteractableDefinition` references through `RoomDefinition.interactableDefinitions`.
- Hit areas remain coarse data placeholders and should be tuned during room-specific hotspot placement.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `Stage1HotspotSceneInstances`.

## Next Action

Start `Stage1HotspotSceneInstances` to place room-specific hotspot prefab instances in `Stage1.unity` using the seeded interactable definition assets.
