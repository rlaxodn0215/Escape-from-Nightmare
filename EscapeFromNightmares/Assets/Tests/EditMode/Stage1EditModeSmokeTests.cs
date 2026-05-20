using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class Stage1EditModeSmokeTests
{
    private const string StageScenePath = "Assets/Scenes/Stage1.unity";
    private const string StageDefinitionPath = "Assets/ScriptableObjects/Stage1/Stage1Definition.asset";

    [Test]
    public void Stage1Scene_HasRequiredPlayableRoots()
    {
        EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);

        string[] requiredRoots =
        {
            "StageRoot",
            "Systems",
            "RoomView",
            "InteractableLayer",
            "MonsterLayer",
            "AudioRoot",
            "UICanvas",
            "EventSystem",
            "DebugRoot"
        };

        foreach (string rootName in requiredRoots)
        {
            Assert.That(FindObjectIncludingInactive(rootName), Is.Not.Null, $"{rootName} root is missing.");
        }
    }

    [Test]
    public void SystemsRoot_HasCoreRuntimeComponents()
    {
        EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);
        GameObject systems = GameObject.Find("Systems");
        Assert.That(systems, Is.Not.Null);

        string[] requiredComponents =
        {
            "GameStateManager",
            "SceneFlowController",
            "InputRouter",
            "SaveManager",
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
            Assert.That(HasComponentNamed(systems, componentName), Is.True, $"Systems is missing {componentName}.");
        }
    }

    [Test]
    public void Stage1_HasRequiredUiSurfacesAndHudButtons()
    {
        EditorSceneManager.OpenScene(StageScenePath, OpenSceneMode.Single);

        string[] requiredUiObjects =
        {
            "TitleUI",
            "PauseUI",
            "SettingsUI",
            "InventoryUI",
            "MapUI",
            "PuzzleUI",
            "HidingGaugeUI",
            "GameOverUI",
            "HUDInventoryButton",
            "HUDMapButton",
            "HUDSettingsButton"
        };

        foreach (string objectName in requiredUiObjects)
        {
            Assert.That(FindObjectIncludingInactive(objectName), Is.Not.Null, $"{objectName} is missing.");
        }
    }

    [Test]
    public void RequiredPrefabs_ExistOnDisk()
    {
        string[] prefabPaths =
        {
            "Assets/Prefabs/UI/TitleUI.prefab",
            "Assets/Prefabs/UI/PauseUI.prefab",
            "Assets/Prefabs/UI/SettingsUI.prefab",
            "Assets/Prefabs/UI/InventoryUI.prefab",
            "Assets/Prefabs/UI/MapUI.prefab",
            "Assets/Prefabs/UI/PuzzleUI.prefab",
            "Assets/Prefabs/UI/HidingGaugeUI.prefab",
            "Assets/Prefabs/UI/GameOverUI.prefab",
            "Assets/Prefabs/RoomView.prefab",
            "Assets/Prefabs/Interactables/InteractableHotspot.prefab",
            "Assets/Prefabs/Interactables/ScreenEdgeHotspot.prefab",
            "Assets/Prefabs/Interactables/HideSpot.prefab",
            "Assets/Prefabs/Monster/MonsterOverlay.prefab",
            "Assets/Prefabs/Audio/AudioEmitter.prefab"
        };

        foreach (string prefabPath in prefabPaths)
        {
            Assert.That(File.Exists(Path.Combine(Application.dataPath, "..", prefabPath)), Is.True, $"{prefabPath} is missing.");
        }
    }

    [Test]
    public void BuildSettings_EnableStage1Scene()
    {
        string[] enabledScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        Assert.That(enabledScenes, Does.Contain(StageScenePath));
        Assert.That(enabledScenes, Does.Not.Contain("Assets/Scenes/SampleScene.unity"));
    }

    [Test]
    public void StageDefinitionAsset_Exists()
    {
        Object stageDefinition = AssetDatabase.LoadAssetAtPath<Object>(StageDefinitionPath);
        Assert.That(stageDefinition, Is.Not.Null);
    }

    private static bool HasComponentNamed(GameObject gameObject, string componentName)
    {
        return gameObject.GetComponents<Component>().Any(component => component != null && component.GetType().Name == componentName);
    }

    private static GameObject FindObjectIncludingInactive(string objectName)
    {
        Transform[] transforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return transforms.FirstOrDefault(transform => transform.name == objectName)?.gameObject;
    }
}
