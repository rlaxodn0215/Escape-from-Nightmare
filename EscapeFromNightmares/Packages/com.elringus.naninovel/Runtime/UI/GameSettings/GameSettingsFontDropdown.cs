using System.Collections.Generic;
using System.Linq;

namespace Naninovel.UI
{
    public class GameSettingsFontDropdown : ScriptableDropdown
    {
        [ManagedText("DefaultUI")]
        protected static string DefaultFontName = "Default";

        private IUIManager uis;
        private ILocalizationManager l10n;
        private ICommunityLocalization communityL10n;

        protected override void Awake ()
        {
            base.Awake();

            uis = Engine.GetServiceOrErr<IUIManager>();
            l10n = Engine.GetServiceOrErr<ILocalizationManager>();
            communityL10n = Engine.GetServiceOrErr<ICommunityLocalization>();
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            InitializeOptions();

            if (Engine.TryGetService<ILocalizationManager>(out var l10n))
                l10n.OnLocaleChanged += HandleLocaleChanged;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            if (Engine.TryGetService<ILocalizationManager>(out var l10n))
                l10n.OnLocaleChanged -= HandleLocaleChanged;
        }

        protected override void OnValueChanged (int value)
        {
            uis.FontName = value == 0 ? null : UIComponent.options[value].text;
        }

        protected virtual void InitializeOptions ()
        {
            var availableOptions = new List<string> { DefaultFontName };
            if (!communityL10n.Active && l10n.IsSourceLocaleSelected() && uis.Configuration.FontOptions?.Count > 0)
            {
                availableOptions.AddRange(uis.Configuration.FontOptions
                    .Where(o => string.IsNullOrWhiteSpace(o.ApplyOnLocale) || o.ApplyOnLocale == l10n.SelectedLocale)
                    .Select(o => o.FontName));
                if (availableOptions.Count <= 1) transform.parent.gameObject.SetActive(false);
            }
            else transform.parent.gameObject.SetActive(false);

            UIComponent.ClearOptions();
            UIComponent.AddOptions(availableOptions);
            UIComponent.value = GetCurrentIndex();
            UIComponent.RefreshShownValue();
        }

        protected virtual int GetCurrentIndex ()
        {
            if (string.IsNullOrEmpty(uis.FontName)) return 0;
            var option = UIComponent.options.Find(o => o.text == uis.FontName);
            return UIComponent.options.IndexOf(option);
        }

        protected virtual void HandleLocaleChanged (LocaleChangedArgs _) => InitializeOptions();
    }
}
