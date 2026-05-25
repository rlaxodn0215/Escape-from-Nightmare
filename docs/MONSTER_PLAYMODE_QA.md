# Monster Play Mode QA

Date: 2026-05-25

## Result

Ready for manual Play Mode QA with one QA seed placement. This seed is only for runtime visibility verification and is not final monster placement.

## Catalog Check

- Total entries: 76
- Enabled entries: 1
- Ready entries: 1
- Ready definition: `enabled=true` and `normalizedRect.width > 0` and `normalizedRect.height > 0`

## QA Seed Placement

- Room/face: `kitchen / North`
- Serialized value: `roomId=kitchen`, `faceDirection=0`
- `normalizedRect`: `x=0.42`, `y=0.10`, `width=0.18`, `height=0.62`
- Purpose: QA-only placement to let `placement ready` be reached in Play Mode.

## Build Check

- `dotnet build EscapeFromNightmares.csproj /p:UseSharedCompilation=false`: pass
- `dotnet build EscapeFromNightmares.Editor.csproj /p:UseSharedCompilation=false`: pass
- `dotnet build EscapeFromNightmares.EditModeTests.csproj /p:UseSharedCompilation=false`: pass

## QA Status

- F9 QA panel runtime code: build verified
- F10 monster state cycling runtime code: build verified
- F11 monster reset runtime code: build verified
- Play Mode visual monster placement acceptance: ready to repeat with the QA seed

## Manual QA

- Enter Play Mode.
- Move to `kitchen / North`.
- Press `F9` to show `MonsterRuntimeQaPanel`.
- Press `F10` until the state reaches `Approaching`, `Searching`, `NearDetection`, or `Chase`.
- Expected status: `placement ready`.
- Expected result: `MonsterImage` appears as a separate object over the room object layer.
