// -----------------------------------------------------------------------------
// Codex comment pass: Clue Image Manager
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\ClueImageManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmare
{
    // Runtime owner for the Clue Image Manager system, keeping shared state and events behind one access point.
    public class ClueImageManager : Singleton<ClueImageManager>
    {
        [SerializeField] private ClueImagePanelUI clueImagePanel;
        [SerializeField] private bool loadUnlockedCluesOnStart = true;
        [SerializeField] private bool unlockStartsUnlockedCluesOnStart = true;
        [SerializeField] private bool setExamineGameState = true;

        // Stores the unlocked Clue Ids value used by this script's runtime or editor workflow.
        private readonly HashSet<string> unlockedClueIds = new HashSet<string>();
        // Stores the current Clue value used by this script's runtime or editor workflow.
        private ClueRecord currentClue;

        public event Action<string> ClueUnlocked;
        public event Action<string> ClueShown;
        public event Action ClueHidden;

        // Finishes startup after the scene has initialized other objects and managers.
        private void Start()
        {
            if (loadUnlockedCluesOnStart)
            {
                LoadUnlockedCluesFromSave();
            }

            if (unlockStartsUnlockedCluesOnStart)
            {
                UnlockStartsUnlockedClues();
            }
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        public void LoadUnlockedCluesFromSave()
        {
            unlockedClueIds.Clear();

            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("SaveManager instance is missing.");
                return;
            }

            IReadOnlyList<string> clueIds = SaveManager.Instance.GetUnlockedClueIds();
            for (int i = 0; i < clueIds.Count; i++)
            {
                if (!string.IsNullOrEmpty(clueIds[i]))
                {
                    unlockedClueIds.Add(clueIds[i]);
                }
            }
        }

        // Performs the Unlock Starts Unlocked Clues operation while keeping its implementation details inside this script.
        public void UnlockStartsUnlockedClues()
        {
            if (GameDataManager.Instance == null || GameDataManager.Instance.Clues == null)
            {
                Debug.LogWarning("GameDataManager instance is missing.");
                return;
            }

            bool changed = false;
            IReadOnlyList<ClueRecord> clues = GameDataManager.Instance.Clues;
            for (int i = 0; i < clues.Count; i++)
            {
                ClueRecord clue = clues[i];
                if (clue == null || !clue.startsUnlocked || string.IsNullOrEmpty(clue.clueId))
                {
                    continue;
                }

                if (unlockedClueIds.Add(clue.clueId))
                {
                    changed = true;
                    if (SaveManager.Instance != null)
                    {
                        SaveManager.Instance.MarkClueUnlocked(clue.clueId);
                    }
                }
            }

            if (changed && SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        // Makes the related panel or visual element visible and fills in its current content.
        public void ShowClueImage(string clueImageId)
        {
            if (string.IsNullOrEmpty(clueImageId))
            {
                Debug.LogWarning("Cannot show clue image with an empty clue id.");
                return;
            }

            if (clueImagePanel == null)
            {
                Debug.LogWarning("ClueImagePanelUI is not assigned.");
                return;
            }

            ClueRecord record = GetClueRecord(clueImageId);
            if (record == null)
            {
                clueImagePanel.ShowMessageOnly("Missing Clue", clueImageId);
                SetGameStateForShow();
                return;
            }

            if (!CanShowClue(record))
            {
                clueImagePanel.ShowLockedMessage(GetLockedMessage(record));
                SetGameStateForShow();
                return;
            }

            UnlockClue(record.clueId);

            Sprite sprite = LoadClueSprite(record);
            clueImagePanel.ShowClue(record, sprite);
            currentClue = record;
            SetGameStateForShow();

            if (ClueShown != null)
            {
                ClueShown.Invoke(record.clueId);
            }
        }

        // Hides the related panel or visual element and clears transient interaction state.
        public void HideCurrentImage()
        {
            if (clueImagePanel != null)
            {
                clueImagePanel.Hide();
            }

            currentClue = null;
            SetGameStateForHide();

            if (ClueHidden != null)
            {
                ClueHidden.Invoke();
            }
        }

        // Performs the Unlock Clue operation while keeping its implementation details inside this script.
        public void UnlockClue(string clueId)
        {
            if (string.IsNullOrEmpty(clueId))
            {
                return;
            }

            if (!unlockedClueIds.Add(clueId))
            {
                return;
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkClueUnlocked(clueId);
                SaveManager.Instance.SaveGame();
            }
            else
            {
                Debug.LogWarning("SaveManager instance is missing.");
            }

            if (ClueUnlocked != null)
            {
                ClueUnlocked.Invoke(clueId);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsClueUnlocked(string clueId)
        {
            if (string.IsNullOrEmpty(clueId))
            {
                return false;
            }

            if (unlockedClueIds.Contains(clueId))
            {
                return true;
            }

            if (SaveManager.Instance != null && SaveManager.Instance.IsClueUnlocked(clueId))
            {
                return true;
            }

            ClueRecord record = GetClueRecord(clueId);
            return record != null && record.startsUnlocked;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool CanShowClue(ClueRecord record)
        {
            if (record == null)
            {
                return false;
            }

            if (record.startsUnlocked)
            {
                return true;
            }

            if (IsClueUnlocked(record.clueId))
            {
                return true;
            }

            return AreRequirementsSatisfied(record);
        }

        // Performs the Are Requirements Satisfied operation while keeping its implementation details inside this script.
        public bool AreRequirementsSatisfied(ClueRecord record)
        {
            if (record == null)
            {
                return false;
            }

            bool hasPuzzleRequirement = !string.IsNullOrEmpty(record.requiredPuzzleId);
            bool hasItemRequirement = !string.IsNullOrEmpty(record.requiredItemId);

            if (!hasPuzzleRequirement && !hasItemRequirement)
            {
                return false;
            }

            if (hasPuzzleRequirement && !IsRequiredPuzzleCompleted(record.requiredPuzzleId))
            {
                return false;
            }

            if (hasItemRequirement && !IsRequiredItemSatisfied(record.requiredItemId))
            {
                return false;
            }

            return true;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public string GetLockedMessage(ClueRecord record)
        {
            if (record != null && !string.IsNullOrEmpty(record.lockedMessage))
            {
                return record.lockedMessage;
            }

            if (record != null && !string.IsNullOrEmpty(record.requiredPuzzleId))
            {
                return "A clue is still missing.";
            }

            if (record != null && !string.IsNullOrEmpty(record.requiredItemId))
            {
                return "You need the right item.";
            }

            return "You cannot examine this yet.";
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private ClueRecord GetClueRecord(string clueId)
        {
            if (GameDataManager.Instance == null)
            {
                Debug.LogWarning("GameDataManager instance is missing.");
                return null;
            }

            return GameDataManager.Instance.GetClueById(clueId);
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        private Sprite LoadClueSprite(ClueRecord record)
        {
            if (record == null || string.IsNullOrEmpty(record.imagePath))
            {
                return null;
            }

            Sprite sprite = Resources.Load<Sprite>(record.imagePath);
            if (sprite == null)
            {
                Debug.LogWarning("Clue image sprite not found at Resources path: " + record.imagePath);
            }

            return sprite;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsRequiredPuzzleCompleted(string puzzleId)
        {
            if (string.IsNullOrEmpty(puzzleId))
            {
                return true;
            }

            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("SaveManager instance is missing.");
                return false;
            }

            return SaveManager.Instance.IsPuzzleCompleted(puzzleId);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsRequiredItemSatisfied(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return true;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("InventoryManager instance is missing.");
                return false;
            }

            return InventoryManager.Instance.HasItem(itemId);
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetGameStateForShow()
        {
            if (setExamineGameState && GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Examine);
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private void SetGameStateForHide()
        {
            if (setExamineGameState && GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Examine)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }
        }
    }
}
