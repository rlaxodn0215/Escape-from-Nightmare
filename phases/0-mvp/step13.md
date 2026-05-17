# Step 13: windows-package

## 읽어야 할 파일

- `/AGENTS.md`
- `/docs/ARCHITECTURE.md`
- `/docs/ADR.md`
- `/docs/CODEX_HARNESS.md`
- `/design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- `/design/08_REMAINING_TASKS.txt`
- `/phases/0-mvp/index.json`
- `/build/EscapeFromNightmares.love`

## 작업

Windows 실행용 exe 패키지를 `build/windows/` 아래에 만든다.

1. 로컬 LÖVE runtime 경로를 step 0 summary 또는 환경 설정에서 확인한다.
2. `build/EscapeFromNightmares.love`를 입력으로 사용한다.
3. Windows 실행용 exe와 필요한 runtime dll/파일을 `build/windows/`에 모은다.
4. 패키지 이름은 `EscapeFromNightmares`를 기준으로 한다.
5. LÖVE runtime 경로가 없거나 실행 불가하면 blocked 처리하고 객관식 선택지를 제시한다.

## Acceptance Criteria

```bash
dir build\windows
```

환경에서 가능한 경우:

```bash
build\windows\EscapeFromNightmares.exe
```

## 검증 절차

1. `build/windows/` 아래 Windows 실행용 파일들이 생성됐는지 확인한다.
2. exe 실행 시 LÖVE 게임이 시작되는지 확인한다.
3. `build/` 밖의 source/data/assets 소유 위치가 바뀌지 않았는지 확인한다.
4. 성공 시 step 13을 `completed`로 바꾸고 Windows 패키지 산출물을 `summary`에 적는다.

## 금지사항

- LÖVE runtime 경로를 상상하지 마라. 이유: 로컬 환경에 따라 다르다.
- repo에 runtime을 무단 추가하지 마라. 이유: 런타임 포함 여부는 사용자 결정이 필요하다.
- Windows 패키징 실패를 completed로 표시하지 마라. 이유: 최종 산출물 검증이 깨진다.
