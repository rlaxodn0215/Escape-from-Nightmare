// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Sequence Option Button
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleSequenceOptionButton.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Puzzle controller for the Puzzle Sequence Option Button screen, translating UI input into puzzle progress and completion.
    public class PuzzleSequenceOptionButton : MonoBehaviour
    {
        [SerializeField] private string optionId;
        [SerializeField] private Text labelText;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private PuzzleSequenceUIBase target;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;

        public string OptionId
        {
            get { return optionId; }
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
        public void SetTarget(PuzzleSequenceUIBase newTarget)
        {
            target = newTarget;
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetOptionId(string newOptionId)
        {
            optionId = newOptionId;
            RefreshVisualFromSymbolRecord();
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetSelected(bool selected)
        {
            if (selectedRoot != null)
            {
                selectedRoot.SetActive(selected);
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetInteractable(bool interactable)
        {
            CacheReferences();

            if (button != null)
            {
                button.interactable = interactable;
            }
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
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

        // Performs the Handle Click operation while keeping its implementation details inside this script.
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

        // Performs the Cache References operation while keeping its implementation details inside this script.
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
