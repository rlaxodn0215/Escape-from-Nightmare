using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// 방과 바라보는 방향마다 몬스터 그림자를 어느 위치에 표시할지 정의하는 카탈로그입니다.
    /// </summary>
    [CreateAssetMenu(menuName = "Escape From Nightmares/Monster Placement Catalog")]
    public sealed class MonsterPlacementCatalog : ScriptableObject
    {
        [SerializeField] private List<MonsterPlacementEntry> placements = new List<MonsterPlacementEntry>();

        /// <summary>현재 등록된 몬스터 배치 항목입니다.</summary>
        public IReadOnlyList<MonsterPlacementEntry> Placements => placements;

        /// <summary>
        /// 현재 방과 방향에 대응하는 몬스터 배치 정보를 찾습니다.
        /// </summary>
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

        /// <summary>
        /// 에디터 빌더나 런타임 기본 생성기가 만든 배치 목록으로 내용을 교체합니다.
        /// </summary>
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

        /// <summary>
        /// 스테이지의 모든 몬스터 허용 후보 방에 대해 비활성 기본 배치 카탈로그를 만듭니다.
        /// </summary>
        public static MonsterPlacementCatalog CreateDefault(StageDefinition stage)
        {
            var catalog = CreateInstance<MonsterPlacementCatalog>();
            catalog.SetPlacements(CreateDefaultEntries(stage));
            return catalog;
        }

        /// <summary>
        /// 스테이지 방/방향 구조를 기준으로 기본 몬스터 배치 항목을 열거합니다.
        /// </summary>
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

        /// <summary>
        /// 기존에 조정한 enabled/rect 값을 보존하면서 현재 스테이지 구조에 맞는 기본 항목을 병합합니다.
        /// </summary>
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

    /// <summary>
    /// 특정 방의 한 방향에서 몬스터 이미지를 표시할지와 표시 영역을 정의합니다.
    /// </summary>
    [Serializable]
    public sealed class MonsterPlacementEntry
    {
        public string roomId;
        public RoomFaceDirection faceDirection;
        public bool enabled;
        public Rect normalizedRect = Rect.zero;
    }
}
