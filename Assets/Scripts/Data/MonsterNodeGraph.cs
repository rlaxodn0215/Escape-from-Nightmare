using System;
using System.Collections.Generic;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Authored monster navigation graph for room-to-room pressure.
    /// </summary>
    [Serializable]
    public sealed class MonsterNodeGraph
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
