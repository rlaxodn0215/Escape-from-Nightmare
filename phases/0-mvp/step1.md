# Step 1: project-skeleton

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/phases/0-mvp/index.json`

이전 step의 `summary`를 읽고 환경/검증 스크립트 결정을 따른다.

## 작업

LÖVE 2D 프로젝트의 최소 실행 골격을 만든다.

1. `main.lua`와 `conf.lua`를 생성한다.
2. `src/core/`, `src/scenes/`, `src/systems/`, `src/ai/`, `src/ui/`, `data/`, `assets/`, `saves/`, `build/` 구조를 만든다.
3. `src/core/state_manager.lua`, `src/core/game.lua`, `src/core/input.lua`, `src/core/save_manager.lua`의 최소 인터페이스를 만든다.
4. `main.lua`는 state manager를 통해 title scene 또는 최소 placeholder scene을 실행할 수 있어야 한다.
5. 설정/클리어 기록 저장 외 진행 저장 구조는 만들지 않는다.

## Acceptance Criteria

```bash
luac -p main.lua
```

환경에서 가능한 경우:

```bash
love .
```

## 검증 절차

1. LÖVE 프로젝트가 에러 없이 로드되는지 확인한다.
2. 폴더 구조가 `/docs/ARCHITECTURE.md`와 맞는지 확인한다.
3. `saves/`에는 설정/클리어 기록 이외 진행 저장을 만들지 않았는지 확인한다.
4. 성공 시 step 1을 `completed`로 바꾸고 생성된 골격을 `summary`에 적는다.

## 금지사항

- 웹/Unity/Ren'Py/HTML5/JavaScript 구조를 만들지 마라. 이유: 확정 기술 스택은 LÖVE 2D/Lua다.
- 게임 진행 저장 파일을 만들지 마라. 이유: 진행 저장은 CRITICAL 금지사항이다.
