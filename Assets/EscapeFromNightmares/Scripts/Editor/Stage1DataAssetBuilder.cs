using System.Collections.Generic;
using System.IO;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EscapeFromNightmares.Editor
{
    /// <summary>
    /// Converts the current Stage 1 seed data into stable ScriptableObject assets.
    /// </summary>
    public static class Stage1DataAssetBuilder
    {
        public const string Root = "Assets/EscapeFromNightmares";
        public const string StageRoot = Root + "/ScriptableObjects/Stage1";
        public const string ItemsRoot = StageRoot + "/Items";
        public const string PuzzlesRoot = StageRoot + "/Puzzles";
        public const string RoomsRoot = StageRoot + "/Rooms";
        public const string DataResourcesRoot = Root + "/Resources/EscapeFromNightmares/Data";
        public const string MainScenePath = Root + "/Scenes/MainScene.unity";
        public const string StageAssetPath = StageRoot + "/Stage1.asset";
        public const string MonsterGraphAssetPath = StageRoot + "/Stage1MonsterNodeGraph.asset";
        public const string SoundCatalogAssetPath = StageRoot + "/Stage1SoundCatalog.asset";
        public const string StageCatalogAssetPath = DataResourcesRoot + "/StageCatalog.asset";

        [MenuItem("Escape From Nightmares/Rebuild Stage 1 Data Assets")]
        public static void RebuildStage1DataAssets()
        {
            EnsureStage1Assets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static StageDefinition EnsureStage1Assets()
        {
            EnsureFolders();

#pragma warning disable 618
            var seed = RuntimeStageFactory.CreateStage1();
#pragma warning restore 618
            var itemAssets = CopyAssets(seed.items, ItemsRoot, item => item.itemId, ItemAssetPath, "Item");
            var puzzleAssets = CopyAssets(seed.puzzles, PuzzlesRoot, puzzle => puzzle.puzzleId, PuzzleAssetPath, "Puzzle");
            var roomAssets = CopyAssets(seed.rooms, RoomsRoot, room => room.roomId, RoomAssetPath, "Room");
            var graphAsset = CopySingleAsset(seed.monsterNodeGraph, MonsterGraphAssetPath);
            var soundCatalogAsset = CopySingleAsset(seed.soundCatalog, SoundCatalogAssetPath);

            var stageAsset = EnsureAsset<StageDefinition>(StageAssetPath);
            stageAsset.stageId = seed.stageId;
            stageAsset.displayName = seed.displayName;
            stageAsset.startRoomId = seed.startRoomId;
            stageAsset.clearFlag = seed.clearFlag;
            stageAsset.items = itemAssets;
            stageAsset.puzzles = puzzleAssets;
            stageAsset.rooms = roomAssets;
            stageAsset.monsterNodeGraph = graphAsset;
            stageAsset.soundCatalog = soundCatalogAsset;
            EditorUtility.SetDirty(stageAsset);

            var catalog = EnsureAsset<StageCatalog>(StageCatalogAssetPath);
            catalog.stage1 = stageAsset;
            EditorUtility.SetDirty(catalog);
            AssignStageToMainScene(stageAsset);

            WarnForStaleAssets<ItemDefinition>(ItemsRoot, itemAssets, item => item.itemId, "item");
            WarnForStaleAssets<PuzzleDefinition>(PuzzlesRoot, puzzleAssets, puzzle => puzzle.puzzleId, "puzzle");
            WarnForStaleAssets<RoomDefinition>(RoomsRoot, roomAssets, room => room.roomId, "room");

            return stageAsset;
        }

        private static List<T> CopyAssets<T>(IEnumerable<T> sourceAssets, string folder, System.Func<T, string> idSelector, System.Func<string, string> pathSelector, string label)
            where T : ScriptableObject
        {
            var results = new List<T>();
            foreach (var source in sourceAssets)
            {
                if (source == null)
                {
                    continue;
                }

                var id = idSelector(source);
                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning(label + " seed has an empty id and was skipped.");
                    continue;
                }

                var path = pathSelector(id);
                var destination = EnsureAsset<T>(path);
                EditorUtility.CopySerializedManagedFieldsOnly(source, destination);
                destination.name = Path.GetFileNameWithoutExtension(path);
                EditorUtility.SetDirty(destination);
                results.Add(destination);
            }

            return results;
        }

        private static T CopySingleAsset<T>(T source, string path)
            where T : ScriptableObject
        {
            if (source == null)
            {
                return null;
            }

            var destination = EnsureAsset<T>(path);
            EditorUtility.CopySerializedManagedFieldsOnly(source, destination);
            destination.name = Path.GetFileNameWithoutExtension(path);
            EditorUtility.SetDirty(destination);
            return destination;
        }

        private static T EnsureAsset<T>(string path)
            where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            if (File.Exists(path))
            {
                Debug.LogWarning("Replacing invalid " + typeof(T).Name + " asset at " + path);
                AssetDatabase.DeleteAsset(path);
            }

            asset = ScriptableObject.CreateInstance<T>();
            asset.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void WarnForStaleAssets<T>(string folder, IReadOnlyCollection<T> currentAssets, System.Func<T, string> idSelector, string label)
            where T : ScriptableObject
        {
            var currentPaths = new HashSet<string>(currentAssets.Select(AssetDatabase.GetAssetPath));
            foreach (var guid in AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { folder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!currentPaths.Contains(path))
                {
                    var staleAsset = AssetDatabase.LoadAssetAtPath<T>(path);
                    var staleId = staleAsset != null ? idSelector(staleAsset) : "(unknown)";
                    Debug.LogWarning("Stale " + label + " asset kept for manual review: " + staleId + " at " + path);
                }
            }
        }

        private static string ItemAssetPath(string itemId)
        {
            return ItemsRoot + "/" + SafeFileName(itemId) + ".asset";
        }

        private static string PuzzleAssetPath(string puzzleId)
        {
            return PuzzlesRoot + "/" + SafeFileName(puzzleId) + ".asset";
        }

        private static string RoomAssetPath(string roomId)
        {
            return RoomsRoot + "/" + SafeFileName(roomId) + ".asset";
        }

        private static string SafeFileName(string id)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var characters = id.Select(character => invalid.Contains(character) ? '_' : character).ToArray();
            return new string(characters);
        }

        private static void EnsureFolders()
        {
            EnsureFolder(Root + "/ScriptableObjects");
            EnsureFolder(StageRoot);
            EnsureFolder(ItemsRoot);
            EnsureFolder(PuzzlesRoot);
            EnsureFolder(RoomsRoot);
            EnsureFolder(Root + "/Resources");
            EnsureFolder(Root + "/Resources/EscapeFromNightmares");
            EnsureFolder(DataResourcesRoot);
        }

        private static void AssignStageToMainScene(StageDefinition stageAsset)
        {
            if (stageAsset == null || !File.Exists(MainScenePath))
            {
                return;
            }

            var scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            var director = UnityEngine.Object.FindFirstObjectByType<GameDirector>();
            if (director == null)
            {
                Debug.LogWarning("MainScene has no GameDirector to receive Stage 1 asset.");
                return;
            }

            var serializedDirector = new SerializedObject(director);
            var stageProperty = serializedDirector.FindProperty("stageDefinition");
            if (stageProperty == null)
            {
                Debug.LogWarning("GameDirector.stageDefinition serialized field was not found.");
                return;
            }

            stageProperty.objectReferenceValue = stageAsset;
            serializedDirector.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            var parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
            var name = Path.GetFileName(folder);
            if (!string.IsNullOrWhiteSpace(parent))
            {
                EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
