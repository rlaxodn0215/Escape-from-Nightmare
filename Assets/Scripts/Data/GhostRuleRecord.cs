using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class GhostRuleRecord
    {
        public string ruleId;
        public string locationId;
        public float minArrivalTime;
        public float maxArrivalTime;
        public float minLeaveTime;
        public float maxLeaveTime;
        public float dangerIncreasePerSecond;
    }
}
