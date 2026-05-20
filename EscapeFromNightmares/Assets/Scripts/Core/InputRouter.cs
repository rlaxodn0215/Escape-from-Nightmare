using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeFromNightmares.Core
{
    public sealed class InputRouter : MonoBehaviour
    {
        public event Action<Vector2> PrimaryClick;

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
            {
                return;
            }

            PrimaryClick?.Invoke(mouse.position.ReadValue());
        }
    }
}
