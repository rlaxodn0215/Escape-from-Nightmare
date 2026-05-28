using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text labelText;
        [SerializeField] private GameObject selectedIndicator;
        [SerializeField] private GameObject emptyRoot;
        [SerializeField] private GameObject filledRoot;
        [SerializeField] private string itemId;

        private Button button;

        public string ItemId
        {
            get { return itemId; }
        }

        private void Awake()
        {
            CacheButton();
        }

        private void OnEnable()
        {
            CacheButton();

            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }

        private void Reset()
        {
            CacheButton();
        }

        public void SetItem(string newItemId)
        {
            itemId = newItemId;
            RefreshVisual();
        }

        public void Clear()
        {
            itemId = string.Empty;
            SetSelected(false);
            RefreshVisual();
        }

        public void SetSelected(bool selected)
        {
            if (selectedIndicator != null)
            {
                selectedIndicator.SetActive(selected);
            }
        }

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

        private ItemRecord GetItemRecord(string targetItemId)
        {
            if (GameDataManager.Instance == null)
            {
                return null;
            }

            return GameDataManager.Instance.GetItemById(targetItemId);
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
