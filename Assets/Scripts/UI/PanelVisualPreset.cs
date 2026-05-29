using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class PanelVisualPreset : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text[] textElements;
        [SerializeField] private Button[] buttons;
        [SerializeField] private bool applyOnAwake = true;
        [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);
        [SerializeField] private Color textColor = Color.white;

        private void Awake()
        {
            if (applyOnAwake)
            {
                ApplyPreset();
            }
        }

        private void Reset()
        {
            backgroundImage = GetComponent<Image>();
            textElements = GetComponentsInChildren<Text>(true);
            buttons = GetComponentsInChildren<Button>(true);
        }

        public void ApplyPreset()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            if (textElements != null)
            {
                for (int i = 0; i < textElements.Length; i++)
                {
                    if (textElements[i] != null)
                    {
                        textElements[i].color = textColor;
                    }
                }
            }

            if (buttons != null)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    Image image = buttons[i] != null ? buttons[i].GetComponent<Image>() : null;
                    if (image != null)
                    {
                        Color color = image.color;
                        if (color.a <= 0f)
                        {
                            color.a = 0.35f;
                        }
                        image.color = color;
                        image.raycastTarget = true;
                    }
                }
            }
        }
    }
}
