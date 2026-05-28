using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EscapeFromNightmare
{
    [Serializable]
    public class SaveData
    {
        public string currentLocationId;
        public string currentViewId;
        public List<string> completedPuzzleIds = new List<string>();
        public List<string> ownedItemIds = new List<string>();
        public List<string> usedItemIds = new List<string>();
        public List<string> openedDoorIds = new List<string>();
        public List<string> unlockedClueIds = new List<string>();
        public bool finalChaseStarted;
        public bool hasStarted;
        public string lastSavedUtc;
    }

    public class SaveManager : Singleton<SaveManager>
    {
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

        public IReadOnlyList<string> GetOwnedItemIds()
        {
            EnsureData();
            return currentData.ownedItemIds;
        }

        public IReadOnlyList<string> GetUnlockedClueIds()
        {
            EnsureData();
            return currentData.unlockedClueIds;
        }

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

        public void RemoveOwnedItem(string itemId)
        {
            EnsureData();

            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            currentData.ownedItemIds.Remove(itemId);
        }

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

        public void SetFinalChaseStarted(bool value)
        {
            EnsureData();
            currentData.finalChaseStarted = value;
        }

        public bool IsFinalChaseStarted()
        {
            EnsureData();
            return currentData.finalChaseStarted;
        }

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
            {
                return;
            }

            EnsureData();
        }

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

        public void MarkGameStarted()
        {
            EnsureData();
            currentData.hasStarted = true;
            currentData.lastSavedUtc = DateTime.UtcNow.ToString("o");
        }

        public void SetCurrentPosition(string locationId, string viewId)
        {
            EnsureData();
            currentData.currentLocationId = locationId;
            currentData.currentViewId = viewId;
        }

        public bool IsPuzzleCompleted(string puzzleId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(puzzleId) && currentData.completedPuzzleIds.Contains(puzzleId);
        }

        public void MarkPuzzleCompleted(string puzzleId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(puzzleId) && !currentData.completedPuzzleIds.Contains(puzzleId))
            {
                currentData.completedPuzzleIds.Add(puzzleId);
            }
        }

        public void MarkCheckpointAfterPuzzle(string puzzleId)
        {
            if (string.IsNullOrEmpty(puzzleId))
            {
                return;
            }

            MarkPuzzleCompleted(puzzleId);
            SaveGame();
        }

        public bool IsDoorOpened(string doorId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(doorId) && currentData.openedDoorIds.Contains(doorId);
        }

        public void MarkDoorOpened(string doorId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(doorId) && !currentData.openedDoorIds.Contains(doorId))
            {
                currentData.openedDoorIds.Add(doorId);
            }
        }

        public bool IsItemOwned(string itemId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(itemId) && currentData.ownedItemIds.Contains(itemId);
        }

        public void MarkItemOwned(string itemId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(itemId) && !currentData.ownedItemIds.Contains(itemId))
            {
                currentData.ownedItemIds.Add(itemId);
            }
        }

        public bool IsItemUsed(string itemId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(itemId) && currentData.usedItemIds.Contains(itemId);
        }

        public void MarkItemUsed(string itemId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(itemId) && !currentData.usedItemIds.Contains(itemId))
            {
                currentData.usedItemIds.Add(itemId);
            }
        }

        public bool IsClueUnlocked(string clueId)
        {
            EnsureData();
            return !string.IsNullOrEmpty(clueId) && currentData.unlockedClueIds.Contains(clueId);
        }

        public void MarkClueUnlocked(string clueId)
        {
            EnsureData();

            if (!string.IsNullOrEmpty(clueId) && !currentData.unlockedClueIds.Contains(clueId))
            {
                currentData.unlockedClueIds.Add(clueId);
            }
        }

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

        public string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, "save_data.json");
        }

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

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                AutoSaveIfPossible();
            }
        }

        private void OnApplicationQuit()
        {
            AutoSaveIfPossible();
        }
    }
}
