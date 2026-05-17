# Step 7: puzzle-system

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/UI_GUIDE.md`
- `/design/03_PUZZLES_ITEMS_EVENTS.txt`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/src/systems/inventory_system.lua`
- `/data/puzzle_inputs.lua`
- `/data/puzzle_chains.lua`
- `/data/events.lua`

## 작업

주요 퍼즐 입력과 성공/실패 처리를 구현한다.

1. `src/systems/puzzle_system.lua`와 `src/ui/puzzle_ui.lua`를 만든다.
2. number_lock, symbol_sequence, silent_sequence, color_sequence, symbol_item_matching, item_use 타입을 지원하는 구조를 만든다.
3. `study_safe`, `laundry_storage_box`, `mirror_symbol_panel`, `attic_toy_sequence`, `master_bedroom_drawer`, `basement_altar`, `front_door_escape`를 data 기반으로 처리한다.
4. 퍼즐 성공은 보상 아이템 또는 이벤트를 발생시킨다.
5. 퍼즐 실패는 즉사가 아니라 오류음/흔들림/위험도 증가 hook으로 연결한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. 최소 하나의 number_lock과 하나의 symbol/item matching 퍼즐을 수동으로 풀 수 있는지 확인한다.
2. 실패가 즉사로 이어지지 않는지 확인한다.
3. 성공 이벤트와 보상 아이템이 data 기반으로 연결되는지 확인한다.
4. 성공 시 step 7을 `completed`로 바꾸고 구현된 puzzle 타입을 `summary`에 적는다.

## 금지사항

- 퍼즐 정답을 시스템 코드에 흩뿌리지 마라. 이유: 정답은 `data/puzzle_inputs.lua`가 소유해야 한다.
- 오답 즉사를 만들지 마라. 이유: 오답은 위험도 증가로 처리한다.
