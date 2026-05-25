using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Bootstrap
{
    /// <summary>
    /// 게임 씬에 명시적인 런타임 오브젝트가 없을 때 기본 GameDirector를 자동으로 구성합니다.
    /// </summary>
    public static class EscapeFromNightmaresBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateRuntime()
        {
            if (SceneManager.GetActiveScene().name == "TitleScene" || Object.FindFirstObjectByType<TitleSceneController>() != null)
            {
                return;
            }

            if (Object.FindFirstObjectByType<GameDirector>() != null)
            {
                return;
            }

            var gameObject = new GameObject("Escape From Nightmares Runtime");
            gameObject.AddComponent<GameDirector>();
        }
    }
}
