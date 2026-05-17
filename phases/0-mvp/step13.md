# Step 13: love-package

## Resource Gate

Before creating `build/EscapeFromNightmares.love`, verify step 12 is completed and `assets/` contains real files. If `assets/` has zero files or any referenced resource path is missing, stop as `blocked` with objective choices instead of packaging.

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/CODEX_HARNESS.md`
- `/design/05_IMPLEMENTATION_STRUCTURE.txt`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/design/08_REMAINING_TASKS.txt`
- `/phases/0-mvp/index.json`

## 작업

배포용 `.love` 패키지를 만든다.

1. 필요한 경우 packaging script를 작성하거나 기존 script를 보강한다.
2. source root의 `main.lua`, `conf.lua`, `src/`, `data/`, `assets/` 등 LÖVE 실행에 필요한 파일만 archive에 포함한다.
3. `phases/`, `.git/`, `docs/`, `design/`, `scripts/`, `build/`, `saves/`의 런타임 불필요 파일은 `.love`에 포함하지 않는다. 단, 실제 구현이 runtime scripts를 필요로 하면 이유를 명시한다.
4. 최종 산출물은 `build/EscapeFromNightmares.love`에 저장한다.
5. 패키징 도구가 없으면 blocked 처리하고 객관식 선택지를 제시한다.

## Acceptance Criteria

```bash
dir build
```

환경에서 가능한 경우:

```bash
love build/EscapeFromNightmares.love
```

## 검증 절차

1. `build/EscapeFromNightmares.love`가 생성됐는지 확인한다.
2. archive에 runtime 불필요 파일이 들어가지 않았는지 확인한다.
3. LÖVE에서 `.love`가 실행 가능한지 확인한다.
4. 성공 시 step 13을 `completed`로 바꾸고 패키징 스크립트/산출물을 `summary`에 적는다.

## 금지사항

- `build/`를 소스의 소유 위치로 바꾸지 마라. 이유: build는 배포 산출물 전용이다.
- 실패한 패키지를 성공 산출물처럼 표시하지 마라. 이유: Windows 패키징의 입력이 된다.
