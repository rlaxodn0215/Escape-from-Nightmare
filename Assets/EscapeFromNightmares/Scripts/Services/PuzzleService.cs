using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class PuzzleService
    {
        private readonly GameSession session;
        private readonly FlagService flags;

        public PuzzleService(GameSession session)
            : this(session, new FlagService(session))
        {
        }

        public PuzzleService(GameSession session, FlagService flags)
        {
            this.session = session;
            this.flags = flags;
        }

        public bool TrySolve(PuzzleDefinition puzzle, IReadOnlyList<string> inputTokens)
        {
            if (puzzle == null || inputTokens == null)
            {
                return false;
            }

            if (puzzle.oneShot && session.HasSolvedPuzzle(puzzle.puzzleId))
            {
                return false;
            }

            if (!flags.ConditionsMet(puzzle.conditions))
            {
                return false;
            }

            foreach (var requiredItemId in puzzle.requiredItemIds)
            {
                if (!session.HasItem(requiredItemId))
                {
                    return false;
                }
            }

            if (inputTokens.Count != puzzle.answerTokens.Length)
            {
                return false;
            }

            for (var index = 0; index < puzzle.answerTokens.Length; index++)
            {
                if (inputTokens[index] != puzzle.answerTokens[index])
                {
                    return false;
                }
            }

            if (puzzle.deferSolvedUntilRewardPickup)
            {
                session.SetFlag(puzzle.successFlag);
                session.SetFlag(puzzle.successEventId);
                return true;
            }

            foreach (var itemId in puzzle.rewardItemIds)
            {
                session.AddItem(itemId);
            }

            if (puzzle.consumeRequiredItems)
            {
                foreach (var itemId in puzzle.requiredItemIds)
                {
                    session.RemoveItem(itemId);
                }
            }

            session.MarkPuzzleSolved(puzzle.puzzleId);
            session.SetFlag(puzzle.successFlag);
            session.SetFlag(puzzle.successEventId);
            return true;
        }
    }
}
