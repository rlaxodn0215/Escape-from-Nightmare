# Step 2: scene-shells

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/UI_GUIDE.md`
- `/design/01_GAME_SYSTEMS_UI_RULES.txt`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/src/core/state_manager.lua`
- `/src/core/game.lua`
- `/main.lua`

## 작업

게임 흐름에 필요한 scene shell을 구현한다.

1. `src/scenes/title_scene.lua`, `game_scene.lua`, `pause_scene.lua`, `gameover_scene.lua`, `ending_scene.lua`를 만든다.
2. 각 scene은 `enter`, `update`, `draw`, `mousepressed` 형태의 일관된 인터페이스를 제공한다.
3. title scene에는 Start, Settings, Quit 클릭 영역을 둔다.
4. pause scene에는 Continue, Settings, Return to Title 흐름을 둔다.
5. gameover scene은 짧은 Game Over 표시 후 child_room에서 재시작할 수 있는 hook을 제공한다.
6. ending scene은 Stage 1 clear 후 바깥도 악몽임을 암시하는 최소 화면을 제공한다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. title에서 Start 클릭 시 game scene으로 이동하는지 확인한다.
2. pause/gameover/ending scene으로 전환 가능한 최소 hook이 있는지 확인한다.
3. UI가 클릭 오브젝트 하이라이트 규칙을 위반하지 않는지 확인한다.
4. 성공 시 step 2를 `completed`로 바꾸고 scene shell 결과를 `summary`에 적는다.

## 금지사항

- 장식적인 웹 UI 스타일을 넣지 마라. 이유: UI는 어두운 손그림풍 미니멀 스타일이어야 한다.
- 긴 스토리 텍스트를 추가하지 마라. 이유: 스토리 중심 게임이 아니다.
