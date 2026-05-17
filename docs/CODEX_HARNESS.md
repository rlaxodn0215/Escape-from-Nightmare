# Codex Harness

이 프로젝트는 Codex 기반 Harness 워크플로우를 사용한다. Harness는 큰 작업을 phase와 step으로 나누고, 각 step을 독립 실행 가능한 지시서로 만들어 순차 실행한다.

`docs/*.md`는 step 실행 시 빠르게 주입할 압축 가드레일이다. `design/*.txt`는 상세 원문이며, 모든 step에서 전부 읽지 말고 현재 작업에 필요한 파일만 골라 읽는다.

## Resource / Prefab / Scene Gate

Playable build 단계는 빈 Unity asset tree나 script-only 구현을 대상으로 진행하면 안 된다. 모든 구현 phase는 Windows Build 전에 placeholder/final 리소스, prefab, scene hierarchy, serialized reference를 확인해야 한다.

첫 Harness step은 반드시 `resource-inventory`다. `resource-inventory`가 `ready_for_review`가 되고 사용자가 `approved`로 승인하기 전에는 C# 시스템, scene, prefab 개발 step을 시작하지 않는다. `project-setup`은 `resource-inventory` 다음 step이거나, 같은 phase 안에서 `resource-inventory` 승인 후 실행한다.

Resource Gate는 다음을 수행한다.

1. `design/06_RESOURCES_LIST.txt`의 모든 필수 이미지/오디오 파일명을 파싱한다.
2. 각 파일명을 Unity target path로 매핑한다.
3. `EscapeFromNightmares/Assets/Sprites/**`, `Assets/Audio/**`, `Assets/UI/**`에서 final 또는 placeholder asset 존재 여부를 확인한다.
4. `resource_manifest.json` 또는 동등한 검증 산출물을 만든다.
5. 0바이트 파일, 잘못된 확장자, 누락 파일, 이름 불일치, Unity import 불가능 파일을 `blocked`로 처리한다.
6. ScriptableObject, prefab, scene, serialized data에서 참조하는 sprite/audio 경로와 asset reference를 확인한다.
7. placeholder를 final art/audio라고 주장하지 않는다.

`resource_manifest.json` 항목은 최소한 아래 필드를 가진다.

```json
{
  "required_filename": "room_child_room.png",
  "unity_target_path": "Assets/Sprites/Rooms/room_child_room.png",
  "category": "room_background",
  "status": "missing",
  "referenced_by": "pending"
}
```

`status`는 `"missing"`, `"placeholder"`, `"final"` 중 하나다. `referenced_by`는 `ScriptableObject`, `prefab`, `scene`, 또는 `"pending"`을 기록한다.

리소스 일관성 검사:

- `ItemDefinition`, `PuzzleDefinition`, 이벤트 설계와 리소스 명세의 ID/파일명이 맞는지 확인한다.
- 예: 설계에서 `fuse_holder`를 요구하지만 리소스 명세가 `item_electric_part.png`만 제공하면 사용자 검토 대상으로 올리고 해당 step을 `blocked` 처리한다.
- 이름 불일치는 임의로 고치지 않고 `blocked_reason`에 후보와 추천안을 적는다.

Resource validation 이름:

- `ResourceManifestValidation`
- `RequiredAssetPresenceValidation`
- `ZeroByteAssetValidation`
- `UnityImportableAssetValidation`
- `DesignResourceConsistencyValidation`

현재 repository 상태 기준 첫 Harness 판정:

- Unity `Assets` 안에 필수 이미지, 오디오, prefab이 없으면 `resource-inventory`는 `blocked`다.
- Build Settings가 `SampleScene`만 가리키면 `BuildSettingsValidation`은 `blocked`다.
- `phases/`와 step 실행 파일이 없으면 Harness execution setup은 `blocked`다.

Prefab Gate는 다음을 수행한다.

1. UI prefab(title, pause, settings, inventory, map, puzzle, hiding gauge, game over)이 존재하는지 확인한다.
2. gameplay prefab(room view, interactable hotspot, screen edge hotspot, hide spot, monster overlay, audio emitter)이 존재하는지 확인한다.
3. 각 prefab이 scene에 배치되었거나 ScriptableObject/manager에서 참조되는지 확인한다.
4. missing script, missing component, missing prefab reference가 있으면 해당 step을 `blocked` 또는 `error`로 처리한다.

Scene Gate는 다음을 수행한다.

1. playable scene에 `StageRoot`, `Systems`, `RoomView`, `InteractableLayer`, `MonsterLayer`, `AudioRoot`, `UICanvas`, `EventSystem`, `DebugRoot` root object가 있는지 확인한다.
2. Build Settings에 `Boot` 또는 `Stage1` scene이 등록되어 있는지 확인한다.
3. `SampleScene`만 enabled 상태면 playable build 전 `blocked` 처리한다.
4. scene object가 script와 data asset을 실제로 연결하고 있는지 확인한다.

## Unity MCP Policy

Unity MCP 패키지(`com.coplaydev.unity-mcp`)가 설치되어 있으므로, callable Unity MCP tool이 있는 세션에서는 Unity-native 검증을 우선 사용한다.

Unity MCP 사용 대상:

- scene hierarchy 점검
- prefab 존재와 prefab instance 점검
- missing component 점검
- serialized reference 누락 점검
- Build Settings scene 점검

현재 세션에서 Unity MCP callable tool이 없으면 Unity BatchMode, YAML/static validation, ProjectSettings/EditorBuildSettings.asset 검사로 fallback한다. 이 경우 step summary에 "Unity MCP unavailable; used fallback validation"을 남긴다.

## Publish Policy

- Generated Unity player builds are not source history.
- Publish source code, scenes, prefabs, ScriptableObject assets, placeholder assets, docs, scripts, and phase metadata.
- Do not commit `Library/`, `Logs/`, `Temp/`, `Obj/`, `UserSettings/`, or local build output folders.
- If checkout, pull, merge, or push fails, stop and leave the issue for manual review.

## Workflow

### A. 탐색

`/docs/` 하위 문서(PRD, ARCHITECTURE, ADR, UI_GUIDE 등)를 읽고 프로젝트의 기획, 아키텍처, 설계 의도를 파악한다. 세부 구현이 필요한 경우 `AGENTS.md`의 Context Map을 따라 관련 `design/*.txt` 원문만 추가로 읽는다.

### B. 논의

구현 전에 구체화하거나 기술적으로 결정해야 할 사항이 있으면 사용자에게 제시하고 확정한다.

### C. Step 설계

구현 계획은 여러 step으로 나누어 작성한다. 한 phase 전체를 한 번에 구현하지 않는다. 각 step은 사용자 검토를 통과해야 다음 step으로 넘어간다.

## Collaborative Unit Workflow

Harness는 게임을 한 번에 만들지 않는다. 각 step은 원칙적으로 하나의 game unit만 다룬다.

game unit 예시:

- `TitleUI.prefab`
- `InventoryUI.prefab`
- `Stage1.unity`
- `child_room`
- `study_safe`
- `MonsterOverlay.prefab`
- `RoomView.prefab`

하나의 step에서 여러 prefab, 여러 scene, 여러 room, 여러 puzzle을 동시에 만들지 않는다. 예외가 필요하면 `Pre-Implementation Proposal`에서 이유를 설명하고 사용자 승인을 받아야 한다.

구현 전 상담 규칙:

- 모든 game unit step은 먼저 `design_review` 상태가 된다.
- `design_review`에서는 파일 생성, prefab 생성, scene 편집, C# 구현을 하지 않는다.
- `design_review`에서는 의도, 구성, Unity hierarchy/prefab 구조, 리소스, 완료 기준, 검토 방법을 사용자와 확정한다.
- 사용자가 승인하면 `approved_for_implementation`으로 전환한다.
- `approved_for_implementation` 전에는 어떤 구현 변경도 시작하지 않는다.

구성요소별 분할 기준:

- prefab은 하나씩 설계, 승인, 제작한다.
- scene은 hierarchy skeleton, UI 배치, gameplay layer 연결을 분리해 단계별로 진행한다.
- level/room은 방 하나 또는 방 연결 하나 단위로 진행한다.
- puzzle은 퍼즐 하나 단위로 진행한다.
- resource는 파일 묶음 생성 전에 manifest와 placeholder/final 여부를 먼저 승인받는다.

설계 원칙:

1. **Scope 최소화**: 하나의 step에서는 하나의 game unit만 다룬다. 레이어나 모듈 전체가 너무 크면 prefab, scene, room, puzzle, UI surface 단위로 쪼갠다.
2. **자기완결성**: 각 step 파일은 독립된 Codex 세션에서 실행될 수 있어야 한다.
3. **사전 준비 강제**: 관련 문서 경로와 이전 step에서 생성 또는 수정된 파일 경로를 명시한다.
4. **시그니처 수준 지시**: 클래스, ScriptableObject, MonoBehaviour, public method/interface는 제시하되 내부 구현은 step 목적과 규칙 안에서 결정하게 한다.
5. **실행 가능한 AC**: 추상적인 완료 조건 대신 Unity BatchMode EditMode/PlayMode 테스트, C# 컴파일, scene validation, Windows Build 검증 같은 실제 검증 커맨드를 포함한다.
6. **구체적인 금지사항**: "조심해라" 대신 "X를 하지 마라. 이유: Y" 형식으로 적는다.
7. **네이밍**: step name은 kebab-case slug로 작성한다. 예: `project-setup`, `data-foundation`, `room-system`.
8. **모르는 부분 처리**: 상상하거나 임의 결정하지 않는다. 사용자 결정이 필요하면 `blocked_reason`에 질문과 2~3개의 객관식 선택지를 적고, 추천 선택지는 `(Recommended)`로 표시한다.
9. **사용자 검토 게이트**: step 구현과 검증이 끝나면 `ready_for_review`로 멈춘다. 사용자가 승인하기 전에는 다음 step을 실행하지 않는다.
10. **Unity 산출물 동등성**: C# script, ScriptableObject, prefab, scene hierarchy는 모두 동등한 구현 산출물이다. script만 작성된 step은 scene/prefab 작업이 필요한 기능의 완료로 보지 않는다.
11. **구현 전 승인 게이트**: `approved_for_implementation` 전에는 구현을 시작하지 않는다. 작은 구성요소 하나라도 먼저 사용자와 설계안을 확정한다.

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

### `phases/{task-name}/index.json`

```json
{
  "project": "Escape From Nightmares",
  "phase": "<task-name>",
  "steps": [
    {
      "step": 0,
      "name": "resource-inventory",
      "status": "pending",
      "unit_type": "resource",
      "unit_id": "stage1_required_assets",
      "requires_user_design_approval": true
    },
    {
      "step": 1,
      "name": "title-ui-prefab",
      "status": "pending",
      "unit_type": "prefab",
      "unit_id": "TitleUI.prefab",
      "requires_user_design_approval": true
    },
    {
      "step": 2,
      "name": "stage1-scene-skeleton",
      "status": "pending",
      "unit_type": "scene",
      "unit_id": "Stage1.unity",
      "requires_user_design_approval": true
    }
  ]
}
```

`status`는 `"pending"`, `"design_review"`, `"approved_for_implementation"`, `"in_progress"`, `"ready_for_review"`, `"approved"`, `"completed"`, `"error"`, `"blocked"` 중 하나다.

상태 전이:

| 전이 | 의미 |
|------|------|
| `pending -> design_review` | 구현 전 사용자 상담 시작 |
| `design_review -> approved_for_implementation` | 사용자가 구성요소 제작을 승인 |
| `approved_for_implementation -> in_progress` | step 구현 시작 |
| `in_progress -> ready_for_review` | 구현과 검증 완료, 사용자 검토 대기 |
| `ready_for_review -> approved` | 사용자가 다음 step 진행을 승인 |
| `approved -> completed` | 승인된 step을 완료 처리 |
| `design_review -> blocked` | 구현 전 결정이 필요 |
| `in_progress -> blocked` | 사용자 결정 또는 환경 문제가 필요 |
| `in_progress -> error` | 구현 또는 검증 실패 |

`approved_for_implementation` 전에는 파일 생성, prefab 생성, scene 편집, C# 구현을 시작하지 않는다.
`ready_for_review` 상태에서는 다음 step을 실행하지 않는다.

### `phases/{task-name}/step{N}.md`

각 step마다 하나의 Markdown 파일을 둔다.

```markdown
# Step {N}: {이름}

## Game Unit

- unit_type: resource | prefab | scene | room | puzzle | ui | monster_event | system
- unit_id: {예: TitleUI.prefab, child_room, study_safe}
- requires_user_design_approval: true

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

{구체적인 구현 지시. 파일 경로, 클래스/메서드/ScriptableObject 구조, 씬 또는 프리팹 작업을 포함한다.}

## Pre-Implementation Proposal

{구현 전에 사용자에게 보여줄 구성안. 목적, 화면/씬/프리팹 구성, 리소스, 인터랙션, 완료 기준, 검토 방법을 포함한다. 이 섹션이 승인되기 전에는 구현하지 않는다.}

## User Decisions

{사용자가 확정한 선택을 기록한다. 결정되지 않은 항목은 blocked 처리한다.}

## Out of Scope

{이번 game unit에서 절대 만들지 않을 prefab, scene, room, puzzle, system을 명시한다.}

## Scene/Prefab Changes

{생성/수정할 scene root, prefab, prefab instance, serialized reference를 명시한다. 해당 step이 script-only라면 scene/prefab 변경이 필요 없는 이유를 적는다.}

## Resource Inventory

{이 step에서 요구되는 필수 리소스, `resource_manifest.json` 변경, missing/placeholder/final 상태, zero-byte 검사 결과, Unity import 가능성, 설계-리소스 이름 불일치를 적는다. `resource-inventory` step이 아니더라도 새 리소스 참조가 생기면 갱신한다.}

## Unity MCP Checks

{Unity MCP로 수행할 scene hierarchy, prefab, missing component, serialized reference 검사 목록을 적는다. MCP가 없을 때의 BatchMode/YAML fallback도 적는다.}

## Acceptance Criteria

```powershell
Unity.exe -batchmode -quit -projectPath EscapeFromNightmares -runTests -testPlatform EditMode
Unity.exe -batchmode -quit -projectPath EscapeFromNightmares -runTests -testPlatform PlayMode
Unity.exe -batchmode -quit -projectPath EscapeFromNightmares -executeMethod BuildScript.BuildWindows
```

필수 validation:

- `ResourceManifestValidation`
- `RequiredAssetPresenceValidation`
- `ZeroByteAssetValidation`
- `UnityImportableAssetValidation`
- `DesignResourceConsistencyValidation`
- `SceneHierarchyValidation`
- `PrefabReferenceValidation`
- `ScriptableObjectReferenceValidation`
- `BuildSettingsValidation`

## User Review Checklist

{사용자가 Unity Editor 또는 산출물 diff에서 확인할 항목을 적는다. UI/prefab/scene 변화가 있으면 시각적으로 확인할 경로를 포함한다.}

## Review Artifact

{사용자가 검토할 prefab, scene, resource manifest, screenshot, validation output, 문서 경로를 적는다.}

## Next Step Blocker

이 step이 `ready_for_review`가 되면 사용자가 승인하기 전에는 다음 step을 실행하지 않는다.
`design_review` 상태에서도 사용자가 `approved_for_implementation`으로 승인하기 전에는 구현을 시작하지 않는다.

## 검증 절차

1. AC 커맨드 또는 해당 step의 검증 스크립트를 실행한다.
2. 아키텍처 체크리스트를 확인한다.
3. 결과에 따라 `phases/{task-name}/index.json`의 해당 step을 업데이트한다.

## 금지사항

- Unity generated folders를 직접 편집하거나 커밋하지 마라. 이유: 환경별 산출물이다.
- 기존 테스트를 깨뜨리지 마라.
```

## 리뷰 체크리스트

변경 사항을 리뷰할 때는 먼저 `/AGENTS.md`, `/docs/ARCHITECTURE.md`, `/docs/ADR.md`를 읽고 아래 항목을 확인한다.

| 항목 | 확인 내용 |
|------|----------|
| 아키텍처 준수 | ARCHITECTURE.md의 Unity 디렉토리 구조를 따르는가 |
| 기술 스택 준수 | ADR의 Unity 6, URP 2D, C#, Input System 선택을 벗어나지 않았는가 |
| 테스트 존재 | 새로운 기능에 대한 EditMode 또는 PlayMode 테스트가 작성되었는가 |
| CRITICAL 규칙 | AGENTS.md의 Hard Rules를 위반하지 않았는가 |
| 실행 가능 | Unity BatchMode 테스트, scene validation, Windows Build 등 해당 step의 검증이 통과하는가 |
| 리소스 | 필수 리소스 manifest가 있고 missing/placeholder/final 상태가 검토되었는가 |
| 리소스 무결성 | 0바이트, 잘못된 확장자, import 불가, 설계-파일명 불일치가 없는가 |
| Scene/Prefab | 필수 scene hierarchy와 prefab reference가 실제로 연결되었는가 |
| 구성요소 단위 | step이 하나의 game unit만 다루는가 |
| 구현 전 승인 | `design_review`에서 사용자 승인 후 `approved_for_implementation`으로 넘어갔는가 |
| 사용자 검토 | step이 `ready_for_review`에서 멈추고 사용자 승인 후 다음 step으로 진행하는가 |

## Build 자동화 정책

- 첫 구현 phase에는 `resource-inventory` step을 가장 먼저 포함한다.
- `resource-inventory` 승인 전에는 환경 확인을 제외한 개발 step을 시작하지 않는다.
- 환경 확인 step은 Unity editor path, Unity version, project path, build output path, 검증/빌드 스크립트 위치를 확정한다.
- Unity editor executable은 repo에 포함하지 않고 로컬 설치 경로로 사용한다.
- Unity가 없거나 실행 불가하면 Windows Build step은 `blocked`로 멈춘다.
- 최종 Windows player build는 local ignored build output folder에 저장한다.
- Generated player build에는 배포 산출물만 둔다. 런타임 소스, 원본 assets, ScriptableObject asset의 소유 위치를 build output으로 옮기지 않는다.
- Build Settings에 `SampleScene`만 있으면 playable build를 진행하지 않고 `blocked` 처리한다.

## Blocked 질문 형식

사용자 결정이나 환경 정보가 필요하면 추측하지 말고 즉시 blocked 처리한다.

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
- Unity 코드 구조와 데이터 asset: `design/05_IMPLEMENTATION_STRUCTURE.txt`
- 리소스와 사운드: `design/06_RESOURCES_LIST.txt`
- 최종 개발 지시: `design/07_CODEX_DEVELOPMENT_INSTRUCTION.txt`
- 미확정 항목: `design/08_REMAINING_TASKS.txt`
