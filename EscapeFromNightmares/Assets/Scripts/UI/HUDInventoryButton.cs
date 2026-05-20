using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class HUDInventoryButton : MonoBehaviour
    {
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private Button button;

        private void Awake()
        {
            button ??= GetComponent<Button>();
            inventoryUI ??= FindFirstObjectByType<InventoryUI>(FindObjectsInactive.Include);

            if (button != null)
            {
                button.onClick.AddListener(OpenInventory);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OpenInventory);
            }
        }

        public void Bind(InventoryUI nextInventoryUI)
        {
            inventoryUI = nextInventoryUI;
        }

        public void OpenInventory()
        {
            inventoryUI?.Show();
        }
    }
}
