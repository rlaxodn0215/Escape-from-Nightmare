// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Number Button
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzleNumberButton.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    [RequireComponent(typeof(Button))]
    // Puzzle controller for the Puzzle Number Button screen, translating UI input into puzzle progress and completion.
    public class PuzzleNumberButton : MonoBehaviour
    {
        [SerializeField] private int digit;
        [SerializeField] private PuzzleNumberCodeUIBase target;

        // Stores the button value used by this script's runtime or editor workflow.
        private Button button;

        public int Digit
        {
            get { return digit; }
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
        public void SetTarget(PuzzleNumberCodeUIBase newTarget)
        {
            target = newTarget;
        }

        // Performs the Handle Click operation while keeping its implementation details inside this script.
        private void HandleClick()
        {
            if (target == null)
            {
                target = GetComponentInParent<PuzzleNumberCodeUIBase>();
            }

            if (target == null)
            {
                Debug.LogWarning("PuzzleNumberButton target is missing: " + name, this);
                return;
            }

            target.AppendDigit(digit);
        }

        // Performs the Cache References operation while keeping its implementation details inside this script.
        private void CacheReferences()
        {
            digit = Mathf.Clamp(digit, 0, 9);

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (target == null)
            {
                target = GetComponentInParent<PuzzleNumberCodeUIBase>();
            }
        }
    }
}
