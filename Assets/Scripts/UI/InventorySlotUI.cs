// -----------------------------------------------------------------------------
// Codex comment pass: Inventory Slot UI
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\InventorySlotUI.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Presentation controller for Inventory Slot UI UI elements, keeping references cached and visuals synchronized.
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text labelText;
        [SerializeField] private GameObject selectedIndicator;
        [SerializeField] private GameObject emptyRoot;
        [SerializeField] private GameObject filledRoot;
        [SerializeField] private string itemId;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;

        public string ItemId
        {
            get { return itemId; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheButton();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
                button.onClick.AddListener(OnClick);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheButton();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetItem(string newItemId)
        {
            itemId = newItemId;
            RefreshVisual();
        }

        // Performs the Clear operation while keeping its implementation details inside this script.
        public void Clear()
        {
            itemId = string.Empty;
            SetSelected(false);
            RefreshVisual();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetSelected(bool selected)
        {
            if (selectedIndicator != null)
            {
                selectedIndicator.SetActive(selected);
            }
        }

        // Performs the On Click operation while keeping its implementation details inside this script.
        public void OnClick()
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return;
            }

            InventoryManager.Instance.SelectItem(itemId);
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        private void RefreshVisual()
        {
            bool hasItem = !string.IsNullOrEmpty(itemId);

            if (emptyRoot != null)
            {
                emptyRoot.SetActive(!hasItem);
            }

            if (filledRoot != null)
            {
                filledRoot.SetActive(hasItem);
            }

            if (iconImage != null)
            {
                iconImage.sprite = hasItem ? LoadIconSprite(itemId) : null;
                iconImage.enabled = hasItem && iconImage.sprite != null;
            }

            if (labelText != null)
            {
                if (!hasItem)
                {
                    labelText.text = string.Empty;
                }
                else
                {
                    ItemRecord itemRecord = GetItemRecord(itemId);
                    labelText.text = itemRecord != null && !string.IsNullOrEmpty(itemRecord.displayName) ? itemRecord.displayName : itemId;
                }
            }
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        private Sprite LoadIconSprite(string targetItemId)
        {
            ItemRecord itemRecord = GetItemRecord(targetItemId);
            if (itemRecord == null || string.IsNullOrEmpty(itemRecord.iconPath))
            {
                return null;
            }

            Sprite sprite = Resources.Load<Sprite>(itemRecord.iconPath);
            if (sprite == null)
            {
                Debug.LogWarning("Item icon sprite not found at Resources path: " + itemRecord.iconPath);
            }

            return sprite;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private ItemRecord GetItemRecord(string targetItemId)
        {
            if (GameDataManager.Instance == null)
            {
                return null;
            }

            return GameDataManager.Instance.GetItemById(targetItemId);
        }

        // Performs the Cache Button operation while keeping its implementation details inside this script.
        private void CacheButton()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }
    }
}
