using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class HUDSettingsButton : MonoBehaviour
    {
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private Button button;

        private void Awake()
        {
            button ??= GetComponent<Button>();
            settingsUI ??= FindFirstObjectByType<SettingsUI>(FindObjectsInactive.Include);

            if (button != null)
            {
                button.onClick.AddListener(OpenSettings);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OpenSettings);
            }
        }

        public void Bind(SettingsUI nextSettingsUI)
        {
            settingsUI = nextSettingsUI;
        }

        public void OpenSettings()
        {
            settingsUI?.Show();
        }
    }
}
