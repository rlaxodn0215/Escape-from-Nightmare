# Step 8: monster-fsm

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/design/04_MONSTER_HIDING_AI.txt`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/data/monster_nodes.lua`
- `/data/events.lua`
- `/src/systems/room_system.lua`

## 작업

몬스터 FSM과 방/노드 단위 이동 기반을 구현한다.

1. `src/ai/fsm.lua`, `src/ai/monster.lua`를 만든다.
2. FSM 상태는 `NORMAL`, `APPROACHING`, `SEARCHING`, `NEAR_DETECTION`, `CHASE`를 지원한다.
3. monster는 현재 노드, 목표 노드, 마지막 소리 위치, 마지막 플레이어 목격 위치를 다룰 수 있어야 한다.
4. `event_kitchen_first_appearance`, `event_final_chase_trigger`, `event_player_captured` hook을 만든다.
5. 플레이어 위치는 방 단위, 몬스터 위치는 방/노드 단위로 분리한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. 몬스터 FSM 상태 전이가 코드상 확인 가능한지 확인한다.
2. kitchen 첫 등장 이벤트 이후 몬스터 시스템이 활성화될 수 있는지 확인한다.
3. final chase trigger가 CHASE 상태로 연결되는지 확인한다.
4. 성공 시 step 8을 `completed`로 바꾸고 FSM/monster 결과를 `summary`에 적는다.

## 금지사항

- 몬스터 이동을 화면 좌표 추적으로만 구현하지 마라. 이유: 설계는 방/노드 단위다.
- 몬스터가 들어갈 수 없는 안전 구역을 만들지 마라. 이유: 안전 구역은 없다.
