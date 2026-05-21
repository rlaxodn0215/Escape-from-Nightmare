# 아키텍처

## Unity 구조
```text
Assets/EscapeFromNightmares/
├─ Scripts/
│  ├─ Bootstrap/       # 빈 씬 자동 실행
│  ├─ Data/            # ScriptableObject 정의
│  ├─ Editor/          # 씬/프리팹/더미 리소스 생성기
│  ├─ Runtime/         # GameDirector, 런타임 데이터 팩토리
│  ├─ Services/        # 세션, 인벤토리, 퍼즐, 몬스터, 저장
│  └─ UI/              # 추후 UI 전용 컴포넌트
├─ ScriptableObjects/  # 실제 데이터 에셋 배치 예정
├─ Prefabs/            # 방, 클릭 영역, UI 프리팹 예정
├─ Sprites/            # Rooms, Objects, Items, UI, Monster, Effects
├─ Audio/              # BGM, Ambience, SFX, Monster, UI
└─ Tests/EditMode/
```

## 런타임 패턴
- `GameDirector`가 현재 MVP의 진입점이며 게임 루프와 uGUI 더미 화면을 조율한다.
- `GameSession`은 현재 방, 인벤토리, 플래그만 들고 있으며 저장하지 않는다.
- `InventoryService`, `PuzzleService`, `DangerSystem`, `HidingSystem`, `MonsterAIController`는 기능별 순수 로직을 담당한다.
- `ResourcePathCatalog`는 직접 에셋 참조 대신 `Resources` 경로 문자열을 관리한다.
- `ResourceManager`와 `SoundManager`는 경로 기반 로딩과 AudioMixer 볼륨 적용을 담당한다.
- `RuntimeStageFactory`는 실제 ScriptableObject 에셋이 준비되기 전까지 Stage 1 데이터를 런타임 생성한다.

## 데이터 흐름
```text
마우스 클릭
→ GameDirector
→ InteractableDefinition 해석
→ GameSession / Service 갱신
→ UI 갱신
→ 필요 시 settings.json 또는 clear_records.json 저장
```

## 상태 관리
- 진행 상태는 메모리의 `GameSession`에만 둔다.
- 설정과 클리어 기록만 `Application.persistentDataPath`에 JSON으로 저장한다.
- 몬스터 상태는 `MonsterAIController`, 수치 압박은 `DangerSystem`, 은신 소음은 `HidingSystem`이 관리한다.
