using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    [CreateAssetMenu(menuName = "Escape From Nightmares/Monster Placement Catalog")]
    public sealed class MonsterPlacementCatalog : ScriptableObject
    {
        [SerializeField] private List<MonsterPlacementEntry> placements = new List<MonsterPlacementEntry>();

        public IReadOnlyList<MonsterPlacementEntry> Placements => placements;

        public bool TryFind(string roomId, RoomFaceDirection faceDirection, out MonsterPlacementEntry placement)
        {
            foreach (var entry in placements)
            {
                if (entry != null && entry.roomId == roomId && entry.faceDirection == faceDirection)
                {
                    placement = entry;
                    return true;
                }
            }

            placement = null;
            return false;
        }

        public void SetPlacements(IEnumerable<MonsterPlacementEntry> entries)
        {
            placements.Clear();
            if (entries == null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                if (entry != null)
                {
                    placements.Add(entry);
                }
            }
        }

        public static MonsterPlacementCatalog CreateDefault(StageDefinition stage)
        {
            var catalog = CreateInstance<MonsterPlacementCatalog>();
            catalog.SetPlacements(CreateDefaultEntries(stage));
            return catalog;
        }

        public static IEnumerable<MonsterPlacementEntry> CreateDefaultEntries(StageDefinition stage)
        {
            if (stage == null || stage.rooms == null)
            {
                yield break;
            }

            foreach (var room in stage.rooms)
            {
                if (room == null || room.roomId == "child_room" || room.faces == null)
                {
                    continue;
                }

                foreach (var face in room.faces)
                {
                    yield return new MonsterPlacementEntry
                    {
                        roomId = room.roomId,
                        faceDirection = face.direction,
                        enabled = false,
                        normalizedRect = Rect.zero
                    };
                }
            }
        }

        public static IEnumerable<MonsterPlacementEntry> CreateMergedDefaultEntries(StageDefinition stage, IEnumerable<MonsterPlacementEntry> preservedEntries)
        {
            var preserved = new Dictionary<string, MonsterPlacementEntry>();
            if (preservedEntries != null)
            {
                foreach (var entry in preservedEntries)
                {
                    if (entry == null)
                    {
                        continue;
                    }

                    preserved[PlacementKey(entry.roomId, entry.faceDirection)] = entry;
                }
            }

            foreach (var defaultEntry in CreateDefaultEntries(stage))
            {
                if (preserved.TryGetValue(PlacementKey(defaultEntry.roomId, defaultEntry.faceDirection), out var existing))
                {
                    yield return new MonsterPlacementEntry
                    {
                        roomId = defaultEntry.roomId,
                        faceDirection = defaultEntry.faceDirection,
                        enabled = existing.enabled,
                        normalizedRect = existing.normalizedRect
                    };
                    continue;
                }

                yield return defaultEntry;
            }
        }

        private static string PlacementKey(string roomId, RoomFaceDirection faceDirection)
        {
            return roomId + "|" + faceDirection;
        }
    }

    [Serializable]
    public sealed class MonsterPlacementEntry
    {
        public string roomId;
        public RoomFaceDirection faceDirection;
        public bool enabled;
        public Rect normalizedRect = Rect.zero;
    }
}
