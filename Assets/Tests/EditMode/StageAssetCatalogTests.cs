using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EscapeFromNightmares.Tests.EditMode
{
    public sealed class StageAssetCatalogTests
    {
        private const string ResourceCatalogPath = "Assets/Resources/EscapeFromNightmares/ResourcePathCatalog.asset";
        private const string MainScenePath = "Assets/Scenes/Main.unity";
        private static readonly string[] ChildRoomRuntimeSpriteIds =
        {
            "child_room_east",
            "child_room_north",
            "child_room_north_drawer_empty",
            "child_room_south",
            "child_room_west"
        };

        [Test]
        public void RuntimeStageFactory_CreatesStage1Data()
        {
            var stage = RuntimeStageFactory.CreateStage1();

            Assert.That(stage, Is.Not.Null);
            Assert.That(stage.stageId, Is.EqualTo(StageRepository.Stage1Id));
            Assert.That(stage.rooms, Is.Not.Empty);
            Assert.That(stage.items, Is.Not.Empty);
            Assert.That(stage.puzzles, Is.Not.Empty);
            Assert.That(stage.monsterNodeGraph, Is.Not.Null);
            Assert.That(stage.sounds, Is.Not.Empty);
        }

        [Test]
        public void Stage1Data_ReferencesAreCompleteAndIdsAreUnique()
        {
            var stage = StageTestData.LoadStage1();
            var roomIds = new HashSet<string>(stage.rooms.Select(room => room.roomId));
            var puzzleIds = new HashSet<string>(stage.puzzles.Select(puzzle => puzzle.puzzleId));
            var itemIds = new HashSet<string>(stage.items.Select(item => item.itemId));

            AssertUnique(stage.rooms.Select(room => room.roomId), "roomId");
            AssertUnique(stage.items.Select(item => item.itemId), "itemId");
            AssertUnique(stage.puzzles.Select(puzzle => puzzle.puzzleId), "puzzleId");
            AssertUnique(stage.rooms.SelectMany(RoomInteractableIds), "interactableId");

            foreach (var room in stage.rooms)
            {
                foreach (var connectedRoomId in room.connectedRoomIds)
                {
                    Assert.That(roomIds.Contains(connectedRoomId), Is.True, room.roomId + " connectedRoomIds contains missing room " + connectedRoomId);
                }

                foreach (var face in room.faces)
                {
                    foreach (var interactable in face.interactables ?? new InteractableDefinition[0])
                    {
                        if (!string.IsNullOrWhiteSpace(interactable.targetRoomId))
                        {
                            Assert.That(roomIds.Contains(interactable.targetRoomId), Is.True, interactable.interactableId + " targets missing room " + interactable.targetRoomId);
                        }

                        if (!string.IsNullOrWhiteSpace(interactable.puzzleId))
                        {
                            Assert.That(puzzleIds.Contains(interactable.puzzleId), Is.True, interactable.interactableId + " targets missing puzzle " + interactable.puzzleId);
                        }

                        if (!string.IsNullOrWhiteSpace(interactable.grantsItemId))
                        {
                            Assert.That(itemIds.Contains(interactable.grantsItemId), Is.True, interactable.interactableId + " grants missing item " + interactable.grantsItemId);
                        }
                    }
                }
            }
        }

        [Test]
        public void ResourcePathCatalog_ContainsEveryStage1VisibleSpriteReference()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ResourcePathCatalog>(ResourceCatalogPath);
            Assert.That(catalog, Is.Not.Null, ResourceCatalogPath + " is missing. Run Escape From Nightmares/Rebuild Title Scene Assets.");

            foreach (var resourcePath in Stage1VisibleSpriteResources(StageTestData.LoadStage1())
                .Where(resourcePath => !string.IsNullOrWhiteSpace(resourcePath))
                .Distinct())
            {
                var spriteId = SpriteId(resourcePath);
                Assert.That(catalog.TryFindSprite(spriteId, out var sprite), Is.True, resourcePath);
                Assert.That(sprite, Is.Not.Null, resourcePath);
            }
        }

        [Test]
        public void ResourcePathCatalog_LoadsFromResourcesForBootstrapFallback()
        {
            var catalog = Resources.Load<ResourcePathCatalog>(GameDirector.DefaultResourceCatalogPath);
            Assert.That(catalog, Is.Not.Null, "Bootstrap-created GameDirector must be able to load the sprite catalog.");

            foreach (var spriteId in ChildRoomRuntimeSpriteIds.Concat(new[] { "study_north", "stage1_clear_background", "monster_shadow" }))
            {
                Assert.That(catalog.TryFindSprite(spriteId, out var sprite), Is.True, spriteId);
                Assert.That(sprite, Is.Not.Null, spriteId);
            }
        }

        [Test]
        public void GameDirector_DefaultResourceCatalogHelperLoadsFallbackCatalog()
        {
            var catalog = GameDirector.LoadDefaultResourceCatalog();
            Assert.That(catalog, Is.Not.Null);
            foreach (var spriteId in ChildRoomRuntimeSpriteIds)
            {
                Assert.That(catalog.TryFindSprite(spriteId, out var sprite), Is.True, spriteId);
                Assert.That(sprite, Is.Not.Null, spriteId);
            }
        }

        [Test]
        public void MainScene_GameDirectorSpriteBindingsAreCompleteUniqueAndCatalogLinked()
        {
            var scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            Assert.That(scene.IsValid(), Is.True, MainScenePath + " could not be opened.");

            var director = Object.FindFirstObjectByType<GameDirector>();
            Assert.That(director, Is.Not.Null, MainScenePath + " is missing GameDirector.");

            var serialized = new SerializedObject(director);
            Assert.That(serialized.FindProperty("resourceCatalog").objectReferenceValue, Is.Not.Null, "GameDirector.resourceCatalog is not linked.");

            var spriteBindings = serialized.FindProperty("spriteBindings");
            Assert.That(spriteBindings, Is.Not.Null);
            Assert.That(spriteBindings.arraySize, Is.GreaterThan(0), "GameDirector.spriteBindings is empty.");

            var spriteIds = new List<string>();
            for (var index = 0; index < spriteBindings.arraySize; index++)
            {
                var element = spriteBindings.GetArrayElementAtIndex(index);
                var spriteId = element.FindPropertyRelative("spriteId").stringValue;
                var sprite = element.FindPropertyRelative("sprite").objectReferenceValue;
                Assert.That(spriteId, Is.Not.Empty, "spriteBindings[" + index + "] has an empty id.");
                Assert.That(sprite, Is.Not.Null, "spriteBindings[" + index + "] " + spriteId + " has no Sprite.");
                spriteIds.Add(spriteId);
            }

            foreach (var spriteId in ChildRoomRuntimeSpriteIds)
            {
                Assert.That(spriteIds.Contains(spriteId), Is.True, "Main scene is missing child room sprite binding: " + spriteId);
            }

            AssertUnique(spriteIds, "spriteBinding.spriteId");
        }

        [Test]
        public void MainScene_GameDirectorMonsterPlacementsAreGenerated()
        {
            var scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            Assert.That(scene.IsValid(), Is.True, MainScenePath + " could not be opened.");

            var director = Object.FindFirstObjectByType<GameDirector>();
            Assert.That(director, Is.Not.Null, MainScenePath + " is missing GameDirector.");

            var serialized = new SerializedObject(director);
            var placements = serialized
                .FindProperty("monsterPlacementCatalog")
                .FindPropertyRelative("placements");

            var expectedCount = StageTestData.LoadStage1().rooms
                .Where(room => room.roomId != "child_room")
                .Sum(room => room.faces.Length);

            Assert.That(placements.arraySize, Is.EqualTo(expectedCount));
        }

        private static IEnumerable<string> RoomInteractableIds(RoomDefinition room)
        {
            return room.faces
                .SelectMany(face => face.interactables ?? new InteractableDefinition[0])
                .Where(interactable => interactable != null)
                .Select(interactable => interactable.interactableId);
        }

        private static void AssertUnique(IEnumerable<string> values, string label)
        {
            var duplicates = values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .GroupBy(value => value)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();
            Assert.That(duplicates, Is.Empty, "Duplicate " + label + ": " + string.Join(", ", duplicates));
        }

        private static IEnumerable<string> Stage1VisibleSpriteResources(StageDefinition stage)
        {
            yield return GameDirector.StageClearBackgroundResource;
            yield return GameDirector.MonsterShadowResource;

            foreach (var item in stage.items)
            {
                yield return item.iconResource;
            }

            foreach (var puzzle in stage.puzzles)
            {
                yield return puzzle.closeUpResource;
            }

            foreach (var room in stage.rooms)
            {
                yield return room.backgroundResource;

                foreach (var face in room.faces)
                {
                    yield return face.backgroundResource;

                    foreach (var conditional in face.conditionalBackgrounds ?? new ConditionalBackgroundDefinition[0])
                    {
                        yield return conditional.backgroundResource;
                    }

                    foreach (var interactable in face.interactables ?? new InteractableDefinition[0])
                    {
                        yield return interactable.clueViewResource;
                        yield return interactable.closeUpClosedResource;
                        yield return interactable.closeUpOpenWithItemResource;
                        yield return interactable.closeUpOpenEmptyResource;
                        yield return interactable.hideViewResource;

                        if (interactable.showWorldImage)
                        {
                            yield return interactable.imageResource;
                        }
                    }
                }
            }
        }

        private static string SpriteId(string resourcePathOrId)
        {
            if (string.IsNullOrWhiteSpace(resourcePathOrId))
            {
                return string.Empty;
            }

            var slashIndex = resourcePathOrId.LastIndexOf('/');
            return slashIndex >= 0 ? resourcePathOrId.Substring(slashIndex + 1) : resourcePathOrId;
        }
    }
}
