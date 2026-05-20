# Stage 1 Manual Tuning Checklist

Saved at: 2026-05-20

## Current Confidence

- Windows build output exists at `EscapeFromNightmares/Build/Windows/EscapeFromNightmares.exe`.
- Generated-player startup smoke starts and logs without error-pattern matches.
- Unity automated tests exist and pass:
  - 6 EditMode smoke tests.
  - 7 PlayMode smoke/deep-flow tests.
- The finale data chain has automated runtime coverage:
  - `event_front_door_key_appears`
  - `event_final_chase_trigger`
  - `event_stage1_clear`
  - `event_player_captured`

## Completion Decision

Do not mark Stage 1 fully complete yet.

Do not delete heartbeat automation `escape-harness-next-step` yet.

The remaining work is no longer broad implementation; it is focused tuning and deterministic validation.

## Checklist

### 1. Hit-Area Tuning

Goal: Confirm every clickable area is fair, invisible, and does not require pixel hunting.

Method:
- Run the generated Windows player visibly.
- Visit each room.
- Click every intended door, edge, item, clue, puzzle object, hide spot, and escape door.
- Confirm nearby non-interactive background clicks do not trigger actions.

Pass criteria:
- Player can reasonably find each interactable by visual inspection.
- No visible hover highlights or obvious clickable markers appear.
- Hit areas do not overlap in a way that triggers the wrong object.
- Screen-edge movement areas feel reachable without stealing room-object clicks.

Primary edit targets:
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Interactables/*.asset`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1InteractableDefinitionSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`

Known risk:
- `stage1_interactable_definitions_validation.json` and `stage1_hotspot_scene_instances_validation.json` record hit areas as coarse placeholder values.

### 2. Map Marker Tuning

Goal: Confirm the current-room marker appears on the correct floor and roughly correct location for every Stage 1 room.

Method:
- Start from `child_room`.
- Open the map in every room after movement.
- Confirm floor selection and marker placement.
- Check first floor, second floor, basement, and attic separately.

Pass criteria:
- Marker appears on the expected floor.
- Marker does not cover floor controls or labels.
- Marker is close enough to the room footprint to avoid misleading the player.

Primary edit targets:
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1MapRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`

Known risk:
- Map marker positions are approximate seeded values.

### 3. Monster/Hiding Balance

Goal: Confirm monster pressure feels tense but not arbitrary.

Method:
- Trigger near-detection and chase states.
- Enter hiding from several rooms.
- Time how long capture pressure takes to become dangerous.
- Test hiding success when entering a hide spot promptly.
- Test failure when delaying too long.

Pass criteria:
- Near-detection gives a clear warning window.
- Chase is dangerous but survivable if the player reacts quickly.
- Hide duration does not feel too long for a repeated action.
- Capture gauge drains at a readable pace.
- Game over feels earned, not random.

Primary edit targets:
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1HidingRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1MonsterRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scenes/Stage1.unity`

Current baseline:
- `defaultHideSeconds`: 6
- `captureDecayPerSecond`: 0.25
- `nearCapturePressurePerSecond`: 0.08
- `chaseCapturePressurePerSecond`: 0.22

### 4. Puzzle And Finale Walkthrough

Goal: Confirm a player can clear Stage 1 through real interaction, not only direct event execution.

Method:
- Play from `child_room`.
- Collect required clues/items.
- Solve required puzzles in intended order.
- Activate basement altar.
- Pick up `front_door_key`.
- Survive final chase.
- Open the front door.

Pass criteria:
- No required item or clue is unreachable.
- Puzzle success/failure feedback is understandable.
- `front_door_key` appears only after the altar success path.
- Final chase starts after key pickup.
- Front door clear requires `front_door_key`.
- Stage clear sets `stage1_clear` and reaches Ending state.

Primary edit targets:
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Puzzles/*.asset`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Events/*.asset`
- `EscapeFromNightmares/Assets/ScriptableObjects/Stage1/Interactables/*.asset`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1EventRuntimeSeeder.cs`
- `EscapeFromNightmares/Assets/Scripts/Editor/Stage1InteractableDefinitionSeeder.cs`

Automated coverage already present:
- Direct event-chain tests pass in `Stage1DeepFlowPlayModeTests`.

Remaining gap:
- Click-by-click gameplay path is not automated.

### 5. Game-Over Restart

Goal: Confirm game over resets run state cleanly.

Method:
- Trigger capture during gameplay.
- Confirm GameOver UI appears.
- Press restart.
- Confirm the run returns to `child_room`.
- Confirm inventory, puzzle state, monster state, and runtime flags do not persist improperly.

Pass criteria:
- GameOver UI blocks gameplay input.
- Restart hides GameOver UI.
- Current room returns to `child_room`.
- No progress save/checkpoint is created.
- Settings and `stage1_clear` are the only persisted data.

Primary edit targets:
- `EscapeFromNightmares/Assets/Scripts/UI/GameOverUI.cs`
- `EscapeFromNightmares/Assets/Scripts/Core/GameStateManager.cs`
- `EscapeFromNightmares/Assets/Scripts/Core/SaveManager.cs`
- `EscapeFromNightmares/Assets/Scripts/Systems/EventRuntimeSystem.cs`

Automated coverage already present:
- `event_player_captured` sets GameOver and `StartStage1Run` returns to `child_room`.

Remaining gap:
- Button-driven restart is not yet click-tested.

### 6. Generated-Player Deterministic Smoke

Goal: Make build artifact validation finish without killing the process.

Method:
- Add a command-line smoke mode or quit hook only if approved as a separate Harness unit.
- Run the generated player with that argument.
- Confirm it loads Stage1, advances enough frames to initialize systems, writes a success line, and exits with code 0.

Pass criteria:
- Player exits on its own.
- Log includes a deliberate smoke success line.
- No error-pattern matches appear.
- No manual process termination is needed.

Primary edit targets:
- `EscapeFromNightmares/Assets/Scripts/Core`
- `EscapeFromNightmares/Assets/Scripts/Editor/BuildScript.cs`
- `reports/unity-validation/generated_player_smoke.log`

Current status:
- Headless smoke starts, creates a log, and has no error-pattern matches.
- It does not exit naturally within the current smoke window.

## Recommended Next Unit

`Stage1HitAreaTuningPass`

Scope:
- Tune interactable hit areas room by room using the current generated player and Stage1 scene.
- Keep visuals invisible.
- Do not tune monster balance or map markers in the same unit.

Reason:
- Hit areas are the highest-risk remaining player-facing tuning gap because they directly affect whether Stage 1 can be completed through normal point-and-click interaction.
