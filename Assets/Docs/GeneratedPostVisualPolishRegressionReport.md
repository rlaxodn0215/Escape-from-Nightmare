# Post Visual Polish Regression Report

- Generated At: 2026-05-29 23:15:10
- Editor Safe Checks Invoked: Visual Resource Validator, Art Resource Binding Validator
- Play Mode Tests: Not chained by this menu; run through UnityMCP or the individual test menus.

## Required Execution Order

1. Escape From Nightmare / Validate Game Data
2. Escape From Nightmare / Validate Puzzle Prefab Contracts
3. Escape From Nightmare / Validate Source Route Scene Wiring
4. Escape From Nightmare / Visual Polish / Validate Visual Resources
5. Escape From Nightmare / Art Resources / Validate Art Resource Bindings
6. Escape From Nightmare / Tests / Run First Five Puzzle Runtime Tests
7. Escape From Nightmare / Tests / Run Remaining Puzzle Runtime Tests
8. Escape From Nightmare / Tests / Run Full Game Route Runtime Test
9. Escape From Nightmare / Tests / Run GameScene Interaction Runtime Tests

## Latest Report Files

| Report | Exists | Summary |
|---|---:|---|
| Game Data | No | Report file is missing. |
| Puzzle Prefab Contracts | No | Report file is missing. |
| Source Route Scene Wiring | Yes | Errors: 0; Warnings: 0 |
| Visual Resources | Yes | Errors: 0; Warnings: 45; Required Found: 0; Required Missing: 45 |
| Art Resource Bindings | Yes | Errors: 0; Warnings: 45 |
| First Five Runtime | Yes | Total: 5; Passed: 5; Failed: 0 |
| Remaining Runtime | Yes | Total: 4; Passed: 4; Failed: 0 |
| Full Route Runtime | Yes | Total Steps: 13; Passed: 13; Failed: 0 |
| GameScene Interaction Runtime | Yes | Total: 39; Passed: 39; Failed: 0 |

## Notes

- Runtime test menus enter Play Mode, so they are safer to run one at a time through UnityMCP.
- Missing Sprite warnings are expected before actual art files are placed in `Assets/ArtIntake` or `Assets/Resources`.
- This report intentionally does not modify Scene, Prefab, or JSON data.
