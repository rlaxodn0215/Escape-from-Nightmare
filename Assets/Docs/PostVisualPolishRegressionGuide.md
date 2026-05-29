# Post Visual Polish Regression Guide

## Purpose

Run this sequence after placeholder polish or art resource changes to make sure visual binding work did not break gameplay.

## Execution Order

1. `Escape From Nightmare / Validate Game Data`
2. `Escape From Nightmare / Validate Puzzle Prefab Contracts`
3. `Escape From Nightmare / Validate Source Route Scene Wiring`
4. `Escape From Nightmare / Visual Polish / Validate Visual Resources`
5. `Escape From Nightmare / Art Resources / Validate Art Resource Bindings`
6. `Escape From Nightmare / Tests / Run First Five Puzzle Runtime Tests`
7. `Escape From Nightmare / Tests / Run Remaining Puzzle Runtime Tests`
8. `Escape From Nightmare / Tests / Run Full Game Route Runtime Test`
9. `Escape From Nightmare / Tests / Run GameScene Interaction Runtime Tests`

## Success Criteria

- Compile Error: 0
- Validator Errors: 0
- Runtime Test Failed: 0
- Missing Sprite warnings are allowed before final art files are delivered.

## UnityMCP Recovery

If UnityMCP disconnects, reconnect or reopen Unity, confirm the active instance, then rerun the sequence one menu at a time. Treat transport logs separately from project Console errors.
