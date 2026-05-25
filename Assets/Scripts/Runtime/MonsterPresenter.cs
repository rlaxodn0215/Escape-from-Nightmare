using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Presentation and QA rules for the monster shadow overlay.
    /// </summary>
    public static class MonsterPresenter
    {
        public static bool ShouldShowMonster(MonsterState state)
        {
            return state == MonsterState.Approaching
                || state == MonsterState.Searching
                || state == MonsterState.NearDetection
                || state == MonsterState.Chase;
        }

        public static MonsterState NextQaState(MonsterState state)
        {
            switch (state)
            {
                case MonsterState.Approaching:
                    return MonsterState.Searching;
                case MonsterState.Searching:
                    return MonsterState.NearDetection;
                case MonsterState.NearDetection:
                    return MonsterState.Chase;
                case MonsterState.Chase:
                    return MonsterState.Normal;
                default:
                    return MonsterState.Approaching;
            }
        }

        public static bool TryResolvePlacement(MonsterPlacementCatalog catalog, string roomId, RoomFaceDirection faceDirection, MonsterState state, out Rect normalizedRect)
        {
            normalizedRect = Rect.zero;
            if (!ShouldShowMonster(state) || catalog == null || string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }

            if (!catalog.TryFind(roomId, faceDirection, out var placement) || placement == null || !placement.enabled)
            {
                return false;
            }

            normalizedRect = placement.normalizedRect;
            return normalizedRect.width > 0f && normalizedRect.height > 0f;
        }

        public static GameDirector.MonsterQaSnapshot CreateQaSnapshot(MonsterPlacementCatalog catalog, string roomId, RoomFaceDirection faceDirection, MonsterState state, bool monsterImageActive)
        {
            var snapshot = new GameDirector.MonsterQaSnapshot
            {
                RoomId = string.IsNullOrWhiteSpace(roomId) ? "(none)" : roomId,
                FaceDirection = faceDirection,
                State = state,
                MonsterImageActive = monsterImageActive,
                Status = GameDirector.MonsterQaStatus.PlacementReady,
                StatusText = "placement ready"
            };

            if (!ShouldShowMonster(state))
            {
                snapshot.Status = GameDirector.MonsterQaStatus.StateHidden;
                snapshot.StatusText = "state hidden";
                return snapshot;
            }

            if (catalog == null)
            {
                snapshot.Status = GameDirector.MonsterQaStatus.CatalogMissing;
                snapshot.StatusText = "catalog missing";
                return snapshot;
            }

            if (!catalog.TryFind(roomId, faceDirection, out var placement) || placement == null)
            {
                snapshot.Status = GameDirector.MonsterQaStatus.PlacementMissing;
                snapshot.StatusText = "placement missing";
                return snapshot;
            }

            snapshot.HasPlacement = true;
            snapshot.PlacementEnabled = placement.enabled;
            snapshot.NormalizedRect = placement.normalizedRect;

            if (!placement.enabled)
            {
                snapshot.Status = GameDirector.MonsterQaStatus.PlacementDisabled;
                snapshot.StatusText = "placement disabled";
                return snapshot;
            }

            if (placement.normalizedRect.width <= 0f || placement.normalizedRect.height <= 0f)
            {
                snapshot.Status = GameDirector.MonsterQaStatus.PlacementEmpty;
                snapshot.StatusText = "placement empty";
                return snapshot;
            }

            snapshot.ShouldShowMonster = true;
            return snapshot;
        }

        public static void ApplyPlacement(RectTransform rectTransform, Rect normalizedRect)
        {
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = new Vector2(normalizedRect.xMin, normalizedRect.yMin);
            rectTransform.anchorMax = new Vector2(normalizedRect.xMax, normalizedRect.yMax);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
