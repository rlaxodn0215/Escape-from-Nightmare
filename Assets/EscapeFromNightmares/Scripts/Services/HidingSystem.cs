using UnityEngine;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 은신 중 마우스 움직임의 급격한 변화로 발생하는 은신 소음을 계산합니다.
    /// </summary>
    public sealed class HidingSystem
    {
        private Vector2 previousMousePosition;
        private Vector2 previousVelocity;

        /// <summary>현재 플레이어가 은신 화면에 들어와 있는지 여부입니다.</summary>
        public bool IsHiding { get; private set; }
        /// <summary>은신 중 움직임 때문에 발생한 추가 소음입니다.</summary>
        public float HideNoise { get; private set; }

        /// <summary>
        /// 은신을 시작하고 현재 마우스 위치를 기준점으로 저장합니다.
        /// </summary>
        public void Enter(Vector2 mousePosition)
        {
            IsHiding = true;
            HideNoise = 0f;
            previousMousePosition = mousePosition;
            previousVelocity = Vector2.zero;
        }

        /// <summary>은신을 종료하고 은신 소음을 제거합니다.</summary>
        public void Exit()
        {
            IsHiding = false;
            HideNoise = 0f;
        }

        /// <summary>
        /// 은신 중 마우스 가속도를 소음으로 환산하고 시간이 지나면 자연스럽게 줄입니다.
        /// </summary>
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
