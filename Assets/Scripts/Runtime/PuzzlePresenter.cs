using System.Collections.Generic;
using EscapeFromNightmares.Data;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeFromNightmares.Runtime
{
    public static class PuzzlePresenter
    {
        public static bool IsStudySafePuzzle(PuzzleDefinition puzzle, string studySafePuzzleId)
        {
            return puzzle != null && puzzle.puzzleId == studySafePuzzleId;
        }

        public static void ApplyHeader(PuzzleDefinition puzzle, Text titleText, Text inputText, Text logText, Image closeUpImage, System.Func<string, Sprite> resolveSprite)
        {
            if (puzzle == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = puzzle.displayName + " / " + puzzle.puzzleType;
            }

            if (inputText != null)
            {
                inputText.text = string.Empty;
            }

            if (logText != null)
            {
                logText.text = string.Empty;
            }

            if (closeUpImage != null)
            {
                closeUpImage.sprite = resolveSprite != null ? resolveSprite(puzzle.closeUpResource) : null;
                closeUpImage.color = Color.white;
                closeUpImage.preserveAspect = true;
            }
        }

        public static IEnumerable<string> TokenOptions(PuzzleDefinition puzzle)
        {
            if (puzzle == null)
            {
                yield break;
            }

            switch (puzzle.puzzleType)
            {
                case PuzzleType.NumberLock:
                    for (var index = 0; index <= 9; index++)
                    {
                        yield return index.ToString();
                    }
                    break;
                case PuzzleType.ColorSequence:
                    foreach (var token in new[] { "black", "white", "red", "gray", "blue", "green" })
                    {
                        yield return token;
                    }
                    break;
                case PuzzleType.SymbolSequence:
                    foreach (var token in new[] { "heart", "child_hand", "cracked_circle", "keyhole" })
                    {
                        yield return token;
                    }
                    break;
                case PuzzleType.SilentSequence:
                    foreach (var token in new[] { "doll", "train", "block", "bell" })
                    {
                        yield return token;
                    }
                    break;
                default:
                    foreach (var token in puzzle.answerTokens)
                    {
                        yield return token;
                    }
                    break;
            }
        }

        public static string FormatInput(IEnumerable<string> tokens)
        {
            return tokens == null ? string.Empty : string.Join(" ", tokens);
        }
    }
}
