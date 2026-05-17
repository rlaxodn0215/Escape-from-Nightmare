# Step 11: stage1-integration

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/UI_GUIDE.md`
- `/design/00_PROJECT_OVERVIEW.txt`
- `/design/03_PUZZLES_ITEMS_EVENTS.txt`
- `/design/04_MONSTER_HIDING_AI.txt`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/data/stage1.lua`
- `/src/scenes/game_scene.lua`

## 작업

Stage 1을 처음부터 끝까지 플레이 가능한 MVP로 연결한다.

1. child_room 시작, 아이템/퍼즐 체인, 몬스터 첫 등장, 전기 복구, 지하실 해금, 제단 퍼즐, front_door_key 생성, 최종 추격, 현관 탈출을 연결한다.
2. `settings.json`과 `clear_records.json` 저장만 허용한다.
3. 잡히면 Game Over 후 child_room에서 처음부터 재시작한다.
4. 획득 아이템, 퍼즐 상태, 열린 잠금장치, 몬스터 상태는 restart 시 초기화한다.
5. dummy asset/sound fallback이 누락 리소스에서도 에러 없이 동작하도록 연결한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. Start 클릭 후 child_room에서 시작하는지 확인한다.
2. 주요 체인이 final chase와 stage1_clear까지 이어지는지 확인한다.
3. Game Over 후 진행 상태가 초기화되는지 확인한다.
4. 진행 저장 파일이 생기지 않는지 확인한다.
5. 성공 시 step 11을 `completed`로 바꾸고 Stage 1 연결 결과를 `summary`에 적는다.

## 금지사항

- Stage 1 완료를 위해 design에 없는 shortcut을 만들지 마라. 이유: MVP의 핵심 체인을 검증해야 한다.
- 진행 상태를 저장하지 마라. 이유: CRITICAL 금지사항이다.
