# Step 3: room-navigation

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/design/02_STAGE1_SPACE_ROOMS.txt`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/src/scenes/game_scene.lua`
- `/src/core/input.lua`

## 작업

Stage 1 방 이동의 최소 동작을 구현한다.

1. `src/systems/room_system.lua`와 `src/systems/interaction_system.lua`를 만든다.
2. `data/rooms.lua`에 Stage 1 방 ID, 층, 연결, 배경 경로 placeholder를 정의한다.
3. `data/room_objects.lua`에 door, edge navigation, locked_door, escape_door 등 최소 클릭 오브젝트 구조를 만든다.
4. 모든 클릭 판정은 `{x, y, w, h}` 사각형 hitbox를 사용한다.
5. 시작 위치는 `child_room`이어야 한다.
6. 방 이동은 문 오브젝트 클릭과 화면 가장자리 클릭 모두를 지원하는 구조여야 한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. child_room에서 시작하는지 확인한다.
2. 최소 2개 이상의 방을 클릭 이동할 수 있는지 확인한다.
3. 방 이동 데이터가 코드에 하드코딩되지 않고 `data/rooms.lua`, `data/room_objects.lua` 중심인지 확인한다.
4. 성공 시 step 3을 `completed`로 바꾸고 이동 시스템과 데이터 파일을 `summary`에 적는다.

## 금지사항

- 클릭 가능 오브젝트를 화면에 하이라이트하지 마라. 이유: 플레이어가 직접 관찰해야 한다.
- design에 없는 방을 추가하지 마라. 이유: Stage 1 범위를 벗어난다.
