using System;
using EscapeFromNightmares.Data;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    public static class HideViewPresenter
    {
        public static bool CanOpen(InteractableDefinition interactable)
        {
            return interactable != null && !string.IsNullOrWhiteSpace(interactable.hideViewResource);
        }

        public static void Apply(InteractableDefinition interactable, Image image, Func<string, Sprite> resolveSprite)
        {
            if (!CanOpen(interactable) || image == null)
            {
                return;
            }

            image.sprite = resolveSprite != null ? resolveSprite(interactable.hideViewResource) : null;
            image.color = Color.white;
            image.preserveAspect = true;
        }
    }
}
