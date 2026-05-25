using UnityEngine;

namespace EscapeFromNightmares.Data
{
    /// <summary>
    /// Runtime entry point for stage data assets.
    /// </summary>
    [CreateAssetMenu(menuName = "Escape From Nightmares/Stage Catalog")]
    public sealed class StageCatalog : ScriptableObject
    {
        public StageDefinition stage1;

        public bool TryFind(string stageId, out StageDefinition stage)
        {
            if (stage1 != null && stage1.stageId == stageId)
            {
                stage = stage1;
                return true;
            }

            stage = null;
            return false;
        }
    }
}
