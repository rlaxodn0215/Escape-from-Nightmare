# Step 5: map-ui

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/UI_GUIDE.md`
- `/design/01_GAME_SYSTEMS_UI_RULES.txt`
- `/design/02_STAGE1_SPACE_ROOMS.txt`
- `/design/06_RESOURCES_LIST.txt`
- `/src/systems/room_system.lua`
- `/src/scenes/game_scene.lua`

## 작업

지도 UI와 현재 위치 표시를 구현한다.

1. `src/systems/map_system.lua`와 `src/ui/map_ui.lua`를 만든다.
2. 지도 버튼 클릭으로 지도 패널을 열고 닫을 수 있게 한다.
3. 1층, 2층, 지하실, 다락방을 단순 평면도 스타일로 표시하는 데이터 구조를 둔다.
4. 현재 방 위치 marker를 표시한다.
5. 미니맵은 실용적인 평면도여야 하며, 장식적이거나 화면을 과하게 가리지 않아야 한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. 지도 버튼으로 map UI가 열리고 닫히는지 확인한다.
2. 방 이동 후 현재 위치 marker가 갱신되는지 확인한다.
3. 일반 플레이 화면에 방 이름이 항상 표시되지 않는지 확인한다.
4. 성공 시 step 5를 `completed`로 바꾸고 map system/UI 결과를 `summary`에 적는다.

## 금지사항

- 지도를 과하게 장식하지 마라. 이유: 지도는 실용적인 평면도다.
- 일반 화면에 현재 방 이름을 항상 표시하지 마라. 이유: 위치는 지도에서 확인한다.
