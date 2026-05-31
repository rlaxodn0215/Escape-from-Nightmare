// -----------------------------------------------------------------------------
// Codex comment pass: Location Record
// Role: Stores serializable records that bridge JSON data, Unity serialization, and runtime lookup code.
// Scope: This script belongs to Data\LocationRecord.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;

namespace EscapeFromNightmare
{
    [Serializable]
    // Serializable Location Record data container whose field names intentionally match the project data files.
    public class LocationRecord
    {
        // Stores the location Id value used by this script's runtime or editor workflow.
        public string locationId;
        // Stores the display Name value used by this script's runtime or editor workflow.
        public string displayName;
        // Stores the default View Id value used by this script's runtime or editor workflow.
        public string defaultViewId;
        // Stores the view Ids value used by this script's runtime or editor workflow.
        public string[] viewIds;
        // Stores the is Connector Location value used by this script's runtime or editor workflow.
        public bool isConnectorLocation;
    }
}
