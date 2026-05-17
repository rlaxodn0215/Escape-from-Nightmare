# Step 0: environment-foundation

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/CODEX_HARNESS.md`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/design/08_REMAINING_TASKS.txt`

## 작업

LÖVE 2D 개발과 최종 패키징을 시작하기 위한 환경 기반을 만든다.

1. 현재 Windows 환경에서 사용할 수 있는 LÖVE 실행 파일 경로를 확인한다.
2. `luac` 또는 Lua 문법 검증을 대체할 수 있는 방법을 확인한다.
3. `build/`와 `build/windows/` 디렉토리를 생성한다.
4. 검증/패키징에 사용할 스크립트 위치를 정하고, 필요한 최소 스크립트를 작성한다.
5. LÖVE 실행 파일 경로나 문법 검증 방법을 찾을 수 없으면 추측하지 말고 `blocked`로 중단한다.

환경 설정은 repo에 LÖVE runtime을 복사하지 않는 방식이어야 한다. 로컬 경로 설정을 사용하라.

## Acceptance Criteria

```bash
git status --short
```

추가로, 환경에서 가능한 경우 아래 검증을 실행한다.

```bash
luac -v
love --version
```

## 검증 절차

1. `build/`와 `build/windows/`가 존재하는지 확인한다.
2. LÖVE 실행 파일 경로 또는 blocked 사유가 명확한지 확인한다.
3. Lua 문법 검증 방법 또는 blocked 사유가 명확한지 확인한다.
4. 성공 시 `phases/0-mvp/index.json`의 step 0을 `completed`로 바꾸고, `summary`에 확정된 환경/스크립트 정보를 한 줄로 적는다.
5. 사용자 결정이 필요하면 `blocked_reason`에 질문과 2~3개 선택지를 적고 `(Recommended)`를 표시한다.

## 금지사항

- LÖVE runtime을 repo에 임의로 복사하지 마라. 이유: 런타임 경로는 로컬 환경 설정으로 다룬다.
- LÖVE 경로나 문법 검사 방법을 상상해서 적지 마라. 이유: 이후 빌드 자동화가 실패한다.
- 실패/blocked 상태에서 직접 git commit 하지 마라. 이유: 성공한 step만 harness가 커밋한다.
