using System;

namespace EscapeFromNightmare
{
    [Serializable]
    public class LocationRecord
    {
        public string locationId;
        public string displayName;
        public string defaultViewId;
        public string[] viewIds;
        public bool isConnectorLocation;
    }
}
