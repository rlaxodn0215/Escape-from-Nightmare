using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Data/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemCategory category;
        [SerializeField] private string acquiredFrom;
        [SerializeField] private string[] usedIn = new string[0];
        [SerializeField] private string altarSymbol;
        [SerializeField] private string[] flagsOnAcquire = new string[0];

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public ItemCategory Category => category;
        public string AcquiredFrom => acquiredFrom;
        public string[] UsedIn => usedIn;
        public string AltarSymbol => altarSymbol;
        public string[] FlagsOnAcquire => flagsOnAcquire;
    }
}
