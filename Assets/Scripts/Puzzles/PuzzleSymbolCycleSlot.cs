using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    public class PuzzleSymbolCycleSlot : MonoBehaviour
    {
        [SerializeField] private Image symbolImage;
        [SerializeField] private Text labelText;
        [SerializeField] private int slotIndex;
        [SerializeField] private PuzzleSymbolCycleUIBase target;

        private Button button;
        private string currentSymbolId;
        private int currentSymbolIndex = -1;

        public int SlotIndex
        {
            get { return slotIndex; }
        }

        public string CurrentSymbolId
        {
            get { return currentSymbolId; }
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

        public void SetTarget(PuzzleSymbolCycleUIBase newTarget)
        {
            target = newTarget;
        }

        public void SetSlotIndex(int index)
        {
            slotIndex = Mathf.Max(0, index);
        }

        public void SetSymbol(string symbolId)
        {
            currentSymbolId = symbolId;
            currentSymbolIndex = target != null ? target.IndexOfAvailableSymbol(symbolId) : currentSymbolIndex;
            RefreshVisual();
        }

        public void Clear()
        {
            currentSymbolId = string.Empty;
            currentSymbolIndex = -1;
            RefreshVisual();
        }

        public void CycleNext()
        {
            string[] symbols = target != null ? target.AvailableSymbolIds : null;
            if (symbols == null || symbols.Length == 0)
            {
                symbols = new string[] { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
            }

            currentSymbolIndex++;
            if (currentSymbolIndex >= symbols.Length)
            {
                currentSymbolIndex = 0;
            }

            currentSymbolId = symbols[currentSymbolIndex];
            RefreshVisual();
        }

        public void RefreshVisual()
        {
            SymbolRecord symbol = null;
            if (GameDataManager.Instance != null && !string.IsNullOrEmpty(currentSymbolId))
            {
                symbol = GameDataManager.Instance.GetSymbolById(currentSymbolId);
            }

            Sprite sprite = null;
            if (symbol != null && !string.IsNullOrEmpty(symbol.spritePath))
            {
                sprite = Resources.Load<Sprite>(symbol.spritePath);
                if (sprite == null)
                {
                    Debug.LogWarning("Symbol sprite not found at Resources path: " + symbol.spritePath, this);
                }
            }

            if (symbolImage != null)
            {
                symbolImage.sprite = sprite;
                symbolImage.enabled = sprite != null;
            }

            if (labelText != null)
            {
                if (sprite != null)
                {
                    labelText.text = string.Empty;
                }
                else if (symbol != null && !string.IsNullOrEmpty(symbol.displayName))
                {
                    labelText.text = symbol.displayName;
                }
                else
                {
                    labelText.text = !string.IsNullOrEmpty(currentSymbolId) ? currentSymbolId : string.Empty;
                }
            }
        }

        private void HandleClick()
        {
            if (target == null)
            {
                target = GetComponentInParent<PuzzleSymbolCycleUIBase>();
            }

            if (target != null)
            {
                target.CycleSlot(this);
                return;
            }

            CycleNext();
        }

        private void CacheReferences()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzleSymbolCycleUIBase>();
            }
        }
    }
}
