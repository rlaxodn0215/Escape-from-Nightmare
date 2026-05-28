using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    public class EndingManager : Singleton<EndingManager>
    {
        [SerializeField] private EndingPanelUI endingPanel;
        [SerializeField] private float returnToTitleDelaySeconds = 3f;
        [SerializeField] private bool returnToTitleAfterEnding = true;
        [SerializeField] private string endingTitle = "Ending";
        [SerializeField] private string endingMessage = "You escaped from the nightmare.";

        private Coroutine endingRoutine;

        private void OnEnable()
        {
            SubscribePanel();
        }

        private void OnDisable()
        {
            UnsubscribePanel();
        }

        public void PlayEnding()
        {
            if (endingRoutine != null)
            {
                StopCoroutine(endingRoutine);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Ending);
            }

            endingRoutine = StartCoroutine(EndingRoutine());
        }

        public IEnumerator EndingRoutine()
        {
            if (endingPanel != null)
            {
                endingPanel.Show(endingTitle, endingMessage);
            }
            else
            {
                Debug.LogWarning("EndingPanelUI is not assigned.");
            }

            yield return new WaitForSeconds(Mathf.Max(0f, returnToTitleDelaySeconds));

            if (returnToTitleAfterEnding)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ReturnToTitle();
                }
                else
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }

            endingRoutine = null;
        }

        public void SkipEnding()
        {
            if (endingRoutine != null)
            {
                StopCoroutine(endingRoutine);
                endingRoutine = null;
            }

            if (endingPanel != null)
            {
                endingPanel.Hide();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToTitle();
            }
            else
            {
                SceneManager.LoadScene("TitleScene");
            }
        }

        private void SubscribePanel()
        {
            if (endingPanel != null)
            {
                endingPanel.SkipRequested -= SkipEnding;
                endingPanel.SkipRequested += SkipEnding;
            }
        }

        private void UnsubscribePanel()
        {
            if (endingPanel != null)
            {
                endingPanel.SkipRequested -= SkipEnding;
            }
        }
    }
}
