using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class DoorRecord
    {
        public string doorId;
        public string fromLocationId;
        public string fromViewId;
        public string toLocationId;
        public string toViewId;
        public string requiredItemId;
        public string requiredPuzzleId;
        public bool startsLocked;
        public bool staysUnlockedAfterOpen;
    }
}
