using System;
using EscapeFromNightmares.Services;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Executes resolved domain actions against session, UI, and audio boundaries.
    /// </summary>
    public sealed class EscapeActionExecutor
    {
        private readonly Action<string> openRoom;
        private readonly Action<int> rotateFace;
        private readonly Action<string> acquireItem;
        private readonly Action<string> openCloseUp;
        private readonly Action<string> openPuzzle;
        private readonly FlagService flags;
        private readonly Action<string> showClue;
        private readonly SoundManager soundManager;
        private readonly Action<string> enterHideSpot;
        private readonly GameSession session;
        private readonly Action completeStage;

        public EscapeActionExecutor(
            Action<string> openRoom,
            Action<int> rotateFace,
            Action<string> acquireItem,
            Action<string> openCloseUp,
            Action<string> openPuzzle,
            FlagService flags,
            Action<string> showClue,
            SoundManager soundManager,
            Action<string> enterHideSpot,
            GameSession session,
            Action completeStage)
        {
            this.openRoom = openRoom;
            this.rotateFace = rotateFace;
            this.acquireItem = acquireItem;
            this.openCloseUp = openCloseUp;
            this.openPuzzle = openPuzzle;
            this.flags = flags;
            this.showClue = showClue;
            this.soundManager = soundManager;
            this.enterHideSpot = enterHideSpot;
            this.session = session;
            this.completeStage = completeStage;
        }

        public void Execute(EscapeAction action)
        {
            switch (action.Type)
            {
                case EscapeActionType.MoveRoom:
                    openRoom?.Invoke(action.Value);
                    break;
                case EscapeActionType.RotateFace:
                    rotateFace?.Invoke(action.IntValue);
                    break;
                case EscapeActionType.AcquireItem:
                    acquireItem?.Invoke(action.Value);
                    break;
                case EscapeActionType.OpenCloseUp:
                    openCloseUp?.Invoke(action.Value);
                    break;
                case EscapeActionType.OpenPuzzle:
                    openPuzzle?.Invoke(action.Value);
                    break;
                case EscapeActionType.SetFlag:
                    flags?.Set(action.Value);
                    break;
                case EscapeActionType.ClearFlag:
                    flags?.Clear(action.Value);
                    break;
                case EscapeActionType.ShowClue:
                    showClue?.Invoke(action.Value);
                    break;
                case EscapeActionType.PlaySound:
                    soundManager?.Play(action.SoundEntry);
                    break;
                case EscapeActionType.EnterHideSpot:
                    enterHideSpot?.Invoke(action.Value);
                    break;
                case EscapeActionType.MarkPuzzleSolved:
                    session?.MarkPuzzleSolved(action.Value);
                    break;
                case EscapeActionType.CompleteStage:
                    completeStage?.Invoke();
                    break;
            }
        }
    }
}
