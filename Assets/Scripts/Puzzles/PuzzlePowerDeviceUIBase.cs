// -----------------------------------------------------------------------------
// Codex comment pass: Puzzle Power Device UI Base
// Role: Controls puzzle UI input, answer validation, retry behavior, and reward handoff to PuzzleManager.
// Scope: This script belongs to Puzzles\PuzzlePowerDeviceUIBase.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    // Puzzle controller for the Puzzle Power Device UI Base screen, translating UI input into puzzle progress and completion.
    public class PuzzlePowerDeviceUIBase : PuzzleUIBase
    {
        [SerializeField] protected Text inputText;
        [SerializeField] protected Text messageText;
        [SerializeField] protected Transform switchButtonRoot;
        [SerializeField] protected List<PuzzlePowerSwitchButton> switchButtons = new List<PuzzlePowerSwitchButton>();
        [SerializeField] protected Button powerButton;
        [SerializeField] protected Button resetButton;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected bool autoCollectSwitchButtons = true;
        [SerializeField] protected int requiredInputLength = 5;
        [SerializeField] protected string requiredSecondItemId = "SmallClockworkDevice";
        [SerializeField] protected string transformedItemId = "ModifiedClockworkDevice";
        [SerializeField] protected string unlockDoorId = "Door_BasementStorage_LockedRoom";
        [SerializeField] protected string unlockClueId = "BasementClueImage";

        // Stores the current Inputs value used by this script's runtime or editor workflow.
        protected readonly List<string> currentInputs = new List<string>();
        // Stores the expected Pattern value used by this script's runtime or editor workflow.
        protected string[] expectedPattern;

        public IReadOnlyList<string> CurrentInputs
        {
            get { return currentInputs; }
        }

        public string[] ExpectedPattern
        {
            get { return expectedPattern; }
        }

        // Caches required component references and prepares this object before other startup code runs.
        protected virtual void Awake()
        {
            if (autoCollectSwitchButtons)
            {
                CacheSwitchButtons();
            }
        }

        // Reconnects event subscriptions and visible state whenever this object becomes active.
        protected virtual void OnEnable()
        {
            HookButtons();
            RefreshDisplay();
        }

        // Disconnects event subscriptions so inactive objects do not receive duplicate callbacks.
        protected virtual void OnDisable()
        {
            UnhookButtons();
        }

        // Initializes local UI and state from an external record before the player can interact with it.
        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            ResolveAnswer();
            ResetInput();
            SetMessage(string.Empty);
        }

        // Performs the Input Switch operation while keeping its implementation details inside this script.
        public void InputSwitch(string switchId)
        {
            if (string.IsNullOrEmpty(switchId))
            {
                Debug.LogWarning("Cannot input empty switchId.", this);
                return;
            }

            if (currentInputs.Count >= requiredInputLength)
            {
                return;
            }

            currentInputs.Add(switchId);
            RefreshDisplay();
        }

        // Performs the Press Power Button operation while keeping its implementation details inside this script.
        public void PressPowerButton()
        {
            if (!HasRequiredItems())
            {
                SetMessage("Missing item.");
                return;
            }

            if (currentInputs.Count < requiredInputLength)
            {
                SetMessage("Not enough inputs.");
                return;
            }

            if (IsCorrectPattern())
            {
                ApplyPowerDeviceReward();
                SetMessage("Correct.");
                Complete();
                return;
            }

            SetMessage("Wrong.");
            RegisterFailure();
            ResetInput();
        }

        // Returns runtime state to its defaults for a new game, retry, or clean test run.
        public void ResetInput()
        {
            currentInputs.Clear();
            RefreshDisplay();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        protected virtual bool HasRequiredItems()
        {
            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return false;
            }

            if (puzzleRecord != null && !string.IsNullOrEmpty(puzzleRecord.requiredItemId) && !InventoryManager.Instance.HasItem(puzzleRecord.requiredItemId))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(requiredSecondItemId) && !InventoryManager.Instance.HasItem(requiredSecondItemId))
            {
                return false;
            }

            return true;
        }

        // Performs the Resolve Answer operation while keeping its implementation details inside this script.
        protected virtual void ResolveAnswer()
        {
            expectedPattern = null;

            if (GameDataManager.Instance != null && puzzleRecord != null)
            {
                expectedPattern = GameDataManager.Instance.GetAnswerSequence(puzzleRecord);
            }

            if (expectedPattern == null)
            {
                expectedPattern = new string[0];
            }

            if (expectedPattern.Length > 0)
            {
                requiredInputLength = expectedPattern.Length;
            }
            else
            {
                Debug.LogWarning("Power device expected pattern is empty for puzzle: " + puzzleId, this);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        protected virtual bool IsCorrectPattern()
        {
            if (expectedPattern == null || currentInputs.Count != expectedPattern.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedPattern.Length; i++)
            {
                string current = currentInputs[i] != null ? currentInputs[i] : string.Empty;
                string expected = expectedPattern[i] != null ? expectedPattern[i] : string.Empty;
                if (current.ToUpperInvariant() != expected.ToUpperInvariant())
                {
                    return false;
                }
            }

            return true;
        }

        // Applies calculated settings to Unity components or runtime state.
        protected virtual void ApplyPowerDeviceReward()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkDoorOpened(unlockDoorId);
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.UnlockClue(unlockClueId);
            }
            else
            {
                Debug.LogWarning("ClueImageManager instance is missing.");
            }

            if (InventoryManager.Instance != null)
            {
                if (puzzleRecord != null && !string.IsNullOrEmpty(puzzleRecord.requiredItemId))
                {
                    InventoryManager.Instance.TryRemoveItem(puzzleRecord.requiredItemId);
                }

                if (!string.IsNullOrEmpty(requiredSecondItemId) && !string.IsNullOrEmpty(transformedItemId))
                {
                    if (!InventoryManager.Instance.TryTransformItem(requiredSecondItemId, transformedItemId))
                    {
                        InventoryManager.Instance.TryRemoveItem(requiredSecondItemId);
                        InventoryManager.Instance.TryAddItem(transformedItemId);
                    }
                }
            }
            else
            {
                Debug.LogWarning("InventoryManager instance is missing.");
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        // Performs the Cache Switch Buttons operation while keeping its implementation details inside this script.
        protected virtual void CacheSwitchButtons()
        {
            Transform root = switchButtonRoot != null ? switchButtonRoot : transform;
            PuzzlePowerSwitchButton[] foundButtons = root.GetComponentsInChildren<PuzzlePowerSwitchButton>(true);

            switchButtons.Clear();
            for (int i = 0; i < foundButtons.Length; i++)
            {
                if (foundButtons[i] != null && !switchButtons.Contains(foundButtons[i]))
                {
                    switchButtons.Add(foundButtons[i]);
                    foundButtons[i].SetTarget(this);
                }
            }
        }

        // Re-reads current game data and manager state, then redraws the visible UI.
        protected virtual void RefreshDisplay()
        {
            if (inputText != null)
            {
                inputText.text = string.Join(" > ", currentInputs.ToArray());
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        protected virtual void SetMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        // Performs the Hook Buttons operation while keeping its implementation details inside this script.
        protected virtual void HookButtons()
        {
            if (autoCollectSwitchButtons && switchButtons.Count == 0)
            {
                CacheSwitchButtons();
            }

            if (powerButton != null)
            {
                powerButton.onClick.RemoveListener(PressPowerButton);
                powerButton.onClick.AddListener(PressPowerButton);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveListener(ResetInput);
                resetButton.onClick.AddListener(ResetInput);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
                closeButton.onClick.AddListener(Close);
            }
        }

        // Performs the Unhook Buttons operation while keeping its implementation details inside this script.
        protected virtual void UnhookButtons()
        {
            if (powerButton != null)
            {
                powerButton.onClick.RemoveListener(PressPowerButton);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveListener(ResetInput);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
            }
        }
    }
}
