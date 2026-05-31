using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class InventoryToggleButtonUI : MonoBehaviour
    {
        [SerializeField] private InventoryBarUI inventoryBar;

        private Button button;

        private void Awake()
        {
            CacheButton();
        }

        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(ToggleInventory);
                button.onClick.AddListener(ToggleInventory);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(ToggleInventory);
            }
        }

        public void SetInventoryBar(InventoryBarUI target)
        {
            inventoryBar = target;
        }

        public void ToggleInventory()
        {
            if (inventoryBar != null)
            {
                inventoryBar.ToggleVisibility();
            }
        }

        private void CacheButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }
    }
}
