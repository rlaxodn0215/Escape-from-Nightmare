using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    public sealed class PuzzleService
    {
        private readonly GameSession session;

        public PuzzleService(GameSession session)
        {
            this.session = session;
        }

        public bool TrySolve(PuzzleDefinition puzzle, IReadOnlyList<string> inputTokens)
        {
            if (puzzle == null || inputTokens == null)
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

            foreach (var itemId in puzzle.rewardItemIds)
            {
                session.AddItem(itemId);
            }

            session.SetFlag(puzzle.successFlag);
            session.SetFlag(puzzle.successEventId);
            return true;
        }
    }
}
