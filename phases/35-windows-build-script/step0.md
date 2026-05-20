# Step 0 - WindowsBuildScript

## Pre-Implementation Proposal

Add a batchmode-friendly `BuildScript.BuildWindows` entry point under Unity Editor scripts. The method should build enabled Build Settings scenes for Windows x64 to a local generated output path, fail loudly when no scenes are configured or the build fails, and remain callable through Unity `-executeMethod BuildScript.BuildWindows`.

## User Decisions

- `approved_for_implementation`: The user has instructed Codex to continue Harness work automatically through the recurring next-step automation.

## Decision Log

- 2026-05-20: Started exactly one next Harness unit, `WindowsBuildScript`, because `phases/index.json` had no `in_progress` or `blocked` phase and `NEXT_ACTION.md` recommended this unit.

## Out of Scope

- Generating the final Windows player build.
- Changing gameplay content.
- Creating new UI or gameplay prefabs.
- Adding smoke tests.
- Changing build output tracking policy.

## Validation Results

- Passed: `reports/unity-validation/windows_build_script_validation.json`
- Unity MCP script validation reported zero diagnostics for `BuildScript.cs`.
- Unity MCP build scene inspection confirmed `Assets/Scenes/Stage1.unity` is enabled.
- Unity asset refresh generated `BuildScript.cs.meta`.
- Static inspection confirmed `public static class BuildScript`, `public static void BuildWindows`, `StandaloneWindows64`, `BuildPipeline.BuildPlayer`, and `Build/Windows` output usage.
- Actual player build was not run in this unit by design.

## Review Artifact

- `EscapeFromNightmares/Assets/Scripts/Editor/BuildScript.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/BuildScript.cs.meta`
- `reports/unity-validation/windows_build_script_validation.json`
- `EscapeFromNightmares/ProjectSettings/EditorBuildSettings.asset`

## Current State

- `BuildScript.BuildWindows` is now callable through Unity `-executeMethod BuildScript.BuildWindows`.
- It collects enabled Build Settings scenes and fails if none exist.
- It targets `StandaloneWindows64`.
- It writes generated output to `Build/Windows/EscapeFromNightmares.exe` under the Unity project root.
- It throws if Unity reports a failed build result.
- The final Windows player build has not been generated yet.

## Resume Instructions

This phase is complete. Resume from `phases/index.json`; if there is no active or blocked phase, start one new Harness unit for `WindowsPlayerBuild`.

## Next Action

Start `WindowsPlayerBuild` to run `BuildScript.BuildWindows` and verify the generated Windows player output.
