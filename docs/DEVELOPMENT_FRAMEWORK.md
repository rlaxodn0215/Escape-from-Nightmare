# 개발 프레임워크

## 환경
- Unity: `6000.3.9f1`
- 렌더 파이프라인: Universal Render Pipeline 2D
- 언어: C#
- UI: uGUI
- 입력: Unity Input System, 마우스 클릭 중심
- 리소스: `ResourcePathCatalog` + `Resources.Load<T>(path)`
- 오디오: AudioMixer `Master/BGM/SFX/UI`
- 테스트: Unity Test Framework
- 대상 플랫폼: Windows PC

## 구현 단계
1. 런타임 골격: `GameDirector`, `GameSession`, 서비스 계층, 더미 UI
2. 데이터 골격: ScriptableObject 타입과 Stage 1 런타임 데이터
3. 게임 루프: 방 이동, 아이템 획득, 퍼즐 해결, 플래그 처리
4. 공포 시스템: 몬스터 FSM, 위험도, 은신, 게임오버
5. 리소스 교체: 더미 UI/배경을 Sprite, Prefab, AudioClip으로 교체
6. 검증: EditMode, PlayMode, Windows 빌드

## 코딩 규칙
- MonoBehaviour는 씬/Unity 생명주기 조율에만 사용한다.
- 규칙 계산은 가능한 한 순수 C# 서비스로 분리한다.
- public 필드보다 `[SerializeField] private`를 우선한다.
- 클래스명과 파일명은 일치시킨다.
- 진행 저장 로직을 추가하지 않는다.

## 데이터 규칙
- 새 방은 `RoomDefinition`으로 추가한다.
- 새 클릭 대상은 `InteractableDefinition`으로 추가한다.
- 새 아이템은 `ItemDefinition`, 새 퍼즐은 `PuzzleDefinition`으로 추가한다.
- 실제 에셋이 준비되면 `RuntimeStageFactory` 데이터를 `Assets/EscapeFromNightmares/ScriptableObjects`의 에셋으로 이전한다.
- 이미지/오디오는 컴포넌트에 직접 할당하지 않고 `ResourcePathCatalog`의 문자열 경로로 로드한다.
- 타이틀 씬/프리팹/더미 리소스는 `Escape From Nightmares/Rebuild Title Scene Assets` 메뉴에서 재생성할 수 있다.

## 검증 체크리스트
- Unity 콘솔 컴파일 에러 없음
- `EscapeFromNightmares.EditModeTests` 어셈블리의 EditMode 테스트 통과
- Start 버튼으로 게임 시작 가능
- `child_room`에서 시작
- 필수 퍼즐 해결 가능
- 몬스터 상태 변화 확인
- 은신 진입/해제 가능
- Game Over 후 재시작
- `stage1_clear` 기록 저장
