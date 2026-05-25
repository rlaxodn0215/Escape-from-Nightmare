using EscapeFromNightmares.Data;
using EscapeFromNightmares.Runtime;
using NUnit.Framework;

namespace EscapeFromNightmares.Tests.EditMode
{
    internal static class StageTestData
    {
        public static StageDefinition LoadStage1()
        {
            var stage = StageRepository.LoadStage1();
            Assert.That(stage, Is.Not.Null, "Stage 1 ScriptableObject asset is missing. Rebuild Stage 1 data assets before running tests.");
            return stage;
        }
    }
}
