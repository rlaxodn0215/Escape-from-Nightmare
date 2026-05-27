using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Owns the monster shadow image lifecycle and placement inside the room object layer.
    /// </summary>
    public sealed class MonsterViewController
    {
        private readonly RectTransform roomObjectLayer;
        private readonly MonsterPlacementCatalog monsterPlacementCatalog;
        private readonly System.Func<string, Sprite> resolveSprite;
        private readonly System.Action<Image> setMonsterImage;
        private Image monsterImage;

        public MonsterViewController(
            RectTransform roomObjectLayer,
            Image monsterImage,
            MonsterPlacementCatalog monsterPlacementCatalog,
            System.Func<string, Sprite> resolveSprite,
            System.Action<Image> setMonsterImage)
        {
            this.roomObjectLayer = roomObjectLayer;
            this.monsterImage = monsterImage;
            this.monsterPlacementCatalog = monsterPlacementCatalog;
            this.resolveSprite = resolveSprite;
            this.setMonsterImage = setMonsterImage;
        }

        public Transform ImageTransform => monsterImage == null ? null : monsterImage.transform;

        public bool ImageActive => monsterImage != null && monsterImage.gameObject.activeSelf;

        public void Render(GameSession session, MonsterAIController monsterAI)
        {
            if (session == null || monsterAI == null || string.IsNullOrWhiteSpace(session.CurrentRoomId))
            {
                Hide();
                return;
            }

            if (!MonsterPresenter.TryResolvePlacement(monsterPlacementCatalog, session.CurrentRoomId, session.CurrentFaceDirection, monsterAI.State, out var placementRect))
            {
                Hide();
                return;
            }

            var image = EnsureImage();
            if (image == null)
            {
                return;
            }

            image.sprite = resolveSprite == null ? null : resolveSprite(GameDirector.MonsterShadowResource);
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            MonsterPresenter.ApplyPlacement(image.rectTransform, placementRect);
            image.transform.SetAsLastSibling();
            image.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (monsterImage != null)
            {
                monsterImage.gameObject.SetActive(false);
            }
        }

        private Image EnsureImage()
        {
            if (monsterImage != null)
            {
                return monsterImage;
            }

            if (roomObjectLayer == null)
            {
                return null;
            }

            monsterImage = FallbackUiBuilder.CreateImage("MonsterImage", roomObjectLayer, Color.white);
            monsterImage.color = Color.white;
            monsterImage.raycastTarget = false;
            monsterImage.preserveAspect = true;
            monsterImage.gameObject.SetActive(false);
            setMonsterImage?.Invoke(monsterImage);
            return monsterImage;
        }
    }
}
