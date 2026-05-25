using EscapeFromNightmares.Data;
using UnityEngine;

namespace EscapeFromNightmares.Runtime
{
    /// <summary>
    /// Loads stage data from serialized references or the Resources stage catalog.
    /// </summary>
    public static class StageRepository
    {
        public const string StageCatalogResourcePath = "EscapeFromNightmares/Data/StageCatalog";
        public const string Stage1Id = "stage1";

        public static StageCatalog LoadCatalog()
        {
            return Resources.Load<StageCatalog>(StageCatalogResourcePath);
        }

        public static StageDefinition LoadStage1(StageDefinition serializedStage = null)
        {
            if (serializedStage != null)
            {
                return serializedStage;
            }

            var catalog = LoadCatalog();
            if (catalog != null && catalog.TryFind(Stage1Id, out var stage))
            {
                return stage;
            }

            Debug.LogError("Stage 1 data asset is missing. Rebuild stage data from Escape From Nightmares/Rebuild Stage 1 Data Assets.");
            return null;
        }
    }
}
