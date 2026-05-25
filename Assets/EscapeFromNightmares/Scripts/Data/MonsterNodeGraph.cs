using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Authored monster navigation graph for room-to-room pressure.
    /// </summary>
    [CreateAssetMenu(menuName = "Escape From Nightmares/Monster Node Graph")]
    public sealed class MonsterNodeGraph : ScriptableObject
    {
        public List<MonsterNode> nodes = new List<MonsterNode>();
    }

    [Serializable]
    public sealed class MonsterNode
    {
        public string nodeId;
        public string roomId;
        public string[] connectedNodeIds = Array.Empty<string>();
    }
}
