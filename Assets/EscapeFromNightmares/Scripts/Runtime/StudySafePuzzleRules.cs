using System.Collections.Generic;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Pure rules for the study safe special puzzle.
    /// </summary>
    public static class StudySafePuzzleRules
    {
        public const string PuzzleId = "study_safe";
        public const string UnlockedFlag = "study_safe_unlocked";
        public const string OpenedFlag = "study_safe_opened";
        public const string ClearFlag = "puzzle_study_safe_clear";
        public const string SuccessEventId = "event_study_safe_success";
        public const int DigitCount = 4;

        public static int NextDigitValue(int currentValue)
        {
            return (currentValue + 1) % 10;
        }

        public static string DigitResource(int digit)
        {
            return "EscapeFromNightmares/Puzzles/study_safe_digit_" + digit;
        }

        public static bool ShouldPreserveDigitLayout(bool hasSerializedReference, bool foundSceneObject)
        {
            return hasSerializedReference || foundSceneObject;
        }

        public static bool DigitsMatchAnswer(IReadOnlyList<int> digits, IReadOnlyList<string> answerTokens)
        {
            if (digits == null || answerTokens == null || digits.Count != answerTokens.Count)
            {
                return false;
            }

            for (var index = 0; index < digits.Count; index++)
            {
                if (digits[index].ToString() != answerTokens[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
