using System.Collections.Generic;
using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace EscapeFromNightmares.Tests.EditMode
{
    public sealed class StageAssetCatalogTests
    {
        [Test]
        public void StageCatalog_Stage1AssetIsRegistered()
        {
            var catalog = StageRepository.LoadCatalog();
            Assert.That(catalog, Is.Not.Null);
            Assert.That(catalog.stage1, Is.Not.Null);
            Assert.That(catalog.stage1.stageId, Is.EqualTo(StageRepository.Stage1Id));
        }

        [Test]
        public void Stage1Asset_ReferencesAreCompleteAndIdsAreUnique()
        {
            var stage = StageTestData.LoadStage1();
            Assert.That(stage.rooms, Is.Not.Empty);
            Assert.That(stage.items, Is.Not.Empty);
            Assert.That(stage.puzzles, Is.Not.Empty);
            Assert.That(stage.monsterNodeGraph, Is.Not.Null);
            Assert.That(stage.soundCatalog, Is.Not.Null);

            AssertUnique(stage.rooms.Select(room => room.roomId), "roomId");
            AssertUnique(stage.items.Select(item => item.itemId), "itemId");
            AssertUnique(stage.puzzles.Select(puzzle => puzzle.puzzleId), "puzzleId");
            AssertUnique(stage.rooms.SelectMany(RoomInteractableIds), "interactableId");
        }

        [Test]
        public void Stage1Asset_RoomGraphDoorTargetsAndResourcesAreValid()
        {
            var stage = StageTestData.LoadStage1();
            var roomIds = new HashSet<string>(stage.rooms.Select(room => room.roomId));
            var puzzleIds = new HashSet<string>(stage.puzzles.Select(puzzle => puzzle.puzzleId));
            var itemIds = new HashSet<string>(stage.items.Select(item => item.itemId));

            foreach (var room in stage.rooms)
            {
                foreach (var connectedRoomId in room.connectedRoomIds)
                {
                    Assert.That(roomIds.Contains(connectedRoomId), Is.True, room.roomId + " connectedRoomIds contains missing room " + connectedRoomId);
                }

                foreach (var face in room.faces)
                {
                    AssertSpriteLoads(RoomPresenter.ResolveRoomFaceBackgroundResource(face, null), room.roomId + " " + face.direction);
                    foreach (var conditionalBackground in face.conditionalBackgrounds ?? new ConditionalBackgroundDefinition[0])
                    {
                        AssertSpriteLoads(conditionalBackground.backgroundResource, room.roomId + " " + face.direction + " conditional background");
                    }

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

                        AssertSpriteLoads(interactable.closeUpClosedResource, interactable.interactableId + " close-up closed");
                        AssertSpriteLoads(interactable.closeUpOpenWithItemResource, interactable.interactableId + " close-up open with item");
                        AssertSpriteLoads(interactable.closeUpOpenEmptyResource, interactable.interactableId + " close-up open empty");
                        AssertSpriteLoads(interactable.hideViewResource, interactable.interactableId + " hide view");
                        AssertSpriteLoads(interactable.clueViewResource, interactable.interactableId + " clue view");
                    }
                }
            }

            foreach (var item in stage.items)
            {
                AssertSpriteLoads(item.iconResource, item.itemId + " icon");
            }

            foreach (var puzzle in stage.puzzles)
            {
                AssertSpriteLoads(puzzle.closeUpResource, puzzle.puzzleId + " close-up");
            }

            foreach (var sound in stage.soundCatalog.entries)
            {
                AssertAudioLoads(sound.resourcePath, sound.soundId);
            }
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

        private static void AssertSpriteLoads(string resourcePath, string context)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                return;
            }

            Assert.That(Resources.Load<Sprite>(resourcePath), Is.Not.Null, context + " resource missing: " + resourcePath);
        }

        private static void AssertAudioLoads(string resourcePath, string context)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                return;
            }

            Assert.That(Resources.Load<AudioClip>(resourcePath), Is.Not.Null, context + " audio missing: " + resourcePath);
        }
    }
}
