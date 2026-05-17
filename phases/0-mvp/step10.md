# Step 10: hiding-system

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/UI_GUIDE.md`
- `/design/02_STAGE1_SPACE_ROOMS.txt`
- `/design/04_MONSTER_HIDING_AI.txt`
- `/src/ai/danger.lua`
- `/src/ai/monster.lua`
- `/data/room_objects.lua`

## 작업

은신 시스템과 은신 위험 게이지 UI를 구현한다.

1. `src/systems/hiding_system.lua`와 `src/ui/danger_gauge_ui.lua`를 만든다.
2. `hide_spot` 타입 오브젝트 클릭 시 은신 상태로 진입한다.
3. 마우스 움직임의 속도/가속도 기반으로 `hide_noise`와 위험 게이지가 변하도록 한다.
4. 몬스터가 가까운 상태에서 게이지가 높으면 발각될 수 있게 한다.
5. 몬스터가 지나간 뒤 일정 시간이 지나면 은신 해제 가능 hook을 둔다.
6. living_room 은신 포인트는 final chase에서 사용할 수 있어야 한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. hide_spot 클릭으로 은신 상태에 들어가는지 확인한다.
2. 마우스 움직임이 위험 게이지에 반영되는지 확인한다.
3. 은신 실패가 gameover 또는 chase 유지로 연결되는지 확인한다.
4. 성공 시 step 10을 `completed`로 바꾸고 hiding system 결과를 `summary`에 적는다.

## 금지사항

- 단순 이동 여부만으로 은신 실패를 판정하지 마라. 이유: 설계 기준은 마우스 움직임 가속도다.
- 모든 방에 은신 장소를 만들지 마라. 이유: 방별 은신 개수는 design에 따른다.
