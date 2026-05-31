// -----------------------------------------------------------------------------
// Codex comment pass: Full Game Route Runtime Test Runner
// Role: Runs play-mode route and puzzle checks, then records failures in a form that can be reported by editor tools.
// Scope: This script belongs to Tests\FullGameRouteRuntimeTestRunner.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeFromNightmare
{
    // Runtime test helper for the Full Game Route Runtime Test Runner scenario, including setup, execution, and readable failure output.
    public class FullGameRouteRuntimeTestRunner : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = false;
        [SerializeField] private float waitAfterOpenSeconds = 0.1f;
        [SerializeField] private float waitAfterActionSeconds = 0.1f;
        [SerializeField] private string reportRelativePath = "Docs/GeneratedFullGameRouteRuntimeTestReport.md";
        [SerializeField] private bool allowTemporaryManagerBootstrap = true;
        [SerializeField] private bool runNegativeGateChecks = true;

        // Stores the steps value used by this script's runtime or editor workflow.
        private readonly List<FullGameRouteTestStepResult> steps = new List<FullGameRouteTestStepResult>();
        // Stores the created Runtime Objects value used by this script's runtime or editor workflow.
        private readonly List<GameObject> createdRuntimeObjects = new List<GameObject>();
        // Stores the original Save Backup Path value used by this script's runtime or editor workflow.
        private string originalSaveBackupPath;
        // Stores the original Save Existed value used by this script's runtime or editor workflow.
        private bool originalSaveExisted;
        // Stores the save Backup Succeeded value used by this script's runtime or editor workflow.
        private bool saveBackupSucceeded;
        // Stores the current Step Index value used by this script's runtime or editor workflow.
        private int currentStepIndex;
        // Stores the original Return To Title After Ending value used by this script's runtime or editor workflow.
        private bool originalReturnToTitleAfterEnding = true;
        // Stores the captured Ending Return Setting value used by this script's runtime or editor workflow.
        private bool capturedEndingReturnSetting;

        // Finishes startup after the scene has initialized other objects and managers.
        private IEnumerator Start()
        {
            if (runOnStart || RuntimeTestLaunchGate.ConsumeRun(nameof(FullGameRouteRuntimeTestRunner)))
            {
                yield return RunFullRouteTest();
            }
        }

        // Performs the Run Full Route Test operation while keeping its implementation details inside this script.
        public IEnumerator RunFullRouteTest()
        {
            steps.Clear();
            currentStepIndex = 0;
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
                AddFail("Save Backup", string.Empty, "Could not back up save_data.json safely. Full route test aborted.", 0f);
                WriteMarkdownReport();
                yield break;
            }

            try
            {
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

                if (runNegativeGateChecks)
                {
                    yield return RunNegativeGateChecksRoutine();
                }

                ResetRuntimeStateForFullRoute();
                yield return Step_InitialState();
                yield return Step_BedroomCode();
                yield return Step_LivingRoomItemUse();
                yield return Step_ChildRoomCardOrder();
                yield return Step_StudyBookOrder();
                yield return Step_LivingRoomSymbolSequence();
                yield return Step_KitchenCode();
                yield return Step_BasementPowerDevice();
                yield return Step_LockedRoomFinal();
                yield return Step_EntranceEnding();
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
                int passed = CountPassed();
                Debug.Log("Full game route runtime test completed. Passed: " + passed + ", Failed: " + (steps.Count - passed));
            }
        }

        // Performs the Run Negative Gate Checks Routine operation while keeping its implementation details inside this script.
        private IEnumerator RunNegativeGateChecksRoutine()
        {
            yield return Check_BasementPowerRequiresItems();
            yield return Check_LockedRoomRequiresModifiedClockwork();
            yield return Check_EntranceRequiresFrontDoorKey();
        }

        // Performs the Check Basement Power Requires Items operation while keeping its implementation details inside this script.
        private IEnumerator Check_BasementPowerRequiresItems()
        {
            const string stepName = "Gate: Basement Power Requires Items";
            const string puzzleId = "Puzzle_BasementStorage_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForFullRoute();
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzlePowerDeviceUIBase ui = FindCurrentPuzzleUi<PuzzlePowerDeviceUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzlePowerDeviceUIBase was not found.", Elapsed(start));
                yield break;
            }

            InputBasementPattern(ui);
            ui.PressPowerButton();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool blocked = !IsPuzzleCompleted(puzzleId) && !HasItem("ModifiedClockworkDevice");
            AddGateResult(blocked, stepName, puzzleId, "Basement power did not complete without required items.", "Basement power completed without required items.", start);
        }

        // Performs the Check Locked Room Requires Modified Clockwork operation while keeping its implementation details inside this script.
        private IEnumerator Check_LockedRoomRequiresModifiedClockwork()
        {
            const string stepName = "Gate: Locked Room Requires ModifiedClockworkDevice";
            const string puzzleId = "Puzzle_LockedRoom_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForFullRoute();
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleUI_LockedRoomFinal ui = FindCurrentPuzzleUi<PuzzleUI_LockedRoomFinal>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleUI_LockedRoomFinal was not found.", Elapsed(start));
                yield break;
            }

            SetFinalSymbolSequence(ui);
            ui.Submit();
            ui.UseRequiredItem();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool blocked = !IsPuzzleCompleted(puzzleId) && !HasItem("FrontDoorKey");
            AddGateResult(blocked, stepName, puzzleId, "LockedRoomFinal did not complete without ModifiedClockworkDevice.", "LockedRoomFinal completed without ModifiedClockworkDevice.", start);
        }

        // Performs the Check Entrance Requires Front Door Key operation while keeping its implementation details inside this script.
        private IEnumerator Check_EntranceRequiresFrontDoorKey()
        {
            const string stepName = "Gate: Entrance Requires FrontDoorKey";
            const string puzzleId = "Puzzle_Entrance_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForFullRoute();
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleItemUseUIBase ui = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleItemUseUIBase was not found.", Elapsed(start));
                yield break;
            }

            ui.UseSelectedItem();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool ending = GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Ending;
            AddGateResult(!ending, stepName, puzzleId, "Entrance did not enter Ending without FrontDoorKey.", "Entrance entered Ending without FrontDoorKey.", start);
        }

        // Performs the Add Gate Result operation while keeping its implementation details inside this script.
        private void AddGateResult(bool passed, string stepName, string targetId, string passMessage, string failMessage, float start)
        {
            if (passed)
            {
                AddPass(stepName, targetId, passMessage, Elapsed(start));
            }
            else
            {
                AddFail(stepName, targetId, failMessage, Elapsed(start));
            }
        }

        // Performs the Step Initial State operation while keeping its implementation details inside this script.
        private IEnumerator Step_InitialState()
        {
            const string stepName = "Initial State";
            float start = Time.realtimeSinceStartup;
            bool clean = !IsPuzzleCompleted("Puzzle_Bedroom_01")
                && !HasItem("OldDrawerKey")
                && !HasItem("FrontDoorKey")
                && !IsFinalChaseStarted();
            if (clean)
            {
                AddPass(stepName, "NewGame", "New game state is clean.", Elapsed(start));
            }
            else
            {
                AddFail(stepName, "NewGame", "Expected clean new game state.", Elapsed(start));
            }
            yield break;
        }

        // Performs the Step Bedroom Code operation while keeping its implementation details inside this script.
        private IEnumerator Step_BedroomCode()
        {
            const string stepName = "Bedroom Code";
            const string puzzleId = "Puzzle_Bedroom_01";
            float start = Time.realtimeSinceStartup;
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleNumberCodeUIBase ui = FindCurrentPuzzleUi<PuzzleNumberCodeUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleNumberCodeUIBase was not found.", Elapsed(start));
                yield break;
            }

            AppendDigits(ui, "7319");
            ui.Submit();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            AddProgressResult(IsPuzzleCompleted(puzzleId) && HasItem("OldDrawerKey"), stepName, puzzleId, "OldDrawerKey acquired.", "Expected OldDrawerKey and completed Bedroom puzzle.", start);
        }

        // Performs the Step Living Room Item Use operation while keeping its implementation details inside this script.
        private IEnumerator Step_LivingRoomItemUse()
        {
            const string stepName = "LivingRoom ItemUse";
            const string puzzleId = "Puzzle_LivingRoom_01";
            float start = Time.realtimeSinceStartup;
            if (!HasItem("OldDrawerKey"))
            {
                AddFail(stepName, puzzleId, "Skipped due to missing OldDrawerKey.", Elapsed(start));
                yield break;
            }

            InventoryManager.Instance.SelectItem("OldDrawerKey");
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleItemUseUIBase ui = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleItemUseUIBase was not found.", Elapsed(start));
                yield break;
            }

            ui.UseSelectedItem();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            AddProgressResult(IsPuzzleCompleted(puzzleId) && HasItem("SmallClockworkDevice"), stepName, puzzleId, "SmallClockworkDevice acquired.", "Expected SmallClockworkDevice and completed LivingRoom item-use puzzle.", start);
        }

        // Performs the Step Child Room Card Order operation while keeping its implementation details inside this script.
        private IEnumerator Step_ChildRoomCardOrder()
        {
            yield return RunSequenceStep("ChildRoom Card Order", "Puzzle_ChildRoom_01", new[] { "Symbol_01", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" }, "ChildRoomCardSymbolClueImage");
        }

        // Performs the Step Study Book Order operation while keeping its implementation details inside this script.
        private IEnumerator Step_StudyBookOrder()
        {
            yield return RunSequenceStep("Study Book Order", "Puzzle_Study_01", new[] { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" }, "StudyBookSymbolClueImage");
        }

        // Performs the Run Sequence Step operation while keeping its implementation details inside this script.
        private IEnumerator RunSequenceStep(string stepName, string puzzleId, string[] sequence, string clueId)
        {
            float start = Time.realtimeSinceStartup;
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleSequenceUIBase ui = FindCurrentPuzzleUi<PuzzleSequenceUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleSequenceUIBase was not found.", Elapsed(start));
                yield break;
            }

            for (int i = 0; i < sequence.Length; i++)
            {
                ui.SelectOption(sequence[i]);
            }
            yield return new WaitForSeconds(waitAfterActionSeconds);
            if (!IsPuzzleCompleted(puzzleId))
            {
                ui.Submit();
                yield return new WaitForSeconds(waitAfterActionSeconds);
            }

            AddProgressResult(IsPuzzleCompleted(puzzleId) && IsClueUnlocked(clueId), stepName, puzzleId, clueId + " unlocked.", "Expected clue unlock: " + clueId, start);
        }

        // Performs the Step Living Room Symbol Sequence operation while keeping its implementation details inside this script.
        private IEnumerator Step_LivingRoomSymbolSequence()
        {
            const string stepName = "LivingRoom Symbol Sequence";
            const string puzzleId = "Puzzle_LivingRoom_02";
            float start = Time.realtimeSinceStartup;
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleSymbolCycleUIBase ui = FindCurrentPuzzleUi<PuzzleSymbolCycleUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleSymbolCycleUIBase was not found.", Elapsed(start));
                yield break;
            }

            SetFinalSymbolSequence(ui);
            ui.Submit();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            AddProgressResult(IsPuzzleCompleted(puzzleId) && IsClueUnlocked("KitchenCodeClueImage"), stepName, puzzleId, "KitchenCodeClueImage unlocked.", "Expected KitchenCodeClueImage and completed LivingRoom symbol puzzle.", start);
        }

        // Performs the Step Kitchen Code operation while keeping its implementation details inside this script.
        private IEnumerator Step_KitchenCode()
        {
            const string stepName = "Kitchen Code";
            const string puzzleId = "Puzzle_Kitchen_01";
            float start = Time.realtimeSinceStartup;
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleNumberCodeUIBase ui = FindCurrentPuzzleUi<PuzzleNumberCodeUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleNumberCodeUIBase was not found.", Elapsed(start));
                yield break;
            }

            AppendDigits(ui, "4826");
            ui.Submit();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool ok = IsPuzzleCompleted(puzzleId) && HasItem("BasementFuse") && !HasItem("FrontDoorKey");
            AddProgressResult(ok, stepName, puzzleId, "BasementFuse acquired and FrontDoorKey not granted.", "Expected BasementFuse and no FrontDoorKey.", start);
        }

        // Performs the Step Basement Power Device operation while keeping its implementation details inside this script.
        private IEnumerator Step_BasementPowerDevice()
        {
            const string stepName = "Basement Power Device";
            const string puzzleId = "Puzzle_BasementStorage_01";
            float start = Time.realtimeSinceStartup;
            if (!HasItem("BasementFuse") || !HasItem("SmallClockworkDevice"))
            {
                AddFail(stepName, puzzleId, "Skipped due to missing BasementFuse or SmallClockworkDevice.", Elapsed(start));
                yield break;
            }

            yield return OpenPuzzleAndWait(puzzleId);
            PuzzlePowerDeviceUIBase ui = FindCurrentPuzzleUi<PuzzlePowerDeviceUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzlePowerDeviceUIBase was not found.", Elapsed(start));
                yield break;
            }

            InputBasementPattern(ui);
            ui.PressPowerButton();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool ok = IsPuzzleCompleted(puzzleId) && IsDoorOpened("Door_BasementStorage_LockedRoom") && IsClueUnlocked("BasementClueImage") && HasItem("ModifiedClockworkDevice");
            AddProgressResult(ok, stepName, puzzleId, "Basement door opened, clue unlocked, ModifiedClockworkDevice acquired.", "Expected basement power rewards.", start);
        }

        // Performs the Step Locked Room Final operation while keeping its implementation details inside this script.
        private IEnumerator Step_LockedRoomFinal()
        {
            const string stepName = "LockedRoom Final";
            const string puzzleId = "Puzzle_LockedRoom_01";
            float start = Time.realtimeSinceStartup;
            if (!HasItem("ModifiedClockworkDevice"))
            {
                AddFail(stepName, puzzleId, "Skipped due to missing ModifiedClockworkDevice.", Elapsed(start));
                yield break;
            }

            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleUI_LockedRoomFinal ui = FindCurrentPuzzleUi<PuzzleUI_LockedRoomFinal>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleUI_LockedRoomFinal was not found.", Elapsed(start));
                yield break;
            }

            SetFinalSymbolSequence(ui);
            ui.Submit();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            InventoryManager.Instance.SelectItem("ModifiedClockworkDevice");
            ui.UseRequiredItem();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool ok = IsPuzzleCompleted(puzzleId) && HasItem("FrontDoorKey") && IsFinalChaseStarted();
            AddProgressResult(ok, stepName, puzzleId, "FrontDoorKey acquired and finalChaseStarted is true.", "Expected FrontDoorKey and final chase state.", start);
        }

        // Performs the Step Entrance Ending operation while keeping its implementation details inside this script.
        private IEnumerator Step_EntranceEnding()
        {
            const string stepName = "Entrance Ending";
            const string puzzleId = "Puzzle_Entrance_01";
            float start = Time.realtimeSinceStartup;
            if (!HasItem("FrontDoorKey"))
            {
                AddFail(stepName, puzzleId, "Skipped due to missing FrontDoorKey.", Elapsed(start));
                yield break;
            }

            InventoryManager.Instance.SelectItem("FrontDoorKey");
            yield return OpenPuzzleAndWait(puzzleId);
            PuzzleItemUseUIBase ui = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
            if (ui == null)
            {
                AddFail(stepName, puzzleId, "PuzzleItemUseUIBase was not found.", Elapsed(start));
                yield break;
            }

            ui.UseSelectedItem();
            yield return new WaitForSeconds(waitAfterActionSeconds);
            bool ending = GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Ending;
            AddProgressResult(ending, stepName, puzzleId, "Ending state reached.", "Expected GameState.Ending.", start);
        }

        // Performs the Add Progress Result operation while keeping its implementation details inside this script.
        private void AddProgressResult(bool passed, string stepName, string targetId, string passMessage, string failMessage, float start)
        {
            if (passed)
            {
                AddPass(stepName, targetId, passMessage, Elapsed(start));
            }
            else
            {
                AddFail(stepName, targetId, failMessage, Elapsed(start));
            }
        }

        // Adds a formatted section, row, or detail line to a report or UI string builder.
        private void AppendDigits(PuzzleNumberCodeUIBase ui, string digits)
        {
            for (int i = 0; i < digits.Length; i++)
            {
                int digit;
                if (int.TryParse(digits[i].ToString(), out digit))
                {
                    ui.AppendDigit(digit);
                }
            }
        }

        // Performs the Input Basement Pattern operation while keeping its implementation details inside this script.
        private void InputBasementPattern(PuzzlePowerDeviceUIBase ui)
        {
            ui.InputSwitch("Switch_Left");
            ui.InputSwitch("Switch_Right");
            ui.InputSwitch("Switch_Center");
            ui.InputSwitch("Switch_Left");
            ui.InputSwitch("Switch_Right");
        }

        // Stores an incoming value and updates any dependent visual or runtime state.
        private bool SetFinalSymbolSequence(PuzzleSymbolCycleUIBase ui)
        {
            string[] sequence = { "Symbol_01", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" };
            for (int i = 0; i < sequence.Length; i++)
            {
                if (!ui.SetSlotSymbolForTest(i, sequence[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // Opens the requested puzzle, clue, screen, or navigation target for the player.
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
                Debug.LogWarning("PuzzleManager.OpenPuzzle did not produce a usable puzzle UI. Falling back to direct route test instantiate: " + puzzleId);
                DirectInstantiatePuzzleForTest(puzzleId);
                yield return new WaitForSeconds(waitAfterOpenSeconds);
            }
        }

        // Performs the Direct Instantiate Puzzle For Test operation while keeping its implementation details inside this script.
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

        // Closes the active UI or interaction and returns control to the normal game flow.
        private void ClosePuzzleIfOpen()
        {
            if (PuzzleManager.Instance != null && PuzzleManager.Instance.HasOpenPuzzle)
            {
                PuzzleManager.Instance.CloseCurrentPuzzle();
            }
        }

        // Performs the Backup Current Save operation while keeping its implementation details inside this script.
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
                originalSaveBackupPath = savePath + ".full_route_runtime_test_backup";
                originalSaveExisted = File.Exists(savePath);
                if (originalSaveExisted)
                {
                    File.Copy(savePath, originalSaveBackupPath, true);
                }

                saveBackupSucceeded = true;
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not back up save_data.json before full route test: " + exception.Message);
            }
        }

        // Performs the Restore Original Save operation while keeping its implementation details inside this script.
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
                Debug.LogWarning("Could not restore save_data.json after full route test: " + exception.Message);
            }
        }

        // Finds or creates a required reference so later logic can run without null setup errors.
        private bool EnsureRequiredManagers()
        {
            if (!allowTemporaryManagerBootstrap)
            {
                return GameDataManager.Instance != null && SaveManager.Instance != null && InventoryManager.Instance != null && PuzzleManager.Instance != null && ClueImageManager.Instance != null;
            }

            EnsureManager<GameManager>("FullRouteTest_GameManager");
            EnsureManager<GameDataManager>("FullRouteTest_GameDataManager");
            EnsureManager<SaveManager>("FullRouteTest_SaveManager");
            EnsureManager<InventoryManager>("FullRouteTest_InventoryManager");
            EnsureManager<PuzzleManager>("FullRouteTest_PuzzleManager");
            EnsureManager<ClueImageManager>("FullRouteTest_ClueImageManager");
            EnsureManager<PuzzleRetryLockManager>("FullRouteTest_PuzzleRetryLockManager");

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.LoadAllData();
            }

            if (PuzzleManager.Instance != null && PuzzleManager.Instance.puzzleUiRoot == null)
            {
                GameObject root = new GameObject("FullRouteTest_PuzzleUIRoot", typeof(RectTransform));
                createdRuntimeObjects.Add(root);
                PuzzleManager.Instance.puzzleUiRoot = root.transform;
            }

            return GameDataManager.Instance != null && SaveManager.Instance != null && InventoryManager.Instance != null && PuzzleManager.Instance != null && ClueImageManager.Instance != null;
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
            if (typeof(T) == typeof(PuzzleRetryLockManager) && PuzzleRetryLockManager.Instance != null) return PuzzleRetryLockManager.Instance as T;
            return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        }

        // Returns runtime state to its defaults for a new game, retry, or clean test run.
        private void ResetRuntimeStateForFullRoute()
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
            if (PuzzleRetryLockManager.Instance != null)
            {
                PuzzleRetryLockManager.Instance.ClearAllLocks();
            }
            ClosePuzzleIfOpen();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }
        }

        // Performs the Add Pass operation while keeping its implementation details inside this script.
        private void AddPass(string stepName, string targetId, string message, float duration)
        {
            AddStep(stepName, targetId, true, message, duration);
        }

        // Performs the Add Fail operation while keeping its implementation details inside this script.
        private void AddFail(string stepName, string targetId, string message, float duration)
        {
            Debug.LogError("[FullGameRouteRuntimeTestRunner] " + stepName + " failed. " + message);
            AddStep(stepName, targetId, false, message, duration);
        }

        // Performs the Add Step operation while keeping its implementation details inside this script.
        private void AddStep(string stepName, string targetId, bool passed, string message, float duration)
        {
            FullGameRouteTestStepResult result = new FullGameRouteTestStepResult();
            result.stepIndex = ++currentStepIndex;
            result.stepName = stepName;
            result.targetId = targetId;
            result.passed = passed;
            result.message = message;
            result.durationSeconds = duration;
            steps.Add(result);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsPuzzleCompleted(string puzzleId) { return SaveManager.Instance != null && SaveManager.Instance.IsPuzzleCompleted(puzzleId); }
        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsItemUsed(string itemId) { return SaveManager.Instance != null && SaveManager.Instance.IsItemUsed(itemId); }
        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsClueUnlocked(string clueId) { return (ClueImageManager.Instance != null && ClueImageManager.Instance.IsClueUnlocked(clueId)) || (SaveManager.Instance != null && SaveManager.Instance.IsClueUnlocked(clueId)); }
        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsDoorOpened(string doorId) { return SaveManager.Instance != null && SaveManager.Instance.IsDoorOpened(doorId); }
        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool IsFinalChaseStarted() { return SaveManager.Instance != null && SaveManager.Instance.IsFinalChaseStarted(); }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private bool HasItem(string itemId)
        {
            return (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId)) || (SaveManager.Instance != null && SaveManager.Instance.IsItemOwned(itemId));
        }

        // Writes validation or generation results to a report that can be inspected from the project files.
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
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# Full Game Route Runtime Test Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Active Scene: " + SceneManager.GetActiveScene().name);
                builder.AppendLine("- Total Steps: " + steps.Count);
                builder.AppendLine("- Passed: " + passed);
                builder.AppendLine("- Failed: " + (steps.Count - passed));
                builder.AppendLine();
                builder.AppendLine("## Route Summary");
                builder.AppendLine();
                builder.AppendLine("Bedroom -> LivingRoom ItemUse -> ChildRoom -> Study -> LivingRoom Symbol -> Kitchen -> BasementStorage -> LockedRoom -> Entrance -> Ending");
                builder.AppendLine();
                builder.AppendLine("## Step Results");
                builder.AppendLine();
                builder.AppendLine("| Step | Target ID | Result | Message | Duration |");
                builder.AppendLine("|---:|---|---|---|---:|");
                for (int i = 0; i < steps.Count; i++)
                {
                    FullGameRouteTestStepResult step = steps[i];
                    builder.Append("| ");
                    builder.Append(step.stepIndex);
                    builder.Append(". ");
                    builder.Append(EscapeMarkdown(step.stepName));
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(step.targetId));
                    builder.Append(" | ");
                    builder.Append(step.passed ? "Pass" : "Fail");
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(step.message));
                    builder.Append(" | ");
                    builder.Append(step.durationSeconds.ToString("0.00"));
                    builder.AppendLine("s |");
                }
                builder.AppendLine();
                builder.AppendLine("## Expected Progression State");
                builder.AppendLine();
                builder.AppendLine("| Stage | Expected State |");
                builder.AppendLine("|---|---|");
                builder.AppendLine("| Bedroom | OldDrawerKey |");
                builder.AppendLine("| LivingRoom ItemUse | SmallClockworkDevice |");
                builder.AppendLine("| ChildRoom | ChildRoomCardSymbolClueImage |");
                builder.AppendLine("| Study | StudyBookSymbolClueImage |");
                builder.AppendLine("| LivingRoom Symbol | KitchenCodeClueImage |");
                builder.AppendLine("| Kitchen | BasementFuse |");
                builder.AppendLine("| BasementStorage | Door_BasementStorage_LockedRoom, BasementClueImage, ModifiedClockworkDevice |");
                builder.AppendLine("| LockedRoom | FrontDoorKey, finalChaseStarted |");
                builder.AppendLine("| Entrance | GameState.Ending |");
                builder.AppendLine();
                builder.AppendLine("## Negative Gate Checks");
                builder.AppendLine();
                builder.AppendLine("| Gate | Result | Message |");
                builder.AppendLine("|---|---|---|");
                bool wroteGate = false;
                for (int i = 0; i < steps.Count; i++)
                {
                    FullGameRouteTestStepResult step = steps[i];
                    if (step != null && !string.IsNullOrEmpty(step.stepName) && step.stepName.StartsWith("Gate:", StringComparison.Ordinal))
                    {
                        wroteGate = true;
                        builder.Append("| ");
                        builder.Append(EscapeMarkdown(step.stepName));
                        builder.Append(" | ");
                        builder.Append(step.passed ? "Pass" : "Fail");
                        builder.Append(" | ");
                        builder.Append(EscapeMarkdown(step.message));
                        builder.AppendLine(" |");
                    }
                }
                if (!wroteGate)
                {
                    builder.AppendLine("| None | N/A | Negative gate checks were disabled. |");
                }
                builder.AppendLine();
                builder.AppendLine("## Save Protection");
                builder.AppendLine();
                builder.AppendLine("- save_data.json is backed up before test.");
                builder.AppendLine("- save_data.json is restored after test.");
                builder.AppendLine("- If no save existed before test, generated save_data.json is deleted.");
                builder.AppendLine();
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- This test does not validate final art.");
                builder.AppendLine("- This test does not validate manual Scene button click positions.");
                builder.AppendLine("- This test validates progression logic, rewards, unlocks, and ending state.");
                File.WriteAllText(path, builder.ToString());
                Debug.Log("Full game route runtime test report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not write full route runtime test report: " + exception.Message);
            }
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private string GetReportPath()
        {
            string relativePath = string.IsNullOrEmpty(reportRelativePath) ? "Docs/GeneratedFullGameRouteRuntimeTestReport.md" : reportRelativePath;
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relativePath);
        }

        // Queries current data or scene state and returns a value used by the caller's next branch.
        private int CountPassed()
        {
            int count = 0;
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] != null && steps[i].passed)
                {
                    count++;
                }
            }
            return count;
        }

        // Performs the Elapsed operation while keeping its implementation details inside this script.
        private float Elapsed(float start) { return Time.realtimeSinceStartup - start; }

        // Performs the Escape Markdown operation while keeping its implementation details inside this script.
        private string EscapeMarkdown(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }

        // Performs the Cleanup Runtime Objects operation while keeping its implementation details inside this script.
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
