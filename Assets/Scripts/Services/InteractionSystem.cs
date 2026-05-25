using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 단순 상호작용 판정 결과의 종류입니다.
    /// </summary>
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

    /// <summary>
    /// 상호작용 판정 결과와 그 결과가 참조하는 방/아이템/퍼즐/이벤트 ID를 담습니다.
    /// </summary>
    public readonly struct InteractionResult
    {
        public InteractionResult(InteractionResultType resultType, string value)
        {
            ResultType = resultType;
            Value = value;
        }

        /// <summary>상호작용 결과 종류입니다.</summary>
        public InteractionResultType ResultType { get; }
        /// <summary>결과 종류에 따라 해석되는 대상 ID입니다.</summary>
        public string Value { get; }
    }

    /// <summary>
    /// InteractableDefinition을 간단한 결과 타입으로 해석하는 초기 상호작용 판정기입니다.
    /// </summary>
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

        /// <summary>
        /// 상호작용 조건을 확인한 뒤 이동, 아이템 획득, 퍼즐 열기 같은 결과로 변환합니다.
        /// </summary>
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

    /// <summary>
    /// GameDirector가 실제로 실행할 수 있는 상호작용 액션 종류입니다.
    /// </summary>
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

    /// <summary>
    /// 하나의 런타임 액션과 액션 실행에 필요한 문자열/정수/사운드 데이터를 담습니다.
    /// </summary>
    public readonly struct EscapeAction
    {
        public EscapeAction(EscapeActionType type, string value, int intValue = 0, SoundEntry soundEntry = null)
        {
            Type = type;
            Value = value;
            IntValue = intValue;
            SoundEntry = soundEntry;
        }

        /// <summary>실행할 액션 종류입니다.</summary>
        public EscapeActionType Type { get; }
        /// <summary>방 ID, 아이템 ID, 플래그 ID처럼 액션이 참조하는 문자열 값입니다.</summary>
        public string Value { get; }
        /// <summary>회전 방향처럼 정수 값이 필요한 액션에 사용합니다.</summary>
        public int IntValue { get; }
        /// <summary>사운드 재생 액션에 연결된 사운드 카탈로그 항목입니다.</summary>
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

    /// <summary>
    /// 상호작용 해석의 성공 여부와 GameDirector가 순서대로 실행할 액션 목록입니다.
    /// </summary>
    public sealed class EscapeActionResult
    {
        private readonly List<EscapeAction> actions = new List<EscapeAction>();

        public EscapeActionResult(bool succeeded, string message = "")
        {
            Succeeded = succeeded;
            Message = message;
        }

        /// <summary>상호작용이나 퍼즐 해석이 성공했는지 여부입니다.</summary>
        public bool Succeeded { get; }
        /// <summary>실패 또는 디버그 로그에 사용할 메시지입니다.</summary>
        public string Message { get; }
        /// <summary>성공 시 실행할 액션 목록입니다.</summary>
        public IReadOnlyList<EscapeAction> Actions => actions;

        /// <summary>성공 결과를 생성합니다.</summary>
        public static EscapeActionResult Success(string message = "")
        {
            return new EscapeActionResult(true, message);
        }

        /// <summary>실패 결과를 생성합니다.</summary>
        public static EscapeActionResult Failure(string message)
        {
            return new EscapeActionResult(false, message);
        }

        /// <summary>None이 아닌 액션을 결과 목록에 추가하고 같은 결과 객체를 반환합니다.</summary>
        public EscapeActionResult Add(EscapeAction action)
        {
            if (action.Type != EscapeActionType.None)
            {
                actions.Add(action);
            }

            return this;
        }
    }

    /// <summary>
    /// 상호작용과 퍼즐 결과를 GameDirector가 실행할 액션 목록으로 변환합니다.
    /// </summary>
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

        /// <summary>
        /// 클릭된 상호작용의 조건, 사운드, 플래그, 결과 동작을 액션 목록으로 해석합니다.
        /// </summary>
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

            // 사운드와 플래그는 상호작용 종류와 무관하게 공통으로 먼저 누적합니다.
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

        /// <summary>방향 전환 버튼 입력을 회전 액션으로 변환합니다.</summary>
        public EscapeActionResult ResolveRotateFace(int offset)
        {
            return EscapeActionResult.Success().Add(EscapeAction.RotateFace(offset));
        }

        /// <summary>퍼즐 성공 시 필요한 사운드와 성공 플래그 액션을 생성합니다.</summary>
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

        /// <summary>퍼즐 실패 시 실패 사운드를 포함한 실패 결과를 생성합니다.</summary>
        public EscapeActionResult ResolvePuzzleFailure(PuzzleDefinition puzzle)
        {
            var result = EscapeActionResult.Failure("퍼즐 해결 실패.");
            if (puzzle != null)
            {
                AddSound(result, puzzle.failureSoundId);
            }

            return result;
        }

        /// <summary>스테이지 클리어 플래그와 완료 액션을 생성합니다.</summary>
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
