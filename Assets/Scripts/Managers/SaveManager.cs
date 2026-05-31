// -----------------------------------------------------------------------------
// Codex comment pass: Save Data
// Role: Coordinates a runtime system that other UI, puzzle, and interaction scripts call into.
// Scope: This script belongs to Managers\SaveManager.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EscapeFromNightmare
{
    [Serializable]
    // Runtime owner for the Save Data system, keeping shared state and events behind one access point.
    public class SaveData
    {
        // Stores the current Location Id value used by this script's runtime or editor workflow.
        public string currentLocationId;
        // Stores the current View Id value used by this script's runtime or editor workflow.
        public string currentViewId;
        // Stores the completed Puzzle Ids value used by this script's runtime or editor workflow.
        public List<string> completedPuzzleIds = new List<string>();
        // Stores the owned Item Ids value used by this script's runtime or editor workflow.
        public List<string> ownedItemIds = new List<string>();
        // Stores the used Item Ids value used by this script's runtime or editor workflow.
        public List<string> usedItemIds = new List<string>();
        // Stores the opened Door Ids value used by this script's runtime or editor workflow.
        public List<string> openedDoorIds = new List<string>();
        // Stores the unlocked Clue Ids value used by this script's runtime or editor workflow.
        public List<string> unlockedClueIds = new List<string>();
        // Stores the final Chase Started value used by this script's runtime or editor workflow.
        public bool finalChaseStarted;
        // Stores the has Started value used by this script's runtime or editor workflow.
        public bool hasStarted;
        // Stores the last Saved Utc value used by this script's runtime or editor workflow.
        public string lastSavedUtc;
    }

    // Runtime owner for the Save Manager system, keeping shared state and events behind one access point.
    public class SaveManager : Singleton<SaveManager>
    {
        // Stores the current Data value used by this script's runtime or editor workflow.
        private SaveData currentData;

        protected override bool UseDontDestroyOnLoad
        {
            get { return true; }
        }

        public SaveData CurrentData
        {
            get
            {
                EnsureData();
                return currentData;
            }
        }

        public string CurrentLocationId
        {
            get
            {
                EnsureData();
                return currentData.currentLocationId;
            }
        }

        public string CurrentViewId
        {
            get
            {
                EnsureData();
                return currentData.currentViewId;
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public IReadOnlyList<string> GetOwnedItemIds()
        {
            EnsureData();
            return currentData.ownedItemIds;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public IReadOnlyList<string> GetUnlockedClueIds()
        {
            EnsureData();
            return currentData.unlockedClueIds;
        }

        // Performs the Replace Owned Items operation while keeping its implementation details inside this script.
        public void ReplaceOwnedItems(IEnumerable<string> itemIds)
        {
            EnsureData();
            currentData.ownedItemIds.Clear();

            if (itemIds == null)
            {
                return;
            }

            foreach (string itemId in itemIds)
            {
                if (!string.IsNullOrEmpty(itemId) && !currentData.ownedItemIds.Contains(itemId))
                {
                    currentData.ownedItemIds.Add(itemId);
                }
            }
        }

        // Performs the Remove Owned Item operation while keeping its implementation details inside this script.
        public void RemoveOwnedItem(string itemId)
        {
            EnsureData();

            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            currentData.ownedItemIds.Remove(itemId);
        }

        // Performs the Replace Unlocked Clues operation while keeping its implementation details inside this script.
        public void ReplaceUnlockedClues(IEnumerable<string> clueIds)
        {
            EnsureData();
            currentData.unlockedClueIds.Clear();

            if (clueIds == null)
            {
                return;
            }

            foreach (string clueId in clueIds)
            {
                if (!string.IsNullOrEmpty(clueId) && !currentData.unlockedClueIds.Contains(clueId))
                {
                    currentData.unlockedClueIds.Add(clueId);
                }
            }
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetFinalChaseStarted(bool value)
        {
            EnsureData();
            currentData.finalChaseStarted = value;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsFinalChaseStarted()
        {
            EnsureData();
            return currentData.finalChaseStarted;
        }

        // Caches required component references and prepares this object before other startup code runs.
        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            EnsureData();
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private void EnsureData()
        {
            if (currentData == null)
            {
                currentData = CreateDefaultSaveData();
            }

            if (currentData.completedPuzzleIds == null)
            {
                currentData.completedPuzzleIds = new List<string>();
            }

            if (currentData.ownedItemIds == null)
            {
                currentData.ownedItemIds = new List<string>();
            }

            if (currentData.usedItemIds == null)
            {
                currentData.usedItemIds = new List<string>();
            }

            if (currentData.openedDoorIds == null)
            {
                currentData.openedDoorIds = new List<string>();
            }

            if (currentData.unlockedClueIds == null)
            {
                currentData.unlockedClueIds = new List<string>();
            }

            if (currentData.currentLocationId == null)
            {
                currentData.currentLocationId = string.Empty;
            }

            if (currentData.currentViewId == null)
            {
                currentData.currentViewId = string.Empty;
            }

            if (currentData.lastSavedUtc == null)
            {
                currentData.lastSavedUtc = string.Empty;
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool HasSaveData()
        {
            try
            {
                return File.Exists(GetSavePath());
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not check save file: " + exception.Message);
                return false;
            }
        }

        // Creates the required Unity objects and components, then places them in the expected hierarchy.
        public SaveData CreateDefaultSaveData()
        {
            SaveData data = new SaveData();
            data.currentLocationId = string.Empty;
            data.currentViewId = string.Empty;
            data.completedPuzzleIds = new List<string>();
            data.ownedItemIds = new List<string>();
            data.usedItemIds = new List<string>();
            data.openedDoorIds = new List<string>();
            data.unlockedClueIds = new List<string>();
            data.finalChaseStarted = false;
            data.hasStarted = false;
            data.lastSavedUtc = string.Empty;
            return data;
        }

        // Returns runtime state to its defaults for a new game, retry, or clean test run.
        public void ResetDataForNewGame()
        {
            string path = GetSavePath();
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not delete previous save file: " + exception.Message);
            }

            currentData = CreateDefaultSaveData();
            MarkGameStarted();
            SaveGame();
        }

        // Resets runtime state for editor-only manual play without deleting or overwriting save_data.json.
        public void ResetDataForNewGameInMemory()
        {
            currentData = CreateDefaultSaveData();
            MarkGameStarted();
        }

        // Performs the Try Load Game operation while keeping its implementation details inside this script.
        public bool TryLoadGame()
        {
            bool loaded = LoadGame();
            if (!loaded)
            {
                currentData = CreateDefaultSaveData();
                EnsureData();
                return false;
            }

            EnsureData();
            return true;
        }

        // Performs the Mark Game Started operation while keeping its implementation details inside this script.
        public void MarkGameStarted()
        {
            EnsureData();
            currentData.hasStarted = true;
            currentData.lastSavedUtc = DateTime.UtcNow.ToString("o");
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        public void SetCurrentPosition(string locationId, string viewId)
        {
            EnsureData();
            currentData.currentLocationId = locationId;
            currentData.currentViewId = viewId;
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsPuzzleCompleted(string puzzleId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(puzzleId) && currentData.completedPuzzleIds.Contains(puzzleId);
        }

        // Performs the Mark Puzzle Completed operation while keeping its implementation details inside this script.
        public void MarkPuzzleCompleted(string puzzleId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(puzzleId) && !currentData.completedPuzzleIds.Contains(puzzleId))
            {
                currentData.completedPuzzleIds.Add(puzzleId);
            }
        }

        // Performs the Mark Checkpoint After Puzzle operation while keeping its implementation details inside this script.
        public void MarkCheckpointAfterPuzzle(string puzzleId)
        {
            if (string.IsNullOrEmpty(puzzleId))
            {
                return;
            }

            MarkPuzzleCompleted(puzzleId);
            SaveGame();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsDoorOpened(string doorId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(doorId) && currentData.openedDoorIds.Contains(doorId);
        }

        // Performs the Mark Door Opened operation while keeping its implementation details inside this script.
        public void MarkDoorOpened(string doorId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(doorId) && !currentData.openedDoorIds.Contains(doorId))
            {
                currentData.openedDoorIds.Add(doorId);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsItemOwned(string itemId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(itemId) && currentData.ownedItemIds.Contains(itemId);
        }

        // Performs the Mark Item Owned operation while keeping its implementation details inside this script.
        public void MarkItemOwned(string itemId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(itemId) && !currentData.ownedItemIds.Contains(itemId))
            {
                currentData.ownedItemIds.Add(itemId);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsItemUsed(string itemId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(itemId) && currentData.usedItemIds.Contains(itemId);
        }

        // Performs the Mark Item Used operation while keeping its implementation details inside this script.
        public void MarkItemUsed(string itemId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(itemId) && !currentData.usedItemIds.Contains(itemId))
            {
                currentData.usedItemIds.Add(itemId);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public bool IsClueUnlocked(string clueId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(clueId) && currentData.unlockedClueIds.Contains(clueId);
        }

        // Performs the Mark Clue Unlocked operation while keeping its implementation details inside this script.
        public void MarkClueUnlocked(string clueId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(clueId) && !currentData.unlockedClueIds.Contains(clueId))
            {
                currentData.unlockedClueIds.Add(clueId);
            }
        }

        // Collects current runtime state and writes it to the configured save location.
        public void SaveGame()
        {
            EnsureData();
            MarkGameStarted();

            if (LocationManager.Instance != null)
            {
                currentData.currentLocationId = LocationManager.Instance.CurrentLocationId;
                currentData.currentViewId = LocationManager.Instance.CurrentViewId;
            }

            if (InventoryManager.Instance != null)
            {
                currentData.ownedItemIds = new List<string>(InventoryManager.Instance.OwnedItemIds);
            }

            try
            {
                string json = JsonUtility.ToJson(currentData, true);
                File.WriteAllText(GetSavePath(), json);
                Debug.Log("Game saved: " + GetSavePath());
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not save game: " + exception.Message);
            }
        }

        // Loads saved data or Resources assets and converts them into runtime-ready values.
        public bool LoadGame()
        {
            string path = GetSavePath();
            try
            {
                if (!File.Exists(path))
                {
                    Debug.LogWarning("Save file not found: " + path);
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not check save file: " + exception.Message);
                return false;
            }

            try
            {
                string json = File.ReadAllText(path);
                currentData = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Save file could not be read: " + exception.Message);
                currentData = CreateDefaultSaveData();
                return false;
            }

            if (currentData == null)
            {
                Debug.LogWarning("Save file could not be parsed: " + path);
                currentData = CreateDefaultSaveData();
                return false;
            }

            EnsureData();

            if (LocationManager.Instance != null)
            {
                LocationManager.Instance.SetLocation(currentData.currentLocationId, currentData.currentViewId);
            }
            else
            {
                Debug.LogWarning("LocationManager instance is missing.");
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ReplaceInventory(currentData.ownedItemIds);
            }
            else
            {
                Debug.LogWarning("InventoryManager instance is missing.");
            }

            return true;
        }

        // Performs the Delete Save operation while keeping its implementation details inside this script.
        public void DeleteSave()
        {
            string path = GetSavePath();
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not delete save file: " + exception.Message);
            }

            currentData = CreateDefaultSaveData();
            EnsureData();
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        public string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, "save_data.json");
        }

        // Performs the Auto Save If Possible operation while keeping its implementation details inside this script.
        public void AutoSaveIfPossible()
        {
            if (currentData == null)
            {
                return;
            }

            if (GameManager.Instance != null)
            {
                GameState state = GameManager.Instance.CurrentState;
                if (state == GameState.None || state == GameState.Title || state == GameState.GameOver || state == GameState.Ending)
                {
                    return;
                }
            }

            SaveGame();
        }

        // Performs the On Application Pause operation while keeping its implementation details inside this script.
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                AutoSaveIfPossible();
            }
        }

        // Performs the On Application Quit operation while keeping its implementation details inside this script.
        private void OnApplicationQuit()
        {
            AutoSaveIfPossible();
        }
    }
}
