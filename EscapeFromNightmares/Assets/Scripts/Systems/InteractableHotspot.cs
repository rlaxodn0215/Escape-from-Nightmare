using EscapeFromNightmares.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EscapeFromNightmares.Systems
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(Image))]
    public sealed class InteractableHotspot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private InteractableDefinition definition;
        [SerializeField] private InteractionSystem interactionSystem;
        [SerializeField] private Image raycastImage;

        public InteractableDefinition Definition => definition;

        private void Awake()
        {
            interactionSystem ??= FindFirstObjectByType<InteractionSystem>();
            raycastImage ??= GetComponent<Image>();
            ConfigureRaycastImage();
        }

        public void Configure(InteractableDefinition nextDefinition, InteractionSystem nextInteractionSystem)
        {
            definition = nextDefinition;
            interactionSystem = nextInteractionSystem;

            if (definition != null)
            {
                ApplyHitArea(definition.HitArea);
            }
        }

        public void ApplyHitArea(Rect hitArea)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(hitArea.width, hitArea.height);
            rectTransform.anchoredPosition = new Vector2(hitArea.x + hitArea.width * 0.5f - 640f, 360f - hitArea.y - hitArea.height * 0.5f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            interactionSystem?.HandleInteractableClicked(definition);
        }

        private void ConfigureRaycastImage()
        {
            if (raycastImage == null)
            {
                return;
            }

            raycastImage.color = new Color(0f, 0f, 0f, 0f);
            raycastImage.raycastTarget = true;
        }
    }
}
