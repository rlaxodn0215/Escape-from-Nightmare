using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class RoomViewPresenter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private SpriteRenderer[] visualLayerRenderers = new SpriteRenderer[0];

        private void Awake()
        {
            backgroundRenderer ??= GetComponent<SpriteRenderer>();
        }

        public void ShowRoom(RoomDefinition roomDefinition)
        {
            if (backgroundRenderer == null || roomDefinition == null)
            {
                return;
            }

            backgroundRenderer.sprite = roomDefinition.BackgroundSprite;
            backgroundRenderer.enabled = roomDefinition.BackgroundSprite != null;
            ClearVisualLayers();
        }

        public void ClearVisualLayers()
        {
            foreach (SpriteRenderer layerRenderer in visualLayerRenderers)
            {
                if (layerRenderer == null)
                {
                    continue;
                }

                layerRenderer.sprite = null;
                layerRenderer.enabled = false;
            }
        }
    }
}
