using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public class ScreenFadeManager : Singleton<ScreenFadeManager>
	{
		private const string OverlayName = "ScreenFadeOverlay";

		[SerializeField] private Image fadeImage;
		[SerializeField] private CanvasGroup fadeCanvasGroup;
		[SerializeField] private float fadeDuration = 0.25f;
		[SerializeField] private Color fadeColor = Color.black;

		private Coroutine transitionRoutine;

		public bool IsTransitioning { get; private set; }

		public static ScreenFadeManager EnsureInstance()
		{
			ScreenFadeManager manager = Instance;
			if (manager != null)
			{
				return manager;
			}

			Canvas canvas = FindFirstObjectByType<Canvas>();
			GameObject fadeObject = new GameObject("ScreenFadeManager", typeof(RectTransform));
			if (canvas != null)
			{
				fadeObject.transform.SetParent(canvas.transform, false);
				fadeObject.transform.SetAsLastSibling();
			}

			RectTransform rectTransform = fadeObject.GetComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;

			return fadeObject.AddComponent<ScreenFadeManager>();
		}

		protected override bool DontDestroy => false;

		protected override void Awake()
		{
			base.Awake();

			if (Instance != this)
			{
				return;
			}

			FindFadeTargetsIfNeeded();
			SetAlpha(0f);
			SetBlocksRaycasts(false);
		}

		public void PlayTransition(Action onHidden)
		{
			if (IsTransitioning)
			{
				return;
			}

			transitionRoutine = StartCoroutine(PlayTransitionRoutine(onHidden));
		}

		private IEnumerator PlayTransitionRoutine(Action onHidden)
		{
			IsTransitioning = true;
			SetBlocksRaycasts(true);

			yield return FadeTo(1f);
			onHidden?.Invoke();
			yield return null;
			yield return FadeTo(0f);

			SetBlocksRaycasts(false);
			IsTransitioning = false;
			transitionRoutine = null;
		}

		private IEnumerator FadeTo(float targetAlpha)
		{
			FindFadeTargetsIfNeeded();

			float startAlpha = GetAlpha();
			float elapsed = 0f;
			float duration = Mathf.Max(0.01f, fadeDuration);

			while (elapsed < duration)
			{
				elapsed += Time.unscaledDeltaTime;
				SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration));
				yield return null;
			}

			SetAlpha(targetAlpha);
		}

		private void FindFadeTargetsIfNeeded()
		{
			if (fadeImage == null)
			{
				fadeImage = GetComponentInChildren<Image>(true);
			}

			if (fadeCanvasGroup == null)
			{
				fadeCanvasGroup = GetComponent<CanvasGroup>();
				if (fadeCanvasGroup == null)
				{
					fadeCanvasGroup = GetComponentInChildren<CanvasGroup>(true);
				}
			}

			if (fadeImage == null || fadeCanvasGroup == null)
			{
				EnsureFadeOverlay();
			}
		}

		private void EnsureFadeOverlay()
		{
			Canvas canvas = GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				canvas = FindFirstObjectByType<Canvas>();
			}

			Transform parent = canvas != null ? canvas.transform : transform;
			Transform overlayTransform = parent.Find(OverlayName);
			GameObject overlayObject;
			if (overlayTransform != null)
			{
				overlayObject = overlayTransform.gameObject;
			}
			else
			{
				overlayObject = new GameObject(OverlayName, typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
				overlayObject.transform.SetParent(parent, false);
			}

			overlayObject.transform.SetAsLastSibling();

			RectTransform rectTransform = overlayObject.GetComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;

			fadeCanvasGroup = overlayObject.GetComponent<CanvasGroup>();
			if (fadeCanvasGroup == null)
			{
				fadeCanvasGroup = overlayObject.AddComponent<CanvasGroup>();
			}

			fadeImage = overlayObject.GetComponent<Image>();
			if (fadeImage == null)
			{
				fadeImage = overlayObject.AddComponent<Image>();
			}

			SetAlpha(0f);
			SetBlocksRaycasts(false);
		}

		private float GetAlpha()
		{
			if (fadeCanvasGroup != null)
			{
				return fadeCanvasGroup.alpha;
			}

			return fadeImage != null ? fadeImage.color.a : 0f;
		}

		private void SetAlpha(float alpha)
		{
			if (fadeImage != null)
			{
				Color color = fadeColor;
				color.a = alpha;
				fadeImage.color = color;
			}

			if (fadeCanvasGroup != null)
			{
				fadeCanvasGroup.alpha = alpha;
			}
		}

		private void SetBlocksRaycasts(bool blocksRaycasts)
		{
			if (fadeCanvasGroup != null)
			{
				fadeCanvasGroup.blocksRaycasts = blocksRaycasts;
				fadeCanvasGroup.interactable = blocksRaycasts;
			}

			if (fadeImage != null)
			{
				fadeImage.raycastTarget = blocksRaycasts;
			}
		}
	}
}
