using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Creates and controls the fallback stage clear UI.
    /// </summary>
    public static class StageClearPresenter
    {
        public static GameDirector.StageClearUi CreatePanel(RectTransform root, Sprite backgroundSprite, UnityAction titleAction)
        {
            var panel = CreatePanelObject("StageClearPanel", root, Color.black);
            Stretch(panel.GetComponent<RectTransform>(), new Rect(0f, 0f, 1f, 1f));

            var background = CreateImage("StageClearBackground", panel.transform, Color.white);
            background.sprite = backgroundSprite;
            background.preserveAspect = true;
            Stretch(background.rectTransform, new Rect(0f, 0f, 1f, 1f));

            var title = CreateText("StageClearTitle", panel.transform, 52, TextAnchor.MiddleCenter);
            title.text = "Stage Clear";
            title.color = new Color(0.96f, 0.92f, 0.78f, 1f);
            Stretch(title.rectTransform, new Rect(0.24f, 0.64f, 0.52f, 0.14f));

            var titleButton = CreateTextButton("StageClearTitleButton", panel.transform, "Title", titleAction);
            Stretch(titleButton.GetComponent<RectTransform>(), new Rect(0.42f, 0.18f, 0.16f, 0.08f));

            var group = EnsureCanvasGroup(panel);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
            panel.SetActive(false);
            return new GameDirector.StageClearUi(panel, background, titleButton);
        }

        private static GameObject CreatePanelObject(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            var image = imageObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, int size, TextAnchor anchor)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = anchor;
            text.color = Color.white;
            return text;
        }

        private static Button CreateTextButton(string name, Transform parent, string label, UnityAction action)
        {
            var buttonObject = CreatePanelObject(name, parent, new Color(0.08f, 0.07f, 0.08f, 0.95f));
            var button = buttonObject.AddComponent<Button>();
            var text = CreateText("Text", buttonObject.transform, 22, TextAnchor.MiddleCenter);
            text.text = label;
            Stretch(text.rectTransform, new Rect(0f, 0f, 1f, 1f));
            if (action != null)
            {
                button.onClick.AddListener(action);
            }

            return button;
        }

        private static void Stretch(RectTransform rectTransform, Rect normalizedRect)
        {
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = new Vector2(normalizedRect.xMin, normalizedRect.yMin);
            rectTransform.anchorMax = new Vector2(normalizedRect.xMax, normalizedRect.yMax);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static CanvasGroup EnsureCanvasGroup(GameObject panel)
        {
            var group = panel.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = panel.AddComponent<CanvasGroup>();
            }

            return group;
        }
    }
}
