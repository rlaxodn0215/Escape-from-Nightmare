using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EscapeFromNightmare
{
    public class GameSceneInteractionRuntimeTestRunner : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private float waitAfterClickSeconds = 0.1f;
        [SerializeField] private float waitAfterOpenSeconds = 0.1f;
        [SerializeField] private string reportRelativePath = "Docs/GeneratedGameSceneInteractionRuntimeTestReport.md";
        [SerializeField] private bool testDoorButtons = true;
        [SerializeField] private bool testPuzzleButtons = true;
        [SerializeField] private bool testClueButtons = true;
        [SerializeField] private bool testNavigationButtons = true;
        [SerializeField] private bool testHidePointButtons = true;
        [SerializeField] private bool testFinalDoorButton = true;
        [SerializeField] private bool runFullClickRoute = true;

        private static readonly string[] RequiredDoorIds =
        {
            "Door_Bedroom_SecondFloorHallway",
            "Door_SecondFloorHallway_Bedroom",
            "Door_SecondFloorHallway_ChildRoom",
            "Door_ChildRoom_SecondFloorHallway",
            "Door_SecondFloorHallway_Study",
            "Door_Study_SecondFloorHallway",
            "Door_SecondFloorHallway_LivingRoom",
            "Door_LivingRoom_SecondFloorHallway",
            "Door_LivingRoom_Kitchen",
            "Door_Kitchen_LivingRoom",
            "Door_Kitchen_BasementStorage",
            "Door_BasementStorage_Kitchen",
            "Door_BasementStorage_LockedRoom",
            "Door_LockedRoom_BasementStorage",
            "Door_LivingRoom_Entrance",
            "Door_Entrance_LivingRoom"
        };

        private static readonly string[] RequiredPuzzleIds =
        {
            "Puzzle_Bedroom_01",
            "Puzzle_LivingRoom_01",
            "Puzzle_ChildRoom_01",
            "Puzzle_Study_01",
            "Puzzle_LivingRoom_02",
            "Puzzle_Kitchen_01",
            "Puzzle_BasementStorage_01",
            "Puzzle_LockedRoom_01",
            "Puzzle_Entrance_01"
        };

        private static readonly string[] RecommendedClueIds =
        {
            "BedroomPhotoCodeClue",
            "LivingRoomEntranceCodeClue",
            "ChildRoomCardSymbolClueImage",
            "StudyBookSymbolClueImage",
            "KitchenCodeClueImage",
            "KitchenFridgeSurfaceSymbolClue",
            "BasementPowerPatternClue",
            "BasementClueImage"
        };

        private readonly List<GameSceneInteractionTestResult> results = new List<GameSceneInteractionTestResult>();
        private string originalSaveBackupPath;
        private bool originalSaveExisted;
        private bool saveBackupSucceeded;
        private int currentIndex;
        private bool originalReturnToTitleAfterEnding = true;
        private bool capturedEndingReturnSetting;

        private IEnumerator Start()
        {
            if (runOnStart)
            {
                yield return RunAllInteractionTests();
            }
        }

        public IEnumerator RunAllInteractionTests()
        {
            results.Clear();
            currentIndex = 0;
            saveBackupSucceeded = false;
            capturedEndingReturnSetting = false;

            if (!EnsureRequiredSceneObjects())
            {
                AddFail("Setup", "RequiredSceneObjects", "Required GameScene managers or UI roots are missing.", 0f);
                WriteMarkdownReport();
                yield break;
            }

            BackupCurrentSave();
            if (!saveBackupSucceeded)
            {
                AddFail("Setup", "SaveBackup", "Could not safely back up save_data.json. Interaction tests aborted.", 0f);
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

                ResetRuntimeStateForInteractionTests();

                if (testNavigationButtons)
                {
                    yield return TestNavigationButtons();
                }

                if (testDoorButtons)
                {
                    yield return TestDoorButtons();
                }

                if (testPuzzleButtons)
                {
                    yield return TestPuzzleButtons();
                }

                if (testClueButtons)
                {
                    yield return TestClueButtons();
                }

                if (testHidePointButtons)
                {
                    yield return TestHidePointButtons();
                }

                if (testFinalDoorButton)
                {
                    yield return TestFinalDoorButton();
                }

                if (runFullClickRoute)
                {
                    yield return TestFullSceneClickRoute();
                }
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

                int passed = CountPassed();
                Debug.Log("GameScene interaction runtime tests completed. Passed: " + passed + ", Failed: " + (results.Count - passed));
            }
        }

        private bool EnsureRequiredSceneObjects()
        {
            bool ok = true;
            ok &= RequireManager(GameManager.Instance, "GameManager");
            ok &= RequireManager(GameDataManager.Instance, "GameDataManager");
            ok &= RequireManager(SaveManager.Instance, "SaveManager");
            ok &= RequireManager(LocationManager.Instance, "LocationManager");
            ok &= RequireManager(InteractionManager.Instance, "InteractionManager");
            ok &= RequireManager(InventoryManager.Instance, "InventoryManager");
            ok &= RequireManager(PuzzleManager.Instance, "PuzzleManager");
            ok &= RequireManager(ClueImageManager.Instance, "ClueImageManager");
            ok &= RequireManager(HideManager.Instance, "HideManager");
            ok &= RequireManager(EndingManager.Instance, "EndingManager");

            if (PuzzleManager.Instance != null && PuzzleManager.Instance.puzzleUiRoot == null)
            {
                Debug.LogError("PuzzleManager.puzzleUiRoot is missing.");
                ok = false;
            }

            if (FindFirstSceneObject<ClueImagePanelUI>() == null)
            {
                Debug.LogError("ClueImagePanelUI is missing.");
                ok = false;
            }

            if (FindFirstSceneObject<InventoryBarUI>() == null)
            {
                Debug.LogError("InventoryBarUI is missing.");
                ok = false;
            }

            return ok;
        }

        private bool RequireManager(UnityEngine.Object manager, string managerName)
        {
            if (manager != null)
            {
                return true;
            }

            Debug.LogError(managerName + " is missing from the active scene/runtime.");
            return false;
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
                originalSaveBackupPath = savePath + ".interaction_runtime_test_backup";
                originalSaveExisted = File.Exists(savePath);
                if (originalSaveExisted)
                {
                    File.Copy(savePath, originalSaveBackupPath, true);
                }

                saveBackupSucceeded = true;
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not back up save_data.json before interaction test: " + exception.Message);
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
                Debug.LogError("Could not restore save_data.json after interaction test: " + exception.Message);
            }
        }

        private void ResetRuntimeStateForInteractionTests()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ResetDataForNewGame();
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ReplaceInventory(new string[0]);
                InventoryManager.Instance.ClearSelectedItem();
            }

            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.HideCurrentImage();
                ClueImageManager.Instance.LoadUnlockedCluesFromSave();
            }

            if (PuzzleRetryLockManager.Instance != null)
            {
                PuzzleRetryLockManager.Instance.ClearAllLocks();
            }

            if (HideManager.Instance != null && HideManager.Instance.IsHiding)
            {
                HideManager.Instance.ForceExitHidePoint();
            }

            ClosePuzzleIfOpen();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }

            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.SetReturnToTitleAfterEnding(false);
            }

            if (LocationManager.Instance != null)
            {
                LocationManager.Instance.SetLocation("Bedroom", "Bedroom_Front");
            }
        }

        private IEnumerator TestNavigationButtons()
        {
            const string category = "Navigation";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForInteractionTests();

            NavigationButton rotateRight = FindNavigationButton(NavigationActionType.RotateRight);
            NavigationButton rotateLeft = FindNavigationButton(NavigationActionType.RotateLeft);
            if (rotateRight == null || rotateLeft == null)
            {
                AddFail(category, "RotateLeft/RotateRight", "RotateLeft or RotateRight NavigationButton is missing.", Elapsed(start));
                yield break;
            }

            LocationManager.Instance.SetLocation("Bedroom", "Bedroom_Front");
            yield return ClickButtonAndWait(GetButtonForNavigation(rotateRight));
            string afterRight = LocationManager.Instance.CurrentViewId;
            bool rightOk = afterRight == "Bedroom_Right";

            yield return ClickButtonAndWait(GetButtonForNavigation(rotateLeft));
            string afterLeft = LocationManager.Instance.CurrentViewId;
            bool leftOk = afterLeft == "Bedroom_Front";

            if (rightOk && leftOk)
            {
                AddPass(category, "RotateLeft/RotateRight", "RotateRight moved to Bedroom_Right and RotateLeft returned to Bedroom_Front.", Elapsed(start));
            }
            else
            {
                AddFail(category, "RotateLeft/RotateRight", "Unexpected views. After right: " + afterRight + ", after left: " + afterLeft, Elapsed(start));
            }
        }

        private IEnumerator TestDoorButtons()
        {
            const string category = "Door Buttons";
            for (int i = 0; i < RequiredDoorIds.Length; i++)
            {
                ResetRuntimeStateForInteractionTests();
                yield return TestDoorButton(category, RequiredDoorIds[i]);
            }
        }

        private IEnumerator TestDoorButton(string category, string doorId)
        {
            float start = Time.realtimeSinceStartup;
            DoorRecord door = GameDataManager.Instance != null ? GameDataManager.Instance.GetDoorById(doorId) : null;
            if (door == null)
            {
                AddFail(category, doorId, "DoorRecord was not found.", Elapsed(start));
                yield break;
            }

            ClickableButton clickable = FindClickableByDoorId(doorId);
            if (clickable == null)
            {
                AddFail(category, doorId, "Door ClickableButton was not found.", Elapsed(start));
                yield break;
            }

            Button button = GetButtonForClickable(clickable);
            if (button == null)
            {
                AddFail(category, doorId, "Door Button component was not found at " + GetHierarchyPath(clickable.gameObject), Elapsed(start));
                yield break;
            }

            if (!string.IsNullOrEmpty(door.fromLocationId))
            {
                LocationManager.Instance.SetLocation(door.fromLocationId, door.fromViewId);
            }
            else
            {
                ActivateClickableContext(clickable);
            }

            SatisfyDoorRequirements(door);
            yield return ClickButtonAndWait(button);

            bool locationOk = LocationManager.Instance.CurrentLocationId == door.toLocationId;
            bool viewOk = string.IsNullOrEmpty(door.toViewId) || LocationManager.Instance.CurrentViewId == door.toViewId;
            if (locationOk && viewOk)
            {
                AddPass(category, doorId, "Door click moved to " + LocationManager.Instance.CurrentLocationId + "/" + LocationManager.Instance.CurrentViewId + ".", Elapsed(start));
            }
            else
            {
                AddFail(category, doorId, "Expected " + door.toLocationId + "/" + door.toViewId + " but got " + LocationManager.Instance.CurrentLocationId + "/" + LocationManager.Instance.CurrentViewId + ".", Elapsed(start));
            }
        }

        private IEnumerator TestPuzzleButtons()
        {
            const string category = "Puzzle Buttons";
            for (int i = 0; i < RequiredPuzzleIds.Length; i++)
            {
                ResetRuntimeStateForInteractionTests();
                yield return TestPuzzleButtonOpen(category, RequiredPuzzleIds[i]);
            }
        }

        private IEnumerator TestPuzzleButtonOpen(string category, string puzzleId)
        {
            float start = Time.realtimeSinceStartup;
            PuzzleRecord puzzle = GameDataManager.Instance != null ? GameDataManager.Instance.GetPuzzleById(puzzleId) : null;
            if (puzzle == null)
            {
                AddFail(category, puzzleId, "PuzzleRecord was not found.", Elapsed(start));
                yield break;
            }

            ClickableButton clickable = FindClickableByPuzzleId(puzzleId);
            if (clickable == null)
            {
                AddFail(category, puzzleId, "Puzzle ClickableButton was not found.", Elapsed(start));
                yield break;
            }

            Button button = GetButtonForClickable(clickable);
            if (button == null)
            {
                AddFail(category, puzzleId, "Puzzle Button component was not found at " + GetHierarchyPath(clickable.gameObject), Elapsed(start));
                yield break;
            }

            SatisfyPuzzleRequirements(puzzle);
            ActivateClickableContext(clickable);
            yield return ClickButtonAndWait(button);
            yield return new WaitForSeconds(waitAfterOpenSeconds);

            PuzzleUIBase ui = FindCurrentPuzzleUi<PuzzleUIBase>();
            bool opened = PuzzleManager.Instance != null
                && PuzzleManager.Instance.HasOpenPuzzle
                && PuzzleManager.Instance.CurrentPuzzle != null
                && PuzzleManager.Instance.CurrentPuzzle.puzzleId == puzzleId
                && ui != null;

            if (opened)
            {
                AddPass(category, puzzleId, "Scene button opened puzzle UI: " + ui.GetType().Name + ".", Elapsed(start));
            }
            else
            {
                AddFail(category, puzzleId, "Scene button did not open the expected puzzle UI.", Elapsed(start));
            }

            ClosePuzzleIfOpen();
        }

        private IEnumerator TestClueButtons()
        {
            const string category = "ExamineImage Buttons";
            for (int i = 0; i < RecommendedClueIds.Length; i++)
            {
                ResetRuntimeStateForInteractionTests();
                yield return TestClueButton(category, RecommendedClueIds[i]);
            }
        }

        private IEnumerator TestClueButton(string category, string clueId)
        {
            float start = Time.realtimeSinceStartup;
            ClickableButton clickable = FindClickableByClueId(clueId);
            if (clickable == null)
            {
                AddFail(category, clueId, "ExamineImage ClickableButton was not found.", Elapsed(start));
                yield break;
            }

            Button button = GetButtonForClickable(clickable);
            if (button == null)
            {
                AddFail(category, clueId, "Button component was not found at " + GetHierarchyPath(clickable.gameObject), Elapsed(start));
                yield break;
            }

            ActivateClickableContext(clickable);
            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.UnlockClue(clueId);
            }

            yield return ClickButtonAndWait(button);
            ClueImagePanelUI panel = FindFirstSceneObject<ClueImagePanelUI>();
            bool visible = panel != null && panel.IsVisible;
            if (ClueImageManager.Instance != null)
            {
                ClueImageManager.Instance.HideCurrentImage();
            }

            if (visible)
            {
                AddPass(category, clueId, "Clue panel opened from scene button.", Elapsed(start));
            }
            else
            {
                AddFail(category, clueId, "Clue panel did not become visible.", Elapsed(start));
            }
        }

        private IEnumerator TestHidePointButtons()
        {
            const string category = "HidePoint Buttons";
            ClickableButton[] hideButtons = FindClickables(ClickableType.HidePoint);
            int passBefore = CountPassed(category);
            if (hideButtons.Length == 0)
            {
                AddFail(category, "HidePoint", "No HidePoint ClickableButton was found.", 0f);
                yield break;
            }

            for (int i = 0; i < hideButtons.Length; i++)
            {
                ResetRuntimeStateForInteractionTests();
                ClickableButton clickable = hideButtons[i];
                float start = Time.realtimeSinceStartup;
                string targetId = !string.IsNullOrEmpty(clickable.TargetObjectId) ? clickable.TargetObjectId : clickable.ClickableId;
                Button button = GetButtonForClickable(clickable);
                if (button == null)
                {
                    AddFail(category, targetId, "Button component was not found at " + GetHierarchyPath(clickable.gameObject), Elapsed(start));
                    continue;
                }

                ActivateClickableContext(clickable);
                yield return ClickButtonAndWait(button);
                bool entered = HideManager.Instance != null && HideManager.Instance.IsHiding;
                if (HideManager.Instance != null)
                {
                    HideManager.Instance.ForceExitHidePoint();
                }
                bool exited = HideManager.Instance != null && !HideManager.Instance.IsHiding;

                if (entered && exited)
                {
                    AddPass(category, targetId, "HidePoint entered and ForceExitHidePoint restored state.", Elapsed(start));
                }
                else
                {
                    AddFail(category, targetId, "HidePoint did not enter/exit correctly. Entered=" + entered + ", Exited=" + exited, Elapsed(start));
                }
            }

            if (CountPassed(category) == passBefore)
            {
                AddFail(category, "HidePointSummary", "No HidePoint interaction passed.", 0f);
            }
        }

        private IEnumerator TestFinalDoorButton()
        {
            const string category = "FinalDoor";
            const string puzzleId = "Puzzle_Entrance_01";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForInteractionTests();
            ClickableButton finalDoor = FindFinalDoorButton();
            if (finalDoor == null)
            {
                AddFail(category, "FinalDoor_FrontDoorKey", "FinalDoor ClickableButton was not found.", Elapsed(start));
                yield break;
            }

            InventoryManager.Instance.TryAddItem("FrontDoorKey");
            SelectItemIfNeeded("FrontDoorKey");
            LocationManager.Instance.SetLocation("Entrance", "Entrance_Front");
            yield return ClickButtonAndWait(GetButtonForClickable(finalDoor));
            yield return new WaitForSeconds(waitAfterOpenSeconds);

            bool ending = IsEndingState();
            if (!ending && PuzzleManager.Instance != null && PuzzleManager.Instance.HasOpenPuzzle && PuzzleManager.Instance.CurrentPuzzle != null && PuzzleManager.Instance.CurrentPuzzle.puzzleId == puzzleId)
            {
                PuzzleItemUseUIBase ui = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
                if (ui != null)
                {
                    SelectItemIfNeeded("FrontDoorKey");
                    ui.UseSelectedItem();
                    yield return new WaitForSeconds(waitAfterClickSeconds);
                }

                ending = IsEndingState();
            }

            if (ending)
            {
                AddPass(category, "FinalDoor_FrontDoorKey", "FinalDoor reached Ending through scene button flow.", Elapsed(start));
            }
            else
            {
                AddFail(category, "FinalDoor_FrontDoorKey", "FinalDoor did not open Entrance puzzle or reach Ending.", Elapsed(start));
            }
        }

        private IEnumerator TestFullSceneClickRoute()
        {
            const string category = "Full Scene Click Route";
            float start = Time.realtimeSinceStartup;
            ResetRuntimeStateForInteractionTests();

            bool ok = true;
            ok &= yieldReturnCompatible(true);

            yield return CompletePuzzleFromSceneButton("Puzzle_Bedroom_01");
            ok &= HasItem("OldDrawerKey") && IsPuzzleCompleted("Puzzle_Bedroom_01");

            yield return ClickDoorAndWait("Door_Bedroom_SecondFloorHallway");
            ok &= LocationManager.Instance.CurrentLocationId == "SecondFloorHallway";

            yield return ClickDoorAndWait("Door_SecondFloorHallway_LivingRoom");
            ok &= LocationManager.Instance.CurrentLocationId == "LivingRoom";

            SelectItemIfNeeded("OldDrawerKey");
            yield return CompletePuzzleFromSceneButton("Puzzle_LivingRoom_01");
            ok &= HasItem("SmallClockworkDevice") && IsPuzzleCompleted("Puzzle_LivingRoom_01");

            yield return ClickDoorAndWait("Door_LivingRoom_SecondFloorHallway");
            yield return ClickDoorAndWait("Door_SecondFloorHallway_ChildRoom");
            ok &= LocationManager.Instance.CurrentLocationId == "ChildRoom";

            yield return CompletePuzzleFromSceneButton("Puzzle_ChildRoom_01");
            ok &= IsClueUnlocked("ChildRoomCardSymbolClueImage");

            yield return ClickDoorAndWait("Door_ChildRoom_SecondFloorHallway");
            yield return ClickDoorAndWait("Door_SecondFloorHallway_Study");
            ok &= LocationManager.Instance.CurrentLocationId == "Study";

            yield return CompletePuzzleFromSceneButton("Puzzle_Study_01");
            ok &= IsClueUnlocked("StudyBookSymbolClueImage");

            yield return ClickDoorAndWait("Door_Study_SecondFloorHallway");
            yield return ClickDoorAndWait("Door_SecondFloorHallway_LivingRoom");
            ok &= LocationManager.Instance.CurrentLocationId == "LivingRoom";

            yield return CompletePuzzleFromSceneButton("Puzzle_LivingRoom_02");
            ok &= IsClueUnlocked("KitchenCodeClueImage");

            yield return ClickDoorAndWait("Door_LivingRoom_Kitchen");
            ok &= LocationManager.Instance.CurrentLocationId == "Kitchen";

            yield return CompletePuzzleFromSceneButton("Puzzle_Kitchen_01");
            ok &= HasItem("BasementFuse") && !HasItem("FrontDoorKey");

            yield return ClickDoorAndWait("Door_Kitchen_BasementStorage");
            ok &= LocationManager.Instance.CurrentLocationId == "BasementStorage";

            yield return CompletePuzzleFromSceneButton("Puzzle_BasementStorage_01");
            ok &= IsDoorOpened("Door_BasementStorage_LockedRoom") && IsClueUnlocked("BasementClueImage") && HasItem("ModifiedClockworkDevice");

            yield return ClickDoorAndWait("Door_BasementStorage_LockedRoom");
            ok &= LocationManager.Instance.CurrentLocationId == "LockedRoom";

            yield return CompletePuzzleFromSceneButton("Puzzle_LockedRoom_01");
            ok &= HasItem("FrontDoorKey") && IsFinalChaseStarted();

            yield return ClickDoorAndWait("Door_LockedRoom_BasementStorage");
            yield return ClickDoorAndWait("Door_BasementStorage_Kitchen");
            yield return ClickDoorAndWait("Door_Kitchen_LivingRoom");
            SelectItemIfNeeded("FrontDoorKey");
            yield return ClickDoorAndWait("Door_LivingRoom_Entrance");
            ok &= LocationManager.Instance.CurrentLocationId == "Entrance";

            SelectItemIfNeeded("FrontDoorKey");
            ClickableButton finalDoor = FindFinalDoorButton();
            if (finalDoor != null)
            {
                yield return ClickButtonAndWait(GetButtonForClickable(finalDoor));
                yield return new WaitForSeconds(waitAfterOpenSeconds);
                PuzzleItemUseUIBase entranceUi = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
                if (entranceUi != null)
                {
                    SelectItemIfNeeded("FrontDoorKey");
                    entranceUi.UseSelectedItem();
                    yield return new WaitForSeconds(waitAfterClickSeconds);
                }
            }
            else
            {
                yield return CompletePuzzleFromSceneButton("Puzzle_Entrance_01");
            }

            ok &= IsEndingState();

            if (ok)
            {
                AddPass(category, "BedroomToEnding", "Scene buttons completed the full route and reached Ending.", Elapsed(start));
            }
            else
            {
                AddFail(category, "BedroomToEnding", "Full scene click route did not satisfy every expected progression state.", Elapsed(start));
            }
        }

        private bool yieldReturnCompatible(bool value)
        {
            return value;
        }

        private IEnumerator CompletePuzzleFromSceneButton(string puzzleId)
        {
            PuzzleRecord puzzle = GameDataManager.Instance.GetPuzzleById(puzzleId);
            if (puzzle == null)
            {
                yield break;
            }

            SatisfyPuzzleRequirements(puzzle);
            ClickableButton clickable = FindClickableByPuzzleId(puzzleId);
            if (clickable == null)
            {
                yield break;
            }

            ActivateClickableContext(clickable);
            yield return ClickButtonAndWait(GetButtonForClickable(clickable));
            yield return new WaitForSeconds(waitAfterOpenSeconds);

            switch (puzzleId)
            {
                case "Puzzle_Bedroom_01":
                    PuzzleNumberCodeUIBase bedroom = FindCurrentPuzzleUi<PuzzleNumberCodeUIBase>();
                    if (bedroom != null)
                    {
                        AppendDigits(bedroom, "7319");
                        bedroom.Submit();
                    }
                    break;
                case "Puzzle_Kitchen_01":
                    PuzzleNumberCodeUIBase kitchen = FindCurrentPuzzleUi<PuzzleNumberCodeUIBase>();
                    if (kitchen != null)
                    {
                        AppendDigits(kitchen, "4826");
                        kitchen.Submit();
                    }
                    break;
                case "Puzzle_ChildRoom_01":
                    CompleteSequence(new[] { "Symbol_01", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" });
                    break;
                case "Puzzle_Study_01":
                    CompleteSequence(new[] { "Symbol_01", "Symbol_02", "Symbol_03", "Symbol_04", "Symbol_05", "Symbol_06" });
                    break;
                case "Puzzle_LivingRoom_02":
                    CompleteSymbolCycle();
                    break;
                case "Puzzle_LivingRoom_01":
                    PuzzleItemUseUIBase itemUse = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
                    if (itemUse != null)
                    {
                        itemUse.UseSelectedItem();
                    }
                    break;
                case "Puzzle_BasementStorage_01":
                    PuzzlePowerDeviceUIBase power = FindCurrentPuzzleUi<PuzzlePowerDeviceUIBase>();
                    if (power != null)
                    {
                        power.InputSwitch("Switch_Left");
                        power.InputSwitch("Switch_Right");
                        power.InputSwitch("Switch_Center");
                        power.InputSwitch("Switch_Left");
                        power.InputSwitch("Switch_Right");
                        power.PressPowerButton();
                    }
                    break;
                case "Puzzle_LockedRoom_01":
                    PuzzleUI_LockedRoomFinal lockedRoom = FindCurrentPuzzleUi<PuzzleUI_LockedRoomFinal>();
                    if (lockedRoom != null)
                    {
                        SetSymbolSequence(lockedRoom);
                        lockedRoom.Submit();
                        SelectItemIfNeeded("ModifiedClockworkDevice");
                        lockedRoom.UseRequiredItem();
                    }
                    break;
                case "Puzzle_Entrance_01":
                    PuzzleItemUseUIBase entrance = FindCurrentPuzzleUi<PuzzleItemUseUIBase>();
                    if (entrance != null)
                    {
                        entrance.UseSelectedItem();
                    }
                    break;
            }

            yield return new WaitForSeconds(waitAfterClickSeconds);
        }

        private void CompleteSequence(string[] sequence)
        {
            PuzzleSequenceUIBase ui = FindCurrentPuzzleUi<PuzzleSequenceUIBase>();
            if (ui == null)
            {
                return;
            }

            for (int i = 0; i < sequence.Length; i++)
            {
                ui.SelectOption(sequence[i]);
            }

            if (PuzzleManager.Instance != null && PuzzleManager.Instance.HasOpenPuzzle)
            {
                ui.Submit();
            }
        }

        private void CompleteSymbolCycle()
        {
            PuzzleSymbolCycleUIBase ui = FindCurrentPuzzleUi<PuzzleSymbolCycleUIBase>();
            if (ui == null)
            {
                return;
            }

            SetSymbolSequence(ui);
            ui.Submit();
        }

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

        private bool SetSymbolSequence(PuzzleSymbolCycleUIBase ui)
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

        private IEnumerator ClickDoorAndWait(string doorId)
        {
            DoorRecord door = GameDataManager.Instance != null ? GameDataManager.Instance.GetDoorById(doorId) : null;
            ClickableButton clickable = FindClickableByDoorId(doorId);
            if (door != null)
            {
                SatisfyDoorRequirements(door);
            }

            if (clickable != null)
            {
                ActivateClickableContext(clickable);
                yield return ClickButtonAndWait(GetButtonForClickable(clickable));
            }

            yield return new WaitForSeconds(waitAfterClickSeconds);
        }

        private IEnumerator ClickButtonAndWait(Button button)
        {
            if (button == null)
            {
                yield break;
            }

            if (!button.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Button is inactive before click: " + GetHierarchyPath(button.gameObject));
            }

            if (!button.interactable)
            {
                Debug.LogWarning("Button is not interactable: " + GetHierarchyPath(button.gameObject));
            }

            button.onClick.Invoke();
            yield return new WaitForSeconds(waitAfterClickSeconds);
        }

        private Button GetButtonForClickable(ClickableButton clickable)
        {
            return clickable != null ? clickable.GetComponent<Button>() : null;
        }

        private Button GetButtonForNavigation(NavigationButton navigationButton)
        {
            return navigationButton != null ? navigationButton.GetComponent<Button>() : null;
        }

        private ClickableButton FindClickableByDoorId(string doorId)
        {
            ClickableButton[] clickables = FindClickables(ClickableType.Door);
            for (int i = 0; i < clickables.Length; i++)
            {
                if (clickables[i] != null && clickables[i].LinkedDoorId == doorId)
                {
                    return clickables[i];
                }
            }

            return null;
        }

        private ClickableButton FindClickableByPuzzleId(string puzzleId)
        {
            ClickableButton[] clickables = FindClickables(ClickableType.Puzzle);
            for (int i = 0; i < clickables.Length; i++)
            {
                if (clickables[i] != null && clickables[i].LinkedPuzzleId == puzzleId)
                {
                    return clickables[i];
                }
            }

            return null;
        }

        private ClickableButton FindClickableByClueId(string clueId)
        {
            ClickableButton[] clickables = FindClickables(ClickableType.ExamineImage);
            for (int i = 0; i < clickables.Length; i++)
            {
                if (clickables[i] == null)
                {
                    continue;
                }

                if (clickables[i].LinkedClueImageId == clueId || clickables[i].TargetObjectId == clueId || clickables[i].ClickableId == clueId)
                {
                    return clickables[i];
                }
            }

            return null;
        }

        private ClickableButton FindFinalDoorButton()
        {
            ClickableButton[] clickables = FindClickables(ClickableType.FinalDoor);
            for (int i = 0; i < clickables.Length; i++)
            {
                if (clickables[i] != null && clickables[i].RequiredItemId == "FrontDoorKey")
                {
                    return clickables[i];
                }
            }

            return clickables.Length > 0 ? clickables[0] : null;
        }

        private NavigationButton FindNavigationButton(NavigationActionType actionType)
        {
            NavigationButton[] buttons = Resources.FindObjectsOfTypeAll<NavigationButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (IsSceneObject(buttons[i]) && buttons[i].ActionType == actionType)
                {
                    return buttons[i];
                }
            }

            return null;
        }

        private ClickableButton[] FindClickables(ClickableType clickableType)
        {
            List<ClickableButton> result = new List<ClickableButton>();
            ClickableButton[] clickables = Resources.FindObjectsOfTypeAll<ClickableButton>();
            for (int i = 0; i < clickables.Length; i++)
            {
                if (IsSceneObject(clickables[i]) && clickables[i].ClickableType == clickableType)
                {
                    result.Add(clickables[i]);
                }
            }

            return result.ToArray();
        }

        private T FindFirstSceneObject<T>() where T : Component
        {
            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]))
                {
                    return objects[i];
                }
            }

            return null;
        }

        private bool IsSceneObject(Component component)
        {
            return component != null && component.gameObject != null && component.gameObject.scene.IsValid();
        }

        private void ActivateClickableContext(ClickableButton clickable)
        {
            if (clickable == null || LocationManager.Instance == null)
            {
                return;
            }

            LocationView view = clickable.GetComponentInParent<LocationView>(true);
            LocationController location = clickable.GetComponentInParent<LocationController>(true);
            if (location != null)
            {
                string viewId = view != null ? view.ViewId : location.DefaultViewId;
                LocationManager.Instance.SetLocation(location.LocationId, viewId);
                return;
            }

            if (view != null)
            {
                LocationManager.Instance.SetView(view.ViewId);
            }
        }

        private void SatisfyDoorRequirements(DoorRecord door)
        {
            if (door == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(door.requiredPuzzleId) && SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkPuzzleCompleted(door.requiredPuzzleId);
            }

            if (!string.IsNullOrEmpty(door.requiredItemId) && InventoryManager.Instance != null)
            {
                InventoryManager.Instance.TryAddItem(door.requiredItemId);
                SelectItemIfNeeded(door.requiredItemId);
            }
        }

        private void SatisfyPuzzleRequirements(PuzzleRecord puzzle)
        {
            if (puzzle == null || InventoryManager.Instance == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(puzzle.requiredItemId))
            {
                InventoryManager.Instance.TryAddItem(puzzle.requiredItemId);
                SelectItemIfNeeded(puzzle.requiredItemId);
            }

            if (puzzle.puzzleId == "Puzzle_BasementStorage_01")
            {
                InventoryManager.Instance.TryAddItem("BasementFuse");
                InventoryManager.Instance.TryAddItem("SmallClockworkDevice");
                SelectItemIfNeeded("BasementFuse");
            }
            else if (puzzle.puzzleId == "Puzzle_LockedRoom_01")
            {
                InventoryManager.Instance.TryAddItem("ModifiedClockworkDevice");
                SelectItemIfNeeded("ModifiedClockworkDevice");
            }
            else if (puzzle.puzzleId == "Puzzle_Entrance_01")
            {
                InventoryManager.Instance.TryAddItem("FrontDoorKey");
                SelectItemIfNeeded("FrontDoorKey");
            }
        }

        private void SelectItemIfNeeded(string itemId)
        {
            if (InventoryManager.Instance == null || string.IsNullOrEmpty(itemId))
            {
                return;
            }

            if (InventoryManager.Instance.SelectedItemId == itemId)
            {
                return;
            }

            InventoryManager.Instance.SelectItem(itemId);
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

            T[] objects = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (IsSceneObject(objects[i]))
                {
                    return objects[i];
                }
            }

            return null;
        }

        private void ClosePuzzleIfOpen()
        {
            if (PuzzleManager.Instance != null && PuzzleManager.Instance.HasOpenPuzzle)
            {
                PuzzleManager.Instance.CloseCurrentPuzzle();
            }
        }

        private bool IsPuzzleCompleted(string puzzleId)
        {
            return SaveManager.Instance != null && SaveManager.Instance.IsPuzzleCompleted(puzzleId);
        }

        private bool HasItem(string itemId)
        {
            return (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId))
                || (SaveManager.Instance != null && SaveManager.Instance.IsItemOwned(itemId));
        }

        private bool IsClueUnlocked(string clueId)
        {
            return (ClueImageManager.Instance != null && ClueImageManager.Instance.IsClueUnlocked(clueId))
                || (SaveManager.Instance != null && SaveManager.Instance.IsClueUnlocked(clueId));
        }

        private bool IsDoorOpened(string doorId)
        {
            return SaveManager.Instance != null && SaveManager.Instance.IsDoorOpened(doorId);
        }

        private bool IsFinalChaseStarted()
        {
            return SaveManager.Instance != null && SaveManager.Instance.IsFinalChaseStarted();
        }

        private bool IsEndingState()
        {
            return GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Ending;
        }

        private void AddPass(string category, string targetId, string message, float duration)
        {
            AddResult(category, targetId, true, message, duration);
        }

        private void AddFail(string category, string targetId, string message, float duration)
        {
            Debug.LogError("[GameSceneInteractionRuntimeTestRunner] " + category + " failed. Target: " + targetId + ". " + message);
            AddResult(category, targetId, false, message, duration);
        }

        private void AddResult(string category, string targetId, bool passed, string message, float duration)
        {
            GameSceneInteractionTestResult result = new GameSceneInteractionTestResult();
            result.index = ++currentIndex;
            result.category = category;
            result.targetId = targetId;
            result.passed = passed;
            result.message = message;
            result.durationSeconds = duration;
            results.Add(result);
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

        private int CountPassed(string category)
        {
            int count = 0;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i] != null && results[i].passed && results[i].category == category)
                {
                    count++;
                }
            }

            return count;
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
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("# GameScene Interaction Runtime Test Report");
                builder.AppendLine();
                builder.AppendLine("- Generated At: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendLine("- Active Scene: " + SceneManager.GetActiveScene().name);
                builder.AppendLine("- Total: " + results.Count);
                builder.AppendLine("- Passed: " + passed);
                builder.AppendLine("- Failed: " + (results.Count - passed));
                builder.AppendLine();
                builder.AppendLine("## Summary");
                builder.AppendLine();
                builder.AppendLine("| Category | Passed | Failed |");
                builder.AppendLine("|---|---:|---:|");
                AppendCategorySummary(builder, "Navigation");
                AppendCategorySummary(builder, "Door Buttons");
                AppendCategorySummary(builder, "Puzzle Buttons");
                AppendCategorySummary(builder, "ExamineImage Buttons");
                AppendCategorySummary(builder, "HidePoint Buttons");
                AppendCategorySummary(builder, "FinalDoor");
                AppendCategorySummary(builder, "Full Scene Click Route");
                builder.AppendLine();
                builder.AppendLine("## Results");
                builder.AppendLine();
                builder.AppendLine("| # | Category | Target ID | Result | Message | Duration |");
                builder.AppendLine("|---:|---|---|---|---|---:|");
                for (int i = 0; i < results.Count; i++)
                {
                    GameSceneInteractionTestResult result = results[i];
                    builder.Append("| ");
                    builder.Append(result.index);
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(result.category));
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(result.targetId));
                    builder.Append(" | ");
                    builder.Append(result.passed ? "Pass" : "Fail");
                    builder.Append(" | ");
                    builder.Append(EscapeMarkdown(result.message));
                    builder.Append(" | ");
                    builder.Append(result.durationSeconds.ToString("0.00"));
                    builder.AppendLine("s |");
                }

                builder.AppendLine();
                builder.AppendLine("## Failed Interactions");
                builder.AppendLine();
                bool wroteFailure = false;
                for (int i = 0; i < results.Count; i++)
                {
                    GameSceneInteractionTestResult result = results[i];
                    if (result != null && !result.passed)
                    {
                        wroteFailure = true;
                        builder.AppendLine("- " + EscapeMarkdown(result.category) + " / " + EscapeMarkdown(result.targetId) + ": " + EscapeMarkdown(result.message));
                    }
                }
                if (!wroteFailure)
                {
                    builder.AppendLine("- None");
                }

                builder.AppendLine();
                builder.AppendLine("## Notes");
                builder.AppendLine();
                builder.AppendLine("- This test invokes actual Scene Button.onClick listeners.");
                builder.AppendLine("- This test verifies ClickableButton / NavigationButton wiring.");
                builder.AppendLine("- This test does not validate final visual design.");
                builder.AppendLine("- This test does not require final Sprite or Audio assets.");

                File.WriteAllText(path, builder.ToString());
                Debug.Log("GameScene interaction runtime test report written: " + path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not write GameScene interaction runtime test report: " + exception.Message);
            }
        }

        private void AppendCategorySummary(StringBuilder builder, string category)
        {
            int passed = 0;
            int failed = 0;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i] == null || results[i].category != category)
                {
                    continue;
                }

                if (results[i].passed)
                {
                    passed++;
                }
                else
                {
                    failed++;
                }
            }

            builder.Append("| ");
            builder.Append(category);
            builder.Append(" | ");
            builder.Append(passed);
            builder.Append(" | ");
            builder.Append(failed);
            builder.AppendLine(" |");
        }

        private string GetReportPath()
        {
            string relativePath = string.IsNullOrEmpty(reportRelativePath) ? "Docs/GeneratedGameSceneInteractionRuntimeTestReport.md" : reportRelativePath;
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relativePath);
        }

        private float Elapsed(float start)
        {
            return Time.realtimeSinceStartup - start;
        }

        private string EscapeMarkdown(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }

        private string GetHierarchyPath(GameObject go)
        {
            if (go == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(go.name);
            Transform current = go.transform.parent;
            while (current != null)
            {
                builder.Insert(0, current.name + "/");
                current = current.parent;
            }

            return builder.ToString();
        }
    }
}
