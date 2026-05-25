using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EscapeFromNightmares.UI
{
    /// <summary>
    /// 타이틀 씬의 배경, 버튼, 설정 패널, 시작/종료 흐름을 제어합니다.
    /// </summary>
    public sealed class TitleSceneController : MonoBehaviour
    {
        [SerializeField] private ResourcePathCatalog resourceCatalog;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image titleLogoImage;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private SettingsAudioPanel settingsPanel;
        [SerializeField] private string gameSceneName = "SampleScene";

        private ResourceManager resourceManager;
        private SoundManager soundManager;
        private SettingsSaveService settingsSaveService;
        private SettingsSaveService.SettingsData settings;

        private void Awake()
        {
            if (resourceCatalog == null)
            {
                resourceCatalog = Resources.Load<ResourcePathCatalog>("EscapeFromNightmares/ResourcePathCatalog");
            }

            if (resourceCatalog == null)
            {
                resourceCatalog = ResourcePathCatalog.CreateDefault();
            }
            resourceManager = new ResourceManager(resourceCatalog);
            settingsSaveService = new SettingsSaveService(Application.persistentDataPath);
            settings = settingsSaveService.LoadSettings();
            soundManager = GetComponentInChildren<SoundManager>();
            if (soundManager == null)
            {
                soundManager = new GameObject("Sound Manager").AddComponent<SoundManager>();
                soundManager.transform.SetParent(transform, false);
            }

            soundManager.Initialize(resourceManager, audioMixer);
            soundManager.ApplyVolumes(settings);
            ApplyResources();
            BindButtons();
            InitializeSettingsPanel();
        }

        private void Start()
        {
            soundManager.PlayBgm(resourceCatalog.titleBgmPath);
        }

        /// <summary>
        /// 확인음을 재생하고 설정된 게임 씬으로 이동합니다.
        /// </summary>
        public void StartGame()
        {
            soundManager.PlaySfx(resourceCatalog.confirmSfxPath);
            SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// UI 클릭음을 재생하고 오디오 설정 패널을 엽니다.
        /// </summary>
        public void OpenSettings()
        {
            soundManager.PlayUi(resourceCatalog.uiClickPath);
            if (settingsPanel != null)
            {
                settingsPanel.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// UI 클릭음을 재생한 뒤 에디터에서는 Play Mode를 종료하고 빌드에서는 애플리케이션을 종료합니다.
        /// </summary>
        public void QuitGame()
        {
            soundManager.PlayUi(resourceCatalog.uiClickPath);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 타이틀 로고가 없는 구버전 호출부에서 씬 참조를 주입할 때 사용합니다.
        /// </summary>
        public void SetReferences(
            ResourcePathCatalog catalog,
            AudioMixer mixer,
            Image background,
            Button start,
            Button settingsButtonReference,
            Button quit,
            SettingsAudioPanel audioPanel,
            string targetSceneName)
        {
            SetReferences(catalog, mixer, background, null, start, settingsButtonReference, quit, audioPanel, targetSceneName);
        }

        /// <summary>
        /// 에디터 빌더가 생성한 타이틀 씬 UI 참조와 이동 대상 씬 이름을 주입합니다.
        /// </summary>
        public void SetReferences(
            ResourcePathCatalog catalog,
            AudioMixer mixer,
            Image background,
            Image titleLogo,
            Button start,
            Button settingsButtonReference,
            Button quit,
            SettingsAudioPanel audioPanel,
            string targetSceneName)
        {
            resourceCatalog = catalog;
            audioMixer = mixer;
            backgroundImage = background;
            titleLogoImage = titleLogo;
            startButton = start;
            settingsButton = settingsButtonReference;
            quitButton = quit;
            settingsPanel = audioPanel;
            gameSceneName = targetSceneName;
        }

        private void ApplyResources()
        {
            if (backgroundImage != null)
            {
                if (backgroundImage.sprite == null)
                {
                    backgroundImage.sprite = resourceManager.LoadSprite(resourceCatalog.titleBackgroundPath);
                }

                backgroundImage.color = Color.white;
            }

            SetImageSprite(titleLogoImage, resourceCatalog.titleLogoPath);
            SetButtonSprite(startButton, resourceCatalog.titleStartButtonPath);
            SetButtonSprite(settingsButton, resourceCatalog.titleSettingsButtonPath);
            SetButtonSprite(quitButton, resourceCatalog.titleQuitButtonPath);
            settingsPanel?.ApplySprites(resourceManager, resourceCatalog);
        }

        private void BindButtons()
        {
            BindButton(startButton, StartGame);
            BindButton(settingsButton, OpenSettings);
            BindButton(quitButton, QuitGame);
        }

        private void InitializeSettingsPanel()
        {
            if (settingsPanel == null)
            {
                return;
            }

            settingsPanel.Initialize(settingsSaveService, soundManager, settings);
            settingsPanel.gameObject.SetActive(false);
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

        private void SetButtonSprite(Button button, string path)
        {
            if (button == null)
            {
                return;
            }

            SetImageSprite(button.GetComponent<Image>(), path);
        }

        private void SetImageSprite(Image image, string path)
        {
            if (image == null)
            {
                return;
            }

            if (image.sprite == null)
            {
                image.sprite = resourceManager.LoadSprite(path);
            }

            image.color = Color.white;
        }
    }
}
