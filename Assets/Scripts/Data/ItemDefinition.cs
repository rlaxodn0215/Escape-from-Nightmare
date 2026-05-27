namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Authored inventory item data.
    /// </summary>
    [System.Serializable]
    public sealed class ItemDefinition
    {
        public string itemId;
        public string displayName;
        public string description;
        public string iconResource;
        public ItemType itemType;
        public string altarSymbol;
    }
}
