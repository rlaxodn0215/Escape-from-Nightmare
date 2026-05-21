using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Resource Path Catalog")]
    public sealed class ResourcePathCatalog : ScriptableObject
    {
        [Header("Title")]
        public string titleBackgroundPath = "EscapeFromNightmares/Title/title_background";
        public string titleBgmPath = "EscapeFromNightmares/Audio/BGM/title_loop";

        [Header("UI Audio")]
        public string uiClickPath = "EscapeFromNightmares/Audio/UI/ui_click";
        public string confirmSfxPath = "EscapeFromNightmares/Audio/SFX/sfx_confirm";

        public static ResourcePathCatalog CreateDefault()
        {
            return CreateInstance<ResourcePathCatalog>();
        }
    }
}
