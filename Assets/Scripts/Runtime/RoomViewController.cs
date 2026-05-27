using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Renders the active room face and its interactable hitboxes.
    /// </summary>
    public sealed class RoomViewController
    {
        private readonly Image roomFaceImage;
        private readonly RectTransform roomObjectLayer;
        private readonly System.Func<string, Sprite> resolveSprite;
        private readonly System.Action<InteractableDefinition> onInteractable;
        private readonly MonsterViewController monsterView;

        public RoomViewController(
            Image roomFaceImage,
            RectTransform roomObjectLayer,
            System.Func<string, Sprite> resolveSprite,
            System.Action<InteractableDefinition> onInteractable,
            MonsterViewController monsterView)
        {
            this.roomFaceImage = roomFaceImage;
            this.roomObjectLayer = roomObjectLayer;
            this.resolveSprite = resolveSprite;
            this.onInteractable = onInteractable;
            this.monsterView = monsterView;
        }

        public void Render(RoomDefinition room, RoomFaceDefinition face, GameSession session, FlagService flags, MonsterAIController monsterAI)
        {
            if (room == null || face == null)
            {
                return;
            }

            if (roomFaceImage != null)
            {
                roomFaceImage.sprite = resolveSprite == null ? null : resolveSprite(RoomPresenter.ResolveRoomFaceBackgroundResource(face, flags));
                roomFaceImage.color = Color.white;
                roomFaceImage.preserveAspect = true;
            }

            RenderObjects(face.interactables ?? System.Array.Empty<InteractableDefinition>(), session, flags);
            monsterView?.Render(session, monsterAI);
        }

        private void RenderObjects(InteractableDefinition[] interactables, GameSession session, FlagService flags)
        {
            if (roomObjectLayer == null)
            {
                return;
            }

            FallbackUiBuilder.ClearChildrenExcept(roomObjectLayer, monsterView == null ? null : monsterView.ImageTransform);
            foreach (var interactable in interactables)
            {
                if (!RoomPresenter.ShouldRenderRoomHitbox(interactable, session, flags)
                    || string.IsNullOrWhiteSpace(interactable.imageResource))
                {
                    continue;
                }

                var captured = interactable;
                var button = interactable.showWorldImage
                    ? FallbackUiBuilder.CreateImageButton(interactable.interactableId, roomObjectLayer, resolveSprite == null ? null : resolveSprite(interactable.imageResource), () => onInteractable?.Invoke(captured))
                    : FallbackUiBuilder.CreateTransparentButton(interactable.interactableId, roomObjectLayer, () => onInteractable?.Invoke(captured));
                FallbackUiBuilder.Stretch(button.GetComponent<RectTransform>(), interactable.normalizedHitbox);
            }
        }
    }
}
