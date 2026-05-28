using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class PuzzleSequenceOptionButton : MonoBehaviour
    {
        [SerializeField] private string optionId;
        [SerializeField] private Text labelText;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private PuzzleSequenceUIBase target;

        private Button button;

        public string OptionId
        {
            get { return optionId; }
        }

        private void Awake()
        {
            CacheReferences();
        }

        private void OnEnable()
        {
            CacheReferences();

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void Reset()
        {
            CacheReferences();
        }

        public void SetTarget(PuzzleSequenceUIBase newTarget)
        {
            target = newTarget;
        }

        public void SetOptionId(string newOptionId)
        {
            optionId = newOptionId;
            RefreshVisualFromSymbolRecord();
        }

        public void SetSelected(bool selected)
        {
            if (selectedRoot != null)
            {
                selectedRoot.SetActive(selected);
            }
        }

        public void SetInteractable(bool interactable)
        {
            CacheReferences();

            if (button != null)
            {
                button.interactable = interactable;
            }
        }

        public void RefreshVisualFromSymbolRecord()
        {
            if (GameDataManager.Instance == null || string.IsNullOrEmpty(optionId))
            {
                return;
            }

            SymbolRecord symbol = GameDataManager.Instance.GetSymbolById(optionId);
            if (symbol == null)
            {
                return;
            }

            if (labelText != null)
            {
                labelText.text = !string.IsNullOrEmpty(symbol.displayName) ? symbol.displayName : optionId;
            }

            if (iconImage != null)
            {
                iconImage.sprite = null;

                if (!string.IsNullOrEmpty(symbol.spritePath))
                {
                    Sprite sprite = Resources.Load<Sprite>(symbol.spritePath);
                    if (sprite == null)
                    {
                        Debug.LogWarning("Symbol sprite not found at Resources path: " + symbol.spritePath);
                    }

                    iconImage.sprite = sprite;
                }

                iconImage.enabled = iconImage.sprite != null;
            }
        }

        private void HandleClick()
        {
            if (string.IsNullOrEmpty(optionId))
            {
                Debug.LogWarning("PuzzleSequenceOptionButton optionId is empty: " + name, this);
                return;
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzleSequenceUIBase>();
            }

            if (target == null)
            {
                Debug.LogWarning("PuzzleSequenceOptionButton target is missing: " + name, this);
                return;
            }

            target.SelectOption(optionId);
        }

        private void CacheReferences()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzleSequenceUIBase>();
            }
        }
    }
}
