using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class ClueRecord
    {
        public string clueId;
        public string locationId;
        public string imagePath;
        public string requiredPuzzleId;
        public string requiredItemId;
        public bool startsUnlocked;
        public string displayName;
        public string description;
        public string lockedMessage;
    }
}
