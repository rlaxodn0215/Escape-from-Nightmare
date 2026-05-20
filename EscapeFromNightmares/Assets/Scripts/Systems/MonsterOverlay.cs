using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    public sealed class MonsterOverlay : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer overlayRenderer;
        [SerializeField] private Sprite silhouetteSprite;
        [SerializeField] private Sprite nearDetectionSprite;
        [SerializeField] private Sprite chaseSprite;
        [SerializeField] private Sprite gameOverSprite;

        private void Awake()
        {
            overlayRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
            Hide();
        }

        public void ShowSilhouette()
        {
            Show(silhouetteSprite);
        }

        public void ShowNearDetection()
        {
            Show(nearDetectionSprite);
        }

        public void ShowChase()
        {
            Show(chaseSprite);
        }

        public void ShowGameOver()
        {
            Show(gameOverSprite);
        }

        public void Hide()
        {
            if (overlayRenderer == null)
            {
                return;
            }

            overlayRenderer.enabled = false;
        }

        private void Show(Sprite sprite)
        {
            if (overlayRenderer == null)
            {
                return;
            }

            overlayRenderer.sprite = sprite;
            overlayRenderer.enabled = sprite != null;
        }
    }
}
