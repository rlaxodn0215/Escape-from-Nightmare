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

        public void StartGame()
        {
            soundManager.PlaySfx(resourceCatalog.confirmSfxPath);
            SceneManager.LoadScene(gameSceneName);
        }

        public void OpenSettings()
        {
            soundManager.PlayUi(resourceCatalog.uiClickPath);
            if (settingsPanel != null)
            {
                settingsPanel.gameObject.SetActive(true);
            }
        }

        public void QuitGame()
        {
            soundManager.PlayUi(resourceCatalog.uiClickPath);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

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
