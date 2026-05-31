using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class InteractionInputGate : MonoBehaviour
    {
        private readonly Dictionary<Button, bool> defaultInteractableByButton = new Dictionary<Button, bool>();
        private readonly List<Button> knownButtons = new List<Button>();

        private void OnEnable()
        {
            RefreshKnownButtons();
            ApplyCurrentPolicy();
        }

        private void Update()
        {
            RefreshKnownButtons();
            ApplyCurrentPolicy();
        }

        private void RefreshKnownButtons()
        {
            Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Button button = buttons[i];
                if (button == null || !IsSceneObject(button.gameObject))
                {
                    continue;
                }

                if (!defaultInteractableByButton.ContainsKey(button))
                {
                    defaultInteractableByButton.Add(button, button.interactable);
                    knownButtons.Add(button);
                }
            }
        }

        private void ApplyCurrentPolicy()
        {
            bool transitioning = ScreenFadeManager.Instance != null && ScreenFadeManager.Instance.IsTransitioning;
            bool hiding = IsHiding();

            for (int i = knownButtons.Count - 1; i >= 0; i--)
            {
                Button button = knownButtons[i];
                if (button == null)
                {
                    knownButtons.RemoveAt(i);
                    continue;
                }

                bool defaultInteractable = true;
                defaultInteractableByButton.TryGetValue(button, out defaultInteractable);

                if (transitioning)
                {
                    button.interactable = false;
                }
                else if (hiding)
                {
                    button.interactable = defaultInteractable && IsAllowedWhileHiding(button);
                }
                else
                {
                    button.interactable = defaultInteractable;
                }
            }
        }

        private bool IsHiding()
        {
            if (HideManager.Instance != null && HideManager.Instance.IsHiding)
            {
                return true;
            }

            return GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Hiding;
        }

        private bool IsAllowedWhileHiding(Button button)
        {
            if (button == null)
            {
                return false;
            }

            return button.GetComponent<HideExitButton>() != null
                || button.GetComponent<InventoryToggleButtonUI>() != null
                || button.GetComponent<InventorySlotUI>() != null;
        }

        private bool IsSceneObject(GameObject target)
        {
            return target != null && target.scene.IsValid();
        }
    }
}
