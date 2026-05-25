using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Authored inventory item data.
    /// </summary>
    [CreateAssetMenu(menuName = "Escape From Nightmares/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public string description;
        public string iconResource;
        public ItemType itemType;
        public string altarSymbol;
    }
}
