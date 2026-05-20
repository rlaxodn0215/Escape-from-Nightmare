using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class Stage1PlayModeSmokeTests
{
    [UnityTest]
    public IEnumerator Stage1_LoadsAndStartsInChildRoom()
    {
        SceneManager.LoadScene("Stage1");
        yield return null;
        yield return null;

        GameObject systems = GameObject.Find("Systems");
        Assert.That(systems, Is.Not.Null);

        Component roomSystem = GetComponentNamed(systems, "RoomSystem");
        Assert.That(roomSystem, Is.Not.Null);

        object currentRoom = roomSystem.GetType().GetProperty("CurrentRoom")?.GetValue(roomSystem);
        Assert.That(currentRoom, Is.Not.Null, "RoomSystem did not initialize a current room.");

        object roomId = currentRoom.GetType().GetProperty("RoomId")?.GetValue(currentRoom);
        Assert.That(roomId, Is.EqualTo("child_room"));
    }

    [Test]
    public void Stage1SmokeArgument_IsDetectedOnlyForExplicitSmokeMode()
    {
        System.Type runnerType = GetSmokeRunnerType();
        System.Reflection.MethodInfo hasSmokeArgument = runnerType.GetMethod("HasSmokeArgument");
        Assert.That(hasSmokeArgument, Is.Not.Null);

        Assert.That(hasSmokeArgument.Invoke(null, new object[] { new[] { "-batchmode", "--stage1-smoke" } }), Is.EqualTo(true));
        Assert.That(hasSmokeArgument.Invoke(null, new object[] { new[] { "-batchmode", "-nographics" } }), Is.EqualTo(false));
        Assert.That(hasSmokeArgument.Invoke(null, new object[] { null }), Is.EqualTo(false));
    }

    [UnityTest]
    public IEnumerator Stage1SmokeValidation_PassesAfterStage1Load()
    {
        SceneManager.LoadScene("Stage1");
        yield return null;
        yield return null;

        System.Type runnerType = GetSmokeRunnerType();
        System.Reflection.MethodInfo tryValidate = runnerType.GetMethod("TryValidateLoadedStage1", new[] { typeof(string).MakeByRefType() });
        Assert.That(tryValidate, Is.Not.Null);

        object[] args = { null };
        Assert.That((bool)tryValidate.Invoke(null, args), Is.True, args[0]?.ToString());
    }

    [UnityTest]
    public IEnumerator Stage1_RuntimeSystemsAndUiSurfaces_ArePresentAfterLoad()
    {
        SceneManager.LoadScene("Stage1");
        yield return null;
        yield return null;

        GameObject systems = GameObject.Find("Systems");
        Assert.That(systems, Is.Not.Null);

        string[] requiredComponents =
        {
            "RoomSystem",
            "InteractionSystem",
            "InventorySystem",
            "PuzzleSystem",
            "EventRuntimeSystem",
            "SoundRuntimeSystem",
            "HidingRuntimeSystem",
            "MonsterRuntimeSystem",
            "MapRuntimeSystem"
        };

        foreach (string componentName in requiredComponents)
        {
            Assert.That(GetComponentNamed(systems, componentName), Is.Not.Null, $"Systems is missing {componentName}.");
        }

        string[] requiredUiObjects =
        {
            "InventoryUI",
            "MapUI",
            "SettingsUI",
            "PuzzleUI",
            "HidingGaugeUI",
            "GameOverUI",
            "HUDInventoryButton",
            "HUDMapButton",
            "HUDSettingsButton"
        };

        foreach (string objectName in requiredUiObjects)
        {
            Assert.That(FindObjectIncludingInactive(objectName), Is.Not.Null, $"{objectName} is missing after Stage1 load.");
        }
    }

    [UnityTest]
    public IEnumerator Stage1_MapRuntimeHasMarkers()
    {
        SceneManager.LoadScene("Stage1");
        yield return null;
        yield return null;

        GameObject systems = GameObject.Find("Systems");
        Component mapRuntimeSystem = GetComponentNamed(systems, "MapRuntimeSystem");
        Assert.That(mapRuntimeSystem, Is.Not.Null);

        object markers = mapRuntimeSystem.GetType()
            .GetField("markers", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.GetValue(mapRuntimeSystem);

        Assert.That(markers, Is.Not.Null);
        Assert.That(((System.Array)markers).Length, Is.GreaterThanOrEqualTo(27));
    }

    [UnityTest]
    public IEnumerator Stage1_MapCurrentRoomMarkerOnlyShowsOnCurrentFloor()
    {
        SceneManager.LoadScene("Stage1");
        yield return null;
        yield return null;

        GameObject mapObject = FindObjectIncludingInactive("MapUI");
        Assert.That(mapObject, Is.Not.Null);

        Component mapUI = mapObject.GetComponents<Component>()
            .FirstOrDefault(component => component != null && component.GetType().Name == "MapUI");
        Assert.That(mapUI, Is.Not.Null);

        Image marker = mapObject.GetComponentsInChildren<Image>(true)
            .FirstOrDefault(image => image.name == "CurrentRoomMarker");
        Assert.That(marker, Is.Not.Null);

        System.Reflection.MethodInfo setCurrentRoom = mapUI.GetType().GetMethod("SetCurrentRoom");
        Assert.That(setCurrentRoom, Is.Not.Null);
        System.Type floorType = setCurrentRoom.GetParameters()[0].ParameterType;
        object secondFloor = System.Enum.Parse(floorType, "SecondFloor");

        setCurrentRoom.Invoke(mapUI, new object[] { secondFloor, new Vector2(-214f, 84f), "child_room" });
        Assert.That(marker.enabled, Is.True);
        Assert.That(marker.rectTransform.anchoredPosition, Is.EqualTo(new Vector2(-214f, 84f)));

        mapUI.GetType().GetMethod("ShowFloor")?.Invoke(mapUI, new object[] { 0 });
        Assert.That(marker.enabled, Is.False);

        mapUI.GetType().GetMethod("ShowFloor")?.Invoke(mapUI, new object[] { 1 });
        Assert.That(marker.enabled, Is.True);
        Assert.That(marker.rectTransform.anchoredPosition, Is.EqualTo(new Vector2(-214f, 84f)));
    }

    private static Component GetComponentNamed(GameObject gameObject, string componentName)
    {
        return gameObject.GetComponents<Component>().FirstOrDefault(component => component != null && component.GetType().Name == componentName);
    }

    private static GameObject FindObjectIncludingInactive(string objectName)
    {
        Transform[] transforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return transforms.FirstOrDefault(transform => transform.name == objectName)?.gameObject;
    }

    private static System.Type GetSmokeRunnerType()
    {
        System.Type runnerType = System.AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType("EscapeFromNightmares.Core.GeneratedPlayerSmokeRunner"))
            .FirstOrDefault(type => type != null);
        Assert.That(runnerType, Is.Not.Null);
        return runnerType;
    }
}
