using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
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

        protected readonly List<string> currentInputs = new List<string>();
        protected string[] expectedPattern;

        public IReadOnlyList<string> CurrentInputs
        {
            get { return currentInputs; }
        }

        public string[] ExpectedPattern
        {
            get { return expectedPattern; }
        }

        protected virtual void Awake()
        {
            if (autoCollectSwitchButtons)
            {
                CacheSwitchButtons();
            }
        }

        protected virtual void OnEnable()
        {
            HookButtons();
            RefreshDisplay();
        }

        protected virtual void OnDisable()
        {
            UnhookButtons();
        }

        public override void Initialize(PuzzleRecord record)
        {
            base.Initialize(record);
            ResolveAnswer();
            ResetInput();
            SetMessage(string.Empty);
        }

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

        public void ResetInput()
        {
            currentInputs.Clear();
            RefreshDisplay();
        }

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

        protected virtual void RefreshDisplay()
        {
            if (inputText != null)
            {
                inputText.text = string.Join(" > ", currentInputs.ToArray());
            }
        }

        protected virtual void SetMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

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
