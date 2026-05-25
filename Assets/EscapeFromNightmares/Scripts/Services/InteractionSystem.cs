using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public enum InteractionResultType
    {
        None,
        MoveRoom,
        AcquireItem,
        OpenCloseUp,
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
        private readonly FlagService flags;

        public InteractionSystem(GameSession session)
            : this(session, new FlagService(session))
        {
        }

        public InteractionSystem(GameSession session, FlagService flags)
        {
            this.session = session;
            this.flags = flags;
        }

        public InteractionResult Resolve(InteractableDefinition interactable)
        {
            if (interactable == null
                || !session.HasItem(interactable.requiredItemId)
                || !flags.ConditionsMet(interactable.conditions)
                || (interactable.oneShot && session.HasUsedInteractable(interactable.interactableId)))
            {
                return new InteractionResult(InteractionResultType.None, string.Empty);
            }

            switch (interactable.type)
            {
                case InteractableType.Door:
                case InteractableType.LockedDoor:
                    return new InteractionResult(InteractionResultType.MoveRoom, interactable.targetRoomId);
                case InteractableType.ItemPickup:
                    if (!string.IsNullOrWhiteSpace(interactable.closeUpClosedResource))
                    {
                        return new InteractionResult(InteractionResultType.OpenCloseUp, interactable.interactableId);
                    }

                    return new InteractionResult(InteractionResultType.AcquireItem, interactable.grantsItemId);
                case InteractableType.PuzzleObject:
                case InteractableType.EscapeDoor:
                    return new InteractionResult(InteractionResultType.OpenPuzzle, interactable.puzzleId);
                case InteractableType.HideSpot:
                    return new InteractionResult(InteractionResultType.EnterHideSpot, interactable.interactableId);
                case InteractableType.ClueObject:
                    if (!string.IsNullOrWhiteSpace(interactable.clueViewResource))
                    {
                        return new InteractionResult(InteractionResultType.OpenCloseUp, interactable.interactableId);
                    }

                    return new InteractionResult(InteractionResultType.TriggerEvent, interactable.eventId);
                case InteractableType.EventTrigger:
                    return new InteractionResult(InteractionResultType.TriggerEvent, interactable.eventId);
                default:
                    return new InteractionResult(InteractionResultType.None, string.Empty);
            }
        }
    }

    public enum EscapeActionType
    {
        None,
        MoveRoom,
        RotateFace,
        AcquireItem,
        OpenCloseUp,
        OpenPuzzle,
        SetFlag,
        ClearFlag,
        ShowClue,
        PlaySound,
        EnterHideSpot,
        MarkPuzzleSolved,
        CompleteStage
    }

    public readonly struct EscapeAction
    {
        public EscapeAction(EscapeActionType type, string value, int intValue = 0, SoundEntry soundEntry = null)
        {
            Type = type;
            Value = value;
            IntValue = intValue;
            SoundEntry = soundEntry;
        }

        public EscapeActionType Type { get; }
        public string Value { get; }
        public int IntValue { get; }
        public SoundEntry SoundEntry { get; }

        public static EscapeAction MoveRoom(string roomId)
        {
            return new EscapeAction(EscapeActionType.MoveRoom, roomId);
        }

        public static EscapeAction RotateFace(int offset)
        {
            return new EscapeAction(EscapeActionType.RotateFace, string.Empty, offset);
        }

        public static EscapeAction AcquireItem(string itemId)
        {
            return new EscapeAction(EscapeActionType.AcquireItem, itemId);
        }

        public static EscapeAction OpenCloseUp(string interactableId)
        {
            return new EscapeAction(EscapeActionType.OpenCloseUp, interactableId);
        }

        public static EscapeAction OpenPuzzle(string puzzleId)
        {
            return new EscapeAction(EscapeActionType.OpenPuzzle, puzzleId);
        }

        public static EscapeAction SetFlag(string flagId)
        {
            return new EscapeAction(EscapeActionType.SetFlag, flagId);
        }

        public static EscapeAction ClearFlag(string flagId)
        {
            return new EscapeAction(EscapeActionType.ClearFlag, flagId);
        }

        public static EscapeAction ShowClue(string clueId)
        {
            return new EscapeAction(EscapeActionType.ShowClue, clueId);
        }

        public static EscapeAction PlaySound(SoundEntry soundEntry)
        {
            return new EscapeAction(EscapeActionType.PlaySound, soundEntry?.soundId ?? string.Empty, 0, soundEntry);
        }

        public static EscapeAction EnterHideSpot(string hideSpotId)
        {
            return new EscapeAction(EscapeActionType.EnterHideSpot, hideSpotId);
        }

        public static EscapeAction MarkPuzzleSolved(string puzzleId)
        {
            return new EscapeAction(EscapeActionType.MarkPuzzleSolved, puzzleId);
        }

        public static EscapeAction CompleteStage(string stageId)
        {
            return new EscapeAction(EscapeActionType.CompleteStage, stageId);
        }
    }

    public sealed class EscapeActionResult
    {
        private readonly List<EscapeAction> actions = new List<EscapeAction>();

        public EscapeActionResult(bool succeeded, string message = "")
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; }
        public string Message { get; }
        public IReadOnlyList<EscapeAction> Actions => actions;

        public static EscapeActionResult Success(string message = "")
        {
            return new EscapeActionResult(true, message);
        }

        public static EscapeActionResult Failure(string message)
        {
            return new EscapeActionResult(false, message);
        }

        public EscapeActionResult Add(EscapeAction action)
        {
            if (action.Type != EscapeActionType.None)
            {
                actions.Add(action);
            }

            return this;
        }
    }

    public sealed class EscapeActionResolver
    {
        private readonly GameSession session;
        private readonly FlagService flags;
        private readonly SoundCatalog soundCatalog;

        public EscapeActionResolver(GameSession session, FlagService flags, SoundCatalog soundCatalog)
        {
            this.session = session;
            this.flags = flags;
            this.soundCatalog = soundCatalog;
        }

        public EscapeActionResult ResolveInteractable(InteractableDefinition interactable)
        {
            if (interactable == null)
            {
                return EscapeActionResult.Failure("상호작용 대상이 없습니다.");
            }

            if (interactable.oneShot && session.HasUsedInteractable(interactable.interactableId))
            {
                return EscapeActionResult.Failure("이미 확인했습니다.");
            }

            if (!session.HasItem(interactable.requiredItemId) || !flags.ConditionsMet(interactable.conditions))
            {
                return EscapeActionResult.Failure("필요 조건을 만족하지 못했습니다.");
            }

            var result = EscapeActionResult.Success();
            AddSound(result, interactable.soundId);
            AddFlags(result, interactable.setFlagIds);
            if (!string.IsNullOrWhiteSpace(interactable.eventId))
            {
                result.Add(EscapeAction.SetFlag(interactable.eventId));
            }

            switch (interactable.type)
            {
                case InteractableType.Door:
                case InteractableType.LockedDoor:
                    return result.Add(EscapeAction.MoveRoom(interactable.targetRoomId));
                case InteractableType.ItemPickup:
                    if (!string.IsNullOrWhiteSpace(interactable.closeUpClosedResource))
                    {
                        return result.Add(EscapeAction.OpenCloseUp(interactable.interactableId));
                    }

                    result.Add(EscapeAction.AcquireItem(interactable.grantsItemId));
                    if (!string.IsNullOrWhiteSpace(interactable.solvesPuzzleId))
                    {
                        result.Add(EscapeAction.MarkPuzzleSolved(interactable.solvesPuzzleId));
                    }

                    return result;
                case InteractableType.PuzzleObject:
                case InteractableType.EscapeDoor:
                    return result.Add(EscapeAction.OpenPuzzle(interactable.puzzleId));
                case InteractableType.HideSpot:
                    return result.Add(EscapeAction.EnterHideSpot(interactable.interactableId));
                case InteractableType.ClueObject:
                    if (!string.IsNullOrWhiteSpace(interactable.clueViewResource))
                    {
                        return result.Add(EscapeAction.OpenCloseUp(interactable.interactableId));
                    }

                    if (!string.IsNullOrWhiteSpace(interactable.eventId))
                    {
                        result.Add(EscapeAction.ShowClue(interactable.eventId));
                    }

                    return result;
                case InteractableType.EventTrigger:
                    if (!string.IsNullOrWhiteSpace(interactable.eventId))
                    {
                        result.Add(EscapeAction.ShowClue(interactable.eventId));
                    }

                    return result;
                default:
                    return result;
            }
        }

        public EscapeActionResult ResolveRotateFace(int offset)
        {
            return EscapeActionResult.Success().Add(EscapeAction.RotateFace(offset));
        }

        public EscapeActionResult ResolvePuzzleSuccess(PuzzleDefinition puzzle)
        {
            if (puzzle == null)
            {
                return EscapeActionResult.Failure("퍼즐 데이터가 없습니다.");
            }

            var result = EscapeActionResult.Success();
            AddSound(result, puzzle.successSoundId);
            result.Add(EscapeAction.SetFlag(puzzle.successFlag));
            result.Add(EscapeAction.SetFlag(puzzle.successEventId));
            return result;
        }

        public EscapeActionResult ResolvePuzzleFailure(PuzzleDefinition puzzle)
        {
            var result = EscapeActionResult.Failure("퍼즐 해결 실패.");
            if (puzzle != null)
            {
                AddSound(result, puzzle.failureSoundId);
            }

            return result;
        }

        public EscapeActionResult ResolveCompleteStage(StageDefinition stage)
        {
            var result = EscapeActionResult.Success();
            AddSound(result, "sfx_confirm");
            return result
                .Add(EscapeAction.SetFlag(stage.clearFlag))
                .Add(EscapeAction.CompleteStage(stage.stageId));
        }

        private void AddFlags(EscapeActionResult result, string[] flagIds)
        {
            if (flagIds == null)
            {
                return;
            }

            foreach (var flagId in flagIds)
            {
                result.Add(EscapeAction.SetFlag(flagId));
            }
        }

        private void AddSound(EscapeActionResult result, string soundId)
        {
            if (string.IsNullOrWhiteSpace(soundId) || soundCatalog == null || !soundCatalog.TryFind(soundId, out var entry))
            {
                return;
            }

            result.Add(EscapeAction.PlaySound(entry));
        }
    }
}
