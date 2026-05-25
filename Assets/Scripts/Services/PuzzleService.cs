using System.Collections.Generic;
using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Services
{
    /// <summary>
    /// 퍼즐 조건과 입력 토큰을 검증하고 성공 시 보상, 플래그, 소모 처리를 적용합니다.
    /// </summary>
    public sealed class PuzzleService
    {
        private readonly GameSession session;
        private readonly FlagService flags;

        /// <summary>
        /// 별도 FlagService가 필요 없는 테스트나 단순 사용을 위해 기본 플래그 서비스를 구성합니다.
        /// </summary>
        public PuzzleService(GameSession session)
            : this(session, new FlagService(session))
        {
        }

        /// <summary>세션과 조건 판정 서비스를 주입받아 퍼즐 서비스를 구성합니다.</summary>
        public PuzzleService(GameSession session, FlagService flags)
        {
            this.session = session;
            this.flags = flags;
        }

        /// <summary>
        /// 입력 토큰이 퍼즐 정답과 일치하는지 확인하고, 성공 처리까지 한 번에 수행합니다.
        /// </summary>
        public bool TrySolve(PuzzleDefinition puzzle, IReadOnlyList<string> inputTokens)
        {
            // 퍼즐 데이터, 조건, 필수 아이템, 입력 길이를 먼저 걸러 실제 정답 비교를 단순하게 유지합니다.
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
                // 금고처럼 퍼즐 성공과 보상 획득 시점이 분리된 경우에는 플래그만 먼저 세웁니다.
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
