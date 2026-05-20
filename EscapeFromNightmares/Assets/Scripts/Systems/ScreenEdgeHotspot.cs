using UnityEngine;

namespace EscapeFromNightmares.Systems
{
    [RequireComponent(typeof(InteractableHotspot))]
    public sealed class ScreenEdgeHotspot : MonoBehaviour
    {
        [SerializeField] private ScreenEdge edge = ScreenEdge.Right;

        public ScreenEdge Edge => edge;
    }

    public enum ScreenEdge
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
