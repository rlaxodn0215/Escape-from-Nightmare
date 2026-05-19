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
- `fuse_holder` versus `item_electric_part.png` is recorded as a design-resource consistency finding and is not renamed during this unit.

## Out of Scope

- C# runtime systems.
- Unity scenes and Build Settings changes.
- Prefabs and prefab references.
- ScriptableObject data authoring.
- Final hand-drawn art or final audio production.

## Resource Inventory

- Manifest artifact: `resource_manifest.json`.
- Placeholder images are written under `EscapeFromNightmares/Assets/Sprites`.
- Placeholder audio files are written under `EscapeFromNightmares/Assets/Audio`.
- Validation reports are written under `reports/unity-validation`.

## Unity MCP Checks

Unity MCP tools were not callable in this session. Scene hierarchy, prefab reference, missing component, serialized reference, and Build Settings checks were not claimed as passed.

## Tooling Gap

Because Unity MCP validation is unavailable, future scene/prefab/build validation should create a `unity-validation-tooling` system unit before relying on custom validation scripts.

## Acceptance Criteria

- `ResourceManifestValidation` includes every required filename from `design/06_RESOURCES_LIST.txt`.
- `RequiredAssetPresenceValidation` confirms a file exists at every target path.
- `ZeroByteAssetValidation` confirms no placeholder file is empty.
- `UnityImportableAssetValidation` checks file extensions and reports any encoder/importability limitations.
- `DesignResourceConsistencyValidation` records known naming mismatches without silently changing design intent.

## Review Artifact

- `resource_manifest.json`
- `reports/unity-validation/resource_inventory_validation.json`
- `EscapeFromNightmares/Assets/Sprites/**`
- `EscapeFromNightmares/Assets/Audio/**`

## Next Step Blocker

Do not begin C# systems, scene, prefab, or ScriptableObject implementation until this resource-inventory unit is reviewed and approved.

Current blocked reasons:

- OGG placeholder files are non-zero review markers, but they are not encoded Vorbis audio because no local OGG encoder was available.
- `design/03_PUZZLES_ITEMS_EVENTS.txt` uses `fuse_holder`, while `design/06_RESOURCES_LIST.txt` provides `item_electric_part.png`; this needs a user design decision before item/resource naming is normalized.
