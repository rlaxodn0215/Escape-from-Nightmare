# Repository Guidelines

## Project Structure & Module Organization

This is a Unity 6 project for `Escape_from_Nightmares`, using Unity `6000.3.9f1`, URP 2D, uGUI, and the Unity Input System. Game-specific content lives under `Assets/EscapeFromNightmares/`; keep runtime scripts in `Scripts/Runtime`, data definitions in `Scripts/Data`, services in `Scripts/Services`, and tests in `Tests/EditMode` or `Tests/PlayMode`. Keep shared Unity scenes and render settings in `Assets/Scenes` and `Assets/Settings`. Do not hand-edit generated `*.csproj`, `*.slnx`, `Library/`, `Logs/`, or `UserSettings/` content.

## Build, Test, and Development Commands

Open the project with Unity Hub or the matching editor version:

```powershell
Unity.exe -projectPath "D:\Game\Project\Escape_from_Nightmares"
```

Run EditMode tests in batch mode:

```powershell
Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults TestResults/EditMode.xml
```

Run PlayMode tests similarly with `-testPlatform PlayMode`. For local validation, open `Assets/Scenes/SampleScene.unity`; the runtime bootstrap should create the prototype UI automatically.

## Coding Style & Naming Conventions

Use C# conventions common to Unity: four-space indentation, `PascalCase` for classes, methods, properties, and public serialized fields, and `camelCase` for private fields and locals. Prefer `[SerializeField] private` over public fields unless another component must access the member. Name MonoBehaviour files exactly after the class, for example `PlayerController.cs`. Keep one primary type per file and avoid broad manager classes unless they own a clearly defined system.

## Testing Guidelines

Use Unity Test Framework from `com.unity.test-framework`. Put fast logic and editor utility tests in `Assets/EscapeFromNightmares/Tests/EditMode`; use PlayMode tests for scene, input, timing, and UI behavior. Name tests after behavior, such as `Puzzle_TrySolve_GrantsRewardAndFlag`. Add or update tests when changing gameplay rules, save data, input actions, or shared systems.

## Commit & Pull Request Guidelines

Recent history uses short imperative commits such as `init project` and occasional conventional prefixes like `chore:`. Keep commits focused and write subjects in the imperative form, for example `add player movement controller` or `chore: update URP settings`. Pull requests should include a concise summary, affected scenes/assets, test results, and screenshots or short captures for visible gameplay or UI changes. Mention linked issues when applicable.

## Asset & Configuration Notes

Keep `.meta` files with their assets so Unity GUID references remain stable. Manage dependencies through `Packages/manifest.json` and `Packages/packages-lock.json`; avoid committing local editor cache output from `Library/` or `Logs/`. Do not add progress saves; only `settings.json` and `clear_records.json` are allowed runtime save files.

## Feature Deliverable Rules

When a requirement, visual detail, puzzle rule, room layout, story beat, or harness behavior is unknown or ambiguous, do not invent a final answer. State the uncertainty and ask the user before locking it into a plan, asset, scene, prefab, or runtime rule. Temporary placeholders are allowed only when they are clearly labeled as placeholders and do not imply final lore, final art, or final gameplay rules.

Classify every feature before implementation. Pure logic, save data, calculations, service classes, and tests may be `Script only`. Anything the player sees or clicks at runtime, including UI panels, buttons, interactable objects, repeated scene elements, hiding spots, and monster nodes, must include the required Prefab or Scene work, not just scripts. Independent entry screens or flows that need a camera, Canvas, EventSystem, or AudioListener must include Scene or scene-builder changes.

When new images or audio are needed, load them through `ResourcePathCatalog` or another ScriptableObject path catalog rather than direct component references. If a feature needs repeatable generation or recovery of scenes, prefabs, or dummy resources, include an Editor builder menu item. Feature plans must explicitly state whether they require `Script only`, `Script + Prefab`, `Script + Scene`, `ScriptableObject asset`, `ResourceCatalog update`, or `Editor builder required`.

Choose scene placement deliberately. Use direct Scene placement for one-off objects that belong only to a specific stage scene. Use an Editor builder when an entry screen, UI prefab, default scene infrastructure, or dummy resource set must be recreated safely by another developer. Do not rely on runtime-only generated UI as the final deliverable for visible gameplay or menu features.
