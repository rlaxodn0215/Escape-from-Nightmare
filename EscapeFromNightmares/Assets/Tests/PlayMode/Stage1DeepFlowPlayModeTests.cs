using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class Stage1DeepFlowPlayModeTests
{
    [UnityTest]
    public IEnumerator EventPlayerCaptured_SetsGameOverAndRestartReturnsToChildRoom()
    {
        yield return LoadStage1();

        Component eventRuntimeSystem = GetSystemComponent("EventRuntimeSystem");
        Component gameStateManager = GetSystemComponent("GameStateManager");
        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_player_captured"), Is.True);
        yield return null;

        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("GameOver"));
        Assert.That(GetProperty(gameStateManager, "CurrentRoomId"), Is.EqualTo("child_room"));

        Invoke(gameStateManager, "StartStage1Run");
        yield return null;

        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("Playing"));
        Assert.That(GetProperty(gameStateManager, "CurrentRoomId"), Is.EqualTo("child_room"));
    }

    [UnityTest]
    public IEnumerator FinalChaseEvent_SetsFlagAndStartsMonster()
    {
        yield return LoadStage1();

        Component eventRuntimeSystem = GetSystemComponent("EventRuntimeSystem");
        Component monsterRuntimeSystem = GetSystemComponent("MonsterRuntimeSystem");
        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_final_chase_trigger"), Is.True);
        yield return null;

        Assert.That(HasFlag(eventRuntimeSystem, "final_chase_started"), Is.True);
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Approaching"));
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentMonsterRoomId"), Is.EqualTo("child_room"));
    }

    [UnityTest]
    public IEnumerator StageClearEvent_MarksStateAndSaveClear()
    {
        PlayerPrefs.DeleteKey("stage1_clear");
        PlayerPrefs.Save();
        yield return LoadStage1();

        Component eventRuntimeSystem = GetSystemComponent("EventRuntimeSystem");
        Component gameStateManager = GetSystemComponent("GameStateManager");
        Component saveManager = GetSystemComponent("SaveManager");
        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_stage1_clear"), Is.True);
        yield return null;

        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("Ending"));
        Assert.That(GetProperty(gameStateManager, "Stage1Clear"), Is.EqualTo(true));
        Assert.That(GetProperty(saveManager, "Stage1Clear"), Is.EqualTo(true));
        Assert.That(PlayerPrefs.GetInt("stage1_clear", 0), Is.EqualTo(1));
    }

    [UnityTest]
    public IEnumerator FinaleDataChain_EventsExistAndCanExecuteInOrder()
    {
        PlayerPrefs.DeleteKey("stage1_clear");
        PlayerPrefs.Save();
        yield return LoadStage1();

        Component eventRuntimeSystem = GetSystemComponent("EventRuntimeSystem");
        Component gameStateManager = GetSystemComponent("GameStateManager");
        Component monsterRuntimeSystem = GetSystemComponent("MonsterRuntimeSystem");

        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_front_door_key_appears"), Is.True);
        Assert.That(HasFlag(eventRuntimeSystem, "front_door_key_appeared"), Is.True);

        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_final_chase_trigger"), Is.True);
        Assert.That(HasFlag(eventRuntimeSystem, "final_chase_started"), Is.True);
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Approaching"));

        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_stage1_clear"), Is.True);
        yield return null;

        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("Ending"));
        Assert.That(GetProperty(gameStateManager, "Stage1Clear"), Is.EqualTo(true));
    }

    [UnityTest]
    public IEnumerator Chase_CanBeEscapedByMovingAcrossThreeRooms()
    {
        yield return LoadStage1();

        Component roomSystem = GetSystemComponent("RoomSystem");
        Component monsterRuntimeSystem = GetSystemComponent("MonsterRuntimeSystem");
        SetMonsterState(monsterRuntimeSystem, "Chase");

        Assert.That(ChangeRoom(roomSystem, "second_floor_hallway"), Is.True);
        yield return null;
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Chase"));

        Assert.That(ChangeRoom(roomSystem, "stairwell_2f"), Is.True);
        yield return null;
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Chase"));

        Assert.That(ChangeRoom(roomSystem, "stairwell_1f"), Is.True);
        yield return null;
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Normal"));
    }

    [UnityTest]
    public IEnumerator HidingCompletion_CalmsActiveMonsterThreat()
    {
        yield return LoadStage1();

        Component hidingRuntimeSystem = GetSystemComponent("HidingRuntimeSystem");
        Component monsterRuntimeSystem = GetSystemComponent("MonsterRuntimeSystem");
        SetMonsterState(monsterRuntimeSystem, "Chase");

        Invoke(hidingRuntimeSystem, "SetCaptureGauge01", 0f);
        Invoke(hidingRuntimeSystem, "EnterHideSpot", "living_room_curtain_hide", 0.05f);
        yield return new WaitForSeconds(0.12f);

        Assert.That(GetProperty(hidingRuntimeSystem, "IsHiding"), Is.EqualTo(false));
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Normal"));
    }

    [UnityTest]
    public IEnumerator PuzzleFinaleWalkthrough_UsesInteractablesPuzzlesAndItemGates()
    {
        PlayerPrefs.DeleteKey("stage1_clear");
        PlayerPrefs.Save();
        yield return LoadStage1();

        Component interactionSystem = GetSystemComponent("InteractionSystem");
        Component puzzleSystem = GetSystemComponent("PuzzleSystem");
        Component inventorySystem = GetSystemComponent("InventorySystem");
        Component eventRuntimeSystem = GetSystemComponent("EventRuntimeSystem");
        Component gameStateManager = GetSystemComponent("GameStateManager");
        Component monsterRuntimeSystem = GetSystemComponent("MonsterRuntimeSystem");

        ClickInteractable(interactionSystem, "study_safe");
        Assert.That(SubmitPuzzle(puzzleSystem, "3142"), Is.True);
        Assert.That(HasItem(inventorySystem, "fuse_holder"), Is.True);

        ClickInteractable(interactionSystem, "laundry_storage_box");
        Assert.That(SubmitPuzzle(puzzleSystem, "0915"), Is.True);
        Assert.That(HasItem(inventorySystem, "fuse"), Is.True);

        Assert.That(SelectItem(inventorySystem, "fuse_holder"), Is.True);
        ClickInteractable(interactionSystem, "breaker_box");
        Assert.That(HasFlag(eventRuntimeSystem, "electricity_restored"), Is.True);
        Assert.That(HasItem(inventorySystem, "old_keychain"), Is.True);

        ClickInteractable(interactionSystem, "mirror_symbol_panel");
        Assert.That(SubmitPuzzle(puzzleSystem, "heart>child_hand>cracked_circle>keyhole"), Is.True);
        Assert.That(HasItem(inventorySystem, "broken_hand_mirror"), Is.True);

        ClickInteractable(interactionSystem, "master_bedroom_drawer");
        Assert.That(SubmitPuzzle(puzzleSystem, "black>white>red>gray"), Is.True);
        Assert.That(HasItem(inventorySystem, "old_necklace"), Is.True);

        ClickInteractable(interactionSystem, "attic_toy_box");
        Assert.That(SubmitPuzzle(puzzleSystem, "doll>train>block>bell"), Is.True);
        Assert.That(HasItem(inventorySystem, "small_doll"), Is.True);
        Assert.That(HasItem(inventorySystem, "symbol_fragment"), Is.True);

        ClickInteractable(interactionSystem, "front_door_key_on_altar");
        Assert.That(HasItem(inventorySystem, "front_door_key"), Is.False, "Final key should be gated until the altar puzzle succeeds.");

        ClickInteractable(interactionSystem, "basement_altar");
        Assert.That(SubmitPuzzle(puzzleSystem, "cracked_circle=broken_hand_mirror;child_hand=small_doll;keyhole=old_keychain;heart=old_necklace"), Is.True);
        Assert.That(HasFlag(eventRuntimeSystem, "front_door_key_appeared"), Is.True);

        ClickInteractable(interactionSystem, "front_door_key_on_altar");
        Assert.That(HasItem(inventorySystem, "front_door_key"), Is.True);
        Assert.That(HasFlag(eventRuntimeSystem, "final_chase_started"), Is.True);
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Approaching"));

        Assert.That(SelectItem(inventorySystem, "front_door_key"), Is.True);
        ClickInteractable(interactionSystem, "front_door");
        Assert.That(SubmitPuzzle(puzzleSystem, "front_door_key"), Is.True);
        yield return null;

        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("Ending"));
        Assert.That(GetProperty(gameStateManager, "Stage1Clear"), Is.EqualTo(true));
        Assert.That(PlayerPrefs.GetInt("stage1_clear", 0), Is.EqualTo(1));
    }

    [UnityTest]
    public IEnumerator GameOverRestartButton_ResetsRunStateWithoutSavingProgress()
    {
        PlayerPrefs.DeleteKey("stage1_clear");
        PlayerPrefs.Save();
        yield return LoadStage1();

        Component eventRuntimeSystem = GetSystemComponent("EventRuntimeSystem");
        Component inventorySystem = GetSystemComponent("InventorySystem");
        Component monsterRuntimeSystem = GetSystemComponent("MonsterRuntimeSystem");
        Component hidingRuntimeSystem = GetSystemComponent("HidingRuntimeSystem");
        Component gameStateManager = GetSystemComponent("GameStateManager");
        Component roomSystem = GetSystemComponent("RoomSystem");

        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_window_silhouette"), Is.True);
        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_open_study_safe"), Is.True);
        Assert.That(HasItem(inventorySystem, "fuse_holder"), Is.True);
        Assert.That(SelectItem(inventorySystem, "fuse_holder"), Is.True);
        SetMonsterState(monsterRuntimeSystem, "Chase");
        Invoke(hidingRuntimeSystem, "SetCaptureGauge01", 0.75f);
        Assert.That(ChangeRoom(roomSystem, "kitchen"), Is.True);

        Assert.That(ExecuteEvent(eventRuntimeSystem, "event_player_captured"), Is.True);
        yield return null;

        GameObject gameOverObject = FindObjectIncludingInactive("GameOverUI");
        Assert.That(gameOverObject, Is.Not.Null);
        CanvasGroup canvasGroup = gameOverObject.GetComponent<CanvasGroup>();
        Assert.That(canvasGroup, Is.Not.Null);
        Assert.That(canvasGroup.blocksRaycasts, Is.True);
        Assert.That(canvasGroup.interactable, Is.True);
        Assert.That(canvasGroup.alpha, Is.GreaterThan(0.5f));
        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("GameOver"));

        Button restartButton = FindChildComponent<Button>(gameOverObject, "RestartButton");
        Assert.That(restartButton, Is.Not.Null);
        restartButton.onClick.Invoke();
        yield return null;
        yield return null;

        Assert.That(GetProperty(gameStateManager, "CurrentState")?.ToString(), Is.EqualTo("Playing"));
        Assert.That(GetProperty(gameStateManager, "CurrentRoomId"), Is.EqualTo("child_room"));
        object currentRoom = GetProperty(roomSystem, "CurrentRoom");
        Assert.That(currentRoom, Is.Not.Null);
        Assert.That(GetProperty(currentRoom, "RoomId"), Is.EqualTo("child_room"));
        Assert.That(HasItem(inventorySystem, "fuse_holder"), Is.False);
        Assert.That(GetProperty(inventorySystem, "SelectedItem"), Is.Null);
        Assert.That(HasFlag(eventRuntimeSystem, "window_silhouette_seen"), Is.False);
        Assert.That(GetProperty(monsterRuntimeSystem, "CurrentState")?.ToString(), Is.EqualTo("Normal"));
        Assert.That(GetProperty(hidingRuntimeSystem, "IsHiding"), Is.EqualTo(false));
        Assert.That((float)GetProperty(hidingRuntimeSystem, "CaptureGauge01"), Is.EqualTo(0f).Within(0.001f));
        Assert.That(canvasGroup.blocksRaycasts, Is.False);
        Assert.That(canvasGroup.interactable, Is.False);
        Assert.That(canvasGroup.alpha, Is.LessThan(0.5f));
        Assert.That(PlayerPrefs.GetInt("stage1_clear", 0), Is.EqualTo(0));
    }

    private static IEnumerator LoadStage1()
    {
        SceneManager.LoadScene("Stage1");
        yield return null;
        yield return null;
    }

    private static Component GetSystemComponent(string componentName)
    {
        GameObject systems = GameObject.Find("Systems");
        Assert.That(systems, Is.Not.Null);

        Component component = systems.GetComponents<Component>()
            .FirstOrDefault(candidate => candidate != null && candidate.GetType().Name == componentName);
        Assert.That(component, Is.Not.Null, $"Systems is missing {componentName}.");
        return component;
    }

    private static bool ExecuteEvent(Component eventRuntimeSystem, string eventId)
    {
        return (bool)eventRuntimeSystem.GetType()
            .GetMethod("ExecuteEvent", new[] { typeof(string) })
            .Invoke(eventRuntimeSystem, new object[] { eventId });
    }

    private static bool HasFlag(Component eventRuntimeSystem, string flagId)
    {
        return (bool)eventRuntimeSystem.GetType()
            .GetMethod("HasFlag")
            .Invoke(eventRuntimeSystem, new object[] { flagId });
    }

    private static object GetProperty(object component, string propertyName)
    {
        return component.GetType().GetProperty(propertyName)?.GetValue(component);
    }

    private static void Invoke(Component component, string methodName)
    {
        component.GetType().GetMethod(methodName)?.Invoke(component, null);
    }

    private static void Invoke(Component component, string methodName, params object[] args)
    {
        component.GetType().GetMethod(methodName)?.Invoke(component, args);
    }

    private static bool ChangeRoom(Component roomSystem, string roomId)
    {
        return (bool)roomSystem.GetType()
            .GetMethod("ChangeRoom")
            .Invoke(roomSystem, new object[] { roomId });
    }

    private static void SetMonsterState(Component monsterRuntimeSystem, string stateName)
    {
        System.Reflection.MethodInfo setState = monsterRuntimeSystem.GetType().GetMethod("SetState");
        Assert.That(setState, Is.Not.Null);
        System.Type stateType = setState.GetParameters()[0].ParameterType;
        object state = System.Enum.Parse(stateType, stateName);
        setState.Invoke(monsterRuntimeSystem, new[] { state });
    }

    private static void ClickInteractable(Component interactionSystem, string interactableId)
    {
        object interactable = FindInteractable(interactableId);
        Assert.That(interactable, Is.Not.Null, $"Interactable '{interactableId}' was not found in StageDefinition.");
        interactionSystem.GetType()
            .GetMethod("HandleInteractableClicked")
            .Invoke(interactionSystem, new[] { interactable });
    }

    private static object FindInteractable(string interactableId)
    {
        object stageDefinition = GetStageDefinition();
        System.Array rooms = (System.Array)GetProperty(stageDefinition, "Rooms");
        foreach (object room in rooms)
        {
            System.Array interactables = (System.Array)GetProperty(room, "InteractableDefinitions");
            foreach (object interactable in interactables)
            {
                if ((string)GetProperty(interactable, "InteractableId") == interactableId)
                {
                    return interactable;
                }
            }
        }

        return null;
    }

    private static object GetStageDefinition()
    {
        GameObject stageRoot = GameObject.Find("StageRoot");
        Assert.That(stageRoot, Is.Not.Null);
        Component bootstrap = stageRoot.GetComponents<Component>()
            .FirstOrDefault(candidate => candidate != null && candidate.GetType().Name == "GameBootstrap");
        Assert.That(bootstrap, Is.Not.Null);
        return GetProperty(bootstrap, "StageDefinition");
    }

    private static bool SubmitPuzzle(Component puzzleSystem, string answer)
    {
        return (bool)puzzleSystem.GetType()
            .GetMethod("TrySubmitAnswer")
            .Invoke(puzzleSystem, new object[] { answer });
    }

    private static bool HasItem(Component inventorySystem, string itemId)
    {
        return (bool)inventorySystem.GetType()
            .GetMethod("HasItem")
            .Invoke(inventorySystem, new object[] { itemId });
    }

    private static bool SelectItem(Component inventorySystem, string itemId)
    {
        return (bool)inventorySystem.GetType()
            .GetMethod("SelectItem", new[] { typeof(string) })
            .Invoke(inventorySystem, new object[] { itemId });
    }

    private static GameObject FindObjectIncludingInactive(string objectName)
    {
        Transform[] transforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return transforms.FirstOrDefault(transform => transform.name == objectName)?.gameObject;
    }

    private static T FindChildComponent<T>(GameObject parent, string objectName) where T : Component
    {
        Transform[] transforms = parent.GetComponentsInChildren<Transform>(true);
        Transform target = transforms.FirstOrDefault(transform => transform.name == objectName);
        return target != null ? target.GetComponent<T>() : null;
    }
}
