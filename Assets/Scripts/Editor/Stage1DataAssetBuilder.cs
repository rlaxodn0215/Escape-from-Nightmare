using EscapeFromNightmares.Runtime;
using UnityEditor;
using UnityEngine;

namespace EscapeFromNightmares.Editor
{
    /// <summary>
    /// Validates that Stage 1 runtime seed data can be created without external data assets.
    /// </summary>
    public static class Stage1DataAssetBuilder
    {
        [MenuItem("Escape From Nightmares/Rebuild Stage 1 Data Assets")]
        public static void RebuildStage1DataAssets()
        {
            EnsureStage1Data();
        }

        public static void EnsureStage1Data()
        {
            var stage = RuntimeStageFactory.CreateStage1();
            Debug.Log("Stage 1 runtime data ready: " + stage.rooms.Count + " rooms, " + stage.items.Count + " items, " + stage.puzzles.Count + " puzzles.");
        }
    }
}
