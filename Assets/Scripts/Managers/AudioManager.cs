// -----------------------------------------------------------------------------
// Codex comment pass: Audio Cue
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\AudioManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Lists the supported Audio Cue states so callers can branch without fragile string comparisons.
    public enum AudioCue
    {
        UiClick,
        UiConfirm,
        UiFail,
        DoorMove,
        DoorUnlock,
        DoorLocked,
        ItemPickup,
        PuzzleSuccess,
        PuzzleFail,
        ChaseStart,
        GameOverImpact
    }

    // Runtime owner for the Audio Manager system, keeping shared state and events behind one access point.
    public class AudioManager : Singleton<AudioManager>
    {
        // Stores the title Music value loaded from audio_settings.json.
        private AudioClip titleMusic;
        // Stores the gameplay Music value loaded from audio_settings.json.
        private AudioClip gameplayMusic;
        // Stores the chase Music value loaded from audio_settings.json.
        private AudioClip chaseMusic;
        // Stores the ending Music value loaded from audio_settings.json.
        private AudioClip endingMusic;
        // Stores the title Ambience value loaded from audio_settings.json.
        private AudioClip titleAmbience;
        // Stores the room Ambience value loaded from audio_settings.json.
        private AudioClip roomAmbience;

        // Stores the door Move Sfx value loaded from audio_settings.json.
        private AudioClip doorMoveSfx;
        // Stores the door Unlock Sfx value loaded from audio_settings.json.
        private AudioClip doorUnlockSfx;
        // Stores the door Locked Sfx value loaded from audio_settings.json.
        private AudioClip doorLockedSfx;
        // Stores the item Pickup Sfx value loaded from audio_settings.json.
        private AudioClip itemPickupSfx;
        // Stores the chase Start Sfx value loaded from audio_settings.json.
        private AudioClip chaseStartSfx;
        // Stores the game Over Impact Sfx value loaded from audio_settings.json.
        private AudioClip gameOverImpactSfx;

        // Stores the ui Click Sfx value loaded from audio_settings.json.
        private AudioClip uiClickSfx;
        // Stores the ui Confirm Sfx value loaded from audio_settings.json.
        private AudioClip uiConfirmSfx;
        // Stores the ui Fail Sfx value loaded from audio_settings.json.
        private AudioClip uiFailSfx;

        // Stores the music Volume value loaded from audio_settings.json.
        private float musicVolume = 0.55f;
        // Stores the ambience Volume value loaded from audio_settings.json.
        private float ambienceVolume = 0.22f;
        // Stores the sfx Volume value loaded from audio_settings.json.
        private float sfxVolume = 0.85f;
        // Stores the ui Volume value loaded from audio_settings.json.
        private float uiVolume = 0.75f;
        // Stores the fade Seconds value loaded from audio_settings.json.
        private float fadeSeconds = 0.75f;

        // Stores the music Source value used by this script's runtime or editor workflow.
        private AudioSource musicSource;
        // Stores the ambience Source value used by this script's runtime or editor workflow.
        private AudioSource ambienceSource;
        // Stores the sfx Source value used by this script's runtime or editor workflow.
        private AudioSource sfxSource;
        // Stores the ui Source value used by this script's runtime or editor workflow.
        private AudioSource uiSource;
        // Stores the music Fade Routine value used by this script's runtime or editor workflow.
        private Coroutine musicFadeRoutine;
        // Stores the ambience Fade Routine value used by this script's runtime or editor workflow.
        private Coroutine ambienceFadeRoutine;
        // Stores the subscribed Game Manager value used by this script's runtime or editor workflow.
        private GameManager subscribedGameManager;
        // Stores the subscribed Chase Manager value used by this script's runtime or editor workflow.
        private ChaseManager subscribedChaseManager;
        // Stores the subscribed Hide Manager value used by this script's runtime or editor workflow.
        private HideManager subscribedHideManager;
        // Stores the subscribed Noise Manager value used by this script's runtime or editor workflow.
        private NoiseManager subscribedNoiseManager;
        // Stores the last Game Over Impact Time value used by this script's runtime or editor workflow.
        private float lastGameOverImpactTime = -999f;

        protected override bool UseDontDestroyOnLoad
        {
            get { return true; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            CreateSources();
            LoadAudioSettings();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            if (Instance != this)
            {
                return;
            }

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (Instance != this)
            {
                return;
            }

            RefreshSubscriptions();
            LoadAudioSettings();
            ApplyCurrentAudioState(true);
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (Instance != this)
            {
                return;
            }

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            ClearSubscriptions();
        }

        // Performs the Play Sfx operation while keeping its implementation details inside this script.
        public void PlaySfx(AudioCue cue)
        {
            PlaySfx(cue, 1f);
        }

        // Performs the Play Sfx operation while keeping its implementation details inside this script.
        public void PlaySfx(AudioCue cue, float volumeScale)
        {
            AudioClip clip = GetClip(cue);
            if (clip == null || sfxSource == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * sfxVolume);
        }

        // Performs the Play Ui operation while keeping its implementation details inside this script.
        public void PlayUi(AudioCue cue)
        {
            PlayUi(cue, 1f);
        }

        // Performs the Play Ui operation while keeping its implementation details inside this script.
        public void PlayUi(AudioCue cue, float volumeScale)
        {
            AudioClip clip = GetClip(cue);
            if (clip == null || uiSource == null)
            {
                return;
            }

            uiSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * uiVolume);
        }

        // Performs the Handle Scene Loaded operation while keeping its implementation details inside this script.
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshSubscriptions();
            ApplyCurrentAudioState(false);
        }

        // Performs the Handle Game State Changed operation while keeping its implementation details inside this script.
        private void HandleGameStateChanged(GameState state)
        {
            ApplyState(state, false);
        }

        // Performs the Handle Chase Started operation while keeping its implementation details inside this script.
        private void HandleChaseStarted()
        {
            PlaySfx(AudioCue.ChaseStart);
        }

        // Performs the Handle Chase Failed operation while keeping its implementation details inside this script.
        private void HandleChaseFailed()
        {
            PlayGameOverImpact();
        }

        // Performs the Handle Hide Entered operation while keeping its implementation details inside this script.
        private void HandleHideEntered(string hidePointId)
        {
            PlaySfx(AudioCue.DoorMove, 0.45f);
        }

        // Performs the Handle Hide Became Safe operation while keeping its implementation details inside this script.
        private void HandleHideBecameSafe(string hidePointId)
        {
            PlayUi(AudioCue.UiConfirm, 0.55f);
        }

        // Performs the Handle Noise Made operation while keeping its implementation details inside this script.
        private void HandleNoiseMade(string locationId, string sourceId)
        {
            PlaySfx(AudioCue.PuzzleFail, 0.45f);
        }

        // Applies calculated settings to Unity components or runtime state.
        private void ApplyCurrentAudioState(bool instant)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.None)
            {
                ApplyState(GameManager.Instance.CurrentState, instant);
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid() && activeScene.name == "TitleScene")
            {
                ApplyState(GameState.Title, instant);
            }
            else
            {
                ApplyState(GameState.Playing, instant);
            }
        }

        // Applies calculated settings to Unity components or runtime state.
        private void ApplyState(GameState state, bool instant)
        {
            switch (state)
            {
                case GameState.Title:
                    PlayMusic(titleMusic, true, instant);
                    PlayAmbience(titleAmbience, true, instant);
                    break;
                case GameState.Chase:
                    PlayMusic(chaseMusic, true, instant);
                    PlayAmbience(roomAmbience, true, instant);
                    break;
                case GameState.GameOver:
                    PlayMusic(titleMusic, true, instant);
                    PlayAmbience(null, false, instant);
                    PlayGameOverImpact();
                    break;
                case GameState.Ending:
                    PlayMusic(endingMusic, false, instant);
                    PlayAmbience(null, false, instant);
                    break;
                case GameState.Playing:
                case GameState.Puzzle:
                case GameState.Examine:
                case GameState.Hiding:
                    PlayMusic(gameplayMusic, true, instant);
                    PlayAmbience(roomAmbience, true, instant);
                    break;
            }
        }

        // Performs the Play Music operation while keeping its implementation details inside this script.
        private void PlayMusic(AudioClip clip, bool loop, bool instant)
        {
            PlayLoopingClip(musicSource, clip, loop, musicVolume, instant, ref musicFadeRoutine);
        }

        // Performs the Play Ambience operation while keeping its implementation details inside this script.
        private void PlayAmbience(AudioClip clip, bool loop, bool instant)
        {
            PlayLoopingClip(ambienceSource, clip, loop, ambienceVolume, instant, ref ambienceFadeRoutine);
        }

        // Performs the Play Looping Clip operation while keeping its implementation details inside this script.
        private void PlayLoopingClip(AudioSource source, AudioClip clip, bool loop, float targetVolume, bool instant, ref Coroutine routine)
        {
            if (source == null)
            {
                return;
            }

            if (source.clip == clip && source.loop == loop && source.isPlaying)
            {
                source.volume = targetVolume;
                return;
            }

            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            if (instant || fadeSeconds <= 0f)
            {
                source.Stop();
                source.clip = clip;
                source.loop = loop;
                source.volume = targetVolume;

                if (clip != null)
                {
                    source.Play();
                }

                return;
            }

            routine = StartCoroutine(FadeToClip(source, clip, loop, targetVolume));
        }

        // Performs the Fade To Clip operation while keeping its implementation details inside this script.
        private IEnumerator FadeToClip(AudioSource source, AudioClip nextClip, bool loop, float targetVolume)
        {
            float startVolume = source.volume;
            float halfFade = Mathf.Max(0.01f, fadeSeconds * 0.5f);
            for (float time = 0f; time < halfFade; time += Time.unscaledDeltaTime)
            {
                source.volume = Mathf.Lerp(startVolume, 0f, time / halfFade);
                yield return null;
            }

            source.Stop();
            source.clip = nextClip;
            source.loop = loop;
            source.volume = 0f;

            if (nextClip != null)
            {
                source.Play();
            }

            for (float time = 0f; time < halfFade; time += Time.unscaledDeltaTime)
            {
                source.volume = Mathf.Lerp(0f, targetVolume, time / halfFade);
                yield return null;
            }

            source.volume = nextClip != null ? targetVolume : 0f;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private AudioClip GetClip(AudioCue cue)
        {
            switch (cue)
            {
                case AudioCue.UiClick:
                    return uiClickSfx;
                case AudioCue.UiConfirm:
                case AudioCue.PuzzleSuccess:
                    return uiConfirmSfx;
                case AudioCue.UiFail:
                case AudioCue.PuzzleFail:
                    return uiFailSfx;
                case AudioCue.DoorMove:
                    return doorMoveSfx;
                case AudioCue.DoorUnlock:
                    return doorUnlockSfx;
                case AudioCue.DoorLocked:
                    return doorLockedSfx;
                case AudioCue.ItemPickup:
                    return itemPickupSfx;
                case AudioCue.ChaseStart:
                    return chaseStartSfx;
                case AudioCue.GameOverImpact:
                    return gameOverImpactSfx;
                default:
                    return null;
            }
        }

        // Loads audio clip Resources paths and mix values from StreamingAssets/Data/audio_settings.json.
        private void LoadAudioSettings()
        {
            AudioSettingsRecord settings = null;
            if (GameDataManager.Instance != null && HasAnyAudioPath(GameDataManager.Instance.AudioSettings))
            {
                settings = GameDataManager.Instance.AudioSettings;
            }

            if (settings == null)
            {
                settings = LoadAudioSettingsFromJson();
            }

            if (settings == null)
            {
                return;
            }

            musicVolume = Mathf.Clamp01(settings.musicVolume);
            ambienceVolume = Mathf.Clamp01(settings.ambienceVolume);
            sfxVolume = Mathf.Clamp01(settings.sfxVolume);
            uiVolume = Mathf.Clamp01(settings.uiVolume);
            fadeSeconds = Mathf.Max(0f, settings.fadeSeconds);

            titleMusic = LoadClip(settings.titleMusicPath, "titleMusicPath");
            gameplayMusic = LoadClip(settings.gameplayMusicPath, "gameplayMusicPath");
            chaseMusic = LoadClip(settings.chaseMusicPath, "chaseMusicPath");
            endingMusic = LoadClip(settings.endingMusicPath, "endingMusicPath");
            titleAmbience = LoadClip(settings.titleAmbiencePath, "titleAmbiencePath");
            roomAmbience = LoadClip(settings.roomAmbiencePath, "roomAmbiencePath");

            doorMoveSfx = LoadClip(settings.doorMoveSfxPath, "doorMoveSfxPath");
            doorUnlockSfx = LoadClip(settings.doorUnlockSfxPath, "doorUnlockSfxPath");
            doorLockedSfx = LoadClip(settings.doorLockedSfxPath, "doorLockedSfxPath");
            itemPickupSfx = LoadClip(settings.itemPickupSfxPath, "itemPickupSfxPath");
            chaseStartSfx = LoadClip(settings.chaseStartSfxPath, "chaseStartSfxPath");
            gameOverImpactSfx = LoadClip(settings.gameOverImpactSfxPath, "gameOverImpactSfxPath");

            uiClickSfx = LoadClip(settings.uiClickSfxPath, "uiClickSfxPath");
            uiConfirmSfx = LoadClip(settings.uiConfirmSfxPath, "uiConfirmSfxPath");
            uiFailSfx = LoadClip(settings.uiFailSfxPath, "uiFailSfxPath");
        }

        // Loads audio settings directly when GameDataManager has not completed its Start-time load yet.
        private AudioSettingsRecord LoadAudioSettingsFromJson()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Data", "audio_settings.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning("Audio settings data file not found: " + path);
                return null;
            }

            try
            {
                AudioSettingsWrapper wrapper = JsonUtility.FromJson<AudioSettingsWrapper>(File.ReadAllText(path));
                if (wrapper == null || wrapper.audio == null)
                {
                    Debug.LogWarning("Audio settings data file could not be parsed: " + path);
                    return null;
                }

                return wrapper.audio;
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("Exception while reading audio_settings.json: " + exception.Message);
                return null;
            }
        }

        // Queries whether the record contains at least one Resources path.
        private bool HasAnyAudioPath(AudioSettingsRecord settings)
        {
            return settings != null &&
                (!string.IsNullOrEmpty(settings.titleMusicPath) ||
                !string.IsNullOrEmpty(settings.gameplayMusicPath) ||
                !string.IsNullOrEmpty(settings.chaseMusicPath) ||
                !string.IsNullOrEmpty(settings.endingMusicPath) ||
                !string.IsNullOrEmpty(settings.titleAmbiencePath) ||
                !string.IsNullOrEmpty(settings.roomAmbiencePath) ||
                !string.IsNullOrEmpty(settings.doorMoveSfxPath) ||
                !string.IsNullOrEmpty(settings.doorUnlockSfxPath) ||
                !string.IsNullOrEmpty(settings.doorLockedSfxPath) ||
                !string.IsNullOrEmpty(settings.itemPickupSfxPath) ||
                !string.IsNullOrEmpty(settings.chaseStartSfxPath) ||
                !string.IsNullOrEmpty(settings.gameOverImpactSfxPath) ||
                !string.IsNullOrEmpty(settings.uiClickSfxPath) ||
                !string.IsNullOrEmpty(settings.uiConfirmSfxPath) ||
                !string.IsNullOrEmpty(settings.uiFailSfxPath));
        }

        // Loads an AudioClip from a Resources path and reports missing optional setup without interrupting gameplay.
        private AudioClip LoadClip(string resourcesPath, string fieldName)
        {
            if (string.IsNullOrEmpty(resourcesPath))
            {
                Debug.LogWarning("Audio settings path is empty: " + fieldName);
                return null;
            }

            AudioClip clip = Resources.Load<AudioClip>(resourcesPath);
            if (clip == null)
            {
                Debug.LogWarning("Audio clip not found at Resources path: " + resourcesPath);
            }

            return clip;
        }

        // Performs the Play Game Over Impact operation while keeping its implementation details inside this script.
        private void PlayGameOverImpact()
        {
            if (Time.unscaledTime - lastGameOverImpactTime < 0.25f)
            {
                return;
            }

            lastGameOverImpactTime = Time.unscaledTime;
            PlaySfx(AudioCue.GameOverImpact);
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private void CreateSources()
        {
            musicSource = CreateSource("MusicSource", true);
            ambienceSource = CreateSource("AmbienceSource", true);
            sfxSource = CreateSource("SfxSource", false);
            uiSource = CreateSource("UiSource", false);
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        private AudioSource CreateSource(string sourceName, bool loop)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform, false);

            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            return source;
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        private void RefreshSubscriptions()
        {
            SubscribeGameManager(GameManager.Instance);
            SubscribeChaseManager(ChaseManager.Instance);
            SubscribeHideManager(HideManager.Instance);
            SubscribeNoiseManager(NoiseManager.Instance);
        }

        // Performs the Clear Subscriptions operation while keeping its implementation details inside this script.
        private void ClearSubscriptions()
        {
            SubscribeGameManager(null);
            SubscribeChaseManager(null);
            SubscribeHideManager(null);
            SubscribeNoiseManager(null);
        }

        // Performs the Subscribe Game Manager operation while keeping its implementation details inside this script.
        private void SubscribeGameManager(GameManager manager)
        {
            if (subscribedGameManager == manager)
            {
                return;
            }

            if (subscribedGameManager != null)
            {
                subscribedGameManager.StateChanged -= HandleGameStateChanged;
            }

            subscribedGameManager = manager;

            if (subscribedGameManager != null)
            {
                subscribedGameManager.StateChanged -= HandleGameStateChanged;
                subscribedGameManager.StateChanged += HandleGameStateChanged;
            }
        }

        // Performs the Subscribe Chase Manager operation while keeping its implementation details inside this script.
        private void SubscribeChaseManager(ChaseManager manager)
        {
            if (subscribedChaseManager == manager)
            {
                return;
            }

            if (subscribedChaseManager != null)
            {
                subscribedChaseManager.ChaseStarted -= HandleChaseStarted;
                subscribedChaseManager.ChaseFailed -= HandleChaseFailed;
            }

            subscribedChaseManager = manager;

            if (subscribedChaseManager != null)
            {
                subscribedChaseManager.ChaseStarted -= HandleChaseStarted;
                subscribedChaseManager.ChaseStarted += HandleChaseStarted;
                subscribedChaseManager.ChaseFailed -= HandleChaseFailed;
                subscribedChaseManager.ChaseFailed += HandleChaseFailed;
            }
        }

        // Performs the Subscribe Hide Manager operation while keeping its implementation details inside this script.
        private void SubscribeHideManager(HideManager manager)
        {
            if (subscribedHideManager == manager)
            {
                return;
            }

            if (subscribedHideManager != null)
            {
                subscribedHideManager.HideEntered -= HandleHideEntered;
                subscribedHideManager.HideBecameSafe -= HandleHideBecameSafe;
            }

            subscribedHideManager = manager;

            if (subscribedHideManager != null)
            {
                subscribedHideManager.HideEntered -= HandleHideEntered;
                subscribedHideManager.HideEntered += HandleHideEntered;
                subscribedHideManager.HideBecameSafe -= HandleHideBecameSafe;
                subscribedHideManager.HideBecameSafe += HandleHideBecameSafe;
            }
        }

        // Performs the Subscribe Noise Manager operation while keeping its implementation details inside this script.
        private void SubscribeNoiseManager(NoiseManager manager)
        {
            if (subscribedNoiseManager == manager)
            {
                return;
            }

            if (subscribedNoiseManager != null)
            {
                subscribedNoiseManager.NoiseMade -= HandleNoiseMade;
            }

            subscribedNoiseManager = manager;

            if (subscribedNoiseManager != null)
            {
                subscribedNoiseManager.NoiseMade -= HandleNoiseMade;
                subscribedNoiseManager.NoiseMade += HandleNoiseMade;
            }
        }
    }
}
