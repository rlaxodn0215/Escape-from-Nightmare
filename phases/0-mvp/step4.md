# Step 4: inventory-items

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/UI_GUIDE.md`
- `/design/01_GAME_SYSTEMS_UI_RULES.txt`
- `/design/03_PUZZLES_ITEMS_EVENTS.txt`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/src/systems/interaction_system.lua`
- `/data/room_objects.lua`

## 작업

아이템과 인벤토리의 기본 흐름을 구현한다.

1. `src/systems/inventory_system.lua`를 만든다.
2. `src/ui/inventory_ui.lua`를 만든다.
3. `data/items.lua`에 Stage 1 아이템 10개의 id, name, icon, type, 획득/사용 관계를 정의한다.
4. `room_objects.lua`의 item_pickup 오브젝트와 inventory system을 연결한다.
5. 인벤토리 버튼 클릭으로 패널을 열고, 아이템 선택 후 대상 오브젝트 클릭으로 사용 가능한 구조를 만든다.
6. 아이템 조합은 기본 구조만 만들고, 실제 조합식은 최소화한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. 아이템 획득이 inventory state에 반영되는지 확인한다.
2. 인벤토리 UI 열기/닫기와 아이템 선택이 작동하는지 확인한다.
3. 진행 저장이 생기지 않았는지 확인한다.
4. 성공 시 step 4를 `completed`로 바꾸고 inventory/data 결과를 `summary`에 적는다.

## 금지사항

- 획득 아이템을 파일에 저장하지 마라. 이유: 죽으면 처음부터 재시작해야 한다.
- 아이템 목록을 design과 다르게 임의 확장하지 마라. 이유: Stage 1 범위가 고정되어 있다.
