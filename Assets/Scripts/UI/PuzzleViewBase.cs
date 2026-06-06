using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
	public abstract class PuzzleViewBase : MonoBehaviour
	{
		[SerializeField] private string puzzleId;
		[SerializeField] private string title;

		private bool isBuilt;
		private UIPuzzleOverlay overlay;
		private RectTransform rectTransform;

		public string PuzzleId => string.IsNullOrWhiteSpace(puzzleId) ? DefaultPuzzleId : puzzleId;
		public string Title => string.IsNullOrWhiteSpace(title) ? DefaultTitle : title;

		protected abstract string DefaultPuzzleId { get; }
		protected abstract string DefaultTitle { get; }
		protected UIPuzzleOverlay Overlay => overlay;

		public void Open(UIPuzzleOverlay owner)
		{
			overlay = owner;
			EnsureBuilt();
			OnOpened();
		}

		public void Close()
		{
			OnClosed();
		}

		protected virtual void OnOpened()
		{
		}

		protected virtual void OnClosed()
		{
		}

		protected abstract void BuildPuzzle(RectTransform root);

		protected void CompletePuzzle()
		{
			overlay?.CompleteCurrentPuzzle();
		}

		protected void ShowMessage(string message)
		{
			overlay?.SetMessage(message);
		}

		protected RectTransform CreatePanel(string name, Transform parent)
		{
			GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
			panelObject.layer = 5;
			panelObject.transform.SetParent(parent, false);
			RectTransform rect = panelObject.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(920f, 520f);

			Image image = panelObject.GetComponent<Image>();
			image.color = new Color(0.1f, 0.09f, 0.08f, 0.92f);
			image.raycastTarget = true;
			return rect;
		}

		protected Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment)
		{
			GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
			textObject.layer = 5;
			textObject.transform.SetParent(parent, false);
			RectTransform rect = textObject.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = new Vector2(0f, 210f);
			rect.sizeDelta = new Vector2(820f, 70f);
			Text text = textObject.GetComponent<Text>();
			text.text = value;
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			if (text.font == null)
			{
				text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			text.fontSize = fontSize;
			text.alignment = alignment;
			text.color = new Color(0.93f, 0.89f, 0.78f, 1f);
			text.raycastTarget = false;
			return text;
		}

		protected Button CreateButton(string name, Transform parent, string label)
		{
			GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
			buttonObject.layer = 5;
			buttonObject.transform.SetParent(parent, false);

			Image image = buttonObject.GetComponent<Image>();
			image.color = new Color(0.28f, 0.23f, 0.18f, 0.96f);
			image.raycastTarget = true;

			Button button = buttonObject.GetComponent<Button>();
			button.targetGraphic = image;

			Text text = CreateText("Text", buttonObject.transform, label, 22, TextAnchor.MiddleCenter);
			RectTransform textRect = text.GetComponent<RectTransform>();
			Stretch(textRect);
			return button;
		}

		protected Image CreateImage(string name, Transform parent, Sprite sprite, bool raycastTarget)
		{
			GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
			imageObject.layer = 5;
			imageObject.transform.SetParent(parent, false);
			Image image = imageObject.GetComponent<Image>();
			image.sprite = sprite;
			image.color = Color.white;
			image.preserveAspect = true;
			image.raycastTarget = raycastTarget;
			return image;
		}

		protected void SetRect(RectTransform rect, Vector2 anchoredPosition, Vector2 size)
		{
			rect.anchorMin = new Vector2(0.5f, 0.5f);
			rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = anchoredPosition;
			rect.sizeDelta = size;
			rect.localScale = Vector3.one;
		}

		protected void Stretch(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;
			rect.localScale = Vector3.one;
		}

		private void EnsureBuilt()
		{
			if (isBuilt)
			{
				return;
			}

			isBuilt = true;
			rectTransform = GetComponent<RectTransform>();
			if (rectTransform == null)
			{
				rectTransform = gameObject.AddComponent<RectTransform>();
			}

			Stretch(rectTransform);
			BuildPuzzle(rectTransform);
		}
	}
}
