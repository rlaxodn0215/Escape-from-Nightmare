// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Symbol Cycle Slot
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleSymbolCycleSlot.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Puzzle controller for the Puzzle Symbol Cycle Slot screen, translating UI input into puzzle progress and completion.
    public class PuzzleSymbolCycleSlot : MonoBehaviour
    {
        [SerializeField] private Image symbolImage;
        [SerializeField] private Text labelText;
        [SerializeField] private int slotIndex;
        [SerializeField] private PuzzleSymbolCycleUIBase target;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;
        // Stores the current Symbol Id value used by this script's runtime or editor workflow.
        private string currentSymbolId;
        // Stores the current Symbol Index value used by this script's runtime or editor workflow.
        private int currentSymbolIndex = -1;

        public int SlotIndex
        {
            get { return slotIndex; }
        }

        public string CurrentSymbolId
        {
            get { return currentSymbolId; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        private void Awake()
        {
            CacheReferences();
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        private void OnEnable()
        {
            CacheReferences();

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
                button.onClick.AddListener(HandleClick);
            }
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        // Provides safe default Inspector values when the component is first attached.
        private void Reset()
        {
            CacheReferences();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetTarget(PuzzleSymbolCycleUIBase newTarget)
        {
            target = newTarget;
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetSlotIndex(int index)
        {
            slotIndex = Mathf.Max(0, index);
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetSymbol(string symbolId)
        {
            currentSymbolId = symbolId;
            currentSymbolIndex = target != null ? target.IndexOfAvailableSymbol(symbolId) : currentSymbolIndex;
            RefreshVisual();
        }

        // Performs the Clear operation while keeping its implementation details inside this script.
        public void Clear()
        {
            currentSymbolId = string.Empty;
            currentSymbolIndex = -1;
            RefreshVisual();
        }

        // Performs the Cycle Next operation while keeping its implementation details inside this script.
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

        // Re-reads current game data and manager state, then redraws the visible UI.
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

        // Performs the Handle Click operation while keeping its implementation details inside this script.
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

        // Performs the Cache References operation while keeping its implementation details inside this script.
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
