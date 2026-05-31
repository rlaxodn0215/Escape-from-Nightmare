// -----------------------------------------------------------------------------
// Codex comment pass: Panel Visual Preset
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\PanelVisualPreset.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Presentation controller for Panel Visual Preset UI elements, keeping references cached and visuals synchronized.
    public class PanelVisualPreset : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text[] textElements;
        [SerializeField] private Button[] buttons;
        [SerializeField] private bool applyOnAwake = true;
        [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);
        [SerializeField] private Color textColor = Color.white;

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            if (applyOnAwake)
            {
                ApplyPreset();
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            backgroundImage = GetComponent<Image>();
            textElements = GetComponentsInChildren<Text>(true);
            buttons = GetComponentsInChildren<Button>(true);
        }

        // Applies calculated settings to Unity components or runtime state.
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
