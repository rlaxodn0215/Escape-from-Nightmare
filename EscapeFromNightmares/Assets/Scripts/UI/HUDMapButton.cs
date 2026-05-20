using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class HUDMapButton : MonoBehaviour
    {
        [SerializeField] private MapUI mapUI;
        [SerializeField] private Button button;

        private void Awake()
        {
            button ??= GetComponent<Button>();
            mapUI ??= FindFirstObjectByType<MapUI>(FindObjectsInactive.Include);

            if (button != null)
            {
                button.onClick.AddListener(OpenMap);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OpenMap);
            }
        }

        public void Bind(MapUI nextMapUI)
        {
            mapUI = nextMapUI;
        }

        public void OpenMap()
        {
            mapUI?.Show();
        }
    }
}
