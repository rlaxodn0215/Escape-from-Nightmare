using UnityEngine;

namespace EscapeFromNightmares.Services
{
    public sealed class HidingSystem
    {
        private Vector2 previousMousePosition;
        private Vector2 previousVelocity;

        public bool IsHiding { get; private set; }
        public float HideNoise { get; private set; }

        public void Enter(Vector2 mousePosition)
        {
            IsHiding = true;
            HideNoise = 0f;
            previousMousePosition = mousePosition;
            previousVelocity = Vector2.zero;
        }

        public void Exit()
        {
            IsHiding = false;
            HideNoise = 0f;
        }

        public void Tick(float deltaTime, Vector2 mousePosition)
        {
            if (!IsHiding || deltaTime <= 0f)
            {
                return;
            }

            var velocity = (mousePosition - previousMousePosition) / deltaTime;
            var acceleration = (velocity - previousVelocity).magnitude;
            HideNoise = Mathf.Clamp(HideNoise + acceleration * 0.0008f - deltaTime * 8f, 0f, 100f);
            previousMousePosition = mousePosition;
            previousVelocity = velocity;
        }
    }
}
