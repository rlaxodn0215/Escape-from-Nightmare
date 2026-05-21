# Architecture Decision Records

## 철학
스테이지 1을 먼저 끝까지 플레이 가능하게 만든다. 실제 리소스가 없어도 구조를 유지하며 실행 가능해야 하고, 기획에 없는 시스템 확장은 피한다.

---

### ADR-001: Unity 6 + URP 2D 선택
**결정**: 프로젝트 표준 엔진은 Unity `6000.3.9f1`과 URP 2D로 한다.  
**이유**: 현재 저장소가 Unity 프로젝트이며 2D 조명/렌더링과 Windows 빌드 흐름을 바로 사용할 수 있다.  
**트레이드오프**: LÖVE 2D/Lua 기반 문서 구조는 폐기한다.

### ADR-002: ScriptableObject 중심 데이터
**결정**: Stage, Room, Item, Puzzle, MonsterNodeGraph, SoundCatalog는 ScriptableObject로 정의한다.  
**이유**: Unity 에디터에서 데이터 검수와 리소스 연결을 쉽게 하기 위해서다.  
**트레이드오프**: 초기 MVP에서는 실제 에셋 대신 RuntimeStageFactory로 임시 데이터를 생성한다.

### ADR-003: 진행 저장 금지
**결정**: 진행 저장, 체크포인트, 자동 저장을 구현하지 않는다.  
**이유**: 죽으면 처음부터 다시 시작하는 압박이 핵심 규칙이다.  
**트레이드오프**: 플레이어 편의성은 낮아지지만 기획 의도와 난이도 감각을 보존한다.

### ADR-004: uGUI 기반 MVP UI
**결정**: MVP UI는 uGUI로 구현한다.  
**이유**: 현재 패키지에 `com.unity.ugui`가 있고 빠르게 타이틀, 인벤토리, 지도, 설정 UI를 만들 수 있다.  
**트레이드오프**: 최종 손그림풍 UI는 별도 Sprite와 Prefab으로 교체해야 한다.
