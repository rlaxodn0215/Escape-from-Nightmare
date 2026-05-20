# Repository Guidelines

## Project Structure & Module Organization

This repository contains a Unity project in `EscapeFromNightmares/`. Open that folder as the Unity project root. Runtime assets live under `EscapeFromNightmares/Assets/`: scenes in `Assets/Scenes`, URP and render settings in `Assets/Settings`, Naninovel configuration and generated data in `Assets/NaninovelData`, and scenario content in `Assets/Scenario`. Unity package dependencies are declared in `EscapeFromNightmares/Packages/manifest.json`; do not edit `Library/`, `Logs/`, `UserSettings/`, or generated `.csproj` files by hand.

## Build, Test, and Development Commands

Use Unity Editor `6000.3.9f1` as recorded in `EscapeFromNightmares/ProjectSettings/ProjectVersion.txt`.

```powershell
# Open the project from Unity Hub or command line
Unity.exe -projectPath .\EscapeFromNightmares

# Run edit mode tests in batch mode
Unity.exe -batchmode -projectPath .\EscapeFromNightmares -runTests -testPlatform editmode -quit

# Run play mode tests in batch mode
Unity.exe -batchmode -projectPath .\EscapeFromNightmares -runTests -testPlatform playmode -quit
```

For local development, prefer the Unity Editor Play button for scene validation, then run batch tests before submitting changes when test coverage exists.

## Coding Style & Naming Conventions

Use C# conventions for Unity scripts: four-space indentation, `PascalCase` for classes, methods, properties, and public serialized fields, and `camelCase` for locals and private fields. Name MonoBehaviours after their file names exactly, for example `NightmareController.cs` containing `NightmareController`. Keep scene, prefab, and asset names descriptive and stable because Unity references them by GUID-backed `.meta` files.

## Testing Guidelines

Use Unity Test Framework (`com.unity.test-framework`) for EditMode and PlayMode tests. Place future tests under `Assets/Tests/EditMode` or `Assets/Tests/PlayMode` and name test classes with a `Tests` suffix, such as `InventoryStateTests`. Commit test assets and their `.meta` files together. If a change is visual or narrative-only, document the scene or scenario path manually verified.

## Commit & Pull Request Guidelines

Recent history uses short imperative summaries such as `First Build` and `Upload necessary assets`; keep commits concise and focused. Use prefixes when useful, for example `chore: update Naninovel config` or `fix: correct scene transition`.

Pull requests should include a brief change summary, affected scenes or assets, test results, and screenshots or clips for visible gameplay/UI changes. Link related issues when available and call out Unity version, package, or project setting changes explicitly.

## Agent-Specific Instructions

Preserve Unity `.meta` files with their assets. Avoid committing transient folders (`Library`, `Logs`, `UserSettings`) or editor-generated solution files unless the project intentionally starts tracking them.
