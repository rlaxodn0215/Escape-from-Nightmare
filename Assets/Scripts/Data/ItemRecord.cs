using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class ItemRecord
    {
        public string itemId;
        public string displayName;
        public string description;
        public string iconPath;
        public bool consumable;
    }
}
