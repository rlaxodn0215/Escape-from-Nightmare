using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Monster Node Definition")]
    public sealed class MonsterNodeDefinition : ScriptableObject
    {
        [SerializeField] private string nodeId;
        [SerializeField] private string roomId;
        [SerializeField] private string[] connectedNodeIds = new string[0];
        [SerializeField] private float searchWeight = 1f;
        [SerializeField] private float chaseWeight = 1f;
        [SerializeField] private bool canCheckHideSpot = true;

        public string NodeId => nodeId;
        public string RoomId => roomId;
        public string[] ConnectedNodeIds => connectedNodeIds;
        public float SearchWeight => searchWeight;
        public float ChaseWeight => chaseWeight;
        public bool CanCheckHideSpot => canCheckHideSpot;
    }
}
