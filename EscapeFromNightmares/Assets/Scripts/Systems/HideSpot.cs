using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    [RequireComponent(typeof(InteractableHotspot))]
    public sealed class HideSpot : MonoBehaviour
    {
        [SerializeField] private string hideSpotId = "hide_spot";
        [SerializeField, Min(0f)] private float recommendedHoldSeconds = 6f;

        public string HideSpotId => hideSpotId;
        public float RecommendedHoldSeconds => recommendedHoldSeconds;
    }
}
