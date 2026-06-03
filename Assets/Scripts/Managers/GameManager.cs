using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
	// 게임의 큰 흐름을 제어하는 매니저입니다.
	// 현재는 타이틀 화면 버튼에서 호출할 게임 시작과 종료 기능을 담당합니다.
	// Singleton<GameManager>를 상속하므로 씬 전환 후에도 하나의 GameManager 인스턴스를 유지할 수 있습니다.
	public class GameManager : Singleton<GameManager>
	{
		// 타이틀 화면의 "게임 시작" 버튼에서 호출할 메서드입니다.
		// 호출되면 Main 씬을 로드하여 실제 게임 플레이 화면으로 이동합니다.
		public void StartGame()
		{
			Debug.Log("Loading main scene.");
			SceneManager.LoadScene(Common.MainSceneName);
		}

		// 타이틀 화면의 "종료" 버튼에서 호출할 메서드입니다.
		// 빌드된 게임에서는 애플리케이션을 종료하고, Unity 에디터에서는 Play Mode를 중지합니다.
		public void QuitGame()
		{
			Debug.Log("Quit game requested.");

#if UNITY_EDITOR
			// 에디터 실행 중에는 Application.Quit()이 실제로 에디터를 종료하지 않으므로,
			// 테스트 편의를 위해 Play Mode를 꺼서 종료 동작을 확인합니다.
			EditorApplication.isPlaying = false;
#else
			// 실제 빌드 환경에서는 게임 애플리케이션을 종료합니다.
			Application.Quit();
#endif
		}
	}
}
