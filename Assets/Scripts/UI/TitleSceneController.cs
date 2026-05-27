using EscapeFromNightmares.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EscapeFromNightmares.UI
{
    /// <summary>
    /// Controls the title screen buttons and title audio using direct scene/prefab references.
    /// </summary>
    public sealed class TitleSceneController : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image titleLogoImage;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private AudioClip titleBgm;
        [SerializeField] private AudioClip uiClickClip;
        [SerializeField] private AudioClip confirmClip;
        [SerializeField] private string gameSceneName = "Main";

        private SoundManager soundManager;
        private SettingsSaveService settingsSaveService;
        private SettingsSaveService.SettingsData settings;

        private void Awake()
        {
            settingsSaveService = new SettingsSaveService(Application.persistentDataPath);
            settings = settingsSaveService.LoadSettings();
            soundManager = GetComponentInChildren<SoundManager>();
            if (soundManager == null)
            {
                soundManager = new GameObject("Sound Manager").AddComponent<SoundManager>();
                soundManager.transform.SetParent(transform, false);
            }

            soundManager.Initialize();
            soundManager.ApplyVolumes(settings);
            ApplyDirectVisuals();
            BindButtons();
        }

        private void Start()
        {
            soundManager.PlayBgm(titleBgm);
        }

        public void StartGame()
        {
            soundManager.PlaySfx(confirmClip);
            SceneManager.LoadScene(gameSceneName);
        }

        public void QuitGame()
        {
            soundManager.PlayUi(uiClickClip);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetReferences(
            Image background,
            Image titleLogo,
            Button start,
            Button quit,
            AudioClip titleMusic,
            AudioClip uiClick,
            AudioClip confirm,
            string targetSceneName)
        {
            backgroundImage = background;
            titleLogoImage = titleLogo;
            startButton = start;
            quitButton = quit;
            titleBgm = titleMusic;
            uiClickClip = uiClick;
            confirmClip = confirm;
            gameSceneName = targetSceneName;
        }

        private void ApplyDirectVisuals()
        {
            SetImageReady(backgroundImage);
            SetImageReady(titleLogoImage);
            SetButtonReady(startButton);
            SetButtonReady(quitButton);
        }

        private void BindButtons()
        {
            BindButton(startButton, StartGame);
            BindButton(quitButton, QuitGame);
        }

        private static void BindButton(Button button, UnityEngine.Events.UnityAction callback)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        private static void SetButtonReady(Button button)
        {
            if (button == null)
            {
                return;
            }

            SetImageReady(button.GetComponent<Image>());
        }

        private static void SetImageReady(Image image)
        {
            if (image == null)
            {
                return;
            }

            image.color = Color.white;
        }
    }
}
