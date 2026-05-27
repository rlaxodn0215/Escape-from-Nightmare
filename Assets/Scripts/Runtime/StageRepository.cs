using EscapeFromNightmares.Data;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Provides runtime stage data without loading external data assets.
    /// </summary>
    public static class StageRepository
    {
        public const string Stage1Id = "stage1";

        public static StageDefinition LoadStage1(StageDefinition serializedStage = null)
        {
            return serializedStage ?? RuntimeStageFactory.CreateStage1();
        }
    }
}
