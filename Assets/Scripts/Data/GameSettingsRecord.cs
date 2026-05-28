using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class GameSettingsRecord
    {
        public int referenceWidth;
        public int referenceHeight;
        public float hideMinSeconds;
        public float hideMaxSeconds;
        public string titleSceneName;
        public string gameSceneName;

        public float ghostDefaultMinArrivalTime;
        public float ghostDefaultMaxArrivalTime;
        public float ghostDefaultMinLeaveTime;
        public float ghostDefaultMaxLeaveTime;
        public float ghostDangerThreshold;
        public int chaseMoveLimit;
    }
}
