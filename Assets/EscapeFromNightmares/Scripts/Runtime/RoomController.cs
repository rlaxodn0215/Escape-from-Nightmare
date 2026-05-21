using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    public sealed class RoomController : MonoBehaviour
    {
        [SerializeField] private RoomDefinition roomDefinition;

        public RoomDefinition RoomDefinition => roomDefinition;

        public void Bind(RoomDefinition definition)
        {
            roomDefinition = definition;
            gameObject.name = string.IsNullOrWhiteSpace(definition.displayName) ? definition.roomId : definition.displayName;
        }
    }
}
