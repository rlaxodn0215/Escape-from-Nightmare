# Step 0 - Stage1HotspotSceneInstances

## Pre-Implementation Proposal

Place room-specific hotspot prefab instances in `Assets/Scenes/Stage1.unity` from the seeded `InteractableDefinition` assets. Keep the required `InteractableLayer` root, add a dedicated transparent `HotspotCanvas` beneath it for UGUI click handling, group instances by room, and activate only the current room group through `RoomSystem.RoomChanged`.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `Stage1HotspotSceneInstances`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Puzzle solving UI flow and answer validation.
- Event runtime effects.
- Monster AI and hiding gameplay.
- Final art-accurate hit-area tuning.
- Visible clickable markers or hover highlights.

## Validation Results

- Passed: `reports/unity-validation/stage1_hotspot_scene_instances_validation.json`
- Unity MCP `refresh_unity` compiled the added runtime and editor scripts with no C# compiler errors observed.
- Unity MCP `execute_menu_item` ran `Escape From Nightmares/Seed Stage1 Hotspot Scene Instances`.
- Unity MCP found `HotspotCanvas` and confirmed it has `Canvas`, `CanvasScaler`, `GraphicRaycaster`, and `RoomHotspotLayer`.
- Unity MCP found 53 scene objects with `InteractableHotspot`.
- Static scene review confirmed 23 `Room_*` groups, 53 serialized definition references, and 53 serialized `InteractionSystem` references.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Systems/RoomHotspotLayer.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1HotspotSceneInstanceSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`
- `reports/unity-validation/stage1_hotspot_scene_instances_validation.json`

## Current State

- `InteractableLayer/HotspotCanvas` exists in `Stage1.unity`.
- `HotspotCanvas` has a 1280x720-scaled transparent UGUI canvas and graphic raycaster.
- 53 hotspot scene instances are grouped under 23 room groups.
- 48 instances use `InteractableHotspot.prefab`; 5 hide spot instances use `HideSpot.prefab`.
- No `ScreenEdgeHotspot.prefab` instances were created because the current seeded definitions use door-type movement interactables rather than `ScreenEdge` definitions.
- Each hotspot instance references its `InteractableDefinition` and the scene `InteractionSystem`.
- `RoomHotspotLayer` subscribes to `RoomSystem.RoomChanged` and activates only the active room group.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `PuzzleRuntimeSystem`.

## Next Action

Start `PuzzleRuntimeSystem` to open `PuzzleUI` from puzzle interactables and validate `PuzzleDefinition` answers.
