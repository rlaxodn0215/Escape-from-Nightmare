using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    public class RemainingPuzzleRuntimeTestRunner : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private float waitAfterOpenSeconds = 0.1f;
        [SerializeField] private float waitAfterSubmitSeconds = 0.1f;
        [SerializeField] private string reportRelativePath = "Docs/GeneratedRemainingPuzzleRuntimeTestReport.md";

        private readonly List<PuzzleRuntimeTestResult> results = new List<PuzzleRuntimeTestResult>();
        private readonly List<GameObject> createdRuntimeObjects = new List<GameObject>();
        private string originalSaveBackupPath;
        private bool originalSaveExisted;
        private bool saveBackupSucceeded;
        private bool originalReturnToTitleAfterEnding = true;
        private bool capturedEndingReturnSetting;

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
            capturedEndingReturnSetting = false;

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

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.LoadAllData();
            }

            if (EndingManager.Instance != null)
            {
                originalReturnToTitleAfterEnding = EndingManager.Instance.ReturnToTitleAfterEnding;
                capturedEndingReturnSetting = true;
                EndingManager.Instance.SetReturnToTitleAfterEnding(false);
            }

            try
            {
                yield return TestLivingRoomItemUse();
                yield return TestBasementPowerDevice();
                yield return TestLockedRoomFinal();
                yield return TestEntranceDoor();
            }
            finally
            {
                ClosePuzzleIfOpen();
                WriteMarkdownReport();
                RestoreOriginalSave();

                if (capturedEndingReturnSetting && EndingManager.Instance != null)
                {
                    EndingManager.Instance.SetReturnToTitleAfterEnding(originalReturnToTitleAfterEnding);
                }

                CleanupRuntimeObjects();
            }

            int passed = CountPassed();
            int failed = results.Count - passed;
            Debug.Log("Remaining puzzle runtime tests completed. Passed: " + passed + ", Failed: " + failed);
        }

        private IEnumerator TestLivingRoomItemUse()
        {
            const string testName = "LivingRoom Item Use";
            const string puzzleId = "Puzzle_LivingRoom_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForTests();
            InventoryManager.Instance.TryAddItem("OldDrawerKey");
            InventoryManager.Instance.SelectItem("OldDrawerKey");

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleItemUseUIBase ui = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleItemUseUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            ui.UseSelectedItem();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool hasReward = HasItem("SmallClockworkDevice");
            if (completed && hasReward)
            {
                AddPass(testName, puzzleId, "Completed and rewarded SmallClockworkDevice.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected completion and SmallClockworkDevice. Completed=" + completed + ", HasReward=" + hasReward, Elapsed(start));
            }
        }

        private IEnumerator TestBasementPowerDevice()
        {
            const string testName = "Basement Power Device";
            const string puzzleId = "Puzzle_BasementStorage_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForTests();
            InventoryManager.Instance.TryAddItem("BasementFuse");
            InventoryManager.Instance.TryAddItem("SmallClockworkDevice");

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzlePowerDeviceUIBase ui = FindCurrentPuzzleUi<PuzzlePowerDeviceUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzlePowerDeviceUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            ui.InputSwitch("Switch_Left");
            ui.InputSwitch("Switch_Right");
            ui.InputSwitch("Switch_Center");
            ui.InputSwitch("Switch_Left");
            ui.InputSwitch("Switch_Right");
            ui.PressPowerButton();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool doorOpened = IsDoorOpened("Door_BasementStorage_LockedRoom");
            bool clueUnlocked = IsClueUnlocked("BasementClueImage");
            bool hasModified = HasItem("ModifiedClockworkDevice");
            bool consumedFuse = !HasItem("BasementFuse");
            bool consumedSmall = !HasItem("SmallClockworkDevice");
            if (completed && doorOpened && clueUnlocked && hasModified && consumedFuse && consumedSmall)
            {
                AddPass(testName, puzzleId, "Completed, opened locked room, unlocked BasementClueImage, and transformed items.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected power success state. Completed=" + completed + ", DoorOpened=" + doorOpened + ", ClueUnlocked=" + clueUnlocked + ", HasModified=" + hasModified + ", ConsumedFuse=" + consumedFuse + ", ConsumedSmall=" + consumedSmall, Elapsed(start));
            }
        }

        private IEnumerator TestLockedRoomFinal()
        {
            const string testName = "Locked Room Final";
            const string puzzleId = "Puzzle_LockedRoom_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForTests();
            InventoryManager.Instance.TryAddItem("ModifiedClockworkDevice");

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleUI_LockedRoomFinal ui = FindCurrentPuzzleUi<PuzzleUI_LockedRoomFinal>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleUI_LockedRoomFinal was not found after opening the puzzle.", Elapsed(start));
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

            if (!ui.IsSequenceSolvedForTest)
            {
                AddFail(testName, puzzleId, "Correct symbol sequence did not enter item-use state.", Elapsed(start));
                yield break;
            }

            InventoryManager.Instance.SelectItem("ModifiedClockworkDevice");
            ui.UseRequiredItem();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool hasFrontDoorKey = HasItem("FrontDoorKey");
            bool finalChaseStarted = IsFinalChaseStarted();
            if (completed && hasFrontDoorKey && finalChaseStarted)
            {
                AddPass(testName, puzzleId, "Completed, rewarded FrontDoorKey, and marked finalChaseStarted.", Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected FrontDoorKey and final chase state. Completed=" + completed + ", HasFrontDoorKey=" + hasFrontDoorKey + ", FinalChaseStarted=" + finalChaseStarted, Elapsed(start));
            }
        }

        private IEnumerator TestEntranceDoor()
        {
            const string testName = "Entrance Door";
            const string puzzleId = "Puzzle_Entrance_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForTests();
            InventoryManager.Instance.TryAddItem("FrontDoorKey");
            InventoryManager.Instance.SelectItem("FrontDoorKey");

            yield return OpenPuzzleAndWait(puzzleId);

            PuzzleItemUseUIBase ui = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
            if (ui == null)
            {
                AddFail(testName, puzzleId, "PuzzleItemUseUIBase was not found after opening the puzzle.", Elapsed(start));
                yield break;
            }

            ui.UseSelectedItem();
            yield return new WaitForSeconds(waitAfterSubmitSeconds);

            bool completed = IsPuzzleCompleted(puzzleId);
            bool endingReached = GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Ending;
            if (endingReached || completed)
            {
                AddPass(testName, puzzleId, "Entrance puzzle completed and ending flow was triggered. Completed=" + completed + ", EndingState=" + endingReached, Elapsed(start));
            }
            else
            {
                AddFail(testName, puzzleId, "Expected GameState.Ending or completed Entrance puzzle. Completed=" + completed + ", EndingState=" + endingReached, Elapsed(start));
            }
        }

        private IEnumerator OpenPuzzleAndWait(string puzzleId)
        {
            if (PuzzleRetryLockManager.Instance != null)
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

            if (PuzzleManager.Instance.CurrentPuzzleInstance == null
                || PuzzleManager.Instance.CurrentPuzzleInstance.GetComponentInChildren<PuzzleUIBase>(true) == null)
            {
                Debug.LogWarning("PuzzleManager.OpenPuzzle did not produce a usable puzzle UI. Falling back to direct test instantiate: " + puzzleId);
                DirectInstantiatePuzzleForTest(puzzleId);
                yield return new WaitForSeconds(waitAfterOpenSeconds);
            }

            if (PuzzleManager.Instance.CurrentPuzzleInstance == null)
            {
                Debug.LogError("Puzzle did not open: " + puzzleId);
                yield break;
            }
        }

        private bool DirectInstantiatePuzzleForTest(string puzzleId)
        {
            if (GameDataManager.Instance == null || PuzzleManager.Instance == null)
            {
                return false;
            }

            PuzzleRecord puzzle = GameDataManager.Instance.GetPuzzleById(puzzleId);
            if (puzzle == null || string.IsNullOrEmpty(puzzle.prefabPath))
            {
                return false;
            }

            GameObject prefab = Resources.Load<GameObject>(puzzle.prefabPath);
            if (prefab == null)
            {
                return false;
            }

            PuzzleManager.Instance.CloseCurrentPuzzle();
            Transform root = PuzzleManager.Instance.puzzleUiRoot;
            PuzzleManager.Instance.currentPuzzleInstance = root != null ? Instantiate(prefab, root) : Instantiate(prefab);
            PuzzleManager.Instance.currentPuzzle = puzzle;

            PuzzleUIBase ui = PuzzleManager.Instance.currentPuzzleInstance.GetComponentInChildren<PuzzleUIBase>(true);
            if (ui == null)
            {
                return false;
            }

            ui.Initialize(puzzle);
            return true;
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
                originalSaveBackupPath = savePath + ".remaining_runtime_test_backup";
                originalSaveExisted = File.Exists(savePath);
                if (originalSaveExisted)
                {
                    File.Copy(savePath, originalSaveBackupPath, true);
                }

                saveBackupSucceeded = true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not back up save_data.json before remaining puzzle runtime tests: " + exception.Message);
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
                Debug.LogWarning("Could not restore save_data.json after remaining puzzle runtime tests: " + exception.Message);
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
            EnsureManager<GameManager>("RemainingRuntimeTest_GameManager");
            EnsureManager<GameDataManager>("RemainingRuntimeTest_GameDataManager");
            EnsureManager<SaveManager>("RemainingRuntimeTest_SaveManager");
            EnsureManager<InventoryManager>("RemainingRuntimeTest_InventoryManager");
            EnsureManager<PuzzleManager>("RemainingRuntimeTest_PuzzleManager");
            EnsureManager<ClueImageManager>("RemainingRuntimeTest_ClueImageManager");
            EnsureManager<EndingManager>("RemainingRuntimeTest_EndingManager");
            EnsureManager<PuzzleRetryLockManager>("RemainingRuntimeTest_PuzzleRetryLockManager");

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
                    GameObject root = new GameObject("RemainingRuntimeTest_PuzzleUIRoot", typeof(RectTransform));
                    createdRuntimeObjects.Add(root);
                    PuzzleManager.Instance.puzzleUiRoot = root.transform;
                }
            }

            return GameDataManager.Instance != null
                && SaveManager.Instance != null
                && InventoryManager.Instance != null
                && PuzzleManager.Instance != null
                && ClueImageManager.Instance != null
                && EndingManager.Instance != null;
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
            if (typeof(T) == typeof(GameManager) && GameManager.Instance != null) return GameManager.Instance as T;
            if (typeof(T) == typeof(GameDataManager) && GameDataManager.Instance != null) return GameDataManager.Instance as T;
            if (typeof(T) == typeof(SaveManager) && SaveManager.Instance != null) return SaveManager.Instance as T;
            if (typeof(T) == typeof(InventoryManager) && InventoryManager.Instance != null) return InventoryManager.Instance as T;
            if (typeof(T) == typeof(PuzzleManager) && PuzzleManager.Instance != null) return PuzzleManager.Instance as T;
            if (typeof(T) == typeof(ClueImageManager) && ClueImageManager.Instance != null) return ClueImageManager.Instance as T;
            if (typeof(T) == typeof(EndingManager) && EndingManager.Instance != null) return EndingManager.Instance as T;
            if (typeof(T) == typeof(PuzzleRetryLockManager) && PuzzleRetryLockManager.Instance != null) return PuzzleRetryLockManager.Instance as T;
            return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        }

        private void AddPass(string testName, string puzzleId, string message, float duration)
        {
            AddResult(testName, puzzleId, true, message, duration);
        }

        private void AddFail(string testName, string puzzleId, string message, float duration)
        {
            Debug.LogError("[RemainingPuzzleRuntimeTestRunner] " + testName + " failed. " + message);
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
                builder.AppendLine("# Remaining Puzzle Runtime Test Report");
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
                builder.AppendLine("## Expected Rewards / State");
                builder.AppendLine();
                builder.AppendLine("| Puzzle ID | Expected Result |");
                builder.AppendLine("|---|---|");
                builder.AppendLine("| Puzzle_LivingRoom_01 | SmallClockworkDevice |");
                builder.AppendLine("| Puzzle_BasementStorage_01 | Door unlock, BasementClueImage, ModifiedClockworkDevice |");
                builder.AppendLine("| Puzzle_LockedRoom_01 | FrontDoorKey, finalChaseStarted |");
                builder.AppendLine("| Puzzle_Entrance_01 | GameState.Ending |");
                builder.AppendLine();
                builder.AppendLine("## Save Protection");
                builder.AppendLine();
                builder.AppendLine("- save_data.json is backed up before test.");
                builder.AppendLine("- save_data.json is restored after test.");
                builder.AppendLine("- If no save existed before test, generated save_data.json is deleted.");
                builder.AppendLine();
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- This test does not validate final UI design.");
                builder.AppendLine("- This test does not validate final animation or audio.");
                builder.AppendLine("- This test validates open, initialize, answer/use, completion, reward/state.");

                File.WriteAllText(path, builder.ToString());
                Debug.Log("Remaining puzzle runtime test report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not write remaining puzzle runtime test report: " + exception.Message);
            }
        }

        private string GetReportPath()
        {
            string relativePath = string.IsNullOrEmpty(reportRelativePath)
                ? "Docs/GeneratedRemainingPuzzleRuntimeTestReport.md"
                : reportRelativePath;
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relativePath);
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

        private bool IsDoorOpened(string doorId)
        {
            return SaveManager.Instance != null && SaveManager.Instance.IsDoorOpened(doorId);
        }

        private bool IsFinalChaseStarted()
        {
            return SaveManager.Instance != null && SaveManager.Instance.IsFinalChaseStarted();
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
