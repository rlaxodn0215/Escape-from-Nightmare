# 개발 프레임워크

## 환경
- Unity: `6000.3.9f1`
- 렌더 파이프라인: URP 2D
- 언어: C#
- UI: uGUI
- 입력: Unity Input System, 마우스 클릭 중심
- 리소스: `ResourcePathCatalog` + `Resources.Load<T>(path)`
- 오디오: AudioMixer asset 유지, 실제 볼륨은 `SoundManager` fallback 보장
- 테스트: Unity Test Framework
- 빌드 대상: Windows PC

## 개발 산출물 판정 매트릭스
기능 계획을 세우기 전에 산출물을 먼저 분류한다. 플레이어가 보거나 클릭하는 런타임 요소는 스크립트만으로 완료하지 않는다.

| 분류 | 적용 대상 | 필수 산출물 |
| --- | --- | --- |
| `Script only` | 저장, 계산, 순수 서비스, 데이터 검증, 테스트 유틸 | C# 스크립트와 EditMode 테스트 |
| `Script + ScriptableObject asset` | 스테이지, 방, 아이템, 퍼즐, 사운드, 리소스 경로 | 데이터 타입, `.asset`, 검증 테스트 |
| `Script + Prefab` | 버튼, 패널, 인벤토리, 지도, 상호작용 오브젝트, 숨는 장소, 몬스터 노드 | MonoBehaviour, Prefab, Inspector 연결 또는 런타임 바인딩 |
| `Script + Scene` | 타이틀, 스테이지 시작 화면, 독립 진입점 | Scene, Camera, Canvas, EventSystem, AudioListener |
| `ResourceCatalog update required` | 이미지/오디오를 경로 기반으로 로딩하는 기능 | `ResourcePathCatalog` 또는 관련 경로 카탈로그 갱신 |
| `Editor builder required` | 씬/프리팹/더미 리소스를 반복 생성하거나 복구해야 하는 기능 | Editor 메뉴와 재생성 로직 |

## 기능 계획서 필수 항목
새 기능 계획에는 아래 항목을 반드시 포함한다.

- 모르는 요구사항, 시각 디테일, 퍼즐 규칙, 방 구조, 스토리 설정, 하네스 동작은 상상해서 확정하지 않는다. 불확실한 부분은 계획이나 구현에 넣기 전에 질문한다.
- 임시 샘플이나 placeholder는 허용하지만, 최종 설정/최종 아트/최종 규칙처럼 보이지 않게 명확히 표시한다.
- 목표와 성공 기준
- 산출물 판정: `Script`, `Prefab`, `Scene`, `ScriptableObject`, `ResourceCatalog`, `Editor Builder`
- 생성 또는 수정할 주요 위치
- 런타임 연결 방식: Inspector 참조, 런타임 바인딩, 경로 로딩 중 무엇인지
- 실패 fallback: 리소스 없음, 참조 없음, 씬 미등록 시 동작
- 검증 방법: EditMode, PlayMode, 수동 확인

## Prefab/Scene 체크리스트
- 플레이어가 화면에서 보거나 클릭하면 Prefab 또는 Scene 배치가 필요하다.
- UI 버튼, 패널, 슬라이더, 인벤토리 슬롯, 지도 마커는 `Script + Prefab`으로 분류한다.
- 카메라, Canvas, EventSystem, AudioListener가 필요한 독립 화면은 `Script + Scene`으로 분류한다.
- 반복 생성될 수 있는 오브젝트는 Prefab으로 만든다. 런타임 코드 생성만으로 완료하지 않는다.
- 버튼 이벤트는 프리팹 저장에만 의존하지 말고 런타임 초기화에서 다시 바인딩한다.
- 씬/프리팹이 깨졌을 때 메뉴로 재생성할 필요가 있으면 Editor builder를 포함한다.

## Scene 배치와 Editor Builder 선택 기준
- 특정 스테이지에만 존재하는 방 배경, 문, 숨는 장소, 퍼즐 오브젝트는 해당 Scene에 직접 배치해도 된다.
- 여러 씬에서 재사용되거나 Inspector 연결이 많은 UI/상호작용 요소는 Prefab으로 만든 뒤 Scene에 배치한다.
- 타이틀, 설정, 공통 HUD처럼 진입 화면 또는 공통 UI는 Prefab과 Scene을 모두 만들고 필요하면 Editor builder를 둔다.
- Camera, Canvas, EventSystem, AudioListener 같은 기본 씬 인프라가 필요한 기능은 씬 생성 또는 씬 수정 계획을 포함한다.
- 다른 개발자가 메뉴 한 번으로 복구해야 하는 TitleScene, 기본 UI, 더미 리소스 세트는 Editor builder 대상으로 본다.
- 런타임 코드 생성 UI는 임시 검증용으로만 허용한다. 플레이어가 보는 최종 기능은 Prefab 또는 Scene에서 확인 가능해야 한다.

## ScriptableObject/ResourceCatalog 체크리스트
- 기획자가 바꾸거나 리소스 교체 가능성이 있는 값은 하드코딩하지 않는다.
- 방, 아이템, 퍼즐, 스테이지, 몬스터 그래프, 사운드 목록은 ScriptableObject 후보로 본다.
- 이미지와 오디오는 컴포넌트에 직접 파일을 할당하지 않고 경로 카탈로그를 통해 로딩한다.
- 리소스가 없으면 Sprite fallback 또는 무음 처리를 허용하되, 교체 경로와 명명 규칙을 문서화한다.
- 새 리소스 경로가 추가되면 카탈로그 asset과 관련 테스트 또는 수동 검증 항목을 함께 갱신한다.

## 완료 기준 예시
- 설정 메뉴 추가: `Script + Prefab + ResourceCatalog optional`, PlayMode 또는 수동 UI 클릭 검증.
- 퍼즐 정답 판정 추가: `Script + ScriptableObject asset`, EditMode 테스트로 정답/오답/보상 검증.
- 타이틀 씬 추가: `Script + Scene + Prefab + ResourceCatalog + Editor Builder`, Play 실행과 버튼 동작 검증.
- 저장 옵션 추가: `Script only`, JSON 저장/로드 EditMode 테스트.
- 새 방 추가: `ScriptableObject asset + Prefab optional + ResourceCatalog update`, 방 이동과 배경 로딩 검증.
- 스테이지 전용 숨는 장소 추가: `Script + Prefab optional + Scene placement`, 해당 Scene에서 클릭/은신 동작 검증.
- 공통 HUD 추가: `Script + Prefab + Scene placement`, 모든 진입 씬에서 Canvas/EventSystem 충돌 없이 표시되는지 검증.

## 검증 체크리스트
- Unity Console에 컴파일 에러와 반복 경고가 없다.
- 산출물 판정과 실제 변경 파일이 일치한다.
- 플레이어가 보는 기능은 Prefab 또는 Scene에서 직접 확인할 수 있다.
- Scene 배치가 필요한 기능은 대상 Scene Hierarchy에서 오브젝트를 확인할 수 있다.
- Editor builder 대상 기능은 메뉴 실행 후 Scene/Prefab/더미 리소스가 다시 만들어진다.
- `ResourcePathCatalog` 경로가 실제 `Resources` 위치와 일치한다.
- `settings.json`과 `clear_records.json` 외의 진행 저장 파일을 만들지 않는다.
- `.meta` 파일은 에셋과 함께 유지하고, `Library`, `Logs`, `UserSettings`, `*.csproj`, `*.slnx`는 직접 수정하지 않는다.
