using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class GameSceneNightmareIntroController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlayCanvasGroup;
        [SerializeField] private Image overlayImage;
        [SerializeField] private RectTransform locationRoot;
        [SerializeField, Range(0.5f, 6f)] private float totalDuration = 2.6f;
        [SerializeField, Range(0f, 1f)] private float startAlpha = 0.96f;
        [SerializeField, Range(0f, 1f)] private float revealAlpha = 0.34f;
        [SerializeField, Range(1f, 1.12f)] private float pullInScale = 1.045f;
        [SerializeField, Range(0f, 24f)] private float shakePixels = 9f;
        [SerializeField, Range(0f, 1f)] private float redPulseStrength = 0.55f;

        private readonly List<TitleBackgroundFlickerState> flickerStates = new List<TitleBackgroundFlickerState>();
        private Coroutine introRoutine;
        private GameManager subscribedGameManager;
        private bool hasStartedIntro;
        private bool subscribedToSceneLoads;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneLoadHook()
        {
            SceneManager.sceneLoaded -= HandleSceneLoadedForBootstrap;
            SceneManager.sceneLoaded += HandleSceneLoadedForBootstrap;
        }

        private static void HandleSceneLoadedForBootstrap(Scene scene, LoadSceneMode mode)
        {
            string gameSceneName = GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.GameSceneName)
                ? GameManager.Instance.GameSceneName
                : "GameScene";

            if (scene.IsValid() && scene.name == gameSceneName)
            {
                EnsureSceneInstance();
            }
        }

        public static GameSceneNightmareIntroController EnsureSceneInstance()
        {
            GameSceneNightmareIntroController existing = UnityEngine.Object.FindFirstObjectByType<GameSceneNightmareIntroController>(FindObjectsInactive.Include);
            if (existing != null)
            {
                return existing;
            }

            Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            if (canvas == null)
            {
                return null;
            }

            GameObject overlayObject = new GameObject("NightmareIntroOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            overlayObject.transform.SetParent(canvas.transform, false);
            RectTransform rect = overlayObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            Image image = overlayObject.GetComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = true;

            CanvasGroup group = overlayObject.GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            GameSceneNightmareIntroController controller = overlayObject.AddComponent<GameSceneNightmareIntroController>();
            controller.overlayCanvasGroup = group;
            controller.overlayImage = image;
            GameObject locationRootObject = GameObject.Find("LocationRoot");
            controller.locationRoot = locationRootObject != null ? locationRootObject.GetComponent<RectTransform>() : null;
            overlayObject.transform.SetAsLastSibling();
            return controller;
        }

        private void Awake()
        {
            CacheReferences();
            SetOverlay(0f, Color.black, false);
        }

        private void OnEnable()
        {
            Subscribe(GameManager.Instance);
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            subscribedToSceneLoads = true;
        }

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.LastGameSceneStartMode == GameStartMode.NewGame)
            {
                TryPlayIntro(GameStartMode.NewGame);
            }
        }

        private void OnDisable()
        {
            Subscribe(null);
            if (subscribedToSceneLoads)
            {
                SceneManager.sceneLoaded -= HandleSceneLoaded;
                subscribedToSceneLoads = false;
            }
        }

        private void OnDestroy()
        {
            RestoreFlickers();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            hasStartedIntro = false;
            Subscribe(GameManager.Instance);
        }

        private void HandleGameSceneStarted(GameStartMode mode)
        {
            TryPlayIntro(mode);
        }

        private void TryPlayIntro(GameStartMode mode)
        {
            if (mode != GameStartMode.NewGame || hasStartedIntro)
            {
                return;
            }

            hasStartedIntro = true;
            CacheReferences();
            if (overlayCanvasGroup == null || overlayImage == null)
            {
                Debug.LogWarning("Nightmare intro overlay is missing required UI references.", this);
                return;
            }

            if (introRoutine != null)
            {
                StopCoroutine(introRoutine);
            }

            introRoutine = StartCoroutine(PlayIntroRoutine());
        }

        private IEnumerator PlayIntroRoutine()
        {
            Vector3 originalScale = locationRoot != null ? locationRoot.localScale : Vector3.one;
            Vector2 originalAnchoredPosition = locationRoot != null ? locationRoot.anchoredPosition : Vector2.zero;
            EnableActiveViewFlickers();

            if (overlayCanvasGroup != null)
            {
                overlayCanvasGroup.gameObject.SetActive(true);
            }

            SetOverlay(startAlpha, Color.black, true);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(AudioCue.GameOverImpact, 0.35f);
            }

            float duration = Mathf.Max(0.1f, totalDuration);
            for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
            {
                float revealT = Mathf.Clamp01((time - 0.2f) / 1.1f);
                float fadeOutT = Mathf.Clamp01((time - 1.8f) / 0.8f);
                float redPulse = Mathf.Max(GetPulse(time, 0.92f, 0.18f), GetPulse(time, 1.46f, 0.16f)) * redPulseStrength;
                float alpha = Mathf.Lerp(startAlpha, revealAlpha, Smooth(revealT));

                if (time > 0.8f && time < 1.8f)
                {
                    alpha += Mathf.Sin(time * 17f) * 0.045f;
                    alpha += redPulse * 0.2f;
                }

                if (time > 1.8f)
                {
                    alpha = Mathf.Lerp(alpha, 0f, Smooth(fadeOutT));
                }

                Color tint = Color.Lerp(Color.black, new Color(0.42f, 0f, 0.02f, 1f), redPulse);
                SetOverlay(Mathf.Clamp01(alpha), tint, true);
                ApplyPullInTransform(time, originalScale, originalAnchoredPosition);

                if (AudioManager.Instance != null && time >= 0.2f && time - Time.unscaledDeltaTime < 0.2f)
                {
                    AudioManager.Instance.PlaySfx(AudioCue.ChaseStart, 0.18f);
                }

                yield return null;
            }

            RestoreLocationRoot(originalScale, originalAnchoredPosition);
            RestoreFlickers();
            SetOverlay(0f, Color.black, false);

            if (overlayCanvasGroup != null)
            {
                overlayCanvasGroup.gameObject.SetActive(false);
            }

            introRoutine = null;
        }

        private void ApplyPullInTransform(float time, Vector3 originalScale, Vector2 originalAnchoredPosition)
        {
            if (locationRoot == null)
            {
                return;
            }

            float pullT = Mathf.Clamp01((time - 0.2f) / 1.1f);
            float releaseT = Mathf.Clamp01((time - 1.8f) / 0.8f);
            float scaleAmount = Mathf.Lerp(1f, pullInScale, Smooth(pullT));
            scaleAmount = Mathf.Lerp(scaleAmount, 1f, Smooth(releaseT));

            float shakeFade = 1f - Smooth(Mathf.Clamp01((time - 1.4f) / 1.1f));
            float shake = shakePixels * shakeFade;
            Vector2 offset = new Vector2(
                Mathf.Sin(time * 41f) * shake,
                Mathf.Cos(time * 37f) * shake * 0.65f);

            locationRoot.localScale = originalScale * scaleAmount;
            locationRoot.anchoredPosition = originalAnchoredPosition + offset;
        }

        private void RestoreLocationRoot(Vector3 originalScale, Vector2 originalAnchoredPosition)
        {
            if (locationRoot == null)
            {
                return;
            }

            locationRoot.localScale = originalScale;
            locationRoot.anchoredPosition = originalAnchoredPosition;
        }

        private void EnableActiveViewFlickers()
        {
            RestoreFlickers();
            if (locationRoot == null)
            {
                return;
            }

            TitleBackgroundFlicker[] flickers = locationRoot.GetComponentsInChildren<TitleBackgroundFlicker>(true);
            for (int i = 0; i < flickers.Length; i++)
            {
                TitleBackgroundFlicker flicker = flickers[i];
                if (flicker == null || !flicker.gameObject.activeInHierarchy)
                {
                    continue;
                }

                flickerStates.Add(new TitleBackgroundFlickerState(flicker, flicker.enabled));
                flicker.enabled = true;
            }
        }

        private void RestoreFlickers()
        {
            for (int i = 0; i < flickerStates.Count; i++)
            {
                TitleBackgroundFlickerState state = flickerStates[i];
                if (state.Flicker != null)
                {
                    state.Flicker.enabled = state.WasEnabled;
                }
            }

            flickerStates.Clear();
        }

        private void SetOverlay(float alpha, Color color, bool blockInput)
        {
            if (overlayImage != null)
            {
                color.a = 1f;
                overlayImage.color = color;
                overlayImage.raycastTarget = true;
            }

            if (overlayCanvasGroup != null)
            {
                overlayCanvasGroup.alpha = Mathf.Clamp01(alpha);
                overlayCanvasGroup.interactable = blockInput;
                overlayCanvasGroup.blocksRaycasts = blockInput;
            }
        }

        private void CacheReferences()
        {
            if (overlayCanvasGroup == null)
            {
                overlayCanvasGroup = GetComponent<CanvasGroup>();
            }

            if (overlayImage == null)
            {
                overlayImage = GetComponent<Image>();
            }

            if (locationRoot == null)
            {
                GameObject locationRootObject = GameObject.Find("LocationRoot");
                locationRoot = locationRootObject != null ? locationRootObject.GetComponent<RectTransform>() : null;
            }
        }

        private void Subscribe(GameManager manager)
        {
            if (subscribedGameManager == manager)
            {
                return;
            }

            if (subscribedGameManager != null)
            {
                subscribedGameManager.GameSceneStarted -= HandleGameSceneStarted;
            }

            subscribedGameManager = manager;

            if (manager != null)
            {
                manager.GameSceneStarted -= HandleGameSceneStarted;
                manager.GameSceneStarted += HandleGameSceneStarted;
            }
        }

        private static float Smooth(float value)
        {
            return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(value));
        }

        private static float GetPulse(float time, float center, float halfWidth)
        {
            float distance = Mathf.Abs(time - center);
            if (distance >= halfWidth)
            {
                return 0f;
            }

            return 1f - (distance / halfWidth);
        }

        private readonly struct TitleBackgroundFlickerState
        {
            public readonly TitleBackgroundFlicker Flicker;
            public readonly bool WasEnabled;

            public TitleBackgroundFlickerState(TitleBackgroundFlicker flicker, bool wasEnabled)
            {
                Flicker = flicker;
                WasEnabled = wasEnabled;
            }
        }
    }
}
