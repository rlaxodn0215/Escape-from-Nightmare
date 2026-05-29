using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    public class FirstFivePuzzleRuntimeTestRunner : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private bool exitPlayModeWhenDone = false;
        [SerializeField] private float waitAfterOpenSeconds = 0.1f;
        [SerializeField] private float waitAfterSubmitSeconds = 0.1f;
        [SerializeField] private string reportRelativePath = "Docs/GeneratedFirstFivePuzzleRuntimeTestReport.md";

        private readonly List<PuzzleRuntimeTestResult> results = new List<PuzzleRuntimeTestResult>();
        private readonly List<GameObject> createdRuntimeObjects = new List<GameObject>();
        private string originalSaveBackupPath;
        private bool originalSaveExisted;
        private bool saveBackupSucceeded;

        private IEnumerator Start()
        {
            if (runOnStart)
            {
                yield return RunAllTests();
            }
        }

        public IEnumerator RunAllTests()
        {
            results.Clear();
            saveBackupSucceeded = false;

            if (!EnsureRequiredManagers())
            {
                AddFail("Required Managers", string.Empty, "Required managers could not be prepared.", 0f);
                WriteMarkdownReport();
                yield break;
            }

            BackupCurrentSave();
            if (!saveBackupSucceeded)
            {
                AddFail("Save Backup", string.Empty, "Could not back up save_data.json safely. Runtime tests aborted.", 0f);
                WriteMarkdownReport();
                yield break;
            }

            ResetRuntimeStateForTests();

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.LoadAllData();
            }

            yield return TestBedroomNumberCode();
            yield return TestKitchenNumberCode();
            yield return TestChildRoomSequence();
            yield return TestStudySequence();
            yield return TestLivingRoomSymbolCycle();

            ClosePuzzleIfOpen();
            WriteMarkdownReport();
            RestoreOriginalSave();
            CleanupRuntimeObjects();

            int passed = CountPassed();
            int failed = results.Count - passed;
            Debug.Log("First five puzzle runtime tests completed. Passed: " + passed + ", Failed: " + failed);

            if (exitPlayModeWhenDone)
            {
                Debug.Log("exitPlayModeWhenDone is set, but runtime code does not stop the Unity Editor. Use the Editor menu or stop Play Mode manually.");
            }
        }

        private IEnumerator TestBedroomNumberCode()
        {
            const string testName = "Bedroom Number Code";
            const string puzzleId = "Puzzle_Bedroom_01";
            float start = Time.realtimeSinceStartup;

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleNumberCodeUIBase ui = FindCurrentPuzzleUi<PuzzleNumberCodeUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleNumberCodeUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            ui.AppendDigit(7);
            ui.AppendDigit(3);
            ui.AppendDigit(1);
            ui.AppendDigit(9);
            ui.Submit();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool hasReward = HasItem("OldDrawerKey");
            if (completed && hasReward)
            {
                AddPass(testName, puzzleId, "Completed and rewarded OldDrawerKey.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected completion and OldDrawerKey. Completed=" + completed + ", HasReward=" + hasReward, Elapsed(start));
            }
        }

        private IEnumerator TestKitchenNumberCode()
        {
            const string testName = "Kitchen Number Code";
            const string puzzleId = "Puzzle_Kitchen_01";
            float start = Time.realtimeSinceStartup;

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleNumberCodeUIBase ui = FindCurrentPuzzleUi<PuzzleNumberCodeUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleNumberCodeUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            ui.AppendDigit(4);
            ui.AppendDigit(8);
            ui.AppendDigit(2);
            ui.AppendDigit(6);
            ui.Submit();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool hasReward = HasItem("BasementFuse");
            bool hasFrontDoorKey = HasItem("FrontDoorKey");
            if (completed && hasReward && !hasFrontDoorKey)
            {
                AddPass(testName, puzzleId, "Completed and rewarded BasementFuse without granting FrontDoorKey.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected BasementFuse only. Completed=" + completed + ", HasBasementFuse=" + hasReward + ", HasFrontDoorKey=" + hasFrontDoorKey, Elapsed(start));
            }
        }

        private IEnumerator TestChildRoomSequence()
        {
            const string testName = "ChildRoom Sequence";
            const string puzzleId = "Puzzle_ChildRoom_01";
            float start = Time.realtimeSinceStartup;

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleSequenceUIBase ui = FindCurrentPuzzleUi<PuzzleSequenceUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleSequenceUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            string[] sequence = { "Symbol_01", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
            for (int i = 0; i < sequence.Length; i++)
            {
                ui.SelectOption(sequence[i]);
            }

            yield return new WaitForSeconds(waitAfterSubmitSeconds);
            if (!IsPuzzleCompleted(puzzleId))
            {
                ui.Submit();
                yield return new WaitForSeconds(waitAfterSubmitSeconds);
            }

            bool completed = IsPuzzleCompleted(puzzleId);
            bool clueUnlocked = IsClueUnlocked("ChildRoomCardSymbolClueImage");
            if (completed && clueUnlocked)
            {
                AddPass(testName, puzzleId, "Completed and unlocked ChildRoomCardSymbolClueImage.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected completion and ChildRoomCardSymbolClueImage. Completed=" + completed + ", ClueUnlocked=" + clueUnlocked, Elapsed(start));
            }
        }

        private IEnumerator TestStudySequence()
        {
            const string testName = "Study Sequence";
            const string puzzleId = "Puzzle_Study_01";
            float start = Time.realtimeSinceStartup;

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleSequenceUIBase ui = FindCurrentPuzzleUi<PuzzleSequenceUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleSequenceUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            string[] sequence = { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
            for (int i = 0; i < sequence.Length; i++)
            {
                ui.SelectOption(sequence[i]);
            }

            yield return new WaitForSeconds(waitAfterSubmitSeconds);
            if (!IsPuzzleCompleted(puzzleId))
            {
                ui.Submit();
                yield return new WaitForSeconds(waitAfterSubmitSeconds);
            }

            bool completed = IsPuzzleCompleted(puzzleId);
            bool clueUnlocked = IsClueUnlocked("StudyBookSymbolClueImage");
            if (completed && clueUnlocked)
            {
                AddPass(testName, puzzleId, "Completed and unlocked StudyBookSymbolClueImage.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected completion and StudyBookSymbolClueImage. Completed=" + completed + ", ClueUnlocked=" + clueUnlocked, Elapsed(start));
            }
        }

        private IEnumerator TestLivingRoomSymbolCycle()
        {
            const string testName = "LivingRoom Symbol Cycle";
            const string puzzleId = "Puzzle_LivingRoom_02";
            float start = Time.realtimeSinceStartup;

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleSymbolCycleUIBase ui = FindCurrentPuzzleUi<PuzzleSymbolCycleUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleSymbolCycleUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            string[] sequence = { "Symbol_01", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
            for (int i = 0; i < sequence.Length; i++)
            {
                if (!ui.SetSlotSymbolForTest(i, sequence[i]))
                {
                    AddFail(testName, puzzleId, "Could not set symbol slot " + i + " to " + sequence[i] + ".", Elapsed(start));
                    yield break;
                }
            }

            ui.Submit();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool clueUnlocked = IsClueUnlocked("KitchenCodeClueImage");
            if (completed && clueUnlocked)
            {
                AddPass(testName, puzzleId, "Completed and unlocked KitchenCodeClueImage.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected completion and KitchenCodeClueImage. Completed=" + completed + ", ClueUnlocked=" + clueUnlocked, Elapsed(start));
            }
        }

        private IEnumerator OpenPuzzleAndWait(string puzzleId)
        {
            if (PuzzleRetryLockManager.Instance != null && PuzzleRetryLockManager.Instance.IsLocked(puzzleId))
            {
                PuzzleRetryLockManager.Instance.ClearAllLocks();
            }

            if (PuzzleManager.Instance == null)
            {
                Debug.LogError("PuzzleManager instance is missing.");
                yield break;
            }

            PuzzleManager.Instance.CloseCurrentPuzzle();
            PuzzleManager.Instance.OpenPuzzle(puzzleId);
            yield return new WaitForSeconds(waitAfterOpenSeconds);

            if (PuzzleManager.Instance.CurrentPuzzleInstance == null)
            {
                Debug.LogError("Puzzle did not open: " + puzzleId);
                yield break;
            }

            PuzzleUIBase ui = PuzzleManager.Instance.CurrentPuzzleInstance.GetComponentInChildren<PuzzleUIBase>(true);
            if (ui == null)
            {
                Debug.LogError("Opened puzzle has no PuzzleUIBase: " + puzzleId);
            }
        }

        private void ClosePuzzleIfOpen()
        {
            if (PuzzleManager.Instance != null && PuzzleManager.Instance.HasOpenPuzzle)
            {
                PuzzleManager.Instance.CloseCurrentPuzzle();
            }
        }

        private void BackupCurrentSave()
        {
            saveBackupSucceeded = false;
            if (SaveManager.Instance == null)
            {
                Debug.LogError("SaveManager instance is missing.");
                return;
            }

            try
            {
                string savePath = SaveManager.Instance.GetSavePath();
                originalSaveBackupPath = savePath + ".runtime_test_backup";
                originalSaveExisted = File.Exists(savePath);

                if (originalSaveExisted)
                {
                    File.Copy(savePath, originalSaveBackupPath, true);
                }

                saveBackupSucceeded = true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not back up save_data.json before runtime tests: " + exception.Message);
            }
        }

        private void RestoreOriginalSave()
        {
            if (SaveManager.Instance == null)
            {
                return;
            }

            try
            {
                string savePath = SaveManager.Instance.GetSavePath();
                if (originalSaveExisted)
                {
                    if (!string.IsNullOrEmpty(originalSaveBackupPath) && File.Exists(originalSaveBackupPath))
                    {
                        File.Copy(originalSaveBackupPath, savePath, true);
                        File.Delete(originalSaveBackupPath);
                        SaveManager.Instance.LoadGame();
                    }
                }
                else if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not restore save_data.json after runtime tests: " + exception.Message);
            }
        }

        private void ResetRuntimeStateForTests()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ResetDataForNewGame();
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ReplaceInventory(new string[0]);
            }

            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.LoadUnlockedCluesFromSave();
            }

            ClosePuzzleIfOpen();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }

            if (PuzzleRetryLockManager.Instance != null)
            {
                PuzzleRetryLockManager.Instance.ClearAllLocks();
            }
        }

        private bool EnsureRequiredManagers()
        {
            EnsureManager<GameManager>("RuntimeTest_GameManager");
            EnsureManager<GameDataManager>("RuntimeTest_GameDataManager");
            EnsureManager<SaveManager>("RuntimeTest_SaveManager");
            EnsureManager<InventoryManager>("RuntimeTest_InventoryManager");
            EnsureManager<PuzzleManager>("RuntimeTest_PuzzleManager");
            EnsureManager<ClueImageManager>("RuntimeTest_ClueImageManager");
            EnsureManager<PuzzleRetryLockManager>("RuntimeTest_PuzzleRetryLockManager");

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.LoadAllData();
            }

            if (PuzzleManager.Instance != null && PuzzleManager.Instance.puzzleUiRoot == null)
            {
                GameObject existingRoot = GameObject.Find("PuzzleUIRoot");
                if (existingRoot != null)
                {
                    PuzzleManager.Instance.puzzleUiRoot = existingRoot.transform;
                }
                else
                {
                    GameObject root = new GameObject("RuntimeTest_PuzzleUIRoot", typeof(RectTransform));
                    createdRuntimeObjects.Add(root);
                    PuzzleManager.Instance.puzzleUiRoot = root.transform;
                }
            }

            return GameDataManager.Instance != null
                && SaveManager.Instance != null
                && InventoryManager.Instance != null
                && PuzzleManager.Instance != null
                && ClueImageManager.Instance != null;
        }

        private void EnsureManager<T>(string objectName) where T : MonoBehaviour
        {
            if (FindExistingManager<T>() != null)
            {
                return;
            }

            GameObject managerObject = new GameObject(objectName);
            createdRuntimeObjects.Add(managerObject);
            managerObject.AddComponent<T>();
        }

        private T FindExistingManager<T>() where T : MonoBehaviour
        {
            if (typeof(T) == typeof(GameManager) && GameManager.Instance != null)
            {
                return GameManager.Instance as T;
            }

            if (typeof(T) == typeof(GameDataManager) && GameDataManager.Instance != null)
            {
                return GameDataManager.Instance as T;
            }

            if (typeof(T) == typeof(SaveManager) && SaveManager.Instance != null)
            {
                return SaveManager.Instance as T;
            }

            if (typeof(T) == typeof(InventoryManager) && InventoryManager.Instance != null)
            {
                return InventoryManager.Instance as T;
            }

            if (typeof(T) == typeof(PuzzleManager) && PuzzleManager.Instance != null)
            {
                return PuzzleManager.Instance as T;
            }

            if (typeof(T) == typeof(ClueImageManager) && ClueImageManager.Instance != null)
            {
                return ClueImageManager.Instance as T;
            }

            if (typeof(T) == typeof(PuzzleRetryLockManager) && PuzzleRetryLockManager.Instance != null)
            {
                return PuzzleRetryLockManager.Instance as T;
            }

            return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        }

        private void AddPass(string testName, string puzzleId, string message, float duration)
        {
            AddResult(testName, puzzleId, true, message, duration);
        }

        private void AddFail(string testName, string puzzleId, string message, float duration)
        {
            Debug.LogError("[FirstFivePuzzleRuntimeTestRunner] " + testName + " failed. " + message);
            AddResult(testName, puzzleId, false, message, duration);
        }

        private void AddResult(string testName, string puzzleId, bool passed, string message, float duration)
        {
            PuzzleRuntimeTestResult result = new PuzzleRuntimeTestResult();
            result.testName = testName;
            result.puzzleId = puzzleId;
            result.passed = passed;
            result.message = message;
            result.durationSeconds = duration;
            results.Add(result);
        }

        private void WriteMarkdownReport()
        {
            string path = GetReportPath();
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                int passed = CountPassed();
                int failed = results.Count - passed;
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# First Five Puzzle Runtime Test Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Active Scene: " + SceneManager.GetActiveScene().name);
                builder.AppendLine("- Total: " + results.Count);
                builder.AppendLine("- Passed: " + passed);
                builder.AppendLine("- Failed: " + failed);
                builder.AppendLine();
                builder.AppendLine("## Results");
                builder.AppendLine();
                builder.AppendLine("| Test | Puzzle ID | Result | Message | Duration |");
                builder.AppendLine("|---|---|---|---|---|");

                for (int i = 0; i < results.Count; i++)
                {
                    PuzzleRuntimeTestResult result = results[i];
                    builder.Append("| ");
                    builder.Append(EscapeMarkdown(result.testName));
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(result.puzzleId));
                    builder.Append(" | ");
                    builder.Append(result.passed ? "Pass" : "Fail");
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(result.message));
                    builder.Append(" | ");
                    builder.Append(result.durationSeconds.ToString("0.00"));
                    builder.AppendLine("s |");
                }

                builder.AppendLine();
                builder.AppendLine("## Expected Rewards");
                builder.AppendLine();
                builder.AppendLine("| Puzzle ID | Expected Result |");
                builder.AppendLine("|---|---|");
                builder.AppendLine("| Puzzle_Bedroom_01 | OldDrawerKey |");
                builder.AppendLine("| Puzzle_Kitchen_01 | BasementFuse |");
                builder.AppendLine("| Puzzle_ChildRoom_01 | ChildRoomCardSymbolClueImage |");
                builder.AppendLine("| Puzzle_Study_01 | StudyBookSymbolClueImage |");
                builder.AppendLine("| Puzzle_LivingRoom_02 | KitchenCodeClueImage |");
                builder.AppendLine();
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- This test backs up and restores save_data.json.");
                builder.AppendLine("- This test does not validate final visual design.");
                builder.AppendLine("- This test does not test manual button layout.");
                builder.AppendLine("- This test validates open, initialize, answer submit, completion, and reward state.");

                File.WriteAllText(path, builder.ToString());
                Debug.Log("First five puzzle runtime test report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not write first five puzzle runtime test report: " + exception.Message);
            }
        }

        private string GetReportPath()
        {
            string relativePath = string.IsNullOrEmpty(reportRelativePath)
                ? "Docs/GeneratedFirstFivePuzzleRuntimeTestReport.md"
                : reportRelativePath;
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relativePath);
        }

        private T FindCurrentPuzzleUi<T>() where T : PuzzleUIBase
        {
            if (PuzzleManager.Instance != null && PuzzleManager.Instance.CurrentPuzzleInstance != null)
            {
                T current = PuzzleManager.Instance.CurrentPuzzleInstance.GetComponentInChildren<T>(true);
                if (current != null)
                {
                    return current;
                }
            }

            return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        }

        private bool IsPuzzleCompleted(string puzzleId)
        {
            return SaveManager.Instance != null && SaveManager.Instance.IsPuzzleCompleted(puzzleId);
        }

        private bool HasItem(string itemId)
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId))
            {
                return true;
            }

            return SaveManager.Instance != null && SaveManager.Instance.IsItemOwned(itemId);
        }

        private bool IsClueUnlocked(string clueId)
        {
            if (ClueImageManager.Instance != null && ClueImageManager.Instance.IsClueUnlocked(clueId))
            {
                return true;
            }

            return SaveManager.Instance != null && SaveManager.Instance.IsClueUnlocked(clueId);
        }

        private int CountPassed()
        {
            int count = 0;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i] != null && results[i].passed)
                {
                    count++;
                }
            }

            return count;
        }

        private float Elapsed(float start)
        {
            return Time.realtimeSinceStartup - start;
        }

        private string EscapeMarkdown(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }

        private void CleanupRuntimeObjects()
        {
            for (int i = 0; i < createdRuntimeObjects.Count; i++)
            {
                if (createdRuntimeObjects[i] != null)
                {
                    Destroy(createdRuntimeObjects[i]);
                }
            }

            createdRuntimeObjects.Clear();
        }
    }
}
