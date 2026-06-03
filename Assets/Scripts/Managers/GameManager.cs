using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
	// 게임의 큰 흐름을 제어하는 전역 매니저입니다.
	// 타이틀 화면 버튼에서 게임 시작과 종료 요청을 처리합니다.
	public class GameManager : Singleton<GameManager>
	{
		private const string MainSceneName = "Main";

		// 메인 게임 씬을 불러와 플레이 화면으로 이동합니다.
		public void StartGame()
		{
			Debug.Log("Loading main scene.");
			SceneManager.LoadScene(MainSceneName);
		}

		// 빌드에서는 애플리케이션을 종료하고, 에디터에서는 플레이 모드를 중지합니다.
		public void QuitGame()
		{
			Debug.Log("Quit game requested.");

#if UNITY_EDITOR
			// 에디터 실행 중에는 종료 함수 대신 플레이 모드를 끕니다.
			EditorApplication.isPlaying = false;
#else
			// 실제 빌드 환경에서는 게임 애플리케이션을 종료합니다.
			Application.Quit();
#endif
		}
	}
}
