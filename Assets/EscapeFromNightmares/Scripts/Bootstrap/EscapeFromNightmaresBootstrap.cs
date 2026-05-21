using EscapeFromNightmares.Runtime;
using EscapeFromNightmares.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Bootstrap
{
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
