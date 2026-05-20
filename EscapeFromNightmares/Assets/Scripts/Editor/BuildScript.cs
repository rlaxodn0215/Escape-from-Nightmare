using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    private const string ProductName = "EscapeFromNightmares";
    private const string OutputDirectoryName = "Build/Windows";
    private const string ExecutableName = ProductName + ".exe";

    [MenuItem("Escape From Nightmares/Build Windows Player")]
    public static void BuildWindows()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new InvalidOperationException("Windows build failed because no enabled Build Settings scenes were found.");
        }

        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (string.IsNullOrWhiteSpace(projectRoot))
        {
            throw new InvalidOperationException("Windows build failed because the Unity project root could not be resolved.");
        }

        string outputDirectory = Path.Combine(projectRoot, OutputDirectoryName);
        Directory.CreateDirectory(outputDirectory);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(outputDirectory, ExecutableName),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException($"Windows build failed with result {summary.result}. Errors: {summary.totalErrors}");
        }

        Debug.Log($"Windows build succeeded: {summary.outputPath}");
    }
}
