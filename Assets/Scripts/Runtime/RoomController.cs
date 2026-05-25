using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// 씬 오브젝트와 RoomDefinition 데이터를 연결해 에디터/런타임에서 방 식별을 쉽게 합니다.
    /// </summary>
    public sealed class RoomController : MonoBehaviour
    {
        [SerializeField] private RoomDefinition roomDefinition;

        /// <summary>이 오브젝트가 대표하는 방 데이터입니다.</summary>
        public RoomDefinition RoomDefinition => roomDefinition;

        /// <summary>
        /// 지정된 방 데이터를 연결하고, Hierarchy에서 알아보기 쉽도록 오브젝트 이름을 갱신합니다.
        /// </summary>
        public void Bind(RoomDefinition definition)
        {
            roomDefinition = definition;
            gameObject.name = string.IsNullOrWhiteSpace(definition.displayName) ? definition.roomId : definition.displayName;
        }
    }
}
