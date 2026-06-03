// EscapeFromNightmare 프로젝트의 공통 네임스페이스를 선언하는 파일입니다.
// 현재는 공통 상수, 열거형, 유틸리티 타입이 없어서 비어 있지만,
// 여러 시스템에서 함께 사용하는 작은 타입을 추가할 때 이 네임스페이스 아래에 배치할 수 있습니다.

namespace EscapeFromNightmare
{
	// 공통 코드가 필요해질 때 이 영역에 프로젝트 전역 타입을 추가합니다.
	// 예: 씬 이름 상수, 공용 이벤트 타입, 게임 전역 설정 키 등.

	public static class Common
	{
		// 게임 시작 시 이동할 메인 게임 씬 이름입니다.
		// ProjectSettings의 Build Settings에 등록된 Assets/Scenes/Main.unity의 씬 이름과 일치해야 합니다.
		public const string MainSceneName = "Main";
	}
}
