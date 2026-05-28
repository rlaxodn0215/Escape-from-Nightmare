using UnityEngine;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(ClickableButton))]
    public class PickupItemController : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private bool hideWhenAlreadyOwned = true;

        private ClickableButton clickableButton;

        private void Awake()
        {
            CacheReferences();
        }

        private void Start()
        {
            RefreshVisibility();
        }

        private void Reset()
        {
            CacheReferences();
        }

        public void RefreshVisibility()
        {
            if (!hideWhenAlreadyOwned)
            {
                return;
            }

            string itemId = GetLinkedItemId();
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            bool alreadyOwned = false;
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId))
            {
                alreadyOwned = true;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsItemOwned(itemId))
            {
                alreadyOwned = true;
            }

            if (alreadyOwned)
            {
                EnsureRootObject();
                rootObject.SetActive(false);
            }
        }

        public void MarkPickedUp()
        {
            EnsureRootObject();
            rootObject.SetActive(false);
        }

        private string GetLinkedItemId()
        {
            CacheReferences();
            return clickableButton != null ? clickableButton.LinkedItemId : string.Empty;
        }

        private void CacheReferences()
        {
            EnsureRootObject();

            if (clickableButton == null)
            {
                clickableButton = GetComponent<ClickableButton>();
            }
        }

        private void EnsureRootObject()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }
        }
    }
}
