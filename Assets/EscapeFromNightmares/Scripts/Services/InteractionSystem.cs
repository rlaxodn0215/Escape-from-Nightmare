using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public enum InteractionResultType
    {
        None,
        MoveRoom,
        AcquireItem,
        OpenPuzzle,
        EnterHideSpot,
        TriggerEvent
    }

    public readonly struct InteractionResult
    {
        public InteractionResult(InteractionResultType resultType, string value)
        {
            ResultType = resultType;
            Value = value;
        }

        public InteractionResultType ResultType { get; }
        public string Value { get; }
    }

    public sealed class InteractionSystem
    {
        private readonly GameSession session;

        public InteractionSystem(GameSession session)
        {
            this.session = session;
        }

        public InteractionResult Resolve(InteractableDefinition interactable)
        {
            if (interactable == null || !session.HasItem(interactable.requiredItemId))
            {
                return new InteractionResult(InteractionResultType.None, string.Empty);
            }

            switch (interactable.type)
            {
                case InteractableType.Door:
                case InteractableType.LockedDoor:
                    return new InteractionResult(InteractionResultType.MoveRoom, interactable.targetRoomId);
                case InteractableType.ItemPickup:
                    return new InteractionResult(InteractionResultType.AcquireItem, interactable.grantsItemId);
                case InteractableType.PuzzleObject:
                case InteractableType.EscapeDoor:
                    return new InteractionResult(InteractionResultType.OpenPuzzle, interactable.puzzleId);
                case InteractableType.HideSpot:
                    return new InteractionResult(InteractionResultType.EnterHideSpot, interactable.interactableId);
                case InteractableType.ClueObject:
                case InteractableType.EventTrigger:
                    return new InteractionResult(InteractionResultType.TriggerEvent, interactable.eventId);
                default:
                    return new InteractionResult(InteractionResultType.None, string.Empty);
            }
        }
    }
}
