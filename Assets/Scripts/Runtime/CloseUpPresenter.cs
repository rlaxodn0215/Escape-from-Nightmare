using System;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    public static class CloseUpPresenter
    {
        public static CloseUpState ResolveState(InteractableDefinition interactable, GameSession session, string studySafePuzzleId, string studySafeOpenedFlag)
        {
            if (interactable == null || session == null)
            {
                return CloseUpState.Closed;
            }

            if (!string.IsNullOrWhiteSpace(interactable.clueViewResource))
            {
                return CloseUpState.Closed;
            }

            if (interactable.puzzleId == studySafePuzzleId && session.HasFlag(studySafeOpenedFlag))
            {
                return session.HasItem(interactable.closeUpItemId) || session.HasUsedInteractable(interactable.interactableId)
                    ? CloseUpState.OpenEmpty
                    : CloseUpState.OpenWithItem;
            }

            return session.HasItem(interactable.closeUpItemId) || session.HasUsedInteractable(interactable.interactableId)
                ? CloseUpState.OpenEmpty
                : CloseUpState.Closed;
        }

        public static void ApplyState(
            InteractableDefinition interactable,
            CloseUpState state,
            Image image,
            Button actionButton,
            Button itemButton,
            RectTransform itemHitbox,
            Func<string, Sprite> resolveSprite,
            Action<Button, bool> setButtonVisible,
            Action<RectTransform, Rect> stretch)
        {
            if (interactable == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(interactable.clueViewResource))
            {
                ApplySprite(image, resolveSprite, interactable.clueViewResource);
                setButtonVisible?.Invoke(actionButton, false);
                setButtonVisible?.Invoke(itemButton, false);
                return;
            }

            ApplySprite(image, resolveSprite, ResolveSpriteResource(interactable, state));
            setButtonVisible?.Invoke(actionButton, state == CloseUpState.Closed);
            setButtonVisible?.Invoke(itemButton, state == CloseUpState.OpenWithItem);
            if (itemHitbox != null)
            {
                stretch?.Invoke(itemHitbox, interactable.closeUpItemHitbox);
            }
        }

        public static string ResolveSpriteResource(InteractableDefinition interactable, CloseUpState state)
        {
            if (interactable == null)
            {
                return null;
            }

            switch (state)
            {
                case CloseUpState.OpenWithItem:
                    return interactable.closeUpOpenWithItemResource;
                case CloseUpState.OpenEmpty:
                    return interactable.closeUpOpenEmptyResource;
                default:
                    return interactable.closeUpClosedResource;
            }
        }

        private static void ApplySprite(Image image, Func<string, Sprite> resolveSprite, string resource)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = resolveSprite != null ? resolveSprite(resource) : null;
            image.color = Color.white;
            image.preserveAspect = true;
        }
    }
}
