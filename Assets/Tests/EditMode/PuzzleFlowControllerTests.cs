using System.Collections.Generic;
using EscapeFromNightmares.Runtime;
using NUnit.Framework;

namespace EscapeFromNightmares.Tests.EditMode
{
    public sealed class PuzzleFlowControllerTests
    {
        [Test]
        public void AddToken_ReplacesLastToken_WhenInputIsFull()
        {
            var tokens = new List<string> { "A", "B" };
            var controller = new PuzzleFlowController(tokens);

            controller.AddToken("C", 2);

            Assert.That(tokens, Is.EqualTo(new[] { "A", "C" }));
            Assert.That(controller.InputLabel(), Is.EqualTo("Input: A C"));
        }

        [Test]
        public void Reset_ClearsInputAndShowsEmptyLabel()
        {
            var tokens = new List<string> { "north" };
            var controller = new PuzzleFlowController(tokens);

            controller.Reset();

            Assert.That(tokens, Is.Empty);
            Assert.That(controller.InputLabel(), Is.EqualTo("Input: -"));
        }
    }
}
