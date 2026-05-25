using System.Linq;
using EscapeFromNightmares.Data;
using EscapeFromNightmares.Services;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Shared room presentation rules independent from the GameDirector lifecycle.
    /// </summary>
    public static class RoomPresenter
    {
        public static RoomFaceDirection NextFaceDirection(RoomDefinition room, RoomFaceDirection currentDirection, int offset)
        {
            if (room?.faces == null || room.faces.Length == 0)
            {
                return currentDirection;
            }

            var directions = room.faces
                .Select(face => face.direction)
                .Distinct()
                .OrderBy(direction => (int)direction)
                .ToArray();
            if (directions.Length == 0)
            {
                return currentDirection;
            }

            var currentIndex = System.Array.IndexOf(directions, currentDirection);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var nextIndex = (currentIndex + offset) % directions.Length;
            if (nextIndex < 0)
            {
                nextIndex += directions.Length;
            }

            return directions[nextIndex];
        }

        public static string ResolveRoomFaceBackgroundResource(RoomFaceDefinition face, FlagService flags)
        {
            if (face == null)
            {
                return string.Empty;
            }

            if (face.conditionalBackgrounds != null && flags != null)
            {
                foreach (var candidate in face.conditionalBackgrounds)
                {
                    if (candidate != null
                        && !string.IsNullOrWhiteSpace(candidate.backgroundResource)
                        && flags.ConditionsMet(candidate.conditions))
                    {
                        return candidate.backgroundResource;
                    }
                }
            }

            return face.backgroundResource;
        }

        public static bool ShouldRenderRoomHitbox(InteractableDefinition interactable, GameSession currentSession)
        {
            return ShouldRenderRoomHitbox(interactable, currentSession, currentSession != null ? new FlagService(currentSession) : null);
        }

        public static bool ShouldRenderRoomHitbox(InteractableDefinition interactable, GameSession currentSession, FlagService flags)
        {
            return interactable != null
                && (flags == null || flags.ConditionsMet(interactable.conditions))
                && (!interactable.disableRoomHitboxWhenUsed
                    || currentSession == null
                    || !currentSession.HasUsedInteractable(interactable.interactableId));
        }
    }
}
