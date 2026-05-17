# Step 6: stage-data

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/design/02_STAGE1_SPACE_ROOMS.txt`
- `/design/03_PUZZLES_ITEMS_EVENTS.txt`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/design/06_RESOURCES_LIST.txt`
- `/src/systems/room_system.lua`
- `/src/systems/inventory_system.lua`

## 작업

Stage 1 전체 진행에 필요한 data 파일을 채운다.

1. `data/puzzle_chains.lua`, `data/puzzle_inputs.lua`, `data/events.lua`, `data/monster_nodes.lua`, `data/sound_data.lua`, `data/stage1.lua`를 만든다.
2. 기존 `data/rooms.lua`, `data/room_objects.lua`, `data/items.lua`와 id가 일관되게 연결되도록 정리한다.
3. 리소스 경로는 `design/06_RESOURCES_LIST.txt`의 파일명 기준을 따른다.
4. 실제 좌표/리소스가 없으면 placeholder 값과 fallback 경로를 사용하되, 최종 리소스로 교체 가능한 구조로 둔다.
5. 데이터 로드 검증 스크립트가 있다면 모든 `data/*.lua`를 로드하도록 갱신한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

가능한 경우 데이터 로드 검증을 실행한다.

## 검증 절차

1. 모든 `data/*.lua`가 문법 오류 없이 로드되는지 확인한다.
2. item, puzzle, event, room, monster node id가 서로 끊기지 않았는지 확인한다.
3. resource 경로가 최종 교체 가능한 형태인지 확인한다.
4. 성공 시 step 6을 `completed`로 바꾸고 완성된 data 파일 범위를 `summary`에 적는다.

## 금지사항

- 정확하지 않은 최종 hitbox 좌표를 확정값처럼 쓰지 마라. 이유: 최종 방 이미지가 아직 없다.
- design에 없는 퍼즐이나 아이템을 추가하지 마라. 이유: 데이터 체인이 꼬인다.
