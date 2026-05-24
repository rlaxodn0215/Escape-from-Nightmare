using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Resource Path Catalog")]
    public sealed class ResourcePathCatalog : ScriptableObject
    {
        [Header("Title")]
        public string titleBackgroundPath = "EscapeFromNightmares/Title/title_background";
        public string titleLogoPath = "EscapeFromNightmares/Title/title_logo_escape_from_nightmare";
        public string titleStartButtonPath = "EscapeFromNightmares/Title/UI/button_start";
        public string titleSettingsButtonPath = "EscapeFromNightmares/Title/UI/button_settings";
        public string titleQuitButtonPath = "EscapeFromNightmares/Title/UI/button_quit";
        public string titleCloseButtonPath = "EscapeFromNightmares/Title/UI/button_close";
        public string settingsPanelBackgroundPath = "EscapeFromNightmares/Title/UI/settings_panel_bg";
        public string settingsHeaderPath = "EscapeFromNightmares/Title/UI/settings_header";
        public string settingsMasterLabelPath = "EscapeFromNightmares/Title/UI/settings_label_master";
        public string settingsBgmLabelPath = "EscapeFromNightmares/Title/UI/settings_label_bgm";
        public string settingsSfxLabelPath = "EscapeFromNightmares/Title/UI/settings_label_sfx";
        public string settingsUiLabelPath = "EscapeFromNightmares/Title/UI/settings_label_ui";
        public string settingsSliderTrackPath = "EscapeFromNightmares/Title/UI/slider_track";
        public string settingsSliderFillPath = "EscapeFromNightmares/Title/UI/slider_fill";
        public string settingsSliderHandlePath = "EscapeFromNightmares/Title/UI/slider_handle";
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
