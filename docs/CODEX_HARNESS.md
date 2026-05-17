# Resource Gate

Packaging steps must never run against an empty `assets/` tree. Every implementation phase that leads to a playable build must include a resource fallback step before `.love` or Windows packaging.

The resource fallback step must:

1. scan data files for referenced `assets/images/...` and `assets/sounds/...` paths;
2. create valid placeholder files at those exact paths when final resources are not available;
3. verify `assets/` has at least one real file;
4. fail or block if any referenced resource is missing;
5. keep placeholders replaceable and avoid claiming they are final art or final audio.

If local tools cannot generate or verify a required image/audio format, the step must stop as `blocked` and include 2-3 objective choices in `blocked_reason`, with one marked `(Recommended)`.

# Codex Harness

이 프로젝트는 Codex 기반 Harness 워크플로우를 사용한다. Harness는 큰 작업을 phase와 step으로 나누고, 각 step을 독립 실행 가능한 지시서로 만들어 순차 실행한다.

`docs/*.md`는 step 실행 시 빠르게 주입할 압축 가드레일이다. `design/*.txt`는 상세 원문이며, 모든 step에서 전부 읽지 말고 현재 작업에 필요한 파일만 골라 읽는다.

## 워크플로우

### A. 탐색

`/docs/` 하위 문서(PRD, ARCHITECTURE, ADR, UI_GUIDE 등)를 읽고 프로젝트의 기획, 아키텍처, 설계 의도를 파악한다. 세부 구현이 필요한 경우 `AGENTS.md`의 Context Map을 따라 관련 `design/*.txt` 원문만 추가로 읽는다.

### B. 논의

구현 전에 구체화하거나 기술적으로 결정해야 할 사항이 있으면 사용자에게 제시하고 확정한다.

### C. Step 설계

구현 계획은 여러 step으로 나누어 작성한다.

설계 원칙:

1. **Scope 최소화**: 하나의 step에서는 하나의 레이어 또는 모듈만 다룬다. 여러 모듈을 동시에 수정해야 하면 step을 쪼갠다.
2. **자기완결성**: 각 step 파일은 독립된 Codex 세션에서 실행될 수 있어야 한다. 이전 대화를 전제로 하지 말고 필요한 정보를 파일 안에 적는다.
3. **사전 준비 강제**: 관련 문서 경로와 이전 step에서 생성 또는 수정된 파일 경로를 명시한다.
4. **시그니처 수준 지시**: 함수와 클래스의 인터페이스는 제시하되, 내부 구현은 해당 step의 목적과 규칙 안에서 결정하게 한다.
5. **실행 가능한 AC**: 추상적인 완료 조건 대신 Lua 문법 검사, 데이터 로드 검증, LÖVE smoke run처럼 실제 검증 커맨드를 포함한다.
6. **구체적인 금지사항**: "조심해라" 대신 "X를 하지 마라. 이유: Y" 형식으로 적는다.
7. **네이밍**: step name은 kebab-case slug로 작성한다. 예: `project-setup`, `api-layer`, `auth-flow`.
8. **모르는 부분 처리**: 상상하거나 임의 결정하지 않는다. 사용자 결정이 필요하면 `blocked_reason`에 질문과 2~3개의 객관식 선택지를 적고, 추천 선택지는 `(Recommended)`로 표시한다.

## Phase 파일 구조

### `phases/index.json`

여러 task를 관리하는 top-level 인덱스다. 이미 존재하면 `phases` 배열에 새 항목을 추가한다.

```json
{
  "phases": [
    {
      "dir": "0-mvp",
      "status": "pending"
    }
  ]
}
```

- `dir`: task 디렉토리명.
- `status`: `"pending"` | `"completed"` | `"error"` | `"blocked"`.
- 타임스탬프(`completed_at`, `failed_at`, `blocked_at`)는 `scripts/execute.py`가 상태 변경 시 자동 기록한다.

### `phases/{task-name}/index.json`

```json
{
  "project": "<프로젝트명>",
  "phase": "<task-name>",
  "steps": [
    { "step": 0, "name": "project-setup", "status": "pending" },
    { "step": 1, "name": "data-foundation", "status": "pending" },
    { "step": 2, "name": "room-system", "status": "pending" }
  ]
}
```

- `project`: 프로젝트명. 프로젝트 규칙은 `AGENTS.md`를 기준으로 한다.
- `phase`: task 이름. 디렉토리명과 일치시킨다.
- `steps[].step`: 0부터 시작하는 순번.
- `steps[].name`: kebab-case slug.
- `steps[].status`: 초기값은 모두 `"pending"`.

상태 전이:

| 전이 | 기록되는 필드 | 기록 주체 |
|------|-------------|----------|
| `completed` | `completed_at`, `summary` | Codex 세션, `execute.py` |
| `error` | `failed_at`, `error_message` | Codex 세션, `execute.py` |
| `blocked` | `blocked_at`, `blocked_reason` | Codex 세션, `execute.py` |

`summary`는 step 완료 시 산출물을 한 줄로 요약한 값이다. `execute.py`가 다음 step 프롬프트에 컨텍스트로 누적 전달하므로, 생성된 파일과 핵심 결정을 담아야 한다.

커밋 정책:

- 성공한 step만 harness가 커밋한다.
- Codex 세션은 직접 `git commit`을 실행하지 않는다.
- 실패하거나 blocked 된 step의 변경사항은 커밋하지 않는다.
- 실패/blocked 상태의 dirty worktree는 수동 점검 대상이다. 수정하거나 폐기한 뒤 해당 step status를 `pending`으로 되돌려 재실행한다.

### `phases/{task-name}/step{N}.md`

각 step마다 하나의 Markdown 파일을 둔다.

```markdown
# Step {N}: {이름}

## 읽어야 할 파일

먼저 아래 파일들을 읽고 프로젝트의 아키텍처와 설계 의도를 파악하라:

- `/AGENTS.md`
- `/docs/PRD.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/UI_GUIDE.md`
- {이전 step에서 생성/수정된 파일 경로}
- {현재 step에 필요한 design/*.txt 원문 1~2개}

## 작업

{구체적인 구현 지시. 파일 경로, 클래스/함수 시그니처, 로직 설명을 포함한다.}

## Acceptance Criteria

```bash
luac -p main.lua             # Lua 문법 검증 예시. 실제 명령은 환경에 맞게 조정
love .                       # LÖVE smoke run 예시. 로컬 LÖVE 경로 확인 필요
```

## 검증 절차

1. AC 커맨드 또는 해당 step의 검증 스크립트를 실행한다.
2. 아키텍처 체크리스트를 확인한다.
3. 결과에 따라 `phases/{task-name}/index.json`의 해당 step을 업데이트한다.

## 금지사항

- {이 step에서 하지 말아야 할 것. "X를 하지 마라. 이유: Y" 형식}
- 기존 테스트를 깨뜨리지 마라.
```

## 실행

```bash
python3 scripts/execute.py {task-name}
python3 scripts/execute.py {task-name} --push
```

`execute.py`가 자동으로 처리하는 것:

- `feat-{task-name}` 브랜치 생성 또는 checkout.
- 가드레일 주입: `AGENTS.md`와 `docs/*.md` 내용을 매 step 프롬프트에 포함.
- 컨텍스트 절약: step 파일에는 필요한 `design/*.txt` 원문만 명시.
- 컨텍스트 누적: 완료된 step의 `summary`를 다음 step 프롬프트에 전달.
- 자가 교정: 실패 시 최대 3회 재시도하며 이전 에러 메시지를 프롬프트에 피드백.
- 커밋: 성공한 step만 코드 변경(`feat`)과 메타데이터(`chore`)를 분리 커밋.
- 타임스탬프: `started_at`, `completed_at`, `failed_at`, `blocked_at` 자동 기록.
- step 실행: `codex exec --sandbox workspace-write --json <prompt>` 형식으로 비대화형 Codex를 호출.

## 리뷰 체크리스트

변경 사항을 리뷰할 때는 먼저 `/AGENTS.md`, `/docs/ARCHITECTURE.md`, `/docs/ADR.md`를 읽고 아래 항목을 확인한다.

| 항목 | 확인 내용 |
|------|----------|
| 아키텍처 준수 | ARCHITECTURE.md의 디렉토리 구조를 따르는가 |
| 기술 스택 준수 | ADR의 기술 선택을 벗어나지 않았는가 |
| 테스트 존재 | 새로운 기능에 대한 테스트가 작성되어 있는가 |
| CRITICAL 규칙 | AGENTS.md의 CRITICAL 규칙을 위반하지 않았는가 |
| 실행 가능 | Lua 문법 검증, 데이터 로드 검증, LÖVE smoke run 등 해당 step의 검증이 통과하는가 |

## Build 자동화 정책

- 첫 구현 phase에는 환경 확인 step을 포함한다.
- 환경 확인 step은 LÖVE 실행 파일 경로, `luac` 또는 대체 Lua 문법 검증 방법, `build/` 디렉토리 생성, 검증/패키징 스크립트 위치를 확정한다.
- LÖVE runtime은 repo에 포함하지 않고 로컬 경로 설정으로 사용한다.
- LÖVE 경로가 없거나 실행 불가하면 Windows exe 패키징 step은 `blocked`로 멈춘다.
- 최종 `.love` 산출물은 `build/EscapeFromNightmares.love`에 저장한다.
- 최종 Windows exe 패키지는 `build/windows/` 아래에 저장한다.
- `build/`에는 배포 산출물만 둔다. 런타임 소스, 원본 `assets/`, 원본 `data/`의 소유 위치를 `build/`로 옮기지 않는다.

## Blocked 질문 형식

사용자 결정이나 환경 정보가 필요하면 추측하지 말고 즉시 blocked 처리한다. `blocked_reason`에는 아래 형식을 포함한다.

```text
Question: <결정이 필요한 질문>
Options:
- <선택지 A> (Recommended): <영향>
- <선택지 B>: <영향>
- <선택지 C>: <영향>
```

## Design 파일 선택 기준

- 프로젝트와 범위: `design/00_PROJECT_OVERVIEW.txt`
- UI, 입력, 저장 규칙: `design/01_GAME_SYSTEMS_UI_RULES.txt`
- Stage 1 방 구조: `design/02_STAGE1_SPACE_ROOMS.txt`
- 퍼즐, 아이템, 이벤트: `design/03_PUZZLES_ITEMS_EVENTS.txt`
- 몬스터와 은신 AI: `design/04_MONSTER_HIDING_AI.txt`
- 코드 구조와 데이터 파일: `design/05_IMPLEMENTATION_STRUCTURE.txt`
- 리소스와 사운드: `design/06_RESOURCES_LIST.txt`
- 최종 개발 지시: `design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- 미확정 항목: `design/08_REMAINING_TASKS.txt`

## 에러 복구

- `error` 발생 시: 커밋되지 않은 변경을 수동 점검한다. 수정하거나 폐기한 뒤 `phases/{task-name}/index.json`에서 해당 step의 `status`를 `"pending"`으로 바꾸고 `error_message`를 삭제한 뒤 재실행한다.
- `blocked` 발생 시: `blocked_reason`의 객관식 질문에 대한 결정을 반영한다. 커밋되지 않은 변경을 수동 점검한 뒤 `status`를 `"pending"`으로 바꾸고 `blocked_reason`을 삭제한 뒤 재실행한다.
