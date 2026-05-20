# Step 0: resource-inventory

## Game Unit

- unit_type: resource
- unit_id: stage1_required_assets
- requires_user_design_approval: true

## Read Before Work

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/docs/CODEX_HARNESS.md`
- `/design/06_RESOURCES_LIST.txt`
- `/design/08_REMAINING_TASKS.txt`

## Pre-Implementation Proposal

Inventory every required Stage 1 resource from `design/06_RESOURCES_LIST.txt`, map each filename to a Unity asset path, and create replaceable placeholder assets for the first pass. This unit does not create gameplay scripts, prefabs, ScriptableObjects, or scenes.

## User Decisions

- Resource strategy: placeholder-first.
- Placeholder filenames must match final replacement filenames.
- `fuse_holder` versus `item_electric_part.png` was resolved by preserving design item ID `fuse_holder` and using `item_electric_part.png` as its icon filename.
- Audio target files were changed from OGG placeholders to final WAV target assets.
- User approved all remaining approval-gated resource work and requested completion without separate approvals.

## Out of Scope

- C# runtime systems.
- Unity scenes and Build Settings changes.
- Prefabs and prefab references.
- ScriptableObject data authoring.
- Final hand-drawn art or final audio production.

## Resource Inventory

- Manifest artifact: `resource_manifest.json`.
- Final images are written under `EscapeFromNightmares/Assets/Sprites`.
- Final audio target files are written under `EscapeFromNightmares/Assets/Audio`.
- Validation reports are written under `reports/unity-validation`.
- Final resource status: 129 / 129 `final`.

## Unity MCP Checks

Unity MCP became callable during the resource completion pass.

- Instance: `EscapeFromNightmares@c6f9dc4f`
- Unity version: `6000.3.9f1`
- Project root: `D:/Game/Project/Escape_from_Nightmares/EscapeFromNightmares`
- PASS: `Assets/Sprites/Monster` reimported successfully.
- PASS: `Assets/Sprites/UI` reimported successfully.
- PASS: Unity MCP found 7 Texture2D assets under `Assets/Sprites/Monster`.
- PASS: Unity MCP found 23 Texture2D assets under `Assets/Sprites/UI`.
- PASS: Unity MCP found 67 Texture2D assets under `Assets/Sprites`, matching all room, item, monster, and UI image resources.

## Tooling Gap

Unity MCP is now available for asset checks. Scene hierarchy, prefab reference, serialized reference, missing component, and Build Settings checks still belong to later scene/prefab/build validation units.

## Acceptance Criteria

- `ResourceManifestValidation` includes every required filename from `design/06_RESOURCES_LIST.txt`.
- `RequiredAssetPresenceValidation` confirms a file exists at every target path.
- `ZeroByteAssetValidation` confirms no placeholder file is empty.
- `UnityImportableAssetValidation` checks file extensions and reports any encoder/importability limitations.
- `DesignResourceConsistencyValidation` records known naming mismatches without silently changing design intent.

## Review Artifact

- `resource_manifest.json`
- `reports/unity-validation/resource_inventory_validation.json`
- `reports/unity-validation/final_resource_inventory_validation.json`
- `reports/unity-validation/remaining_visual_resources_validation.json`
- `EscapeFromNightmares/Assets/Sprites/**`
- `EscapeFromNightmares/Assets/Audio/**`

## Validation Results

- PASS: `ResourceManifestValidation` includes every required filename from `design/06_RESOURCES_LIST.txt`.
- PASS: `RequiredAssetPresenceValidation` confirms files exist at all target paths.
- PASS: `ZeroByteAssetValidation` confirms no target resource file is empty.
- PASS: `UnityImportableAssetValidation` for sprites was checked through Unity MCP import/search.
- PASS: `DesignResourceConsistencyValidation` records the `fuse_holder` / `item_electric_part.png` decision.
- PASS: All required resources are `final`.

## Current State

- Status: `completed`.
- Resource inventory is complete.
- No resource placeholders remain.

## Resume Instructions

Read `phases/0-resource-inventory/handoff.md`, `phases/0-resource-inventory/index.json`, and `resource_manifest.json`. Continue with Unity validation/tooling or the next approved Harness unit.

## Next Action

Open the next Harness unit after resource inventory.

## Next Step Blocker

Do not begin C# systems, scenes, prefabs, or ScriptableObjects unless the next Harness unit is opened and validation requirements are satisfied.
