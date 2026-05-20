using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmares.Core
{
    public sealed class SceneFlowController : MonoBehaviour
    {
        [SerializeField] private string stage1SceneName = "Stage1";

        public string Stage1SceneName => stage1SceneName;

        public void LoadStage1()
        {
            SceneManager.LoadScene(stage1SceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
